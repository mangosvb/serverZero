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
Imports System.Runtime.Remoting
Imports System.Collections.Generic
Imports System.Security.Permissions
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common

Public Module WS_Network

    Public Authenticator As Authenticator

    Private LastPing As Integer = 0
    Public WC_MsTime As Integer = 0

    Public Function msTime() As Integer
        'DONE: Calculate the clusters timeGetTime("")
        Return WC_MsTime + (timeGetTime("") - LastPing)
    End Function

    Public Class WorldServerClass
        Inherits MarshalByRefObject
        Implements IWorld
        Implements IDisposable

        <CLSCompliant(False)> _
        Public _flagStopListen As Boolean = False
        Private m_RemoteChannel As Channels.IChannel = Nothing
        Private m_RemoteURI As String = ""
        Private m_LocalURI As String = ""
        Private m_Connection As Timer
        Private m_TimerCPU As Timer
        Private LastInfo As Date
        Private LastCPUTime As Double = 0.0F
        Private UsageCPU As Single = 0.0F
        Public Cluster As ICluster = Nothing

        Public Sub New()
            Try
                m_RemoteURI = String.Format("{0}://{1}:{2}/Cluster.rem", Config.ClusterConnectMethod, Config.ClusterConnectHost, Config.ClusterConnectPort)
                m_LocalURI = String.Format("{0}://{1}:{2}/WorldServer.rem", Config.ClusterConnectMethod, Config.LocalConnectHost, Config.LocalConnectPort)
                Cluster = Nothing

                'Create Remoting Channel
                Select Case Config.ClusterConnectMethod
                    Case "ipc"
                        m_RemoteChannel = New Channels.Ipc.IpcChannel(String.Format("{0}:{1}", Config.LocalConnectHost, Config.LocalConnectPort))
                    Case "tcp"
                        m_RemoteChannel = New Channels.Tcp.TcpChannel(Config.LocalConnectPort)
                End Select

                Channels.ChannelServices.RegisterChannel(m_RemoteChannel, False)

                'NOTE: Not protected remoting
                'RemotingServices.Marshal(CType(Me, IWorld), "WorldServer.rem")

                'NOTE: Password protected remoting
                Authenticator = New Authenticator(Me, Config.ClusterPassword)

                RemotingServices.Marshal(CType(Authenticator, Authenticator), "WorldServer.rem")
                Log.WriteLine(LogType.INFORMATION, "Interface UP at: {0}", m_LocalURI)

                'Notify Cluster About Us
                ClusterConnect()

                'Creating connection timer
                LastPing = timeGetTime("")
                m_Connection = New Timer(AddressOf CheckConnection, Nothing, 10000, 10000)

                'Creating CPU check timer
                m_TimerCPU = New Timer(AddressOf CheckCPU, Nothing, 1000, 1000)

            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error in {1}: {0}.", e.Message, e.Source)
            End Try
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                ClusterDisconnect()

                Channels.ChannelServices.UnregisterChannel(m_RemoteChannel)
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

        <SecurityPermissionAttribute(SecurityAction.Demand, Flags:=SecurityPermissionFlag.Infrastructure)> _
        Public Overrides Function InitializeLifetimeService() As Object
            Return Nothing
        End Function

        Public Sub ClusterConnect()
            While Cluster Is Nothing
                Try
                    'NOTE: Password protected remoting
                    Dim a As Authenticator = Activator.GetObject(GetType(Authenticator), m_RemoteURI)
                    Cluster = a.Login(Config.ClusterPassword)

                    'NOTE: Not protected remoting
                    'Cluster = RemotingServices.Connect(GetType(ICluster), m_RemoteURI)

                    If Cluster.Connect(m_LocalURI, Config.Maps) Then Exit While
                    Cluster.Disconnect(m_LocalURI, Config.Maps)
                Catch e As Exception
                    Log.WriteLine(LogType.FAILED, "Unable to connect to cluster. [{0}]", e.Message)
                End Try
                Cluster = Nothing
                Thread.Sleep(3000)
            End While

            Log.WriteLine(LogType.SUCCESS, "Contacted cluster [{0}]", m_RemoteURI)
        End Sub
        Public Sub ClusterDisconnect()
            Try
                Cluster.Disconnect(m_LocalURI, Config.Maps)
            Catch
            Finally
                Cluster = Nothing
            End Try
        End Sub

        Public Sub ClientTransfer(ByVal ID As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As Integer)
            If Not Maps.ContainsKey(map) Then
                CLIENTs(ID).Character.Dispose()
                CLIENTs(ID).Delete()
            End If

            Cluster.ClientTransfer(ID, posX, posY, posZ, ori, map)
        End Sub

        Public Sub ClientConnect(ByVal id As UInteger, ByVal client As ClientInfo) Implements IWorld.ClientConnect
            Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client connected", id)

            Dim objCharacter As New ClientClass(Client)

            If CLIENTs.ContainsKey(id) = True Then  'Ooops, the character is already loaded, remove it
                CLIENTs.Remove(id)
            End If
            CLIENTs.Add(id, objCharacter)
        End Sub

        Public Sub ClientDisconnect(ByVal id As UInteger) Implements IWorld.ClientDisconnect
            Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client disconnected", ID)

            If CLIENTs(ID).Character IsNot Nothing Then
                CLIENTs(ID).Character.Save()
            End If

            CLIENTs(ID).Delete()
            CLIENTs.Remove(ID)
        End Sub
        Public Sub ClientLogin(ByVal id As UInteger, ByVal guid As ULong) Implements IWorld.ClientLogin
            Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client login [0x{1:X}]", id, GUID)

            Try
                Dim client As ClientClass = CLIENTs(id)
                Dim Character As New CharacterObject(client, GUID)

                CHARACTERs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                CHARACTERs(GUID) = Character
                CHARACTERs_Lock.ReleaseWriterLock()

                'DONE: SMSG_CORPSE_RECLAIM_DELAY
                SendCorpseReclaimDelay(client, Character)

                'DONE: Cast talents and racial passive spells
                InitializeTalentSpells(Character)

                Character.Login()

                Log.WriteLine(LogType.USER, "[{0}:{1}] Player login complete [0x{2:X}]", client.IP, client.Port, GUID)
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error on login: {0}", e.ToString)
            End Try
        End Sub
        Public Sub ClientLogout(ByVal id As UInteger) Implements IWorld.ClientLogout
            Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client logout", ID)

            CLIENTs(ID).Character.Logout(Nothing)
        End Sub
        Public Sub ClientPacket(ByVal id As UInteger, ByVal data() As Byte) Implements IWorld.ClientPacket
            Dim p As New PacketClass(Data)
            Try
                CLIENTs(id).Packets.Enqueue(p)
                ThreadPool.QueueUserWorkItem(AddressOf CLIENTs(id).OnPacket)
            Finally
                p.Dispose()
            End Try
        End Sub
        Public Function ClientCreateCharacter(ByVal account As String, ByVal name As String, ByVal race As Byte, ByVal classe As Byte, ByVal gender As Byte, ByVal skin As Byte, ByVal face As Byte, ByVal hairStyle As Byte, ByVal hairColor As Byte, ByVal facialHair As Byte, ByVal outfitId As Byte) As Integer Implements IWorld.ClientCreateCharacter
            Return CreateCharacter(account, name, race, classe, gender, skin, face, hairStyle, hairColor, facialHair, outfitId)
        End Function

        Public Function Ping(ByVal timestamp As Integer, ByVal latency As Integer) As Integer Implements IWorld.Ping
            Log.WriteLine(LogType.INFORMATION, "Cluster ping: [{0}ms]", timeGetTime("") - timestamp)
            LastPing = timeGetTime("")
            WC_MsTime = timestamp + Latency

            Return timeGetTime("")
        End Function

        Public Sub CheckConnection(ByVal State As Object)
            If (timeGetTime("") - LastPing) > 40000 Then
                If Cluster IsNot Nothing Then
                    Log.WriteLine(LogType.FAILED, "Cluster timed out. Reconnecting")
                    ClusterDisconnect()
                End If
                ClusterConnect()
                LastPing = timeGetTime("")
            End If
        End Sub

        Public Sub CheckCPU(ByVal State As Object)
            Dim TimeSinceLastCheck As TimeSpan = Now.Subtract(LastInfo)
            UsageCPU = ((Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds - LastCPUTime) / TimeSinceLastCheck.TotalMilliseconds) * 100
            LastInfo = Now
            LastCPUTime = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds
        End Sub

        Public Sub ServerInfo(ByRef cpuUsage As Single, ByRef memoryUsage As ULong) Implements IWorld.ServerInfo
            memoryUsage = CULng(Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024))
            CPUUsage = UsageCPU
        End Sub

        Public Sub InstanceCreate(ByVal MapID As UInteger) Implements IWorld.InstanceCreate
            If Maps.ContainsKey(MapID) = False Then
                Dim Map As New TMap(MapID)
                'The New does a an add to the .Containskey collection above
            End If
        End Sub
        Public Sub InstanceDestroy(ByVal MapID As UInteger) Implements IWorld.InstanceDestroy
            Maps(MapID).Dispose()
        End Sub
        Public Function InstanceCanCreate(ByVal Type As Integer) As Boolean Implements IWorld.InstanceCanCreate
            Select Case CType(Type, MapTypes)
                Case MapTypes.MAP_BATTLEGROUND
                    Return Config.CreateBattlegrounds
                Case MapTypes.MAP_INSTANCE
                    Return Config.CreatePartyInstances
                Case MapTypes.MAP_RAID
                    Return Config.CreateRaidInstances
                Case MapTypes.MAP_COMMON
                    Return Config.CreateOther
            End Select
        End Function

        Public Sub ClientSetGroup(ByVal ID As UInteger, ByVal GroupID As Long) Implements IWorld.ClientSetGroup
            If Not CLIENTs.ContainsKey(ID) Then Return

            If GroupID = -1 Then
                Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client group set [G NULL]", ID)

                CLIENTs(ID).Character.Group = Nothing
                InstanceMapLeave(CLIENTs(ID).Character)
            Else
                Log.WriteLine(LogType.NETWORK, "[{0:000000}] Client group set [G{1:00000}]", ID, GroupID)

                If Not Groups.ContainsKey(GroupID) Then
                    Dim Group As New Group(GroupID)
                    'The New does a an add to the .Containskey collection above
                    'Groups.Add(GroupID, Group)
                    Cluster.GroupRequestUpdate(ID)
                End If

                CLIENTs(ID).Character.Group = Groups(GroupID)
                InstanceMapEnter(CLIENTs(ID).Character)
            End If
        End Sub
        Public Sub GroupUpdate(ByVal GroupID As Long, ByVal GroupType As Byte, ByVal GroupLeader As ULong, ByVal Members() As ULong) Implements IWorld.GroupUpdate
            If Groups.ContainsKey(GroupID) Then

                Dim list As New List(Of ULong)
                For Each GUID As ULong In Members
                    If CHARACTERs.ContainsKey(GUID) Then list.Add(GUID)
                Next

                Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update [{2}, {1} local members]", GroupID, list.Count, CType(GroupType, GroupType))

                If list.Count = 0 Then
                    Groups(GroupID).Dispose()
                Else
                    Groups(GroupID).Type = GroupType
                    Groups(GroupID).Leader = GroupLeader
                    Groups(GroupID).LocalMembers = list
                End If
            End If
        End Sub
        Public Sub GroupUpdateLoot(ByVal GroupID As Long, ByVal Difficulty As Byte, ByVal Method As Byte, ByVal Threshold As Byte, ByVal Master As ULong) Implements IWorld.GroupUpdateLoot
            If Groups.ContainsKey(GroupID) Then

                Log.WriteLine(LogType.NETWORK, "[G{0:00000}] Group update loot", GroupID)

                Groups(GroupID).DungeonDifficulty = Difficulty
                Groups(GroupID).LootMethod = Method
                Groups(GroupID).LootThreshold = Threshold

                If CHARACTERs.ContainsKey(Master) Then
                    Groups(GroupID).LocalLootMaster = CHARACTERs(Master)
                Else
                    Groups(GroupID).LocalLootMaster = Nothing
                End If
            End If
        End Sub

        Public Function GroupMemberStats(ByVal GUID As ULong, ByVal Flag As Integer) As Byte() Implements IWorld.GroupMemberStats
            If Flag = 0 Then Flag = PartyMemberStatsFlag.GROUP_UPDATE_FULL
            Dim p As PacketClass = BuildPartyMemberStats(CHARACTERs(GUID), Flag)
            p.UpdateLength()
            Return p.Data
        End Function

        Public Sub GuildUpdate(ByVal GUID As ULong, ByVal GuildID As UInteger, ByVal GuildRank As Byte) Implements IWorld.GuildUpdate
            CHARACTERs(GUID).GuildID = GuildID
            CHARACTERs(GUID).GuildRank = GuildRank

            CHARACTERs(GUID).SetUpdateFlag(EPlayerFields.PLAYER_GUILDID, GuildID)
            CHARACTERs(GUID).SetUpdateFlag(EPlayerFields.PLAYER_GUILDRANK, CInt(GuildRank))
            CHARACTERs(GUID).SendCharacterUpdate()
        End Sub

        Public Sub BattlefieldCreate(ByVal BattlefieldID As Integer, ByVal BattlefieldMapType As Byte, ByVal Map As UInteger) Implements IWorld.BattlefieldCreate
            Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Battlefield created", BattlefieldID)
        End Sub
        Public Sub BattlefieldDelete(ByVal BattlefieldID As Integer) Implements IWorld.BattlefieldDelete
            Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Battlefield deleted", BattlefieldID)
        End Sub
        Public Sub BattlefieldJoin(ByVal BattlefieldID As Integer, ByVal GUID As ULong) Implements IWorld.BattlefieldJoin
            Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Character [0x{1:X}] joined battlefield", BattlefieldID, GUID)
        End Sub
        Public Sub BattlefieldLeave(ByVal BattlefieldID As Integer, ByVal GUID As ULong) Implements IWorld.BattlefieldLeave
            Log.WriteLine(LogType.NETWORK, "[B{0:0000}] Character [0x{1:X}] left battlefield", BattlefieldID, GUID)
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
            While Packets.Count > 0
                Dim p As PacketClass = Packets.Dequeue
                Dim start As Integer = timeGetTime("")
                Try

                    Dim opCode As OPCODES = p.OpCode
                    If PacketHandlers.ContainsKey(opCode) = True Then
                        Try
                            PacketHandlers(opCode).Invoke(p, Me)

                            If timeGetTime("") - start > 100 Then
                                Log.WriteLine(LogType.WARNING, "Packet processing took too long: {0}, {1}ms", p.OpCode, timeGetTime("") - start)
                            End If
                        Catch e As Exception 'TargetInvocationException
                            Log.WriteLine(LogType.FAILED, "Opcode handler {2}:{3} caused an error:{1}{0}", e.ToString, vbNewLine, p.OpCode, p.OpCode)
                            'DumpPacket(packet.Data, Me)
                        End Try
                    Else
                        Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [DataLen={3} {4}]", IP, Port, CType(p.OpCode, Integer), p.Data.Length, p.OpCode)
                        DumpPacket(p.Data, Me)
                    End If
                Catch err As Exception
                    Log.WriteLine(LogType.FAILED, "Connection from [{0}:{1}] cause error {2}{3}", IP, Port, err.ToString, vbNewLine)
                    Me.Delete()
                Finally
                    p.Dispose()
                End Try
            End While
        End Sub

        Public Sub Send(ByRef data() As Byte)
            SyncLock Me
                Try
                    ClsWorldServer.Cluster.ClientSend(Index, data)
                Catch Err As Exception
                    If DEBUG_CONNECTION Then Exit Sub
                    Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {3}{2}", IP, Port, Err.ToString, vbNewLine)
                    ClsWorldServer.Cluster = Nothing
                    Me.Delete()
                End Try
            End SyncLock
        End Sub
        Public Sub Send(ByRef packet As PacketClass)
            If packet Is Nothing Then Throw New ApplicationException("Packet doesn't contain data!")
            SyncLock Me
                Try
                    If packet.OpCode = OPCODES.SMSG_UPDATE_OBJECT Then packet.CompressUpdatePacket()
                    packet.UpdateLength()

                    ClsWorldServer.Cluster.ClientSend(Index, packet.Data)
                    packet.Dispose()
                Catch Err As Exception
                    If DEBUG_CONNECTION Then Exit Sub
                    Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {3}{2}", IP, Port, Err.ToString, vbNewLine)
                    ClsWorldServer.Cluster = Nothing
                    Me.Delete()
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
                    ClsWorldServer.Cluster.ClientSend(Index, data)

                Catch Err As Exception
                    If DEBUG_CONNECTION Then Exit Sub
                    Log.WriteLine(LogType.CRITICAL, "Connection from [{0}:{1}] cause error {3}{2}", IP, Port, Err.ToString, vbNewLine)
                    ClsWorldServer.Cluster = Nothing
                    Me.Delete()
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
                Log.WriteLine(LogType.NETWORK, "Connection from [{0}:{1}] disposed", IP, Port)

                ClsWorldServer.Cluster.ClientDrop(Index)
                CLIENTs.Remove(Index)
                If Not Me.Character Is Nothing Then
                    Me.Character.Client = Nothing
                    Me.Character.Dispose()
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

            CLIENTs.Remove(Index)
            If Not Me.Character Is Nothing Then
                Me.Character.Client = Nothing
                Me.Character.Dispose()
            End If
            Me.Dispose()
        End Sub
        Public Sub Disconnect()
            Me.Delete()
        End Sub

        Public Sub New()
            Log.WriteLine(LogType.WARNING, "Creating debug connection!", Nothing)
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