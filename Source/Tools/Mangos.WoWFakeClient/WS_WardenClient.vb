'
' Copyright (C) 2013-2021 getMaNGOS <https://getmangos.eu>
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

Imports System.IO
Imports System.Security.Cryptography
Imports System.Runtime.InteropServices
Imports System.Numerics

Public Module WS_WardenClient

#Region "Consts"
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
#End Region

#Region "Warden Init"

    Public Sub InitWarden()
        Dim m As New MaievData(SS_Hash)
        Dim seedOut As Byte() = m.GetBytes(16)
        Dim seedIn As Byte() = m.GetBytes(16)
        Maiev.KeyOut = RC4.Init(seedOut)
        Maiev.KeyIn = RC4.Init(seedIn)
    End Sub

    Public Class MaievData
        Public index As Integer = 0
        Public source1 As Byte()
        Public source2 As Byte()
        Public data As Byte() = New Byte() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

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

#Region "Warden Handlers"
    Public ModuleLength As Integer = 0

    Public Sub On_SMSG_WARDEN_DATA(ByRef Packet As PacketClass)

        'START Warden Decryption
        Dim b(Packet.Data.Length - 4 - 1) As Byte
        Buffer.BlockCopy(Packet.Data, 4, b, 0, b.Length)
        RC4.Crypt(b, Maiev.KeyIn)
        Buffer.BlockCopy(b, 0, Packet.Data, 4, b.Length)
        'END

        Dim WardenData(Packet.Data.Length - 4 - 1) As Byte
        Buffer.BlockCopy(Packet.Data, 4, WardenData, 0, WardenData.Length)

        Dim Opcode As MaievOpcode = Packet.GetInt8

        Console.ForegroundColor = System.ConsoleColor.Cyan
        Console.WriteLine("SMSG_WARDEN_DATA [{0}]", Opcode)
        Console.ForegroundColor = System.ConsoleColor.White

        Select Case Opcode
            Case MaievOpcode.MAIEV_MODULE_INFORMATION
                Dim Name() As Byte = Packet.GetByteArray(16)
                Dim Key() As Byte = Packet.GetByteArray(16)
                Dim Size As UInteger = Packet.GetUInt32()

                Maiev.ModuleName = BitConverter.ToString(Name).Replace("-", "")
                Maiev.ModuleKey = Key
                ModuleLength = Size
                Maiev.ModuleData = New Byte() {}

                If File.Exists("modules\" & Maiev.ModuleName & ".mod") = False Then
                    Console.WriteLine("[{0}][WARDEN] Module is missing.", Format(TimeOfDay, "HH:mm:ss"))

                    Dim response As New PacketClass(OPCODES.CMSG_WARDEN_DATA)
                    response.AddInt8(MaievResponse.MAIEV_RESPONSE_FAILED_OR_MISSING)
                    SendWardenPacket(response)
                    response.Dispose()
                Else
                    Console.WriteLine("[{0}][WARDEN] Module is initiated.", Format(TimeOfDay, "HH:mm:ss"))

                    Maiev.ModuleData = File.ReadAllBytes("modules\" & Maiev.ModuleName & ".mod")
                    If Maiev.LoadModule(Maiev.ModuleName, Maiev.ModuleData, Maiev.ModuleKey) Then
                        Console.WriteLine("[{0}][WARDEN] Successfully loaded the module.", Format(TimeOfDay, "HH:mm:ss"))

                        Dim response As New PacketClass(OPCODES.CMSG_WARDEN_DATA)
                        response.AddInt8(MaievResponse.MAIEV_RESPONSE_SUCCESS)
                        SendWardenPacket(response)
                        response.Dispose()
                    Else
                        Dim response As New PacketClass(OPCODES.CMSG_WARDEN_DATA)
                        response.AddInt8(MaievResponse.MAIEV_RESPONSE_FAILED_OR_MISSING)
                        SendWardenPacket(response)
                        response.Dispose()
                    End If
                End If
            Case MaievOpcode.MAIEV_MODULE_TRANSFER
                Dim Size As UShort = Packet.GetUInt16()
                Dim Data() As Byte = Packet.GetByteArray(Size)

                Maiev.ModuleData = Concat(Maiev.ModuleData, Data)
                ModuleLength -= Size

                If ModuleLength <= 0 Then
                    Console.WriteLine("[{0}][WARDEN] Module is fully transfered.", Format(TimeOfDay, "HH:mm:ss"))

                    If Directory.Exists("modules") = False Then Directory.CreateDirectory("modules")
                    File.WriteAllBytes("modules\" & Maiev.ModuleName & ".mod", Maiev.ModuleData)

                    If Maiev.LoadModule(Maiev.ModuleName, Maiev.ModuleData, Maiev.ModuleKey) Then
                        Console.WriteLine("[{0}][WARDEN] Successfully loaded the module.", Format(TimeOfDay, "HH:mm:ss"))

                        Dim response As New PacketClass(OPCODES.CMSG_WARDEN_DATA)
                        response.AddInt8(MaievResponse.MAIEV_RESPONSE_SUCCESS)
                        SendWardenPacket(response)
                        response.Dispose()
                    End If
                Else
                    Console.WriteLine("[{0}][WARDEN] Module transfer. Bytes left: {1}", Format(TimeOfDay, "HH:mm:ss"), ModuleLength)
                End If

            Case MaievOpcode.MAIEV_MODULE_RUN
                Console.WriteLine("[{0}][WARDEN] Requesting a scan.", Format(TimeOfDay, "HH:mm:ss"))

                'TODO: Encrypt?
                Maiev.ReadKeys2()
                RC4.Crypt(WardenData, Maiev.ModKeyIn)

                Dim HandledBytes As Integer = Maiev.HandlePacket(WardenData)
                If HandledBytes <= 0 Then Exit Sub
                Dim thePacket() As Byte = Maiev.ReadPacket()
                If thePacket.Length = 0 Then Exit Sub

                RC4.Crypt(WardenData, Maiev.ModKeyOut)

                'TODO: Decrypt?

                DumpPacket(thePacket)
                Dim response As New PacketClass(OPCODES.CMSG_WARDEN_DATA)
                response.AddByteArray(thePacket)
                SendWardenPacket(response)
                response.Dispose()

            Case MaievOpcode.MAIEV_MODULE_UNK
                'TODO: Encrypt?
                Maiev.ReadKeys2()
                RC4.Crypt(WardenData, Maiev.ModKeyIn)

                Dim HandledBytes As Integer = Maiev.HandlePacket(WardenData)
                If HandledBytes <= 0 Then Exit Sub
                Dim thePacket() As Byte = Maiev.ReadPacket()
                If thePacket.Length = 0 Then Exit Sub

                RC4.Crypt(WardenData, Maiev.ModKeyOut)
                'TODO: Decrypt?

                DumpPacket(thePacket)
                Dim response As New PacketClass(OPCODES.CMSG_WARDEN_DATA)
                response.AddByteArray(thePacket)
                SendWardenPacket(response)
                response.Dispose()

            Case MaievOpcode.MAIEV_MODULE_SEED
                Maiev.GenerateNewRC4Keys(SS_Hash)
                Dim HandledBytes As Integer = Maiev.HandlePacket(WardenData)
                If HandledBytes <= 0 Then Exit Sub
                Dim thePacket() As Byte = Maiev.ReadPacket()

                Maiev.ModKeyIn = New Byte(258 - 1) {}
                Maiev.ModKeyOut = New Byte(258 - 1) {}

                Dim response As New PacketClass(OPCODES.CMSG_WARDEN_DATA)
                response.AddByteArray(thePacket)
                SendWardenPacket(response)
                response.Dispose()

                Maiev.ReadKeys()
            Case Else
                Console.WriteLine("[{0}][WARDEN] Unhandled Opcode [{1}] 0x{2:X}", Format(TimeOfDay, "HH:mm:ss"), Opcode, CType(Opcode, Integer))
        End Select
    End Sub

    Public Sub SendWardenPacket(ByRef Packet As PacketClass)
        'START Warden Encryption
        Dim b(Packet.Data.Length - 6 - 1) As Byte
        Buffer.BlockCopy(Packet.Data, 6, b, 0, b.Length)
        RC4.Crypt(b, Maiev.KeyOut)
        Buffer.BlockCopy(b, 0, Packet.Data, 6, b.Length)
        'END

        Send(Packet)
    End Sub
#End Region

#Region "Maiev"
    Public Maiev As New WardenMaiev

    Public Class WardenMaiev
        Public WardenModule As Byte() = {}
        Public ModuleName As String = ""
        Public ModuleKey As Byte() = {}
        Public ModuleData() As Byte = {}

        Public KeyIn() As Byte = {}
        Public KeyOut() As Byte = {}

        Public ModKeyIn() As Byte = {}
        Public ModKeyOut() As Byte = {}

#Region "Load Module"
        Public Function LoadModule(ByVal Name As String, ByRef Data() As Byte, ByVal Key() As Byte) As Boolean
            Key = RC4.Init(Key)
            RC4.Crypt(Data, Key)

            Dim UncompressedLen As Integer = BitConverter.ToInt32(Data, 0)
            If UncompressedLen < 0 Then
                Console.WriteLine("[WARDEN] Failed to decrypt {0}, incorrect length.", Name)
                Return False
            End If

            Dim CompressedData(Data.Length - &H108 - 1) As Byte
            Array.Copy(Data, 4, CompressedData, 0, CompressedData.Length)
            Dim dataPos As Integer = 4 + CompressedData.Length
            Dim Sign As String = Chr(Data(dataPos + 3)) & Chr(Data(dataPos + 2)) & Chr(Data(dataPos + 1)) & Chr(Data(dataPos))
            If Sign <> "SIGN" Then
                Console.WriteLine("[WARDEN] Failed to decrypt {0}, sign missing.", Name)
                Return False
            End If
            dataPos += 4
            Dim Signature(&H100 - 1) As Byte
            Array.Copy(Data, dataPos, Signature, 0, Signature.Length)

            'Check signature
            If CheckSignature(Signature, Data, Data.Length - &H104) = False Then
                Console.WriteLine("[WARDEN] Signature fail on Warden Module.")
                Return False
            End If

            Dim DecompressedData() As Byte = DeCompress(CompressedData)

            Dim ms As New MemoryStream(DecompressedData)
            Dim br As New BinaryReader(ms)
            ModuleData = PrepairModule(br)
            ms.Close()
            ms.Dispose()
            br = Nothing

            Console.WriteLine("[WARDEN] Successfully prepaired Warden Module.")

            If Not InitModule(ModuleData) Then Return False

            Return True
        End Function

        Public Function CheckSignature(ByVal Signature() As Byte, ByVal Data() As Byte, ByVal DataLen As Integer) As Boolean
            Dim power As New BigInteger(New Byte() {&H1, &H0, &H1, &H0}, True)
            Dim pmod As New BigInteger(New Byte() {&H6B, &HCE, &HF5, &H2D, &H2A, &H7D, &H7A, &H67, &H21, &H21, &H84, &HC9, &HBC, &H25, &HC7, &HBC, &HDF, &H3D, &H8F, &HD9, &H47, &HBC, &H45, &H48, &H8B, &H22, &H85, &H3B, &HC5, &HC1, &HF4, &HF5, &H3C, &HC, &H49, &HBB, &H56, &HE0, &H3D, &HBC, &HA2, &HD2, &H35, &HC1, &HF0, &H74, &H2E, &H15, &H5A, &H6, &H8A, &H68, &H1, &H9E, &H60, &H17, &H70, &H8B, &HBD, &HF8, &HD5, &HF9, &H3A, &HD3, &H25, &HB2, &H66, &H92, &HBA, &H43, &H8A, &H81, &H52, &HF, &H64, &H98, &HFF, &H60, &H37, &HAF, &HB4, &H11, &H8C, &HF9, &H2E, &HC5, &HEE, &HCA, &HB4, &H41, &H60, &H3C, &H7D, &H2, &HAF, &HA1, &H2B, &H9B, &H22, &H4B, &H3B, &HFC, &HD2, &H5D, &H73, &HE9, &H29, &H34, &H91, &H85, &H93, &H4C, &HBE, &HBE, &H73, &HA9, &HD2, &H3B, &H27, &H7A, &H47, &H76, &HEC, &HB0, &H28, &HC9, &HC1, &HDA, &HEE, &HAA, &HB3, &H96, &H9C, &H1E, &HF5, &H6B, &HF6, &H64, &HD8, &H94, &H2E, &HF1, &HF7, &H14, &H5F, &HA0, &HF1, &HA3, &HB9, &HB1, &HAA, &H58, &H97, &HDC, &H9, &H17, &HC, &H4, &HD3, &H8E, &H2, &H2C, &H83, &H8A, &HD6, &HAF, &H7C, &HFE, &H83, &H33, &HC6, &HA8, &HC3, &H84, &HEF, &H29, &H6, &HA9, &HB7, &H2D, &H6, &HB, &HD, &H6F, &H70, &H9E, &H34, &HA6, &HC7, &H31, &HBE, &H56, &HDE, &HDD, &H2, &H92, &HF8, &HA0, &H58, &HB, &HFC, &HFA, &HBA, &H49, &HB4, &H48, &HDB, &HEC, &H25, &HF3, &H18, &H8F, &H2D, &HB3, &HC0, &HB8, &HDD, &HBC, &HD6, &HAA, &HA6, &HDB, &H6F, &H7D, &H7D, &H25, &HA6, &HCD, &H39, &H6D, &HDA, &H76, &HC, &H79, &HBF, &H48, &H25, &HFC, &H2D, &HC5, &HFA, &H53, &H9B, &H4D, &H60, &HF4, &HEF, &HC7, &HEA, &HAC, &HA1, &H7B, &H3, &HF4, &HAF, &HC7}, True)
            Dim sig As New BigInteger(Signature, True)
            Dim res As BigInteger = BigInteger.ModPow(sig, power, pmod)
            Dim result() As Byte = res.ToByteArray(True)

            Dim digest() As Byte
            Dim properResult() As Byte = New Byte(&H100 - 1) {}

            For i As Integer = 0 To properResult.Length - 1
                properResult(i) = &HBB
            Next
            properResult(&H100 - 1) = &HB

            Dim tmpKey As String = "MAIEV.MOD"
            Dim bKey(tmpKey.Length - 1) As Byte
            For i As Integer = 0 To tmpKey.Length - 1
                bKey(i) = Asc(tmpKey(i))
            Next

            Dim newData(DataLen + bKey.Length - 1) As Byte
            Array.Copy(Data, 0, newData, 0, DataLen)
            Array.Copy(bKey, 0, newData, DataLen, bKey.Length)

            Dim sha1 As New SHA1Managed
            digest = sha1.ComputeHash(newData)
            Array.Copy(digest, 0, properResult, 0, digest.Length)

            For i As Integer = 0 To result.Length - 1
                If result(i) <> properResult(i) Then Return False
            Next

            Return True
        End Function
#End Region

#Region "Prepare Module"
        Private Declare Function LoadLibrary Lib "kernel32.dll" Alias "LoadLibraryA" (ByVal lpLibFileName As String) As Integer
        Private Declare Function GetProcAddress Lib "kernel32.dll" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer
        Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As Integer) As Integer

        Private Function PrepairModule(ByRef br As BinaryReader) As Byte()
            Dim length As Integer = br.ReadInt32()
            m_Mod = malloc(length)

            Dim bModule(length - 1) As Byte
            Dim ms As New MemoryStream(bModule)
            Dim bw As New BinaryWriter(ms)
            Dim br2 As New BinaryReader(ms)

            'Console.WriteLine("Allocated {0} (0x{1:X}) bytes for new module.", length, length)

            'Copy 40 bytes from the original module to the new one.
            br.BaseStream.Position = 0
            Dim tmpBytes() As Byte = br.ReadBytes(40)
            bw.Write(tmpBytes, 0, tmpBytes.Length)

            br2.BaseStream.Position = &H24
            Dim source_location As Integer = &H28 + (br2.ReadInt32() * 12)
            br.BaseStream.Position = &H28
            Dim destination_location As Integer = br.ReadInt32()
            br.BaseStream.Position = &H0
            Dim limit As Integer = br.ReadInt32()

            Dim skip As Boolean = False

            'Console.WriteLine("Copying code sections to module.")
            Do While destination_location < limit
                br.BaseStream.Position = source_location
                Dim count As Integer = br.ReadInt16() '(CInt(br.ReadByte()) << 0) Or (CInt(br.ReadByte()) << 8)

                source_location += 2

                If Not skip Then
                    br.BaseStream.Position = source_location
                    tmpBytes = br.ReadBytes(count)
                    bw.BaseStream.Position = destination_location
                    bw.Write(tmpBytes, 0, tmpBytes.Length)
                    source_location += count
                End If

                skip = Not skip
                destination_location += count
            Loop

            'Console.WriteLine("Adjusting references to global variables...")
            br.BaseStream.Position = 8
            source_location = br.ReadInt32()
            destination_location = 0
            Dim counter As Integer = 0
            br2.BaseStream.Position = &HC
            Dim CountTo As Integer = br2.ReadInt32()
            Do While counter < CountTo
                br2.BaseStream.Position = source_location
                Dim tmpByte1 As Byte = br2.ReadByte()
                Dim tmpByte2 As Byte = br2.ReadByte()
                destination_location += CInt(tmpByte2) Or (CInt(tmpByte1) << 8)

                source_location += 2

                br2.BaseStream.Position = destination_location
                Dim address As Integer = br2.ReadInt32() + m_Mod

                bw.BaseStream.Position = destination_location
                bw.Write(address)

                counter += 1
            Loop

            'Console.WriteLine("Updating API library references...")
            br2.BaseStream.Position = &H20
            limit = br2.ReadInt32()
            Dim library As String

            For counter = 0 To limit - 1
                br2.BaseStream.Position = &H1C
                Dim proc_start As Integer = br2.ReadInt32() + (counter * 8)

                br2.BaseStream.Position = proc_start
                library = getNTString(br2, br2.ReadInt32())
                'Console.WriteLine("  Library: {0}", library)

                Dim hModule As Integer = LoadLibrary(library)

                br2.BaseStream.Position = proc_start + 4
                Dim proc_offset As Integer = br2.ReadInt32()

                br2.BaseStream.Position = proc_offset
                Dim proc As Integer = br2.ReadInt32()
                Do While proc <> 0

                    If proc > 0 Then
                        Dim strProc As String = getNTString(br2, proc)
                        Dim addr As Integer = GetProcAddress(hModule, strProc)

                        'Console.WriteLine("    Function: {0} (0x{1:X})", strProc, addr)
                        bw.BaseStream.Position = proc_offset
                        bw.Write(addr)
                    Else
                        proc = (proc And &H7FFFFFFF)
                        'Console.WriteLine("Proc: ord(0x{0:X})", proc)
                    End If

                    proc_offset += 4
                    br2.BaseStream.Position = proc_offset
                    proc = br2.ReadInt32()
                Loop

                CloseHandle(hModule)
            Next

            Return ms.ToArray
        End Function

        Private Function getNTString(ByRef br As BinaryReader, ByVal pos As Integer) As String
            br.BaseStream.Position = pos

            Dim i As Integer = 0
            Dim tmpByte As Byte
            Dim tmpStr As String = ""
            Do
                tmpByte = br.ReadByte()
                If tmpByte = 0 Then Return tmpStr
                tmpStr &= Chr(tmpByte)

                i += 1
            Loop
        End Function
#End Region

#Region "Init Module"
        Public Delegate Sub SendPacketDelegate(ByVal ptrPacket As Integer, ByVal dwSize As Integer)
        Public Delegate Function CheckModuleDelegate(ByVal ptrMod As Integer, ByVal ptrKey As Integer) As Integer
        Public Delegate Function ModuleLoadDelegate(ByVal ptrRC4Key As Integer, ByVal pModule As Integer, ByVal dwModSize As Integer) As Integer
        Public Delegate Function AllocateMemDelegate(ByVal dwSize As Integer) As Integer
        Public Delegate Sub FreeMemoryDelegate(ByVal dwMemory As Integer)
        Public Delegate Function SetRC4DataDelegate(ByVal lpKeys As Integer, ByVal dwSize As Integer) As Integer
        Public Delegate Function GetRC4DataDelegate(ByVal lpBuffer As Integer, ByRef dwSize As Integer) As Integer

        <UnmanagedFunctionPointer(CallingConvention.ThisCall)>
        Public Delegate Function InitializeModule(ByVal lpPtr2Table As Integer) As Integer

        <UnmanagedFunctionPointer(CallingConvention.ThisCall)>
        Public Delegate Sub GenerateRC4KeysDelegate(ByVal ppFncList As Integer, ByVal lpData As Integer, ByVal dwSize As Integer)
        <UnmanagedFunctionPointer(CallingConvention.ThisCall)>
        Public Delegate Sub UnloadModuleDelegate(ByVal ppFncList As Integer)
        <UnmanagedFunctionPointer(CallingConvention.ThisCall)>
        Public Delegate Sub PacketHandlerDelegate(ByVal ppFncList As Integer, ByVal pPacket As Integer, ByVal dwSize As Integer, ByVal dwBuffer As Integer)
        <UnmanagedFunctionPointer(CallingConvention.ThisCall)>
        Public Delegate Sub TickDelegate(ByVal ppFncList As Integer, ByVal dwTick As Integer)

        Private SendPacketD As SendPacketDelegate = Nothing
        Private CheckModuleD As CheckModuleDelegate = Nothing
        Private ModuleLoadD As ModuleLoadDelegate = Nothing
        Private AllocateMemD As AllocateMemDelegate = Nothing
        Private FreeMemoryD As FreeMemoryDelegate = Nothing
        Private SetRC4DataD As SetRC4DataDelegate = Nothing
        Private GetRC4DataD As GetRC4DataDelegate = Nothing

        Private GenerateRC4Keys As GenerateRC4KeysDelegate = Nothing
        Private UnloadModule As UnloadModuleDelegate = Nothing
        Private PacketHandler As PacketHandlerDelegate = Nothing
        Private Tick As TickDelegate = Nothing

        Private m_Mod As Integer = 0
        Private m_ModMem As Integer = 0

        Private InitPointer As Integer = 0
        Private init As InitializeModule = Nothing
        Private myFuncList As IntPtr = IntPtr.Zero
        Private myFunctionList As FuncList = Nothing
        Private pFuncList As Integer = 0
        Private ppFuncList As Integer = 0
        Private myWardenList As WardenFuncList = Nothing
        Private pWardenList As Integer = 0

        Private gchSendPacket As GCHandle
        Private gchCheckModule As GCHandle
        Private gchModuleLoad As GCHandle
        Private gchAllocateMem As GCHandle
        Private gchReleaseMem As GCHandle
        Private gchSetRC4Data As GCHandle
        Private gchGetRC4Data As GCHandle

        Private Function InitModule(ByRef Data() As Byte) As Boolean
            Dim A As Integer
            Dim B As Integer
            Dim C As Integer
            Dim bCode(15) As Byte

            Dim ms As New MemoryStream(Data)
            Dim br As New BinaryReader(ms)

            Marshal.Copy(Data, 0, New IntPtr(m_Mod), Data.Length)

            br.BaseStream.Position = &H18
            C = br.ReadInt32()
            B = 1 - C
            br.BaseStream.Position = &H14
            If B > br.ReadInt32() Then Return False
            br.BaseStream.Position = &H10
            A = br.ReadInt32()
            br.BaseStream.Position = A + (B * 4)
            A = br.ReadInt32() + m_Mod
            InitPointer = A
            Console.WriteLine("Initialize Function is mapped at 0x{0:X}", InitPointer)

            SendPacketD = CType(AddressOf SendPacket, SendPacketDelegate)
            CheckModuleD = CType(AddressOf CheckModule, CheckModuleDelegate)
            ModuleLoadD = CType(AddressOf ModuleLoad, ModuleLoadDelegate)
            AllocateMemD = CType(AddressOf AllocateMem, AllocateMemDelegate)
            FreeMemoryD = CType(AddressOf FreeMemory, FreeMemoryDelegate)
            SetRC4DataD = CType(AddressOf SetRC4Data, SetRC4DataDelegate)
            GetRC4DataD = CType(AddressOf GetRC4Data, GetRC4DataDelegate)

            myFunctionList = New FuncList
            myFunctionList.fpSendPacket = Marshal.GetFunctionPointerForDelegate(SendPacketD).ToInt32()
            myFunctionList.fpCheckModule = Marshal.GetFunctionPointerForDelegate(CheckModuleD).ToInt32()
            myFunctionList.fpLoadModule = Marshal.GetFunctionPointerForDelegate(ModuleLoadD).ToInt32()
            myFunctionList.fpAllocateMemory = Marshal.GetFunctionPointerForDelegate(AllocateMemD).ToInt32()
            myFunctionList.fpReleaseMemory = Marshal.GetFunctionPointerForDelegate(FreeMemoryD).ToInt32()
            myFunctionList.fpSetRC4Data = Marshal.GetFunctionPointerForDelegate(SetRC4DataD).ToInt32()
            myFunctionList.fpGetRC4Data = Marshal.GetFunctionPointerForDelegate(GetRC4DataD).ToInt32()

            Console.WriteLine("Imports: ")
            Console.WriteLine("  SendPacket: 0x{0:X}", myFunctionList.fpSendPacket)
            Console.WriteLine("  CheckModule: 0x{0:X}", myFunctionList.fpCheckModule)
            Console.WriteLine("  LoadModule: 0x{0:X}", myFunctionList.fpLoadModule)
            Console.WriteLine("  AllocateMemory: 0x{0:X}", myFunctionList.fpAllocateMemory)
            Console.WriteLine("  ReleaseMemory: 0x{0:X}", myFunctionList.fpReleaseMemory)
            Console.WriteLine("  SetRC4Data: 0x{0:X}", myFunctionList.fpSetRC4Data)
            Console.WriteLine("  GetRC4Data: 0x{0:X}", myFunctionList.fpGetRC4Data)

            'http://forum.valhallalegends.com/index.php?topic=17758.0
            myFuncList = New IntPtr(malloc(&H1C))
            Marshal.StructureToPtr(myFunctionList, myFuncList, False)
            pFuncList = myFuncList.ToInt32()
            ppFuncList = VarPtr(pFuncList)

            Console.WriteLine("Initializing module")
            init = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(InitPointer), GetType(InitializeModule)), InitializeModule)
            m_ModMem = init.Invoke(ppFuncList)

            pWardenList = Marshal.ReadInt32(New IntPtr(m_ModMem))
            myWardenList = Marshal.PtrToStructure(New IntPtr(pWardenList), GetType(WardenFuncList))

            Console.WriteLine("Exports:")
            Console.WriteLine("  GenerateRC4Keys: 0x{0:X}", myWardenList.fpGenerateRC4Keys)
            Console.WriteLine("  Unload: 0x{0:X}", myWardenList.fpUnload)
            Console.WriteLine("  PacketHandler: 0x{0:X}", myWardenList.fpPacketHandler)
            Console.WriteLine("  Tick: 0x{0:X}", myWardenList.fpTick)

            GenerateRC4Keys = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(myWardenList.fpGenerateRC4Keys), GetType(GenerateRC4KeysDelegate)), GenerateRC4KeysDelegate)
            UnloadModule = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(myWardenList.fpUnload), GetType(UnloadModuleDelegate)), UnloadModuleDelegate)
            PacketHandler = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(myWardenList.fpPacketHandler), GetType(PacketHandlerDelegate)), PacketHandlerDelegate)
            Tick = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(myWardenList.fpTick), GetType(TickDelegate)), TickDelegate)

            ms.Close()
            ms.Dispose()
            ms = Nothing
            br = Nothing

            Return True
        End Function

