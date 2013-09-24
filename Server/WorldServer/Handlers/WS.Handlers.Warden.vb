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

Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common
Imports System.Runtime.InteropServices


Public Module WS_Handlers_Warden

    Private OutKeyAdr As Integer
    Private InKeyAdr As Integer

#Region "Warden.Handlers"
    Public Sub On_CMSG_WARDEN_DATA(ByRef packet As PacketClass, ByRef Client As ClientClass)
        'START Warden Decryption
        Dim b(packet.Data.Length - 6 - 1) As Byte
        Buffer.BlockCopy(packet.Data, 6, b, 0, b.Length)
        RC4.Crypt(b, Client.Character.WardenData.KeyOut)
        Buffer.BlockCopy(b, 0, packet.Data, 6, b.Length)
        'END

        packet.GetInt16()
        Dim Response As MaievResponse = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_WARDEN_DATA [{2}]", Client.IP, Client.Port, Response)

        If Client.Character.WardenData.Ready Then
            Select Case Response
                Case MaievResponse.MAIEV_RESPONSE_FAILED_OR_MISSING
                    MaievSendTransfer(Client.Character)

                Case MaievResponse.MAIEV_RESPONSE_SUCCESS
                    MaievSendSeed(Client.Character)

                Case MaievResponse.MAIEV_RESPONSE_RESULT
                    MaievResult(Client.Character, packet)

                Case MaievResponse.MAIEV_RESPONSE_HASH
                    Dim hash(20 - 1) As Byte
                    Buffer.BlockCopy(packet.Data, packet.Offset, hash, 0, 20)

                    'TODO: Only one character can do this at the same time

                    Maiev.GenerateNewRC4Keys(Client.Character.WardenData.K)

                    Dim PacketData(16) As Byte
                    PacketData(0) = MaievOpcode.MAIEV_MODULE_SEED
                    Buffer.BlockCopy(Client.Character.WardenData.Seed, 0, PacketData, 1, 16)

                    Dim HandledBytes As Integer = Maiev.HandlePacket(PacketData)
                    If HandledBytes <= 0 Then
                        Log.WriteLine(LogType.CRITICAL, "[WARDEN] Failed to handle 0x05 packet.")
                        Exit Sub
                    End If
                    Dim thePacket() As Byte = Maiev.ReadPacket()
                    Dim ourHash(20 - 1) As Byte
                    Array.Copy(thePacket, 1, ourHash, 0, ourHash.Length)

                    Maiev.ReadXorByte(Client.Character)
                    Maiev.ReadKeys(Client.Character)

                    Log.WriteLine(LogType.DEBUG, "[WARDEN] XorByte: {0}", Client.Character.WardenData.xorByte)

                    Dim HashCorrect As Boolean = True
                    For i As Integer = 0 To 19
                        If hash(i) <> ourHash(i) Then
                            HashCorrect = False
                            Exit For
                        End If
                    Next

                    If Not HashCorrect Then
                        Log.WriteLine(LogType.CRITICAL, "[WARDEN] Hashes in packet 0x05 didn't match. Cheater?")
                    Else
                        'MaievSendUnk(Client.Character)
                    End If
            End Select
        End If
    End Sub

    Public Sub MaievInit(ByRef c As CharacterObject)
        Dim k As Byte() = WS.Cluster.ClientGetCryptKey(c.Client.Index)
        Dim m As New MaievData(k)
        Dim seedOut As Byte() = m.GetBytes(16)
        Dim seedIn As Byte() = m.GetBytes(16)
        c.WardenData.KeyOut = RC4.Init(seedOut)
        c.WardenData.KeyIn = RC4.Init(seedIn)

        c.WardenData.Ready = True
        c.WardenData.Scan = New WardenScan(c)

        c.WardenData.xorByte = 0
        c.WardenData.K = k
        RAND_bytes(c.WardenData.Seed, 16)

        'Sending our test module
        MaievSendModule(c)
    End Sub
    Public Sub MaievSendModule(ByRef c As CharacterObject)
        If Not c.WardenData.Ready Then Throw New ApplicationException("Maiev.mod not ready!")

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_WARDEN_DATA [{2}]", c.Client.IP, c.Client.Port, Maiev.ModuleName)

        Dim r As New PacketClass(OPCODES.SMSG_WARDEN_DATA)
        r.AddInt8(MaievOpcode.MAIEV_MODULE_INFORMATION)     'Opcode
        r.AddByteArray(Maiev.WardenModule)                  'MD5 checksum of the modules compressed encrypted data
        r.AddByteArray(Maiev.ModuleKey)                     'RC4 seed for decryption of the module
        r.AddUInt32(Maiev.ModuleSize)                       'Module Compressed Length - Size of the packet

        SendWardenPacket(c, r)
    End Sub
    Public Sub MaievSendTransfer(ByRef c As CharacterObject)
        If Not c.WardenData.Ready Then Throw New ApplicationException("Maiev.mod not ready!")

        Dim file As New IO.FileStream(String.Format("warden\{0}.bin", Maiev.ModuleName), IO.FileMode.Open, IO.FileAccess.Read)
        Dim size As Integer = file.Length()

        While size > 500
            Dim r As New PacketClass(OPCODES.SMSG_WARDEN_DATA)
            r.AddInt8(MaievOpcode.MAIEV_MODULE_TRANSFER)                'Opcode
            r.AddInt16(500)                                             'Payload Length
            For i As Integer = 1 To 500                                 'Payload
                r.AddInt8(file.ReadByte)
            Next

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_WARDEN_DATA [data]", c.Client.IP, c.Client.Port)
            'DumpPacket(r.Data, c.Client, 4)

            SendWardenPacket(c, r)

            size -= 500
        End While

        If size > 0 Then
            Dim r As New PacketClass(OPCODES.SMSG_WARDEN_DATA)
            r.AddInt8(MaievOpcode.MAIEV_MODULE_TRANSFER)                'Opcode
            r.AddUInt16(size)                                           'Payload Length
            For i As Integer = 1 To size                                'Payload
                r.AddInt8(file.ReadByte)
            Next

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_WARDEN_DATA [done]", c.Client.IP, c.Client.Port)
            'DumpPacket(r.Data, c.Client, 4)

            SendWardenPacket(c, r)
        End If

    End Sub
    Public Sub MaievSendUnk(ByRef c As CharacterObject)
        Dim unk As New PacketClass(OPCODES.SMSG_WARDEN_DATA)
        unk.AddInt8(MaievOpcode.MAIEV_MODULE_UNK)
        unk.AddByteArray(New Byte() {&H14, &H0, &H60, &HD0, &HFE, &H2C, &H1, &H0, &H2, &H0, &H20, &H1A, &H36, &H0, &HC0, &HE3, &H35, &H0, &H50, &HF1, &H35, &H0, &HC0, &HF5, &H35, &H0, &H3, &H8, &H0, &H77, &H6C, &H93, &HA9, &H4, &H0, &H0, &H60, &HA8, &H40, &H0, &H1, &H3, &H8, &H0, &H36, &H85, &HEA, &HF0, &H1, &H1, &H0, &H90, &HF4, &H45, &H0, &H1})

        SendWardenPacket(c, unk)
        unk.Dispose()
    End Sub
    Public Sub MaievSendCheck(ByRef c As CharacterObject)
        If Not c.WardenData.Ready Then Throw New ApplicationException("Maiev.mod not ready!")

        c.WardenData.Scan.Do_TIMING_CHECK()
        Dim packet As PacketClass = c.WardenData.Scan.GetPacket()
        SendWardenPacket(c, packet)
        packet.Dispose()
    End Sub
    Public Sub MaievSendSeed(ByRef c As CharacterObject)
        Dim r As New PacketClass(OPCODES.SMSG_WARDEN_DATA)
        r.AddInt8(MaievOpcode.MAIEV_MODULE_SEED)
        r.AddByteArray(c.WardenData.Seed)

        SendWardenPacket(c, r)
    End Sub

    Public Sub MaievResult(ByRef c As CharacterObject, ByRef Packet As PacketClass)
        Dim bufLen As UShort = Packet.GetUInt16()
        Dim checkSum As UInteger = Packet.GetUInt32()

        'DONE: Make sure the checkSum is correct
        Dim tmpOffset As Integer = Packet.Offset
        Dim data() As Byte = Packet.GetByteArray()
        Packet.Offset = tmpOffset
        If ControlChecksum(checkSum, data) = False Then
            Log.WriteLine(LogType.CRITICAL, "[WARDEN] Failed checkSum at result packet. Cheater?")
            c.CommandResponse("[WARDEN] Pack your bags cheater, you're going!")
            Exit Sub
        End If

        Log.WriteLine(LogType.DEBUG, "[WARDEN] Result bufLen:{0} checkSum:{1:X}", bufLen, checkSum)

        c.WardenData.Scan.HandleResponse(Packet)
    End Sub

    Public Function ControlChecksum(ByVal checkSum As UInteger, ByVal data() As Byte) As Boolean
        Dim sha1 As New SHA1Managed
        Dim hash() As Byte = sha1.ComputeHash(data)
        Dim ints(4) As UInteger
        For i As Integer = 0 To 4
            ints(i) = BitConverter.ToUInt32(hash, i * 4)
        Next

        Dim ourCheckSum As UInteger = ints(0) Xor ints(1) Xor ints(2) Xor ints(3) Xor ints(4)

        Return (checkSum = ourCheckSum)
    End Function
