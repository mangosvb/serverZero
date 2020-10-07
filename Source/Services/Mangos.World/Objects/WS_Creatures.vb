'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.Data
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Mangos.Common
Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Chat
Imports Mangos.Common.Enums.Faction
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Group
Imports Mangos.Common.Enums.Misc
Imports Mangos.Common.Enums.Player
Imports Mangos.Common.Enums.Spell
Imports Mangos.Common.Enums.Unit
Imports Mangos.Common.Globals
Imports Mangos.World.AI
Imports Mangos.World.DataStores
Imports Mangos.World.Globals
Imports Mangos.World.Handlers
Imports Mangos.World.Loots
Imports Mangos.World.Maps
Imports Mangos.World.Objects.WS_Base
Imports Mangos.World.Player
Imports Mangos.World.Server
Imports Mangos.World.Spells

Namespace Objects

    Public Class WS_Creatures

#Region "WS.Cretures.Constants"

        Public Const SKILL_DETECTION_PER_LEVEL As Integer = 5

#End Region

#Region "WS.Creatures.TypeDef"
        'WARNING: Use only with _WorldServer.WORLD_CREATUREs()
        Public Class CreatureObject
            Inherits BaseUnit
            Implements IDisposable

            Public ReadOnly Property CreatureInfo() As CreatureInfo
                Get
                    Return _WorldServer.CREATURESDatabase(ID)
                End Get
            End Property

            Public ID As Integer = 0
            Public aiScript As WS_Creatures_AI.TBaseAI = Nothing
            Public SpawnX As Single = 0
            Public SpawnY As Single = 0
            Public SpawnZ As Single = 0
            Public SpawnO As Single = 0
            Public Faction As Short = 0
            Public SpawnRange As Single = 0
            Public MoveType As Byte = 0
            Public MoveFlags As Integer = 0
            Public cStandState As Byte = 0
            Public ExpireTimer As Timer = Nothing
            Public SpawnTime As Integer = 0
            Public SpeedMod As Single = 1.0F
            Public EquipmentID As Integer = 0
            Public WaypointID As Integer = 0
            Public GameEvent As Integer = 0

            Public SpellCasted As WS_Spells.CastSpellParameters = Nothing

            Public DestroyAtNoCombat As Boolean = False

            Public Flying As Boolean = False

            Public LastPercent As Integer = 100

            Public ReadOnly Property Name() As String
                Get
                    Return CreatureInfo.Name
                End Get
            End Property
            Public ReadOnly Property MaxDistance() As Single
                Get
                    Return 35.0F 'BoundingRadius * 35
                End Get
            End Property

            Public ReadOnly Property isAbleToWalkOnWater() As Boolean
                Get
                    'TODO: Fix family filter
                    Select Case CreatureInfo.CreatureFamily
                        Case 3, 10, 11, 12, 20, 21, 27
                            Return False
                        Case Else
                            Return True
                    End Select
                End Get
            End Property

            Public ReadOnly Property isAbleToWalkOnGround() As Boolean
                Get
                    'TODO: Fix family filter
                    Select Case CreatureInfo.CreatureFamily
                        Case 255
                            Return False
                        Case Else
                            Return True
                    End Select
                End Get
            End Property

            Public ReadOnly Property isCritter() As Boolean
                Get
                    Return (CreatureInfo.CreatureType = UNIT_TYPE.CRITTER)
                End Get
            End Property

            Public ReadOnly Property isGuard() As Boolean
                Get
                    Return (CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_GUARD) = NPCFlags.UNIT_NPC_FLAG_GUARD
                    'Select Case ID
                    '    Case 68, 197, 240, 466, 727, 853, 1423, 1496, 1642, 1652, 1736, 1738, 1741, 1743, 1744, 1745, 1746, 1756, 1965, 2041, 2714, 2721, 3083, 3084, 3210, 3211, 3212, 3213, 3214, 3215, 3220, 3221, 3222, 3223, 3224, 3296, 3297, 3469, 3502, 3571, 4262, 4624, 5595, 5624, 5952, 5953, 5597, 7980, 8017, 9460, 10676, 10682, 10881, 11190, 11822, 12160, 12996, 13839, 14304, 14375, 14377, 15184, 15371, 15442, 15616, 15940, 16096, 16221, 16222, 16733, 16864, 16921, 18038, 18103, 18948, 18949, 18971, 18986, 19541, 20484, 20485, 20672, 20674, 21976, 22494, 23636, 23721, 25992
                    '        Return True
                    '    Case Else
                    '        Return False
                    'End Select
                End Get
            End Property

            Public Overrides ReadOnly Property IsDead() As Boolean
                Get
                    If aiScript IsNot Nothing Then
                        Return (Life.Current = 0 OrElse aiScript.State = AIState.AI_DEAD OrElse aiScript.State = AIState.AI_RESPAWN)
                    Else
                        Return (Life.Current = 0)
                    End If
                End Get
            End Property

            Public ReadOnly Property Evade() As Boolean
                Get
                    If aiScript IsNot Nothing AndAlso aiScript.State = AIState.AI_MOVING_TO_SPAWN Then
                        Return True
                    Else
                        Return False
                    End If
                End Get
            End Property

            Public ReadOnly Property NPCTextID() As Integer
                Get
                    If _WS_DBCDatabase.CreatureGossip.ContainsKey(GUID - _Global_Constants.GUID_UNIT) Then Return _WS_DBCDatabase.CreatureGossip(GUID - _Global_Constants.GUID_UNIT)
                    Return &HFFFFFF
                End Get
            End Property

            Public Overrides Function IsFriendlyTo(ByRef Unit As BaseUnit) As Boolean
                If Unit Is Me Then Return True

                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    With CType(Unit, WS_PlayerData.CharacterObject)
                        If .GM Then Return True
                        If .GetReputation(.Faction) < ReputationRank.Friendly Then Return False
                        If .GetReaction(.Faction) < TReaction.NEUTRAL Then Return False

                        'TODO: At war with faction?
                    End With
                ElseIf TypeOf Unit Is CreatureObject Then
                    With CType(Unit, CreatureObject)
                        'TODO!!
                    End With
                End If

                Return True
            End Function

            Public Overrides Function IsEnemyTo(ByRef Unit As BaseUnit) As Boolean
                If Unit Is Me Then Return False

                If TypeOf Unit Is WS_PlayerData.CharacterObject Then
                    With CType(Unit, WS_PlayerData.CharacterObject)
                        If .GM Then Return False
                        If .GetReputation(.Faction) < ReputationRank.Friendly Then Return True
                        If .GetReaction(.Faction) < TReaction.NEUTRAL Then Return True

                        'TODO: At war with faction?
                    End With
                ElseIf TypeOf Unit Is CreatureObject Then
                    With CType(Unit, CreatureObject)
                        'TODO!!
                    End With
                End If

                Return False
            End Function

            Public Function AggroRange(ByVal objCharacter As WS_PlayerData.CharacterObject) As Single
                Dim LevelDiff As Short = Level - CShort(objCharacter.Level)
                Dim Range As Single = 20 + LevelDiff
                If Range < 5 Then Range = 5
                If Range > 45 Then Range = 45
                Return Range
            End Function

            Public Sub SendTargetUpdate(ByVal TargetGUID As ULong)
                Dim packet As New Packets.UpdatePacketClass
                Dim tmpUpdate As New Packets.UpdateClass(EUnitFields.UNIT_END)
                tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_TARGET, TargetGUID)
                tmpUpdate.AddToPacket((packet), ObjectUpdateType.UPDATETYPE_VALUES, Me)
                tmpUpdate.Dispose()

                SendToNearPlayers((packet))
                packet.Dispose()
            End Sub
            Public Function GetRandomTarget() As BaseUnit
                If aiScript Is Nothing Then Return Nothing
                If aiScript.aiHateTable.Count = 0 Then Return Nothing
                Dim i As Integer = 0
                Dim target As Integer = _WorldServer.Rnd.Next(0, aiScript.aiHateTable.Count)
                For Each tmpUnit As KeyValuePair(Of BaseUnit, Integer) In aiScript.aiHateTable
                    If target = i Then Return tmpUnit.Key
                    i += 1
                Next
                Return Nothing
            End Function

            Public Sub FillAllUpdateFlags(ByRef Update As Packets.UpdateClass)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, Size)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, ObjectType.TYPE_OBJECT + ObjectType.TYPE_UNIT)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, ID)

                If (Not aiScript Is Nothing) AndAlso (Not aiScript.aiTarget Is Nothing) Then
                    Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_TARGET, aiScript.aiTarget.GUID)
                End If

                If SummonedBy > 0 Then Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_SUMMONEDBY, SummonedBy)
                If CreatedBy > 0 Then Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_CREATEDBY, CreatedBy)
                If CreatedBySpell > 0 Then Update.SetUpdateFlag(EUnitFields.UNIT_CREATED_BY_SPELL, CreatedBySpell)

                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, Model)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_NATIVEDISPLAYID, _WorldServer.CREATURESDatabase(ID).GetFirstModel)
                If Mount > 0 Then Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, Mount)

                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_0, cBytes0)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, cBytes1)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_2, cBytes2)

                Update.SetUpdateFlag(EUnitFields.UNIT_NPC_EMOTESTATE, cEmoteState)

                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + _WorldServer.CREATURESDatabase(ID).ManaType, Mana.Current)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXHEALTH, Life.Maximum)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXPOWER1 + _WorldServer.CREATURESDatabase(ID).ManaType, Mana.Maximum)

                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_LEVEL, Level)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_FACTIONTEMPLATE, Faction)
                Update.SetUpdateFlag(EUnitFields.UNIT_NPC_FLAGS, _WorldServer.CREATURESDatabase(ID).cNpcFlags)

                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)

                Update.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, cDynamicFlags)

                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_PHYSICAL, _WorldServer.CREATURESDatabase(ID).Resistances(DamageTypes.DMG_PHYSICAL))
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_HOLY, _WorldServer.CREATURESDatabase(ID).Resistances(DamageTypes.DMG_HOLY))
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_FIRE, _WorldServer.CREATURESDatabase(ID).Resistances(DamageTypes.DMG_FIRE))
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_NATURE, _WorldServer.CREATURESDatabase(ID).Resistances(DamageTypes.DMG_NATURE))
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_FROST, _WorldServer.CREATURESDatabase(ID).Resistances(DamageTypes.DMG_FROST))
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_SHADOW, _WorldServer.CREATURESDatabase(ID).Resistances(DamageTypes.DMG_SHADOW))
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RESISTANCES + DamageTypes.DMG_ARCANE, _WorldServer.CREATURESDatabase(ID).Resistances(DamageTypes.DMG_ARCANE))

                If EquipmentID > 0 Then
                    Dim EquipmentInfo As WS_DBCDatabase.CreatureEquipInfo = _WS_DBCDatabase.CreatureEquip(EquipmentID)
                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY, EquipmentInfo.EquipModel(0))
                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO, EquipmentInfo.EquipInfo(0))
                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + 1, EquipmentInfo.EquipSlot(0))

                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY + 1, EquipmentInfo.EquipModel(1))
                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + 2, EquipmentInfo.EquipInfo(1))
                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + 2 + 1, EquipmentInfo.EquipSlot(1))

                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY + 2, EquipmentInfo.EquipModel(2))
                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + 4, EquipmentInfo.EquipInfo(2))
                    Update.SetUpdateFlag(EUnitFields.UNIT_VIRTUAL_ITEM_INFO + 4 + 1, EquipmentInfo.EquipSlot(2))
                End If

                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BASEATTACKTIME, _WorldServer.CREATURESDatabase(ID).BaseAttackTime)
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_OFFHANDATTACKTIME, _WorldServer.CREATURESDatabase(ID).BaseAttackTime)
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGEDATTACKTIME, _WorldServer.CREATURESDatabase(ID).BaseRangedAttackTime)
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_ATTACK_POWER, _WorldServer.CREATURESDatabase(ID).AtackPower)
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_RANGED_ATTACK_POWER, _WorldServer.CREATURESDatabase(ID).RangedAtackPower)

                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_BOUNDINGRADIUS, BoundingRadius)
                Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_COMBATREACH, CombatReach)
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MINRANGEDDAMAGE, _WorldServer.CREATURESDatabase(ID).RangedDamage.Minimum)
                'Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_MAXRANGEDDAMAGE, _WorldServer.CREATURESDatabase(ID).RangedDamage.Maximum)

                For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECTs_VISIBLE - 1
                    If ActiveSpells(i) IsNot Nothing Then
                        Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURA + i, ActiveSpells(i).SpellID)
                    End If
                Next
                For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECT_FLAGs - 1
                    Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAFLAGS + i, ActiveSpells_Flags(i))
                Next
                For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECT_LEVELSs - 1
                    Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAAPPLICATIONS + i, ActiveSpells_Count(i))
                    Update.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURALEVELS + i, ActiveSpells_Level(i))
                Next
            End Sub

            Public Sub MoveToInstant(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal o As Single)
                positionX = x
                positionY = y
                positionZ = z
                orientation = o

                If SeenBy.Count > 0 Then
                    Dim packet As New Packets.PacketClass(OPCODES.MSG_MOVE_HEARTBEAT)
                    packet.AddPackGUID(GUID)
                    packet.AddInt32(0) 'Movementflags
                    packet.AddInt32(_NativeMethods.timeGetTime(""))
                    packet.AddSingle(positionX)
                    packet.AddSingle(positionY)
                    packet.AddSingle(positionZ)
                    packet.AddSingle(orientation)
                    packet.AddInt32(0)

                    SendToNearPlayers(packet)

                    packet.Dispose()
                End If
            End Sub
            Public OldX As Single = 0.0F
            Public OldY As Single = 0.0F
            Public OldZ As Single = 0.0F
            Public MoveX As Single = 0.0F
            Public MoveY As Single = 0.0F
            Public MoveZ As Single = 0.0F
            Public LastMove As Integer = 0
            Public LastMove_Time As Integer = 0
            Public PositionUpdated As Boolean = True
            Public Sub SetToRealPosition(Optional ByVal Forced As Boolean = False)
                If aiScript Is Nothing Then Exit Sub
                If Forced = False AndAlso aiScript.State = AIState.AI_MOVING_TO_SPAWN Then Exit Sub

                Dim timeDiff As Integer = _NativeMethods.timeGetTime("") - LastMove
                If (Forced OrElse aiScript.IsMoving) AndAlso LastMove > 0 AndAlso timeDiff < LastMove_Time Then
                    Dim distance As Single

                    If aiScript.State = AIState.AI_MOVING OrElse aiScript.State = AIState.AI_WANDERING Then
                        distance = timeDiff / 1000.0F * (CreatureInfo.WalkSpeed * SpeedMod)
                    Else
                        distance = timeDiff / 1000.0F * (CreatureInfo.RunSpeed * SpeedMod)
                    End If

                    positionX = OldX + Math.Cos(orientation) * distance
                    positionY = OldY + Math.Sin(orientation) * distance
                    positionZ = _WS_Maps.GetZCoord(positionX, positionY, positionZ, MapID)
                ElseIf PositionUpdated = False AndAlso timeDiff >= LastMove_Time Then
                    PositionUpdated = True
                    positionX = MoveX
                    positionY = MoveY
                    positionZ = MoveZ
                End If
            End Sub

            Public Sub StopMoving()
                If aiScript Is Nothing Then Exit Sub
                If aiScript.InCombat Then Exit Sub

                aiScript.Pause(10000)
                SetToRealPosition(True)
                MoveToInstant(positionX, positionY, positionZ, orientation)
            End Sub

            Public Function MoveTo(ByVal x As Single, ByVal y As Single, ByVal z As Single, Optional ByVal o As Single = 0.0F, Optional ByVal Running As Boolean = False) As Integer
                Try
                    If SeenBy.Count = 0 Then
                        Return 10000
                    End If
                Catch
                    _WorldServer.Log.WriteLine(LogType.WARNING, "MoveTo:SeenBy Failed")
                End Try

                Dim TimeToMove As Integer = 1

                Dim SMSG_MONSTER_MOVE As New Packets.PacketClass(OPCODES.SMSG_MONSTER_MOVE)
                Try
                    SMSG_MONSTER_MOVE.AddPackGUID(GUID)
                    SMSG_MONSTER_MOVE.AddSingle(positionX)
                    SMSG_MONSTER_MOVE.AddSingle(positionY)
                    SMSG_MONSTER_MOVE.AddSingle(positionZ)
                    SMSG_MONSTER_MOVE.AddInt32(_WS_Network.MsTime)         'Sequence/MSTime?

                    If o = 0.0F Then
                        SMSG_MONSTER_MOVE.AddInt8(0)                    'Type [If type is 1 then the packet ends here]
                    Else
                        SMSG_MONSTER_MOVE.AddInt8(4)
                        SMSG_MONSTER_MOVE.AddSingle(o)
                    End If

                    Dim moveDist As Single = _WS_Combat.GetDistance(positionX, x, positionY, y, positionZ, z)
                    If Flying Then
                        SMSG_MONSTER_MOVE.AddInt32(&H300)           'Flags [0x0 - Walk, 0x100 - Run, 0x200 - Waypoint, 0x300 - Fly]
                        TimeToMove = moveDist / (CreatureInfo.RunSpeed * SpeedMod) * 1000 + 0.5F
                    Else
                        If Running Then
                            SMSG_MONSTER_MOVE.AddInt32(&H100)           'Flags [0x0 - Walk, 0x100 - Run, 0x200 - Waypoint, 0x300 - Fly]
                            TimeToMove = moveDist / (CreatureInfo.RunSpeed * SpeedMod) * 1000 + 0.5F
                        Else
                            SMSG_MONSTER_MOVE.AddInt32(0)
                            TimeToMove = moveDist / (CreatureInfo.WalkSpeed * SpeedMod) * 1000 + 0.5F
                        End If
                    End If

                    orientation = _WS_Combat.GetOrientation(positionX, x, positionY, y)
                    OldX = positionX
                    OldY = positionY
                    OldZ = positionZ
                    LastMove = _NativeMethods.timeGetTime("")
                    LastMove_Time = TimeToMove
                    PositionUpdated = False
                    positionX = x
                    positionY = y
                    positionZ = z
                    MoveX = x
                    MoveY = y
                    MoveZ = z

                    SMSG_MONSTER_MOVE.AddInt32(TimeToMove)  'Time
                    SMSG_MONSTER_MOVE.AddInt32(1)           'Points Count
                    SMSG_MONSTER_MOVE.AddSingle(x)          'First Point X
                    SMSG_MONSTER_MOVE.AddSingle(y)          'First Point Y
                    SMSG_MONSTER_MOVE.AddSingle(z)          'First Point Z

                    'The points after that are in the same format only if flag 0x200 is set, else they are compressed in 1 uint32

                    SendToNearPlayers(SMSG_MONSTER_MOVE)
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "MoveTo:Main Failed - {0}", ex.Message)
                Finally
                    SMSG_MONSTER_MOVE.Dispose()
                End Try

                MoveCell()
                Return TimeToMove
            End Function

            Public Function CanMoveTo(ByVal x As Single, ByVal y As Single, ByVal z As Single) As Boolean
                If _WS_Maps.IsOutsideOfMap(Me) Then Return False

                If z < _WS_Maps.GetWaterLevel(x, y, MapID) Then
                    If Not isAbleToWalkOnWater Then Return False
                Else
                    If Not isAbleToWalkOnGround Then Return False
                End If

                Return True
            End Function

            Public Sub TurnTo(ByRef Target As BaseObject)
                TurnTo(Target.positionX, Target.positionY)
            End Sub

            Public Sub TurnTo(ByVal x As Single, ByVal y As Single)
                orientation = _WS_Combat.GetOrientation(positionX, x, positionY, y)
                TurnTo(orientation)
            End Sub

            Public Sub TurnTo(ByVal orientation_ As Single)
                orientation = orientation_

                If SeenBy.Count > 0 Then
                    If aiScript Is Nothing OrElse aiScript.IsMoving() = False Then
                        Dim packet As New Packets.PacketClass(OPCODES.MSG_MOVE_HEARTBEAT)
                        Try
                            packet.AddPackGUID(GUID)
                            packet.AddInt32(0) 'Movementflags
                            packet.AddInt32(_NativeMethods.timeGetTime(""))
                            packet.AddSingle(positionX)
                            packet.AddSingle(positionY)
                            packet.AddSingle(positionZ)
                            packet.AddSingle(orientation)
                            packet.AddInt32(0)

                            SendToNearPlayers(packet)
                        Finally
                            packet.Dispose()
                        End Try
                    End If
                End If
            End Sub

            Public Overrides Sub Die(ByRef Attacker As BaseUnit)
                cUnitFlags = UnitFlags.UNIT_FLAG_DEAD 'cUnitFlags Or UnitFlags.UNIT_FLAG_DEAD
                Life.Current = 0
                Mana.Current = 0

                'DONE: Creature stops while it's dead and everyone sees it at the same position
                If aiScript IsNot Nothing Then
                    SetToRealPosition(True)
                    MoveToInstant(positionX, positionY, positionZ, orientation)
                    PositionUpdated = True
                    LastMove = 0
                    LastMove_Time = 0

                    aiScript.State = AIState.AI_DEAD
                    aiScript.DoThink()
                End If

                If aiScript IsNot Nothing Then aiScript.OnDeath()

                If Attacker IsNot Nothing AndAlso TypeOf Attacker Is CreatureObject Then
                    If CType(Attacker, CreatureObject).aiScript IsNot Nothing Then CType(Attacker, CreatureObject).aiScript.OnKill(Me)
                End If

                'DONE: Send the update
                Dim packetForNear As New Packets.UpdatePacketClass
                Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)

                'DONE: Remove all spells when the creature die
                For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECTs_VISIBLE - 1
                    If ActiveSpells(i) IsNot Nothing Then
                        RemoveAura(i, ActiveSpells(i).SpellCaster, , False)
                        UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURA + i, 0)
                    End If
                Next

                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + MyBase.ManaType, Mana.Current)
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                UpdateData.AddToPacket((packetForNear), ObjectUpdateType.UPDATETYPE_VALUES, Me)

                SendToNearPlayers(packetForNear)
                packetForNear.Dispose()
                UpdateData.Dispose()

                If TypeOf Attacker Is WS_PlayerData.CharacterObject Then
                    CType(Attacker, WS_PlayerData.CharacterObject).RemoveFromCombat(Me)

                    'DONE: Don't give xp or loot for guards, civilians or critters
                    If isCritter = False AndAlso isGuard = False AndAlso CreatureInfo.cNpcFlags = 0 Then
                        GiveXP(CType(Attacker, WS_PlayerData.CharacterObject))
                        LootCorpse(CType(Attacker, WS_PlayerData.CharacterObject))
                    End If

                    'DONE: Fire quest event to check for if this monster is required for quest
                    _WorldServer.ALLQUESTS.OnQuestKill(Attacker, Me)
                End If
            End Sub

            Public Overrides Sub DealDamage(ByVal Damage As Integer, Optional ByRef Attacker As BaseUnit = Nothing)
                If Life.Current = 0 Then Exit Sub

                'DONE: Break some spells when taking any damage
                RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_DAMAGE)

                Life.Current -= Damage

                'DONE: Generate hate
                If Attacker IsNot Nothing AndAlso aiScript IsNot Nothing Then aiScript.OnGenerateHate(Attacker, Damage)

                'DONE: Check for dead
                If Life.Current = 0 Then
                    Die(Attacker)
                    Exit Sub
                End If

                Dim tmpPercent As Integer = Fix((Life.Current / Life.Maximum) * 100)
                If tmpPercent <> LastPercent Then
                    LastPercent = tmpPercent
                    If aiScript IsNot Nothing Then aiScript.OnHealthChange(LastPercent)
                End If

                'DONE: Do health update
                If SeenBy.Count > 0 Then
                    Dim packetForNear As New Packets.UpdatePacketClass
                    Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + MyBase.ManaType, Mana.Current)
                    UpdateData.AddToPacket((packetForNear), ObjectUpdateType.UPDATETYPE_VALUES, Me)

                    SendToNearPlayers((packetForNear))
                    packetForNear.Dispose()
                    UpdateData.Dispose()
                End If
            End Sub

            Public Overrides Sub Heal(ByVal Damage As Integer, Optional ByRef Attacker As BaseUnit = Nothing)
                If Life.Current = 0 Then Exit Sub

                Life.Current += Damage

                'DONE: Do health update
                If SeenBy.Count > 0 Then
                    Dim packetForNear As New Packets.UpdatePacketClass
                    Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
                    UpdateData.AddToPacket((packetForNear), ObjectUpdateType.UPDATETYPE_VALUES, Me)

                    SendToNearPlayers((packetForNear))
                    packetForNear.Dispose()
                    UpdateData.Dispose()
                End If
            End Sub

            Public Overrides Sub Energize(ByVal Damage As Integer, ByVal Power As ManaTypes, Optional ByRef Attacker As BaseUnit = Nothing)
                If ManaType <> Power Then Exit Sub

                Mana.Current += Damage

                'DONE: Do health update
                If SeenBy.Count > 0 Then
                    Dim packetForNear As New Packets.UpdatePacketClass
                    Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + MyBase.ManaType, Mana.Current)
                    UpdateData.AddToPacket((packetForNear), ObjectUpdateType.UPDATETYPE_VALUES, Me)

                    SendToNearPlayers((packetForNear))
                    packetForNear.Dispose()
                    UpdateData.Dispose()
                End If
            End Sub

            Public Sub LootCorpse(ByRef Character As WS_PlayerData.CharacterObject)
                If GenerateLoot(Character, LootType.LOOTTYPE_CORPSE) Then
                    cDynamicFlags = DynamicFlags.UNIT_DYNFLAG_LOOTABLE
                ElseIf CreatureInfo.SkinLootID > 0 Then
                    cUnitFlags = cUnitFlags Or UnitFlags.UNIT_FLAG_SKINNABLE
                Else
                    'No loot or skinnable
                    Exit Sub
                End If

                'DONE: Create packet
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                packet.AddInt32(1)
                packet.AddInt8(0)
                Dim UpdateData As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, cDynamicFlags)
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
                UpdateData.Dispose()

                If _WS_Loot.LootTable.ContainsKey(GUID) = False AndAlso (cUnitFlags And UnitFlags.UNIT_FLAG_SKINNABLE) = UnitFlags.UNIT_FLAG_SKINNABLE Then
                    'DONE: There was no loot, so send the skinning update to every nearby player
                    SendToNearPlayers(packet)
                Else
                    If Character.IsInGroup Then
                        'DONE: Group loot rules
                        _WS_Loot.LootTable(GUID).LootOwner = 0

                        Select Case Character.Group.LootMethod
                            Case GroupLootMethod.LOOT_FREE_FOR_ALL
                                For Each objCharacter As ULong In Character.Group.LocalMembers
                                    If SeenBy.Contains(objCharacter) Then
                                        _WS_Loot.LootTable(GUID).LootOwner = objCharacter
                                        _WorldServer.CHARACTERs(objCharacter).client.Send(packet)
                                    End If
                                Next

                            Case GroupLootMethod.LOOT_MASTER
                                If Character.Group.LocalLootMaster Is Nothing Then
                                    _WS_Loot.LootTable(GUID).LootOwner = Character.GUID
                                    Character.client.Send(packet)
                                Else
                                    _WS_Loot.LootTable(GUID).LootOwner = Character.Group.LocalLootMaster.GUID
                                    Character.Group.LocalLootMaster.client.Send(packet)
                                End If

                            Case GroupLootMethod.LOOT_GROUP, GroupLootMethod.LOOT_NEED_BEFORE_GREED, GroupLootMethod.LOOT_ROUND_ROBIN
                                Dim cLooter As WS_PlayerData.CharacterObject = Character.Group.GetNextLooter()
                                While Not SeenBy.Contains(cLooter.GUID) AndAlso (Not cLooter Is Character)
                                    cLooter = Character.Group.GetNextLooter()
                                End While

                                _WS_Loot.LootTable(GUID).LootOwner = cLooter.GUID
                                cLooter.client.Send(packet)
                        End Select
                    Else
                        _WS_Loot.LootTable(GUID).LootOwner = Character.GUID
                        Character.client.Send(packet)
                    End If
                End If

                'DONE: Dispose packet
                packet.Dispose()
            End Sub

            Public Function GenerateLoot(ByRef Character As WS_PlayerData.CharacterObject, ByVal LootType As LootType) As Boolean
                If CreatureInfo.LootID = 0 Then Return False

                'DONE: Loot generation
                Dim Loot As New WS_Loot.LootObject(GUID, LootType)
                Dim Template As WS_Loot.LootTemplate = _WS_Loot.LootTemplates_Creature.GetLoot(CreatureInfo.LootID)
                If Template IsNot Nothing Then
                    Template.Process(Loot, 0)
                End If

                'DONE: Money loot
                If LootType = LootType.LOOTTYPE_CORPSE AndAlso CreatureInfo.CreatureType = UNIT_TYPE.HUMANOID Then
                    Loot.Money = _WorldServer.Rnd.Next(CreatureInfo.MinGold, CreatureInfo.MaxGold + 1)
                End If

                Loot.LootOwner = Character.GUID

                Return True
            End Function

            Public Sub GiveXP(ByRef Character As WS_PlayerData.CharacterObject)
                'NOTE: Formulas taken from http://www.wowwiki.com/Formulas:Mob_XP
                Dim XP As Integer = Level * 5 + 45

                Dim lvlDifference As Integer = Character.Level - CInt(Level)

                If lvlDifference > 0 Then 'Higher level mobs
                    XP = XP * (1 + 0.05 * (Level - CInt(Character.Level)))
                ElseIf lvlDifference < 0 Then 'Lower level mobs
                    Dim GrayLevel As Byte = 0
                    Select Case Character.Level
                        Case Is <= 5 : GrayLevel = 0
                        Case Is <= 39 : GrayLevel = Character.Level - Math.Floor(Character.Level / 10) - 5
                        Case Is <= 59 : GrayLevel = Character.Level - Math.Floor(Character.Level / 5) - 1
                        Case Else : GrayLevel = Character.Level - 9
                    End Select

                    If Level > GrayLevel Then
                        Dim ZD As Integer = 0
                        Select Case Character.Level
                            Case Is <= 7 : ZD = 5
                            Case Is <= 9 : ZD = 6
                            Case Is <= 11 : ZD = 7
                            Case Is <= 15 : ZD = 8
                            Case Is <= 19 : ZD = 9
                            Case Is <= 29 : ZD = 11
                            Case Is <= 39 : ZD = 12
                            Case Is <= 44 : ZD = 13
                            Case Is <= 49 : ZD = 14
                            Case Is <= 54 : ZD = 15
                            Case Is <= 59 : ZD = 16
                            Case Else : ZD = 17
                        End Select

                        XP = XP * (1 - (Character.Level - CInt(Level)) / ZD)
                    Else
                        XP = 0
                    End If
                End If

                'DONE: Killing elites
                If _WorldServer.CREATURESDatabase(ID).Elite > 0 Then XP *= 2
                'DONE: XP Rate config
                XP *= _WorldServer.Config.XPRate

                If Not Character.IsInGroup Then
                    'DONE: Rested
                    Dim RestedXP As Integer = 0
                    If Character.RestBonus >= 0 Then
                        RestedXP = XP
                        If RestedXP > Character.RestBonus Then RestedXP = Character.RestBonus
                        Character.RestBonus -= RestedXP
                        XP += RestedXP
                    End If

                    'DONE: Single kill
                    Character.AddXP(XP, RestedXP, GUID, True)
                Else

                    'DONE: Party bonus
                    XP /= Character.Group.GetMembersCount()

                    Select Case Character.Group.GetMembersCount()
                        Case Is <= 2 : XP *= 1
                        Case 3 : XP *= 1.166
                        Case 4 : XP *= 1.3
                        Case Else : XP *= 1.4
                    End Select

                    'DONE: Party calculate all levels
                    Dim baseLvl As Integer = 0
                    For Each Member As ULong In Character.Group.LocalMembers
                        With _WorldServer.CHARACTERs(Member)
                            If .DEAD = False AndAlso (Math.Sqrt((Me.positionX - positionX) ^ 2 + (Me.positionY - positionY) ^ 2) <= Me.VisibleDistance) Then
                                baseLvl += Me.Level
                            End If
                        End With
                    Next

                    'DONE: Party share
                    For Each Member As ULong In Character.Group.LocalMembers
                        With _WorldServer.CHARACTERs(Member)
                            If .DEAD = False AndAlso (Math.Sqrt((Me.positionX - positionX) ^ 2 + (Me.positionY - positionY) ^ 2) <= Me.VisibleDistance) Then
                                Dim tmpXP As Integer = XP
                                'DONE: Rested
                                Dim RestedXP As Integer = 0
                                If .RestBonus >= 0 Then
                                    RestedXP = tmpXP
                                    If RestedXP > .RestBonus Then RestedXP = .RestBonus
                                    .RestBonus -= RestedXP
                                    tmpXP += RestedXP
                                End If

                                tmpXP = Fix(tmpXP * Me.Level / baseLvl)
                                Call .AddXP(tmpXP, RestedXP, GUID, False)
                                .LogXPGain(tmpXP, RestedXP, GUID, (Character.Group.GetMembersCount() - 1) / 10)
                            End If
                        End With
                    Next

                End If
            End Sub

            Public Sub StopCasting()
                If SpellCasted Is Nothing OrElse SpellCasted.Finished Then Exit Sub
                SpellCasted.StopCast()

                'TODO: Send interrupt to other players
            End Sub

            Public Sub ApplySpell(ByVal SpellID As Integer)
                'TODO: Check if the creature can cast the spell

                If _WS_Spells.SPELLs.ContainsKey(SpellID) = False Then Exit Sub
                Dim t As New WS_Spells.SpellTargets
                t.SetTarget_SELF(Me)
                _WS_Spells.SPELLs(SpellID).Apply(Me, t)
            End Sub

            Public Function CastSpellOnSelf(ByVal SpellID As Integer) As Integer
                If Spell_Silenced Then Return -1

                Dim Targets As New WS_Spells.SpellTargets
                Targets.SetTarget_SELF(Me)
                Dim tmpSpell As New WS_Spells.CastSpellParameters(Targets, Me, SpellID)

                If _WS_Spells.SPELLs(SpellID).GetDuration > 0 Then SpellCasted = tmpSpell
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf tmpSpell.Cast))
                Return _WS_Spells.SPELLs(SpellID).GetCastTime
            End Function

            Public Function CastSpell(ByVal SpellID As Integer, ByVal Target As BaseUnit) As Integer
                If Spell_Silenced Then Return -1
                If Target Is Nothing Then Return -1

                'DONE: Shouldn't be able to cast if we're out of range
                'TODO: Is combatreach used here as well?
                If _WS_Combat.GetDistance(Me, Target) > _WS_Spells.SPELLs(SpellID).GetRange Then Return -1

                Dim Targets As New WS_Spells.SpellTargets
                Targets.SetTarget_UNIT(Target)
                Dim tmpSpell As New WS_Spells.CastSpellParameters(Targets, Me, SpellID)

                If _WS_Spells.SPELLs(SpellID).GetDuration > 0 Then SpellCasted = tmpSpell
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf tmpSpell.Cast))
                Return _WS_Spells.SPELLs(SpellID).GetCastTime
            End Function

            Public Function CastSpell(ByVal SpellID As Integer, ByVal x As Single, ByVal y As Single, ByVal z As Single) As Integer
                If Spell_Silenced Then Return -1

                'DONE: Shouldn't be able to cast if we're out of range
                'TODO: Is combatreach used here as well?
                If _WS_Combat.GetDistance(Me, x, y, z) > _WS_Spells.SPELLs(SpellID).GetRange Then Return -1

                Dim Targets As New WS_Spells.SpellTargets
                Targets.SetTarget_DESTINATIONLOCATION(x, y, z)
                Dim tmpSpell As New WS_Spells.CastSpellParameters(Targets, Me, SpellID)

                If _WS_Spells.SPELLs(SpellID).GetDuration > 0 Then SpellCasted = tmpSpell
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf tmpSpell.Cast))
                Return _WS_Spells.SPELLs(SpellID).GetCastTime
            End Function

            Public Sub SpawnCreature(ByVal Entry As Integer, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single)
                Dim tmpCreature As New CreatureObject(Entry, PosX, PosY, PosZ, 0.0F, MapID) With {
                        .instance = instance,
                        .DestroyAtNoCombat = True
                        }
                tmpCreature.AddToWorld()

                If tmpCreature.aiScript IsNot Nothing Then tmpCreature.aiScript.Dispose()
                tmpCreature.aiScript = New WS_Creatures_AI.DefaultAI(tmpCreature)
                tmpCreature.aiScript.aiHateTable = aiScript.aiHateTable
                tmpCreature.aiScript.OnEnterCombat()
                tmpCreature.aiScript.State = AIState.AI_ATTACKING
                tmpCreature.aiScript.DoThink()
            End Sub

            Public Sub SendChatMessage(ByVal Message As String, ByVal msgType As ChatMsg, ByVal msgLanguage As LANGUAGES, Optional ByVal SecondGUID As ULong = 0)
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_MESSAGECHAT)
                Dim flag As Byte = 0

                packet.AddInt8(msgType)
                packet.AddInt32(msgLanguage)

                Select Case msgType
                    Case ChatMsg.CHAT_MSG_MONSTER_SAY, ChatMsg.CHAT_MSG_MONSTER_EMOTE, ChatMsg.CHAT_MSG_MONSTER_YELL
                        packet.AddUInt64(GUID)
                        packet.AddInt32(Text.Encoding.UTF8.GetByteCount(Name) + 1)
                        packet.AddString(Name)
                        packet.AddUInt64(SecondGUID)
                    Case Else
                        _WorldServer.Log.WriteLine(LogType.WARNING, "Creature.SendChatMessage() must not handle this chat type!")
                End Select

                packet.AddInt32(Text.Encoding.UTF8.GetByteCount(Message) + 1)
                packet.AddString(Message)
                packet.AddInt8(flag)
                SendToNearPlayers(packet)
                packet.Dispose()
            End Sub

            Public Sub ResetAI()
                aiScript.Dispose()
                aiScript = New WS_Creatures_AI.DefaultAI(Me)
                MoveType = 1
            End Sub

            Public Sub Initialize()
                'DONE: Database loading
                Level = _WorldServer.Rnd.Next(_WorldServer.CREATURESDatabase(ID).LevelMin, _WorldServer.CREATURESDatabase(ID).LevelMax)
                Size = _WorldServer.CREATURESDatabase(ID).Size
                If Size = 0 Then Size = 1
                Model = _WorldServer.CREATURESDatabase(ID).GetRandomModel
                ManaType = _WorldServer.CREATURESDatabase(ID).ManaType
                Mana.Base = _WorldServer.CREATURESDatabase(ID).Mana
                Mana.Current = Mana.Maximum
                Life.Base = _WorldServer.CREATURESDatabase(ID).Life
                Life.Current = Life.Maximum
                Faction = _WorldServer.CREATURESDatabase(ID).Faction

                For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                    Resistances(i).Base = _WorldServer.CREATURESDatabase(ID).Resistances(i)
                Next

                If EquipmentID = 0 AndAlso _WorldServer.CREATURESDatabase(ID).EquipmentID > 0 Then
                    EquipmentID = _WorldServer.CREATURESDatabase(ID).EquipmentID
                End If

                If _WS_DBCDatabase.CreatureModel.ContainsKey(Model) Then
                    BoundingRadius = _WS_DBCDatabase.CreatureModel(Model).BoundingRadius
                    CombatReach = _WS_DBCDatabase.CreatureModel(Model).CombatReach
                End If

                MechanicImmunity = _WorldServer.CREATURESDatabase(ID).MechanicImmune

                'DONE: Internal Initializators
                CanSeeInvisibility_Stealth = SKILL_DETECTION_PER_LEVEL * Level
                CanSeeInvisibility_Invisibility = 0

                If (_WorldServer.CREATURESDatabase(ID).cNpcFlags And NPCFlags.UNIT_NPC_FLAG_SPIRITHEALER) = NPCFlags.UNIT_NPC_FLAG_SPIRITHEALER Then
                    Invisibility = InvisibilityLevel.DEAD
                    cUnitFlags = UnitFlags.UNIT_FLAG_SPIRITHEALER
                End If
                cDynamicFlags = _WorldServer.CREATURESDatabase(ID).DynFlags

                StandState = cStandState
                cBytes2 = SHEATHE_SLOT.SHEATHE_WEAPON

                If TypeOf Me Is WS_Pets.PetObject Then
                    'DONE: Load pet AI
                    aiScript = New WS_Pets.PetAI(Me)
                Else
                    'DONE: Load scripted AI
                    If _WorldServer.CREATURESDatabase(ID).AIScriptSource <> "" Then
                        aiScript = _WorldServer.AI.InvokeConstructor(_WorldServer.CREATURESDatabase(ID).AIScriptSource, New Object() {Me})
                    ElseIf IO.File.Exists("scripts\creatures\" & _Functions.FixName(Name) & ".vb") Then
                        Dim tmpScript As New ScriptedObject("scripts\creatures\" & _Functions.FixName(Name) & ".vb", "", True)
                        aiScript = tmpScript.InvokeConstructor("CreatureAI_" & _Functions.FixName(Name).Replace(" ", "_"), New Object() {Me})
                        tmpScript.Dispose()
                    End If

                    'DONE: Load default AI
                    If aiScript Is Nothing Then
                        If isCritter Then
                            aiScript = New WS_Creatures_AI.CritterAI(Me)
                        ElseIf isGuard Then
                            If MoveType = 2 Then
                                aiScript = New WS_Creatures_AI.GuardWaypointAI(Me)
                            Else
                                aiScript = New WS_Creatures_AI.GuardAI(Me)
                            End If
                        ElseIf MoveType = 1 Then
                            aiScript = New WS_Creatures_AI.DefaultAI(Me)
                        ElseIf MoveType = 2 Then
                            aiScript = New WS_Creatures_AI.WaypointAI(Me)
                        Else
                            aiScript = New WS_Creatures_AI.StandStillAI(Me)
                        End If
                    End If
                End If
            End Sub

            Public Sub New(ByVal GUID_ As ULong, Optional ByRef Info As DataRow = Nothing)
                'WARNING: Use only for loading creature from DB
                MyBase.New()

                If Info Is Nothing Then
                    Dim MySQLQuery As New DataTable
                    _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM creature LEFT OUTER JOIN game_event_creature ON creature.guid = game_event_creature.guid WHERE creature.guid = {0};", GUID_), MySQLQuery)
                    If MySQLQuery.Rows.Count > 0 Then
                        Info = MySQLQuery.Rows(0)
                    Else
                        _WorldServer.Log.WriteLine(LogType.FAILED, "Creature Spawn not found in database. [GUID={0:X}]", GUID_)
                        Return
                    End If
                End If

                Dim AddonInfo As DataRow = Nothing
                Dim AddonInfoQuery As New DataTable
                _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM spawns_creatures_addon WHERE spawn_id = {0};", GUID_), AddonInfoQuery)
                If AddonInfoQuery.Rows.Count > 0 Then
                    AddonInfo = AddonInfoQuery.Rows(0)
                End If

                positionX = Info.Item("position_X")
                positionY = Info.Item("position_Y")
                positionZ = Info.Item("position_Z")
                orientation = Info.Item("orientation")

                OldX = positionX
                OldY = positionY
                OldZ = positionZ

                SpawnX = positionX
                SpawnY = positionY
                SpawnZ = positionZ
                SpawnO = orientation

                ID = Info.Item("id")
                MapID = Info.Item("map")
                SpawnID = Info.Item("guid")

                Model = Info.Item("modelid")
                SpawnTime = Info.Item("spawntimesecs")

                SpawnRange = Info.Item("spawndist")
                MoveType = Info.Item("MovementType")

                Life.Current = Info.Item("curhealth")
                Mana.Current = Info.Item("curmana")

                EquipmentID = Info.Item("equipment_id")

                'If Not Info.Item("event") Is DBNull.Value Then
                '    GameEvent = Info.Item("event")
                'Else
                '    GameEvent = 0
                'End If

                'TODO: spawn_deathstate?

                If AddonInfo IsNot Nothing Then
                    Mount = AddonInfo.Item("spawn_mount")
                    cEmoteState = AddonInfo.Item("spawn_emote")
                    MoveFlags = AddonInfo.Item("spawn_moveflags")

                    cBytes0 = AddonInfo.Item("spawn_bytes0")
                    cBytes1 = AddonInfo.Item("spawn_bytes1")
                    cBytes2 = AddonInfo.Item("spawn_bytes2")

                    WaypointID = AddonInfo.Item("spawn_pathid")
                    'TODO: spawn_auras
                End If

                If Not _WorldServer.CREATURESDatabase.ContainsKey(ID) Then
                    Dim baseCreature As New CreatureInfo(ID)
                End If

                GUID = GUID_ + _Global_Constants.GUID_UNIT
                Initialize()

                Try
                    _WorldServer.WORLD_CREATUREs_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    _WorldServer.WORLD_CREATUREs.Add(GUID, Me)
                    _WorldServer.WORLD_CREATUREsKeys.Add(GUID)
                    _WorldServer.WORLD_CREATUREs_Lock.ReleaseWriterLock()
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "WS_Creatures:New failed - Guid: {1}  {0}", ex.Message, GUID_)
                End Try
            End Sub

            Public Sub New(ByVal GUID_ As ULong, ByVal ID_ As Integer)
                'WARNING: Use only for spawning new creature
                MyBase.New()

                If Not _WorldServer.CREATURESDatabase.ContainsKey(ID_) Then
                    Dim baseCreature As New CreatureInfo(ID_)
                End If

                ID = ID_
                GUID = GUID_

                Initialize()

                Try
                    _WorldServer.WORLD_CREATUREs_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    _WorldServer.WORLD_CREATUREs.Add(GUID, Me)
                    _WorldServer.WORLD_CREATUREsKeys.Add(GUID)
                    _WorldServer.WORLD_CREATUREs_Lock.ReleaseWriterLock()
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "WS_Creatures:New failed - Guid: {1} ID: {2}  {0}", ex.Message, GUID_, ID_)
                End Try
            End Sub

            Public Sub New(ByVal ID_ As Integer)
                'WARNING: Use only for spawning new creature
                MyBase.New()

                If Not _WorldServer.CREATURESDatabase.ContainsKey(ID_) Then
                    Dim baseCreature As New CreatureInfo(ID_)
                End If

                ID = ID_
                GUID = _WS_Creatures.GetNewGUID()

                Initialize()

                Try
                    _WorldServer.WORLD_CREATUREs_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    _WorldServer.WORLD_CREATUREs.Add(GUID, Me)
                    _WorldServer.WORLD_CREATUREsKeys.Add(GUID)
                    _WorldServer.WORLD_CREATUREs_Lock.ReleaseWriterLock()
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "WS_Creatures:New failed - Guid: {1} ID: {2}  {0}", ex.Message, ID_)
                End Try
            End Sub

            Public Sub New(ByVal ID_ As Integer, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Orientation_ As Single, ByVal Map As Integer, Optional ByVal Duration As Integer = 0)
                'WARNING: Use only for spawning new creature
                MyBase.New()

                If Not _WorldServer.CREATURESDatabase.ContainsKey(ID_) Then
                    Dim baseCreature As New CreatureInfo(ID_)
                End If

                ID = ID_
                GUID = _WS_Creatures.GetNewGUID()

                positionX = PosX
                positionY = PosY
                positionZ = PosZ
                orientation = Orientation_
                MapID = Map

                SpawnX = PosX
                SpawnY = PosY
                SpawnZ = PosZ
                SpawnO = Orientation_

                Initialize()

                'TODO: Duration
                If Duration > 0 Then
                    ExpireTimer = New Timer(AddressOf Destroy, Nothing, Duration, Duration)
                End If

                Try
                    _WorldServer.WORLD_CREATUREs_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    _WorldServer.WORLD_CREATUREs.Add(GUID, Me)
                    _WorldServer.WORLD_CREATUREsKeys.Add(GUID)
                    _WorldServer.WORLD_CREATUREs_Lock.ReleaseWriterLock()
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "WS_Creatures:New failed - Guid: {1} ID: {2} Map: {3}  {0}", ex.Message, GUID, ID_, Map)
                End Try
            End Sub

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    If aiScript IsNot Nothing Then aiScript.Dispose()

                    RemoveFromWorld()

                    Try
                        _WorldServer.WORLD_CREATUREs_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                        _WorldServer.WORLD_CREATUREs.Remove(GUID)
                        _WorldServer.WORLD_CREATUREsKeys.Remove(GUID)
                        _WorldServer.WORLD_CREATUREs_Lock.ReleaseWriterLock()
                        ExpireTimer.Dispose()
                    Catch ex As Exception
                        _WorldServer.Log.WriteLine(LogType.WARNING, "WS_Creatures:Dispose failed -  {0}", ex.Message)
                    End Try
                End If
                _disposedValue = True
            End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region

            Public Sub Destroy(Optional ByVal state As Object = Nothing)
                'TODO: Remove pets also
                If SummonedBy > 0 Then
                    If _CommonGlobalFunctions.GuidIsPlayer(SummonedBy) AndAlso _WorldServer.CHARACTERs.ContainsKey(SummonedBy) Then
                        If _WorldServer.CHARACTERs(SummonedBy).NonCombatPet IsNot Nothing AndAlso _WorldServer.CHARACTERs(SummonedBy).NonCombatPet Is Me Then
                            _WorldServer.CHARACTERs(SummonedBy).NonCombatPet = Nothing
                        End If
                    End If
                End If

                Dim packet As New Packets.PacketClass(OPCODES.SMSG_DESTROY_OBJECT)
                packet.AddUInt64(GUID)
                SendToNearPlayers(packet)
                packet.Dispose()

                Dispose()
            End Sub

            Public Sub Despawn()
                RemoveFromWorld()

                If _WS_Loot.LootTable.ContainsKey(GUID) Then
                    _WS_Loot.LootTable(GUID).Dispose()
                End If

                If SpawnTime > 0 Then
                    If aiScript IsNot Nothing Then
                        aiScript.State = AIState.AI_RESPAWN
                        aiScript.Pause(SpawnTime * 1000)
                    End If
                Else
                    Dispose()
                End If
            End Sub

            Public Sub Respawn()
                Life.Current = Life.Maximum
                Mana.Current = Mana.Maximum
                cUnitFlags = cUnitFlags And (Not UnitFlags.UNIT_FLAG_DEAD)
                cDynamicFlags = 0

                positionX = SpawnX
                positionY = SpawnY
                positionZ = SpawnZ
                orientation = SpawnO

                If aiScript IsNot Nothing Then
                    aiScript.OnLeaveCombat(False)
                    aiScript.State = AIState.AI_WANDERING
                End If

                If SeenBy.Count > 0 Then
                    Dim packetForNear As New Packets.UpdatePacketClass
                    Dim UpdateData As New Packets.UpdateClass(EUnitFields.UNIT_END)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Life.Current)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + MyBase.ManaType, Mana.Current)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, cUnitFlags)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, cDynamicFlags)
                    UpdateData.AddToPacket((packetForNear), ObjectUpdateType.UPDATETYPE_VALUES, Me)

                    SendToNearPlayers(packetForNear)
                    packetForNear.Dispose()
                    UpdateData.Dispose()

                    MoveToInstant(SpawnX, SpawnY, SpawnZ, SpawnO)
                Else
                    AddToWorld()
                End If
            End Sub

            Public Sub AddToWorld()
                _WS_Maps.GetMapTile(positionX, positionY, CellX, CellY)
                If _WS_Maps.Maps(MapID).Tiles(CellX, CellY) Is Nothing Then _WS_CharMovement.MAP_Load(CellX, CellY, MapID)
                Try
                    _WS_Maps.Maps(MapID).Tiles(CellX, CellY).CreaturesHere.Add(GUID)
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.WARNING, "WS_Creatures:AddToWorld failed - Guid: {1} ID: {2}  {0}", ex.Message)
                    Exit Sub
                End Try

                Dim list() As ULong

                'DONE: Sending to players in nearby cells
                For i As Short = -1 To 1
                    For j As Short = -1 To 1
                        If (CellX + i) >= 0 AndAlso (CellX + i) <= 63 AndAlso (CellY + j) >= 0 AndAlso (CellY + j) <= 63 AndAlso _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j) IsNot Nothing AndAlso _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j).PlayersHere.Count > 0 Then
                            With _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j)
                                list = .PlayersHere.ToArray
                                For Each plGUID As ULong In list
                                    If _WorldServer.CHARACTERs(plGUID).CanSee(Me) Then
                                        Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                                        Try
                                            packet.AddInt32(1)
                                            packet.AddInt8(0)
                                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_UNIT)
                                            FillAllUpdateFlags(tmpUpdate)
                                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, Me)
                                            tmpUpdate.Dispose()

                                            _WorldServer.CHARACTERs(plGUID).client.SendMultiplyPackets(packet)

                                            _WorldServer.CHARACTERs(plGUID).creaturesNear.Add(GUID)
                                            SeenBy.Add(plGUID)
                                        Finally
                                            packet.Dispose()
                                        End Try
                                    End If
                                Next
                            End With
                        End If
                    Next
                Next

            End Sub

            Public Sub RemoveFromWorld()
                _WS_Maps.GetMapTile(positionX, positionY, CellX, CellY)
                _WS_Maps.Maps(MapID).Tiles(CellX, CellY).CreaturesHere.Remove(GUID)

                'DONE: Removing from players who can see the creature
                For Each plGUID As ULong In SeenBy.ToArray
                    If _WorldServer.CHARACTERs.ContainsKey(plGUID) Then
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving.Add(GUID)
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving_Lock.ReleaseWriterLock()

                        _WorldServer.CHARACTERs(plGUID).creaturesNear.Remove(GUID)
                    End If
                Next

                SeenBy.Clear()
            End Sub

            Public Sub MoveCell()
                Try
                    If CellX <> _WS_Maps.GetMapTileX(positionX) OrElse CellY <> _WS_Maps.GetMapTileY(positionY) Then
                        If IsNothing(_WS_Maps.Maps(MapID).Tiles(CellX, CellY).CreaturesHere.Remove(GUID)) = False Then
                            _WS_Maps.Maps(MapID).Tiles(CellX, CellY).CreaturesHere.Remove(GUID)
                        End If
                        _WS_Maps.GetMapTile(positionX, positionY, CellX, CellY)

                        'If creature changes cell then it's sent back to spawn, if the creature is a waypoint walker this won't be very good :/
                        If _WS_Maps.Maps(MapID).Tiles(CellX, CellY) Is Nothing Then
                            aiScript.Reset()
                            Exit Sub
                        Else
                            _WS_Maps.Maps(MapID).Tiles(CellX, CellY).CreaturesHere.Add(GUID)
                        End If
                    End If
                Catch e As Exception
                    'Creature ran outside of mapbounds, reset it
                    _WorldServer.Log.WriteLine(LogType.WARNING, "WS_Creatures:MoveCell - Creature outside of map bounds, Resetting  {0}", e.Message)
                    Try
                        aiScript.Reset()
                    Catch ex As Exception
                        _WorldServer.Log.WriteLine(LogType.FAILED, "WS_Creatures:MoveCell - Couldn't reset creature outside of map bounds, Disposing  {0}", ex.Message)
                        aiScript.Dispose()
                    End Try
                End Try
            End Sub
        End Class
