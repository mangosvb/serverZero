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

Imports System.Runtime.InteropServices

Public Module Functions

    Public Function SearchInFile(ByVal f As IO.Stream, ByVal s As String, Optional ByVal o As Integer = 0) As Integer
        f.Seek(0, IO.SeekOrigin.Begin)
        Dim r As New IO.BinaryReader(f)
        Dim b1() As Byte = r.ReadBytes(f.Length)
        Dim b2() As Byte = Text.Encoding.ASCII.GetBytes(s)

        For i As Integer = o To b1.Length - 1
            For j As Integer = 0 To b2.Length - 1
                If b1(i + j) <> b2(j) Then Exit For

                If j = b2.Length - 1 Then
                    Return i
                End If
            Next
        Next

        Return -1
    End Function
    Public Function SearchInFile(ByVal f As IO.Stream, ByVal v As Integer) As Integer
        f.Seek(0, IO.SeekOrigin.Begin)
        Dim r As New IO.BinaryReader(f)
        Dim b1() As Byte = r.ReadBytes(f.Length)
        Dim b2() As Byte = BitConverter.GetBytes(v)
        'Array.Reverse(b2)

        For i As Integer = 0 To b1.Length - 1
            If i + 3 >= b1.Length Then Exit For
            If b1(i) = b2(0) AndAlso
               b1(i + 1) = b2(1) AndAlso
               b1(i + 2) = b2(2) AndAlso
               b1(i + 3) = b2(3) Then
                Return i
            End If
        Next

        Return -1
    End Function
    Public Function ReadString(ByVal f As IO.FileStream) As String
        Dim r As String = ""
        Dim t As Byte

        'Read if there are zeros
        t = f.ReadByte()
        While t = 0
            t = f.ReadByte()
        End While

        'Read string
        While t <> 0
            r = r + Chr(t)
            t = f.ReadByte()
        End While

        Return r
    End Function

    Public Function ReadString(ByVal f As IO.FileStream, ByVal pos As Long) As String
        Dim r As String = ""
        Dim t As Byte
        If pos = -1 Then Return "*Nothing*"

        f.Seek(pos, IO.SeekOrigin.Begin)

        Try
            'Read if there are zeros
            t = f.ReadByte()
            While t = 0
                t = f.ReadByte()
            End While

            'Read string
            While t <> 0
                r = r + Chr(t)
                t = f.ReadByte()
            End While
        Catch
        End Try

        Return r
    End Function

    Public Function ToField(ByVal sField As String) As String
        'Make the first letter in upper case and the rest in lower case
        Dim tmp As String = sField.Substring(0, 1).ToUpper() & sField.Substring(1).ToLower()
        'Replace lowercase object with Object (used in f.ex Gameobject -> GameObject)
        If tmp.IndexOf("object", StringComparison.OrdinalIgnoreCase) > 0 Then
            If tmp.Length > (tmp.IndexOf("object", StringComparison.OrdinalIgnoreCase) + 6) Then
                tmp = tmp.Substring(0, tmp.IndexOf("object")) & "Object" & tmp.Substring(tmp.IndexOf("object") + 6)
            Else
                tmp = tmp.Substring(0, tmp.IndexOf("object")) & "Object"
            End If
        End If
        Return tmp
    End Function

    Public Function ToType(ByVal iType As Integer) As String
        'Get the typename
        Select Case iType
            Case 1
                Return "INT"
            Case 2
                Return "TWO_SHORT"
            Case 3
                Return "FLOAT"
            Case 4
                Return "GUID"
            Case 5
                Return "BYTES"
            Case Else
                Return "UNK (" & iType & ")"
        End Select
    End Function

    Private Sub AddFlag(ByRef sFlags As String, ByVal sFlag As String)
        If sFlags <> "" Then sFlags &= " + "
        sFlags &= sFlag
    End Sub

    Public Function ToFlags(ByVal iFlags As Integer) As String
        Dim tmp As String = ""
        If iFlags = 0 Then tmp = "NONE"
        If (iFlags And 1) Then AddFlag(tmp, "PUBLIC")
        If (iFlags And 2) Then AddFlag(tmp, "PRIVATE")
        If (iFlags And 4) Then AddFlag(tmp, "OWNER_ONLY")
        If (iFlags And 8) Then AddFlag(tmp, "UNK1")
        If (iFlags And 16) Then AddFlag(tmp, "UNK2")
        If (iFlags And 32) Then AddFlag(tmp, "UNK3")
        If (iFlags And 64) Then AddFlag(tmp, "GROUP_ONLY")
        If (iFlags And 128) Then AddFlag(tmp, "UNK5")
        If (iFlags And 256) Then AddFlag(tmp, "DYNAMIC")

        Return tmp
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Structure TypeEntry
        Public Name As Integer
        Public Offset As Integer
        Public Size As Integer
        Public Type As Integer
        Public Flags As Integer
    End Structure
    Public Enum Types
        NULL
        Int32
        Chars
        Float
        GUID
        Bytes
        NULL2
    End Enum


    Public Sub ExtractUpdateFields()
        Dim f As New IO.FileStream("wow.exe", IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 10000000)
        Dim r1 As New IO.BinaryReader(f)
        Dim r2 As New IO.StreamReader(f)

        Dim o As New IO.FileStream("Global.UpdateFields.vb", IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None, 1024)
        Dim w As New IO.StreamWriter(o)

        Dim FIELD_NAME_OFFSET As Integer = SearchInFile(f, "CORPSE_FIELD_PAD")
        Dim OBJECT_FIELD_GUID As Integer = SearchInFile(f, "OBJECT_FIELD_GUID") + &H400000
        Dim FIELD_TYPE_OFFSET As Integer = SearchInFile(f, OBJECT_FIELD_GUID)

        If FIELD_NAME_OFFSET = -1 Or FIELD_TYPE_OFFSET = -1 Then
            MsgBox("Wrong offsets! " & FIELD_NAME_OFFSET & "  " & FIELD_TYPE_OFFSET, MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
        Else
            Dim Names As New List(Of String)
            Dim Last As String = ""
            Dim Offset As Integer = FIELD_NAME_OFFSET
            f.Seek(Offset, IO.SeekOrigin.Begin)
            While Last <> "OBJECT_FIELD_GUID"
                Last = ReadString(f)
                Names.Add(Last)
            End While


            Dim Info As New List(Of TypeEntry)
            Dim Temp As Integer
            Dim Buffer(3) As Byte
            Offset = 0
            f.Seek(FIELD_TYPE_OFFSET, IO.SeekOrigin.Begin)
            For i = 0 To Names.Count - 1
                f.Seek(FIELD_TYPE_OFFSET + i * 5 * 4 + Offset, IO.SeekOrigin.Begin)
                f.Read(Buffer, 0, 4)
                Temp = BitConverter.ToInt32(Buffer, 0)

                If Temp < &HFFFF Then
                    i -= 1
                    Offset += 4
                    Continue For
                End If

                Dim tmp As New TypeEntry
                tmp.Name = Temp

                f.Read(Buffer, 0, 4)
                Temp = BitConverter.ToInt32(Buffer, 0)
                tmp.Offset = Temp

                f.Read(Buffer, 0, 4)
                Temp = BitConverter.ToInt32(Buffer, 0)
                tmp.Size = Temp

                f.Read(Buffer, 0, 4)
                Temp = BitConverter.ToInt32(Buffer, 0)
                tmp.Type = Temp

                f.Read(Buffer, 0, 4)
                Temp = BitConverter.ToInt32(Buffer, 0)
                tmp.Flags = Temp

                Info.Add(tmp)
            Next

            MsgBox(String.Format("{0} fields extracted.", Names.Count))

            w.WriteLine("' Auto generated file")
            w.WriteLine("' {0}", Now)
            w.WriteLine()

            Dim LastFieldType As String = ""
            Dim sName As String
            Dim sField As String
            Dim BasedOn As Integer = 0
            Dim BasedOnName As String = ""
            Dim EndNum As New Dictionary(Of String, Integer)
            For j As Integer = 0 To Info.Count - 1
                sName = ReadString(f, Info(j).Name - &H400000)
                If sName <> "" Then
                    sField = ToField(sName.Substring(0, sName.IndexOf("_")))
                    If sName = "OBJECT_FIELD_CREATED_BY" Then sField = "GameObject"

                    If LastFieldType <> sField Then
                        If LastFieldType <> "" Then
                            EndNum.Add(LastFieldType, Info(j - 1).Offset + 1)
                            If LastFieldType.ToLower = "object" Then
                                w.WriteLine("    {0,-78}", LastFieldType.ToUpper & "_END = &H" & Hex(Info(j - 1).Offset + Info(j - 1).Size))
                            Else
                                w.WriteLine("    {0,-78}' 0x{1:X3}", LastFieldType.ToUpper & "_END = " & BasedOnName & " + &H" & Hex(Info(j - 1).Offset + Info(j - 1).Size), BasedOn + Info(j - 1).Offset + Info(j - 1).Size)
                            End If
                            w.WriteLine("End Enum")
                        End If

                        w.WriteLine("Public Enum E" & sField & "Fields")

                        If sField.ToLower = "container" Then
                            BasedOn = EndNum("Item")
                            BasedOnName = "EItemFields.ITEM_END"
                        ElseIf sField.ToLower = "player" Then
                            BasedOn = EndNum("Unit")
                            BasedOnName = "EUnitFields.UNIT_END"
                        ElseIf sField.ToLower <> "object" Then
                            BasedOn = EndNum("Object")
                            BasedOnName = "EObjectFields.OBJECT_END"
                        End If

                        LastFieldType = sField
                    End If

                    If BasedOn > 0 Then
                        w.WriteLine("    {0,-78}' 0x{1:X3} - Size: {2} - Type: {3} - Flags: {4}", sName & " = " & BasedOnName & " + &H" & Hex(Info(j).Offset), BasedOn + Info(j).Offset, Info(j).Size, ToType(Info(j).Type), ToFlags(Info(j).Flags))
                    Else
                        w.WriteLine("    {0,-78}' 0x{1:X3} - Size: {2} - Type: {3} - Flags: {4}", sName & " = &H" & Hex(Info(j).Offset), Info(j).Offset, Info(j).Size, ToType(Info(j).Type), ToFlags(Info(j).Flags))
                    End If
                End If
            Next j
            If LastFieldType <> "" Then w.WriteLine("    {0,-78}' 0x{1:X3}", LastFieldType.ToUpper & "_END = " & BasedOnName & " + &H" & Hex(Info(Info.Count - 1).Offset + Info(Info.Count - 1).Size), BasedOn + Info(Info.Count - 1).Offset + Info(Info.Count - 1).Size)
            w.WriteLine("End Enum")
            w.Flush()

        End If
        o.Close()
        f.Close()
    End Sub
    Public Sub ExtractOpcodes()
        Dim f As New IO.FileStream("wow.exe", IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 10000000)
        Dim r1 As New IO.BinaryReader(f)
        Dim r2 As New IO.StreamReader(f)

        Dim o As New IO.FileStream("Global.Opcodes.vb", IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None, 1024)
        Dim w As New IO.StreamWriter(o)

        MsgBox(ReadString(f, SearchInFile(f, "CMSG_REQUEST_PARTY_MEMBER_STATS")))

        Dim START As Integer = SearchInFile(f, "NUM_MSG_TYPES")

        If START = -1 Then
            MsgBox("Wrong offsets!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
        Else
            Dim Names As New Stack(Of String)
            Dim Last As String = ""

            f.Seek(START, IO.SeekOrigin.Begin)
            While Last <> "MSG_NULL_ACTION"
                Last = ReadString(f)
                Names.Push(Last)
            End While

            MsgBox(String.Format("{0} opcodes extracted.", Names.Count))


            w.WriteLine("' Auto generated file")
            w.WriteLine("' {0}", Now)
            w.WriteLine()
            w.WriteLine("Public Enum OPCODES")

            Dim i As Integer = 0
            While Names.Count > 0
                w.WriteLine("    {0,-64}' 0x{1:X3}", Names.Pop & "=" & i, i)
                i += 1
            End While
            w.WriteLine("End Enum")
            w.Flush()

        End If
        o.Close()
        f.Close()
    End Sub


    Public Sub ExtractSpellFailedReason()
        Dim f As New IO.FileStream("wow.exe", IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 10000000)
        Dim r1 As New IO.BinaryReader(f)
        Dim r2 As New IO.StreamReader(f)

        Dim o As New IO.FileStream("Global.SpellFailedReasons.vb", IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None, 1024)
        Dim w As New IO.StreamWriter(o)

        Dim REASON_NAME_OFFSET As Integer = SearchInFile(f, "SPELL_FAILED_UNKNOWN")

        If REASON_NAME_OFFSET = -1 Then
            MsgBox("Wrong offsets!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
        Else
            Dim Names As New Stack(Of String)
            Dim Last As String = ""
            Dim Offset As Integer = REASON_NAME_OFFSET
            Dim i As Integer = 0
            f.Seek(Offset, IO.SeekOrigin.Begin)
            While Last.Length = 0 OrElse Last.Substring(0, 13) = "SPELL_FAILED_"
                Last = ReadString(f)
                If Last.Length > 13 AndAlso Last.Substring(0, 13) = "SPELL_FAILED_" Then Names.Push(Last)
            End While

            MsgBox(String.Format("{0} spell failed reasons extracted.", Names.Count))


            w.WriteLine("' Auto generated file")
            w.WriteLine("' {0}", Now)
            w.WriteLine()
            w.WriteLine("Public Enum SpellFailedReason As Byte")

            i = 0
            While Names.Count > 0
                w.WriteLine("    {0,-64}' 0x{1:X3}", Names.Pop & " = &H" & Hex(i), i)
                i += 1
            End While
            w.WriteLine("    {0,-64}' 0x{1:X3}", "SPELL_NO_ERROR = &H" & Hex(255), 255)
            w.WriteLine("End Enum")
            w.Flush()
        End If

        o.Close()
        f.Close()
    End Sub


    Public Sub ExtractChatTypes()
        Dim f As New IO.FileStream("wow.exe", IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 10000000)
        Dim r1 As New IO.BinaryReader(f)
        Dim r2 As New IO.StreamReader(f)

        Dim o As New IO.FileStream("Global.ChatTypes.vb", IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None, 1024)
        Dim w As New IO.StreamWriter(o)

        Dim START As Integer = SearchInFile(f, "CHAT_MSG_RAID_WARNING")

        If START = -1 Then
            MsgBox("Wrong offsets!", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
        Else
            Dim Names As New Stack(Of String)
            Dim Last As String = ""
            Dim Offset As Integer = START
            Dim i As Integer = 0
            f.Seek(Offset, IO.SeekOrigin.Begin)
            While Last.Length = 0 OrElse Last.Substring(0, 9) = "CHAT_MSG_"
                Last = ReadString(f)
                If Last.Length > 10 AndAlso Last.Substring(0, 9) = "CHAT_MSG_" Then Names.Push(Last)
            End While

            MsgBox(String.Format("{0} chat types extracted.", Names.Count))


            w.WriteLine("' Auto generated file")
            w.WriteLine("' {0}", Now)
            w.WriteLine()
            w.WriteLine("Public Enum ChatMsg As Integer")

            i = 0
            While Names.Count > 0
                w.WriteLine("    {0,-64}' 0x{1:X3}", Names.Pop & " = &H" & Hex(i), i)
                i += 1
            End While
            w.WriteLine("End Enum")
            w.Flush()
        End If

        o.Close()
        f.Close()
    End Sub


End Module