#End Region
#Region "Mediv.MOD"


    Public Class WardenData
        Public Failed As Byte = 0
        Public Ready As Boolean = False
        Public KeyOut() As Byte = Nothing
        Public KeyIn() As Byte = Nothing
        Public Seed() As Byte = Nothing
        Public K() As Byte = Nothing

        Public ClientSeed() As Byte = Nothing
        Public xorByte As Byte = 0

        Public Scan As WardenScan = Nothing

    End Class

    Public Enum MaievResponse As Byte
        MAIEV_RESPONSE_FAILED_OR_MISSING = &H0          'The module was either currupt or not in the cache request transfer
        MAIEV_RESPONSE_SUCCESS = &H1                    'The module was in the cache and loaded successfully
        MAIEV_RESPONSE_RESULT = &H2
        MAIEV_RESPONSE_HASH = &H4
    End Enum
    Public Enum MaievOpcode As Byte
        MAIEV_MODULE_INFORMATION = 0
        MAIEV_MODULE_TRANSFER = 1
        MAIEV_MODULE_RUN = 2
        MAIEV_MODULE_UNK = 3
        MAIEV_MODULE_SEED = 5
    End Enum

    Public Class MaievData
        Public index As Integer = 0
        Public source1 As Byte()
        Public source2 As Byte()
        Public data As Byte() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

        Public Sub New(ByVal seed As Byte())
            'Initialization
            Dim sha1 As New SHA1Managed
            Me.source1 = sha1.ComputeHash(seed, 0, 20)
            Me.source2 = sha1.ComputeHash(seed, 20, 20)
            Me.Update()
        End Sub
        Public Sub Update()
            Dim buffer1(20 * 3 - 1) As Byte
            Dim sha1 As New SHA1Managed
            Buffer.BlockCopy(Me.source1, 0, buffer1, 0, 20)
            Buffer.BlockCopy(Me.data, 0, buffer1, 20, 20)
            Buffer.BlockCopy(Me.source2, 0, buffer1, 40, 20)
            Me.data = sha1.ComputeHash(buffer1)
        End Sub

        Private Function GetByte() As Byte
            Dim r As Byte = Me.data(index)
            Me.index += 1
            If index >= &H14 Then
                Me.Update()
                Me.index = 0
            End If
            Return r
        End Function
        Public Function GetBytes(ByVal count As Integer) As Byte()
            Dim b(count - 1) As Byte
            For i As Integer = 0 To count - 1
                b(i) = Me.GetByte
            Next
            Return b
        End Function

    End Class

