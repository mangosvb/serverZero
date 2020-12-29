'
' Copyright (C) 2013-2021 getMaNGOS <https://getmangos.eu>
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
Imports System.Threading
Imports Mangos.Common.Enums.GameObject
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Player
Imports Mangos.Common

Namespace Objects

    Public Class WS_Transports
        Private Function GetNewGUID() As ULong
            _WorldServer.TransportGUIDCounter += 1
            GetNewGUID = _WorldServer.TransportGUIDCounter
        End Function

#Region "Transports"
        Public Sub LoadTransports()
            Try
                Dim TransportQuery As New DataTable
                _WorldServer.WorldDatabase.Query("SELECT * FROM transports", TransportQuery)

                For Each Transport As DataRow In TransportQuery.Rows
                    Dim TransportEntry As Integer = Transport.Item("entry")
                    Dim TransportName As String = Transport.Item("name")
                    Dim TransportPeriod As Integer = Transport.Item("period")

                    Dim newTransport As New TransportObject(TransportEntry, TransportName, TransportPeriod)
                Next

                _WorldServer.Log.WriteLine(LogType.INFORMATION, "Database: {0} Transports initialized.", TransportQuery.Rows.Count)
            Catch e As IO.DirectoryNotFoundException
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Database : TransportQuery missing.")
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub
#End Region

#Region "Transport.Waypoints"
        Public Class TransportMove
            Public X As Single
            Public Y As Single
            Public Z As Single
            Public MapID As UInteger
            Public ActionFlag As Integer
            Public Delay As Integer
            Public DistSinceStop As Single
            Public DistUntilStop As Single
            Public DistFromPrev As Single
            Public tFrom As Single
            Public tTo As Single

            Public Sub New(ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Map As UInteger, ByVal Action As Integer, ByVal WaitTime As Integer)
                X = PosX
                Y = PosY
                Z = PosZ
                MapID = Map
                ActionFlag = Action
                Delay = WaitTime

                DistSinceStop = -1.0F
                DistUntilStop = -1.0F
                DistSinceStop = -1.0F
                tFrom = 0.0F
                tTo = 0.0F
            End Sub
        End Class

        Public Class TransportWP
            Public ID As Integer
            Public X As Single
            Public Y As Single
            Public Z As Single
            Public MapID As UInteger
            Public Teleport As Boolean
            Public Time As Integer

            Public Sub New(ByVal ID_ As Integer, ByVal Time_ As Integer, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Map As UInteger, ByVal Teleport_ As Boolean)
                ID = ID_
                Time = Time_
                X = PosX
                Y = PosY
                Z = PosZ
                MapID = Map
                Teleport = Teleport_
            End Sub
        End Class
