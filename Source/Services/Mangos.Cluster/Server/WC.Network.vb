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

Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.AspNetCore.SignalR
Imports Mangos.SignalR
Imports Mangos.Common
Imports Mangos.Common.Globals
Imports Mangos.Shared
Imports Mangos.Cluster.DataStores
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Handlers

Namespace Server
    Public Module WC_Network
        Public WorldServer As WorldServerClass

        Private ReadOnly LastPing As Integer = 0

        Public Function MsTime() As Integer
            'DONE: Calculate the clusters timeGetTime("")
            Return (timeGetTime("") - LastPing)
        End Function

        Class WorldServerClass
            Inherits Hub
            Implements ICluster
            Implements IDisposable

            <CLSCompliant(False)>
            Public m_flagStopListen As Boolean = False
            Private m_TimerPing As Timer
            Private m_TimerStats As Timer
            Private m_TimerCPU As Timer

            Private m_Socket As Socket

            Public Sub New()
                Try
                    m_Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    m_Socket.Bind(New IPEndPoint(IPAddress.Parse(Config.WorldClusterAddress), Config.WorldClusterPort))
                    m_Socket.Listen(5)
                    m_Socket.BeginAccept(AddressOf AcceptConnection, Nothing)

                    Log.WriteLine(LogType.SUCCESS, "Listening on {0} on port {1}", IPAddress.Parse(Config.WorldClusterAddress), Config.WorldClusterPort)

                    'Creating ping timer
                    m_TimerPing = New Timer(AddressOf Ping, Nothing, 0, 15000)

                    'Creating stats timer
                    If Config.StatsEnabled Then
                        m_TimerStats = New Timer(AddressOf GenerateStats, Nothing, Config.StatsTimer, Config.StatsTimer)
                    End If

                    'Creating CPU check timer
                    m_TimerCPU = New Timer(AddressOf CheckCpu, Nothing, 1000, 1000)

                Catch e As Exception
                    Console.WriteLine()
                    Log.WriteLine(LogType.FAILED, "Error in {1}: {0}.", e.Message, e.Source)
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
            Private _disposedValue As Boolean ' To detect redundant calls

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
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
                    Log.WriteLine(LogType.INFORMATION, "Connected Map Server: {0}", uri)

                    SyncLock CType(Worlds, ICollection).SyncRoot
                        For Each Map As UInteger In maps

                            'NOTE: Password protected remoting
                            Worlds(Map) = ProxyClient.Create(Of IWorld)(uri)
                            WorldsInfo(Map) = WorldServerInfo
                        Next
                    End SyncLock

                Catch ex As Exception
                    Log.WriteLine(LogType.CRITICAL, "Unable to reverse connect. [{0}]", ex.ToString)
                    Return False
                End Try

                Return True
            End Function

            Public Sub Disconnect(uri As String, maps As List(Of UInteger)) Implements ICluster.Disconnect
                If maps.Count = 0 Then Return

                'TODO: Unload arenas or battlegrounds that is hosted on this server!
                For Each map As UInteger In maps

                    'DONE: Disconnecting clients
                    SyncLock CType(WorldCluster.CLIENTs, ICollection).SyncRoot
                        For Each objCharacter As KeyValuePair(Of UInteger, ClientClass) In WorldCluster.CLIENTs
                            If Not objCharacter.Value.Character Is Nothing AndAlso
                               objCharacter.Value.Character.IsInWorld AndAlso
                               objCharacter.Value.Character.Map = map Then
                                objCharacter.Value.Send(New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE))
                                Call New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE).Dispose()

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
                                Log.WriteLine(LogType.INFORMATION, "Map: {0:000} has been disconnected!", map)
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
                                MyTime = timeGetTime("")
                                ServerTime = w.Value.Ping(MyTime, WorldsInfo(w.Key).Latency)
                                Latency = Math.Abs(MyTime - timeGetTime(""))

                                WorldsInfo(w.Key).Latency = Latency
                                SentPingTo(WorldsInfo(w.Key)) = Latency

                                Log.WriteLine(LogType.NETWORK, "Map {0:000} ping: {1}ms", w.Key, Latency)

                                'Query CPU and Memory usage
                                Dim serverInfo = w.Value.GetServerInfo()
                                WorldsInfo(w.Key).CPUUsage = serverInfo.cpuUsage
                                WorldsInfo(w.Key).MemoryUsage = serverInfo.memoryUsage
                            Else
                                Log.WriteLine(LogType.NETWORK, "Map {0:000} ping: {1}ms", w.Key, SentPingTo(WorldsInfo(w.Key)))
                            End If

                        Catch ex As Exception
                            Log.WriteLine(LogType.WARNING, "Map {0:000} is currently down!", w.Key)
                            DownedServers.Add(w.Key)
                        End Try
                    Next
                End SyncLock

                'Notification message
                If Worlds.Count = 0 Then Log.WriteLine(LogType.WARNING, "No maps are currently available!")

                'Drop WorldServers
                Disconnect("NULL", DownedServers)
            End Sub

            Public Sub ClientSend(ByVal id As UInteger, ByVal data() As Byte) Implements ICluster.ClientSend
                If WorldCluster.CLIENTs.ContainsKey(id) Then WorldCluster.CLIENTs(id).Send(data)
            End Sub

            Public Sub ClientDrop(ByVal ID As UInteger) Implements ICluster.ClientDrop
                If WorldCluster.CLIENTs.ContainsKey(ID) Then
                    Try
                        Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has dropped map {1:000}", ID, WorldCluster.CLIENTs(ID).Character.Map)
                        WorldCluster.CLIENTs(ID).Character.IsInWorld = False
                        WorldCluster.CLIENTs(ID).Character.OnLogout()
                    Catch ex As Exception
                        Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has dropped an exception: {1}", ID, ex.ToString)
                    End Try
                Else
                    Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client connection has been lost.", ID)
                End If
            End Sub

            Public Sub ClientTransfer(ByVal ID As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As UInteger) Implements ICluster.ClientTransfer
                Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has transferred from map {1:000} to map {2:000}", ID, WorldCluster.CLIENTs(ID).Character.Map, map)

                Dim p As New PacketClass(OPCODES.SMSG_NEW_WORLD)
                p.AddUInt32(map)
                p.AddSingle(posX)
                p.AddSingle(posY)
                p.AddSingle(posZ)
                p.AddSingle(ori)
                WorldCluster.CLIENTs(ID).Send(p)

                WorldCluster.CLIENTs(ID).Character.Map = map
            End Sub

            Public Sub ClientUpdate(ByVal ID As UInteger, ByVal zone As UInteger, ByVal level As Byte) Implements ICluster.ClientUpdate
                If WorldCluster.CLIENTs(ID).Character Is Nothing Then Return
                Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client has an updated zone {1:000}", ID, zone)

                WorldCluster.CLIENTs(ID).Character.Zone = zone
                WorldCluster.CLIENTs(ID).Character.Level = level
            End Sub

            Public Sub ClientSetChatFlag(ByVal ID As UInteger, ByVal flag As Byte) Implements ICluster.ClientSetChatFlag
                If WorldCluster.CLIENTs(ID).Character Is Nothing Then Return
                Log.WriteLine(LogType.DEBUG, "[{0:000000}] Client chat flag update [0x{1:X}]", ID, flag)

                WorldCluster.CLIENTs(ID).Character.ChatFlag = flag
            End Sub

            Public Function ClientGetCryptKey(ByVal ID As UInteger) As Byte() Implements ICluster.ClientGetCryptKey
                Log.WriteLine(LogType.DEBUG, "[{0:000000}] Requested client crypt key", ID)
                Return WorldCluster.CLIENTs(ID).SS_Hash
            End Function

            Public Sub Broadcast(ByVal Data() As Byte) Implements ICluster.Broadcast
                Dim b As Byte()
                CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
                For Each objCharacter As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs

                    If objCharacter.Value.IsInWorld AndAlso objCharacter.Value.Client IsNot Nothing Then
                        b = Data.Clone
                        objCharacter.Value.Client.Send(Data)
                    End If

                Next
                CHARACTERs_Lock.ReleaseReaderLock()
            End Sub

            Public Sub BroadcastGroup(ByVal groupId As Long, ByVal Data() As Byte) Implements ICluster.BroadcastGroup
                With WC_Handlers_Group.GROUPs(groupId)
                    For i As Byte = 0 To .Members.Length - 1
                        If .Members(i) IsNot Nothing Then
                            Call .Members(i).Client.Send(CType(Data.Clone, Byte()))
                        End If

                    Next
                End With
            End Sub

            Public Sub BroadcastRaid(ByVal GroupID As Long, ByVal Data() As Byte) Implements ICluster.BroadcastGuild
                With WC_Handlers_Group.GROUPs(GroupID)
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
                If Not WorldServer.Worlds.ContainsKey(MapID) Then
                    'We don't create new continents
                    If IsContinentMap(MapID) Then
                        Log.WriteLine(LogType.WARNING, "[{0:000000}] Requested Instance Map [{1}] is a continent", client.Index, MapID)

                        client.Send(New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE))
                        Call New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE).Dispose()

                        client.Character.IsInWorld = False
                        Return False
                    End If

                    Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Requesting Instance Map [{1}]", client.Index, MapID)
                    Dim ParentMap As IWorld = Nothing
                    Dim ParentMapInfo As WorldInfo = Nothing

                    'Check if we got parent map
                    If WorldServer.Worlds.ContainsKey(Maps(MapID).ParentMap) AndAlso WorldServer.Worlds(Maps(MapID).ParentMap).InstanceCanCreate(Maps(MapID).Type) Then
                        ParentMap = WorldServer.Worlds(Maps(MapID).ParentMap)
                        ParentMapInfo = WorldServer.WorldsInfo(Maps(MapID).ParentMap)
                    ElseIf WorldServer.Worlds.ContainsKey(0) AndAlso WorldServer.Worlds(0).InstanceCanCreate(Maps(MapID).Type) Then
                        ParentMap = WorldServer.Worlds(0)
                        ParentMapInfo = WorldServer.WorldsInfo(0)
                    ElseIf WorldServer.Worlds.ContainsKey(1) AndAlso WorldServer.Worlds(1).InstanceCanCreate(Maps(MapID).Type) Then
                        ParentMap = WorldServer.Worlds(1)
                        ParentMapInfo = WorldServer.WorldsInfo(1)
                    End If

                    If ParentMap Is Nothing Then
                        Log.WriteLine(LogType.WARNING, "[{0:000000}] Requested Instance Map [{1}] can't be loaded", client.Index, MapID)

                        client.Send(New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE))
                        Call New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE).Dispose()

                        client.Character.IsInWorld = False
                        Return False
                    End If

                    ParentMap.InstanceCreate(MapID)
                    WorldServer.Worlds.Add(MapID, ParentMap)
                    WorldServer.WorldsInfo.Add(MapID, ParentMapInfo)
                    Return True
                Else
                    Return True
                End If

            End Function

            Public Function BattlefieldCheck(ByVal MapID As UInteger) As Boolean
                'Create map
                If Not WorldServer.Worlds.ContainsKey(MapID) Then
                    Log.WriteLine(LogType.INFORMATION, "[SERVER] Requesting battlefield map [{0}]", MapID)
                    Dim ParentMap As IWorld = Nothing
                    Dim ParentMapInfo As WorldInfo = Nothing

                    'Check if we got parent map
                    If WorldServer.Worlds.ContainsKey(Maps(MapID).ParentMap) AndAlso WorldServer.Worlds(Maps(MapID).ParentMap).InstanceCanCreate(Maps(MapID).Type) Then
                        ParentMap = WorldServer.Worlds(Maps(MapID).ParentMap)
                        ParentMapInfo = WorldServer.WorldsInfo(Maps(MapID).ParentMap)
                    ElseIf WorldServer.Worlds.ContainsKey(0) AndAlso WorldServer.Worlds(0).InstanceCanCreate(Maps(MapID).Type) Then
                        ParentMap = WorldServer.Worlds(0)
                        ParentMapInfo = WorldServer.WorldsInfo(0)
                    ElseIf WorldServer.Worlds.ContainsKey(1) AndAlso WorldServer.Worlds(1).InstanceCanCreate(Maps(MapID).Type) Then
                        ParentMap = WorldServer.Worlds(1)
                        ParentMapInfo = WorldServer.WorldsInfo(1)
                    End If

                    If ParentMap Is Nothing Then
                        Log.WriteLine(LogType.WARNING, "[SERVER] Requested battlefield map [{0}] can't be loaded", MapID)
                        Return False
                    End If

                    ParentMap.InstanceCreate(MapID)
                    WorldServer.Worlds.Add(MapID, ParentMap)
                    WorldServer.WorldsInfo.Add(MapID, ParentMapInfo)
                    Return True
                Else
                    Return True
                End If
            End Function

            Public Function BattlefieldList(ByVal MapType As Byte) As List(Of Integer) Implements ICluster.BattlefieldList
                Dim BattlefieldMap As New List(Of Integer)

                BATTLEFIELDs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
                For Each BG As KeyValuePair(Of Integer, Battlefield) In BATTLEFIELDs
                    If BG.Value.MapType = MapType Then
                        BattlefieldMap.Add(BG.Value.ID)
                    End If
                Next

                BATTLEFIELDs_Lock.ReleaseReaderLock()
                Return BattlefieldMap
            End Function

            Public Sub BattlefieldFinish(ByVal battlefieldId As Integer) Implements ICluster.BattlefieldFinish
                Log.WriteLine(LogType.INFORMATION, "[B{0:0000}] Battlefield finished", battlefieldId)
            End Sub

            Public Sub GroupRequestUpdate(ByVal ID As UInteger) Implements ICluster.GroupRequestUpdate
                If WorldCluster.CLIENTs.ContainsKey(ID) AndAlso WorldCluster.CLIENTs(ID).Character IsNot Nothing AndAlso WorldCluster.CLIENTs(ID).Character.IsInWorld AndAlso WorldCluster.CLIENTs(ID).Character.IsInGroup Then

                    Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update request", WorldCluster.CLIENTs(ID).Character.Group.Id)

                    Try
                        WorldCluster.CLIENTs(ID).Character.GetWorld.GroupUpdate(WorldCluster.CLIENTs(ID).Character.Group.Id, WorldCluster.CLIENTs(ID).Character.Group.Type, WorldCluster.CLIENTs(ID).Character.Group.GetLeader.Guid, WorldCluster.CLIENTs(ID).Character.Group.GetMembers)
                        WorldCluster.CLIENTs(ID).Character.GetWorld.GroupUpdateLoot(WorldCluster.CLIENTs(ID).Character.Group.Id, WorldCluster.CLIENTs(ID).Character.Group.DungeonDifficulty, WorldCluster.CLIENTs(ID).Character.Group.LootMethod, WorldCluster.CLIENTs(ID).Character.Group.LootThreshold, WorldCluster.CLIENTs(ID).Character.Group.GetLootMaster.Guid)
                    Catch
                        WorldServer.Disconnect("NULL", New List(Of UInteger)() From {WorldCluster.CLIENTs(ID).Character.Map})
                    End Try
                End If
            End Sub

            Public Sub GroupSendUpdate(ByVal GroupID As Long)
                Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update", GroupID)

                SyncLock CType(Worlds, ICollection).SyncRoot
                    For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                        Try
                            w.Value.GroupUpdate(GroupID, WC_Handlers_Group.GROUPs(GroupID).Type, WC_Handlers_Group.GROUPs(GroupID).GetLeader.Guid, WC_Handlers_Group.GROUPs(GroupID).GetMembers)
                        Catch ex As Exception
                            Log.WriteLine(LogType.FAILED, "[G{0:00000}] Group update failed for [M{1:000}]", GroupID, w.Key)
                        End Try
                    Next
                End SyncLock
            End Sub

            Public Sub GroupSendUpdateLoot(ByVal GroupID As Long)
                Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update loot", GroupID)

                SyncLock CType(Worlds, ICollection).SyncRoot

                    For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                        Try
                            w.Value.GroupUpdateLoot(GroupID, WC_Handlers_Group.GROUPs(GroupID).DungeonDifficulty, WC_Handlers_Group.GROUPs(GroupID).LootMethod, WC_Handlers_Group.GROUPs(GroupID).LootThreshold, WC_Handlers_Group.GROUPs(GroupID).GetLootMaster.Guid)
                        Catch ex As Exception
                            Log.WriteLine(LogType.FAILED, "[G{0:00000}] Group update loot failed for [M{1:000}]", GroupID, w.Key)
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
            Public Character As CharacterObject = Nothing

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
                If WorldCluster.CLIENTs Is Nothing Then Throw New ApplicationException("Clients doesn't exist!")

                Dim remoteEndPoint As IPEndPoint = CType(Socket.RemoteEndPoint, IPEndPoint)
                IP = remoteEndPoint.Address.ToString
                Port = remoteEndPoint.Port

                'DONE: Connection spam protection
                If LastConnections.ContainsKey(Ip2Int(IP.ToString)) Then
                    If Now > LastConnections(Ip2Int(IP.ToString)) Then
                        LastConnections(Ip2Int(IP.ToString)) = Now.AddSeconds(5)
                    Else
                        Socket.Close()
                        Dispose()
                        Exit Sub
                    End If
                Else
                    LastConnections.Add(Ip2Int(IP.ToString), Now.AddSeconds(5))
                End If

                Log.WriteLine(LogType.DEBUG, "Incoming connection from [{0}:{1}]", IP, Port)

                Socket.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketFlags.None, AddressOf OnData, Nothing)

                'Send Auth Challenge
                Dim p As New PacketClass(OPCODES.SMSG_AUTH_CHALLENGE)
                p.AddInt32(Index)
                Send(p)

                Index = Interlocked.Increment(CLIETNIDs)

                SyncLock CType(WorldCluster.CLIENTs, ICollection).SyncRoot
                    WorldCluster.CLIENTs.Add(Index, Me)
                End SyncLock

                ConnectionsIncrement()
            End Sub

            Public Sub OnData(ByVal ar As IAsyncResult)
                If Not Socket.Connected Then Return
                If WorldServer.m_flagStopListen Then Return
                If ar Is Nothing Then Throw New ApplicationException("Value ar is empty!")
                If SocketBuffer Is Nothing Then Throw New ApplicationException("SocketBuffer is empty!")
                If Socket Is Nothing Then Throw New ApplicationException("Socket is Null!")
                If WorldCluster.CLIENTs Is Nothing Then Throw New ApplicationException("Clients doesn't exist!")
                If Queue Is Nothing Then Throw New ApplicationException("Queue is Null!")
                If SavedBytes Is Nothing Then Throw New ApplicationException("SavedBytes is empty!")

                Try
                    SocketBytes = Socket.EndReceive(ar)
                    If SocketBytes = 0 Then
                        Dispose(SocketBytes)
                    Else
                        Interlocked.Add(DataTransferIn, SocketBytes)

                        While SocketBytes > 0
                            If SavedBytes.Length = 0 Then
                                If Encryption Then Decode(SocketBuffer)
                            Else
                                SocketBuffer = Concat(SavedBytes, SocketBuffer)
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
                                    Log.WriteLine(LogType.CRITICAL, "[{0}:{1}] BAD PACKET {2}({3}) bytes, ", IP, Port, SocketBytes, PacketLen)
                                End Try
                                Exit While
                            End If

                            'Move packet to Data
                            Dim data(PacketLen - 1) As Byte
                            Array.Copy(SocketBuffer, data, PacketLen)

                            'Create packet and add it to queue
                            Dim p As New PacketClass(data)
                            SyncLock Queue.SyncRoot
                                Queue.Enqueue(p)
                            End SyncLock

                            Try
                                'Delete packet from buffer
                                SocketBytes -= PacketLen
                                Array.Copy(SocketBuffer, PacketLen, SocketBuffer, 0, SocketBytes)
                            Catch Ex As Exception
                                Log.WriteLine(LogType.CRITICAL, "[{0}:{1}] Could not delete packet from buffer! {2}({3}{4}) bytes, ", IP, Port, SocketBuffer, PacketLen, SocketBytes)
                            End Try

                        End While

                        If SocketBuffer.Length > 0 Then
                            Try
                                Socket.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketBytes, SocketFlags.None, AddressOf OnData, Nothing)

                                If HandingPackets = False Then ThreadPool.QueueUserWorkItem(AddressOf OnPacket)
                            Catch ex As Exception
                                Log.WriteLine(LogType.WARNING, "Packet Disconnect from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbNewLine)
                            End Try
                        End If
                    End If
                Catch Err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    Log.WriteLine(LogType.WARNING, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbNewLine)

                    Dispose(SocketBuffer.Length)
                    Dispose(HandingPackets)
                End Try
            End Sub

            <MethodImpl(MethodImplOptions.Synchronized)>
            Public Sub OnPacket(state As Object)
                If SocketBuffer Is Nothing Then Throw New ApplicationException("SocketBuffer is empty!")
                If Socket Is Nothing Then Throw New ApplicationException("Socket is Null!")
                If WorldCluster.CLIENTs Is Nothing Then Throw New ApplicationException("Clients doesn't exist!")
                If Queue Is Nothing Then Throw New ApplicationException("Queue is Null!")
                If SavedBytes Is Nothing Then Throw New ApplicationException("SavedBytes is empty!")
                If PacketHandlers Is Nothing Then Throw New ApplicationException("PacketHandler is empty!")

                Try
                Catch ex As Exception
                    HandingPackets = True
                    Log.WriteLine(LogType.FAILED, "Handing Packets Failed: {0}", HandingPackets)
                End Try
                While Queue.Count > 0

                    Dim p As PacketClass
                    SyncLock Queue.SyncRoot
                        p = Queue.Dequeue
                    End SyncLock

                    If Config.PacketLogging Then LogPacket(p.Data, False, Me)
                    If PacketHandlers.ContainsKey(p.OpCode) <> True Then
                        If Character Is Nothing OrElse Character.IsInWorld = False Then
                            Socket.Dispose()
                            Socket.Close()
                            Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", IP, Port, p.OpCode, vbNewLine, p.Length)
                            DumpPacket(p.Data, Me)
                        Else
                            Try
                                Character.GetWorld.ClientPacket(Index, p.Data)
                            Catch
                                WorldServer.Disconnect("NULL", New List(Of UInteger)() From {Character.Map})
                            End Try
                        End If

                    Else
                        Try
                            PacketHandlers(p.OpCode).Invoke(p, Me)
                        Catch e As Exception
                            Log.WriteLine(LogType.FAILED, "Opcode handler {2}:{2:X} caused an error: {1}{0}", e.ToString, vbNewLine, p.OpCode)
                        End Try
                    End If
                    Try
                    Catch ex As Exception
                        If Queue.Count = 0 Then p.Dispose()
                        Log.WriteLine(LogType.WARNING, "Unable to dispose of packet: {0}", p.OpCode)
                        DumpPacket(p.Data, Me)
                    End Try
                End While
                HandingPackets = False
            End Sub

            Public Sub Send(ByVal data() As Byte)
                If Not Socket.Connected Then Exit Sub

                Try
                    If Config.PacketLogging Then LogPacket(data, True, Me)
                    If Encryption Then Encode(data)
                    Socket.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf OnSendComplete, Nothing)
                Catch Err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbNewLine)
                    Delete()
                End Try
            End Sub

            Public Sub Send(ByRef packet As PacketClass)
                If IsNothing(packet) Then Throw New ApplicationException("Packet doesn't contain data!")
                If IsNothing(Socket) Or Socket.Connected = False Then Exit Sub

                Try
                    Dim data As Byte() = packet.Data
                    If Config.PacketLogging Then LogPacket(data, True, Me)
                    If Encryption Then Encode(data)
                    Socket.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf OnSendComplete, Nothing)
                Catch err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, err.ToString, vbNewLine)
                    Delete()
                End Try

                'Only attempt to dispose of the packet if it actually exists
                If Not IsNothing(packet) Then packet.Dispose()
            End Sub

            Public Sub SendMultiplyPackets(ByRef packet As PacketClass)
                If packet Is Nothing Then Throw New ApplicationException("Packet doesn't contain data!")

                If Not Socket.Connected Then Exit Sub

                Try
                    Dim data As Byte() = packet.Data.Clone
                    If Config.PacketLogging Then LogPacket(data, True, Me)
                    If Encryption Then Encode(data)
                    Socket.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf OnSendComplete, Nothing)
                Catch Err As Exception
                    'NOTE: If it's a error here it means the connection is closed?
                    Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] caused an error {2}{3}", IP, Port, Err.ToString, vbNewLine)
                    Delete()
                End Try

                'Don't forget to clean after using this function
            End Sub

            Public Sub OnSendComplete(ByVal ar As IAsyncResult)
                If Not Socket Is Nothing Then
                    Dim bytesSent As Integer = Socket.EndSend(ar)

                    Interlocked.Add(DataTransferOut, bytesSent)
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

                    SyncLock CType(WorldCluster.CLIENTs, ICollection).SyncRoot
                        WorldCluster.CLIENTs.Remove(Index)
                    End SyncLock

                    If Not Character Is Nothing Then
                        If Character.IsInWorld Then
                            Character.IsInWorld = False
                            Character.GetWorld.ClientDisconnect(Index)
                        End If
                        Character.Dispose()
                    End If

                    Character = Nothing

                    ConnectionsDecrement()
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
                While CHARACTERs.Count > Config.ServerPlayerLimit
                    If Not Socket.Connected Then Exit Sub

                    Call New PacketClass(OPCODES.SMSG_AUTH_RESPONSE).AddInt8(LoginResponse.LOGIN_WAIT_QUEUE)
                    Call New PacketClass(OPCODES.SMSG_AUTH_RESPONSE).AddInt32(WorldCluster.CLIENTs.Count - CHARACTERs.Count)            'amount of players in queue
                    Send(New PacketClass(OPCODES.SMSG_AUTH_RESPONSE))

                    Log.WriteLine(LogType.INFORMATION, "[{1}:{2}] AUTH_WAIT_QUEUE: Server player limit reached!", IP, Port)
                    Thread.Sleep(6000)
                End While
                SendLoginOk(Me)
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

    End Module
End Namespace
