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

Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip.Compression.Streams

Module Main

    Sub Main()
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine("WardenExtractor by UniX")
        Console.WriteLine("")
        Console.WriteLine("")

TryAgain:
        Console.ForegroundColor = ConsoleColor.DarkCyan
        Console.WriteLine("Menu:")
        Console.ForegroundColor = ConsoleColor.DarkGreen
        Console.WriteLine("1: Extract Warden Modules from WDB file")
        Console.WriteLine("2: Converts all .mod files in the directory to .dll")
        Console.WriteLine("3: Converts all .dll files in the directory to .mod")
        Console.WriteLine("4: Converts WDB to the new version format")
        Console.WriteLine("5: Quit this program")
        Console.ForegroundColor = ConsoleColor.DarkMagenta
        Console.Write("Your choice: ")
        Dim sInput As String = Console.ReadLine
        If sInput = "1" Then
            ExtractCache()
        ElseIf sInput = "2" Then
            ModulesToDlls()
        ElseIf sInput = "3" Then
            DllsToModules()
        ElseIf sInput = "4" Then
            ConvertWDB()
        ElseIf sInput = "5" Then
            End
        End If
        GoTo TryAgain
    End Sub

    Public Function ToHex(ByRef bBytes() As Byte) As String
        Dim tmpStr As String = ""
        For i As Integer = 0 To bBytes.Length - 1
            If bBytes(i) < 16 Then
                tmpStr &= "0" & Hex(bBytes(i))
            Else
                tmpStr &= Hex(bBytes(i))
            End If
        Next
        Return tmpStr
    End Function

    Public Function Reverse(ByVal str As String) As String
        Dim tmpStr As String = ""
        For i As Integer = str.Length - 1 To 0 Step -1
            tmpStr &= str(i)
        Next
        Return tmpStr
    End Function

    Public Function Reverse(ByVal bytes() As Byte) As Byte()
        If bytes.Length = 0 Then Return New Byte() {}
        Dim tmpBytes(bytes.Length - 1) As Byte
        For i As Integer = bytes.Length - 1 To 0 Step -1
            tmpBytes((bytes.Length - 1) - i) = bytes(i)
        Next
        Return tmpBytes
    End Function

    Public Function ParseKey(ByVal str As String) As Byte()
        If str.Length = 0 Then Return New Byte() {}
        Dim bBytes(Int((str.Length - 1) \ 2)) As Byte
        For i As Integer = 0 To str.Length - 1 Step 2
            Try
                If i + 1 >= str.Length - 1 Then
                    bBytes(Int(i \ 2)) = CByte("&H" & str(i))
                Else
                    bBytes(Int(i \ 2)) = CByte("&H" & str(i) & str(i + 1))
                End If
            Catch
            End Try
        Next
        Return bBytes
    End Function

#Region "ZLib"
    Public Function Compress(ByVal b As Byte(), ByVal offset As Integer, ByVal len As Integer) As Byte()
        Dim buffer2() As Byte

        Try
            Dim outputStream As New MemoryStream
            Dim compressordStream As New DeflaterOutputStream(outputStream)
            compressordStream.Write(b, offset, len)
            compressordStream.Flush()
            compressordStream.Close()
            buffer2 = outputStream.ToArray
        Catch e As Exception
            buffer2 = Nothing
        End Try

        Return buffer2
    End Function

    Public Function DeCompress(ByVal b As Byte()) As Byte()
        Dim buffer2() As Byte = Nothing
        Dim writeBuffer(Short.MaxValue) As Byte

        Dim decopressorStream As New InflaterInputStream(New MemoryStream(b))
        Try
            Dim bytesRead As Integer = decopressorStream.Read(writeBuffer, 0, writeBuffer.Length)
            If (bytesRead > 0) Then
                buffer2 = New Byte(bytesRead - 1) {}
                Buffer.BlockCopy(writeBuffer, 0, buffer2, 0, bytesRead)
            End If
            decopressorStream.Flush()
            decopressorStream.Close()
        Catch e As Exception
            buffer2 = Nothing
        End Try

        Return buffer2
    End Function
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
                key(key(257) And &HFF) = key(key(256) And &HFF)
                key(key(256) And &HFF) = temp

                data(i) = (data(i) Xor key((CType(key(key(257)), Integer) + CType(key(key(256)), Integer)) And &HFF))
            Next
        End Sub
    End Class


#End Region
End Module
