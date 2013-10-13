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
Imports mangosVB.Common
Imports mangosVB.Common.BaseWriter


Public Module WC_Battlegrounds

    Public BATTLEFIELDs As New Dictionary(Of Integer, Battlefield)
    Public BATTLEFIELDs_Lock As New ReaderWriterLock
    Private BATTLEFIELDs_Counter As Integer = 0

    Public Class Battlefield
        Implements IDisposable

        Public QueueTeam1 As New List(Of CharacterObject)
        Public QueueTeam2 As New List(Of CharacterObject)
        Public InvitedTeam1 As New List(Of CharacterObject)
        Public InvitedTeam2 As New List(Of CharacterObject)
        Public MembersTeam1 As New List(Of CharacterObject)
        Public MembersTeam2 As New List(Of CharacterObject)

        Public ID As Integer
        Public Map As UInteger
        Public MapType As BattlefieldMapType
        Public Type As BattlefieldType
        Public LevelMin As Byte
        Public LevelMax As Byte
        Public MaxPlayersPerTeam As Integer = 10
        Public MinPlayersPerTeam As Integer = 10

        Public bfTimer As Timer

        Public Sub New(ByVal rMapType As BattlefieldMapType, ByVal rLevel As Byte, ByVal rMap As UInteger)
            ID = Interlocked.Increment(BATTLEFIELDs_Counter)
            LevelMin = 0
            LevelMax = 70
            MapType = rMapType
            Map = rMap
            MaxPlayersPerTeam = Battlegrounds(rMapType).MaxPlayersPerTeam
            MinPlayersPerTeam = Battlegrounds(rMapType).MinPlayersPerTeam

            BATTLEFIELDs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
            BATTLEFIELDs.Add(ID, Me)
            BATTLEFIELDs_Lock.ReleaseWriterLock()

            bfTimer = New Timer(AddressOf Update, Nothing, 20000, 20000)
        End Sub


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                BATTLEFIELDs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                BATTLEFIELDs.Remove(ID)
                BATTLEFIELDs_Lock.ReleaseWriterLock()
                bfTimer.Dispose()
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

        Public Sub Update(ByVal State As Object)

            'DONE: Adding members from queue
            If (MembersTeam1.Count + InvitedTeam1.Count) < MaxPlayersPerTeam AndAlso QueueTeam1.Count > 0 Then
                Dim c As CharacterObject = QueueTeam1.Item(0)
                QueueTeam1.RemoveAt(0)
                InvitedTeam1.Add(c)
                SendBattlegroundStatus(c, 0)
            End If
            If (MembersTeam2.Count + InvitedTeam2.Count) < MaxPlayersPerTeam AndAlso QueueTeam2.Count > 0 Then
                Dim c As CharacterObject = QueueTeam2.Item(0)
                QueueTeam2.RemoveAt(0)
                InvitedTeam2.Add(c)
                SendBattlegroundStatus(c, 0)
            End If

            'TODO: Checking minimum players
        End Sub

        Public Sub Enqueue(ByVal c As CharacterObject)
            If GetCharacterSide(c.Race) Then
                QueueTeam1.Add(c)
            Else
                QueueTeam2.Add(c)
            End If

            SendBattlegroundStatus(c, 0)
        End Sub
        Public Sub Join(ByVal c As CharacterObject)
            If InvitedTeam1.Contains(c) OrElse InvitedTeam2.Contains(c) Then
                If InvitedTeam1.Contains(c) Then
                    MembersTeam1.Add(c)
                    InvitedTeam1.Remove(c)
                End If
                If InvitedTeam2.Contains(c) Then
                    MembersTeam2.Add(c)
                    InvitedTeam2.Remove(c)
                End If

                SendBattlegroundStatus(c, 0)

                With WorldSafeLocs(Battlegrounds(MapType).AllianceStartLoc)
                    'TODO: WTF? characters_locations table? when?
                    'Dim q As New DataTable
                    'Database.Query(String.Format("SELECT char_guid FROM characters_locations WHERE char_guid = {0};", c.GUID), q)
                    'If q.Rows.Count = 0 Then
                    '    'Save only first BG location
                    '    Database.Update(String.Format("INSERT INTO characters_locations(char_guid, char_positionX, char_positionY, char_positionZ, char_zone_id, char_map_id, char_orientation) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6});", _
                    '                                                    c.GUID, Trim(Str(c.PositionX)), Trim(Str(c.PositionY)), Trim(Str(c.PositionZ)), c.Zone, c.Map, 0))
                    'End If
                    c.Transfer(.x, .y, .z, Battlegrounds(MapType).AllianceStartO, .map)
                End With
            End If
        End Sub
        Public Sub Leave(ByVal c As CharacterObject)
            If QueueTeam1.Contains(c) OrElse QueueTeam2.Contains(c) Then
                QueueTeam1.Remove(c)
                QueueTeam2.Remove(c)

            ElseIf MembersTeam1.Contains(c) OrElse MembersTeam2.Contains(c) Then
                MembersTeam1.Remove(c)
                MembersTeam2.Remove(c)

                'TODO: Still.. characters_locations, doesn't exist?
                'Dim q As New DataTable
                'Database.Query(String.Format("SELECT * FROM characters_locations WHERE char_guid = {0};", c.GUID), q)
                'If q.Rows.Count = 0 Then
                '    SendMessageSystem(c.Client, "You don't have location saved!")
                'Else
                '    c.Transfer(q.Rows(0).Item("char_positionX"), q.Rows(0).Item("char_positionY"), q.Rows(0).Item("char_positionZ"), q.Rows(0).Item("char_orientation"), q.Rows(0).Item("char_map_id"))
                'End If
            End If

            SendBattlegroundStatus(c, 0)
        End Sub


        Public Enum BattlegroundStatus
            STATUS_CLEAR = 0
            STATUS_WAIT_QUEUE = 1
            STATUS_WAIT_JOIN = 2
            STATUS_IN_PROGRESS = 3
        End Enum
        Public Sub SendBattlegroundStatus(ByVal c As CharacterObject, ByVal Slot As Byte)
            Dim Status As BattlegroundStatus = BattlegroundStatus.STATUS_CLEAR
            If QueueTeam1.Contains(c) Or QueueTeam2.Contains(c) Then
                Status = BattlegroundStatus.STATUS_WAIT_QUEUE
            ElseIf InvitedTeam1.Contains(c) Or InvitedTeam2.Contains(c) Then
                Status = BattlegroundStatus.STATUS_WAIT_JOIN
            ElseIf MembersTeam1.Contains(c) Or MembersTeam2.Contains(c) Then
                Status = BattlegroundStatus.STATUS_IN_PROGRESS
            Else
                'Do nothing
            End If

            Dim p As New PacketClass(OPCODES.SMSG_BATTLEFIELD_STATUS)
            p.AddUInt32(Slot)               'Slot (0, 1 or 2)

            p.AddInt8(0)                    'ArenaType
            p.AddInt8(&HD)                  'Unk1 (0xD?)
            p.AddInt8(MapType)              'MapType
            p.AddInt8(0)                    'Unk2
            p.AddInt16(0)                   'Unk3 (String?)
            p.AddInt16(ID)                  'ID

            p.AddInt32(0)                   'Unk5
            p.AddInt8(0)                    'alliance/horde for BG and skirmish/rated for Arenas
            p.AddInt32(Status)

            Select Case Status
                Case BattlegroundStatus.STATUS_WAIT_QUEUE
                    p.AddUInt32(120000)     'average wait time, milliseconds
                    p.AddUInt32(1)          'time in queue, updated every minute?
                Case BattlegroundStatus.STATUS_WAIT_JOIN
                    p.AddUInt32(Map)        'map id
                    p.AddUInt32(60000)      'time to remove from queue, milliseconds
                Case BattlegroundStatus.STATUS_IN_PROGRESS
                    p.AddUInt32(Map)        'map id
                    p.AddUInt32(0)          '0 at bg start, 120000 after bg end, time to bg auto leave, milliseconds
                    p.AddUInt32(1)          'time from bg start, milliseconds
                    p.AddInt8(1)            'unk sometimes 0x0!
                Case BattlegroundStatus.STATUS_CLEAR
                    'Do nothing
            End Select
            c.Client.Send(p)
            p.Dispose()
        End Sub
    End Class


    Public Function GetBattlefield(ByVal MapType As BattlefieldMapType, ByVal Level As Byte) As Battlefield
        Dim Battlefield As Battlefield = Nothing

        BATTLEFIELDs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
        For Each b As KeyValuePair(Of Integer, Battlefield) In BATTLEFIELDs
            If b.Value.MapType = MapType AndAlso b.Value.LevelMax >= Level AndAlso b.Value.LevelMin <= Level Then
                Battlefield = b.Value
            End If
        Next
        BATTLEFIELDs_Lock.ReleaseReaderLock()

        'DONE: Create new if not found any
        If Battlefield Is Nothing Then
            Dim Map As UInteger = Battlegrounds(MapType).Map
            If WorldServer.BattlefieldCheck(Map) Then
                Battlefield = New Battlefield(MapType, Level, Map)
            Else
                Return Nothing
            End If
        End If
        Return Battlefield
    End Function
    Public Sub SendBattlegroundGroupJoined(ByVal c As CharacterObject)
        '0 - Your group has joined a battleground queue, but you are not eligible
        '1 - Your group has joined the queue for AV
        '2 - Your group has joined the queue for WS
        '3 - Your group has joined the queue for AB
        '4 - Your group has joined the queue for NA
        '5 - Your group has joined the queue for BE Arena
        '6 - Your group has joined the queue for All Arenas
        '7 - Your group has joined the queue for EotS

        Dim p As New PacketClass(OPCODES.SMSG_GROUP_JOINED_BATTLEGROUND)
        p.AddUInt32(&HFFFFFFFEUI)
        c.Client.Send(p)
        p.Dispose()
    End Sub


End Module
