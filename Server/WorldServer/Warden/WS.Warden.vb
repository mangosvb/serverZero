'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getmangos.eu>
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

Imports System.IO
Imports System.Security.Cryptography
Imports System.Runtime.InteropServices
Imports mangosVB.Common.NativeMethods

Public Module WS_Warden

#Region "Maiev"
    Public Maiev As New WardenMaiev

    Public Class WardenMaiev
        Implements IDisposable

        Public WardenModule As Byte() = {}
        Public ModuleName As String = ""
        Public ModuleKey As Byte() = {}
        Public ModuleData() As Byte = {}
        Public ModuleSize As Integer = 0

        Public CheckIDs(7) As Byte

        Public Script As ScriptedObject = Nothing

#Region "Init Warden"
        Public Sub InitWarden()
            'ModuleName = "ED4272452F70779CC12079C155812E0A"
            ModuleName = "5F947B8AB100C93D206D1FC3EA1FE6DD"

            Script = New ScriptedObject(String.Format("warden\{0}.vb", ModuleName), String.Format("warden\{0}.dll", ModuleName), False)

            WardenModule = Script.InvokeField("Main", "MD5")
            ModuleKey = Script.InvokeField("Main", "RC4")
            ModuleSize = Script.InvokeField("Main", "Size")

            CheckIDs(0) = Script.InvokeField("Main", "MEM_CHECK")
            CheckIDs(1) = Script.InvokeField("Main", "PAGE_CHECK_A")
            CheckIDs(2) = Script.InvokeField("Main", "MPQ_CHECK")
            CheckIDs(3) = Script.InvokeField("Main", "LUA_STR_CHECK")
            CheckIDs(4) = Script.InvokeField("Main", "DRIVER_CHECK")
            CheckIDs(5) = Script.InvokeField("Main", "TIMING_CHECK")
            CheckIDs(6) = Script.InvokeField("Main", "PROC_CHECK")
            CheckIDs(7) = Script.InvokeField("Main", "MODULE_CHECK")

            ModuleData = File.ReadAllBytes("warden\" & ModuleName & ".bin")
            If LoadModule(ModuleName, ModuleData, ModuleKey) Then
                Log.WriteLine(LogType.SUCCESS, "[WARDEN] Load of module, success [{0}]", ModuleName)
            Else
                Log.WriteLine(LogType.CRITICAL, "[WARDEN] Failed to load module [{0}]", ModuleName)
            End If
        End Sub
#End Region

#Region "Load Module"
        Public Function LoadModule(ByVal Name As String, ByRef Data() As Byte, ByVal Key() As Byte) As Boolean
            Key = RC4.Init(Key)
            RC4.Crypt(Data, Key)

            Dim UncompressedLen As Integer = BitConverter.ToInt32(Data, 0)
            If UncompressedLen < 0 Then
                Log.WriteLine(LogType.CRITICAL, "[WARDEN] Failed to decrypt {0}, incorrect length.", Name)
                Return False
            End If

            Dim CompressedData(Data.Length - &H108 - 1) As Byte
            Array.Copy(Data, 4, CompressedData, 0, CompressedData.Length)
            Dim dataPos As Integer = 4 + CompressedData.Length
            Dim Sign As String = Chr(Data(dataPos + 3)) & Chr(Data(dataPos + 2)) & Chr(Data(dataPos + 1)) & Chr(Data(dataPos))
            If Sign <> "SIGN" Then
                Log.WriteLine(LogType.CRITICAL, "[WARDEN] Failed to decrypt {0}, sign missing.", Name)
                Return False
            End If
            dataPos += 4
            Dim Signature(&H100 - 1) As Byte
            Array.Copy(Data, dataPos, Signature, 0, Signature.Length)
            dataPos += &H100

            'Check signature
            If CheckSignature(Signature, Data, Data.Length - &H104) = False Then
                Log.WriteLine(LogType.CRITICAL, "[WARDEN] Signature fail on Warden Module.")
                Return False
            End If

            Dim DecompressedData() As Byte = DeCompress(CompressedData)
            CompressedData = Nothing

            If Not PrepairModule(DecompressedData) Then Return False

            Log.WriteLine(LogType.SUCCESS, "[WARDEN] Successfully prepaired Warden Module.")

            Try
                If Not InitModule() Then Return False
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "[WARDEN] InitModule Failed.")
            End Try

            Return True
        End Function

        Public Function CheckSignature(ByVal Signature() As Byte, ByVal Data() As Byte, ByVal DataLen As Integer) As Boolean
            Dim power As New Emil.GMP.BigInt(New Byte() {&H1, &H0, &H1, &H0})
            Dim pmod As New Emil.GMP.BigInt(New Byte() {&H6B, &HCE, &HF5, &H2D, &H2A, &H7D, &H7A, &H67, &H21, &H21, &H84, &HC9, &HBC, &H25, &HC7, &HBC, &HDF, &H3D, &H8F, &HD9, &H47, &HBC, &H45, &H48, &H8B, &H22, &H85, &H3B, &HC5, &HC1, &HF4, &HF5, &H3C, &HC, &H49, &HBB, &H56, &HE0, &H3D, &HBC, &HA2, &HD2, &H35, &HC1, &HF0, &H74, &H2E, &H15, &H5A, &H6, &H8A, &H68, &H1, &H9E, &H60, &H17, &H70, &H8B, &HBD, &HF8, &HD5, &HF9, &H3A, &HD3, &H25, &HB2, &H66, &H92, &HBA, &H43, &H8A, &H81, &H52, &HF, &H64, &H98, &HFF, &H60, &H37, &HAF, &HB4, &H11, &H8C, &HF9, &H2E, &HC5, &HEE, &HCA, &HB4, &H41, &H60, &H3C, &H7D, &H2, &HAF, &HA1, &H2B, &H9B, &H22, &H4B, &H3B, &HFC, &HD2, &H5D, &H73, &HE9, &H29, &H34, &H91, &H85, &H93, &H4C, &HBE, &HBE, &H73, &HA9, &HD2, &H3B, &H27, &H7A, &H47, &H76, &HEC, &HB0, &H28, &HC9, &HC1, &HDA, &HEE, &HAA, &HB3, &H96, &H9C, &H1E, &HF5, &H6B, &HF6, &H64, &HD8, &H94, &H2E, &HF1, &HF7, &H14, &H5F, &HA0, &HF1, &HA3, &HB9, &HB1, &HAA, &H58, &H97, &HDC, &H9, &H17, &HC, &H4, &HD3, &H8E, &H2, &H2C, &H83, &H8A, &HD6, &HAF, &H7C, &HFE, &H83, &H33, &HC6, &HA8, &HC3, &H84, &HEF, &H29, &H6, &HA9, &HB7, &H2D, &H6, &HB, &HD, &H6F, &H70, &H9E, &H34, &HA6, &HC7, &H31, &HBE, &H56, &HDE, &HDD, &H2, &H92, &HF8, &HA0, &H58, &HB, &HFC, &HFA, &HBA, &H49, &HB4, &H48, &HDB, &HEC, &H25, &HF3, &H18, &H8F, &H2D, &HB3, &HC0, &HB8, &HDD, &HBC, &HD6, &HAA, &HA6, &HDB, &H6F, &H7D, &H7D, &H25, &HA6, &HCD, &H39, &H6D, &HDA, &H76, &HC, &H79, &HBF, &H48, &H25, &HFC, &H2D, &HC5, &HFA, &H53, &H9B, &H4D, &H60, &HF4, &HEF, &HC7, &HEA, &HAC, &HA1, &H7B, &H3, &HF4, &HAF, &HC7})
            Dim sig As New Emil.GMP.BigInt(Signature)
            Dim res As Emil.GMP.BigInt = sig.PowerMod(power, pmod)
            Dim result() As Byte = res.ToByteArray()

            'Dim power As New BigInteger(New Byte() {&H1, &H0, &H1, &H0})
            'Dim pmod As New BigInteger(New Byte() {&H6B, &HCE, &HF5, &H2D, &H2A, &H7D, &H7A, &H67, &H21, &H21, &H84, &HC9, &HBC, &H25, &HC7, &HBC, &HDF, &H3D, &H8F, &HD9, &H47, &HBC, &H45, &H48, &H8B, &H22, &H85, &H3B, &HC5, &HC1, &HF4, &HF5, &H3C, &HC, &H49, &HBB, &H56, &HE0, &H3D, &HBC, &HA2, &HD2, &H35, &HC1, &HF0, &H74, &H2E, &H15, &H5A, &H6, &H8A, &H68, &H1, &H9E, &H60, &H17, &H70, &H8B, &HBD, &HF8, &HD5, &HF9, &H3A, &HD3, &H25, &HB2, &H66, &H92, &HBA, &H43, &H8A, &H81, &H52, &HF, &H64, &H98, &HFF, &H60, &H37, &HAF, &HB4, &H11, &H8C, &HF9, &H2E, &HC5, &HEE, &HCA, &HB4, &H41, &H60, &H3C, &H7D, &H2, &HAF, &HA1, &H2B, &H9B, &H22, &H4B, &H3B, &HFC, &HD2, &H5D, &H73, &HE9, &H29, &H34, &H91, &H85, &H93, &H4C, &HBE, &HBE, &H73, &HA9, &HD2, &H3B, &H27, &H7A, &H47, &H76, &HEC, &HB0, &H28, &HC9, &HC1, &HDA, &HEE, &HAA, &HB3, &H96, &H9C, &H1E, &HF5, &H6B, &HF6, &H64, &HD8, &H94, &H2E, &HF1, &HF7, &H14, &H5F, &HA0, &HF1, &HA3, &HB9, &HB1, &HAA, &H58, &H97, &HDC, &H9, &H17, &HC, &H4, &HD3, &H8E, &H2, &H2C, &H83, &H8A, &HD6, &HAF, &H7C, &HFE, &H83, &H33, &HC6, &HA8, &HC3, &H84, &HEF, &H29, &H6, &HA9, &HB7, &H2D, &H6, &HB, &HD, &H6F, &H70, &H9E, &H34, &HA6, &HC7, &H31, &HBE, &H56, &HDE, &HDD, &H2, &H92, &HF8, &HA0, &H58, &HB, &HFC, &HFA, &HBA, &H49, &HB4, &H48, &HDB, &HEC, &H25, &HF3, &H18, &H8F, &H2D, &HB3, &HC0, &HB8, &HDD, &HBC, &HD6, &HAA, &HA6, &HDB, &H6F, &H7D, &H7D, &H25, &HA6, &HCD, &H39, &H6D, &HDA, &H76, &HC, &H79, &HBF, &H48, &H25, &HFC, &H2D, &HC5, &HFA, &H53, &H9B, &H4D, &H60, &HF4, &HEF, &HC7, &HEA, &HAC, &HA1, &H7B, &H3, &HF4, &HAF, &HC7})
            'Dim sig As New BigInteger(Signature)
            'Dim res As BigInteger = sig.ModPow(power, pmod)
            'Dim result() As Byte = res.GetBytes()

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

            Console.WriteLine("Result:       " & BitConverter.ToString(result))
            Console.WriteLine("ProperResult: " & BitConverter.ToString(properResult))

            For i As Integer = 0 To result.Length - 1
                If result(i) <> properResult(i) Then Return False
            Next

            Return True
        End Function