#End Region
#Region "RC4"


    Public Class RC4
        'http://www.skullsecurity.org/wiki/index.php/Crypto_and_Hashing

        Public Shared Function Init(ByVal base() As Byte) As Byte()
            Dim val As Integer = 0
            Dim position As Integer = 0
            Dim temp As Byte

            Dim key(255 + 2) As Byte

            For i As Integer = 0 To 256 - 1
                key(i) = i
            Next

            key(256) = 0
            key(257) = 0

            For i As Integer = 1 To 64
                val = val + key((i * 4) - 4) + base(position Mod base.Length)
                val = val And &HFF
                position += 1
                temp = key((i * 4) - 4)
                key((i * 4) - 4) = key(val And &HFF)
                key(val And &HFF) = temp

                val = val + key((i * 4) - 3) + base(position Mod base.Length)
                val = val And &HFF
                position += 1
                temp = key((i * 4) - 3)
                key((i * 4) - 3) = key(val And &HFF)
                key(val And &HFF) = temp

                val = val + key((i * 4) - 2) + base(position Mod base.Length)
                val = val And &HFF
                position += 1
                temp = key((i * 4) - 2)
                key((i * 4) - 2) = key(val And &HFF)
                key(val And &HFF) = temp

                val = val + key((i * 4) - 1) + base(position Mod base.Length)
                val = val And &HFF
                position += 1
                temp = key((i * 4) - 1)
                key((i * 4) - 1) = key(val And &HFF)
                key(val And &HFF) = temp
            Next

            Return key
        End Function
        Public Shared Sub Crypt(ByRef data As Byte(), ByVal key As Byte())
            Dim temp As Byte
            For i As Integer = 0 To data.Length - 1
                key(256) = (CType(key(256), Integer) + 1) And &HFF
                key(257) = (CType(key(257), Integer) + CType(key(key(256)), Integer)) And &HFF

                temp = key(key(257) And &HFF)
                key(key(257)) = key(key(256))
                key(key(256)) = temp

                data(i) = (data(i) Xor key((CType(key(key(257)), Integer) + CType(key(key(256)), Integer)) And &HFF))
            Next
        End Sub
    End Class


#End Region
#Region "WardenSHA1"
    Private Function FixWardenSHA(ByVal hash() As Byte) As Byte()
        For i As Integer = 0 To hash.Length - 1 Step 4
            Dim tmp As Byte = hash(i + 3)
            hash(i + 3) = hash(i)
            hash(i) = tmp

            tmp = hash(i + 2)
            hash(i + 2) = hash(i + 1)
            hash(i + 1) = tmp
        Next
        Return hash
    End Function
#End Region

End Module