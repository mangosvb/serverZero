'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
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

Imports System.Threading
Imports System.Runtime.CompilerServices
Imports mangosVB.Common.BaseWriter

Public Module WS_GameObjects

#Region "WS.GameObjects.TypeDef"

    'WARNING: Use only with GAMEOBJECTSDatabase()
    Public Class GameObjectInfo
        Implements IDisposable

        Public ID As Integer = 0
        Public Model As Integer = 0
        Public Type As GameObjectType = 0
        Public Name As String = ""
        Public Faction As Short = 0
        Public Flags As Integer = 0
        Public Size As Single = 1
        Public Fields(23) As UInteger
        Public ScriptName As String = ""
        Private found_ As Boolean = False

        Public Sub New(ByVal ID_ As Integer)
            ID = ID_
            GAMEOBJECTSDatabase.Add(ID, Me)

            Dim MySQLQuery As New DataTable
            WorldDatabase.Query(String.Format("SELECT * FROM gameobjects WHERE entry = {0};", ID_), MySQLQuery)

            If MySQLQuery.Rows.Count = 0 Then
                Log.WriteLine(LogType.FAILED, "GameObject {0} not found in SQL database!", ID_)
                found_ = False
                Exit Sub
            End If
            found_ = True

            Model = MySQLQuery.Rows(0).Item("displayId")
            Type = MySQLQuery.Rows(0).Item("type")
            Name = MySQLQuery.Rows(0).Item("name")
            Faction = MySQLQuery.Rows(0).Item("faction")
            Flags = MySQLQuery.Rows(0).Item("flags")
            Size = MySQLQuery.Rows(0).Item("size")

            For i As Byte = 0 To 23
                Fields(i) = MySQLQuery.Rows(0).Item("data" & i)
            Next

            ScriptName = MySQLQuery.Rows(0).Item("ScriptName")
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                GAMEOBJECTSDatabase.Remove(ID)
            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
    'WARNING: Use only with WORLD_GAMEOBJECTs()
    Public Class GameObjectObject
        Inherits BaseObject
        Implements IDisposable

        Public ID As Integer = 0
        Public Flags As Integer = 0
        Public Size As Single = 1
        Public Faction As Integer = 0
        Public State As GameObjectLootState = GameObjectLootState.LOOT_UNLOOTED
        Public Rotations() As Single = {0, 0, 0, 0}
        Public Owner As ULong
        Public Loot As LootObject = Nothing
        Public Despawned As Boolean = False
        Public MineRemaining As Integer = 0
        Public AnimProgress As Integer = 0
        Public SpawnTime As Integer = 0
        Public GameEvent As Integer = 0
        Public CreatedBySpell As Integer = 0
        Public Level As Integer = 0
        Private ToDespawn As Boolean = False

        Public IncludesQuestItems As New List(Of Integer)

        Private RespawnTimer As Threading.Timer = Nothing

        Public ReadOnly Property ObjectInfo() As GameObjectInfo
            Get
                Return GAMEOBJECTSDatabase(ID)
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return ObjectInfo.Name
            End Get
        End Property
        Public ReadOnly Property Type() As GameObjectType
            Get
                Return ObjectInfo.Type
            End Get
        End Property
        Public ReadOnly Property Sound(ByVal Index As Byte) As UInteger
            Get
                Return ObjectInfo.Fields(Index)
            End Get
        End Property
        Public ReadOnly Property IsUsedForQuests() As Boolean
            Get
                Return (IncludesQuestItems.Count > 0)
            End Get
        End Property

        Public ReadOnly Property LockID() As Integer
            Get
                Select Case ObjectInfo.Type
                    Case GameObjectType.GAMEOBJECT_TYPE_DOOR
                        Return Sound(1)
                    Case GameObjectType.GAMEOBJECT_TYPE_BUTTON
                        Return Sound(1)
                    Case GameObjectType.GAMEOBJECT_TYPE_QUESTGIVER
                        Return Sound(0)
                    Case GameObjectType.GAMEOBJECT_TYPE_CHEST
                        Return Sound(0)
                    Case GameObjectType.GAMEOBJECT_TYPE_TRAP
                        Return Sound(0)
                    Case GameObjectType.GAMEOBJECT_TYPE_GOOBER
                        Return Sound(0)
                    Case GameObjectType.GAMEOBJECT_TYPE_AREADAMAGE
                        Return Sound(0)
                    Case GameObjectType.GAMEOBJECT_TYPE_CAMERA
                        Return Sound(0)
                    Case GameObjectType.GAMEOBJECT_TYPE_FLAGSTAND
                        Return Sound(0)
                    Case GameObjectType.GAMEOBJECT_TYPE_FISHINGHOLE
                        Return Sound(4)
                    Case GameObjectType.GAMEOBJECT_TYPE_FLAGDROP
                        Return Sound(0)
                    Case Else
                        Return 0
                End Select
            End Get
        End Property
        Public ReadOnly Property LootID() As Integer
            Get
                Select Case ObjectInfo.Type
                    Case GameObjectType.GAMEOBJECT_TYPE_CHEST
                        Return Sound(1)
                    Case GameObjectType.GAMEOBJECT_TYPE_FISHINGNODE
                        Return Sound(1)
                    Case GameObjectType.GAMEOBJECT_TYPE_FISHINGHOLE
                        Return Sound(1)
                    Case Else
                        Return 0
                End Select
            End Get
        End Property
        Public ReadOnly Property AutoCloseTime() As Integer
            Get
                Select Case ObjectInfo.Type
                    Case GameObjectType.GAMEOBJECT_TYPE_DOOR
                        Return (Sound(2) / &H10000) * 1000
                    Case GameObjectType.GAMEOBJECT_TYPE_BUTTON
                        Return (Sound(2) / &H10000) * 1000
                    Case GameObjectType.GAMEOBJECT_TYPE_TRAP
                        Return (Sound(6) / &H10000) * 1000
                    Case GameObjectType.GAMEOBJECT_TYPE_GOOBER
                        Return (Sound(3) / &H10000) * 1000
                    Case GameObjectType.GAMEOBJECT_TYPE_TRANSPORT
                        Return (Sound(2) / &H10000) * 1000
                    Case GameObjectType.GAMEOBJECT_TYPE_AREADAMAGE
                        Return (Sound(5) / &H10000) * 1000
                    Case Else
                        Return 0
                End Select
            End Get
        End Property
        Public ReadOnly Property IsConsumeable() As Boolean
            Get
                Select Case ObjectInfo.Type
                    Case GameObjectType.GAMEOBJECT_TYPE_CHEST
                        Return (Sound(3) = 1)
                    Case Else
                        Return False
                End Select
            End Get
        End Property

        Public Overridable Sub FillAllUpdateFlags(ByRef Update As UpdateClass, ByRef Character As CharacterObject)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, CType(ObjectType.TYPE_GAMEOBJECT + ObjectType.TYPE_OBJECT, Integer))
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, ID)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, Size)

            If Owner > 0UL Then Update.SetUpdateFlag(EGameObjectFields.OBJECT_FIELD_CREATED_BY, Owner)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_POS_X, positionX)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_POS_Y, positionY)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_POS_Z, positionZ)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FACING, orientation)

            Dim Rotation As Long = 0
            Dim f_rot1 As Single = Math.Sin(orientation / 2)
            Dim i_rot1 As Long = f_rot1 / Math.Atan(Math.Pow(2, -20))
            Rotation = Rotation Or ((CLng(i_rot1) << CLng(43) >> CLng(43)) And &H1FFFFFL)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION, Rotation)

            'If a game object has bit 4 set in the flag it needs to be activated (used for quests)
            'DynFlags = Activate a game object (Chest = 9, Goober = 1)
            Dim DynFlags As Integer = 0
            If Type = GameObjectType.GAMEOBJECT_TYPE_CHEST Then
                Dim questSystem As New WS_Quests()
                Dim UsedForQuest As Byte = questSystem.IsGameObjectUsedForQuest(Me, Character)
                If UsedForQuest > 0 Then
                    Flags = Flags Or 4
                    If UsedForQuest = 2 Then
                        DynFlags = 9
                    End If
                End If
            ElseIf Type = GameObjectType.GAMEOBJECT_TYPE_GOOBER Then
                'TODO: Check conditions
                DynFlags = 1
            End If
            If DynFlags Then Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_DYN_FLAGS, DynFlags)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_STATE, 0, State)

            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_TYPE_ID, Type)
            If Level > 0 Then Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_LEVEL, Level)

            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FACTION, Faction)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FLAGS, Flags)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_DISPLAYID, ObjectInfo.Model)
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION, Rotations(0))
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 1, Rotations(1))
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 2, Rotations(2))
            Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 3, Rotations(3))
            'Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_TIMESTAMP, msTime) ' Changed in 1.12.x client branch?
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Me.RemoveFromWorld()
                If Not Loot Is Nothing AndAlso Type <> GameObjectType.GAMEOBJECT_TYPE_FISHINGNODE Then Loot.Dispose()
                If TypeOf Me Is TransportObject Then
                    WORLD_TRANSPORTs_Lock.AcquireWriterLock(Timeout.Infinite)
                    WORLD_TRANSPORTs.Remove(GUID)
                    WORLD_TRANSPORTs_Lock.ReleaseWriterLock()
                    RespawnTimer.Dispose()
                Else
                    WORLD_GAMEOBJECTs.Remove(GUID)
                End If
            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Public Sub New(ByVal ID_ As Integer)
            'WARNING: Use only for spawning new object
            If Not GAMEOBJECTSDatabase.ContainsKey(ID_) Then
                Dim baseGameObject As New GameObjectInfo(ID_)
            End If

            ID = ID_
            GUID = GetNewGUID()

            Flags = GAMEOBJECTSDatabase(ID).Flags
            Faction = GAMEOBJECTSDatabase(ID).Faction
            Size = GAMEOBJECTSDatabase(ID).Size

            WORLD_GAMEOBJECTs.Add(GUID, Me)
        End Sub
        Public Sub New(ByVal ID_ As Integer, ByVal GUID_ As ULong)
            'WARNING: Use only for spawning new object
            If Not GAMEOBJECTSDatabase.ContainsKey(ID_) Then
                Dim baseGameObject As New GameObjectInfo(ID_)
            End If

            ID = ID_
            GUID = GUID_

            Flags = GAMEOBJECTSDatabase(ID).Flags
            Faction = GAMEOBJECTSDatabase(ID).Faction
            Size = GAMEOBJECTSDatabase(ID).Size
        End Sub
        Public Sub New(ByVal ID_ As Integer, ByVal MapID_ As UInteger, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Rotation As Single, Optional ByVal Owner_ As ULong = 0)
            'WARNING: Use only for spawning new object
            If Not GAMEOBJECTSDatabase.ContainsKey(ID_) Then
                Dim baseGameObject As New GameObjectInfo(ID_)
            End If

            ID = ID_
            GUID = GetNewGUID()
            MapID = MapID_
            positionX = PosX
            positionY = PosY
            positionZ = PosZ
            orientation = Rotation
            Owner = Owner_

            Flags = GAMEOBJECTSDatabase(ID).Flags
            Faction = GAMEOBJECTSDatabase(ID).Faction
            Size = GAMEOBJECTSDatabase(ID).Size

            If Type = GameObjectType.GAMEOBJECT_TYPE_TRANSPORT Then
                VisibleDistance = 99999.0F
                State = GameObjectLootState.DOOR_CLOSED
            End If

            WORLD_GAMEOBJECTs.Add(GUID, Me)
        End Sub
        Public Sub New(ByVal cGUID As ULong, Optional ByRef Info As DataRow = Nothing)
            'WARNING: Use only for loading from DB
            If Info Is Nothing Then
                Dim MySQLQuery As New DataTable
                WorldDatabase.Query(String.Format("SELECT * FROM spawns_gameobjects LEFT OUTER JOIN game_event_gameobject ON spawns_gameobjects.spawn_id = game_event_gameobject.guid WHERE spawn_id = {0};", cGUID), MySQLQuery)
                If MySQLQuery.Rows.Count > 0 Then
                    Info = MySQLQuery.Rows(0)
                Else
                    Log.WriteLine(LogType.FAILED, "GameObject Spawn not found in database. [cGUID={0:X}]", cGUID)
                    Exit Sub
                End If
            End If

            positionX = Info.Item("spawn_positionX")
            positionY = Info.Item("spawn_positionY")
            positionZ = Info.Item("spawn_positionZ")
            orientation = Info.Item("spawn_orientation")
            MapID = Info.Item("spawn_map")

            Rotations(0) = Info.Item("spawn_rotation0")
            Rotations(1) = Info.Item("spawn_rotation1")
            Rotations(2) = Info.Item("spawn_rotation2")
            Rotations(3) = Info.Item("spawn_rotation3")

            ID = Info.Item("spawn_entry")
            AnimProgress = Info.Item("spawn_animprogress")
            SpawnTime = Info.Item("spawn_spawntime")
            State = Info.Item("spawn_state")

            If Not Info.Item("event") Is DBNull.Value Then
                GameEvent = Info.Item("event")
            Else
                GameEvent = 0
            End If

            If Not GAMEOBJECTSDatabase.ContainsKey(ID) Then
                Dim baseGameObject As New GameObjectInfo(ID)
            End If

            Flags = GAMEOBJECTSDatabase(ID).Flags
            Faction = GAMEOBJECTSDatabase(ID).Faction
            Size = GAMEOBJECTSDatabase(ID).Size

            If Type = GameObjectType.GAMEOBJECT_TYPE_TRANSPORT Then
                'State = GameObjectLootState.DOOR_CLOSED
                VisibleDistance = 99999.0F
                GUID = cGUID + GUID_TRANSPORT
            Else
                GUID = cGUID + GUID_GAMEOBJECT
            End If
            WORLD_GAMEOBJECTs.Add(GUID, Me)

            'DONE: If there's a loottable open for this gameobject already then hook it to the gameobject
            If LootTable.ContainsKey(GUID) Then
                Loot = CType(LootTable(GUID), LootObject)
            End If

            'DONE: Calculate mines remaining
            CalculateMineRemaning(True)
        End Sub

        Public Sub AddToWorld()
            GetMapTile(positionX, positionY, CellX, CellY)
            If Maps(MapID).Tiles(CellX, CellY) Is Nothing Then MAP_Load(CellX, CellY, MapID)
            Try
                Maps(MapID).Tiles(CellX, CellY).GameObjectsHere.Add(GUID)
            Catch
                Exit Sub
            End Try

            Dim list() As ULong

            'DONE: Generate loot at spawn
            If Type = GameObjectType.GAMEOBJECT_TYPE_CHEST AndAlso Loot Is Nothing Then GenerateLoot()

            'DONE: Sending to players in nearby cells
            For i As Short = -1 To 1
                For j As Short = -1 To 1
                    If (CellX + i) >= 0 AndAlso (CellX + i) <= 63 AndAlso (CellY + j) >= 0 AndAlso (CellY + j) <= 63 AndAlso Maps(MapID).Tiles(CellX + i, CellY + j) IsNot Nothing AndAlso Maps(MapID).Tiles(CellX + i, CellY + j).PlayersHere.Count > 0 Then
                        With Maps(MapID).Tiles(CellX + i, CellY + j)
                            list = .PlayersHere.ToArray
                            For Each plGUID As ULong In list
                                If CHARACTERs.ContainsKey(plGUID) AndAlso CHARACTERs(plGUID).CanSee(Me) Then
                                    Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                                    packet.AddInt32(1)
                                    packet.AddInt8(0)
                                    Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
                                    FillAllUpdateFlags(tmpUpdate, CHARACTERs(plGUID))
                                    tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, Me)
                                    tmpUpdate.Dispose()

                                    CHARACTERs(plGUID).Client.SendMultiplyPackets(packet)
                                    CHARACTERs(plGUID).gameObjectsNear.Add(GUID)
                                    SeenBy.Add(plGUID)

                                    packet.Dispose()
                                End If
                            Next
                        End With
                    End If
                Next
            Next

        End Sub
        Public Sub RemoveFromWorld()
            If Maps(MapID).Tiles(CellX, CellY) Is Nothing Then Exit Sub
            GetMapTile(positionX, positionY, CellX, CellY)
            Maps(MapID).Tiles(CellX, CellY).GameObjectsHere.Remove(GUID)

            'DONE: Removing from players that can see the object
            For Each plGUID As ULong In SeenBy.ToArray
                If CHARACTERs(plGUID).gameObjectsNear.Contains(GUID) Then
                    CHARACTERs(plGUID).guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                    CHARACTERs(plGUID).guidsForRemoving.Add(GUID)
                    CHARACTERs(plGUID).guidsForRemoving_Lock.ReleaseWriterLock()

                    CHARACTERs(plGUID).gameObjectsNear.Remove(GUID)
                End If
            Next
        End Sub

        Public Sub SetState(ByVal State As GameObjectLootState)
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)
            packet.AddInt8(0)
            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
            tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_STATE, 0, State)
            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
            tmpUpdate.Dispose()
            SendToNearPlayers(packet)
            packet.Dispose()
        End Sub

        Public Sub OpenDoor()
            Flags = Flags Or GameObjectFlags.GO_FLAG_IN_USE
            State = GameObjectLootState.DOOR_OPEN

            Log.WriteLine(LogType.DEBUG, "AutoCloseTime: {0}", AutoCloseTime)
            If AutoCloseTime > 0 Then ThreadPool.RegisterWaitForSingleObject(New AutoResetEvent(False), New WaitOrTimerCallback(AddressOf CloseDoor), Nothing, AutoCloseTime, True)

            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)
            packet.AddInt8(0)
            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
            tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FLAGS, Flags)
            tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_STATE, 0, State)
            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
            tmpUpdate.Dispose()
            SendToNearPlayers(packet)
            packet.Dispose()
        End Sub
        Public Sub CloseDoor()
            Flags = Flags And (Not GameObjectFlags.GO_FLAG_IN_USE)
            State = GameObjectLootState.DOOR_CLOSED

            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)
            packet.AddInt8(0)
            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
            tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FLAGS, Flags)
            tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_STATE, 0, State)
            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
            tmpUpdate.Dispose()
            SendToNearPlayers(packet)
            packet.Dispose()
        End Sub
        Public Sub LootObject(ByRef Character As CharacterObject, ByVal LootingType As LootType)
            State = GameObjectLootState.LOOT_LOOTED
            Select Case Type
                Case GameObjectType.GAMEOBJECT_TYPE_BUTTON, GameObjectType.GAMEOBJECT_TYPE_DOOR
                    'DONE: Open door
                    OpenDoor()
                    'TODO: Close it again after some sec
                    Exit Sub
                Case GameObjectType.GAMEOBJECT_TYPE_QUESTGIVER
                    'TODO: Start or end quest
                    Exit Sub
            End Select

            If Loot Is Nothing Then Exit Sub
            Loot.SendLoot(Character.Client)

            'DONE: So that loot isn't released instantly for gameobject looting
            If Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL) IsNot Nothing Then
                Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL).State = SpellCastState.SPELL_STATE_FINISHED
            End If
        End Sub
        Public Function GenerateLoot() As Boolean
            If Not Loot Is Nothing Then Return True
            If LootID = 0 Then Return False

            'DONE: Loot generation
            Loot = New LootObject(GUID, LootType.LOOTTYPE_SKINNING)
            Dim Template As LootTemplate = LootTemplates_Gameobject.GetLoot(LootID)
            If Template IsNot Nothing Then
                Template.Process(Loot, 0)
            End If

            Loot.LootOwner = 0

            Return True
        End Function

        Public Sub SetupFishingNode()
            Dim RandomTime As Integer = Rnd.Next(3000, 17000)
            ThreadPool.RegisterWaitForSingleObject(New AutoResetEvent(False), New WaitOrTimerCallback(AddressOf SetFishHooked), Nothing, RandomTime, True)

            State = GameObjectLootState.DOOR_CLOSED
        End Sub

        Public Sub SetFishHooked()
            If State <> GameObjectLootState.DOOR_CLOSED Then Exit Sub

            State = GameObjectLootState.DOOR_OPEN
            Flags = GameObjectFlags.GO_FLAG_NODESPAWN

            Loot = New LootObject(GUID, LootType.LOOTTYPE_FISHING)
            Loot.LootOwner = Owner

            Dim AreaFlag As Integer = GetAreaFlag(positionX, positionY, MapID)
            Dim AreaID As Integer = AreaTable(AreaFlag).ID

            Dim Template As LootTemplate = LootTemplates_Fishing.GetLoot(AreaID)
            If Template IsNot Nothing Then
                Template.Process(Loot, 0)
            End If

            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)
            packet.AddInt8(0)
            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
            tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FLAGS, Flags)
            tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_STATE, 0, State)
            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
            tmpUpdate.Dispose()
            SendToNearPlayers(packet)
            packet.Dispose()

            Dim packetAnim As New PacketClass(OPCODES.SMSG_GAMEOBJECT_CUSTOM_ANIM)
            packetAnim.AddUInt64(GUID)
            packetAnim.AddInt32(0)
            SendToNearPlayers(packetAnim)
            packetAnim.Dispose()

            Dim FishEscapeTime As Integer = 2000
            ThreadPool.RegisterWaitForSingleObject(New AutoResetEvent(False), New WaitOrTimerCallback(AddressOf SetFishEscaped), Nothing, FishEscapeTime, True)
        End Sub

        Public Sub SetFishEscaped()
            If State <> GameObjectLootState.DOOR_OPEN Then Exit Sub

            State = GameObjectLootState.DOOR_CLOSED
            Flags = GameObjectFlags.GO_FLAG_LOCKED

            If Loot IsNot Nothing Then
                Loot.Dispose()
                Loot = Nothing
            End If

            If Owner > 0 AndAlso GuidIsPlayer(Owner) AndAlso CHARACTERs.ContainsKey(Owner) Then
                Dim fishEscaped As New PacketClass(OPCODES.SMSG_FISH_ESCAPED)
                CHARACTERs(Owner).Client.Send(fishEscaped)
                fishEscaped.Dispose()

                CHARACTERs(Owner).FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL, True)
            End If
        End Sub

        Public Sub CalculateMineRemaning(Optional ByVal Force As Boolean = False)
            If Type <> GameObjectType.GAMEOBJECT_TYPE_CHEST Then Exit Sub
            If Locks.ContainsKey(LockID) = False Then Exit Sub

            For i As Integer = 0 To 4
                If Locks(LockID).KeyType(i) = LockKeyType.LOCK_KEY_SKILL AndAlso (Locks(LockID).KeyType(i) = LockType.LOCKTYPE_MINING OrElse Locks(LockID).KeyType(i) = LockType.LOCKTYPE_HERBALISM) Then
                    If Force OrElse MineRemaining = 0 Then
                        MineRemaining = Rnd.Next(Sound(4), Sound(5) + 1)
                    End If
                    Exit Sub
                End If
            Next i
        End Sub

        Public Sub SpawnAnimation()
            Dim packet As New PacketClass(OPCODES.SMSG_GAMEOBJECT_SPAWN_ANIM)
            packet.AddUInt64(GUID)
            SendToNearPlayers(packet)
            packet.Dispose()
        End Sub
        Public Sub Respawn()
            Log.WriteLine(LogType.DEBUG, "Gameobject {0:X} respawning.", GUID)

            'DONE: Remove the timer
            If RespawnTimer IsNot Nothing Then
                RespawnTimer.Dispose()
                RespawnTimer = Nothing
                Despawned = False
            End If

            'DONE: Add to world
            Loot = Nothing
            State = GameObjectLootState.LOOT_UNLOOTED
            AddToWorld()

            'DONE: Recalculate mines remaining
            CalculateMineRemaning(True)
        End Sub
        Public Sub Despawn(Optional ByVal Delay As Integer = 0)
            If Delay = 0 Then
                Log.WriteLine(LogType.DEBUG, "Gameobject {0:X} despawning.", GUID)

                Dim packet As New PacketClass(OPCODES.SMSG_GAMEOBJECT_DESPAWN_ANIM)
                packet.AddUInt64(GUID)
                SendToNearPlayers(packet)
                packet.Dispose()

                'DONE: Remove from world
                Despawned = True
                If Not Loot Is Nothing Then Loot.Dispose()
                RemoveFromWorld()

                'DONE: Start the respawn timer
                If SpawnTime > 0 Then
                    RespawnTimer = New Threading.Timer(New TimerCallback(AddressOf Respawn), Nothing, SpawnTime, Timeout.Infinite)
                End If
            Else
                ToDespawn = True
                RespawnTimer = New Threading.Timer(New TimerCallback(AddressOf Destroy), Nothing, Delay, Timeout.Infinite)
            End If
        End Sub
        Public Sub Destroy()
            'DONE: Remove the timer
            If RespawnTimer IsNot Nothing Then
                RespawnTimer.Dispose()
                RespawnTimer = Nothing
                Despawned = False
            End If

            'DONE: If this gameobject were created by a player then remove the reference between them
            If CreatedBySpell > 0 AndAlso Owner > 0 AndAlso CHARACTERs.ContainsKey(Owner) Then
                If CHARACTERs(Owner).gameObjects.Contains(Me) Then CHARACTERs(Owner).gameObjects.Remove(Me)
            End If

            If ToDespawn Then
                ToDespawn = False

                Log.WriteLine(LogType.DEBUG, "Gameobject {0:X} despawning.", GUID)

                Dim despawnPacket As New PacketClass(OPCODES.SMSG_GAMEOBJECT_DESPAWN_ANIM)
                despawnPacket.AddUInt64(GUID)
                SendToNearPlayers(despawnPacket)
                despawnPacket.Dispose()
            End If

            Dim packet As New PacketClass(OPCODES.SMSG_DESTROY_OBJECT)
            packet.AddUInt64(GUID)
            SendToNearPlayers(packet)
            packet.Dispose()

            Me.Dispose()
        End Sub

        Public Sub TurnTo(ByRef Target As BaseObject)
            TurnTo(Target.positionX, Target.positionY)
        End Sub
        Public Sub TurnTo(ByVal x As Single, ByVal y As Single)
            orientation = GetOrientation(positionX, x, positionY, y)
            Rotations(2) = Math.Sin(orientation / 2)
            Rotations(3) = Math.Cos(orientation / 2)

            If SeenBy.Count > 0 Then

                'TODO: Rotation change is not visible with simple update
                Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                packet.AddInt32(2)
                packet.AddInt8(0)
                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
                tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FACING, orientation)
                tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION, Rotations(0))
                tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 1, Rotations(1))
                tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 2, Rotations(2))
                tmpUpdate.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 3, Rotations(3))
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
                tmpUpdate.Dispose()

                SendToNearPlayers(packet)
                packet.Dispose()
            End If
        End Sub
    End Class

    Public Enum GameObjectLootState As Byte
        DOOR_OPEN = 0
        DOOR_CLOSED = 1
        LOOT_UNAVIABLE = 0
        LOOT_UNLOOTED = 1
        LOOT_LOOTED = 2
    End Enum



