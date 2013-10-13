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

Imports System
Imports System.IO
Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.Remoting
Imports System.Runtime.CompilerServices
Imports System.Security.Permissions
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common


Public Module WC_Network

#Region "WS.Sockets"


    Public WorldServer As WorldServerClass
    Public Authenticator As Authenticator

    Class WorldServerClass
        Inherits MarshalByRefObject
        Implements ICluster
        Implements IDisposable

        <CLSCompliant(False)> _
        Public m_flagStopListen As Boolean = False
        Private m_TimerPing As Timer
        Private m_TimerStats As Timer
        Private m_TimerCPU As Timer
        Private m_RemoteChannel As Channels.IChannel = Nothing

        Private m_Socket As Socket

        Public Sub New()
            Try

                m_Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                m_Socket.Bind(New IPEndPoint(Net.IPAddress.Parse(Config.WorldClusterAddress), Config.WorldClusterPort))
                m_Socket.Listen(5)
                m_Socket.BeginAccept(AddressOf AcceptConnection, Nothing)

                Log.WriteLine(LogType.SUCCESS, "Listening on {0} on port {1}", Net.IPAddress.Parse(Config.WorldClusterAddress), Config.WorldClusterPort)

                'Create Remoting Channel
                Select Case Config.ClusterListenMethod
                    Case "ipc"
                        m_RemoteChannel = New Channels.Ipc.IpcChannel(String.Format("{0}:{1}", Config.ClusterListenAddress, Config.ClusterListenPort))
                    Case "tcp"
                        m_RemoteChannel = New Channels.Tcp.TcpChannel(Config.ClusterListenPort)
                End Select
                Channels.ChannelServices.RegisterChannel(m_RemoteChannel, False)

                'NOTE: Not protected remoting
                'RemotingServices.Marshal(CType(Me, ICluster), "Cluster.rem")

                'NOTE: Password protected remoting
                Authenticator = New Authenticator(Me, Config.ClusterPassword)

                RemotingServices.Marshal(CType(Authenticator, Authenticator), "Cluster.rem")
                Log.WriteLine(LogType.INFORMATION, "Interface UP at: {0}://{1}:{2}/Cluster.rem", Config.ClusterListenMethod, Config.ClusterListenAddress, Config.ClusterListenPort)

                'Creating ping timer
                m_TimerPing = New Timer(AddressOf Ping, Nothing, 0, 15000)

                'Creating stats timer
                If Config.StatsEnabled Then
                    m_TimerStats = New Timer(AddressOf GenerateStats, Nothing, Config.StatsTimer, Config.StatsTimer)
                End If

                'Creating CPU check timer
                m_TimerCPU = New Timer(AddressOf CheckCPU, Nothing, 1000, 1000)

            Catch e As Exception
                Console.WriteLine()
                Log.WriteLine(LogType.FAILED, "Error in {1}: {0}.", e.Message, e.Source)
            End Try
        End Sub
        Protected Sub AcceptConnection(ByVal ar As IAsyncResult)
            If m_flagStopListen Then Return

            Dim m_Client As New ClientClass
            m_Client.Socket = m_Socket.EndAccept(ar)
            m_Client.Socket.NoDelay = True
            m_Client.Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.NoDelay, 1)

            m_Socket.BeginAccept(AddressOf AcceptConnection, Nothing)

            ThreadPool.QueueUserWorkItem(New System.Threading.WaitCallback(AddressOf m_Client.OnConnect))
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            Channels.ChannelServices.UnregisterChannel(m_RemoteChannel)

            m_flagStopListen = True
            m_Socket.Close()
        End Sub
        <SecurityPermissionAttribute(SecurityAction.Demand, Flags:=SecurityPermissionFlag.Infrastructure)> _
        Public Overrides Function InitializeLifetimeService() As Object
            Return Nothing
        End Function

        Public Worlds As New Dictionary(Of UInteger, IWorld)
        Public WorldsInfo As New Dictionary(Of UInteger, WorldInfo)

        Public Function Connect(ByVal URI As String, ByVal Maps As System.Collections.ICollection) As Boolean Implements ICluster.Connect
            Try
                Disconnect(URI, Maps)

                'NOTE: Password protected remoting
                Dim a As Authenticator = Activator.GetObject(GetType(Authenticator), URI)
                Dim WorldServer As IWorld = a.Login(Config.ClusterPassword)

                'NOTE: Not protected remoting
                'Dim WorldServer As IWorld = CType(RemotingServices.Connect(GetType(IWorld), URI), IWorld)

                Dim WorldServerInfo As New WorldInfo
                Log.WriteLine(LogType.INFORMATION, "Connected World Server: {0}", URI)

                SyncLock CType(Worlds, ICollection).SyncRoot
                    For Each Map As UInteger In Maps
                        Worlds(Map) = WorldServer
                        WorldsInfo(Map) = WorldServerInfo
                    Next
                End SyncLock

            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "Unable to reverse connect. [{0}]", ex.ToString)
                Return False
            End Try

            Return True
        End Function
        Public Sub Disconnect(ByVal URI As String, ByVal Maps As System.Collections.ICollection) Implements ICluster.Disconnect
            If Maps.Count = 0 Then Return

            'TODO: Unload arenas or battlegrounds that is hosted on this server!

            For Each Map As UInteger In Maps

                'DONE: Disconnecting clients
                SyncLock CType(CLIENTs, ICollection).SyncRoot
                    For Each c As KeyValuePair(Of UInteger, ClientClass) In CLIENTs
                        If Not c.Value.Character Is Nothing AndAlso _
                        c.Value.Character.IsInWorld AndAlso _
                        c.Value.Character.Map = Map Then
                            Dim SMSG_LOGOUT_COMPLETE As New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE)
                            c.Value.Send(SMSG_LOGOUT_COMPLETE)
                            SMSG_LOGOUT_COMPLETE.Dispose()

                            c.Value.Character.Dispose()
                            c.Value.Character = Nothing
                        End If
                    Next
                End SyncLock

                If Worlds.ContainsKey(Map) Then
                    Try
                        RemotingServices.Disconnect(Worlds(Map))
                        Worlds(Map) = Nothing
                        WorldsInfo(Map) = Nothing
                    Catch
                    Finally
                        SyncLock CType(Worlds, ICollection).SyncRoot
                            Worlds.Remove(Map)
                            WorldsInfo.Remove(Map)
                            Log.WriteLine(LogType.INFORMATION, "Disconnected World Server: {0:000}", Map)
                        End SyncLock
                    End Try
                End If
            Next

        End Sub
        Public Sub Ping(ByVal State As Object)
            Dim DeadServers As New List(Of UInteger)
            Dim SentPingTo As New Dictionary(Of WorldInfo, Integer)

            Dim MyTime As Integer
            Dim ServerTime As Integer
            Dim Latency As Integer

            'Ping WorldServers
            SyncLock CType(Worlds, ICollection).SyncRoot
                For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                    Try
                        If SentPingTo.ContainsKey(WorldsInfo(w.Key)) Then
                            Log.WriteLine(LogType.NETWORK, "World [M{0:0000}] ping: {1}ms", w.Key, SentPingTo(WorldsInfo(w.Key)))
                        Else
                            MyTime = timeGetTime
                            ServerTime = w.Value.Ping(MyTime, WorldsInfo(w.Key).Latency)
                            Latency = Math.Abs(MyTime - timeGetTime)

                            WorldsInfo(w.Key).Latency = Latency
                            SentPingTo(WorldsInfo(w.Key)) = Latency

                            Log.WriteLine(LogType.NETWORK, "World [M{0:0000}] ping: {1}ms", w.Key, Latency)

                            'Query CPU and Memory usage
                            w.Value.ServerInfo(WorldsInfo(w.Key).CPUUsage, WorldsInfo(w.Key).MemoryUsage)
                        End If

                    Catch ex As Exception
                        Log.WriteLine(LogType.WARNING, "World [M{0:0000}] down.", w.Key)

                        DeadServers.Add(w.Key)
                    End Try
                Next
            End SyncLock

            'Notification message
            If Worlds.Count = 0 Then Log.WriteLine(LogType.WARNING, "No world servers available!")

            'Drop WorldServers
            Disconnect("NULL", DeadServers)
        End Sub

        Public Sub ClientSend(ByVal ID As UInteger, ByVal Data() As Byte) Implements ICluster.ClientSend
            If CLIENTs.ContainsKey(ID) Then CLIENTs(ID).Send(Data)
        End Sub
        Public Sub ClientDrop(ByVal ID As UInteger) Implements ICluster.ClientDrop
            Try
                Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client drop [M{1:0000}]", ID, CLIENTs(ID).Character.Map)
                CLIENTs(ID).Character.IsInWorld = False
                CLIENTs(ID).Character.OnLogout()
            Catch ex As Exception
                Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client drop exception: {1}", ID, ex.ToString)
            End Try
        End Sub
        Public Sub ClientTransfer(ByVal ID As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As UInteger) Implements ICluster.ClientTransfer
            Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client transfer [M{1:0000}->M{2:0000}]", ID, CLIENTs(ID).Character.Map, map)

            Dim p As New PacketClass(OPCODES.SMSG_NEW_WORLD)
            p.AddUInt32(map)
            p.AddSingle(posX)
            p.AddSingle(posY)
            p.AddSingle(posZ)
            p.AddSingle(ori)
            CLIENTs(ID).Send(p)

            CLIENTs(ID).Character.Map = map
        End Sub
        Public Sub ClientUpdate(ByVal ID As UInteger, ByVal Zone As UInteger, ByVal Level As Byte) Implements ICluster.ClientUpdate
            If CLIENTs(ID).Character Is Nothing Then Return
            Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Client zone update [Z{1:0000}]", ID, Zone)

            CLIENTs(ID).Character.Zone = Zone
            CLIENTs(ID).Character.Level = Level
        End Sub
        Public Sub ClientSetChatFlag(ByVal ID As UInteger, ByVal Flag As Byte) Implements ICluster.ClientSetChatFlag
            If CLIENTs(ID).Character Is Nothing Then Return
            Log.WriteLine(LogType.DEBUG, "[{0:000000}] Client chat flag update [0x{1:X}]", ID, Flag)

            CLIENTs(ID).Character.ChatFlag = Flag
        End Sub
        Public Function ClientGetCryptKey(ByVal ID As UInteger) As Byte() Implements ICluster.ClientGetCryptKey
            Log.WriteLine(LogType.DEBUG, "[{0:000000}] Requested client crypt key", ID)
            Return CLIENTs(ID).SS_Hash
        End Function


        Public Sub Broadcast(ByVal p As PacketClass)
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each c As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If c.Value.IsInWorld AndAlso c.Value.Client IsNot Nothing Then c.Value.Client.SendMultiplyPackets(p)
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
        End Sub
        Public Sub Broadcast(ByVal Data() As Byte) Implements ICluster.Broadcast
            Dim b As Byte()
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each c As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs

                If c.Value.IsInWorld AndAlso c.Value.Client IsNot Nothing Then
                    b = Data.Clone
                    c.Value.Client.Send(Data)
                End If

            Next
            CHARACTERs_Lock.ReleaseReaderLock()
        End Sub
        Public Sub BroadcastGroup(ByVal GroupID As Long, ByVal Data() As Byte) Implements ICluster.BroadcastGroup
            With GROUPs(GroupID)
                For i As Byte = 0 To .Members.Length - 1
                    If .Members(i) IsNot Nothing Then
                        Dim buffer() As Byte = Data.Clone
                        .Members(i).Client.Send(buffer)
                    End If

                Next
            End With
        End Sub
        Public Sub BroadcastRaid(ByVal GroupID As Long, ByVal Data() As Byte) Implements ICluster.BroadcastGuild
            With GROUPs(GroupID)
                For i As Byte = 0 To .Members.Length - 1
                    If .Members(i) IsNot Nothing AndAlso .Members(i).Client IsNot Nothing Then
                        Dim buffer() As Byte = Data.Clone
                        .Members(i).Client.Send(buffer)
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


        Public Function InstanceCheck(ByVal Client As ClientClass, ByVal MapID As UInteger) As Boolean
            If (Not WorldServer.Worlds.ContainsKey(MapID)) Then
                'We don't create new continents
                If IsContinentMap(MapID) Then
                    Log.WriteLine(LogType.WARNING, "[{0:000000}] Requested instance map [{1}] is a continent", Client.Index, MapID)

                    Dim SMSG_LOGOUT_COMPLETE As New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE)
                    Client.Send(SMSG_LOGOUT_COMPLETE)
                    SMSG_LOGOUT_COMPLETE.Dispose()

                    Client.Character.IsInWorld = False
                    Return False
                End If

                Log.WriteLine(LogType.INFORMATION, "[{0:000000}] Requesting instance map [{1}]", Client.Index, MapID)
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
                    Log.WriteLine(LogType.WARNING, "[{0:000000}] Requested instance map [{1}] can't be loaded", Client.Index, MapID)

                    Dim SMSG_LOGOUT_COMPLETE As New PacketClass(OPCODES.SMSG_LOGOUT_COMPLETE)
                    Client.Send(SMSG_LOGOUT_COMPLETE)
                    SMSG_LOGOUT_COMPLETE.Dispose()

                    Client.Character.IsInWorld = False
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
            If (Not WorldServer.Worlds.ContainsKey(MapID)) Then

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
            Dim tmpList As New List(Of Integer)

            BATTLEFIELDs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each BG As KeyValuePair(Of Integer, Battlefield) In BATTLEFIELDs
                If BG.Value.MapType = MapType Then
                    tmpList.Add(BG.Value.ID)
                End If
            Next

            BATTLEFIELDs_Lock.ReleaseReaderLock()
            Return tmpList
        End Function
        Public Sub BattlefieldFinish(ByVal BattlefieldID As Integer) Implements ICluster.BattlefieldFinish
            Log.WriteLine(LogType.INFORMATION, "[B{0:0000}] Battlefield finished", BattlefieldID)
        End Sub

        Public Sub GroupRequestUpdate(ByVal ID As UInteger) Implements ICluster.GroupRequestUpdate
            If CLIENTs.ContainsKey(ID) AndAlso CLIENTs(ID).Character IsNot Nothing AndAlso CLIENTs(ID).Character.IsInWorld AndAlso CLIENTs(ID).Character.IsInGroup Then

                Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update request", CLIENTs(ID).Character.Group.ID)

                Try
                    CLIENTs(ID).Character.GetWorld.GroupUpdate(CLIENTs(ID).Character.Group.ID, CLIENTs(ID).Character.Group.Type, CLIENTs(ID).Character.Group.GetLeader.GUID, CLIENTs(ID).Character.Group.GetMembers)
                    CLIENTs(ID).Character.GetWorld.GroupUpdateLoot(CLIENTs(ID).Character.Group.ID, CLIENTs(ID).Character.Group.DungeonDifficulty, CLIENTs(ID).Character.Group.LootMethod, CLIENTs(ID).Character.Group.LootThreshold, CLIENTs(ID).Character.Group.GetLootMaster.GUID)
                Catch
                    WorldServer.Disconnect("NULL", New Integer() {CLIENTs(ID).Character.Map})
                End Try
            End If
        End Sub
        Public Sub GroupSendUpdate(ByVal GroupID As Long)
            Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update", GroupID)

            SyncLock CType(Worlds, ICollection).SyncRoot
                Dim Type As Byte = GROUPs(GroupID).Type
                Dim Leader As ULong = GROUPs(GroupID).GetLeader.GUID
                Dim Members() As ULong = GROUPs(GroupID).GetMembers
                For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                    Try
                        w.Value.GroupUpdate(GroupID, Type, Leader, Members)
                    Catch ex As Exception
                        Log.WriteLine(LogType.FAILED, "[G{0:00000}] Group update failed for [M{1:000}]", GroupID, w.Key)
                    End Try
                Next
            End SyncLock
        End Sub
        Public Sub GroupSendUpdateLoot(ByVal GroupID As Long)
            Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update loot", GroupID)

            SyncLock CType(Worlds, ICollection).SyncRoot
                Dim Difficulty As GroupDungeonDifficulty = GROUPs(GroupID).DungeonDifficulty
                Dim Method As GroupLootMethod = GROUPs(GroupID).LootMethod
                Dim Threshold As GroupLootThreshold = GROUPs(GroupID).LootThreshold
                Dim Master As ULong = GROUPs(GroupID).GetLootMaster.GUID

                For Each w As KeyValuePair(Of UInteger, IWorld) In Worlds
                    Try
                        w.Value.GroupUpdateLoot(GroupID, Difficulty, Method, Threshold, Master)
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


