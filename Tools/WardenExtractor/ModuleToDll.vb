'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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
Imports Emil.GMP

Public Module Module_ModuleToDll

    Public Declare Function BN_add Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr) As Integer
    Public Declare Function BN_bin2bn Lib "LIBEAY32" (ByVal ByteArrayIn As Byte(), ByVal length As Integer, ByVal [to] As IntPtr) As IntPtr
    Public Declare Function BN_bn2bin Lib "LIBEAY32" (ByVal a As IntPtr, ByVal [to] As Byte()) As Integer
    Public Declare Function BN_CTX_free Lib "LIBEAY32" (ByVal a As IntPtr) As Integer
    Public Declare Function BN_CTX_new Lib "LIBEAY32" () As IntPtr
    Public Declare Function BN_mod Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr) As Integer
    Public Declare Function BN_mod_exp Lib "LIBEAY32" (ByVal res As IntPtr, ByVal a As IntPtr, ByVal p As IntPtr, ByVal m As IntPtr, ByVal ctx As IntPtr) As IntPtr
    Public Declare Function BN_mul Lib "LIBEAY32" (ByVal r As IntPtr, ByVal a As IntPtr, ByVal b As IntPtr, ByVal ctx As IntPtr) As Integer
    Public Declare Function BN_new Lib "LIBEAY32" () As IntPtr

    Public Sub ModulesToDlls()
        If Directory.Exists("Modules") = False Then
            Directory.CreateDirectory("Modules")
        End If
        Dim sFiles() As String = Directory.GetFiles("Modules")
        If sFiles.Length = 0 Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("No modules found.")
            Exit Sub
        End If

        For Each sFile As String In Directory.GetFiles("Modules")
            Console.WriteLine(sFile)
            Dim fileData() As Byte = Nothing
            Try
                Dim fs As New FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.Read)

                If fs.Length = 0 Then
                    fileData = New Byte() {}
                Else
                    ReDim fileData(fs.Length - 1)
                    fs.Read(fileData, 0, fileData.Length)
                End If
                fs.Close()
                fs.Dispose()
                fs = Nothing
                If fileData.Length = 0 Then Continue For
            Catch
            End Try

            If fileData Is Nothing Then Continue For
            ModuleToDll(Path.GetFileName(sFile), fileData)
        Next
    End Sub

    Public Sub DllsToModules()
        If Directory.Exists("Dlls") = False Then
            Directory.CreateDirectory("Dlls")
        End If
        Dim sFiles() As String = Directory.GetFiles("Dlls")
        If sFiles.Length = 0 Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("No dlls found.")
            Exit Sub
        End If

        For Each sFile As String In Directory.GetFiles("Dlls")
            Console.WriteLine(sFile)
            Dim fileData() As Byte = Nothing
            Try
                Dim fs As New FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.Read)

                If fs.Length = 0 Then
                    fileData = New Byte() {}
                Else
                    ReDim fileData(fs.Length - 1)
                    fs.Read(fileData, 0, fileData.Length)
                End If
                fs.Close()
                fs.Dispose()
                fs = Nothing
                If fileData.Length = 0 Then Continue For
            Catch
            End Try

            If fileData Is Nothing Then Continue For
            DllToModule(Path.GetFileName(sFile), fileData)
        Next
    End Sub

    Public Sub ModuleToDll(ByVal ModName As String, ByRef ModData() As Byte)
        'Console.WriteLine("Insert RC4 Key for {0}:", ModName)
        'Dim RC4Key As String = Console.ReadLine()

        Dim Key As Byte() = ParseKey("ECA9A9D1EAAEFD38CC115062FB92996E") 'ParseKey(RC4Key)
        Key = RC4.Init(Key)
        RC4.Crypt(ModData, Key)

        Dim UncompressedLen As Integer = BitConverter.ToInt32(ModData, 0)
        If UncompressedLen < 0 Then
            Console.WriteLine("Failed to decrypt {0}, incorrect length.", ModName)
            Exit Sub
        End If

        Dim CompressedData(ModData.Length - &H108 - 1) As Byte
        Array.Copy(ModData, 4, CompressedData, 0, CompressedData.Length)
        Dim dataPos As Integer = 4 + CompressedData.Length
        Dim Sign As String = Chr(ModData(dataPos + 3)) & Chr(ModData(dataPos + 2)) & Chr(ModData(dataPos + 1)) & Chr(ModData(dataPos))
        If Sign <> "SIGN" Then
            Console.WriteLine("Failed to decrypt {0}, sign missing.", ModName)
            Exit Sub
        End If
        dataPos += 4
        Dim Signature(&H100 - 1) As Byte
        Array.Copy(ModData, dataPos, Signature, 0, Signature.Length)
        dataPos += &H100

        'Check signature
        If CheckSignature(Signature, ModData, ModData.Length - &H104) = False Then
            Console.WriteLine("Signature fail.")
            Exit Sub
        End If

        Dim DecompressedData() As Byte = DeCompress(CompressedData)

        Dim fs3 As New FileStream("dlls\" & ModName.Replace(Path.GetExtension(ModName), "") & ".before.dll", FileMode.Create, FileAccess.Write, FileShare.None)
        fs3.Write(DecompressedData, 0, DecompressedData.Length)
        fs3.Close()
        fs3.Dispose()
        fs3 = Nothing

        FixNormalDll(DecompressedData)

        Dim fs2 As New FileStream("dlls\" & ModName.Replace(Path.GetExtension(ModName), "") & ".after.dll", FileMode.Create, FileAccess.Write, FileShare.None)
        fs2.Write(DecompressedData, 0, DecompressedData.Length)
        fs2.Close()
        fs2.Dispose()
        fs2 = Nothing
    End Sub

    Public Sub DllToModule(ByVal DllName As String, ByRef DllData() As Byte)
        Console.WriteLine("Insert RC4 Key for {0}:", DllName)
        Dim RC4Key As String = Console.ReadLine()
        Dim Key As Byte() = ParseKey(RC4Key)
        Key = RC4.Init(Key)

        Dim CompressedData() As Byte = Compress(DllData, 0, DllData.Length)

        Dim mw As New MemoryStream
        Dim bw As New BinaryWriter(mw)

        bw.Write(DllData.Length) 'Uncompressed buffer
        bw.Write(CompressedData, 0, CompressedData.Length) 'Data
        bw.Write(CByte(Asc("N"c))) '\
        bw.Write(CByte(Asc("G"c))) ' \ Sign
        bw.Write(CByte(Asc("I"c))) ' /
        bw.Write(CByte(Asc("S"c))) '/

        Dim tmpData() As Byte = mw.ToArray
        Dim Signature() As Byte = CreateSignature(tmpData, tmpData.Length - 4)
        tmpData = Nothing

        bw.Write(Signature, 0, Signature.Length)

        tmpData = mw.ToArray
        mw.Close()
        mw.Dispose()
        mw = Nothing
        bw = Nothing


        RC4.Crypt(tmpData, Key)

        Dim fs2 As New FileStream("Modules\" & DllName.Replace(Path.GetExtension(DllName), "") & ".mod", FileMode.Create, FileAccess.Write, FileShare.None)
        fs2.Write(tmpData, 0, tmpData.Length)
        fs2.Close()
        fs2.Dispose()
        fs2 = Nothing
    End Sub

    Public Function CheckSignature(ByVal Signature() As Byte, ByVal Data() As Byte, ByVal DataLen As Integer) As Boolean
        Dim power As New BigInt(New Byte() {&H1, &H0, &H1, &H0})
        Dim pmod As New BigInt(New Byte() {&H6B, &HCE, &HF5, &H2D, &H2A, &H7D, &H7A, &H67, &H21, &H21, &H84, &HC9, &HBC, &H25, &HC7, &HBC, &HDF, &H3D, &H8F, &HD9, &H47, &HBC, &H45, &H48, &H8B, &H22, &H85, &H3B, &HC5, &HC1, &HF4, &HF5, &H3C, &HC, &H49, &HBB, &H56, &HE0, &H3D, &HBC, &HA2, &HD2, &H35, &HC1, &HF0, &H74, &H2E, &H15, &H5A, &H6, &H8A, &H68, &H1, &H9E, &H60, &H17, &H70, &H8B, &HBD, &HF8, &HD5, &HF9, &H3A, &HD3, &H25, &HB2, &H66, &H92, &HBA, &H43, &H8A, &H81, &H52, &HF, &H64, &H98, &HFF, &H60, &H37, &HAF, &HB4, &H11, &H8C, &HF9, &H2E, &HC5, &HEE, &HCA, &HB4, &H41, &H60, &H3C, &H7D, &H2, &HAF, &HA1, &H2B, &H9B, &H22, &H4B, &H3B, &HFC, &HD2, &H5D, &H73, &HE9, &H29, &H34, &H91, &H85, &H93, &H4C, &HBE, &HBE, &H73, &HA9, &HD2, &H3B, &H27, &H7A, &H47, &H76, &HEC, &HB0, &H28, &HC9, &HC1, &HDA, &HEE, &HAA, &HB3, &H96, &H9C, &H1E, &HF5, &H6B, &HF6, &H64, &HD8, &H94, &H2E, &HF1, &HF7, &H14, &H5F, &HA0, &HF1, &HA3, &HB9, &HB1, &HAA, &H58, &H97, &HDC, &H9, &H17, &HC, &H4, &HD3, &H8E, &H2, &H2C, &H83, &H8A, &HD6, &HAF, &H7C, &HFE, &H83, &H33, &HC6, &HA8, &HC3, &H84, &HEF, &H29, &H6, &HA9, &HB7, &H2D, &H6, &HB, &HD, &H6F, &H70, &H9E, &H34, &HA6, &HC7, &H31, &HBE, &H56, &HDE, &HDD, &H2, &H92, &HF8, &HA0, &H58, &HB, &HFC, &HFA, &HBA, &H49, &HB4, &H48, &HDB, &HEC, &H25, &HF3, &H18, &H8F, &H2D, &HB3, &HC0, &HB8, &HDD, &HBC, &HD6, &HAA, &HA6, &HDB, &H6F, &H7D, &H7D, &H25, &HA6, &HCD, &H39, &H6D, &HDA, &H76, &HC, &H79, &HBF, &H48, &H25, &HFC, &H2D, &HC5, &HFA, &H53, &H9B, &H4D, &H60, &HF4, &HEF, &HC7, &HEA, &HAC, &HA1, &H7B, &H3, &HF4, &HAF, &HC7})
        Dim sig As New BigInt(Signature)
        Dim res As BigInt = sig.PowerMod(power, pmod)
        Dim result() As Byte = res.ToByteArray()

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

    Public Function CreateSignature(ByVal Data() As Byte, ByVal DataLen As Integer) As Byte()
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

        Dim power As New BigInt(New Byte() {&H1, &H3, &H3, &H7, &H0, &HD, &HE, &HA, &HD, &HF, &H0, &H0, &HD}) 'Notice our own little private key (original wow clients won't accept this)
        Dim pmod As New BigInt(New Byte() {&H6B, &HCE, &HF5, &H2D, &H2A, &H7D, &H7A, &H67, &H21, &H21, &H84, &HC9, &HBC, &H25, &HC7, &HBC, &HDF, &H3D, &H8F, &HD9, &H47, &HBC, &H45, &H48, &H8B, &H22, &H85, &H3B, &HC5, &HC1, &HF4, &HF5, &H3C, &HC, &H49, &HBB, &H56, &HE0, &H3D, &HBC, &HA2, &HD2, &H35, &HC1, &HF0, &H74, &H2E, &H15, &H5A, &H6, &H8A, &H68, &H1, &H9E, &H60, &H17, &H70, &H8B, &HBD, &HF8, &HD5, &HF9, &H3A, &HD3, &H25, &HB2, &H66, &H92, &HBA, &H43, &H8A, &H81, &H52, &HF, &H64, &H98, &HFF, &H60, &H37, &HAF, &HB4, &H11, &H8C, &HF9, &H2E, &HC5, &HEE, &HCA, &HB4, &H41, &H60, &H3C, &H7D, &H2, &HAF, &HA1, &H2B, &H9B, &H22, &H4B, &H3B, &HFC, &HD2, &H5D, &H73, &HE9, &H29, &H34, &H91, &H85, &H93, &H4C, &HBE, &HBE, &H73, &HA9, &HD2, &H3B, &H27, &H7A, &H47, &H76, &HEC, &HB0, &H28, &HC9, &HC1, &HDA, &HEE, &HAA, &HB3, &H96, &H9C, &H1E, &HF5, &H6B, &HF6, &H64, &HD8, &H94, &H2E, &HF1, &HF7, &H14, &H5F, &HA0, &HF1, &HA3, &HB9, &HB1, &HAA, &H58, &H97, &HDC, &H9, &H17, &HC, &H4, &HD3, &H8E, &H2, &H2C, &H83, &H8A, &HD6, &HAF, &H7C, &HFE, &H83, &H33, &HC6, &HA8, &HC3, &H84, &HEF, &H29, &H6, &HA9, &HB7, &H2D, &H6, &HB, &HD, &H6F, &H70, &H9E, &H34, &HA6, &HC7, &H31, &HBE, &H56, &HDE, &HDD, &H2, &H92, &HF8, &HA0, &H58, &HB, &HFC, &HFA, &HBA, &H49, &HB4, &H48, &HDB, &HEC, &H25, &HF3, &H18, &H8F, &H2D, &HB3, &HC0, &HB8, &HDD, &HBC, &HD6, &HAA, &HA6, &HDB, &H6F, &H7D, &H7D, &H25, &HA6, &HCD, &H39, &H6D, &HDA, &H76, &HC, &H79, &HBF, &H48, &H25, &HFC, &H2D, &HC5, &HFA, &H53, &H9B, &H4D, &H60, &HF4, &HEF, &HC7, &HEA, &HAC, &HA1, &H7B, &H3, &HF4, &HAF, &HC7})
        Dim prop As New BigInt(properResult)
        Dim sig As BigInt = prop.PowerMod(power, pmod)
        Dim result() As Byte = sig.ToByteArray()

        Return result
    End Function

End Module
