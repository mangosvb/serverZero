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

Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.AspNetCore.SignalR
Imports Mangos.SignalR
Imports Mangos.Common
Imports Mangos.Common.Globals
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Handlers
Imports Mangos.Common.Enums.Authentication
Imports Mangos.Common.Enums.Global

Namespace Server
    Public Class WC_Network
        Public WorldServer As WorldServerClass

        Private ReadOnly LastPing As Integer = 0

        Public Function MsTime() As Integer
            'DONE: Calculate the clusters timeGetTime("")
            Return (_NativeMethods.timeGetTime("") - LastPing)
        End Function

        Class WorldServerClass
            Inherits Hub
            Implements ICluster
            Implements IDisposable

            Public m_flagStopListen As Boolean = False
            Private ReadOnly m_TimerPing As Timer
            Private ReadOnly m_TimerStats As Timer
            Private ReadOnly m_TimerCPU As Timer

            Private ReadOnly m_Socket As Socket

            Public Sub New()
                Try
                    m_Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    Dim configuration = _ConfigurationProvider.GetConfiguration()
                    m_Socket.Bind(New IPEndPoint(IPAddress.Parse(configuration.WorldClusterAddress), configuration.WorldClusterPort))
                    m_Socket.Listen(5)
                    m_Socket.BeginAccept(AddressOf AcceptConnection, Nothing)

                    _WorldCluster.Log.WriteLine(LogType.SUCCESS, "Listening on {0} on port {1}", IPAddress.Parse(configuration.WorldClusterAddress), configuration.WorldClusterPort)

                    'Creating ping timer
                    m_TimerPing = New Timer(AddressOf Ping, Nothing, 0, 15000)

                    'Creating stats timer
                    If configuration.StatsEnabled Then
                        m_TimerStats = New Timer(AddressOf _WC_Stats.GenerateStats, Nothing, configuration.StatsTimer, configuration.StatsTimer)
                    End If

                    'Creating CPU check timer
                    m_TimerCPU = New Timer(AddressOf _WC_Stats.CheckCpu, Nothing, 1000, 1000)

                Catch e As Exception
                    Console.WriteLine()
                    _WorldCluster.Log.WriteLine(LogType.FAILED, "Error in {1}: {0}.", e.Message, e.Source)
                End Try
            End Sub

            Protected Sub AcceptConnection(ByVal ar As IAsyncResult)
                If m_flagStopListen Then Return

                Dim m_Client As New ClientClass With {
                    .Socket = m_Socket.EndAccept(ar)
                }

                m_Client.Socket.NoDelay = True

                m_Socket.BeginAccept(AddressOf AcceptConnection, Nothing)

                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf m_Client.OnConnect))
            End Sub

#Region "IDisposable Support"
            Private ReadOnly _disposedValue As Boolean ' To detect redundant calls

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Overloads Sub Dispose() Implements IDisposable.Dispose
                m_flagStopListen = True
                m_Socket.Close()
                m_TimerPing.Dispose()
                m_TimerStats.Dispose()
                m_TimerCPU.Dispose()
                GC.SuppressFinalize(Me)
            End Sub