#End Region

        Public Class TransportObject
            Inherits WS_GameObjects.GameObjectObject
            Implements IDisposable

            Public TransportName As String = ""

            Private ReadOnly Passengers As New List(Of WS_Base.BaseUnit)

            Private ReadOnly Waypoints As New List(Of TransportWP)
            Private ReadOnly Period As Integer = 0
            Private PathTime As Integer = 0

            Private FirstStop As Integer = -1
            Private LastStop As Integer = -1

            Private CurrentWaypoint As Integer = 0
            Private NextWaypoint As Integer = 0
            Private NextNodeTime As Integer = 0

            Private ReadOnly TimeToNextEvent As Integer = 0
            Private ReadOnly TransportState As TransportStates = TransportStates.TRANSPORT_DOCKED
            Private ReadOnly TransportAt As Byte = 0

            Public Sub New(ByVal ID_ As Integer, ByVal Name As String, ByVal Period_ As Integer)
                MyBase.New(ID_, _WS_Transports.GetNewGUID)

                'TODO: Only handle transports on the map(s) this server handles

                TransportName = Name
                Period = Period_

                If Not GenerateWaypoints() Then 'Check if we want to use this transport on this server
                    Exit Sub
                End If

                positionX = Waypoints(0).X
                positionY = Waypoints(0).Y
                positionZ = Waypoints(0).Z
                MapID = Waypoints(0).MapID
                orientation = 1

                VisibleDistance = 99999.0F 'Transports are always visible
                State = GameObjectLootState.DOOR_CLOSED

                TransportState = TransportStates.TRANSPORT_DOCKED
                TimeToNextEvent = 60000

                _WorldServer.WORLD_TRANSPORTs_Lock.AcquireWriterLock(Timeout.Infinite)
                _WorldServer.WORLD_TRANSPORTs.Add(GUID, Me)
                _WorldServer.WORLD_TRANSPORTs_Lock.ReleaseWriterLock()

                'Update the transport so that we get it's current position
                Update()
            End Sub

            Public Function GenerateWaypoints() As Boolean
                Dim PathID As Integer = Sound(0)
                Dim ShipSpeed As Single = Sound(1)
                If _WS_DBCDatabase.TaxiPaths.ContainsKey(PathID) = False Then
                    _WorldServer.Log.WriteLine(LogType.CRITICAL, "An transport [{0} - {1}] is created with an invalid TaxiPath.", ID, TransportName)
                    Return False
                End If

                Dim MapsUsed As Integer = 0
                Dim MapChange As Integer = 0
                Dim PathPoints As New List(Of TransportMove)
                Dim t As Integer = 0

                If _WS_DBCDatabase.TaxiPathNodes.ContainsKey(PathID) = True Then
                    For i As Integer = 0 To (_WS_DBCDatabase.TaxiPathNodes(PathID).Count - 2)
                        If MapChange = 0 Then
                            If _WS_DBCDatabase.TaxiPathNodes(PathID).ContainsKey(i) = True And _WS_DBCDatabase.TaxiPathNodes(PathID).ContainsKey(i + 1) = True Then
                                If _WS_DBCDatabase.TaxiPathNodes(PathID)(i).MapID = _WS_DBCDatabase.TaxiPathNodes(PathID)(i + 1).MapID Then
                                    PathPoints.Add(New TransportMove(_WS_DBCDatabase.TaxiPathNodes(PathID)(i).x, _WS_DBCDatabase.TaxiPathNodes(PathID)(i).y, _WS_DBCDatabase.TaxiPathNodes(PathID)(i).z, _WS_DBCDatabase.TaxiPathNodes(PathID)(i).MapID, _WS_DBCDatabase.TaxiPathNodes(PathID)(i).action, _WS_DBCDatabase.TaxiPathNodes(PathID)(i).waittime))

                                    If _WS_Maps.Maps.ContainsKey(_WS_DBCDatabase.TaxiPathNodes(PathID)(i).MapID) Then
                                        MapsUsed += 1
                                    End If
                                End If
                            Else
                                MapChange = 1
                            End If
                        Else
                            MapChange = 0
                        End If
                    Next

                    If MapsUsed = 0 Then Return False 'No maps for this transport is used on this server

                    PathPoints(0).DistFromPrev = 0.0F
                    If PathPoints(0).ActionFlag = 2 Then
                        LastStop = 0
                    End If

                    For i As Integer = 1 To PathPoints.Count - 1
                        If PathPoints(i).ActionFlag = 1 OrElse PathPoints(i).MapID <> PathPoints(i - 1).MapID Then
                            PathPoints(i).DistFromPrev = 0.0F
                        Else
                            PathPoints(i).DistFromPrev = _WS_Combat.GetDistance(PathPoints(i).X, PathPoints(i - 1).X, PathPoints(i).Y, PathPoints(i - 1).Y, PathPoints(i).Z, PathPoints(i - 1).Z)
                        End If
                        If PathPoints(i).ActionFlag = 2 Then
                            If FirstStop = -1 Then
                                FirstStop = i
                            End If
                            LastStop = i
                        End If
                    Next

                    Dim tmpDist As Single = 0.0F
                    Dim j As Integer

                    For i As Integer = 0 To PathPoints.Count - 1
                        j = (i + LastStop) Mod PathPoints.Count
                        If j >= 0 Then
                            If PathPoints(j).ActionFlag = 2 Then
                                tmpDist = 0.0F
                            Else
                                tmpDist += PathPoints(j).DistFromPrev
                            End If
                            PathPoints(j).DistSinceStop = tmpDist
                        End If
                    Next

                    For i As Integer = PathPoints.Count - 1 To 0 Step -1
                        j = (i + (FirstStop + 1)) Mod PathPoints.Count
                        tmpDist += PathPoints((j + 1) Mod PathPoints.Count).DistFromPrev
                        PathPoints(j).DistUntilStop = tmpDist
                        If PathPoints(j).ActionFlag = 2 Then tmpDist = 0.0F
                    Next

                    For i As Integer = 0 To PathPoints.Count - 1
                        If PathPoints(i).DistSinceStop < (30.0F * 30.0F * 0.5F) Then
                            PathPoints(i).tFrom = Math.Sqrt(2.0F * PathPoints(i).DistSinceStop)
                        Else
                            PathPoints(i).tFrom = ((PathPoints(i).DistSinceStop - (30.0F * 30.0F * 0.5F)) / 30.0F) + 30.0F
                        End If

                        If PathPoints(i).DistUntilStop < (30.0F * 30.0F * 0.5F) Then
                            PathPoints(i).tTo = Math.Sqrt(2.0F * PathPoints(i).DistUntilStop)
                        Else
                            PathPoints(i).tTo = ((PathPoints(i).DistUntilStop - (30.0F * 30.0F * 0.5F)) / 30.0F) + 30.0F
                        End If

                        PathPoints(i).tFrom *= 1000.0F
                        PathPoints(i).tTo *= 1000.0F
                    Next

                    Dim teleport As Boolean = False
                    If PathPoints(PathPoints.Count - 1).MapID <> PathPoints(0).MapID Then teleport = True

                    Waypoints.Add(New TransportWP(0, 0, PathPoints(0).X, PathPoints(0).Y, PathPoints(0).Z, PathPoints(0).MapID, teleport))
                    t += PathPoints(0).Delay * 1000

                    Dim cM As UInteger = PathPoints(0).MapID
                    For i As Integer = 0 To PathPoints.Count - 2
                        Dim d As Single = 0.0F
                        Dim tFrom As Single = PathPoints(i).tFrom
                        Dim tTo As Single = PathPoints(i).tTo

                        If d < PathPoints(i + 1).DistFromPrev AndAlso tTo > 0 Then
                            Do While d < PathPoints(i + 1).DistFromPrev AndAlso tTo > 0
                                tFrom += 100.0F
                                tTo -= 100.0F

                                If d > 0 Then
                                    Dim newX As Single = PathPoints(i).X + (PathPoints(i + 1).X - PathPoints(i).X) * d / PathPoints(i + 1).DistFromPrev
                                    Dim newY As Single = PathPoints(i).Y + (PathPoints(i + 1).Y - PathPoints(i).Y) * d / PathPoints(i + 1).DistFromPrev
                                    Dim newZ As Single = PathPoints(i).Z + (PathPoints(i + 1).Z - PathPoints(i).Z) * d / PathPoints(i + 1).DistFromPrev

                                    teleport = False
                                    If PathPoints(i).MapID <> cM Then
                                        teleport = True
                                        cM = PathPoints(i).MapID
                                    End If

                                    If teleport Then
                                        Waypoints.Add(New TransportWP(i, t, newX, newY, newZ, PathPoints(i).MapID, teleport))
                                    End If
                                End If

                                If tFrom < tTo Then
                                    If tFrom <= 30000.0F Then
                                        d = 0.5F * (tFrom / 1000.0F) * (tFrom / 1000.0F)
                                    Else
                                        d = 0.5F * 30.0F * 30.0F + 30.0F * ((tFrom - 30000.0F) / 1000.0F)
                                    End If
                                    d -= PathPoints(i).DistSinceStop
                                Else
                                    If tTo <= 30000.0F Then
                                        d = 0.5F * (tTo / 1000.0F) * (tTo / 1000.0F)
                                    Else
                                        d = 0.5F * 30.0F * 30.0F + 30.0F * ((tTo - 30000.0F) / 1000.0F)
                                    End If
                                    d = PathPoints(i).DistUntilStop - d
                                End If

                                t += 100
                            Loop
                            t -= 100
                        End If

                        If PathPoints(i + 1).tFrom > PathPoints(i + 1).tTo Then
                            t += 100 - (CLng(Fix(PathPoints(i + 1).tTo)) Mod 100)
                        Else
                            t += CLng(Fix(PathPoints(i + 1).tTo)) Mod 100
                        End If

                        teleport = False
                        If PathPoints(i + 1).ActionFlag = 1 OrElse PathPoints(i + 1).MapID <> PathPoints(i).MapID Then
                            teleport = True
                            cM = PathPoints(i + 1).MapID
                        End If

                        Waypoints.Add(New TransportWP(i, t, PathPoints(i + 1).X, PathPoints(i + 1).Y, PathPoints(i + 1).Z, PathPoints(i + 1).MapID, teleport))

                        t += PathPoints(i + 1).Delay * 1000
                    Next

                    CurrentWaypoint = 0
                    CurrentWaypoint = GetNextWaypoint()
                    NextWaypoint = GetNextWaypoint()
                    PathTime = t

                    NextNodeTime = Waypoints(CurrentWaypoint).Time

                    Return True
                Else
                    Return False
                End If

            End Function

            Public Function GetNextWaypoint() As Integer
                Dim tmpWP As Integer = CurrentWaypoint
                tmpWP += 1
                If tmpWP >= Waypoints.Count Then tmpWP = 0
                Return tmpWP
            End Function

            Public Sub AddPassenger(ByRef Unit As WS_Base.BaseUnit)
                If Passengers.Contains(Unit) Then Exit Sub

                SyncLock Passengers
                    Passengers.Add(Unit)
                End SyncLock
            End Sub

            Public Sub RemovePassenger(ByRef Unit As WS_Base.BaseUnit)
                If Passengers.Contains(Unit) = False Then Exit Sub

                SyncLock Passengers
                    Passengers.Remove(Unit)
                End SyncLock
            End Sub

            Public Sub Update()
                If Waypoints.Count <= 1 Then Exit Sub

                Dim Timer As Integer = _WS_Network.MsTime() Mod Period
                While (Math.Abs(Timer - Waypoints(CurrentWaypoint).Time) Mod PathTime) > (Math.Abs(Waypoints(NextWaypoint).Time - Waypoints(CurrentWaypoint).Time) Mod PathTime)
                    CurrentWaypoint = GetNextWaypoint()
                    NextWaypoint = GetNextWaypoint()

                    If Waypoints(CurrentWaypoint).MapID <> MapID OrElse Waypoints(CurrentWaypoint).Teleport Then
                        'Teleport transport
                        TeleportTransport(Waypoints(CurrentWaypoint).MapID, Waypoints(CurrentWaypoint).X, Waypoints(CurrentWaypoint).Y, Waypoints(CurrentWaypoint).Z)
                    Else
                        'Relocate teleport
                        positionX = Waypoints(CurrentWaypoint).X
                        positionY = Waypoints(CurrentWaypoint).Y
                        positionZ = Waypoints(CurrentWaypoint).Z

                        CheckCell()
                    End If

                    If CurrentWaypoint = FirstStop OrElse CurrentWaypoint = LastStop Then
                        Select Case ID
                            Case 176495, 164871, 175080
                                SendPlaySound(5154) 'ZeppelinDocked
                            Case 20808, 181646, 176231, 176244, 176310, 177233
                                SendPlaySound(5495) 'BoatDockingWarning
                            Case Else
                                SendPlaySound(5154) 'ShipDocked
                        End Select
                    End If

                    NextNodeTime = Waypoints(CurrentWaypoint).Time
                    'TODO: This line aborts the infinite loop sometimes created, all 8 transports wil need to be checked
                    If CurrentWaypoint = 0 Then Exit While
                End While
            End Sub

            Public Sub CreateEveryoneOnTransport(ByRef Character As WS_PlayerData.CharacterObject)
                'Create an update packet for you only one time, more effecient :)
                Dim mePacket As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                mePacket.AddInt32(1)
                mePacket.AddInt8(0)
                Dim meTmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                Character.FillAllUpdateFlags(meTmpUpdate)
                meTmpUpdate.AddToPacket(mePacket, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, Character)
                meTmpUpdate.Dispose()
                mePacket.CompressUpdatePacket()

                Dim tmpArray() As WS_Base.BaseUnit = Passengers.ToArray
                For Each tmpUnit As WS_Base.BaseUnit In tmpArray
                    If tmpUnit Is Nothing Then Continue For

                    If TypeOf tmpUnit Is WS_PlayerData.CharacterObject Then 'If the passenger is a player
                        If Character.CanSee(tmpUnit) Then 'If you can see player
                            Dim myPacket As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                            Try
                                myPacket.AddInt32(1)
                                myPacket.AddInt8(0)
                                Dim myTmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                                CType(tmpUnit, WS_PlayerData.CharacterObject).FillAllUpdateFlags(myTmpUpdate)
                                myTmpUpdate.AddToPacket(myPacket, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, CType(tmpUnit, WS_PlayerData.CharacterObject))
                                myTmpUpdate.Dispose()
                                Character.client.Send(myPacket)
                            Finally
                                myPacket.Dispose()
                            End Try

                            CType(tmpUnit, WS_PlayerData.CharacterObject).SeenBy.Add(Character.GUID)
                            Character.playersNear.Add(tmpUnit.GUID)
                        End If
                        If CType(tmpUnit, WS_PlayerData.CharacterObject).CanSee(Character) Then 'If player can see you
                            CType(tmpUnit, WS_PlayerData.CharacterObject).client.SendMultiplyPackets(mePacket)

                            Character.SeenBy.Add(tmpUnit.GUID)
                            CType(tmpUnit, WS_PlayerData.CharacterObject).playersNear.Add(Character.GUID)
                        End If
                    ElseIf TypeOf tmpUnit Is WS_Creatures.CreatureObject Then 'If the passenger is a creature
                        If Character.CanSee(tmpUnit) Then 'If you can see creature
                            Dim myPacket As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                            Try
                                myPacket.AddInt32(1)
                                myPacket.AddInt8(0)
                                Dim myTmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_UNIT)
                                CType(tmpUnit, WS_Creatures.CreatureObject).FillAllUpdateFlags(myTmpUpdate)
                                myTmpUpdate.AddToPacket(myPacket, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, CType(tmpUnit, WS_Creatures.CreatureObject))
                                myTmpUpdate.Dispose()
                                Character.client.Send(myPacket)
                            Finally
                                myPacket.Dispose()
                            End Try

                            CType(tmpUnit, WS_PlayerData.CharacterObject).SeenBy.Add(Character.GUID)
                            Character.creaturesNear.Add(tmpUnit.GUID)
                        End If
                    End If
                Next

                mePacket.Dispose()
            End Sub

            Public Sub CheckCell(Optional ByVal Teleported As Boolean = False)
                Dim TileX As Byte, TileY As Byte
                _WS_Maps.GetMapTile(positionX, positionY, TileX, TileY)
                If Teleported OrElse CellX <> TileX OrElse CellY <> TileY Then
                    If _WS_Maps.Maps(MapID).Tiles(CellX, CellY) IsNot Nothing Then
                        Try
                            _WS_Maps.Maps(MapID).Tiles(CellX, CellY).GameObjectsHere.Remove(GUID)
                        Catch
                        End Try
                    End If

                    CellX = TileX
                    CellY = TileY

                    If _WS_Maps.Maps(MapID).Tiles(CellX, CellY) IsNot Nothing Then
                        Try
                            _WS_Maps.Maps(MapID).Tiles(CellX, CellY).GameObjectsHere.Add(GUID)
                        Catch
                        End Try
                    End If

                    NotifyEnter()
                End If
            End Sub

            Public Sub NotifyEnter()
                'DONE: Sending to players in nearby cells
                Dim list() As ULong
                For i As Short = -1 To 1
                    For j As Short = -1 To 1
                        If (CellX + i) >= 0 AndAlso (CellX + i) <= 63 AndAlso (CellY + j) >= 0 AndAlso (CellY + j) <= 63 AndAlso _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j) IsNot Nothing AndAlso _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j).PlayersHere.Count > 0 Then
                            With _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j)
                                list = .PlayersHere.ToArray
                                For Each plGUID As ULong In list
                                    If _WorldServer.CHARACTERs.ContainsKey(plGUID) AndAlso _WorldServer.CHARACTERs(plGUID).CanSee(Me) Then
                                        Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                                        Try
                                            packet.AddInt32(1)
                                            packet.AddInt8(0)
                                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_GAMEOBJECT)
                                            Try
                                                FillAllUpdateFlags(tmpUpdate, _WorldServer.CHARACTERs(plGUID))
                                                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, Me)
                                            Finally
                                                tmpUpdate.Dispose()
                                            End Try
                                            _WorldServer.CHARACTERs(plGUID).client.SendMultiplyPackets(packet)
                                            _WorldServer.CHARACTERs(plGUID).gameObjectsNear.Add(GUID)
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

            Public Sub NotifyLeave()
                'DONE: Removing from players that can see the object
                For Each plGUID As ULong In SeenBy.ToArray
                    If _WorldServer.CHARACTERs(plGUID).gameObjectsNear.Contains(GUID) Then
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving.Add(GUID)
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving_Lock.ReleaseWriterLock()

                        _WorldServer.CHARACTERs(plGUID).gameObjectsNear.Remove(GUID)
                        SeenBy.Remove(plGUID)
                    End If
                Next
            End Sub

            Public Sub TeleportTransport(ByVal NewMap As UInteger, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single)
                Dim oldMap As UInteger = MapID

                Dim tmpArray() As WS_Base.BaseUnit = Passengers.ToArray
                For Each tmpUnit As WS_Base.BaseUnit In tmpArray
                    Try
                        'Remove passengers that doesn't exist anymore
                        If tmpUnit Is Nothing Then
                            SyncLock Passengers
                                Passengers.Remove(tmpUnit)
                            End SyncLock
                            Continue For
                        End If

                        If tmpUnit.IsDead Then
                            If TypeOf tmpUnit Is WS_PlayerData.CharacterObject Then
                                _WS_Handlers_Misc.CharacterResurrect(CType(tmpUnit, WS_PlayerData.CharacterObject))
                            ElseIf TypeOf tmpUnit Is WS_Creatures.CreatureObject Then
                                'TODO!
                            End If
                        End If

                        If TypeOf tmpUnit Is WS_PlayerData.CharacterObject Then
                            If CType(tmpUnit, WS_PlayerData.CharacterObject).OnTransport IsNot Nothing AndAlso CType(tmpUnit, WS_PlayerData.CharacterObject).OnTransport Is Me Then
                                CType(tmpUnit, WS_PlayerData.CharacterObject).Teleport(PosX, PosY, PosZ, CType(tmpUnit, WS_PlayerData.CharacterObject).orientation, NewMap)
                            Else
                                'Remove players no longer on this transport
                                SyncLock Passengers
                                    Passengers.Remove(tmpUnit)
                                End SyncLock
                            End If
                        ElseIf TypeOf tmpUnit Is WS_Creatures.CreatureObject Then
                            CType(tmpUnit, WS_Creatures.CreatureObject).positionX = PosX
                            CType(tmpUnit, WS_Creatures.CreatureObject).positionY = PosY
                            CType(tmpUnit, WS_Creatures.CreatureObject).positionZ = PosZ
                            CType(tmpUnit, WS_Creatures.CreatureObject).MapID = MapID
                            'TODO: What more?
                        End If
                    Catch ex As Exception
                        _WorldServer.Log.WriteLine(LogType.CRITICAL, "Failed to transfer player [0x{0:X}].{1}{2}", tmpUnit.GUID, Environment.NewLine, ex.ToString)
                    End Try
                Next

                MapID = NewMap
                positionX = PosX
                positionY = PosY
                positionZ = PosZ

                If NewMap <> oldMap Then
                    NotifyLeave()
                    CheckCell(True)
                End If
            End Sub

            Public Overrides Sub FillAllUpdateFlags(ByRef Update As Packets.UpdateClass, ByRef Character As WS_PlayerData.CharacterObject)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, ObjectType.TYPE_GAMEOBJECT + ObjectType.TYPE_OBJECT)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, ID)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, Size)

                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_POS_X, positionX)
                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_POS_Y, positionY)
                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_POS_Z, positionZ)
                'Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FACING, orientation)

                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_STATE, 0, State)

                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_TYPE_ID, Type)

                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FACTION, Faction)
                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_FLAGS, Flags)
                Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_DISPLAYID, ObjectInfo.Model)
                'Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION, Rotations(0))
                'Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 1, Rotations(1))
                'Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 2, Rotations(2))
                'Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_ROTATION + 3, Rotations(3))

                'Update.SetUpdateFlag(EGameObjectFields.GAMEOBJECT_TIMESTAMP, msTime) ' Changed in 1.12.x client branch?
            End Sub
        End Class
    End Class
End Namespace