#End Region

#Region "WS.Creatures.HelperSubs"
        Public CorpseDecay() As Integer = {30, 150, 150, 150, 1800}

        Public Sub On_CMSG_CREATURE_QUERY(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 17 Then Exit Sub
            Dim response As New Packets.PacketClass(OPCODES.SMSG_CREATURE_QUERY_RESPONSE)

            packet.GetInt16()
            Dim CreatureID As Integer = packet.GetInt32
            Dim CreatureGUID As ULong = packet.GetUInt64

            Try
                Dim Creature As CreatureInfo

                If _WorldServer.CREATURESDatabase.ContainsKey(CreatureID) = False Then
                    _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CREATURE_QUERY [Creature {2} not loaded.]", client.IP, client.Port, CreatureID)

                    response.AddUInt32((CreatureID Or &H80000000))
                    client.Send(response)
                    response.Dispose()
                    Exit Sub
                Else
                    Creature = _WorldServer.CREATURESDatabase(CreatureID)
                    '_WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CREATURE_QUERY [CreatureID={2} CreatureGUID={3:X}]", Format(TimeOfDay, "hh:mm:ss"), client.IP, client.Port, CreatureID, CreatureGUID - _Global_Constants.GUID_UNIT)
                End If

                response.AddInt32(Creature.Id)
                response.AddString(Creature.Name)
                response.AddInt8(0)                         'Creature.Name2
                response.AddInt8(0)                         'Creature.Name3
                response.AddInt8(0)                         'Creature.Name4
                response.AddString(Creature.SubName)

                response.AddInt32(Creature.TypeFlags)       'TypeFlags
                response.AddInt32(Creature.CreatureType)    'Type
                response.AddInt32(Creature.CreatureFamily)  'Family
                response.AddInt32(Creature.Elite)           'Rank
                response.AddInt32(0)                        'Unk
                response.AddInt32(Creature.PetSpellDataID)  'PetSpellDataID
                response.AddInt32(Creature.ModelA1)         'ModelA1
                response.AddInt32(Creature.ModelA2)         'ModelA2
                response.AddInt32(Creature.ModelH1)         'ModelH1
                response.AddInt32(Creature.ModelH2)         'ModelH2
                response.AddSingle(1.0F)                    'Unk
                response.AddSingle(1.0F)                    'Unk
                response.AddInt8(Creature.Leader)           'RacialLeader

                client.Send(response)
                response.Dispose()
                '_WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_CREATURE_QUERY_RESPONSE", client.IP, client.Port)
            Catch ex As Exception
                _WorldServer.Log.WriteLine(LogType.FAILED, "Unknown Error: Unable to find CreatureID={0} in database. {1}", CreatureID, ex.Message)
            End Try
        End Sub

        Public Sub On_CMSG_NPC_TEXT_QUERY(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 17 Then Exit Sub
            packet.GetInt16()
            Dim TextID As Long = packet.GetInt32
            Dim TargetGUID As ULong = packet.GetUInt64
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_NPC_TEXT_QUERY [TextID={2}]", client.IP, client.Port, TextID)

            client.Character.SendTalking(TextID)
        End Sub

        Public Sub On_CMSG_GOSSIP_HELLO(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GOSSIP_HELLO [GUID={2:X}]", client.IP, client.Port, GUID)
            If _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) = False OrElse _WorldServer.WORLD_CREATUREs(GUID).CreatureInfo.cNpcFlags = 0 Then
                _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] Client tried to speak with a creature that didn't exist or couldn't interact with. [GUID={2:X}  ID={3}]", client.IP, client.Port, GUID, _WorldServer.WORLD_CREATUREs(GUID).ID)
                Exit Sub
            End If
            If _WorldServer.WORLD_CREATUREs(GUID).Evade Then Exit Sub

            _WorldServer.WORLD_CREATUREs(GUID).StopMoving()
            client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TALK)

            Try
                If _WorldServer.CREATURESDatabase(_WorldServer.WORLD_CREATUREs(GUID).ID).TalkScript Is Nothing Then
                    Dim test As New Packets.PacketClass(OPCODES.SMSG_NPC_WONT_TALK)
                    test.AddUInt64(GUID)
                    test.AddInt8(1)
                    client.Send(test)
                    test.Dispose()

                    If NPCTexts.ContainsKey(34) = False Then
                        Dim tmpText As New NPCText(34, "Hi $N, I'm not yet scripted to talk with you.")
                    End If
                    client.Character.SendTalking(34)

                    client.Character.SendGossip(GUID, 34)
                Else
                    _WorldServer.CREATURESDatabase(_WorldServer.WORLD_CREATUREs(GUID).ID).TalkScript.OnGossipHello(client.Character, GUID)
                End If
            Catch ex As Exception
                _WorldServer.Log.WriteLine(LogType.CRITICAL, "Error in gossip hello.{0}{1}", Environment.NewLine, ex.ToString)
            End Try
        End Sub

        Public Sub On_CMSG_GOSSIP_SELECT_OPTION(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 17 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64
            Dim SelOption As Integer = packet.GetInt32
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GOSSIP_SELECT_OPTION [SelOption={3} GUID={2:X}]", client.IP, client.Port, GUID, SelOption)
            If _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) = False OrElse _WorldServer.WORLD_CREATUREs(GUID).CreatureInfo.cNpcFlags = 0 Then
                _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] Client tried to speak with a creature that didn't exist or couldn't interact with. [GUID={2:X}  ID={3}]", client.IP, client.Port, GUID, _WorldServer.WORLD_CREATUREs(GUID).ID)
                Exit Sub
            End If

            If _WorldServer.CREATURESDatabase(_WorldServer.WORLD_CREATUREs(GUID).ID).TalkScript Is Nothing Then
                Throw New ApplicationException("Invoked OnGossipSelect() on creature without initialized TalkScript!")
            Else
                _WorldServer.CREATURESDatabase(_WorldServer.WORLD_CREATUREs(GUID).ID).TalkScript.OnGossipSelect(client.Character, GUID, SelOption)
            End If
        End Sub

        Public Sub On_CMSG_SPIRIT_HEALER_ACTIVATE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SPIRIT_HEALER_ACTIVATE [GUID={2}]", client.IP, client.Port, GUID)

            Try
                For i As Byte = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                    If client.Character.Items.ContainsKey(i) Then client.Character.Items(i).ModifyDurability(0.25F, client)
                Next
            Catch ex As Exception
                _WorldServer.Log.WriteLine(LogType.FAILED, "Error activating spirit healer: {0}", ex.ToString)
            End Try

            _WS_Handlers_Misc.CharacterResurrect(client.Character)

            client.Character.ApplySpell(15007)
        End Sub

        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Function GetNewGUID() As ULong
            _WorldServer.CreatureGUIDCounter += 1
            Return _WorldServer.CreatureGUIDCounter
        End Function
        Public NPCTexts As New Dictionary(Of Integer, NPCText)
        Public Class NPCText
            Public Count As Byte = 1

            Public TextID As Integer = 0
            Public Probability() As Single = {0, 0, 0, 0, 0, 0, 0, 0}
            Public Language() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
            Public TextLine1() As String = {"", "", "", "", "", "", "", ""}
            Public TextLine2() As String = {"", "", "", "", "", "", "", ""}
            Public Emote1() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
            Public Emote2() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
            Public Emote3() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
            Public EmoteDelay1() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
            Public EmoteDelay2() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
            Public EmoteDelay3() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}

            Public Sub New(ByVal _TextID As Integer)
                TextID = _TextID

                Dim MySQLQuery As New DataTable
                _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM npc_text WHERE ID = {0};", TextID), MySQLQuery)

                If MySQLQuery.Rows.Count > 0 Then
                    For i As Integer = 0 To 7
                        Probability(i) = MySQLQuery.Rows(0).Item("prob" & i & "")
                        If IsDBNull(MySQLQuery.Rows(0).Item("text" & i & "_0")) = False Then
                            TextLine1(i) = MySQLQuery.Rows(0).Item("text" & i & "_0")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("text" & i & "_1")) = False Then
                            TextLine2(i) = MySQLQuery.Rows(0).Item("text" & i & "_1")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("lang" & i & "")) = False Then
                            Language(i) = MySQLQuery.Rows(0).Item("lang" & i & "")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("em" & i & "_0_delay")) = False Then
                            EmoteDelay1(i) = MySQLQuery.Rows(0).Item("em" & i & "_0_delay")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("em" & i & "_0")) = False Then
                            Emote1(i) = MySQLQuery.Rows(0).Item("em" & i & "_0")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("em" & i & "_1_delay")) = False Then
                            EmoteDelay2(i) = MySQLQuery.Rows(0).Item("em" & i & "_1_delay")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("em" & i & "_1")) = False Then
                            Emote2(i) = MySQLQuery.Rows(0).Item("em" & i & "_1")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("em" & i & "_2_delay")) = False Then
                            EmoteDelay3(i) = MySQLQuery.Rows(0).Item("em" & i & "_2_delay")
                        End If

                        If IsDBNull(MySQLQuery.Rows(0).Item("em" & i & "_2")) = False Then
                            Emote3(i) = MySQLQuery.Rows(0).Item("em" & i & "_2")
                        End If

                        If TextLine1(i) <> "" Then Count = CByte(i) + 1
                    Next
                Else
                    Probability(0) = 1
                    TextLine1(0) = "Hey there, $N. How can I help you?"
                    TextLine2(0) = TextLine1(0)
                    Count = 0
                End If

                _WS_Creatures.NPCTexts.Add(TextID, Me)
            End Sub

            Public Sub New(ByVal _TextID As Integer, ByVal TextLine As String)
                TextID = _TextID
                TextLine1(0) = TextLine
                TextLine2(0) = TextLine
                Count = 0

                _WS_Creatures.NPCTexts.Add(TextID, Me)
            End Sub
        End Class
