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
Imports System.Net.Sockets
Imports System.Net

Public Module Worldserver
    Private Connection As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP)
    Private ConnIP As IPAddress
    Private ConnPort As Integer
    Public Queue As New Queue

    Private PingTimer As Timer = Nothing
    Public PingSent As Integer = 0
    Public CurrentPing As UInteger = 0
    Public CurrentLatency As Integer = 0

    Public PacketHandlers As New Dictionary(Of OPCODES, HandlePacket)
    Delegate Sub HandlePacket(ByRef Packet As PacketClass)

    Public ClientSeed As UInteger = 0
    Public ServerSeed As UInteger = 0
    Public Key(3) As Byte

    Public CharacterGUID As ULong = 0

    Public Encoding As Boolean = False
    Public Decoding As Boolean = False

    Public Declare Function timeGetTime Lib "winmm.dll" () As Integer
    Public Declare Function timeBeginPeriod Lib "winmm.dll" (ByVal uPeriod As Integer) As Integer

    Public Function WS_Connected() As Boolean
        Return ((Connection IsNot Nothing) AndAlso Connection.Connected)
    End Function

    Sub InitializePackets()
        PacketHandlers(OPCODES.SMSG_PONG) = CType(AddressOf On_SMSG_PONG, HandlePacket)
        PacketHandlers(OPCODES.SMSG_AUTH_CHALLENGE) = CType(AddressOf On_SMSG_AUTH_CHALLENGE, HandlePacket)
        PacketHandlers(OPCODES.SMSG_AUTH_RESPONSE) = CType(AddressOf On_SMSG_AUTH_RESPONSE, HandlePacket)
        PacketHandlers(OPCODES.SMSG_CHAR_ENUM) = CType(AddressOf On_SMSG_CHAR_ENUM, HandlePacket)
        PacketHandlers(OPCODES.SMSG_SET_REST_START) = CType(AddressOf On_SMSG_SET_REST_START, HandlePacket)
        PacketHandlers(OPCODES.SMSG_MESSAGECHAT) = CType(AddressOf On_SMSG_MESSAGECHAT, HandlePacket)
        PacketHandlers(OPCODES.SMSG_WARDEN_DATA) = CType(AddressOf On_SMSG_WARDEN_DATA, HandlePacket)
    End Sub

    Sub ConnectToServer(ByVal IP As String, ByVal Port As Integer)
        Try
            Connection.Connect(IP, Port)
            Dim NewThread As Thread
            NewThread = New Thread(AddressOf ProcessServerConnection)
            NewThread.Name = "World Server, Connected"
            NewThread.Start()
        Catch e As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Could not connect to the world server.")
            Console.ForegroundColor = ConsoleColor.White
        End Try
    End Sub

    Sub ProcessServerConnection()
        ConnIP = CType(Connection.RemoteEndPoint, IPEndPoint).Address
        ConnPort = CType(Connection.RemoteEndPoint, IPEndPoint).Port
        Console.WriteLine("[{0}][World] Connected to [{1}:{2}].", Format(TimeOfDay, "HH:mm:ss"), ConnIP, ConnPort)

        Dim Buffer() As Byte
        Dim bytes As Integer
        Dim oThread As Thread
        oThread = Thread.CurrentThread()

        OnConnect()

        Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000)
        Connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000)

        Try
            While True
                Thread.Sleep(10)
                If Connection.Available > 0 Then
                    ReDim Buffer(Connection.Available - 1)
                    bytes = Connection.Receive(Buffer, Buffer.Length, 0)

                    While bytes > 0
                        If Decoding Then Decode(Buffer)

                        'Calculate Length from packet
                        Dim PacketLen As Integer = (Buffer(1) + Buffer(0) * 256) + 2

                        If bytes < PacketLen Then
                            Console.WriteLine("[{0}][World] Bad Packet length [{1}][{2}] bytes", Format(TimeOfDay, "HH:mm:ss"), bytes, PacketLen)
                            Exit While
                        End If

                        'Move packet to Data
                        Dim data(PacketLen - 1) As Byte
                        Array.Copy(Buffer, data, PacketLen)

                        Dim Packet As New PacketClass(data)
                        SyncLock Queue.SyncRoot
                            Queue.Enqueue(Packet)
                        End SyncLock

                        bytes -= PacketLen
                        Array.Copy(Buffer, PacketLen, Buffer, 0, bytes)
                    End While

                    ThreadPool.QueueUserWorkItem(AddressOf OnData)
                End If
                If Not Connection.Connected Then Exit While
                If (Connection.Poll(100, SelectMode.SelectRead)) And (Connection.Available = 0) Then Exit While
            End While
        Catch
        End Try

        Connection.Close()
        Console.WriteLine("[{0}][World] Disconnected.", Format(TimeOfDay, "HH:mm:ss"))

        oThread.Abort()
    End Sub

    Sub Disconnect()
        Connection.Close()
    End Sub

    Sub OnConnect()
        'Disconnect the realm server now that we're connected
        Realmserver.Disconnect()

        'Reset values
        Encoding = False
        Decoding = False
        Key(0) = 0
        Key(1) = 0
        Key(2) = 0
        Key(3) = 0

        'Start the ping timer
        PingTimer = New Timer(AddressOf Ping, Nothing, 30000, 30000)
    End Sub

    Sub Ping(ByVal State As Object)
        Try
            If CurrentPing = UInteger.MaxValue Then CurrentPing = 0
            CurrentPing += 1
            PingSent = timeGetTime
            Dim Ping As New PacketClass(OPCODES.CMSG_PING)
            Ping.AddUInt32(CurrentPing)
            Ping.AddInt32(CurrentLatency)
            Send(Ping)
            Ping.Dispose()
        Catch e As Exception
            PingTimer.Dispose()
        End Try
    End Sub

    Sub OnData()
        Dim Packet As PacketClass
        While Queue.Count > 0
            SyncLock Queue.SyncRoot
                Packet = Queue.Dequeue
            End SyncLock

            If PacketHandlers.ContainsKey(Packet.OpCode) = True Then
                Try
                    PacketHandlers(Packet.OpCode).Invoke(Packet)
                Catch e As Exception
                    Console.WriteLine("Opcode handler {2}:{2:X} caused an error:{1}{0}", e.ToString, vbNewLine, Packet.OpCode)
                End Try
            Else
                'Console.WriteLine("[{0}][World] Unknown Opcode 0x{1:X} [{1}], DataLen={2}", Format(TimeOfDay, "HH:mm:ss"), Packet.OpCode, Packet.Length)
                'DumpPacket(Packet.Data)
            End If

            Packet.Dispose()
        End While
    End Sub

    Sub Send(ByVal Packet As PacketClass)
        If Encoding Then Encode(Packet.Data)
        Dim i As Integer = Connection.Send(Packet.Data, 0, Packet.Data.Length, SocketFlags.None)
    End Sub

    Sub Encode(ByRef Buffer() As Byte)
        'Encoding client messages
        Dim T As Integer

        For T = 0 To 5
            Buffer(T) = CByte((((SS_Hash(Key(3)) Xor Buffer(T)) And &HFF) + Key(2)) And &HFF)
            Key(2) = Buffer(T)
            Key(3) = CByte((Key(3) + 1) Mod 40)
        Next T
    End Sub

    Sub Decode(ByRef Buffer() As Byte)
        'Decoding server messages
        Dim T As Integer
        Dim A As Integer
        Dim B As Integer
        Dim d As Integer

        For T = 0 To 3
            A = Key(0)
            Key(0) = Buffer(T)
            B = Buffer(T)

            B = CByte((B - A) And &HFF)
            d = Key(1)
            A = SS_Hash(d)
            A = CByte((A Xor B) And &HFF)
            Buffer(T) = A

            A = Key(1)
            A = A + 1
            Key(1) = A Mod 40
        Next T
    End Sub
End Module