#End Region
#Region "WS.Analyzer"


    Public Enum AccessLevel As Byte
        Trial = 0
        Player = 1
        GameMaster = 2
        Developer = 3
        Admin = 4
    End Enum

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
        Private Key() As Byte = {0, 0, 0, 0}

        Private HandingPackets As Boolean = False

        Public Function GetClientInfo() As ClientInfo
            Dim ci As New ClientInfo

            ci.Access = Access
            ci.Account = Account
            ci.Index = Index
            ci.IP = IP
            ci.Port = Port

            Return ci
        End Function


        Public Sub OnConnect(ByVal state As Object)
            IP = CType(Socket.RemoteEndPoint, IPEndPoint).Address
            Port = CType(Socket.RemoteEndPoint, IPEndPoint).Port

            'DONE: Connection spam protection
            'TODO: Connection spamming still increases a lot of CPU. How do we protect against this?
            Dim IpInt As UInteger = IP2Int(IP.ToString)
            If LastConnections.ContainsKey(IpInt) Then
                If Now > LastConnections(IpInt) Then
                    LastConnections(IpInt) = Now.AddSeconds(5)
                Else
                    Socket.Close()
                    Me.Dispose()
                    Exit Sub
                End If
            Else
                LastConnections.Add(IpInt, Now.AddSeconds(5))
            End If

            Log.WriteLine(LogType.DEBUG, "Incoming connection from [{0}:{1}]", IP, Port)

            Socket.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketFlags.None, AddressOf OnData, Nothing)

            'Send Auth Challenge
            Dim p As New PacketClass(OPCODES.SMSG_AUTH_CHALLENGE)
            p.AddInt32(Index)
            Me.Send(p)

            Me.Index = Interlocked.Increment(CLIETNIDs)

            SyncLock CType(CLIENTs, ICollection).SyncRoot
                CLIENTs.Add(Me.Index, Me)
            End SyncLock

            ConnectionsIncrement()
        End Sub
        Public Sub OnData(ByVal ar As IAsyncResult)
            If Not Socket.Connected Then Return
            If WorldServer.m_flagStopListen Then Return

            Try
                SocketBytes = Socket.EndReceive(ar)
                If SocketBytes = 2 Then
                    Me.Dispose()
                Else
                    Interlocked.Add(DataTransferIn, SocketBytes)

                    While SocketBytes > 1
                        If SavedBytes.Length > 1 Then
                            SocketBuffer = Concat(SavedBytes, SocketBuffer)
                            SavedBytes = New Byte() {}
                        Else
                            If Encryption Then Decode(SocketBuffer)
                        End If

                        'Calculate Length from packet
                        Dim PacketLen As Integer = (SocketBuffer(1) + SocketBuffer(0) * 256) + 2

                        If SocketBytes < PacketLen Then
                            SavedBytes = New Byte(SocketBytes - 1) {}
                            Array.Copy(SocketBuffer, 1, SavedBytes, 1, SocketBytes)
                            Log.WriteLine(LogType.CRITICAL, "[{0}:{1}] BAD PACKET {2}({3}) bytes, ", IP, Port, SocketBytes, PacketLen)
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

                        'Delete packet from buffer
                        SocketBytes -= PacketLen
                        Array.Copy(SocketBuffer, PacketLen, SocketBuffer, 0, SocketBytes)

                    End While
					
					If SocketBuffer.Length > 1 Then
                    Socket.BeginReceive(SocketBuffer, 0, SocketBuffer.Length, SocketFlags.None, AddressOf OnData, Nothing)

                    If HandingPackets = False Then ThreadPool.QueueUserWorkItem(AddressOf OnPacket)
                End If
			End If
            Catch Err As Exception