#End Region

#Region "Unload Module"
        Private Sub Unload_Module()
            free(m_Mod)
        End Sub
#End Region

#Region "Module Handlers"

        <StructLayout(LayoutKind.Explicit, Size:=&H1C)>
        Private Structure FuncList
            <FieldOffset(&H0)>
            Public fpSendPacket As Integer
            <FieldOffset(&H4)>
            Public fpCheckModule As Integer
            <FieldOffset(&H8)>
            Public fpLoadModule As Integer
            <FieldOffset(&HC)>
            Public fpAllocateMemory As Integer
            <FieldOffset(&H10)>
            Public fpReleaseMemory As Integer
            <FieldOffset(&H14)>
            Public fpSetRC4Data As Integer
            <FieldOffset(&H18)>
            Public fpGetRC4Data As Integer
        End Structure

        <StructLayout(LayoutKind.Explicit, Size:=&H10)>
        Private Structure WardenFuncList
            <FieldOffset(&H0)>
            Public fpGenerateRC4Keys As Integer
            <FieldOffset(&H4)>
            Public fpUnload As Integer
            <FieldOffset(&H8)>
            Public fpPacketHandler As Integer
            <FieldOffset(&HC)>
            Public fpTick As Integer
        End Structure

        Private m_RC4 As Integer = 0
        Private m_PKT() As Byte = {}

        Private Sub SendPacket(ByVal ptrPacket As Integer, ByVal dwSize As Integer)
            If (dwSize < 1) Then Exit Sub
            If (dwSize > 5000) Then Exit Sub

            m_PKT = New Byte(dwSize - 1) {}
            Marshal.Copy(New IntPtr(ptrPacket), m_PKT, 0, dwSize)

            Console.WriteLine("Warden.SendPacket() ptrPacket={0}, size={1}", ptrPacket, dwSize)
        End Sub
        Private Function CheckModule(ByVal ptrMod As Integer, ByVal ptrKey As Integer) As Integer
            'CheckModule = 0 '//Need to download
            'CheckModule = 1 '//Don't need to download
            Console.WriteLine("Warden.CheckModule() Mod={0}, size={1}", ptrMod, ptrKey)
            Return 1
        End Function
        Private Function ModuleLoad(ByVal ptrRC4Key As Integer, ByVal pModule As Integer, ByVal dwModSize As Integer) As Integer
            'ModuleLoad = 0 '//Need to download
            'ModuleLoad = 1 '//Don't need to download
            Console.WriteLine("Warden.ModuleLoad() Key={0}, Mod={1}, size={2}", ptrRC4Key, pModule, dwModSize)
            Return 1
        End Function
        Private Function AllocateMem(ByVal dwSize As Integer) As Integer
            Console.WriteLine("Warden.AllocateMem() Size={0}", dwSize)
            Return malloc(dwSize)
        End Function
        Private Sub FreeMemory(ByVal dwMemory As Integer)
            Console.WriteLine("Warden.FreeMemory() Memory={0}", dwMemory)
            free(dwMemory)
        End Sub
        Private Function SetRC4Data(ByVal lpKeys As Integer, ByVal dwSize As Integer) As Integer
            Console.WriteLine("Warden.SetRC4Data() Keys={0}, Size={1}", lpKeys, dwSize)
            Return 1
        End Function
        Private Function GetRC4Data(ByVal lpBuffer As Integer, ByRef dwSize As Integer) As Integer
            Console.WriteLine("Warden.GetRC4Data() Buffer={0}, Size={1}", lpBuffer, dwSize)
            'GetRC4Data = 1 'got the keys already
            'GetRC4Data = 0 'generate new keys

            For i As Integer = 0 To dwSize - 1 'Clear the keys
                Marshal.WriteByte(New IntPtr(lpBuffer + i), 0)
            Next

            m_RC4 = lpBuffer
            Return 1
        End Function

        Public Sub GenerateNewRC4Keys(ByVal K() As Byte)
            m_RC4 = 0
            GenerateRC4Keys(m_ModMem, ByteArrPtr(K), K.Length)
        End Sub

        Public Function HandlePacket(ByVal PacketData() As Byte) As Integer
            m_PKT = New Byte() {}
            Dim BytesRead As Integer = 0
            BytesRead = VarPtr(BytesRead)
            PacketHandler(m_ModMem, ByteArrPtr(PacketData), PacketData.Length, BytesRead)
            Return Marshal.ReadInt32(New IntPtr(BytesRead))
        End Function

        Public Function ReadPacket() As Byte()
            Return m_PKT
        End Function

        Public Sub ReadKeys()
            Dim KeyData(&H204 - 1) As Byte
            Marshal.Copy(New IntPtr(m_ModMem + 32), KeyData, 0, KeyData.Length)
            Buffer.BlockCopy(KeyData, 0, KeyOut, 0, 258)
            Buffer.BlockCopy(KeyData, 258, KeyIn, 0, 258)
        End Sub

        Public Sub ReadKeys2()
            Dim KeyData(&H204 - 1) As Byte
            Marshal.Copy(New IntPtr(m_ModMem + 32), KeyData, 0, KeyData.Length)
            Buffer.BlockCopy(KeyData, 0, ModKeyOut, 0, 258)
            Buffer.BlockCopy(KeyData, 258, ModKeyIn, 0, 258)
        End Sub