#End Region

#Region "Prepare Module"
        <StructLayout(LayoutKind.Explicit, Size:=&H8)>
        Public Structure CLibraryEntry
            <FieldOffset(&H0)>
            Public dwFileName As Integer
            <FieldOffset(&H4)>
            Public dwImports As Integer
        End Structure

        <StructLayout(LayoutKind.Explicit, Size:=&H28)>
        Public Structure CHeader
            <FieldOffset(&H0)>
            Public dwModuleSize As Integer
            <FieldOffset(&H4)>
            Public dwDestructor As Integer
            <FieldOffset(&H8)>
            Public dwSizeOfCode As Integer
            <FieldOffset(&HC)>
            Public dwRelocationCount As Integer
            <FieldOffset(&H10)>
            Public dwProcedureTable As Integer
            <FieldOffset(&H14)>
            Public dwProcedureCount As Integer
            <FieldOffset(&H18)>
            Public dwProcedureAdjust As Integer
            <FieldOffset(&H1C)>
            Public dwLibraryTable As Integer
            <FieldOffset(&H20)>
            Public dwLibraryCount As Integer
            <FieldOffset(&H24)>
            Public dwChunkCount As Integer
        End Structure

        Private delegateCache As New Dictionary(Of String, [Delegate])

        Private dwModuleSize As Integer = 0
        Private dwLibraryCount As Integer = 0

        Dim Header As CHeader

        Private Function PrepairModule(ByRef data() As Byte) As Boolean
            Try
                Dim pModule As Integer = ByteArrPtr(data)
                Header = Marshal.PtrToStructure(New IntPtr(pModule), GetType(CHeader))

                dwModuleSize = Header.dwModuleSize

                If dwModuleSize < &H7FFFFFFF Then
                    m_Mod = malloc(dwModuleSize)

                    If m_Mod Then
                        Marshal.Copy(data, 0, m_Mod, &H28)
                        Dim index As Integer = &H28 + Header.dwChunkCount * 3 * 4
                        Dim dwChunkDest As Integer = m_Mod + BitConverter.ToInt32(data, &H28)
                        Dim dwModuleEnd As Integer = m_Mod + dwModuleSize
                        Dim bCopyChunk As Boolean = True
                        While dwChunkDest < dwModuleEnd
                            Dim dwCurrentChunkSize As Integer = BitConverter.ToInt16(data, index)
                            index += 2

                            If bCopyChunk Then
                                Marshal.Copy(data, index, New IntPtr(dwChunkDest), dwCurrentChunkSize)
                                index += dwCurrentChunkSize
                            End If

                            dwChunkDest += dwCurrentChunkSize
                            bCopyChunk = Not bCopyChunk
                        End While

                        Log.WriteLine(LogType.DEBUG, "[WARDEN] Update...")
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] Update: Adjusting references to global variables...")

                        Dim pbRelocationTable As Integer = m_Mod + Header.dwSizeOfCode
                        Dim dwRelocationIndex As Integer = 0
                        Dim dwLastRelocation As Integer = 0
                        While dwRelocationIndex < Header.dwRelocationCount
                            Dim dwValue As Integer = Marshal.ReadByte(New IntPtr(pbRelocationTable))
                            If dwValue < 0 Then
                                dwValue = (dwValue And &H7F) << 8
                                dwValue = (dwValue + Marshal.ReadByte(New IntPtr(pbRelocationTable + 1))) << 8
                                dwValue = (dwValue + Marshal.ReadByte(New IntPtr(pbRelocationTable + 2))) << 8
                                dwValue += Marshal.ReadByte(New IntPtr(pbRelocationTable + 3))
                                pbRelocationTable += 4

                                Dim old As Integer = Marshal.ReadInt32(New IntPtr(m_Mod + dwValue))
                                Marshal.WriteInt32(New IntPtr(m_Mod + dwValue), m_Mod + old)
                            Else
                                dwValue = (dwValue << 8) + dwLastRelocation + Marshal.ReadByte(New IntPtr(pbRelocationTable + 1))
                                pbRelocationTable += 2

                                Dim old As Integer = Marshal.ReadInt32(New IntPtr(m_Mod + dwValue))
                                Marshal.WriteInt32(New IntPtr(m_Mod + dwValue), m_Mod + old)
                            End If
                            dwRelocationIndex += 1
                            dwLastRelocation = dwValue
                        End While

                        Log.WriteLine(LogType.DEBUG, "[WARDEN] Update: Updating API library references...")

                        Dim dwLibraryIndex As Integer = 0
                        While dwLibraryIndex < Header.dwLibraryCount
                            Dim pLibraryTable As CLibraryEntry = Marshal.PtrToStructure(New IntPtr(m_Mod + Header.dwLibraryTable + (dwLibraryIndex * 8)), GetType(CLibraryEntry))

                            Dim procLib As String = Marshal.PtrToStringAnsi(New IntPtr(m_Mod + pLibraryTable.dwFileName))

                            Log.WriteLine(LogType.DEBUG, "    Library: {0}", procLib)
                            Dim hModule As Integer = LoadLibrary(procLib, "")
                            If hModule Then
                                Dim dwImports As Integer = m_Mod + pLibraryTable.dwImports
                                Dim dwCurrent As Integer = Marshal.ReadInt32(New IntPtr(dwImports))
                                While dwCurrent <> 0
                                    Dim procAddr As Integer = 0
                                    dwCurrent = Marshal.ReadInt32(New IntPtr(dwImports))
                                    If dwCurrent <= 0 Then
                                        dwCurrent = (dwCurrent And &H7FFFFFFF)
                                        procAddr = GetProcAddress(hModule, Convert.ToString(New IntPtr(dwCurrent)), "")

                                        Log.WriteLine(LogType.DEBUG, "        Ordinary: 0x{0:X8}", dwCurrent)
                                    Else
                                        Dim procFunc As String = Marshal.PtrToStringAnsi(New IntPtr(m_Mod + dwCurrent))
                                        Dim procRedirector As Reflection.MethodInfo = GetType(ApiRedirector).GetMethod("my" + procFunc)
                                        Dim procDelegate As Type = GetType(ApiRedirector).GetNestedType("d" + procFunc)
                                        If procRedirector Is Nothing OrElse procDelegate Is Nothing Then
                                            procAddr = GetProcAddress(hModule, procFunc, "")
                                            Log.WriteLine(LogType.DEBUG, "        Function: {0} @ 0x{1:X8}", procFunc, procAddr)
                                        Else
                                            delegateCache.Add(procFunc, [Delegate].CreateDelegate(procDelegate, procRedirector))
                                            procAddr = Marshal.GetFunctionPointerForDelegate(delegateCache(procFunc))
                                            Log.WriteLine(LogType.DEBUG, "        Function: {0} @ MY 0x{1:X8}", procFunc, procAddr)
                                        End If
                                        Marshal.WriteInt32(New IntPtr(dwImports), procAddr)
                                    End If
                                    dwImports += 4
                                End While
                            End If

                            dwLibraryIndex += 1
                            dwLibraryCount += 1
                        End While

                        Dim dwIndex As Integer = 0
                        While dwIndex < Header.dwChunkCount
                            Dim pdwChunk2 As Integer = m_Mod + (10 + dwIndex * 3 * 4) * 4

                            Dim dwOldProtect As UInteger = 0
                            Dim lpAddress As Integer = m_Mod + Marshal.ReadInt32(New IntPtr(pdwChunk2))
                            Dim dwSize As Integer = Marshal.ReadInt32(New IntPtr(pdwChunk2 + 4))
                            Dim flNewProtect As Integer = Marshal.ReadInt32(New IntPtr(pdwChunk2 + 8))

                            VirtualProtect(lpAddress, dwSize, flNewProtect, dwOldProtect, "")
                            If (flNewProtect And &HF0) Then
                                FlushInstructionCache(GetCurrentProcess(""), lpAddress, dwSize, "")
                            End If

                            dwIndex += 1
                        End While

                        Dim bUnload As Boolean = True
                        If Header.dwSizeOfCode < dwModuleSize Then
                            Dim dwOffset As Integer = ((Header.dwSizeOfCode + &HFFF) And &HFFFFF000)
                            If dwOffset >= Header.dwSizeOfCode AndAlso dwOffset > dwModuleSize Then
                                VirtualFree(m_Mod + dwOffset, dwModuleSize - dwOffset, &H4000, "")
                            End If

                            bUnload = False
                        End If

                        If bUnload Then
                            'TODO: Unload?
                            Return False
                        End If
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If

                Return True
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "Failed to prepair module.{0}{1}", vbNewLine, ex.ToString)
                Return False
            End Try
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

        Private Function InitModule() As Boolean

            Dim dwProcedureDiff As Integer = 1 - Header.dwProcedureAdjust
            If dwProcedureDiff > Header.dwProcedureCount Then Return False
            Dim fInit As Integer = Marshal.ReadInt32(New IntPtr(m_Mod + Header.dwProcedureTable + (dwProcedureDiff * 4)))
            InitPointer = m_Mod + fInit
            Console.WriteLine("Initialize Function is mapped at 0x{0:X}", InitPointer)

            SendPacketD = AddressOf SendPacket
            CheckModuleD = AddressOf CheckModule
            ModuleLoadD = AddressOf ModuleLoad
            AllocateMemD = AddressOf AllocateMem
            FreeMemoryD = AddressOf FreeMemory
            SetRC4DataD = AddressOf SetRC4Data
            GetRC4DataD = AddressOf GetRC4Data

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
            Try
                Log.WriteLine(LogType.SUCCESS, "[WARDEN] Successfully Initialized Module.")
                init = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(InitPointer), GetType(InitializeModule)), InitializeModule)
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "[WARDEN] Failed to Initialize Module.")
            End Try

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

            Return True
        End Function

