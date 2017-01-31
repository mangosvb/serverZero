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

Public Module Packets

    Public Sub DumpPacket(ByVal data() As Byte)
        '#If DEBUG Then
        Dim j As Integer
        Dim buffer As String = ""
        Try
            buffer = buffer + [String].Format("DEBUG: Packet Dump - Length={0}{1}", data.Length, vbNewLine)

            If data.Length Mod 16 = 0 Then
                For j = 0 To data.Length - 1 Step 16
                    buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                    buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
                Next
            Else
                For j = 0 To data.Length - 1 - 16 Step 16
                    buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                    buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
                Next

                buffer += "|  " & BitConverter.ToString(data, j, data.Length Mod 16).Replace("-", " ")
                buffer += New String(" ", (16 - data.Length Mod 16) * 3)
                buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, data.Length Mod 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?")
                buffer += New String(" ", 16 - data.Length Mod 16)
                buffer += " |" & vbNewLine
            End If

            Console.WriteLine(buffer)
            '#End If
        Catch e As Exception
            ConsoleColor.SetConsoleColor(Global_System.ConsoleColorClass.ForegroundColors.Red)
            Console.WriteLine("Error dumping packet: {0}{1}", vbNewLine, e.ToString)
            ConsoleColor.SetConsoleColor()
        End Try
    End Sub

    Public Class PacketClass
        Implements IDisposable

        Public Data() As Byte
        Public Offset As Integer = 4
        Private Realm As Boolean = False

        Public ReadOnly Property Length() As Integer
            Get
                If Realm Then
                    Return (Data(1) + (Data(2) * 256))
                Else
                    Return (Data(1) + (Data(0) * 256))
                End If
            End Get
        End Property
        Public ReadOnly Property OpCode() As Integer
            Get
                If Realm Then
                    Return Data(0)
                Else
                    Return (Data(2) + (Data(3) * 256))
                End If
            End Get
        End Property

        Public Sub New(ByVal opcode As OPCODES)
            ReDim Preserve Data(5)
            Data(0) = 0
            Data(1) = 4
            Data(2) = CType(opcode, Int16) Mod 256
            Data(3) = CType(opcode, Int16) \ 256
            Data(4) = 0
            Data(5) = 0
        End Sub
        Public Sub New(ByVal opcode As Byte)
            ReDim Preserve Data(0)
            Data(0) = opcode
            Realm = True
        End Sub
        Public Sub New(ByRef rawdata() As Byte, Optional ByVal Realm_ As Boolean = False)
            Data = rawdata
            Realm = Realm_
        End Sub

        Public Sub AddBitArray(ByVal buffer As BitArray, ByVal Len As Integer)
            ReDim Preserve Data(Data.Length - 1 + Len)
            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            Dim bufferarray(CType((buffer.Length + 8) / 8, Byte)) As Byte

            buffer.CopyTo(bufferarray, 0)
            Array.Copy(bufferarray, 0, Data, Data.Length - Len, Len)
        End Sub
        Public Sub AddInt8(ByVal buffer As Byte)
            ReDim Preserve Data(Data.Length)
            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If
            Data(Data.Length - 1) = buffer
        End Sub
        Public Sub AddInt16(ByVal buffer As Short)
            ReDim Preserve Data(Data.Length + 1)
            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            Data(Data.Length - 2) = CType((buffer And 255), Byte)
            Data(Data.Length - 1) = CType(((buffer >> 8) And 255), Byte)
        End Sub
        Public Sub AddInt32(ByVal buffer As Integer, Optional ByVal position As Integer = 0)
            If position <= 0 OrElse position > (Data.Length - 3) Then
                position = Data.Length
                ReDim Preserve Data(Data.Length + 3)
                If Realm = False Then
                    Data(0) = (Data.Length - 2) \ 256
                    Data(1) = (Data.Length - 2) Mod 256
                End If
            End If

            Data(position) = CType((buffer And 255), Byte)
            Data(position + 1) = CType(((buffer >> 8) And 255), Byte)
            Data(position + 2) = CType(((buffer >> 16) And 255), Byte)
            Data(position + 3) = CType(((buffer >> 24) And 255), Byte)
        End Sub
        Public Sub AddInt64(ByVal buffer As Long)
            ReDim Preserve Data(Data.Length + 7)
            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            Data(Data.Length - 8) = CType((buffer And 255), Byte)
            Data(Data.Length - 7) = CType(((buffer >> 8) And 255), Byte)
            Data(Data.Length - 6) = CType(((buffer >> 16) And 255), Byte)
            Data(Data.Length - 5) = CType(((buffer >> 24) And 255), Byte)
            Data(Data.Length - 4) = CType(((buffer >> 32) And 255), Byte)
            Data(Data.Length - 3) = CType(((buffer >> 40) And 255), Byte)
            Data(Data.Length - 2) = CType(((buffer >> 48) And 255), Byte)
            Data(Data.Length - 1) = CType(((buffer >> 56) And 255), Byte)
        End Sub
        Public Sub AddString(ByVal buffer As String, Optional ByVal EndZero As Boolean = True, Optional ByVal Reversed As Boolean = False)
            If IsDBNull(buffer) Or buffer = "" Then
                Me.AddInt8(0)
            Else
                Dim Bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(buffer.ToCharArray)

                Dim Position As Integer = Data.Length
                If EndZero Then
                    ReDim Preserve Data(Data.Length + Bytes.Length)
                Else
                    ReDim Preserve Data(Data.Length + Bytes.Length - 1)
                End If
                If Realm = False Then
                    Data(0) = (Data.Length - 2) \ 256
                    Data(1) = (Data.Length - 2) Mod 256
                End If

                Dim i As Integer
                If Reversed Then
                    For i = Bytes.Length - 1 To 0 Step -1
                        Data(Position) = Bytes(i)
                        Position += 1
                    Next i
                Else
                    For i = 0 To Bytes.Length - 1
                        Data(Position) = Bytes(i)
                        Position += 1
                    Next i
                End If

                If EndZero Then Data(Position) = 0
                End If
        End Sub
        Public Sub AddDouble(ByVal buffer2 As Double)
            Dim buffer1 As Byte() = BitConverter.GetBytes(buffer2)
            ReDim Preserve Data(Data.Length + buffer1.Length - 1)
            Buffer.BlockCopy(buffer1, 0, Data, Data.Length - buffer1.Length, buffer1.Length)

            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If
        End Sub
        Public Sub AddSingle(ByVal buffer2 As Single)
            Dim buffer1 As Byte() = BitConverter.GetBytes(buffer2)
            ReDim Preserve Data(Data.Length + buffer1.Length - 1)
            Buffer.BlockCopy(buffer1, 0, Data, Data.Length - buffer1.Length, buffer1.Length)

            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If
        End Sub
        Public Sub AddByteArray(ByVal buffer() As Byte)
            Dim tmp As Integer = Data.Length
            ReDim Preserve Data(Data.Length + buffer.Length - 1)
            Array.Copy(buffer, 0, Data, tmp, buffer.Length)

            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If
        End Sub
        Public Sub AddPackGUID(ByVal buffer As ULong)
            Dim GUID() As Byte = BitConverter.GetBytes(buffer)
            Dim flags As New BitArray(8)
            Dim offsetStart As Integer = Data.Length
            Dim offsetNewSize As Integer = offsetStart
            Dim i As Byte

            For i = 0 To 7
                flags(i) = (GUID(i) <> 0)
                If flags(i) Then offsetNewSize += 1
            Next

            ReDim Preserve Data(offsetNewSize)

            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            flags.CopyTo(Data, offsetStart)
            offsetStart += 1

            For i = 0 To 7
                If flags(i) Then
                    Data(offsetStart) = GUID(i)
                    offsetStart += 1
                End If
            Next
        End Sub

        Public Sub AddUInt16(ByVal buffer As UShort, Optional ByVal Position As Integer = 0)
            If Position = 0 Then
                Position = Data.Length
                ReDim Preserve Data(Data.Length + 1)
            End If

            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            Data(Position) = CType((buffer And 255), Byte)
            Data(Position + 1) = CType(((buffer >> 8) And 255), Byte)
        End Sub

        Public Sub AddUInt32(ByVal buffer As UInteger)
            ReDim Preserve Data(Data.Length + 3)
            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            Data(Data.Length - 4) = CType((buffer And 255), Byte)
            Data(Data.Length - 3) = CType(((buffer >> 8) And 255), Byte)
            Data(Data.Length - 2) = CType(((buffer >> 16) And 255), Byte)
            Data(Data.Length - 1) = CType(((buffer >> 24) And 255), Byte)
        End Sub

        Public Sub AddUInt64(ByVal buffer As ULong)
            ReDim Preserve Data(Data.Length + 7)
            If Realm = False Then
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            Data(Data.Length - 8) = CType((buffer And 255), Byte)
            Data(Data.Length - 7) = CType(((buffer >> 8) And 255), Byte)
            Data(Data.Length - 6) = CType(((buffer >> 16) And 255), Byte)
            Data(Data.Length - 5) = CType(((buffer >> 24) And 255), Byte)
            Data(Data.Length - 4) = CType(((buffer >> 32) And 255), Byte)
            Data(Data.Length - 3) = CType(((buffer >> 40) And 255), Byte)
            Data(Data.Length - 2) = CType(((buffer >> 48) And 255), Byte)
            Data(Data.Length - 1) = CType(((buffer >> 56) And 255), Byte)
        End Sub

        Public Function GetInt8() As Byte
            Offset = Offset + 1
            Return Data(Offset - 1)
        End Function
        Public Function GetInt8(ByVal Offset As Integer) As Byte
            Offset = Offset + 1
            Return Data(Offset - 1)
        End Function
        Public Function GetInt16() As Short
            Dim num1 As Short = BitConverter.ToInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function
        Public Function GetInt16(ByVal Offset As Integer) As Short
            Dim num1 As Short = BitConverter.ToInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function
        Public Function GetInt32() As Integer
            Dim num1 As Integer = BitConverter.ToInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function
        Public Function GetInt32(ByVal Offset As Integer) As Integer
            Dim num1 As Integer = BitConverter.ToInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function
        Public Function GetInt64() As Long
            Dim num1 As Long = BitConverter.ToInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        Public Function GetInt64(ByVal Offset As Integer) As Long
            Dim num1 As Long = BitConverter.ToInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        Public Function GetFloat() As Single
            Dim single1 As Single = BitConverter.ToSingle(Data, Offset)
            Offset = (Offset + 4)
            Return single1
        End Function
        Public Function GetFloat(ByVal Offset_ As Integer) As Single
            Dim single1 As Single = BitConverter.ToSingle(Data, Offset)
            Offset = (Offset_ + 4)
            Return single1
        End Function
        Public Function GetDouble() As Double
            Dim num1 As Double = BitConverter.ToDouble(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        Public Function GetDouble(ByVal Offset As Integer) As Double
            Dim num1 As Double = BitConverter.ToDouble(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        Public Function GetString() As String
            Dim start As Integer = Offset
            Dim i As Integer = 0

            While Data(start + i) <> 0
                i = i + 1
                Offset = Offset + 1
            End While
            Offset = Offset + 1

            Return System.Text.Encoding.UTF8.GetString(Data, start, i)
        End Function
        Public Function GetString(ByVal Offset As Integer) As String
            Dim i As Integer = Offset
            Dim tmpString As String = ""
            While Data(i) <> 0
                tmpString = tmpString + Chr(Data(i))
                i = i + 1
                Offset = Offset + 1
            End While
            Offset = Offset + 1
            Return tmpString
        End Function

        Public Function GetUInt16() As UShort
            Dim num1 As UShort = BitConverter.ToUInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function
        Public Function GetUInt16(ByVal Offset As Integer) As UShort
            Dim num1 As UShort = BitConverter.ToUInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function
        Public Function GetUInt32() As UInteger
            Dim num1 As UInteger = BitConverter.ToUInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function
        Public Function GetUInt32(ByVal Offset As Integer) As UInteger
            Dim num1 As UInteger = BitConverter.ToUInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function
        Public Function GetUInt64() As ULong
            Dim num1 As ULong = BitConverter.ToUInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        Public Function GetUInt64(ByVal Offset As Integer) As ULong
            Dim num1 As ULong = BitConverter.ToUInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function

        Public Function GetPackGUID() As ULong
            Dim flags As Byte = Data(Offset)
            Dim GUID() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
            Offset += 1

            If (flags And 1) = 1 Then
                GUID(0) = Data(Offset)
                Offset += 1
            End If
            If (flags And 2) = 2 Then
                GUID(1) = Data(Offset)
                Offset += 1
            End If
            If (flags And 4) = 4 Then
                GUID(2) = Data(Offset)
                Offset += 1
            End If
            If (flags And 8) = 8 Then
                GUID(3) = Data(Offset)
                Offset += 1
            End If
            If (flags And 16) = 16 Then
                GUID(4) = Data(Offset)
                Offset += 1
            End If
            If (flags And 32) = 32 Then
                GUID(5) = Data(Offset)
                Offset += 1
            End If
            If (flags And 64) = 64 Then
                GUID(6) = Data(Offset)
                Offset += 1
            End If
            If (flags And 128) = 128 Then
                GUID(7) = Data(Offset)
                Offset += 1
            End If

            Return CType(BitConverter.ToUInt64(GUID, 0), ULong)
        End Function
        Public Function GetPackGUID(ByVal Offset As Integer) As ULong
            Dim flags As Byte = Data(Offset)
            Dim GUID() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
            Offset += 1

            If (flags And 1) = 1 Then
                GUID(0) = Data(Offset)
                Offset += 1
            End If
            If (flags And 2) = 2 Then
                GUID(1) = Data(Offset)
                Offset += 1
            End If
            If (flags And 4) = 4 Then
                GUID(2) = Data(Offset)
                Offset += 1
            End If
            If (flags And 8) = 8 Then
                GUID(3) = Data(Offset)
                Offset += 1
            End If
            If (flags And 16) = 16 Then
                GUID(4) = Data(Offset)
                Offset += 1
            End If
            If (flags And 32) = 32 Then
                GUID(5) = Data(Offset)
                Offset += 1
            End If
            If (flags And 64) = 64 Then
                GUID(6) = Data(Offset)
                Offset += 1
            End If
            If (flags And 128) = 128 Then
                GUID(7) = Data(Offset)
                Offset += 1
            End If

            Return CType(BitConverter.ToUInt64(GUID, 0), ULong)
        End Function

        Public Function GetByteArray(ByVal Length As Integer) As Byte()
            If Length < 0 Then Return New Byte() {}
            Dim tmpArray() As Byte = New Byte(Length - 1) {}
            Array.Copy(Data, Offset, tmpArray, 0, Length)
            Offset += Length
            Return tmpArray
        End Function


        Public Sub Dispose() Implements System.IDisposable.Dispose
        End Sub
    End Class


End Module