#End Region
            Public Worlds As New Dictionary(Of UInteger, IWorld)
            Public WorldsInfo As New Dictionary(Of UInteger, WorldInfo)

            Public Function Connect(ByVal uri As String, ByVal maps As List(Of UInteger)) As Boolean Implements ICluster.Connect
                Try
                    Disconnect(uri, maps)

                    Dim WorldServerInfo As New WorldInfo
                    _WorldCluster.Log.WriteLine(LogType.INFORMATION, "Connected Map Server: {0}", uri)

                    SyncLock CType(Worlds, ICollection).SyncRoot
                        For Each Map As UInteger In maps

                            'NOTE: Password protected remoting
                            Worlds(Map) = ProxyClient.Create(Of IWorld)(uri)
                            WorldsInfo(Map) = WorldServerInfo
                        Next
                    End SyncLock

                Catch ex As Exception
                    _WorldCluster.Log.WriteLine(LogType.CRITICAL, "Unable to reverse connect. [{0}]", ex.ToString)
                    Return False
                End Try

                Return True
            End Function

            Public Sub Disconnect(uri As String, maps As List(Of UInteger)) Implements ICluster.Disconnect
                If maps.Count = 0 Then Return

                'TODO: Unload arenas or battlegrounds that is hosted on this server!
                For Each map As UInteger In maps

                    'DONE: Disconnecting clients
                    SyncLock CType(_WorldCluster.CLIENTs, ICollection).SyncRoot
                        For Each objCharacter As KeyValuePair(Of UInteger, ClientClass) In _WorldCluster.CLIENTs
                            If Not objCharacter.Value.Character Is Nothing AndAlso
                               objCharacter.Value.Character.IsInWorld AndAlso
                               objCharacter.Value.Character.Map = map Then
                                objCharacter.Value.Send(New Packets.PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE))
                                Call New Packets.PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE).Dispose()

                                objCharacter.Value.Character.Dispose()
                                objCharacter.Value.Character = Nothing
                            End If
                        Next
                    End SyncLock

                    If Worlds.ContainsKey(map) Then
                        Try
                            Worlds(map) = Nothing
                            WorldsInfo(map) = Nothing
                        Catch
                        Finally
                            SyncLock CType(Worlds, ICollection).SyncRoot
                                Worlds.Remove(map)
                                WorldsInfo.Remove(map)
                                _WorldCluster.Log.WriteLine(LogType.INFORMATION, "Map: {0:000} has been disconnected!", map)
                            End SyncLock
                        End Try
                    End If
                Next

            End Sub

            Public Sub Ping(ByVal State As Object)
                Dim DownedServers As New List(Of UInteger)
                Dim SentPingTo As New Dictionary(Of WorldInfo, Integer)

                Dim MyTime As Integer
                Dim ServerTime As Integer
                Dim Latency As Integer

                'Ping WorldServers
                SyncLock CType(Worlds, ICollection).SyncRoot
                    For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                        Try
                            If Not SentPingTo.ContainsKey(WorldsInfo(w.Key)) Then
                                MyTime = _NativeMethods.timeGetTime("")
                                ServerTime = w.Value.Ping(MyTime, WorldsInfo(w.Key).Latency)
                                Latency = Math.Abs(MyTime - _NativeMethods.timeGetTime(""))

                                WorldsInfo(w.Key).Latency = Latency
                                SentPingTo(WorldsInfo(w.Key)) = Latency

                                _WorldCluster.Log.WriteLine(LogType.NETWORK, "Map {0:000} ping: {1}ms", w.Key, Latency)

                                'Query CPU and Memory usage
                                Dim serverInfo = w.Value.GetServerInfo()
                                WorldsInfo(w.Key).CPUUsage = serverInfo.cpuUsage
                                WorldsInfo(w.Key).MemoryUsage = serverInfo.memoryUsage
                            Else
                                _WorldCluster.Log.WriteLine(LogType.NETWORK, "Map {0:000} ping: {1}ms", w.Key, SentPingTo(WorldsInfo(w.Key)))
                            End If

                        Catch ex As Exception
                            _WorldCluster.Log.WriteLine(LogType.WARNING, "Map {0:000} is currently down!", w.Key)
                            DownedServers.Add(w.Key)
                        End Try
                    Next
                End SyncLock

                'Notification message
                If Worlds.Count = 0 Then _WorldCluster.Log.WriteLine(LogType.WARNING, "No maps are currently available!")

                'Drop WorldServers
                Disconnect("NULL", DownedServers)
            End Sub

            Public Sub ClientSend(ByVal id As UInteger, ByVal data() As Byte) Implements ICluster.ClientSend
                If _WorldCluster.CLIENTs.ContainsKey(id) Then _WorldCluster.CLIENTs(id).Send(data)
            End Sub

            Public Sub ClientDrop(ByVal ID As UInteger) Implements ICluster.ClientDrop
                If _WorldCluster.CLIENTs.ContainsKey(ID) Then
                    Try
                        _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has dropped map {1:000}", ID, _WorldCluster.CLIENTs(ID).Character.Map)
                        _WorldCluster.CLIENTs(ID).Character.IsInWorld = False
                        _WorldCluster.CLIENTs(ID).Character.OnLogout()
                    Catch ex As Exception
                        _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has dropped an exception: {1}", ID, ex.ToString)
                    End Try
                Else
                    _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client connection has been lost.", ID)
                End If
            End Sub

            Public Sub ClientTransfer(ByVal ID As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As UInteger) Implements ICluster.ClientTransfer
                _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has transferred from map {1:000} to map {2:000}", ID, _WorldCluster.CLIENTs(ID).Character.Map, map)

                Dim p As New Packets.PacketClass(OPCODES.SMSG_NEW_WORLD)
                p.AddUInt32(map)
                p.AddSingle(posX)
                p.AddSingle(posY)
                p.AddSingle(posZ)
                p.AddSingle(ori)
                _WorldCluster.CLIENTs(ID).Send(p)

                _WorldCluster.CLIENTs(ID).Character.Map = map
            End Sub

            Public Sub ClientUpdate(ByVal ID As UInteger, ByVal zone As UInteger, ByVal level As Byte) Implements ICluster.ClientUpdate
                If _WorldCluster.CLIENTs(ID).Character Is Nothing Then Return
                _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has an updated zone {1:000}", ID, zone)

                _WorldCluster.CLIENTs(ID).Character.Zone = zone
                _WorldCluster.CLIENTs(ID).Character.Level = level
            End Sub

            Public Sub ClientSetChatFlag(ByVal ID As UInteger, ByVal flag As Byte) Implements ICluster.ClientSetChatFlag
                If _WorldCluster.CLIENTs(ID).Character Is Nothing Then Return
                _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0:000000}] Client chat flag update [0x{1:X}]", ID, flag)

                _WorldCluster.CLIENTs(ID).Character.ChatFlag = flag
            End Sub

            Public Function ClientGetCryptKey(ByVal ID As UInteger) As Byte() Implements ICluster.ClientGetCryptKey
                _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0:000000}] Requested client crypt key", ID)
                Return _WorldCluster.CLIENTs(ID).SS_Hash
            End Function

            Public Sub Broadcast(ByVal Data() As Byte) Implements ICluster.Broadcast
                Dim b As Byte()
                _WorldCluster.CHARACTERs_Lock.AcquireReaderLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                For Each objCharacter As KeyValuePair(Of ULong, WcHandlerCharacter.CharacterObject) In _WorldCluster.CHARACTERs

                    If objCharacter.Value.IsInWorld AndAlso objCharacter.Value.Client IsNot Nothing Then
                        b = Data.Clone
                        objCharacter.Value.Client.Send(Data)
                    End If

                Next
                _WorldCluster.CHARACTERs_Lock.ReleaseReaderLock()
            End Sub

            Public Sub BroadcastGroup(ByVal groupId As Long, ByVal Data() As Byte) Implements ICluster.BroadcastGroup
                With _WC_Handlers_Group.GROUPs(groupId)
                    For i As Byte = 0 To .Members.Length - 1
                        If .Members(i) IsNot Nothing Then
                            Call .Members(i).Client.Send(CType(Data.Clone, Byte()))
                        End If

                    Next
                End With
            End Sub

            Public Sub BroadcastRaid(ByVal GroupID As Long, ByVal Data() As Byte) Implements ICluster.BroadcastGuild
                With _WC_Handlers_Group.GROUPs(GroupID)
                    For i As Byte = 0 To .Members.Length - 1
                        If .Members(i) IsNot Nothing AndAlso .Members(i).Client IsNot Nothing Then
                            Call .Members(i).Client.Send(CType(Data.Clone, Byte()))
                        End If

                    Next
                End With
            End Sub

            Public Sub BroadcastGuild(ByVal GuildID As Long, ByVal Data() As Byte) Implements ICluster.BroadcastGuildOfficers
                'TODO: Not implement yet
            End Sub

            Public Sub BroadcastGuildOfficers(ByVal GuildID As Long, ByVal Data() As Byte) Implements ICluster.BroadcastRaid
                'TODO: Not implement yet
            End Sub

            Public Function InstanceCheck(ByVal client As ClientClass, ByVal MapID As UInteger) As Boolean
                If Not _WC_Network.WorldServer.Worlds.ContainsKey(MapID) Then
                    'We don't create new continents
                    If _Functions.IsContinentMap(MapID) Then
                        _WorldCluster.Log.WriteLine(LogType.WARNING, "[{0:000000}] Requested Instance Map [{1}] is a continent", client.Index, MapID)

                        client.Send(New Packets.PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE))
                        Call New Packets.PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE).Dispose()

                        client.Character.IsInWorld = False
                        Return False
                    End If

                    _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Requesting Instance Map [{1}]", client.Index, MapID)
                    Dim ParentMap As IWorld = Nothing
                    Dim ParentMapInfo As WorldInfo = Nothing

                    'Check if we got parent map
                    If _WC_Network.WorldServer.Worlds.ContainsKey(_WS_DBCDatabase.Maps(MapID).ParentMap) AndAlso _WC_Network.WorldServer.Worlds(_WS_DBCDatabase.Maps(MapID).ParentMap).InstanceCanCreate(_WS_DBCDatabase.Maps(MapID).Type) Then
                        ParentMap = _WC_Network.WorldServer.Worlds(_WS_DBCDatabase.Maps(MapID).ParentMap)
                        ParentMapInfo = _WC_Network.WorldServer.WorldsInfo(_WS_DBCDatabase.Maps(MapID).ParentMap)
                    ElseIf _WC_Network.WorldServer.Worlds.ContainsKey(0) AndAlso _WC_Network.WorldServer.Worlds(0).InstanceCanCreate(_WS_DBCDatabase.Maps(MapID).Type) Then
                        ParentMap = _WC_Network.WorldServer.Worlds(0)
                        ParentMapInfo = _WC_Network.WorldServer.WorldsInfo(0)
                    ElseIf _WC_Network.WorldServer.Worlds.ContainsKey(1) AndAlso _WC_Network.WorldServer.Worlds(1).InstanceCanCreate(_WS_DBCDatabase.Maps(MapID).Type) Then
                        ParentMap = _WC_Network.WorldServer.Worlds(1)
                        ParentMapInfo = _WC_Network.WorldServer.WorldsInfo(1)
                    End If

                    If ParentMap Is Nothing Then
                        _WorldCluster.Log.WriteLine(LogType.WARNING, "[{0:000000}] Requested Instance Map [{1}] can't be loaded", client.Index, MapID)

                        client.Send(New Packets.PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE))
                        Call New Packets.PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE).Dispose()

                        client.Character.IsInWorld = False
                        Return False
                    End If

                    ParentMap.InstanceCreate(MapID)
                    _WC_Network.WorldServer.Worlds.Add(MapID, ParentMap)
                    _WC_Network.WorldServer.WorldsInfo.Add(MapID, ParentMapInfo)
                    Return True
                Else
                    Return True
                End If

            End Function

            Public Function BattlefieldCheck(ByVal MapID As UInteger) As Boolean
                'Create map
                If Not _WC_Network.WorldServer.Worlds.ContainsKey(MapID) Then
                    _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[SERVER] Requesting battlefield map [{0}]", MapID)
                    Dim ParentMap As IWorld = Nothing
                    Dim ParentMapInfo As WorldInfo = Nothing

                    'Check if we got parent map
                    If _WC_Network.WorldServer.Worlds.ContainsKey(_WS_DBCDatabase.Maps(MapID).ParentMap) AndAlso _WC_Network.WorldServer.Worlds(_WS_DBCDatabase.Maps(MapID).ParentMap).InstanceCanCreate(_WS_DBCDatabase.Maps(MapID).Type) Then
                        ParentMap = _WC_Network.WorldServer.Worlds(_WS_DBCDatabase.Maps(MapID).ParentMap)
                        ParentMapInfo = _WC_Network.WorldServer.WorldsInfo(_WS_DBCDatabase.Maps(MapID).ParentMap)
                    ElseIf _WC_Network.WorldServer.Worlds.ContainsKey(0) AndAlso _WC_Network.WorldServer.Worlds(0).InstanceCanCreate(_WS_DBCDatabase.Maps(MapID).Type) Then
                        ParentMap = _WC_Network.WorldServer.Worlds(0)
                        ParentMapInfo = _WC_Network.WorldServer.WorldsInfo(0)
                    ElseIf _WC_Network.WorldServer.Worlds.ContainsKey(1) AndAlso _WC_Network.WorldServer.Worlds(1).InstanceCanCreate(_WS_DBCDatabase.Maps(MapID).Type) Then
                        ParentMap = _WC_Network.WorldServer.Worlds(1)
                        ParentMapInfo = _WC_Network.WorldServer.WorldsInfo(1)
                    End If

                    If ParentMap Is Nothing Then
                        _WorldCluster.Log.WriteLine(LogType.WARNING, "[SERVER] Requested battlefield map [{0}] can't be loaded", MapID)
                        Return False
                    End If

                    ParentMap.InstanceCreate(MapID)
                    _WC_Network.WorldServer.Worlds.Add(MapID, ParentMap)
                    _WC_Network.WorldServer.WorldsInfo.Add(MapID, ParentMapInfo)
                    Return True
                Else
                    Return True
                End If
            End Function

            Public Function BattlefieldList(ByVal MapType As Byte) As List(Of Integer) Implements ICluster.BattlefieldList
                Dim BattlefieldMap As New List(Of Integer)

                _WC_Handlers_Battleground.BATTLEFIELDs_Lock.AcquireReaderLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                For Each BG As KeyValuePair(Of Integer, WC_Handlers_Battleground.Battlefield) In _WC_Handlers_Battleground.BATTLEFIELDs
                    If BG.Value.MapType = MapType Then
                        BattlefieldMap.Add(BG.Value.ID)
                    End If
                Next

                _WC_Handlers_Battleground.BATTLEFIELDs_Lock.ReleaseReaderLock()
                Return BattlefieldMap
            End Function

            Public Sub BattlefieldFinish(ByVal battlefieldId As Integer) Implements ICluster.BattlefieldFinish
                _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[B{0:0000}] Battlefield finished", battlefieldId)
            End Sub

            Public Sub GroupRequestUpdate(ByVal ID As UInteger) Implements ICluster.GroupRequestUpdate
                If _WorldCluster.CLIENTs.ContainsKey(ID) AndAlso _WorldCluster.CLIENTs(ID).Character IsNot Nothing AndAlso _WorldCluster.CLIENTs(ID).Character.IsInWorld AndAlso _WorldCluster.CLIENTs(ID).Character.IsInGroup Then

                    _WorldCluster.Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update request", _WorldCluster.CLIENTs(ID).Character.Group.Id)

                    Try
                        _WorldCluster.CLIENTs(ID).Character.GetWorld.GroupUpdate(_WorldCluster.CLIENTs(ID).Character.Group.Id, _WorldCluster.CLIENTs(ID).Character.Group.Type, _WorldCluster.CLIENTs(ID).Character.Group.GetLeader.Guid, _WorldCluster.CLIENTs(ID).Character.Group.GetMembers)
                        _WorldCluster.CLIENTs(ID).Character.GetWorld.GroupUpdateLoot(_WorldCluster.CLIENTs(ID).Character.Group.Id, _WorldCluster.CLIENTs(ID).Character.Group.DungeonDifficulty, _WorldCluster.CLIENTs(ID).Character.Group.LootMethod, _WorldCluster.CLIENTs(ID).Character.Group.LootThreshold, _WorldCluster.CLIENTs(ID).Character.Group.GetLootMaster.Guid)
                    Catch
                        _WC_Network.WorldServer.Disconnect("NULL", New List(Of UInteger)() From {_WorldCluster.CLIENTs(ID).Character.Map})
                    End Try
                End If
            End Sub

            Public Sub GroupSendUpdate(ByVal GroupID As Long)
                _WorldCluster.Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update", GroupID)

                SyncLock CType(Worlds, ICollection).SyncRoot
                    For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                        Try
                            w.Value.GroupUpdate(GroupID, _WC_Handlers_Group.GROUPs(GroupID).Type, _WC_Handlers_Group.GROUPs(GroupID).GetLeader.Guid, _WC_Handlers_Group.GROUPs(GroupID).GetMembers)
                        Catch ex As Exception
                            _WorldCluster.Log.WriteLine(LogType.FAILED, "[G{0:00000}] Group update failed for [M{1:000}]", GroupID, w.Key)
                        End Try
                    Next
                End SyncLock
            End Sub

            Public Sub GroupSendUpdateLoot(ByVal GroupID As Long)
                _WorldCluster.Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update loot", GroupID)

                SyncLock CType(Worlds, ICollection).SyncRoot

                    For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                        Try
                            w.Value.GroupUpdateLoot(GroupID, _WC_Handlers_Group.GROUPs(GroupID).DungeonDifficulty, _WC_Handlers_Group.GROUPs(GroupID).LootMethod, _WC_Handlers_Group.GROUPs(GroupID).LootThreshold, _WC_Handlers_Group.GROUPs(GroupID).GetLootMaster.Guid)
                        Catch ex As Exception
                            _WorldCluster.Log.WriteLine(LogType.FAILED, "[G{0:00000}] Group update loot failed for [M{1:000}]", GroupID, w.Key)
                        End Try
                    Next
                End SyncLock
            End Sub

        End Class

        Class WorldInfo
            Public Latency As Integer
            Public Started As Date = Now
            Public CPUUsage As Single
            Public MemoryUsage As ULong
        End Class

        Public LastConnections As New Dictionary(Of UInteger, Date)
        Class ClientClass
            Inherits ClientInfo
            Implements IDisposable

            Public Socket As Socket = Nothing
            Public Queue As New Queue
            Public Character As WcHandlerCharacter.CharacterObject = Nothing

            Public SS_Hash() As Byte
            Public Encryption As Boolean = False

            Protected SocketBuffer(8192) As Byte
            Protected SocketBytes As Integer

            Protected SavedBytes() As Byte = {}

            Public DEBUG_CONNECTION As Boolean = False
            Private ReadOnly Key() As Byte = {0, 0, 0, 0}

            Private HandingPackets As Boolean = False

            Public Function GetClientInfo() As ClientInfo
                Dim ci As New ClientInfo With {
                    .Access = Access,
                    .Account = Account,
                    .Index = Index,
                    .IP = IP,
                    .Port = Port
                }

                Return ci
            End Function

            Public Sub OnConnect(ByVal state As Object)
                If Socket Is Nothing Then Throw New ApplicationException("socket doesn't exist!")
                If _WorldCluster.CLIENTs Is Nothing Then Throw New ApplicationException("Clients doesn't exist!")

                Dim remoteEndPoint As IPEndPoint = CType(Socket.RemoteEndPoint, IPEndPoint)
                IP = remoteEndPoint.Address.ToString
                Port = remoteEndPoint.Port

                'DONE: Connection spam protection
                If _WC_Network.LastConnections.ContainsKey(_WC_Network.Ip2Int(IP.ToString)) Then
                    If Now > _WC_Network.LastConnections(_WC_Network.Ip2Int(IP.ToString)) Then
                        _WC_Network.LastConnections(_WC_Network.Ip2Int(IP.ToString)) = Now.AddSeconds(5)
                    Else
                        Socket.Close()
                        Dispose()
                        Exit Sub
                    End If
                Else
                    _WC_Network.LastConnections.Add(_WC_Network.Ip2Int(IP.ToString), Now.AddSeconds(5))
                End If

                _WorldCluster.Log.WriteLine(LogType.DEBUG, "Incoming connection from [{0}:{1}]", IP, Port)

                Socket.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketFlags.None, AddressOf OnData, Nothing)

                'Send Auth Challenge
                Dim p As New Packets.PacketClass(OPCODES.SMSG_AUTH_CHALLENGE)
                p.AddInt32(Index)
                Send(p)

                Index = Interlocked.Increment(_WorldCluster.CLIETNIDs)

                SyncLock CType(_WorldCluster.CLIENTs, ICollection).SyncRoot
                    _WorldCluster.CLIENTs.Add(Index, Me)
                End SyncLock

                _WC_Stats.ConnectionsIncrement()
            End Sub

            Public Sub OnData(ByVal ar As IAsyncResult)
                If Not Socket.Connected Then Return
                If _WC_Network.WorldServer.m_flagStopListen Then Return
                If ar Is Nothing Then Throw New ApplicationException("Value ar is empty!")
                If SocketBuffer Is Nothing Then Throw New ApplicationException("SocketBuffer is empty!")
                If Socket Is Nothing Then Throw New ApplicationException("Socket is Null!")
                If _WorldCluster.CLIENTs Is Nothing Then Throw New ApplicationException("Clients doesn't exist!")
                If Queue Is Nothing Then Throw New ApplicationException("Queue is Null!")
                If SavedBytes Is Nothing Then Throw New ApplicationException("SavedBytes is empty!")

                Try
                    SocketBytes = Socket.EndReceive(ar)
                    If SocketBytes = 0 Then
                        Dispose(SocketBytes)
                    Else
                        Interlocked.Add(_WC_Stats.DataTransferIn, SocketBytes)

                        While SocketBytes > 0
                            If SavedBytes.Length = 0 Then
                                If Encryption Then Decode(SocketBuffer)
                            Else
                                SocketBuffer = _Functions.Concat(SavedBytes, SocketBuffer)
                                SavedBytes = (New Byte() {})
                            End If

                            'Calculate Length from packet
                            Dim PacketLen As Integer = (SocketBuffer(1) + SocketBuffer(0) * 256) + 2

                            If SocketBytes < PacketLen Then
                                SavedBytes = New Byte(SocketBytes - 1) {}
                                Try
                                    Array.Copy(SocketBuffer, 0, SavedBytes, 0, SocketBytes)
                                Catch ex As Exception
                                    Dispose(SocketBytes)
                                    Socket.Dispose()
                                    Socket.Close()
                                    _WorldCluster.Log.WriteLine(LogType.CRITICAL, "[{0}:{1}] BAD PACKET {2}({3}) bytes, ", IP, Port, SocketBytes, PacketLen)
                                End Try
                                Exit While
                            End If

                            'Move packet to Data
                            Dim data(PacketLen - 1) As Byte
                            Array.Copy(SocketBuffer, data, PacketLen)

                            'Create packet and add it to queue
                            Dim p As New Packets.PacketClass(data)
                            SyncLock Queue.SyncRoot
                                Queue.Enqueue(p)
                            End SyncLock

                            Try
                                'Delete packet from buffer
                                SocketBytes -= PacketLen
                                Array.Copy(SocketBuffer, PacketLen, SocketBuffer, 0, SocketBytes)
                            Catch Ex As Exception
                                _WorldCluster.Log.WriteLine(LogType.CRITICAL, "[{0}:{1}] Could not delete packet from buffer! {2}({3}{4}) bytes, ", IP, Port, SocketBuffer, PacketLen, SocketBytes)
                            End Try

                        End While

                        If SocketBuffer.Length > 0 Then
                            Try
                                Socket.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketBytes, SocketFlags.None, AddressOf OnData, Nothing)

                                If HandingPackets = False Then ThreadPool.QueueUserWorkItem(AddressOf OnPacket)
                            Catch ex As Exception
                                _WorldCluster.Log.WriteLine(LogType.WARNING, "Packet Disconnect from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbCrLf)
                            End Try
                        End If
                    End If
                Catch Err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    _WorldCluster.Log.WriteLine(LogType.WARNING, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbCrLf)

                    Dispose(SocketBuffer.Length)
                    Dispose(HandingPackets)
                End Try
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Sub OnPacket(state As Object)
                If SocketBuffer Is Nothing Then Throw New ApplicationException("SocketBuffer is empty!")
                If Socket Is Nothing Then Throw New ApplicationException("Socket is Null!")
                If _WorldCluster.CLIENTs Is Nothing Then Throw New ApplicationException("Clients doesn't exist!")
                If Queue Is Nothing Then Throw New ApplicationException("Queue is Null!")
                If SavedBytes Is Nothing Then Throw New ApplicationException("SavedBytes is empty!")
                If _WorldCluster.PacketHandlers Is Nothing Then Throw New ApplicationException("PacketHandler is empty!")

                Try
                Catch ex As Exception
                    HandingPackets = True
                    _WorldCluster.Log.WriteLine(LogType.FAILED, "Handing Packets Failed: {0}", HandingPackets)
                End Try
                While Queue.Count > 0

                    Dim p As Packets.PacketClass
                    SyncLock Queue.SyncRoot
                        p = Queue.Dequeue
                    End SyncLock

                    If _ConfigurationProvider.GetConfiguration().PacketLogging Then _Packets.LogPacket(p.Data, False, Me)
                    If _WorldCluster.PacketHandlers.ContainsKey(p.OpCode) <> True Then
                        If Character Is Nothing OrElse Character.IsInWorld = False Then
                            Socket.Dispose()
                            Socket.Close()
                            _WorldCluster.Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", IP, Port, p.OpCode, vbCrLf, p.Length)
                            _Packets.DumpPacket(p.Data, Me)
                        Else
                            Try
                                Character.GetWorld.ClientPacket(Index, p.Data)
                            Catch
                                _WC_Network.WorldServer.Disconnect("NULL", New List(Of UInteger)() From {Character.Map})
                            End Try
                        End If

                    Else
                        Try
                            _WorldCluster.PacketHandlers(p.OpCode).Invoke(p, Me)
                        Catch e As Exception
                            _WorldCluster.Log.WriteLine(LogType.FAILED, "Opcode handler {2}:{2:X} caused an error: {1}{0}", e.ToString, vbCrLf, p.OpCode)
                        End Try
                    End If
                    Try
                    Catch ex As Exception
                        If Queue.Count = 0 Then p.Dispose()
                        _WorldCluster.Log.WriteLine(LogType.WARNING, "Unable to dispose of packet: {0}", p.OpCode)
                        _Packets.DumpPacket(p.Data, Me)
                    End Try
                End While
                HandingPackets = False
            End Sub

            Public Sub Send(ByVal data() As Byte)
                If Not Socket.Connected Then Exit Sub

                Try
                    If _ConfigurationProvider.GetConfiguration().PacketLogging Then _Packets.LogPacket(data, True, Me)
                    If Encryption Then Encode(data)
                    Socket.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf OnSendComplete, Nothing)
                Catch Err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    _WorldCluster.Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbCrLf)
                    Delete()
                End Try
            End Sub

            Public Sub Send(ByRef packet As Packets.PacketClass)
                If IsNothing(packet) Then Throw New ApplicationException("Packet doesn't contain data!")
                If IsNothing(Socket) Or Socket.Connected = False Then Exit Sub

                Try
                    Dim data As Byte() = packet.Data
                    If _ConfigurationProvider.GetConfiguration().PacketLogging Then _Packets.LogPacket(data, True, Me)
                    If Encryption Then Encode(data)
                    Socket.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf OnSendComplete, Nothing)
                Catch err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    _WorldCluster.Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, err.ToString, vbCrLf)
                    Delete()
                End Try

                'Only attempt to dispose of the packet if it actually exists
                If Not IsNothing(packet) Then packet.Dispose()
            End Sub

            Public Sub SendMultiplyPackets(ByRef packet As Packets.PacketClass)
                If packet Is Nothing Then Throw New ApplicationException("Packet doesn't contain data!")

                If Not Socket.Connected Then Exit Sub

                Try
                    Dim data As Byte() = packet.Data.Clone
                    If _ConfigurationProvider.GetConfiguration().PacketLogging Then _Packets.LogPacket(data, True, Me)
                    If Encryption Then Encode(data)
                    Socket.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf OnSendComplete, Nothing)
                Catch Err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    _WorldCluster.Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbCrLf)
                    Delete()
                End Try

                'Don't forget to clean after using this function
            End Sub

            Public Sub OnSendComplete(ByVal ar As IAsyncResult)
                If Not Socket Is Nothing Then
                    Dim bytesSent As Integer = Socket.EndSend(ar)

                    Interlocked.Add(_WC_Stats.DataTransferOut, bytesSent)
                End If
            End Sub

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.

                    '                On Error Resume Next
                    'May have to trap and use exception handler rather than the on error resume next rubbish

                    If Not Socket Is Nothing Then Socket.Close()
                    Socket = Nothing

                    SyncLock CType(_WorldCluster.CLIENTs, ICollection).SyncRoot
                        _WorldCluster.CLIENTs.Remove(Index)
                    End SyncLock

                    If Not Character Is Nothing Then
                        If Character.IsInWorld Then
                            Character.IsInWorld = False
                            Character.GetWorld.ClientDisconnect(Index)
                        End If
                        Character.Dispose()
                    End If

                    Character = Nothing

                    _WC_Stats.ConnectionsDecrement()
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
                Socket.Close()
                Dispose()
            End Sub

            Public Sub Decode(ByRef data() As Byte)
                Dim tmp As Integer

                For i As Integer = 0 To 6 - 1
                    tmp = data(i)
                    data(i) = SS_Hash(Key(1)) Xor CByte((256 + data(i) - Key(0)) Mod 256)
                    Key(0) = tmp
                    Key(1) = (Key(1) + 1) Mod 40
                Next i
            End Sub

            Public Sub Encode(ByRef data() As Byte)
                For i As Integer = 0 To 4 - 1
                    data(i) = (CInt(SS_Hash(Key(3)) Xor data(i)) + Key(2)) Mod 256

                    Key(2) = data(i)
                    Key(3) = (Key(3) + 1) Mod 40
                Next i
            End Sub

            Public Sub EnQueue(ByVal state As Object)
                While _WorldCluster.CHARACTERs.Count > _ConfigurationProvider.GetConfiguration().ServerPlayerLimit
                    If Not Socket.Connected Then Exit Sub

                    Call New Packets.PacketClass(OPCODES.SMSG_AUTH_RESPONSE).AddInt8(LoginResponse.LOGIN_WAIT_QUEUE)
                    Call New Packets.PacketClass(OPCODES.SMSG_AUTH_RESPONSE).AddInt32(_WorldCluster.CLIENTs.Count - _WorldCluster.CHARACTERs.Count)            'amount of players in queue
                    Send(New Packets.PacketClass(OPCODES.SMSG_AUTH_RESPONSE))

                    _WorldCluster.Log.WriteLine(LogType.INFORMATION, "[{1}:{2}] AUTH_WAIT_QUEUE: Server player limit reached!", IP, Port)
                    Thread.Sleep(6000)
                End While
                _WC_Handlers_Auth.SendLoginOk(Me)
            End Sub
        End Class

        Private Function Ip2Int(ByVal ip As String) As UInteger
            If ip.Split(".").Length <> 4 Then Return 0

            Try
                Dim ipBytes(3) As Byte
                ipBytes(0) = ip.Split(".")(3)
                ipBytes(1) = ip.Split(".")(2)
                ipBytes(2) = ip.Split(".")(1)
                ipBytes(3) = ip.Split(".")(0)
                Return BitConverter.ToUInt32(ipBytes, 0)
            Catch
                Return 0
            End Try
        End Function

    End Class
End Namespace