#End Region

#Region "Unload Module"
        Private Sub Unload_Module()
            'TODO!!
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
            Dim pK As Integer = ByteArrPtr(K)
            GenerateRC4Keys(m_ModMem, pK, K.Length)
            free(pK)
        End Sub

        Public Function HandlePacket(ByVal PacketData() As Byte) As Integer
            m_PKT = New Byte() {}
            Dim BytesRead As Integer = 0
            BytesRead = VarPtr(BytesRead)
            Dim pPacket As Integer = ByteArrPtr(PacketData)
            PacketHandler(m_ModMem, pPacket, PacketData.Length, BytesRead)
            free(pPacket)
            Return Marshal.ReadInt32(New IntPtr(BytesRead))
        End Function

        Public Function ReadPacket() As Byte()
            Return m_PKT
        End Function

        Public Sub ReadKeys(ByRef objCharacter As CharacterObject)
            Dim KeyData(&H204 - 1) As Byte
            Marshal.Copy(New IntPtr(m_ModMem + 32), KeyData, 0, KeyData.Length)
            Buffer.BlockCopy(KeyData, 0, objCharacter.WardenData.KeyOut, 0, 258)
            Buffer.BlockCopy(KeyData, 258, objCharacter.WardenData.KeyIn, 0, 258)
        End Sub

        Public Sub ReadXorByte(ByRef objCharacter As CharacterObject)
            Dim ClientSeed(16 - 1) As Byte
            Marshal.Copy(New IntPtr(m_ModMem + 4), ClientSeed, 0, ClientSeed.Length)

            objCharacter.WardenData.ClientSeed = ClientSeed
            objCharacter.WardenData.xorByte = ClientSeed(0)
        End Sub
