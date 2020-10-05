﻿'
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

Imports System.Threading
Imports Mangos.Common.Globals
Imports Mangos.Cluster.DataStores
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Server
Imports Mangos.Common.Enums

Namespace Handlers

    Public Module WC_Handlers_Battleground

        Public Sub On_CMSG_BATTLEFIELD_PORT(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()

            'Dim Unk1 As Byte = packet.GetInt8
            'Dim Unk2 As Byte = packet.GetInt8                   'unk, can be 0x0 (may be if was invited?) and 0x1
            'Dim MapType As UInteger = packet.GetInt32           'type id from dbc
            'Dim MapType As Byte = packet.GetUInt8
            Dim id As UInteger = packet.GetUInt16               'ID
            Dim action As Byte = packet.GetUInt8                 'enter battle 0x1, leave queue 0x0

            'Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BATTLEFIELD_PORT [MapType: {2}, Action: {3}, Unk1: {4}, Unk2: {5}, ID: {6}]", client.IP, client.Port, MapType, Action, Unk1, Unk2, ID)
            Log.WriteLine(GlobalEnum.LogType.DEBUG, "[{0}:{1}] CMSG_BATTLEFIELD_PORT [Action: {1}, ID: {2}]", client.IP, client.Port, action, id)

            If action = 0 Then
                BATTLEFIELDs(id).Leave(client.Character)
            Else
                BATTLEFIELDs(id).Join(client.Character)
            End If
        End Sub

        Public Sub On_CMSG_LEAVE_BATTLEFIELD(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()

            Dim Unk1 As Byte = packet.GetInt8
            Dim Unk2 As Byte = packet.GetInt8
            Dim MapType As UInteger = packet.GetInt32
            Dim ID As UInteger = packet.GetUInt16

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LEAVE_BATTLEFIELD [MapType: {2}, Unk1: {3}, Unk2: {4}, ID: {5}]", client.IP, client.Port, MapType, Unk1, Unk2, ID)

            BATTLEFIELDs(ID).Leave(client.Character)
        End Sub

        Public Sub On_CMSG_BATTLEMASTER_JOIN(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 16 Then Exit Sub
            packet.GetInt16()
            Dim guid As ULong = packet.GetUInt64
            Dim mapType As UInteger = packet.GetInt32
            Dim instance As UInteger = packet.GetInt32
            Dim asGroup As Byte = packet.GetInt8

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BATTLEMASTER_JOIN [MapType: {2}, Instance: {3}, Group: {4}, GUID: {5}]", client.IP, client.Port, mapType, instance, asGroup, guid)

            GetBattlefield(mapType, client.Character.Level).Enqueue(client.Character)
        End Sub

        Public BATTLEFIELDs As New Dictionary(Of Integer, Battlefield)
        Public BATTLEFIELDs_Lock As New ReaderWriterLock
        Private BATTLEFIELDs_Counter As Integer = 0

        Public Class Battlefield
            Implements IDisposable

            Private ReadOnly _queueTeam1 As New List(Of CharacterObject)
            Private ReadOnly _queueTeam2 As New List(Of CharacterObject)
            Private ReadOnly _invitedTeam1 As New List(Of CharacterObject)
            Private ReadOnly _invitedTeam2 As New List(Of CharacterObject)
            Private ReadOnly _membersTeam1 As New List(Of CharacterObject)
            Private ReadOnly _membersTeam2 As New List(Of CharacterObject)

            Public ReadOnly ID As Integer
            Private ReadOnly _map As UInteger
            Public ReadOnly MapType As BattlefieldMapType
            Public Type As BattlefieldType
            Friend ReadOnly LevelMin As Byte
            Friend ReadOnly LevelMax As Byte
            Private ReadOnly _maxPlayersPerTeam As Integer = 10 'Is this right
            Private _minPlayersPerTeam As Integer = 10

            Private ReadOnly _bfTimer As Timer

            Public Sub New(ByVal rMapType As BattlefieldMapType, ByVal rLevel As Byte, ByVal rMap As UInteger)
                ID = Interlocked.Increment(BATTLEFIELDs_Counter)
                LevelMin = 0
                LevelMax = 60
                MapType = rMapType
                _map = rMap
                _maxPlayersPerTeam = Battlegrounds(rMapType).MaxPlayersPerTeam
                _minPlayersPerTeam = Battlegrounds(rMapType).MinPlayersPerTeam

                BATTLEFIELDs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                BATTLEFIELDs.Add(ID, Me)
                BATTLEFIELDs_Lock.ReleaseWriterLock()

                _bfTimer = New Timer(AddressOf Update, Nothing, 20000, 20000)
            End Sub

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    BATTLEFIELDs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                    BATTLEFIELDs.Remove(ID)
                    BATTLEFIELDs_Lock.ReleaseWriterLock()
                    _bfTimer.Dispose()
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

            ''' <summary>
            ''' Updates the specified state.
            ''' </summary>
            ''' <param name="state">The state.</param>
            ''' <returns></returns>
            Private Sub Update(ByVal state As Object)

                'DONE: Adding members from queue
                If (_membersTeam1.Count + _invitedTeam1.Count) < _maxPlayersPerTeam AndAlso _queueTeam1.Count > 0 Then
                    Dim objCharacter As CharacterObject = _queueTeam1.Item(0)
                    _queueTeam1.RemoveAt(0)
                    _invitedTeam1.Add(objCharacter)
                    SendBattlegroundStatus(objCharacter, 0)
                End If
                If (_membersTeam2.Count + _invitedTeam2.Count) < _maxPlayersPerTeam AndAlso _queueTeam2.Count > 0 Then
                    Dim objCharacter As CharacterObject = _queueTeam2.Item(0)
                    _queueTeam2.RemoveAt(0)
                    _invitedTeam2.Add(objCharacter)
                    SendBattlegroundStatus(objCharacter, 0)
                End If

                'TODO: Checking minimum players
            End Sub

            ''' <summary>
            ''' Enqueues the specified obj char.
            ''' </summary>
            ''' <param name="objCharacter">The obj char.</param>
            ''' <returns></returns>
            Public Sub Enqueue(ByVal objCharacter As CharacterObject)
                If GetCharacterSide(objCharacter.Race) Then
                    _queueTeam1.Add(objCharacter)
                Else
                    _queueTeam2.Add(objCharacter)
                End If

                SendBattlegroundStatus(objCharacter, 0)
            End Sub

            ''' <summary>
            ''' Joins the specified obj char.
            ''' </summary>
            ''' <param name="objCharacter">The obj char.</param>
            ''' <returns></returns>
            Public Sub Join(ByVal objCharacter As CharacterObject)
                If _invitedTeam1.Contains(objCharacter) OrElse _invitedTeam2.Contains(objCharacter) Then
                    If _invitedTeam1.Contains(objCharacter) Then
                        _membersTeam1.Add(objCharacter)
                        _invitedTeam1.Remove(objCharacter)
                    End If
                    If _invitedTeam2.Contains(objCharacter) Then
                        _membersTeam2.Add(objCharacter)
                        _invitedTeam2.Remove(objCharacter)
                    End If

                    SendBattlegroundStatus(objCharacter, 0)

                    With WorldSafeLocs(Battlegrounds(MapType).AllianceStartLoc)
                        'TODO: WTF? characters_locations table? when?
                        'Dim q As New DataTable
                        'CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters_locations WHERE char_guid = {0};", objCharacter.GUID), q)
                        'If q.Rows.Count = 0 Then
                        '    'Save only first BG location
                        '    CharacterDatabase.Update(String.Format("INSERT INTO characters_locations(char_guid, char_positionX, char_positionY, char_positionZ, char_zone_id, char_map_id, char_orientation) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6});", _
                        '                                                    objCharacter.GUID, Trim(Str(objCharacter.PositionX)), Trim(Str(objCharacter.PositionY)), Trim(Str(objCharacter.PositionZ)), objCharacter.Zone, objCharacter.Map, 0))
                        'End If
                        objCharacter.Transfer(.x, .y, .z, Battlegrounds(MapType).AllianceStartO, .map)
                    End With
                End If
            End Sub

            ''' <summary>
            ''' Leaves the specified obj char.
            ''' </summary>
            ''' <param name="objCharacter">The obj char.</param>
            ''' <returns></returns>
            Public Sub Leave(ByVal objCharacter As CharacterObject)
                If _queueTeam1.Contains(objCharacter) OrElse _queueTeam2.Contains(objCharacter) Then
                    _queueTeam1.Remove(objCharacter)
                    _queueTeam2.Remove(objCharacter)

                ElseIf _membersTeam1.Contains(objCharacter) OrElse _membersTeam2.Contains(objCharacter) Then
                    _membersTeam1.Remove(objCharacter)
                    _membersTeam2.Remove(objCharacter)

                    'TODO: Still.. characters_locations, doesn't exist?
                    'Dim q As New DataTable
                    'CharacterDatabase.Query(String.Format("SELECT * FROM characters_locations WHERE char_guid = {0};", objCharacter.GUID), q)
                    'If q.Rows.Count = 0 Then
                    '    SendMessageSystem(objCharacter.Client, "You don't have location saved!")
                    'Else
                    '    objCharacter.Transfer(q.Rows(0).Item("char_positionX"), q.Rows(0).Item("char_positionY"), q.Rows(0).Item("char_positionZ"), q.Rows(0).Item("char_orientation"), q.Rows(0).Item("char_map_id"))
                    'End If
                End If

                SendBattlegroundStatus(objCharacter, 0)
            End Sub

            ''' <summary>
            ''' Sends the battleground status.
            ''' </summary>
            ''' <param name="objCharacter">The obj char.</param>
            ''' <param name="slot">The slot.</param>
            ''' <returns></returns>
            Private Sub SendBattlegroundStatus(ByVal objCharacter As CharacterObject, ByVal slot As Byte)
                Dim status As BattlegroundStatus = BattlegroundStatus.STATUS_CLEAR
                If _queueTeam1.Contains(objCharacter) Or _queueTeam2.Contains(objCharacter) Then
                    status = BattlegroundStatus.STATUS_WAIT_QUEUE
                ElseIf _invitedTeam1.Contains(objCharacter) Or _invitedTeam2.Contains(objCharacter) Then
                    status = BattlegroundStatus.STATUS_WAIT_JOIN
                ElseIf _membersTeam1.Contains(objCharacter) Or _membersTeam2.Contains(objCharacter) Then
                    status = BattlegroundStatus.STATUS_IN_PROGRESS
                Else
                    'Do nothing
                End If

                Dim p As New PacketClass(OPCODES.SMSG_BATTLEFIELD_STATUS)
                Try
                    p.AddUInt32(slot)               'Slot (0, 1 or 2)

                    'p.AddInt8(0)                    'ArenaType
                    p.AddUInt32(MapType)              'MapType
                    'p.AddInt8(&HD)                  'Unk1 (0xD?)
                    'p.AddInt8(0)                    'Unk2
                    'p.AddInt16()                   'Unk3 (String?)
                    p.AddUInt32(ID)                  'ID

                    'p.AddInt32(0)                   'Unk5
                    p.AddInt8(0)                    'alliance/horde for BG and skirmish/rated for Arenas
                    p.AddUInt32(status)

                    Select Case status
                        Case BattlegroundStatus.STATUS_WAIT_QUEUE
                            p.AddUInt32(120000)     'average wait time, milliseconds
                            p.AddUInt32(1)          'time in queue, updated every minute?
                        Case BattlegroundStatus.STATUS_WAIT_JOIN
                            p.AddUInt32(_map)        'map id
                            p.AddUInt32(60000)      'time to remove from queue, milliseconds
                        Case BattlegroundStatus.STATUS_IN_PROGRESS
                            p.AddUInt32(_map)        'map id
                            p.AddUInt32(0)          '0 at bg start, 120000 after bg end, time to bg auto leave, milliseconds
                            p.AddUInt32(1)          'time from bg start, milliseconds
                            p.AddInt8(1)            'unk sometimes 0x0!
                        Case BattlegroundStatus.STATUS_CLEAR
                            'Do nothing
                    End Select
                    objCharacter.client.Send(p)
                Finally
                    p.Dispose()
                End Try
            End Sub
        End Class

        ''' <summary>
        ''' Gets the battlefield.
        ''' </summary>
        ''' <param name="mapType">Type of the map.</param>
        ''' <param name="level">The level.</param>
        ''' <returns></returns>
        Public Function GetBattlefield(ByVal mapType As BattlefieldMapType, ByVal level As Byte) As Battlefield
            Dim battlefield As Battlefield = Nothing

            BATTLEFIELDs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each b As KeyValuePair(Of Integer, Battlefield) In BATTLEFIELDs
                If b.Value.MapType = mapType AndAlso b.Value.LevelMax >= level AndAlso b.Value.LevelMin <= level Then
                    battlefield = b.Value
                End If
            Next
            BATTLEFIELDs_Lock.ReleaseReaderLock()

            'DONE: Create new if not found any
            If battlefield Is Nothing Then
                Dim map As UInteger = GetBattleGrounMapIdByTypeId(mapType)
                If WorldServer.BattlefieldCheck(map) Then
                    battlefield = New Battlefield(mapType, level, map)
                Else
                    Return Nothing
                End If
            End If
            Return battlefield
        End Function

        ' ''' <summary>
        ' ''' Gets the battle ground type id by map id.
        ' ''' </summary>
        ' ''' <param name="mapId">The map id.</param>
        ' ''' <returns></returns>
        'Function GetBattleGroundTypeIdByMapId(ByVal mapId As Integer) As Integer
        '    Select Case mapId
        '        Case 30
        '            Return BattleGroundTypeId.BATTLEGROUND_AV
        '        Case 489
        '            Return BattleGroundTypeId.BATTLEGROUND_WS
        '        Case 529
        '            Return BattleGroundTypeId.BATTLEGROUND_AB
        '        Case Else
        '            Return BattleGroundTypeId.BATTLEGROUND_TYPE_NONE
        '    End Select
        'End Function

        ''' <summary>
        ''' Gets the battle groun map id by type id.
        ''' </summary>
        ''' <param name="bgTypeId">The bg type id.</param>
        ''' <returns></returns>
        Private Function GetBattleGrounMapIdByTypeId(ByVal bgTypeId As BattleGroundTypeId) As Integer
            Select Case bgTypeId
                Case BattleGroundTypeId.BATTLEGROUND_AV
                    Return 30
                Case BattleGroundTypeId.BATTLEGROUND_WS
                    Return 489
                Case BattleGroundTypeId.BATTLEGROUND_AB
                    Return 529
                Case Else
                    Return 0
            End Select
        End Function

        ''' <summary>
        ''' Sends the battleground group joined.
        ''' </summary>
        ''' <param name="objCharacter">The objCharacter.</param>
        ''' <returns></returns>
        Public Sub SendBattlegroundGroupJoined(ByVal objCharacter As CharacterObject)
            '0 - Your group has joined a battleground queue, but you are not eligible
            '1 - Your group has joined the queue for AV
            '2 - Your group has joined the queue for WS
            '3 - Your group has joined the queue for AB


            Dim p As New PacketClass(OPCODES.SMSG_GROUP_JOINED_BATTLEGROUND)
            Try
                p.AddUInt32(&HFFFFFFFEUI)
                objCharacter.client.Send(p)
            Finally
                p.Dispose()
            End Try
        End Sub

        Public Sub On_MSG_BATTLEGROUND_PLAYER_POSITIONS()
            Dim p As New PacketClass(OPCODES.MSG_BATTLEGROUND_PLAYER_POSITIONS)
            Try
                p.AddUInt32(0)
                p.AddUInt32(0) 'flagCarrierCount
            Finally
                p.Dispose()
            End Try
        End Sub

    End Module
End Namespace