#End Region

        '#Region "WS.Creatures.MonsterSayCombat"
        '    Public MonsterSayCombat As New Dictionary(Of Integer, TMonsterSayCombat)
        '    Public Class TMonsterSayCombat
        '        Public Entry As Integer
        '        Public EventNo As Integer
        '        Public Chance As Single
        '        Public Language As Integer
        '        Public Type As Integer
        '        Public MonsterName As String
        '        Public Text0 As String
        '        Public Text1 As String
        '        Public Text2 As String
        '        Public Text3 As String
        '        Public Text4 As String

        '        Public Sub New(ByVal Entry_ As Integer, ByVal EventNo_ As Integer, ByVal Chance_ As Single, ByVal Language_ As Integer, ByVal Type_ As Integer, ByVal MonsterName_ As String, ByVal Text0_ As String, ByVal Text1_ As String, ByVal Text2_ As String, ByVal Text3_ As String, ByVal Text4_ As String)
        '            Entry = Entry_
        '            EventNo = EventNo_
        '            Chance = Chance_
        '            Language = Language_
        '            Type = Type_
        '            MonsterName = MonsterName_
        '            Text0 = Text0_
        '            Text1 = Text1_
        '            Text2 = Text2_
        '            Text3 = Text3_
        '            Text4 = Text4
        '        End Sub
        '    End Class
        '#End Region

    End Class

#Region "WS.Creatures.Gossip"

#End Region
End Namespace