#End Region

#Region "ApiRedirector"
        Public Class ApiRedirector

        End Class
#End Region

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Script.Dispose()
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

    End Class
#End Region

#Region "Scans"
    Public Class WardenScan

        Private Character As CharacterObject = Nothing

        Private UsedStrings As New List(Of String)
        Private Checks As New List(Of CheatCheck)

        Public Sub New(ByRef objCharacter As CharacterObject)
            Character = objCharacter
        End Sub

        Public Sub Do_MEM_CHECK(ByVal ScanModule As String, ByVal Offset As Integer, ByVal Length As Byte)
            Dim newCheck As New CheatCheck(CheckTypes.MEM_CHECK)
            newCheck.Str = ScanModule
            newCheck.Addr = Offset
            newCheck.Length = Length

            If ScanModule <> "" Then UsedStrings.Add(ScanModule)
            Checks.Add(newCheck)
        End Sub

        Public Sub Do_PAGE_CHECK_A_B(ByVal Seed As Integer, ByVal Hash() As Byte, ByVal Offset As Integer, ByVal Length As Byte)
            Dim newCheck As New CheatCheck(CheckTypes.PAGE_CHECK_A_B)
            newCheck.Seed = Seed
            newCheck.Hash = Hash
            newCheck.Addr = Offset
            newCheck.Length = Length

            Checks.Add(newCheck)
        End Sub

        Public Sub Do_MPQ_CHECK(ByVal File As String)
            Dim newCheck As New CheatCheck(CheckTypes.MPQ_CHECK)
            newCheck.Str = File

            UsedStrings.Add(File)
            Checks.Add(newCheck)
        End Sub

        Public Sub Do_LUA_STR_CHECK(ByVal str As String)
            Dim newCheck As New CheatCheck(CheckTypes.LUA_STR_CHECK)
            newCheck.Str = str

            UsedStrings.Add(str)
            Checks.Add(newCheck)
        End Sub

        Public Sub Do_DRIVER_CHECK(ByVal Seed As Integer, ByVal Hash() As Byte, ByVal Driver As String)
            Dim newCheck As New CheatCheck(CheckTypes.DRIVER_CHECK)
            newCheck.Seed = Seed
            newCheck.Hash = Hash
            newCheck.Str = Driver

            UsedStrings.Add(Driver)
            Checks.Add(newCheck)
        End Sub

        Public Sub Do_TIMING_CHECK()
            Dim newCheck As New CheatCheck(CheckTypes.TIMING_CHECK)

            Checks.Add(newCheck)
        End Sub

        Public Sub Do_PROC_CHECK(ByVal Seed As Integer, ByVal Hash() As Byte, ByVal ScanModule As String, ByVal ProcName As String, ByVal Offset As Integer, ByVal Length As Byte)
            Dim newCheck As New CheatCheck(CheckTypes.PROC_CHECK)
            newCheck.Seed = Seed
            newCheck.Hash = Hash
            newCheck.Str = ScanModule
            newCheck.Str2 = ProcName
            newCheck.Addr = Offset
            newCheck.Length = Length

            UsedStrings.Add(ScanModule)
            UsedStrings.Add(ProcName)
            Checks.Add(newCheck)
        End Sub

        Public Sub Do_MODULE_CHECK(ByVal Seed As Integer, ByVal Hash() As Byte)
            Dim newCheck As New CheatCheck(CheckTypes.MODULE_CHECK)
            newCheck.Seed = Seed
            newCheck.Hash = Hash

            Checks.Add(newCheck)
        End Sub

        Public Function GetPacket() As PacketClass
            Dim packet As New PacketClass(OPCODES.SMSG_WARDEN_DATA)
            packet.AddInt8(MaievOpcode.MAIEV_MODULE_RUN)
            For Each tmpStr As String In UsedStrings
                packet.AddString2(tmpStr)
            Next
            packet.AddInt8(0)

            Dim i As Byte = 0
            For Each Check As CheatCheck In Checks
                Dim xorCheck As Byte = (Maiev.CheckIDs(Check.Type) Xor Character.WardenData.xorByte)
                Dim checkData() As Byte = Check.ToData(xorCheck, i)
                packet.AddByteArray(checkData)
            Next

            packet.AddInt8(Character.WardenData.xorByte)

            Return packet
        End Function

        Public Sub Reset()
            Checks.Clear()
            UsedStrings.Clear()
        End Sub

        Public Sub HandleResponse(ByRef p As PacketClass)
            'TODO: Now do the check if we have a cheater or not :)

            For Each Check As CheatCheck In Checks
                Select Case Check.Type
                    Case CheckTypes.MEM_CHECK ' MEM_CHECK: uint8 result, uint8[] bytes
                        Dim result As Byte = p.GetInt8()
                        Dim bytes() As Byte = p.GetByteArray '(Check.Length)
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}] Result={1} Bytes=0x{2}", Check.Type, result, BitConverter.ToString(bytes).Replace("-", ""))

                    Case CheckTypes.PAGE_CHECK_A_B ' PAGE_CHECK_A_B: uint8 result
                        Dim result As Byte = p.GetInt8()
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}] Result={1}", Check.Type, result)

                    Case CheckTypes.MPQ_CHECK ' MPQ_CHECK: uint8 result, uint8[20] sha1
                        Dim result As Byte = p.GetInt8()
                        Dim hash() As Byte = p.GetByteArray '(20)
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}] Result={1} Hash=0x{2}", Check.Type, result, BitConverter.ToString(hash).Replace("-", ""))

                    Case CheckTypes.LUA_STR_CHECK ' LUA_STR_CHECK: uint8 unk, uint8 len, char[len] data
                        Dim unk As Byte = p.GetInt8()
                        Dim data As String = p.GetString2()
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}] Result={1} Data={2}", Check.Type, unk, data)

                    Case CheckTypes.DRIVER_CHECK ' DRIVER_CHECK: uint8 result
                        Dim result As Byte = p.GetInt8()
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}] Result={1}", Check.Type, result)

                    Case CheckTypes.TIMING_CHECK ' TIMING_CHECK: uint8 result, uint32 time
                        Dim result As Byte = p.GetInt8()
                        Dim time As Integer = p.GetInt32()
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}] Result={1} Time={2}", Check.Type, result, time)

                    Case CheckTypes.PROC_CHECK ' PROC_CHECK: uint8 result
                        Dim result As Byte = p.GetInt8()
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}] Result={1}", Check.Type, result)

                    Case CheckTypes.MODULE_CHECK
                        'What is the structure for this result?
                        Log.WriteLine(LogType.DEBUG, "[WARDEN] [{0}]", Check.Type)

                End Select
            Next

            Reset()
        End Sub

    End Class

    Public Class CheatCheck
        Public Type As CheckTypes
        Public Str As String = ""
        Public Str2 As String = ""
        Public Addr As Integer = 0
        Public Hash() As Byte = {}
        Public Seed As Integer = 0
        Public Length As Byte = 0

        Public Sub New(ByVal Type_ As CheckTypes)
            Type = Type_
        End Sub

        Public Function ToData(ByVal XorCheck As Byte, ByRef index As Byte) As Byte()
            Dim ms As New MemoryStream
            Dim bw As New BinaryWriter(ms)

            bw.Write(XorCheck)
            Select Case Type
                Case CheckTypes.MEM_CHECK 'byte strIndex + uint Offset + byte Len
                    If Str = "" Then
                        bw.Write(CByte(0))
                    Else
                        bw.Write(index)
                        index += 1
                    End If
                    bw.Write(Addr)
                    bw.Write(Length)
                Case CheckTypes.PAGE_CHECK_A_B 'uint Seed + byte[20] SHA1 + uint Addr + byte Len
                    bw.Write(Seed)
                    bw.Write(Hash, 0, Hash.Length)
                    bw.Write(Addr)
                    bw.Write(Length)
                Case CheckTypes.MPQ_CHECK ' byte strIndex
                    bw.Write(index)
                    index += 1
                Case CheckTypes.LUA_STR_CHECK ' byte strIndex
                    bw.Write(index)
                    index += 1
                Case CheckTypes.DRIVER_CHECK ' uint Seed + byte[20] SHA1 + byte strIndex
                    bw.Write(Seed)
                    bw.Write(Hash, 0, Hash.Length)
                    bw.Write(index)
                    index += 1
                Case CheckTypes.TIMING_CHECK 'empty

                Case CheckTypes.PROC_CHECK ' uint Seed + byte[20] SHA1 + byte strIndex1 + byte strIndex2 + uint Offset + byte Len
                    bw.Write(Seed)
                    bw.Write(Hash, 0, Hash.Length)
                    bw.Write(index)
                    index += 1
                    bw.Write(index)
                    index += 1
                    bw.Write(Addr)
                    bw.Write(Length)
                Case CheckTypes.MODULE_CHECK ' uint Seed + byte[20] SHA1
                    bw.Write(Seed)
                    bw.Write(Hash, 0, Hash.Length)
            End Select

            Dim tmpData() As Byte = ms.ToArray
            ms.Close()
            'ms.Dispose()
            ms = Nothing
            bw = Nothing
            Return tmpData
        End Function

    End Class
