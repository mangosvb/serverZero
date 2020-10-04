'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports Mangos.Common
Imports Mangos.Common.Globals
Imports Mangos.Shared

Public Module WS_Base
    Public Class BaseObject
        Public GUID As ULong = 0
        Public CellX As Byte = 0
        Public CellY As Byte = 0

        Public positionX As Single = 0
        Public positionY As Single = 0
        Public positionZ As Single = 0
        Public orientation As Single = 0
        Public instance As UInteger = 0
        Public MapID As UInteger = 0
        Public CorpseType As CorpseType = CorpseType.CORPSE_BONES
        Public SpawnID As Integer = 0
        Public SeenBy As New List(Of ULong)

        Public VisibleDistance As Single = DEFAULT_DISTANCE_VISIBLE

        Public Invisibility As InvisibilityLevel = InvisibilityLevel.VISIBLE
        Public Invisibility_Value As Integer = 0
        Public Invisibility_Bonus As Integer = 0
        Public CanSeeInvisibility As InvisibilityLevel = InvisibilityLevel.INIVISIBILITY
        Public CanSeeInvisibility_Stealth As Integer = 0
        Public CanSeeStealth As Boolean = False
        Public CanSeeInvisibility_Invisibility As Integer = 0
        Public Overridable Function CanSee(ByRef objCharacter As BaseObject) As Boolean
            If GUID = objCharacter.GUID Then Return False
            If instance <> objCharacter.instance Then Return False

            'DONE: GM and DEAD invisibility
            If objCharacter.Invisibility > CanSeeInvisibility Then Return False
            'DONE: Stealth Detection
            If objCharacter.Invisibility = InvisibilityLevel.STEALTH AndAlso (Math.Sqrt((objCharacter.positionX - positionX) ^ 2 + (objCharacter.positionY - positionY) ^ 2) < DEFAULT_DISTANCE_DETECTION) Then Return True
            'DONE: Check invisibility
            If objCharacter.Invisibility = InvisibilityLevel.INIVISIBILITY AndAlso objCharacter.Invisibility_Value > CanSeeInvisibility_Invisibility Then Return False
            If objCharacter.Invisibility = InvisibilityLevel.STEALTH AndAlso objCharacter.Invisibility_Value > CanSeeInvisibility_Stealth Then Return False

            'DONE: Check distance
            If Math.Sqrt((objCharacter.positionX - positionX) ^ 2 + (objCharacter.positionY - positionY) ^ 2) > objCharacter.VisibleDistance Then Return False
            Return True
        End Function

        Public Sub InvisibilityReset()
            Invisibility = InvisibilityLevel.VISIBLE
            Invisibility_Value = 0
            CanSeeInvisibility = InvisibilityLevel.INIVISIBILITY
            CanSeeInvisibility_Stealth = 0
            CanSeeInvisibility_Invisibility = 0
        End Sub

        Public Sub SendPlaySound(ByVal SoundID As Integer, Optional ByVal OnlyToSelf As Boolean = False)
            Dim packet As New PacketClass(OPCODES.SMSG_PLAY_OBJECT_SOUND)
            Try
                packet.AddInt32(SoundID)
                packet.AddUInt64(GUID)
                If OnlyToSelf AndAlso (TypeOf Me Is CharacterObject) Then
                    CType(Me, CharacterObject).client.Send(packet)
                Else
                    SendToNearPlayers(packet)
                End If
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub SendToNearPlayers(ByRef packet As PacketClass, Optional ByVal NotTo As ULong = 0, Optional ByVal ToSelf As Boolean = True)
            If ToSelf AndAlso (TypeOf Me Is CharacterObject) AndAlso CType(Me, CharacterObject).client IsNot Nothing Then CType(Me, CharacterObject).client.SendMultiplyPackets(packet)
            For Each objCharacter As ULong In SeenBy.ToArray
                If objCharacter <> NotTo AndAlso CHARACTERs.ContainsKey(objCharacter) AndAlso CHARACTERs(objCharacter).client IsNot Nothing Then CHARACTERs(objCharacter).client.SendMultiplyPackets(packet)
            Next
        End Sub
    End Class

    Public Class BaseUnit
        Inherits BaseObject

        Public Const CombatReach_Base As Single = 2.0F

        Public OnTransport As GameObjectObject = Nothing
        Public transportX As Single = 0.0F
        Public transportY As Single = 0.0F
        Public transportZ As Single = 0.0F
        Public transportO As Single = 0.0F

        Public BoundingRadius As Single = 0.389F
        Public CombatReach As Single = 1.5F

        Public cUnitFlags As Integer = UnitFlags.UNIT_FLAG_ATTACKABLE
        Public cDynamicFlags As Integer = 0 'DynamicFlags.UNIT_DYNFLAG_SPECIALINFO

        '                                                    <<0                <<8             <<16                <<24
        Public cBytes0 As Integer = 0                       'Race               Classe          Gender              ManaType
        Public cBytes1 As Integer = 0                       'StandState,        PetLoyalty,     ShapeShift,         StealthFlag [CType(Invisibility > InvisibilityLevel.VISIBLE, Integer) * 2 << 24]
        Public cBytes2 As Integer = &HEEEEEE00              '?                  ?               ?                   ?

        'cBytes0 subfields
        Public Overridable Property ManaType() As ManaTypes
            Get
                Return (cBytes0 And &HFF000000) >> 24
            End Get
            Set(ByVal value As ManaTypes)
                cBytes0 = ((cBytes0 And &HFFFFFF) Or (value << 24))
            End Set
        End Property

        Public Overridable Property Gender() As Genders
            Get
                Return (cBytes0 And &HFF0000) >> 16
            End Get
            Set(ByVal value As Genders)
                cBytes0 = ((cBytes0 And &HFF00FFFF) Or (CInt(value) << 16))
            End Set
        End Property

        Public Overridable Property Classe() As Classes
            Get
                Return (cBytes0 And &HFF00) >> 8
            End Get
            Set(ByVal value As Classes)
                cBytes0 = ((cBytes0 And &HFFFF00FF) Or (CInt(value) << 8))
            End Set
        End Property

        Public Overridable Property Race() As Races
            Get
                Return (cBytes0 And &HFF) >> 0
            End Get
            Set(ByVal value As Races)
                cBytes0 = ((cBytes0 And &HFFFFFF00) Or (CInt(value) << 0))
            End Set
        End Property

        Public ReadOnly Property UnitName() As String
            Get
                If TypeOf Me Is CharacterObject Then
                    Return CType(Me, CharacterObject).Name
                ElseIf TypeOf Me Is CreatureObject Then
                    Return CType(Me, CreatureObject).Name
                Else
                    Return ""
                End If
            End Get
        End Property

        'cBytes1 subfields
        Public Overridable Property StandState() As Byte
            Get
                Return (cBytes1 And &HFF) >> 0
            End Get
            Set(ByVal Value As Byte)
                cBytes1 = ((cBytes1 And &HFFFFFF00) Or (CInt(Value) << 0))
            End Set
        End Property

        Public Overridable Property PetLoyalty() As Byte
            Get
                Return (cBytes1 And &HFF00) >> 8
            End Get
            Set(ByVal Value As Byte)
                cBytes1 = ((cBytes1 And &HFFFF00FF) Or (CInt(Value) << 8))
            End Set
        End Property

        Public Overridable Property ShapeshiftForm() As ShapeshiftForm
            Get
                Return (cBytes1 And &HFF0000) >> 16
            End Get
            Set(ByVal form As ShapeshiftForm)
                cBytes1 = ((cBytes1 And &HFF00FFFF) Or (CInt(form) << 16))
            End Set
        End Property

        Public Level As Byte = 0
        Public Model As Integer = 0
        Public Mount As Integer = 0
        Public Life As New TStatBar(1, 1, 0)
        Public Mana As New TStatBar(1, 1, 0)
        Public Size As Single = 1.0
        Public Resistances(6) As TStat
        Public SchoolImmunity As Byte = 0
        Public MechanicImmunity As UInteger = 0UI
        Public DispellImmunity As UInteger = 0UI

        Public AbsorbSpellLeft As New Dictionary(Of Integer, UInteger)
        Public Invulnerable As Boolean = False

        Public SummonedBy As ULong = 0
        Public CreatedBy As ULong = 0
        Public CreatedBySpell As Integer = 0

        Public cEmoteState As Integer = 0

        'Temporaly variables
        Public AuraState As Integer = 0
        Public Spell_Silenced As Boolean = False
        Public Spell_Pacifyed As Boolean = False
        Public Spell_ThreatModifier As Single = 1.0F
        Public AttackPowerMods As Integer = 0
        Public AttackPowerModsRanged As Integer = 0
        Public dynamicObjects As New List(Of DynamicObjectObject)
        Public gameObjects As New List(Of GameObjectObject)

        Public Overridable Sub Die(ByRef Attacker As BaseUnit)
            Log.WriteLine(LogType.WARNING, "BaseUnit can't die.")
        End Sub

        Public Overridable Sub DealDamage(ByVal Damage As Integer, Optional ByRef Attacker As BaseUnit = Nothing)
            Log.WriteLine(LogType.WARNING, "No damage dealt.")
        End Sub

        Public Overridable Sub Heal(ByVal Damage As Integer, Optional ByRef Attacker As BaseUnit = Nothing)
            Log.WriteLine(LogType.WARNING, "No healing done.")
        End Sub

        Public Overridable Sub Energize(ByVal Damage As Integer, ByVal Power As ManaTypes, Optional ByRef Attacker As BaseUnit = Nothing)
            Log.WriteLine(LogType.WARNING, "No mana increase done.")
        End Sub

        Public Overridable ReadOnly Property IsDead() As Boolean
            Get
                Return (Life.Current = 0)
            End Get
        End Property

        Public Overridable ReadOnly Property Exist() As Boolean
            Get
                If TypeOf Me Is CharacterObject Then
                    Return CHARACTERs.ContainsKey(GUID)
                ElseIf TypeOf Me Is CreatureObject Then
                    Return WORLD_CREATUREs.ContainsKey(GUID)
                End If
                Return False
            End Get
        End Property

        Public Overridable ReadOnly Property IsRooted() As Boolean
            Get
                Return (cUnitFlags And UnitFlags.UNIT_FLAG_ROOTED)
            End Get
        End Property

        Public Overridable ReadOnly Property IsStunned() As Boolean
            Get
                Return (cUnitFlags And UnitFlags.UNIT_FLAG_STUNTED)
            End Get
        End Property

        Public ReadOnly Property IsInFeralForm() As Boolean
            Get
                Return (ShapeshiftForm = ShapeshiftForm.FORM_CAT OrElse ShapeshiftForm = ShapeshiftForm.FORM_BEAR OrElse ShapeshiftForm = ShapeshiftForm.FORM_DIREBEAR)
            End Get
        End Property

        Public ReadOnly Property IsPlayer() As Boolean
            Get
                Return (TypeOf Me Is CharacterObject)
            End Get
        End Property

        Public Overridable Function IsFriendlyTo(ByRef Unit As BaseUnit) As Boolean
            Return False
        End Function

        Public Overridable Function IsEnemyTo(ByRef Unit As BaseUnit) As Boolean
            Return False
        End Function

        'Spell Aura Managment
        Public ActiveSpells(MAX_AURA_EFFECTs - 1) As BaseActiveSpell
        Public ActiveSpells_Flags(MAX_AURA_EFFECT_FLAGs - 1) As Integer
        Public ActiveSpells_Count(MAX_AURA_EFFECT_LEVELSs - 1) As Integer
        Public ActiveSpells_Level(MAX_AURA_EFFECT_LEVELSs - 1) As Integer
        Public Sub SetAura(ByVal SpellID As Integer, ByVal Slot As Integer, ByVal Duration As Integer, Optional ByVal SendUpdate As Boolean = True)
            If ActiveSpells(Slot) Is Nothing Then Exit Sub
            'DONE: Passive auras are not displayed
            If SpellID AndAlso SPELLs.ContainsKey(SpellID) AndAlso SPELLs(SpellID).IsPassive Then Exit Sub

            'DONE: Calculating slots
            Dim AuraLevel_Slot As Integer = Slot \ 4
            Dim AuraFlag_Slot As Integer = Slot >> 3
            Dim AuraFlag_SubSlot As Integer = (Slot And 7) << 2
            Dim AuraFlag_Value As Integer = 9 << AuraFlag_SubSlot

            ActiveSpells_Flags(AuraFlag_Slot) = ActiveSpells_Flags(AuraFlag_Slot) And (Not AuraFlag_Value)
            If SpellID <> 0 Then
                ActiveSpells_Flags(AuraFlag_Slot) = ActiveSpells_Flags(AuraFlag_Slot) Or AuraFlag_Value
            End If

            Dim tmpLevel As Byte = 0
            If SpellID Then tmpLevel = SPELLs(SpellID).spellLevel
            SetAuraStackCount(Slot, 0)
            SetAuraSlotLevel(Slot, tmpLevel)

            'DONE: Sending updates
            If SendUpdate Then
                If TypeOf Me Is CharacterObject Then
                    CType(Me, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AURA + Slot, SpellID)
                    CType(Me, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAFLAGS + AuraFlag_Slot, ActiveSpells_Flags(AuraFlag_Slot))
                    CType(Me, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAAPPLICATIONS + AuraLevel_Slot, ActiveSpells_Count(AuraLevel_Slot))
                    CType(Me, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AURALEVELS + AuraLevel_Slot, ActiveSpells_Level(AuraLevel_Slot))
                    CType(Me, CharacterObject).SendCharacterUpdate(True)

                    Dim SMSG_UPDATE_AURA_DURATION As New PacketClass(OPCODES.SMSG_UPDATE_AURA_DURATION)
                    Try
                        SMSG_UPDATE_AURA_DURATION.AddInt8(Slot)
                        SMSG_UPDATE_AURA_DURATION.AddInt32(Duration)
                        CType(Me, CharacterObject).client.Send(SMSG_UPDATE_AURA_DURATION)
                    Finally
                        SMSG_UPDATE_AURA_DURATION.Dispose()
                    End Try
                Else
                    Dim tmpUpdate As New UpdateClass
                    Dim tmpPacket As New UpdatePacketClass
                    Try
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURA + Slot, SpellID)
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAFLAGS + AuraFlag_Slot, ActiveSpells_Flags(AuraFlag_Slot))
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAAPPLICATIONS + AuraLevel_Slot, ActiveSpells_Count(AuraLevel_Slot))
                        tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURALEVELS + AuraLevel_Slot, ActiveSpells_Level(AuraLevel_Slot))
                        tmpUpdate.AddToPacket(tmpPacket, ObjectUpdateType.UPDATETYPE_VALUES, CType(Me, CreatureObject))
                        SendToNearPlayers(tmpPacket)
                    Finally
                        tmpPacket.Dispose()
                        tmpUpdate.Dispose()
                    End Try
                End If
            End If
        End Sub

        Public Sub SetAuraStackCount(ByVal Slot As Integer, ByVal Count As Byte)
            'NOTE: Stack count is Zero based -> 2 means "Stacked 3 times"

            Dim AuraFlag_Slot As Integer = Slot \ 4
            Dim AuraFlag_SubSlot As Integer = (Slot Mod 4) * 8

            ActiveSpells_Count(AuraFlag_Slot) = ActiveSpells_Count(AuraFlag_Slot) And (Not (&HFF << AuraFlag_SubSlot))
            ActiveSpells_Count(AuraFlag_Slot) = ActiveSpells_Count(AuraFlag_Slot) Or (CInt(Count) << AuraFlag_SubSlot)
        End Sub

        Public Sub SetAuraSlotLevel(ByVal Slot As Integer, ByVal Level As Integer)
            Dim AuraFlag_Slot As Integer = Slot \ 4
            Dim AuraFlag_SubSlot As Integer = (Slot Mod 4) * 8

            ActiveSpells_Level(AuraFlag_Slot) = ActiveSpells_Level(AuraFlag_Slot) And (Not (&HFF << AuraFlag_SubSlot))
            ActiveSpells_Level(AuraFlag_Slot) = ActiveSpells_Level(AuraFlag_Slot) Or (Level << AuraFlag_SubSlot)
        End Sub

        Public Function HaveAura(ByVal SpellID As Integer) As Boolean
            For i As Byte = 0 To MAX_AURA_EFFECTs - 1
                If ActiveSpells(i) IsNot Nothing AndAlso ActiveSpells(i).SpellID = SpellID Then Return True
            Next
            Return False
        End Function

        Public Function HaveAuraType(ByVal AuraIndex As AuraEffects_Names) As Boolean
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If Not ActiveSpells(i) Is Nothing Then
                    For j As Byte = 0 To 2
                        If ActiveSpells(i).Aura_Info(j) IsNot Nothing AndAlso ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraIndex Then
                            Return True
                        End If
                    Next
                End If
            Next
            Return False
        End Function

        Public Function HaveVisibleAura(ByVal SpellID As Integer) As Boolean
            For i As Byte = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If Not ActiveSpells(i) Is Nothing AndAlso ActiveSpells(i).SpellID = SpellID Then Return True
            Next
            Return False
        End Function

        Public Function HavePassiveAura(ByVal SpellID As Integer) As Boolean
            For i As Byte = MAX_AURA_EFFECTs_VISIBLE To MAX_AURA_EFFECTs - 1
                If Not ActiveSpells(i) Is Nothing AndAlso ActiveSpells(i).SpellID = SpellID Then Return True
            Next
            Return False
        End Function

        Public Sub RemoveAura(ByVal Slot As Integer, ByRef Caster As BaseUnit, Optional ByVal RemovedByDuration As Boolean = False, Optional ByVal SendUpdate As Boolean = True)
            'DONE: Removing SpellAura
            Dim RemoveAction As AuraAction = AuraAction.AURA_REMOVE
            If RemovedByDuration Then RemoveAction = AuraAction.AURA_REMOVEBYDURATION
            If ActiveSpells(Slot) IsNot Nothing Then
                For j As Byte = 0 To 2
                    If ActiveSpells(Slot).Aura(j) IsNot Nothing Then ActiveSpells(Slot).Aura(j).Invoke(Me, Caster, ActiveSpells(Slot).Aura_Info(j), ActiveSpells(Slot).SpellID, ActiveSpells(Slot).StackCount + 1, RemoveAction)
                Next j
            End If

            If SendUpdate AndAlso Slot < MAX_AURA_EFFECTs_VISIBLE Then SetAura(0, Slot, 0)
            ActiveSpells(Slot) = Nothing
        End Sub

        Public Sub RemoveAuraBySpell(ByVal SpellID As Integer)
            'DONE: Real aura removing
            For i As Integer = 0 To MAX_AURA_EFFECTs - 1
                If ActiveSpells(i) IsNot Nothing AndAlso ActiveSpells(i).SpellID = SpellID Then
                    RemoveAura(i, ActiveSpells(i).SpellCaster)

                    'DONE: Removing additional spell auras (Mind Vision)
                    If (TypeOf Me Is CharacterObject) AndAlso
                        (CType(Me, CharacterObject).DuelArbiter <> 0) AndAlso (CType(Me, CharacterObject).DuelPartner Is Nothing) Then
                        WORLD_CREATUREs(CType(Me, CharacterObject).DuelArbiter).RemoveAuraBySpell(SpellID)
                        CType(Me, CharacterObject).DuelArbiter = 0
                    End If
                    Exit Sub
                End If
            Next
        End Sub

        Public Sub RemoveAurasOfType(ByVal AuraIndex As AuraEffects_Names, Optional ByRef NotSpellID As Integer = 0)
            'DONE: Removing SpellAuras of a certain type
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing AndAlso ActiveSpells(i).SpellID <> NotSpellID Then
                    For j As Byte = 0 To 2
                        If (Not ActiveSpells(i).Aura_Info(j) Is Nothing) AndAlso ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraIndex Then
                            RemoveAura(i, ActiveSpells(i).SpellCaster)
                            Exit For
                        End If
                    Next
                End If
            Next
        End Sub

        Public Sub RemoveAurasByMechanic(ByVal Mechanic As Integer)
            'DONE: Removing SpellAuras of a certain mechanic
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing AndAlso SPELLs(ActiveSpells(i).SpellID).Mechanic = Mechanic Then
                    RemoveAura(i, ActiveSpells(i).SpellCaster)
                End If
            Next
        End Sub

        Public Sub RemoveAurasByDispellType(ByVal DispellType As Integer, ByVal Amount As Integer)
            'DONE: Removing SpellAuras of a certain dispelltype
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing AndAlso SPELLs(ActiveSpells(i).SpellID).DispellType = DispellType Then
                    RemoveAura(i, ActiveSpells(i).SpellCaster)
                    Amount -= 1
                    If Amount <= 0 Then Exit For
                End If
            Next
        End Sub

        Public Sub RemoveAurasByInterruptFlag(ByVal AuraInterruptFlag As Integer)
            'DONE: Removing SpellAuras with a certain interruptflag
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing Then
                    If SPELLs.ContainsKey(ActiveSpells(i).SpellID) AndAlso (SPELLs(ActiveSpells(i).SpellID).auraInterruptFlags And AuraInterruptFlag) Then
                        If (SPELLs(ActiveSpells(i).SpellID).procFlags And SpellAuraProcFlags.AURA_PROC_REMOVEONUSE) = 0 Then
                            RemoveAura(i, ActiveSpells(i).SpellCaster)
                        End If
                    End If
                End If
            Next

            'DONE: Interrupt channeled spells
            If TypeOf Me Is CharacterObject Then
                With CType(Me, CharacterObject)
                    If .spellCasted(CurrentSpellTypes.CURRENT_CHANNELED_SPELL) IsNot Nothing AndAlso .spellCasted(CurrentSpellTypes.CURRENT_CHANNELED_SPELL).Finished = False AndAlso (.spellCasted(CurrentSpellTypes.CURRENT_CHANNELED_SPELL).SpellInfo.channelInterruptFlags And AuraInterruptFlag) <> 0 Then
                        .FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL)
                    End If
                End With
            ElseIf TypeOf Me Is CreatureObject Then
                With CType(Me, CreatureObject)
                    If .SpellCasted IsNot Nothing AndAlso (.SpellCasted.SpellInfo.channelInterruptFlags And AuraInterruptFlag) <> 0 Then
                        .StopCasting()
                    End If
                End With
            End If
        End Sub

        Public Function GetAuraModifier(ByVal AuraIndex As AuraEffects_Names) As Integer
            Dim Modifier As Integer = 0
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing Then
                    For j As Byte = 0 To 2
                        If (Not ActiveSpells(i).Aura_Info(j) Is Nothing) AndAlso ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraIndex Then
                            Modifier += ActiveSpells(i).Aura_Info(j).GetValue(Level)
                        End If
                    Next
                End If
            Next
            Return Modifier
        End Function

        Public Function GetAuraModifierByMiscMask(ByVal AuraIndex As AuraEffects_Names, ByVal Mask As Integer) As Integer
            Dim Modifier As Integer = 0
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If ActiveSpells(i) IsNot Nothing Then
                    For j As Byte = 0 To 2
                        If (Not ActiveSpells(i).Aura_Info(j) Is Nothing) AndAlso ActiveSpells(i).Aura_Info(j).ApplyAuraIndex = AuraIndex AndAlso (ActiveSpells(i).Aura_Info(j).MiscValue And Mask) = Mask Then
                            Modifier += ActiveSpells(i).Aura_Info(j).GetValue(Level)
                        End If
                    Next
                End If
            Next
            Return Modifier
        End Function

        Public Sub AddAura(ByVal SpellID As Integer, ByVal Duration As Integer, ByRef Caster As BaseUnit)
            Dim AuraStart As Integer = 0
            Dim AuraEnd As Integer = MAX_POSITIVE_AURA_EFFECTs - 1
            If SPELLs(SpellID).IsPassive Then
                AuraStart = MAX_AURA_EFFECTs_VISIBLE
                AuraEnd = MAX_AURA_EFFECTs
            ElseIf SPELLs(SpellID).IsNegative Then
                AuraStart = MAX_POSITIVE_AURA_EFFECTs
                AuraEnd = MAX_AURA_EFFECTs_VISIBLE - 1
            End If

            'Try to remove spells that can't be used at the same time as this one
            Try
                If Not SPELLs(SpellID).IsPassive Then
                    Dim SpellInfo As SpellInfo = SPELLs(SpellID)
                    For slot As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                        If ActiveSpells(slot) IsNot Nothing AndAlso ActiveSpells(slot).GetSpellInfo.Target = SpellInfo.Target AndAlso
                            ActiveSpells(slot).GetSpellInfo.Category = SpellInfo.Category AndAlso ActiveSpells(slot).GetSpellInfo.SpellIconID = SpellInfo.SpellIconID AndAlso
                            ActiveSpells(slot).GetSpellInfo.SpellVisual = SpellInfo.SpellVisual AndAlso ActiveSpells(slot).GetSpellInfo.Attributes = SpellInfo.Attributes AndAlso
                            ActiveSpells(slot).GetSpellInfo.AttributesEx = SpellInfo.AttributesEx AndAlso ActiveSpells(slot).GetSpellInfo.AttributesEx2 = SpellInfo.AttributesEx2 Then
                            RemoveAura(slot, ActiveSpells(slot).SpellCaster)
                        End If
                    Next
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "ERROR ADDING AURA!{0}{1}", Environment.NewLine, ex.ToString)
            End Try

            For slot As Integer = AuraStart To AuraEnd
                If ActiveSpells(slot) Is Nothing Then
                    'DONE: Adding New SpellAura
                    ActiveSpells(slot) = New BaseActiveSpell(SpellID, Duration) With {
                        .SpellCaster = Caster
                    }

                    If slot < MAX_AURA_EFFECTs_VISIBLE Then SetAura(SpellID, slot, Duration)
                    Exit For
                End If
            Next

            If TypeOf Me Is CharacterObject Then
                CType(Me, CharacterObject).GroupUpdateFlag = CType(Me, CharacterObject).GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_AURAS
            ElseIf (TypeOf Me Is PetObject) AndAlso (TypeOf CType(Me, PetObject).Owner Is CharacterObject) Then
                CType(CType(Me, PetObject).Owner, CharacterObject).GroupUpdateFlag = CType(CType(Me, PetObject).Owner, CharacterObject).GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_AURAS
            End If
        End Sub

        Public Sub UpdateAura(ByVal Slot As Integer)
            If ActiveSpells(Slot) Is Nothing Then Exit Sub
            If Slot >= MAX_AURA_EFFECTs_VISIBLE Then Exit Sub

            Dim AuraFlag_Slot As Integer = Slot \ 4
            Dim AuraFlag_SubSlot As Integer = (Slot Mod 4) * 8
            SetAuraStackCount(Slot, ActiveSpells(Slot).StackCount)

            If TypeOf Me Is CharacterObject Then
                CType(Me, CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAAPPLICATIONS + AuraFlag_Slot, ActiveSpells_Count(AuraFlag_Slot))
                CType(Me, CharacterObject).SendCharacterUpdate(True)

                Dim SMSG_UPDATE_AURA_DURATION As New PacketClass(OPCODES.SMSG_UPDATE_AURA_DURATION)
                Try
                    SMSG_UPDATE_AURA_DURATION.AddInt8(Slot)
                    SMSG_UPDATE_AURA_DURATION.AddInt32(ActiveSpells(Slot).SpellDuration)
                    CType(Me, CharacterObject).client.Send(SMSG_UPDATE_AURA_DURATION)
                Finally
                    SMSG_UPDATE_AURA_DURATION.Dispose()
                End Try
            Else
                Dim tmpUpdate As New UpdateClass
                Dim tmpPacket As New UpdatePacketClass
                Try
                    tmpUpdate.SetUpdateFlag(EUnitFields.UNIT_FIELD_AURAAPPLICATIONS + AuraFlag_Slot, ActiveSpells_Count(AuraFlag_Slot))
                    tmpUpdate.AddToPacket(tmpPacket, ObjectUpdateType.UPDATETYPE_VALUES, CType(Me, CreatureObject))
                    SendToNearPlayers(tmpPacket)
                Finally
                    tmpPacket.Dispose()
                    tmpUpdate.Dispose()
                End Try
            End If
        End Sub

        Public Sub DoEmote(ByVal EmoteID As Integer)
            Dim packet As New PacketClass(OPCODES.SMSG_EMOTE)
            Try
                packet.AddInt32(EmoteID)
                packet.AddUInt64(GUID)
                SendToNearPlayers(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub DealSpellDamage(ByRef Caster As BaseUnit, ByRef EffectInfo As SpellEffect, ByVal SpellID As Integer, ByVal Damage As Integer, ByVal DamageType As DamageTypes, ByVal SpellType As SpellType)
            Dim IsHeal As Boolean = False
            Dim IsDot As Boolean = False

            Select Case SpellType
                Case SpellType.SPELL_TYPE_HEAL
                    IsHeal = True
                Case SpellType.SPELL_TYPE_HEALDOT
                    IsHeal = True
                    IsDot = True
                Case SpellType.SPELL_TYPE_DOT
                    IsDot = True
            End Select
            'If SpellType = SpellType.SPELL_TYPE_HEAL Or SpellType = SpellType.SPELL_TYPE_HEALDOT Then
            '    IsHeal = True
            'End If
            'If SpellType = SpellType.SPELL_TYPE_DOT OrElse SpellType = SpellType.SPELL_TYPE_HEALDOT Then
            '    IsDot = True
            'End If

            Dim SpellDamageBenefit As Integer = 0
            If TypeOf Caster Is CharacterObject Then
                With CType(Caster, CharacterObject)
                    Dim PenaltyFactor As Integer = 0
                    Dim EffectCount As Integer = 0
                    For i As Integer = 0 To 2
                        If SPELLs(SpellID).SpellEffects(i) IsNot Nothing Then EffectCount += 1
                    Next
                    If EffectCount > 1 Then PenaltyFactor = 5

                    Dim SpellDamage As Integer = 0
                    If IsHeal Then
                        SpellDamage = CType(Caster, CharacterObject).healing.Value
                    Else
                        SpellDamage = CType(Caster, CharacterObject).spellDamage(DamageType).Value
                    End If

                    If IsDot Then
                        Dim TickAmount As Integer = SPELLs(SpellID).GetDuration / EffectInfo.Amplitude
                        If TickAmount < 5 Then TickAmount = 5
                        SpellDamageBenefit = SpellDamage \ TickAmount
                    Else
                        Dim CastTime As Integer = SPELLs(SpellID).GetCastTime
                        If CastTime < 1500 Then CastTime = 1500
                        If CastTime > 3500 Then CastTime = 3500
                        SpellDamageBenefit = Fix(SpellDamage * (CastTime / 1000.0F) * ((100 - PenaltyFactor) / 100) / 3.5F)
                    End If
                    If SPELLs(SpellID).IsAOE Then SpellDamageBenefit \= 3
                End With
            End If
            Damage += SpellDamageBenefit

            'TODO: Crit
            Dim IsCrit As Boolean = False
            If Not IsDot Then
                If TypeOf Caster Is CharacterObject Then
                    'TODO: Get crit with only the same spell school
                    If RollChance(CType(Caster, CharacterObject).GetCriticalWithSpells) Then
                        Damage = Fix(1.5F * Damage)
                        IsCrit = True
                    End If
                End If
            End If

            Dim Resist As Integer = 0
            Dim Absorb As Integer = 0
            If Not IsHeal Then
                'DONE: Damage reduction
                Dim DamageReduction As Single = GetDamageReduction(Caster, DamageType, Damage)
                Damage -= Damage * DamageReduction

                'DONE: Resist
                If Damage > 0 Then
                    Resist = GetResist(Caster, DamageType, Damage)
                    If Resist > 0 Then Damage -= Resist
                End If

                'DONE: Absorb
                If Damage > 0 Then
                    Absorb = GetAbsorb(DamageType, Damage)
                    If Absorb > 0 Then Damage -= Absorb
                End If

                DealDamage(Damage, Caster)
            Else
                Heal(Damage, Caster)
            End If

            'DONE: Send log
            Select Case SpellType
                Case SpellType.SPELL_TYPE_NONMELEE
                    SendNonMeleeDamageLog(Caster, Me, SpellID, DamageType, Damage, Resist, Absorb, IsCrit)
                Case SpellType.SPELL_TYPE_DOT
                    SendPeriodicAuraLog(Caster, Me, SpellID, DamageType, Damage, EffectInfo.ApplyAuraIndex)
                Case SpellType.SPELL_TYPE_HEAL
                    SendHealSpellLog(Caster, Me, SpellID, Damage, IsCrit)
                Case SpellType.SPELL_TYPE_HEALDOT
                    SendPeriodicAuraLog(Caster, Me, SpellID, DamageType, Damage, EffectInfo.ApplyAuraIndex)
            End Select
        End Sub

        Public Function GetMagicSpellHitResult(ByRef Caster As BaseUnit, ByVal Spell As SpellInfo) As SpellMissInfo
            If IsDead Then Return SpellMissInfo.SPELL_MISS_NONE 'Can't miss dead target

            Dim lchance As Integer = If((TypeOf Me Is CharacterObject), 7, 11)
            Dim leveldiff As Integer = Level - CInt(Caster.Level)
            Dim modHitChance As Integer = 0
            If leveldiff < 3 Then
                modHitChance = 96 - leveldiff
            Else
                modHitChance = 94 - (leveldiff - 2) * lchance
            End If

            'Increase from attacker SPELL_AURA_MOD_INCREASES_SPELL_PCT_TO_HIT auras
            modHitChance += Caster.GetAuraModifierByMiscMask(AuraEffects_Names.SPELL_AURA_MOD_INCREASES_SPELL_PCT_TO_HIT, Spell.SchoolMask)

            'Chance hit from victim SPELL_AURA_MOD_ATTACKER_SPELL_HIT_CHANCE auras
            modHitChance += GetAuraModifierByMiscMask(AuraEffects_Names.SPELL_AURA_MOD_ATTACKER_SPELL_HIT_CHANCE, Spell.SchoolMask)

            'Reduce spell hit chance for Area of effect spells from victim SPELL_AURA_MOD_AOE_AVOIDANCE aura
            If Spell.IsAOE Then modHitChance -= GetAuraModifier(AuraEffects_Names.SPELL_AURA_MOD_AOE_AVOIDANCE)

            'Reduce spell hit chance for dispel mechanic spells from victim SPELL_AURA_MOD_DISPEL_RESIST
            If Spell.IsDispell Then modHitChance -= GetAuraModifier(AuraEffects_Names.SPELL_AURA_MOD_DISPEL_RESIST)

            'Chance resist mechanic (select max value from every mechanic spell effect)
            Dim resist_mech As Integer = 0
            If Spell.Mechanic > 0 Then
                resist_mech = GetAuraModifierByMiscMask(AuraEffects_Names.SPELL_AURA_MOD_MECHANIC_RESISTANCE, Spell.Mechanic)
            End If
            For i As Integer = 0 To 2
                If Spell.SpellEffects(i) IsNot Nothing AndAlso Spell.SpellEffects(i).Mechanic > 0 Then
                    Dim temp As Integer = GetAuraModifierByMiscMask(AuraEffects_Names.SPELL_AURA_MOD_MECHANIC_RESISTANCE, Spell.SpellEffects(i).Mechanic)
                    If resist_mech < temp Then resist_mech = temp
                End If
            Next
            modHitChance -= resist_mech

            'Chance resist debuff
            modHitChance -= GetAuraModifierByMiscMask(AuraEffects_Names.SPELL_AURA_MOD_DEBUFF_RESISTANCE, Spell.DispellType)

            Dim HitChance As Integer = modHitChance * 100
            'Increase hit chance from attacker SPELL_AURA_MOD_SPELL_HIT_CHANCE and attacker ratings
            'HitChance += int32(m_modSpellHitChance*100.0f);

            If HitChance < 100 Then
                HitChance = 100
            ElseIf HitChance > 10000 Then
                HitChance = 10000
            End If

            Dim tmp As Integer = 10000 - HitChance
            Dim rand As Integer = Rnd.Next(0, 10001)

            If rand < tmp Then Return SpellMissInfo.SPELL_MISS_RESIST

            Return SpellMissInfo.SPELL_MISS_NONE
        End Function

        Public Function GetMeleeSpellHitResult(ByRef Caster As BaseUnit, ByVal Spell As SpellInfo) As SpellMissInfo
            Dim attType As WeaponAttackType = WeaponAttackType.BASE_ATTACK
            If Spell.DamageType = SpellDamageType.SPELL_DMG_TYPE_RANGED Then attType = WeaponAttackType.RANGED_ATTACK

            'bonus from skills is 0.04% per skill Diff
            Dim attackerWeaponSkill As Integer = Caster.GetWeaponSkill(attType, Me)
            Dim skillDiff As Integer = attackerWeaponSkill - (Level * 5)
            Dim fullSkillDiff As Integer = attackerWeaponSkill - GetDefenceSkill(Caster)

            Dim roll As Integer = Rnd.Next(0, 10001)
            Dim missChance As Integer = Fix(0.0F * 100.0F)

            'Roll miss
            Dim tmp As Integer = missChance
            If roll < tmp Then Return SpellMissInfo.SPELL_MISS_MISS

            'Chance resist mechanic (select max value from every mechanic spell effect)
            Dim resist_mech As Integer = 0
            If Spell.Mechanic > 0 Then
                resist_mech = GetAuraModifierByMiscMask(AuraEffects_Names.SPELL_AURA_MOD_MECHANIC_RESISTANCE, Spell.Mechanic)
            End If
            For i As Integer = 0 To 2
                If Spell.SpellEffects(i) IsNot Nothing AndAlso Spell.SpellEffects(i).Mechanic > 0 Then
                    Dim temp As Integer = GetAuraModifierByMiscMask(AuraEffects_Names.SPELL_AURA_MOD_MECHANIC_RESISTANCE, Spell.SpellEffects(i).Mechanic)
                    If resist_mech < temp Then resist_mech = temp
                End If
            Next
            tmp += resist_mech
            If roll < tmp Then Return SpellMissInfo.SPELL_MISS_RESIST

            'Same spells cannot be parry/dodge
            If (Spell.Attributes And SpellAttributes.SPELL_ATTR_CANT_BLOCK) Then Return SpellMissInfo.SPELL_MISS_NONE

            'TODO: Dodge and parry!

            Return SpellMissInfo.SPELL_MISS_NONE
        End Function

        Public Function GetDefenceSkill(ByRef Attacker As BaseUnit) As Integer
            If TypeOf Me Is CharacterObject Then
                Dim value As Integer = 0

                With CType(Me, CharacterObject)
                    'in PvP use full skill instead current skill value
                    If Attacker.IsPlayer Then
                        value = .Skills(SKILL_IDs.SKILL_DEFENSE).MaximumWithBonus
                    Else
                        value = .Skills(SKILL_IDs.SKILL_DEFENSE).CurrentWithBonus
                    End If
                End With

                Return value
            Else
                Return Level * 5
            End If
        End Function

        Public Function GetWeaponSkill(ByVal attType As WeaponAttackType, ByRef Victim As BaseUnit) As Integer
            If TypeOf Me Is CharacterObject Then
                Dim value As Integer = 0

                With CType(Me, CharacterObject)
                    Dim item As ItemObject = Nothing
                    Select Case attType
                        Case WeaponAttackType.BASE_ATTACK
                            If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND) Then item = .Items(EquipmentSlots.EQUIPMENT_SLOT_MAINHAND)
                        Case WeaponAttackType.OFF_ATTACK
                            If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND) Then item = .Items(EquipmentSlots.EQUIPMENT_SLOT_OFFHAND)
                        Case WeaponAttackType.RANGED_ATTACK
                            If .Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) Then item = .Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED)
                    End Select

                    'feral or unarmed skill only for base attack
                    If attType <> WeaponAttackType.BASE_ATTACK AndAlso item Is Nothing Then Return 0

                    'always maximized SKILL_FERAL_COMBAT in fact
                    If IsInFeralForm Then Return Level * 5

                    'weapon skill or (unarmed for base attack)
                    Dim skill As Integer = If((item Is Nothing), SKILL_IDs.SKILL_UNARMED, item.GetSkill)

                    'in PvP use full skill instead current skill value
                    If Victim.IsPlayer Then
                        value = .Skills(skill).MaximumWithBonus
                    Else
                        value = .Skills(skill).CurrentWithBonus
                    End If
                End With

                Return value
            Else
                Return Level * 5
            End If
        End Function

        Public Function GetDamageReduction(ByRef t As BaseUnit, ByVal School As DamageTypes, ByVal Damage As Integer) As Single
            Dim DamageReduction As Single = 0.0F

            If School = DamageTypes.DMG_PHYSICAL Then
                DamageReduction = (Resistances(0).Base / (Resistances(0).Base + 400 + 85 * Level))
            Else
                Dim effectiveResistanceRating As Integer = t.Resistances(School).Base + Math.Max((t.Level - CInt(Level)) * 5, 0)
                DamageReduction = (effectiveResistanceRating / (Level * 5)) * 0.75F
            End If

            If DamageReduction > 0.75F Then
                DamageReduction = 0.75F
            ElseIf DamageReduction < 0.0F Then
                DamageReduction = 0.0F
            End If
            Return DamageReduction
        End Function

        Public Function GetResist(ByRef t As BaseUnit, ByVal School As DamageTypes, ByVal Damage As Integer) As Single
            Dim damageReduction As Single = GetDamageReduction(t, School, Damage)

            'DONE: Partial resist
            Dim partialChances() As Integer
            If damageReduction < 0.15F Then
                partialChances = New Integer() {33, 11, 2, 0}
            ElseIf damageReduction < 0.3F Then
                partialChances = New Integer() {49, 24, 6, 1}
            ElseIf damageReduction < 0.45F Then
                partialChances = New Integer() {26, 48, 18, 1}
            ElseIf damageReduction < 0.6F Then
                partialChances = New Integer() {14, 40, 34, 11}
            Else
                partialChances = New Integer() {3, 16, 55, 25}
            End If

            Dim ran As Integer = Rnd.Next(0, 101)
            Dim m As Integer = 0
            Dim val As Integer = 0
            For i As Integer = 0 To 3
                val += partialChances(i)
                If ran > val Then
                    m += 1
                Else
                    Exit For
                End If
            Next

            If m = 0 Then
                Return 0
            ElseIf m = 4 Then
                Return Damage
            Else
                Return (Damage * m) / 4
            End If
        End Function

        Public Function GetAbsorb(ByVal School As DamageTypes, ByVal Damage As Integer) As Integer
            Dim ListChange As New Dictionary(Of Integer, UInteger)
            Dim StartDmg As Integer = Damage

            Log.WriteLine(LogType.DEBUG, "Damage: {0} [{1}]", Damage, School)

            For Each tmpSpell As KeyValuePair(Of Integer, UInteger) In AbsorbSpellLeft
                Dim Schools As Integer = (tmpSpell.Value >> 23UI)
                Dim AbsorbDamage As Integer = tmpSpell.Value And &H7FFFFF

                Log.WriteLine(LogType.DEBUG, "Spell: {0} [{1}]", AbsorbDamage, Schools)

                If HaveFlag(Schools, School) Then
                    Log.WriteLine(LogType.DEBUG, "Apmongo, yes?!")
                    If Damage = AbsorbDamage Then
                        ListChange.Add(tmpSpell.Key, 0)
                        Damage = 0
                        Exit For
                    ElseIf Damage > AbsorbDamage Then
                        ListChange.Add(tmpSpell.Key, 0)
                        Damage -= AbsorbDamage
                    Else
                        AbsorbDamage -= Damage
                        Damage = 0
                        ListChange.Add(tmpSpell.Key, AbsorbDamage)
                        Exit For
                    End If
                ElseIf (Schools And (1 << School)) Then
                    Throw New Exception("AHA?!")
                End If
            Next

            'First remove
            For Each Change As KeyValuePair(Of Integer, UInteger) In ListChange
                If Change.Value = 0 Then
                    RemoveAuraBySpell(Change.Key)
                    If AbsorbSpellLeft.ContainsKey(Change.Key) Then AbsorbSpellLeft.Remove(Change.Key)
                End If
            Next

            'And then change
            For Each Change As KeyValuePair(Of Integer, UInteger) In ListChange
                If Change.Value <> 0 Then
                    AbsorbSpellLeft(Change.Key) = Change.Value
                End If
            Next

            Log.WriteLine(LogType.DEBUG, "Absorbed: {0}", StartDmg - Damage)
            Return StartDmg - Damage
        End Function

        Public Sub New()
            For i As Byte = DamageTypes.DMG_PHYSICAL To DamageTypes.DMG_ARCANE
                Resistances(i) = New TStat
            Next
        End Sub
    End Class

    Public Class BaseActiveSpell
        Public SpellID As Integer = 0
        Public SpellDuration As Integer = 0
        Public SpellCaster As BaseUnit = Nothing

        Public Flags As Byte = 0
        Public Level As Byte = 0
        Public StackCount As Integer = 0

        Public Values() As Integer = {0, 0, 0}

        Public Aura() As ApplyAuraHandler = {Nothing, Nothing, Nothing}
        Public Aura_Info() As SpellEffect = {Nothing, Nothing, Nothing}

        Public Sub New(ByVal ID As Integer, ByVal Duration As Integer)
            SpellID = ID
            SpellDuration = Duration
        End Sub
        Public ReadOnly Property GetSpellInfo() As SpellInfo
            Get
                Return SPELLs(SpellID)
            End Get
        End Property
    End Class
End Module