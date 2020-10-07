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

Public Module Module_CacheExtract

    Public Sub ExtractCache()
        Console.Write("Name of WDB: ")
        Dim sWDB As String = Console.ReadLine

        If File.Exists(sWDB) = False Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("The file [{0}] did not exist.", sWDB)
            Exit Sub
        End If
        Dim fs As New FileStream(sWDB, FileMode.Open, FileAccess.Read, FileShare.Read)
        Dim br As New BinaryReader(fs)

        Dim Header As String = br.ReadChars(4)
        Dim Version As UInteger = br.ReadUInt32()
        Dim Lang As String = Reverse(br.ReadChars(4))
        Dim Unk As Byte() = br.ReadBytes(8)

        Console.ForegroundColor = ConsoleColor.White

        Directory.CreateDirectory("Modules")
        Do While (fs.Position + 20) <= fs.Length
            Dim ModName As String = ToHex(br.ReadBytes(16))
            Dim DataLen As Integer = br.ReadInt32()
            If DataLen = 0 Then Continue Do
            Dim ModLen As Integer = br.ReadInt32()

            Dim ModData(ModLen - 1) As Byte
            br.Read(ModData, 0, ModLen)

            Dim fs2 As New FileStream("Modules\" & ModName & ".mod", FileMode.Create, FileAccess.Write, FileShare.None)
            fs2.Write(ModData, 0, ModLen)
            fs2.Close()
            fs2.Dispose()

            Console.WriteLine("Module: {0} [{1} bytes]", ModName, ModLen)
        Loop

        fs.Close()
        fs.Dispose()
    End Sub

    Public Sub ConvertWDB()
        Console.Write("Name of WDB: ")
        Dim sWDB As String = Console.ReadLine

        If File.Exists(sWDB) = False Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("The file [{0}] did not exist.", sWDB)
            Exit Sub
        End If
        Dim fs As New FileStream(sWDB, FileMode.Open, FileAccess.Read, FileShare.Read)
        Dim br As New BinaryReader(fs)

        Dim ms As New MemoryStream
        Dim bw As New BinaryWriter(ms)

        Dim Header As String = br.ReadChars(4)
        Dim Version As UInteger = br.ReadUInt32()
        Dim Lang As String = Reverse(br.ReadChars(4))
        Dim Unk1 As Integer = br.ReadInt32()
        Dim Unk2 As Integer = br.ReadInt32()

        bw.Write(CByte(Asc(Header(0))))
        bw.Write(CByte(Asc(Header(1))))
        bw.Write(CByte(Asc(Header(2))))
        bw.Write(CByte(Asc(Header(3))))
        bw.Write(Version)
        bw.Write(CByte(Asc(Lang(3))))
        bw.Write(CByte(Asc(Lang(2))))
        bw.Write(CByte(Asc(Lang(1))))
        bw.Write(CByte(Asc(Lang(0))))
        bw.Write(Unk1)
        bw.Write(Unk2)
        Console.WriteLine("Unk1: {0}{1}Unk2: {2}", Unk1, vbNewLine, Unk2)

        Console.ForegroundColor = ConsoleColor.White
        bw.Write(1I) 'Count of modules?
        Do While (fs.Position + 20) <= fs.Length
            Dim byteName() As Byte = br.ReadBytes(16)
            Dim ModName As String = ToHex(byteName)
            Dim DataLen As Integer = br.ReadInt32()

            bw.Write(byteName, 0, byteName.Length)
            bw.Write(DataLen)

            If DataLen = 0 Then
                Continue Do
            End If
            Dim ModLen As Integer = br.ReadInt32()

            bw.Write(ModLen)

            Dim ModData(ModLen - 1) As Byte
            br.Read(ModData, 0, ModLen)
            bw.Write(ModData, 0, ModLen)

            Console.WriteLine("Module: {0} [{1} bytes]", ModName, ModLen)
        Loop
        fs.Close()
        fs.Dispose()

        Dim fs2 As New FileStream(sWDB.Replace(Path.GetExtension(sWDB), "") & ".new.wdb", FileMode.Create, FileAccess.Write, FileShare.Read)
        Dim newFile() As Byte = ms.ToArray
        fs2.Write(newFile, 0, newFile.Length)
        fs2.Close()
        fs2.Dispose()

        ms.Close()
        ms.Dispose()
    End Sub

End Module