#If DEBUG Then
                'NOTE: If it's a error here it means the connection is closed?
                Log.WriteLine(LogType.WARNING, "Connection from [{0}:{1}] cause error {2}{3}", IP, Port, Err.ToString, vbNewLine)
#End If
                Me.Dispose()
            End Try
        End Sub
        <MethodImplAttribute(MethodImplOptions.Synchronized)> _
        Public Sub OnPacket()
            HandingPackets = True
            While Queue.Count > 0
                Dim p As PacketClass

                SyncLock Queue.SyncRoot
                    p = Queue.Dequeue
                End SyncLock

                If Config.PacketLogging Then LogPacket(p.Data, False, Me)
                If PacketHandlers.ContainsKey(p.OpCode) = True Then
                    Try
                        PacketHandlers(p.OpCode).Invoke(p, Me)
                    Catch e As Exception
                        Log.WriteLine(LogType.FAILED, "Opcode handler {2}:{2:X} caused an error:{1}{0}", e.ToString, vbNewLine, p.OpCode)
                    End Try
                Else
                    If Character Is Nothing OrElse Character.IsInWorld = False Then
                        Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", IP, Port, p.OpCode, vbNewLine, p.Length)
                        DumpPacket(p.Data, Me)
                    Else
                        Try
                            Character.GetWorld.ClientPacket(Index, p.Data)
                        Catch
                            WorldServer.Disconnect("NULL", New Integer() {Character.Map})
                        End Try
                    End If

                End If

                p.Dispose()
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
                'Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {2}{3}", IP, Port, Err.ToString, vbNewLine)
                Delete()
            End Try
        End Sub
        Public Sub Send(ByRef packet As PacketClass)
            If packet Is Nothing Then Throw New ApplicationException("Packet doesn't contain data!")
            If Socket Is Nothing OrElse Socket.Connected = False Then Exit Sub

            Try
                Dim data As Byte() = packet.Data
                If Config.PacketLogging Then LogPacket(data, True, Me)
                If Encryption Then Encode(data)
                Socket.BeginSend(data, 0, data.Length, SocketFlags.None, AddressOf OnSendComplete, Nothing)
            Catch Err As Exception
                'NOTE: If it's a error here it means the connection is closed?
                'Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {2}{3}", IP, Port, Err.ToString, vbNewLine)
                Delete()
            End Try

            'Cleaning, no memory leak :)
            packet.Dispose()
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
                'Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {2}{3}", IP, Port, Err.ToString, vbNewLine)
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

        Private Sub Dispose() Implements System.IDisposable.Dispose
            'Log.WriteLine(LogType.NETWORK, "Connection from [{0}:{1}] disposed", IP, Port)

            On Error Resume Next

            If Not Socket Is Nothing Then Socket.Close()
            Socket = Nothing

            SyncLock CType(CLIENTs, ICollection).SyncRoot
                CLIENTs.Remove(Me.Index)
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
        End Sub
        Public Sub Delete()
            On Error Resume Next
            Me.Socket.Close()
            Me.Dispose()
        End Sub

        Public Sub Decode(ByRef data() As Byte)
            Dim i As Integer
            Dim tmp As Integer

            For i = 0 To 6 - 1
                tmp = data(i)
                data(i) = SS_Hash(Key(1)) Xor CByte((256 + CInt(data(i)) - Key(0)) Mod 256)
                Me.Key(0) = tmp
                Me.Key(1) = (Me.Key(1) + 1) Mod 40
            Next i
        End Sub
        Public Sub Encode(ByRef data() As Byte)
            Dim i As Integer
            For i = 0 To 4 - 1
                data(i) = (CInt(SS_Hash(Key(3)) Xor data(i)) + Key(2)) Mod 256

                Me.Key(2) = data(i)
                Me.Key(3) = (Key(3) + 1) Mod 40
            Next i
        End Sub

        Public Sub EnQueue(ByVal state As Object)
            While CHARACTERs.Count > Config.ServerPlayerLimit
                If Not Me.Socket.Connected Then Exit Sub

                Dim response_full As New PacketClass(OPCODES.SMSG_AUTH_RESPONSE)
                response_full.AddInt8(AuthResponseCodes.AUTH_WAIT_QUEUE)
                response_full.AddInt32(CLIENTs.Count - CHARACTERs.Count)            'amount of players in queue
                Me.Send(response_full)

                Log.WriteLine(LogType.INFORMATION, "[{1}:{2}] AUTH_WAIT_QUEUE: Server player limit reached!", Me.IP, Me.Port)
                Thread.Sleep(6000)
            End While
            SendLoginOK(Me)
        End Sub
    End Class



#End Region

    Function IP2Int(ByVal IP As String) As UInteger
        Dim IpSplit() As String = IP.Split(".")
        If IpSplit.Length <> 4 Then Return 0
        Dim IpBytes(3) As Byte
        Try
            IpBytes(0) = CByte(IpSplit(3))
            IpBytes(1) = CByte(IpSplit(2))
            IpBytes(2) = CByte(IpSplit(1))
            IpBytes(3) = CByte(IpSplit(0))
            Return BitConverter.ToUInt32(IpBytes, 0)
        Catch
            Return 0
        End Try
    End Function

End Module