#End Region

    End Class
#End Region

#Region "Other functions"
    Private Declare Function GlobalLock Lib "kernel32.dll" (ByVal hMem As Integer) As Integer
    Private Declare Function GlobalUnlock Lib "kernel32.dll" (ByVal hMem As Integer) As Integer

    Private Function VarPtr(ByRef obj As Object) As Integer
        Dim gc As GCHandle = GCHandle.Alloc(obj, GCHandleType.Pinned)
        Return gc.AddrOfPinnedObject.ToInt32
    End Function

    Private Function ByteArrPtr(ByRef arr() As Byte) As Integer
        Dim pData As Integer = malloc(arr.Length)
        Marshal.Copy(arr, 0, New IntPtr(pData), arr.Length)
        Return pData
    End Function

    Private Function malloc(ByVal length As Integer) As Integer
        Dim tmpHandle As Integer = Marshal.AllocHGlobal(length + 4).ToInt32()
        Dim lockedHandle As Integer = GlobalLock(tmpHandle) + 4
        Marshal.WriteInt32(New IntPtr(lockedHandle - 4), tmpHandle)
        Return lockedHandle
    End Function

    Private Sub free(ByVal ptr As Integer)
        Dim tmpHandle As Integer = Marshal.ReadInt32(New IntPtr(ptr - 4))
        GlobalUnlock(tmpHandle)
        Marshal.FreeHGlobal(New IntPtr(tmpHandle))
    End Sub
#End Region

End Module