#End Region

#Region "Other functions"

    Private Function VarPtr(ByRef obj As Object) As Integer
        Dim gc As GCHandle = GCHandle.Alloc(obj, GCHandleType.Pinned)
        Return gc.AddrOfPinnedObject.ToInt32()
    End Function

    Private Function ByteArrPtr(ByRef arr() As Byte) As Integer
        Dim pData As Integer = malloc(arr.Length)
        Marshal.Copy(arr, 0, New IntPtr(pData), arr.Length)
        Return pData
    End Function

    Private Function malloc(ByVal length As Integer) As Integer
        Dim tmpHandle As Integer = Marshal.AllocHGlobal(length + 4).ToInt32()
        Dim lockedHandle As Integer = GlobalLock(tmpHandle, "") + 4
        Marshal.WriteInt32(New IntPtr(lockedHandle - 4), tmpHandle)
        Return lockedHandle
    End Function

    Private Sub free(ByVal ptr As Integer)
        Dim tmpHandle As Integer = Marshal.ReadInt32(New IntPtr(ptr - 4))
        GlobalUnlock(tmpHandle, "")
        Marshal.FreeHGlobal(New IntPtr(tmpHandle))
    End Sub
#End Region

    Public Sub SendWardenPacket(ByRef objCharacter As CharacterObject, ByRef Packet As PacketClass)
        'START Warden Encryption
        Dim b(Packet.Data.Length - 4 - 1) As Byte
        Buffer.BlockCopy(Packet.Data, 4, b, 0, b.Length)
        RC4.Crypt(b, objCharacter.WardenData.KeyIn)
        Buffer.BlockCopy(b, 0, Packet.Data, 4, b.Length)
        'END

        objCharacter.client.Send(Packet)
    End Sub

End Module
