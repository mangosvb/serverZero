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

Imports System.Threading
Imports Mangos.Common
Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Group
Imports Mangos.Common.Enums.Map
Imports Mangos.Common.Globals
Imports Mangos.SignalR
Imports Mangos.World.Globals
Imports Mangos.World.Handlers
Imports Mangos.World.Maps
Imports Mangos.World.Player
Imports Mangos.World.Social
Imports Microsoft.AspNetCore.SignalR

Namespace Server

    Public Module WS_Network

        Private LastPing As Integer = 0
        Public WC_MsTime As Integer = 0

        Public Function MsTime() As Integer
            'DONE: Calculate the clusters _NativeMethods.timeGetTime("")
            Return WC_MsTime + (_NativeMethods.timeGetTime("") - LastPing)
        End Function

        Public Class WorldServerClass
            Inherits Hub
            Implements IWorld
            Implements IDisposable

            Public _flagStopListen As Boolean = False
            Public LocalURI As String

            Private m_RemoteURI As String
            Private m_Connection As Timer
            Private m_TimerCPU As Timer
            Private LastInfo As Date
            Private LastCPUTime As Double = 0.0F
            Private UsageCPU As Single = 0.0F
            Public Cluster As ICluster = Nothing

            Public Sub New()
                m_RemoteURI = String.Format("http://{0}:{1}", _WorldServer.Config.ClusterConnectHost, _WorldServer.Config.ClusterConnectPort)
                LocalURI = String.Format("http://{0}:{1}", _WorldServer.Config.LocalConnectHost, _WorldServer.Config.LocalConnectPort)
                Cluster = Nothing

                'Creating connection timer
                LastPing = _NativeMethods.timeGetTime("")
                m_Connection = New Timer(AddressOf CheckConnection, Nothing, 10000, 10000)

                'Creating CPU check timer
                m_TimerCPU = New Timer(AddressOf CheckCPU, Nothing, 1000, 1000)
            End Sub

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    ClusterDisconnect()

                    _flagStopListen = True
                    m_TimerCPU.Dispose()
                    m_Connection.Dispose()
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

            Public Sub ClusterConnect()
                While Cluster Is Nothing
                    Try
                        'NOTE: Password protected remoting
                        Cluster = ProxyClient.Create(Of ICluster)(m_RemoteURI)

                        'NOTE: Not protected remoting
                        'Cluster = RemotingServices.Connect(GetType(ICluster), m_RemoteURI)
                        If Not IsNothing(Cluster) Then
                            If Cluster.Connect(LocalURI, _WorldServer.Config.Maps.Select(Function(x) CType(x, UInteger)).ToList()) Then Exit While
                            Cluster.Disconnect(LocalURI, _WorldServer.Config.Maps.Select(Function(x) CType(x, UInteger)).ToList())
                        End If
                    Catch e As Exception
                        _WorldServer.Log.WriteLine(LogType.FAILED, "Unable to connect to cluster. [{0}]", e.Message)
                    End Try
                    Cluster = Nothing
                    Thread.Sleep(3000)
                End While

                _WorldServer.Log.WriteLine(LogType.SUCCESS, "Contacted cluster [{0}]", m_RemoteURI)
            End Sub
            Public Sub ClusterDisconnect()
                Try
                    Cluster.Disconnect(LocalURI, _WorldServer.Config.Maps.Select(Function(x) CType(x, UInteger)).ToList())
                Catch
                Finally
                    Cluster = Nothing
                End Try
            End Sub

            Public Sub ClientTransfer(ByVal ID As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As Integer)
                If Not WS_Maps.Maps.ContainsKey(map) Then
                    _WorldServer.CLIENTs(ID).Character.Dispose()
                    _WorldServer.CLIENTs(ID).Delete()
                End If

                Cluster.ClientTransfer(ID, posX, posY, posZ, ori, map)
            End Sub

            Public Sub ClientConnect(ByVal id As UInteger, ByVal client As ClientInfo) Implements IWorld.ClientConnect
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client connected", id)

                If client Is Nothing Then Throw New ApplicationException("Client doesn't exist!")

                Dim objCharacter As New ClientClass(client)

                If _WorldServer.CLIENTs.ContainsKey(id) = True Then  'Ooops, the character is already loaded, remove it
                    _WorldServer.CLIENTs.Remove(id)
                End If
                _WorldServer.CLIENTs.Add(id, objCharacter)
            End Sub

            Public Sub ClientDisconnect(ByVal id As UInteger) Implements IWorld.ClientDisconnect
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client disconnected", id)

                If _WorldServer.CLIENTs(id).Character IsNot Nothing Then
                    _WorldServer.CLIENTs(id).Character.Save()
                End If

                _WorldServer.CLIENTs(id).Delete()
                _WorldServer.CLIENTs.Remove(id)
            End Sub
            Public Sub ClientLogin(ByVal id As UInteger, ByVal guid As ULong) Implements IWorld.ClientLogin
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client login [0x{1:X}]", id, guid)

                Try
                    Dim client As ClientClass = _WorldServer.CLIENTs(id)
                    Dim Character As New WS_PlayerData.CharacterObject(client, guid)

                    _WorldServer.CHARACTERs_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    _WorldServer.CHARACTERs(guid) = Character
                    _WorldServer.CHARACTERs_Lock.ReleaseWriterLock()

                    'DONE: SMSG_CORPSE_RECLAIM_DELAY
                    SendCorpseReclaimDelay(client, Character)

                    'DONE: Cast talents and racial passive spells
                    InitializeTalentSpells(Character)

                    Character.Login()

                    _WorldServer.Log.WriteLine(LogType.USER, "[{0}:{1}] Player login complete [0x{2:X}]", client.IP, client.Port, guid)
                Catch e As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "Error on login: {0}", e.ToString)
                End Try
            End Sub
            Public Sub ClientLogout(ByVal id As UInteger) Implements IWorld.ClientLogout
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client logout", id)

                _WorldServer.CLIENTs(id).Character.Logout(Nothing)
            End Sub
            Public Sub ClientPacket(ByVal id As UInteger, ByVal data() As Byte) Implements IWorld.ClientPacket
                If data Is Nothing Then Throw New ApplicationException("Packet doesn't contain data!")
                Dim p As New Packets.PacketClass(data)
                Try
                    If _WorldServer.CLIENTs.ContainsKey(id) = False Then _WorldServer.Log.WriteLine(LogType.FAILED, "Client ID doesn't contain a key!: {0}", ToString)
                    _WorldServer.CLIENTs(id).Packets.Enqueue(p)
                    ThreadPool.QueueUserWorkItem(AddressOf _WorldServer.CLIENTs(id).OnPacket)
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "Error on Client OnPacket: {0}", ex.ToString)
                Finally
                    p.Dispose()
                    '_WorldServer.CLIENTs(id).Dispose()
                End Try
            End Sub

            Public Function ClientCreateCharacter(ByVal account As String, ByVal name As String, ByVal race As Byte, ByVal classe As Byte, ByVal gender As Byte, ByVal skin As Byte, ByVal face As Byte, ByVal hairStyle As Byte, ByVal hairColor As Byte, ByVal facialHair As Byte, ByVal outfitId As Byte) As Integer Implements IWorld.ClientCreateCharacter
                _WorldServer.Log.WriteLine(LogType.INFORMATION, "Account {0} Created a character with Name {1}, Race {2}, Class {3}, Gender {4}, Skin {5}, Face {6}, HairStyle {7}, HairColor {8}, FacialHair {9}, outfitID {10}", account, name, race, classe, gender, skin, face, hairStyle, hairColor, facialHair, outfitId)
                Return CreateCharacter(account, name, race, classe, gender, skin, face, hairStyle, hairColor, facialHair, outfitId)
            End Function

            Public Function Ping(ByVal timestamp As Integer, ByVal latency As Integer) As Integer Implements IWorld.Ping
                _WorldServer.Log.WriteLine(LogType.INFORMATION, "Cluster ping: [{0}ms]", _NativeMethods.timeGetTime("") - timestamp)
                LastPing = _NativeMethods.timeGetTime("")
                WC_MsTime = timestamp + latency

                Return _NativeMethods.timeGetTime("")
            End Function

            Public Sub CheckConnection(ByVal State As Object)
                If (_NativeMethods.timeGetTime("") - LastPing) > 40000 Then
                    If Cluster IsNot Nothing Then
                        _WorldServer.Log.WriteLine(LogType.FAILED, "Cluster timed out. Reconnecting")
                        ClusterDisconnect()
                    End If
                    ClusterConnect()
                    LastPing = _NativeMethods.timeGetTime("")
                End If
            End Sub

            Public Sub CheckCPU(ByVal State As Object)
                Dim TimeSinceLastCheck As TimeSpan = Now.Subtract(LastInfo)
                UsageCPU = ((Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds - LastCPUTime) / TimeSinceLastCheck.TotalMilliseconds) * 100
                LastInfo = Now
                LastCPUTime = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds
            End Sub

            Public Function GetServerInfo() As ServerInfo Implements IWorld.GetServerInfo
                Dim serverInfo As New ServerInfo
                serverInfo.cpuUsage = UsageCPU
                serverInfo.memoryUsage = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024)
                Return serverInfo
            End Function

            Public Sub InstanceCreate(ByVal MapID As UInteger) Implements IWorld.InstanceCreate
                If WS_Maps.Maps.ContainsKey(MapID) = False Then
                    Dim Map As New WS_Maps.TMap(MapID)
                    'The New does a an add to the .Containskey collection above
                End If
            End Sub
            Public Sub InstanceDestroy(ByVal MapID As UInteger) Implements IWorld.InstanceDestroy
                WS_Maps.Maps(MapID).Dispose()
            End Sub
            Public Function InstanceCanCreate(ByVal Type As Integer) As Boolean Implements IWorld.InstanceCanCreate
                Select Case Type
                    Case MapTypes.MAP_BATTLEGROUND
                        Return _WorldServer.Config.CreateBattlegrounds
                    Case MapTypes.MAP_INSTANCE
                        Return _WorldServer.Config.CreatePartyInstances
                    Case MapTypes.MAP_RAID
                        Return _WorldServer.Config.CreateRaidInstances
                    Case MapTypes.MAP_COMMON
                        Return _WorldServer.Config.CreateOther
                End Select
            End Function

            Public Sub ClientSetGroup(ByVal ID As UInteger, ByVal GroupID As Long) Implements IWorld.ClientSetGroup
                If Not _WorldServer.CLIENTs.ContainsKey(ID) Then Return

                If GroupID = -1 Then
                    _WorldServer.Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client group set [G NULL]", ID)

                    _WorldServer.CLIENTs(ID).Character.Group = Nothing
                    InstanceMapLeave(_WorldServer.CLIENTs(ID).Character)
                Else
                    _WorldServer.Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client group set [G{1:00000}]", ID, GroupID)

                    If Not WS_Group.Groups.ContainsKey(GroupID) Then
                        Dim Group As New WS_Group.Group(GroupID)
                        'The New does a an add to the .Containskey collection above
                        'Groups.Add(GroupID, Group)
                        Cluster.GroupRequestUpdate(ID)
                    End If

                    _WorldServer.CLIENTs(ID).Character.Group = WS_Group.Groups(GroupID)
                    InstanceMapEnter(_WorldServer.CLIENTs(ID).Character)
                End If
            End Sub
            Public Sub GroupUpdate(ByVal GroupID As Long, ByVal GroupType As Byte, ByVal GroupLeader As ULong, ByVal Members() As ULong) Implements IWorld.GroupUpdate
                If WS_Group.Groups.ContainsKey(GroupID) Then

                    Dim list As New List(Of ULong)
                    For Each GUID As ULong In Members
                        If _WorldServer.CHARACTERs.ContainsKey(GUID) Then list.Add(GUID)
                    Next

                    _WorldServer.Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update [{2}, {1} local members]", GroupID, list.Count, CType(GroupType, GroupType))

                    If list.Count = 0 Then
                        WS_Group.Groups(GroupID).Dispose()
                    Else
                        WS_Group.Groups(GroupID).Type = GroupType
                        WS_Group.Groups(GroupID).Leader = GroupLeader
                        WS_Group.Groups(GroupID).LocalMembers = list
                    End If
                End If
            End Sub
            Public Sub GroupUpdateLoot(ByVal GroupID As Long, ByVal Difficulty As Byte, ByVal Method As Byte, ByVal Threshold As Byte, ByVal Master As ULong) Implements IWorld.GroupUpdateLoot
                If WS_Group.Groups.ContainsKey(GroupID) Then

                    _WorldServer.Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update loot", GroupID)

                    WS_Group.Groups(GroupID).DungeonDifficulty = Difficulty
                    WS_Group.Groups(GroupID).LootMethod = Method
                    WS_Group.Groups(GroupID).LootThreshold = Threshold

                    If _WorldServer.CHARACTERs.ContainsKey(Master) Then
                        WS_Group.Groups(GroupID).LocalLootMaster = _WorldServer.CHARACTERs(Master)
                    Else
                        WS_Group.Groups(GroupID).LocalLootMaster = Nothing
                    End If
                End If
            End Sub

            Public Function GroupMemberStats(ByVal GUID As ULong, ByVal Flag As Integer) As Byte() Implements IWorld.GroupMemberStats
                If Flag = 0 Then Flag = PartyMemberStatsFlag.GROUP_UPDATE_FULL
                Dim p As PacketClass = BuildPartyMemberStats(_WorldServer.CHARACTERs(GUID), Flag)
                p.UpdateLength()
                Return p.Data
            End Function

            Public Sub GuildUpdate(ByVal GUID As ULong, ByVal GuildID As UInteger, ByVal GuildRank As Byte) Implements IWorld.GuildUpdate
                _WorldServer.CHARACTERs(GUID).GuildID = GuildID
                _WorldServer.CHARACTERs(GUID).GuildRank = GuildRank

                _WorldServer.CHARACTERs(GUID).SetUpdateFlag(EPlayerFields.PLAYER_GUILDID, GuildID)
                _WorldServer.CHARACTERs(GUID).SetUpdateFlag(EPlayerFields.PLAYER_GUILDRANK, GuildRank)
                _WorldServer.CHARACTERs(GUID).SendCharacterUpdate()
            End Sub

            Public Sub BattlefieldCreate(ByVal BattlefieldID As Integer, ByVal BattlefieldMapType As Byte, ByVal Map As UInteger) Implements IWorld.BattlefieldCreate
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Battlefield created", BattlefieldID)
            End Sub
            Public Sub BattlefieldDelete(ByVal BattlefieldID As Integer) Implements IWorld.BattlefieldDelete
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Battlefield deleted", BattlefieldID)
            End Sub
            Public Sub BattlefieldJoin(ByVal BattlefieldID As Integer, ByVal GUID As ULong) Implements IWorld.BattlefieldJoin
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Character [0x{1:X}] joined battlefield", BattlefieldID, GUID)
            End Sub
            Public Sub BattlefieldLeave(ByVal BattlefieldID As Integer, ByVal GUID As ULong) Implements IWorld.BattlefieldLeave
                _WorldServer.Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Character [0x{1:X}] left battlefield", BattlefieldID, GUID)
            End Sub

        End Class

        Class ClientClass
            Inherits ClientInfo
            Implements IDisposable

            Public Character As CharacterObject
            Public Packets As New Queue(Of PacketClass)

            Public DEBUG_CONNECTION As Boolean = False

            ''' <summary>
            ''' Called when a packet is recieved.
            ''' </summary>
            ''' <param name="state">The state.</param>
            ''' <returns></returns>
            Public Sub OnPacket(state As Object)
                While Packets.Count >= 1
                    Try ' Trap a Packets.Dequeue issue when no packets are queued... possibly an error with the Packets.Count above'
                        Dim p As PacketClass = Packets.Dequeue
                        Dim start As Integer = _NativeMethods.timeGetTime("")
                        Try
                            If Not IsNothing(p) Then
                                If _WorldServer.PacketHandlers.ContainsKey(p.OpCode) = True Then
                                    Try
                                        _WorldServer.PacketHandlers(p.OpCode).Invoke(p, Me)
                                        If _NativeMethods.timeGetTime("") - start > 100 Then
                                            _WorldServer.Log.WriteLine(LogType.WARNING, "Packet processing took too long: {0}, {1}ms", p.OpCode, _NativeMethods.timeGetTime("") - start)
                                        End If
                                    Catch e As Exception 'TargetInvocationException
                                        _WorldServer.Log.WriteLine(LogType.FAILED, "Opcode handler {2}:{3} caused an error:{1}{0}", e.ToString, Environment.NewLine, p.OpCode, p.OpCode)
                                        If Not IsNothing(p) Then DumpPacket(p.Data, Me)
                                    End Try
                                Else
                                    _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [DataLen={3} {4}]", IP, Port, CType(p.OpCode, Integer), p.Data.Length, p.OpCode)
                                    If Not IsNothing(p) Then DumpPacket(p.Data, Me)
                                End If
                            Else
                                _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] No Packet Information in Queue", IP, Port)
                                If Not IsNothing(p) Then DumpPacket(p.Data, Me)
                            End If
                        Catch err As Exception
                            _WorldServer.Log.WriteLine(LogType.FAILED, "Connection from [{0}:{1}] cause error {2}{3}", IP, Port, err.ToString, Environment.NewLine)
                            Delete()
                        Finally
                            Try
                            Catch ex As Exception
                                If Packets.Count = 0 Then p.Dispose()
                                _WorldServer.Log.WriteLine(LogType.WARNING, "Unable to dispose of packet: {0}", p.OpCode)
                                If Not IsNothing(p) Then DumpPacket(p.Data, Me)
                            End Try
                        End Try
                    Catch err As Exception
                        _WorldServer.Log.WriteLine(LogType.FAILED, "Connection from [{0}:{1}] cause error {2}{3}", IP, Port, err.ToString, Environment.NewLine)
                        Delete()
                    Finally
                        Try
                        Catch ex As Exception
                            'If Packets.Count = 0 Then p.Dispose()
                            _WorldServer.Log.WriteLine(LogType.WARNING, "Unable to dispose of packet")
                            'DumpPacket(p.Data, Me)
                        End Try
                    End Try
                End While
            End Sub

            Public Sub Send(ByRef data() As Byte)
                SyncLock Me
                    Try
                        _WorldServer.ClsWorldServer.Cluster.ClientSend(Index, data)
                    Catch Err As Exception
                        If DEBUG_CONNECTION Then Exit Sub
                        _WorldServer.Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {3}{2}", IP, Port, Err.ToString, Environment.NewLine)
                        _WorldServer.ClsWorldServer.Cluster = Nothing
                        Delete()
                    End Try
                End SyncLock
            End Sub
            Public Sub Send(ByRef packet As PacketClass)
                If packet Is Nothing Then Throw New ApplicationException("Packet doesn't contain data!")
                SyncLock Me
                    Try
                        If packet.OpCode = OPCODES.SMSG_UPDATE_OBJECT Then packet.CompressUpdatePacket()
                        packet.UpdateLength()

                        _WorldServer.ClsWorldServer.Cluster.ClientSend(Index, packet.Data)
                        packet.Dispose()
                    Catch Err As Exception
                        If DEBUG_CONNECTION Then Exit Sub
                        _WorldServer.Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {3}{2}", IP, Port, Err.ToString, Environment.NewLine)
                        _WorldServer.ClsWorldServer.Cluster = Nothing
                        Delete()
                    End Try
                End SyncLock
            End Sub
            Public Sub SendMultiplyPackets(ByRef packet As PacketClass)
                If packet Is Nothing Then Throw New ApplicationException("Packet doesn't contain data!")
                SyncLock Me
                    Try
                        If packet.OpCode = OPCODES.SMSG_UPDATE_OBJECT Then packet.CompressUpdatePacket()
                        packet.UpdateLength()

                        Dim data() As Byte = packet.Data.Clone
                        _WorldServer.ClsWorldServer.Cluster.ClientSend(Index, data)

                    Catch Err As Exception
                        If DEBUG_CONNECTION Then Exit Sub
                        _WorldServer.Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {3}{2}", IP, Port, Err.ToString, Environment.NewLine)
                        _WorldServer.ClsWorldServer.Cluster = Nothing
                        Delete()
                    End Try
                End SyncLock
            End Sub

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    _WorldServer.Log.WriteLine(LogType.NETWORK, "Connection from [{0}:{1}] disposed", IP, Port)

                    _WorldServer.ClsWorldServer.Cluster.ClientDrop(Index)
                    _WorldServer.CLIENTs.Remove(Index)
                    If Not Character Is Nothing Then
                        Character.client = Nothing
                        Character.Dispose()
                    End If
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

            Public Sub Delete()
                On Error Resume Next

                _WorldServer.CLIENTs.Remove(Index)
                If Not Character Is Nothing Then
                    Character.client = Nothing
                    Character.Dispose()
                End If
                Dispose()
            End Sub
            Public Sub Disconnect()
                Delete()
            End Sub

            Public Sub New()
                _WorldServer.Log.WriteLine(LogType.WARNING, "Creating debug connection!", Nothing)
                DEBUG_CONNECTION = True
            End Sub
            Public Sub New(ByVal ci As ClientInfo)
                Access = ci.Access
                Account = ci.Account
                Index = ci.Index
                IP = ci.IP
                Port = ci.Port
            End Sub
        End Class

    End Module
End NameSpace