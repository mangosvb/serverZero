' 
' Copyright (C) 2011 SpuriousZero <http://www.spuriousemu.com/>
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

Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Text
Imports System.Net.Sockets
Imports System.Net
Imports System.Security.Cryptography
Imports System.IO
Imports Emil.GMP

Module Realmserver
    Public ConsoleColor As New Global_System.ConsoleColorClass
    Private Connection As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP)
    Private ConnIP As IPAddress
    Private ConnPort As Integer

    Public Account As String = "test"
    Public Password As String = "test"
    Public VersionA As Byte = 1
    Public VersionB As Byte = 11
    Public VersionC As Byte = 2
    Public Revision As UShort = 5464

    Public RealmIP As String = "127.0.0.1"
    Public RealmPort As Integer = 3724

    Public ServerB(31) As Byte
    Public G() As Byte
    Public N() As Byte
    Public Salt(31) As Byte
    Public CrcSalt(15) As Byte

    Public A(31) As Byte
    Public PublicA(31) As Byte
    Public M1(19) As Byte
    Public CrcHash(19) As Byte
    Public SS_Hash() As Byte

    Const CMD_AUTH_LOGON_CHALLENGE As Byte = &H0
    Const CMD_AUTH_LOGON_PROOF As Byte = &H1
    Const CMD_AUTH_RECONNECT_CHALLENGE As Byte = &H2
    Const CMD_AUTH_RECONNECT_PROOF As Byte = &H3
    Const CMD_AUTH_UPDATESRV As Byte = &H4
    Const CMD_AUTH_REALMLIST As Byte = &H10

    Sub Main()
        Console.Title = "WoW Fake Client"
        ConsoleColor.SetConsoleColor(Global_System.ConsoleColorClass.ForegroundColors.LightGreen)
        Console.WriteLine("WoW Fake Client made by UniX")
        Console.WriteLine()
        ConsoleColor.SetConsoleColor()

        Worldserver.InitializePackets()
        timeBeginPeriod(1)

        ConnectToRealm()

        Dim NewThread As Thread
        NewThread = New Thread(AddressOf CheckConnection)
        NewThread.Name = "Checking Connection State"
        NewThread.Start()

        Dim sChatMsg As String = ""
        While True
            sChatMsg = Console.ReadLine()
            If sChatMsg.ToLower = "quit" Then Exit While
            SendChatMessage(sChatMsg)
        End While
    End Sub

    Public Function RS_Connected() As Boolean
        Return ((Connection IsNot Nothing) AndAlso Connection.Connected)
    End Function

    Sub CheckConnection()
        While True
            If RS_Connected() = False AndAlso WS_Connected() = False Then
                Thread.Sleep(6000)
                ConnectToRealm()
            End If
        End While
    End Sub

    Sub ConnectToRealm()
        Try
            ConsoleColor.SetConsoleColor(Global_System.ConsoleColorClass.ForegroundColors.Gray)
            Console.WriteLine("Connecting to {0}:{1}", RealmIP, RealmPort)
            Console.ForegroundColor = System.ConsoleColor.White

            Connection.Connect(RealmIP, RealmPort)
            Dim NewThread As Thread
            NewThread = New Thread(AddressOf ProcessServerConnection)
            NewThread.Name = "Realm Server, Connected"
            NewThread.Start()
        Catch e As Exception
            ConsoleColor.SetConsoleColor(Global_System.ConsoleColorClass.ForegroundColors.Red)
            Console.WriteLine("Could not connect to the server.")
            ConsoleColor.SetConsoleColor()
        End Try
    End Sub

    Sub ProcessServerConnection()
        ConnIP = CType(Connection.RemoteEndPoint, IPEndPoint).Address
        ConnPort = CType(Connection.RemoteEndPoint, IPEndPoint).Port
        Console.WriteLine("[{0}][Realm] Connected to [{1}:{2}].", Format(TimeOfDay, "HH:mm:ss"), ConnIP, ConnPort)

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
                    OnData(Buffer)
                End If
                If Not Connection.Connected Then Exit While
                If (Connection.Poll(100, SelectMode.SelectRead)) And (Connection.Available = 0) Then Exit While
            End While
        Catch ex As ObjectDisposedException
            'Nothing
        Catch ex As Exception
            Console.WriteLine("Error in realm socket.{0}{1}", vbNewLine, ex.ToString)
        End Try

        Connection.Close()
        Console.WriteLine("[{0}][Realm] Disconnected.", Format(TimeOfDay, "HH:mm:ss"))

        oThread.Abort()
    End Sub

    Sub Disconnect()
        Connection.Close()
    End Sub

    Sub OnConnect()
        Dim LogonChallenge As New PacketClass(CMD_AUTH_LOGON_CHALLENGE)
        LogonChallenge.AddInt8(&H8)
        LogonChallenge.AddUInt16(0) 'Packet length
        LogonChallenge.AddString("WoW")
        LogonChallenge.AddInt8(VersionA) 'Version
        LogonChallenge.AddInt8(VersionB) 'Version
        LogonChallenge.AddInt8(VersionC) 'Version
        LogonChallenge.AddUInt16(Revision) 'Revision
        LogonChallenge.AddString("x86", , True)
        LogonChallenge.AddString("Win", , True)
        LogonChallenge.AddString("enUS", False, True)
        LogonChallenge.AddInt32(&H3C) 'Timezone
        LogonChallenge.AddUInt32(BitConverter.ToUInt32(CType(Connection.LocalEndPoint, IPEndPoint).Address.GetAddressBytes, 0))
        LogonChallenge.AddInt8(Account.Length)
        LogonChallenge.AddString(Account.ToUpper, False)
        LogonChallenge.AddUInt16(LogonChallenge.Data.Length - 4, 2)
        SendR(LogonChallenge)
        LogonChallenge.Dispose()

        Console.WriteLine("[{0}][Realm] Sent Logon Challenge.", Format(TimeOfDay, "HH:mm:ss"))
    End Sub

    Sub OnData(ByVal Buffer() As Byte)
        Dim Packet As New PacketClass(Buffer, True)
        Select Case Packet.OpCode
            Case CMD_AUTH_LOGON_CHALLENGE
                Console.WriteLine("[{0}][Realm] Received Logon Challenged.", Format(TimeOfDay, "HH:mm:ss"))
                Select Case Buffer(1)
                    Case 0 'No error
                        Console.WriteLine("[{0}][Realm] Challenge Success.", Format(TimeOfDay, "HH:mm:ss"))
                        Packet.Offset = 3
                        ServerB = Packet.GetByteArray(32)
                        Dim G_len As Byte = Packet.GetInt8
                        G = Packet.GetByteArray(CInt(G_len))
                        Dim N_len As Byte = Packet.GetInt8
                        N = Packet.GetByteArray(CInt(N_len))
                        Salt = Packet.GetByteArray(32)
                        CrcSalt = Packet.GetByteArray(16)

                        CalculateProof()
                        Thread.Sleep(100)

                        Dim LogonProof As New PacketClass(CMD_AUTH_LOGON_PROOF)
                        LogonProof.AddByteArray(PublicA)
                        LogonProof.AddByteArray(M1)
                        LogonProof.AddByteArray(CrcHash)
                        SendR(LogonProof)
                        LogonProof.Dispose()
                    Case 4, 5 'Bad user
                        Console.WriteLine("[{0}][Realm] Bad account information, the account did not exist.", Format(TimeOfDay, "HH:mm:ss"))
                        Connection.Close()
                    Case 9 'Bad version
                        Console.WriteLine("[{0}][Realm] Bad client version (the server does not allow our version).", Format(TimeOfDay, "HH:mm:ss"))
                        Connection.Close()
                    Case Else
                        Console.WriteLine("[{0}][Realm] Unknown challenge error [{1}].", Format(TimeOfDay, "HH:mm:ss"), Buffer(1))
                        Connection.Close()
                End Select
            Case CMD_AUTH_LOGON_PROOF
                Console.WriteLine("[{0}][Realm] Received Logon Proof.", Format(TimeOfDay, "HH:mm:ss"))
                Select Case Buffer(1)
                    Case 0 'No error
                        Console.WriteLine("[{0}][Realm] Proof Success.", Format(TimeOfDay, "HH:mm:ss"))

                        Dim RealmList As New PacketClass(CMD_AUTH_REALMLIST)
                        RealmList.AddInt32(0)
                        SendR(RealmList)
                        RealmList.Dispose()
                    Case 4, 5 'Bad user
                        Console.WriteLine("[{0}][Realm] Bad account information, your password was incorrect.", Format(TimeOfDay, "HH:mm:ss"))
                        Connection.Close()
                    Case 9 'Bad version
                        Console.WriteLine("[{0}][Realm] Bad client version (the crc files are either too old or to new for this server).", Format(TimeOfDay, "HH:mm:ss"))
                        Connection.Close()
                    Case Else
                        Console.WriteLine("[{0}][Realm] Unknown proof error [{1}].", Format(TimeOfDay, "HH:mm:ss"), Buffer(1))
                        Connection.Close()
                End Select
            Case CMD_AUTH_REALMLIST
                Console.WriteLine("[{0}][Realm] Received Realm List.", Format(TimeOfDay, "HH:mm:ss"))

                Packet.Offset = 7
                Dim RealmCount As Integer = CInt(Packet.GetInt8)
                If RealmCount > 0 Then
                    For i As Integer = 1 To RealmCount
                        Dim RealmType As Byte = Packet.GetInt8
                        Dim RealmLocked As Byte = Packet.GetInt8
                        Dim Unk1 As Byte = Packet.GetInt8
                        Dim Unk2 As Byte = Packet.GetInt8
                        Dim RealmStatus As Byte = Packet.GetInt8
                        Dim RealmName As String = Packet.GetString
                        Dim RealmIP As String = Packet.GetString
                        Dim RealmPopulation As Single = Packet.GetFloat
                        Dim RealmCharacters As Byte = Packet.GetInt8
                        Dim RealmTimezone As Byte = Packet.GetInt8
                        Dim Unk3 As Byte = Packet.GetInt8()

                        Console.WriteLine("[{0}][Realm] Connecting to realm [{1}][{2}].", Format(TimeOfDay, "HH:mm:ss"), RealmName, RealmIP)

                        If InStr(RealmIP, ":") > 0 Then
                            Dim SplitIP() As String = Split(RealmIP, ":")
                            If SplitIP.Length = 2 Then
                                If IsNumeric(SplitIP(1)) Then
                                    Worldserver.ConnectToServer(SplitIP(0), CInt(SplitIP(1)))
                                Else
                                    Console.WriteLine("[{0}][Realm] Invalid IP in realmlist [{1}].", Format(TimeOfDay, "HH:mm:ss"), RealmIP)
                                End If
                            Else
                                Console.WriteLine("[{0}][Realm] Invalid IP in realmlist [{1}].", Format(TimeOfDay, "HH:mm:ss"), RealmIP)
                            End If
                        Else
                            Console.WriteLine("[{0}][Realm] Invalid IP in realmlist [{1}].", Format(TimeOfDay, "HH:mm:ss"), RealmIP)
                        End If
                        Exit For
                    Next
                Else
                    Console.WriteLine("[{0}][Realm] No realms were found.", Format(TimeOfDay, "HH:mm:ss"))
                End If
            Case Else
                Console.WriteLine("[{0}][Realm] Unknown opcode [{1}].", Format(TimeOfDay, "HH:mm:ss"), Packet.OpCode)
        End Select
    End Sub

    Sub SendR(ByVal Packet As PacketClass)
        Dim i As Integer = Connection.Send(Packet.Data, 0, Packet.Data.Length, SocketFlags.None)
    End Sub

    Sub CalculateProof()
        RAND_bytes(A, 32)
        Array.Reverse(A)

        Dim tempStr As String = Account.ToUpper & ":" & Password.ToUpper
        Dim temp() As Byte = System.Text.Encoding.ASCII.GetBytes(tempStr.ToCharArray)
        Dim algorithm1 As New SHA1Managed
        temp = algorithm1.ComputeHash(temp)
        Dim X() As Byte = algorithm1.ComputeHash(Concat(Salt, temp))
        Array.Reverse(X)
        Array.Reverse(N)
        Dim K() As Byte = {3}
        Dim S() As Byte = New Byte(31) {}

        Dim BNg As New BigInteger(G)
        Dim BNa As New BigInteger(A)
        Dim BNn As New BigInteger(N)
        Dim BNx As New BigInteger(X)
        Dim BNk As New BigInteger(K)
        Dim BNpublicA As BigInteger = BNg.modPow(BNa, BNn)
        PublicA = BNpublicA.getBytes
        Array.Reverse(PublicA)

        Dim U() As Byte = algorithm1.ComputeHash(Concat(PublicA, ServerB))
        Array.Reverse(ServerB)
        Array.Reverse(U)
        Dim BNu As New BigInteger(U)
        Dim BNb As New BigInteger(ServerB)

        'S= (B - kg^x) ^ (a + ux)   (mod N)
        Dim temp1 As New BigInteger
        Dim temp2 As New BigInteger
        Dim temp3 As New BigInteger
        Dim temp4 As New BigInteger
        Dim temp5 As New BigInteger

        'Temp1 = g ^ x mod n
        temp1 = BNg.modPow(BNx, BNn)

        'Temp2 = k * Temp1
        temp2 = BNk * temp1

        'Temp3 = B - Temp2
        temp3 = BNb - temp2

        'Temp4 = u * x
        temp4 = BNu * BNx

        'Temp5 = a + Temp4
        temp5 = BNa + temp4

        'S = Temp3 ^ Temp5 mod n
        Dim BNs As BigInteger = temp3.modPow(temp5, BNn)
        S = BNs.getBytes
        Array.Reverse(S)

        Dim list1 As New ArrayList
        list1 = SplitArray(S)
        list1.Item(0) = algorithm1.ComputeHash(CType(list1.Item(0), Byte()))
        list1.Item(1) = algorithm1.ComputeHash(CType(list1.Item(1), Byte()))
        SS_Hash = Combine(CType(list1.Item(0), Byte()), CType(list1.Item(1), Byte()))

        tempStr = UCase(Account.ToUpper)
        Dim User_Hash() As Byte = algorithm1.ComputeHash(UTF8Encoding.UTF8.GetBytes(tempStr.ToCharArray))
        Array.Reverse(N)
        Array.Reverse(ServerB)
        Dim N_Hash() As Byte = algorithm1.ComputeHash(N)
        Dim G_Hash() As Byte = algorithm1.ComputeHash(G)
        Dim NG_Hash(19) As Byte
        For i As Integer = 0 To 19
            NG_Hash(i) = N_Hash(i) Xor G_Hash(i)
        Next

        temp = Concat(NG_Hash, User_Hash)
        temp = Concat(temp, Salt)
        temp = Concat(temp, PublicA)
        temp = Concat(temp, ServerB)
        temp = Concat(temp, SS_Hash)
        M1 = algorithm1.ComputeHash(temp)

        CrcHash = New Byte(16) {}
    End Sub

    Sub CalculateProof2()
        RAND_bytes(A, 32)
        Array.Reverse(A)

        Dim tempStr As String = Account.ToUpper & ":" & Password.ToUpper
        Dim temp() As Byte = System.Text.Encoding.ASCII.GetBytes(tempStr.ToCharArray)
        Dim algorithm1 As New SHA1Managed
        temp = algorithm1.ComputeHash(temp)
        Dim X() As Byte = algorithm1.ComputeHash(Concat(Salt, temp))
        Array.Reverse(X)
        Array.Reverse(N)
        Dim K() As Byte = {3}
        Dim S() As Byte = New Byte(31) {}

        Dim BNpublicA As IntPtr = BN_new
        Dim ptr1 As IntPtr = BN_CTX_new
        Dim BNg As IntPtr = BN_bin2bn(G, G.Length, IntPtr.Zero)
        Dim BNa As IntPtr = BN_bin2bn(A, A.Length, IntPtr.Zero)
        Dim BNn As IntPtr = BN_bin2bn(N, N.Length, IntPtr.Zero)
        Dim BNx As IntPtr = BN_bin2bn(X, X.Length, IntPtr.Zero)
        Dim BNk As IntPtr = BN_bin2bn(K, K.Length, IntPtr.Zero)
        BN_mod_exp(BNpublicA, BNg, BNa, BNn, ptr1)
        BN_bn2bin(BNpublicA, PublicA)
        Array.Reverse(PublicA)

        Dim U() As Byte = algorithm1.ComputeHash(Concat(PublicA, ServerB))
        Array.Reverse(ServerB)
        Array.Reverse(U)
        Dim BNu As IntPtr = BN_bin2bn(U, U.Length, IntPtr.Zero)
        Dim BNb As IntPtr = BN_bin2bn(ServerB, ServerB.Length, IntPtr.Zero)

        'S= (B - kg^x) ^ (a + ux)   (mod N)
        Dim temp1 As IntPtr = BN_new
        Dim temp2 As IntPtr = BN_new
        Dim temp3 As IntPtr = BN_new
        Dim temp4 As IntPtr = BN_new
        Dim temp5 As IntPtr = BN_new
        Dim BNs As IntPtr = BN_new

        'Temp1 = g ^ x mod n
        BN_mod_exp(temp1, BNg, BNx, BNn, ptr1)

        'Temp2 = k * Temp1
        BN_mul(temp2, BNk, temp1, ptr1)

        'Temp3 = B - Temp2
        BN_sub(temp3, BNb, temp2)

        'Temp4 = u * x
        BN_mul(temp4, BNu, BNx, ptr1)

        'Temp5 = a + Temp4
        BN_add(temp5, BNa, temp4)

        'S = Temp3 ^ Temp5 mod n
        BN_mod_exp(BNs, temp3, temp5, BNn, ptr1)
        BN_bn2bin(BNs, S)
        Array.Reverse(S)

        Dim list1 As New ArrayList
        list1 = SplitArray(S)
        list1.Item(0) = algorithm1.ComputeHash(CType(list1.Item(0), Byte()))
        list1.Item(1) = algorithm1.ComputeHash(CType(list1.Item(1), Byte()))
        SS_Hash = Combine(CType(list1.Item(0), Byte()), CType(list1.Item(1), Byte()))

        tempStr = UCase(Account.ToUpper)
        Dim User_Hash() As Byte = algorithm1.ComputeHash(UTF8Encoding.UTF8.GetBytes(tempStr.ToCharArray))
        Array.Reverse(N)
        Array.Reverse(ServerB)
        Dim N_Hash() As Byte = algorithm1.ComputeHash(N)
        Dim G_Hash() As Byte = algorithm1.ComputeHash(G)
        Dim NG_Hash(19) As Byte
        For i As Integer = 0 To 19
            NG_Hash(i) = N_Hash(i) Xor G_Hash(i)
        Next

        temp = Concat(NG_Hash, User_Hash)
        temp = Concat(temp, Salt)
        temp = Concat(temp, PublicA)
        temp = Concat(temp, ServerB)
        temp = Concat(temp, SS_Hash)
        M1 = algorithm1.ComputeHash(temp)

        CrcHash = New Byte(16) {}
    End Sub

    Private Function SplitArray(ByVal bo As Byte()) As ArrayList
        Dim buffer1 As Byte() = New Byte((bo.Length - 1) - 1) {}
        If (((bo.Length Mod 2) <> 0) AndAlso (bo.Length > 2)) Then
            Buffer.BlockCopy(bo, 1, buffer1, 0, bo.Length)
        End If
        Dim buffer2 As Byte() = New Byte((bo.Length / 2) - 1) {}
        Dim buffer3 As Byte() = New Byte((bo.Length / 2) - 1) {}
        Dim num1 As Integer = 0
        Dim num2 As Integer = 1
        Dim num3 As Integer
        For num3 = 0 To buffer2.Length - 1
            buffer2(num3) = bo(num1)
            num1 += 1
            num1 += 1
        Next num3
        Dim num4 As Integer
        For num4 = 0 To buffer3.Length - 1
            buffer3(num4) = bo(num2)
            num2 += 1
            num2 += 1
        Next num4
        Dim list1 As New ArrayList
        list1.Add(buffer2)
        list1.Add(buffer3)
        Return list1
    End Function

    Private Function Combine(ByVal b1 As Byte(), ByVal b2 As Byte()) As Byte()
        If (b1.Length = b2.Length) Then
            Dim buffer1 As Byte() = New Byte((b1.Length + b2.Length) - 1) {}
            Dim num1 As Integer = 0
            Dim num2 As Integer = 1
            Dim num3 As Integer
            For num3 = 0 To b1.Length - 1
                buffer1(num1) = b1(num3)
                num1 += 1
                num1 += 1
            Next num3
            Dim num4 As Integer
            For num4 = 0 To b2.Length - 1
                buffer1(num2) = b2(num4)
                num2 += 1
                num2 += 1
            Next num4
            Return buffer1
        End If
        Return Nothing
    End Function

    Public Function Concat(ByVal a As Byte(), ByVal b As Byte()) As Byte()
        Dim buffer1 As Byte() = New Byte((a.Length + b.Length) - 1) {}
        Dim num1 As Integer
        For num1 = 0 To a.Length - 1
            buffer1(num1) = a(num1)
        Next num1
        Dim num2 As Integer
        For num2 = 0 To b.Length - 1
            buffer1((num2 + a.Length)) = b(num2)
        Next num2
        Return buffer1
    End Function

    Public Declare Function BN_add Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr) As Integer
    Public Declare Function BN_sub Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr) As Integer
    Public Declare Function BN_bin2bn Lib "LIBEAY32" (ByVal ByteArrayIn As Byte(), ByVal length As Integer, ByVal [to] As IntPtr) As IntPtr
    Public Declare Function BN_bn2bin Lib "LIBEAY32" (ByVal a As IntPtr, ByVal [to] As Byte()) As Integer
    Public Declare Function BN_CTX_free Lib "LIBEAY32" (ByVal a As IntPtr) As Integer
    Public Declare Function BN_CTX_new Lib "LIBEAY32" () As IntPtr
    Public Declare Function BN_mod Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr) As Integer
    Public Declare Function BN_mod_exp Lib "LIBEAY32" (ByVal res As IntPtr, ByVal a As IntPtr, ByVal p As IntPtr, ByVal m As IntPtr, ByVal ctx As IntPtr) As IntPtr
    Public Declare Function BN_mul Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr) As Integer
    Public Declare Function BN_new Lib "LIBEAY32" () As IntPtr

    <DllImport("LIBEAY32.DLL")> _
    Public Function RAND_bytes(ByVal buf As Byte(), ByVal num As Integer) As Integer

    End Function

End Module