#End Region
#Region "WS.GameObjects.HelperSubs"

    <MethodImplAttribute(MethodImplOptions.Synchronized)> _
    Private Function GetNewGUID() As ULong
        GameObjectsGUIDCounter += 1
        GetNewGUID = GameObjectsGUIDCounter
    End Function
    Public Enum GameObjectType As Byte
        GAMEOBJECT_TYPE_DOOR = 0
        GAMEOBJECT_TYPE_BUTTON = 1
        GAMEOBJECT_TYPE_QUESTGIVER = 2
        GAMEOBJECT_TYPE_CHEST = 3
        GAMEOBJECT_TYPE_BINDER = 4
        GAMEOBJECT_TYPE_GENERIC = 5
        GAMEOBJECT_TYPE_TRAP = 6
        GAMEOBJECT_TYPE_CHAIR = 7
        GAMEOBJECT_TYPE_SPELL_FOCUS = 8
        GAMEOBJECT_TYPE_TEXT = 9
        GAMEOBJECT_TYPE_GOOBER = 10
        GAMEOBJECT_TYPE_TRANSPORT = 11
        GAMEOBJECT_TYPE_AREADAMAGE = 12
        GAMEOBJECT_TYPE_CAMERA = 13
        GAMEOBJECT_TYPE_MAP_OBJECT = 14
        GAMEOBJECT_TYPE_MO_TRANSPORT = 15
        GAMEOBJECT_TYPE_DUEL_ARBITER = 16
        GAMEOBJECT_TYPE_FISHINGNODE = 17
        GAMEOBJECT_TYPE_RITUAL = 18
        GAMEOBJECT_TYPE_MAILBOX = 19
        GAMEOBJECT_TYPE_AUCTIONHOUSE = 20
        GAMEOBJECT_TYPE_GUARDPOST = 21
        GAMEOBJECT_TYPE_SPELLCASTER = 22
        GAMEOBJECT_TYPE_MEETINGSTONE = 23
        GAMEOBJECT_TYPE_FLAGSTAND = 24
        GAMEOBJECT_TYPE_FISHINGHOLE = 25
        GAMEOBJECT_TYPE_FLAGDROP = 26
        GAMEOBJECT_TYPE_MINI_GAME = 27
        GAMEOBJECT_TYPE_LOTTERY_KIOSK = 28
        GAMEOBJECT_TYPE_CAPTURE_POINT = 29
        GAMEOBJECT_TYPE_AURA_GENERATOR = 30
        GAMEOBJECT_TYPE_DUNGEON_DIFFICULTY = 31
        GAMEOBJECT_TYPE_BARBER_CHAIR = 32
        GAMEOBJECT_TYPE_DESTRUCTIBLE_BUILDING = 33
        GAMEOBJECT_TYPE_GUILD_BANK = 34
        GAMEOBJECT_TYPE_TRAP_DOOR = 35
    End Enum

    Public Enum GameObjectFlags
        GO_FLAG_IN_USE = &H1                        'disables interaction while animated
        GO_FLAG_LOCKED = &H2                        'require key, spell, event, etc to be opened. Makes "Locked" appear in tooltip
        GO_FLAG_INTERACT_COND = &H4                 'cannot interact (condition to interact)
        GO_FLAG_TRANSPORT = &H8                     'any kind of transport? Object can transport (elevator, boat, car)
        GO_FLAG_UNK1 = &H10                         '
        GO_FLAG_NODESPAWN = &H20                    'never despawn, typically for doors, they just change state
        GO_FLAG_TRIGGERED = &H40                    'typically, summoned objects. Triggered by spell or other events
    End Enum

    Public Function GetClosestGameobject(ByRef unit As BaseUnit, Optional ByVal GameObjectEntry As Integer = 0) As GameObjectObject
        Dim minDistance As Single = Single.MaxValue
        Dim tmpDistance As Single
        Dim targetGameobject As GameObjectObject = Nothing
        If TypeOf unit Is CharacterObject Then
            For Each GUID As ULong In CType(unit, CharacterObject).gameObjectsNear.ToArray()
                If WORLD_GAMEOBJECTs.ContainsKey(GUID) AndAlso (GameObjectEntry = 0 OrElse WORLD_GAMEOBJECTs(GUID).ID = GameObjectEntry) Then
                    tmpDistance = GetDistance(WORLD_GAMEOBJECTs(GUID), unit)
                    If tmpDistance < minDistance Then
                        minDistance = tmpDistance
                        targetGameobject = WORLD_GAMEOBJECTs(GUID)
                    End If
                End If
            Next
            Return targetGameobject
        Else
            Dim cellX As Byte, cellY As Byte
            GetMapTile(unit.positionX, unit.positionY, cellX, cellY)

            'TODO: Do we really have to look in all of those tiles?
            For x As Integer = -1 To 1
                For y As Integer = -1 To 1
                    If x + cellX > -1 AndAlso x + cellX < 64 AndAlso y + cellY > -1 AndAlso y + cellY < 64 Then
                        If Maps(unit.MapID).Tiles(x + cellX, y + cellY) IsNot Nothing Then
                            Dim gameobjects() As ULong = Maps(unit.MapID).Tiles(x + cellX, y + cellY).GameObjectsHere.ToArray()
                            For Each GUID As ULong In gameobjects
                                If WORLD_GAMEOBJECTs.ContainsKey(GUID) AndAlso (GameObjectEntry = 0 OrElse WORLD_GAMEOBJECTs(GUID).ID = GameObjectEntry) Then
                                    tmpDistance = GetDistance(WORLD_GAMEOBJECTs(GUID), unit)
                                    If tmpDistance < minDistance Then
                                        minDistance = tmpDistance
                                        targetGameobject = WORLD_GAMEOBJECTs(GUID)
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next
            Next
            Return targetGameobject
        End If
    End Function

    Public Sub On_CMSG_GAMEOBJECT_QUERY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        Dim response As New PacketClass(OPCODES.SMSG_GAMEOBJECT_QUERY_RESPONSE)

        packet.GetInt16()
        Dim GameObjectID As Integer = packet.GetInt32
        Dim GameObjectGUID As ULong = packet.GetUInt64

        Try
            Dim GameObject As GameObjectInfo

            If GAMEOBJECTSDatabase.ContainsKey(GameObjectID) = False Then
                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GAMEOBJECT_QUERY [GameObject {2} not loaded.]", Client.IP, Client.Port, GameObjectID)

                response.AddUInt32((GameObjectID Or &H80000000))
                Client.Send(response)
                response.Dispose()
                Exit Sub
            Else
                GameObject = GAMEOBJECTSDatabase(GameObjectID)
            End If

            response.AddInt32(GameObject.ID)
            response.AddInt32(GameObject.Type)
            response.AddInt32(GameObject.Model)
            response.AddString(GameObject.Name)
            response.AddInt16(0) 'Name2
            response.AddInt8(0) 'Name3
            response.AddInt8(0) 'Name4

            For i As Byte = 0 To 23
                response.AddUInt32(GameObject.Fields(i))
            Next i

            Client.Send(response)
            response.Dispose()
            'Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_GAMEOBJECT_QUERY_RESPONSE", Client.IP, Client.Port)
        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Unknown Error: Unable to find GameObjectID={0} in database.", GameObjectID)
        End Try
    End Sub
    Public Sub On_CMSG_GAMEOBJ_USE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GAMEOBJ_USE [GUID={2:X}]", Client.IP, Client.Port, GameObjectGUID)

        If WORLD_GAMEOBJECTs.ContainsKey(GameObjectGUID) = False Then Exit Sub
        Dim GO As GameObjectObject = WORLD_GAMEOBJECTs(GameObjectGUID)

        Client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_USE)

        Log.WriteLine(LogType.DEBUG, "GameObjectType: {0}", CType(WORLD_GAMEOBJECTs(GameObjectGUID).Type, GameObjectType))
        Select Case CType(GO.Type, GameObjectType)

            Case GameObjectType.GAMEOBJECT_TYPE_BUTTON, GameObjectType.GAMEOBJECT_TYPE_DOOR
                'DONE: Doors opening
                GO.OpenDoor()

            Case GameObjectType.GAMEOBJECT_TYPE_CHAIR
                'DONE: Chair sitting again
                Client.Character.Teleport(GO.positionX, GO.positionY, GO.positionZ, GO.orientation, GO.MapID)

                'STATE_SITTINGCHAIRLOW = 4
                'STATE_SITTINGCHAIRMEDIUM = 5
                'STATE_SITTINGCHAIRHIGH = 6

                Dim packetACK As New PacketClass(OPCODES.SMSG_STANDSTATE_CHANGE_ACK)
                packetACK.AddInt8(4 + GO.Sound(1))
                Client.Send(packetACK)
                packetACK.Dispose()

            Case GameObjectType.GAMEOBJECT_TYPE_QUESTGIVER
                Dim questSystem As New WS_Quests()
                Dim qm As QuestMenu = questSystem.GetQuestMenuGO(Client.Character, GameObjectGUID)
                questSystem.SendQuestMenu(Client.Character, GameObjectGUID, , qm)

            Case GameObjectType.GAMEOBJECT_TYPE_CAMERA
                Dim cinematicPacket As New PacketClass(OPCODES.SMSG_TRIGGER_CINEMATIC)
                cinematicPacket.AddUInt32(GO.Sound(1))
                Client.Send(cinematicPacket)
                cinematicPacket.Dispose()

            Case GameObjectType.GAMEOBJECT_TYPE_RITUAL
                Log.WriteLine(LogType.DEBUG, "Clicked a ritual.")
                'DONE: You can only click on rituals by group members
                If GO.Owner <> Client.Character.GUID AndAlso Client.Character.IsInGroup = False Then Exit Sub
                If GO.Owner <> Client.Character.GUID Then
                    If CHARACTERs.ContainsKey(GO.Owner) = False OrElse CHARACTERs(GO.Owner).IsInGroup = False Then Exit Sub
                    If Not CHARACTERs(GO.Owner).Group Is Client.Character.Group Then Exit Sub
                End If

                Log.WriteLine(LogType.DEBUG, "Casting ritual spell.")
                Client.Character.CastOnSelf(GO.Sound(1))

            Case GameObjectType.GAMEOBJECT_TYPE_SPELLCASTER
                Log.WriteLine(LogType.DEBUG, "Clicked a spellcaster.")

                GO.Flags = 2

                'DONE: Check if you're in the same party
                If GO.Sound(2) Then
                    Log.WriteLine(LogType.DEBUG, "Spellcaster requires same group.")
                    Log.WriteLine(LogType.DEBUG, "Owner: {0:X}  You: {1:X}", WORLD_GAMEOBJECTs(GameObjectGUID).Owner, Client.Character.GUID)
                    If GO.Owner <> Client.Character.GUID AndAlso Client.Character.IsInGroup = False Then Exit Sub
                    If GO.Owner <> Client.Character.GUID Then
                        If CHARACTERs.ContainsKey(GO.Owner) = False OrElse CHARACTERs(GO.Owner).IsInGroup = False Then Exit Sub
                        If Not CHARACTERs(GO.Owner).Group Is Client.Character.Group Then Exit Sub
                    End If
                End If

                Log.WriteLine(LogType.DEBUG, "Casted spellcaster spell.")
                Client.Character.CastOnSelf(GO.Sound(0))

                'TODO: Remove one charge

            Case GameObjectType.GAMEOBJECT_TYPE_MEETINGSTONE
                If Client.Character.Level < GO.Sound(0) Then 'Too low level
                    'TODO: Send the correct packet.
                    SendCastResult(SpellFailedReason.SPELL_FAILED_LEVEL_REQUIREMENT, Client, 23598)
                    Exit Sub
                End If
                If Client.Character.Level > WORLD_GAMEOBJECTs(GameObjectGUID).Sound(1) Then 'Too high level
                    'TODO: Send the correct packet.
                    SendCastResult(SpellFailedReason.SPELL_FAILED_LEVEL_REQUIREMENT, Client, 23598)
                    Exit Sub
                End If
                Client.Character.CastOnSelf(23598)

            Case GameObjectType.GAMEOBJECT_TYPE_FISHINGNODE
                If GO.Owner <> Client.Character.GUID Then Exit Sub

                If GO.Loot Is Nothing Then
                    If GO.State = GameObjectLootState.DOOR_CLOSED Then
                        GO.State = GameObjectLootState.DOOR_OPEN
                        Dim fishNotHookedPacket As New PacketClass(OPCODES.SMSG_FISH_NOT_HOOKED)
                        Client.Send(fishNotHookedPacket)
                        fishNotHookedPacket.Dispose()
                    End If
                Else
                    'DONE: Check if we where able to loot it with our skill level
                    Dim AreaFlag As Integer = GetAreaFlag(GO.positionX, GO.positionY, GO.MapID)
                    Dim AreaID As Integer = AreaTable(AreaFlag).ID

                    Dim MySQLQuery As New DataTable
                    WorldDatabase.Query(String.Format("SELECT * FROM skill_fishing_base_level WHERE entry = {0};", AreaID), MySQLQuery)

                    If MySQLQuery.Rows.Count = 0 Then
                        AreaID = AreaTable(AreaFlag).Zone
                        MySQLQuery.Clear()
                        WorldDatabase.Query(String.Format("SELECT * FROM skill_fishing_base_level WHERE entry = {0};", AreaID), MySQLQuery)
                    End If

                    Dim zoneSkill As Integer = 0
                    If MySQLQuery.Rows.Count > 0 Then
                        zoneSkill = CType(MySQLQuery.Rows(0).Item("skill"), Integer)
                    Else
                        Log.WriteLine(LogType.CRITICAL, "No fishing entry in 'skill_fishing_base_level' for area [{0}] in zone [{1}]", AreaTable(AreaFlag).ID, AreaTable(AreaFlag).Zone)
                    End If

                    Dim skill As Integer = Client.Character.Skills(SKILL_IDs.SKILL_FISHING).CurrentWithBonus
                    Dim chance As Integer = skill - zoneSkill + 5
                    Dim roll As Integer = Rnd.Next(1, 101)

                    If skill > zoneSkill AndAlso roll >= chance Then
                        GO.State = GameObjectLootState.DOOR_CLOSED
                        GO.Loot.SendLoot(Client)

                        'DONE: Update skill!
                        Client.Character.UpdateSkill(SKILL_IDs.SKILL_FISHING, 0.01)
                    Else
                        GO.State = GameObjectLootState.DOOR_CLOSED

                        Dim fishEscaped As New PacketClass(OPCODES.SMSG_FISH_ESCAPED)
                        Client.Send(fishEscaped)
                        fishEscaped.Dispose()
                    End If
                End If

                'Stop channeling!
                Client.Character.FinishSpell(CurrentSpellTypes.CURRENT_CHANNELED_SPELL, True)

        End Select
    End Sub

#End Region


End Module



#Region "WS.GameObjects.HelperTypes"
#End Region



