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
Imports mangosVB.Common
Imports mangosVB.Common.Globals

Public Module Packets

    Public Sub DumpPacket(ByVal data() As Byte, Optional ByRef client As ClientClass = Nothing)
        '#If DEBUG Then
        Dim j As Integer
        Dim buffer As String = ""
        Try
            If Client Is Nothing Then
                buffer = buffer + String.Format("DEBUG: Packet Dump{0}", vbNewLine)
            Else
                buffer = buffer + String.Format("[{0}:{1}] DEBUG: Packet Dump - Length={2}{3}", client.IP, client.Port, data.Length, vbNewLine)
            End If

            If data.Length Mod 16 = 0 Then
                For j = 0 To data.Length - 1 Step 16
                    buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                    buffer += " |  " & Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
                Next
            Else
                For j = 0 To data.Length - 1 - 16 Step 16
                    buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                    buffer += " |  " & Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
                Next

                buffer += "|  " & BitConverter.ToString(data, j, data.Length Mod 16).Replace("-", " ")
                buffer += New String(" ", (16 - data.Length Mod 16) * 3)
                buffer += " |  " & Text.Encoding.ASCII.GetString(data, j, data.Length Mod 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?")
                buffer += New String(" ", 16 - data.Length Mod 16)
                buffer += " |" & vbNewLine
            End If

            Log.WriteLine(LogType.DEBUG, buffer, Nothing)
            '#End If
        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Error dumping packet: {0}{1}", vbNewLine, e.ToString)
        End Try
    End Sub

    Public Sub LogPacket(ByVal data() As Byte, ByVal Server As Boolean, Optional ByRef client As ClientClass = Nothing)
        Dim j As Integer
        Dim buffer As String = ""
        Try
            Dim opcode As OPCODES = BitConverter.ToInt16(data, 2)
            If IgnorePacket(opcode) Then Exit Sub
            Dim StartAt As Integer = 6
            If Server Then StartAt = 4

            Dim TypeStr As String = "IN"
            If Server Then TypeStr = "OUT"
            If client Is Nothing Then
                buffer = buffer + String.Format("{4} Packet: (0x{0:X4}) {1} PacketSize = {2}{3}", CInt(opcode), opcode, data.Length - StartAt, vbNewLine, TypeStr)
            Else
                buffer = buffer + String.Format("[{0}:{1}] {6} Packet: (0x{2:X4}) {3} PacketSize = {4}{5}", client.IP, client.Port, CInt(opcode), opcode, data.Length - StartAt, vbNewLine, TypeStr)
            End If

            buffer += "|------------------------------------------------|----------------|" & vbNewLine
            buffer += "|00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F |0123456789ABCDEF|" & vbNewLine
            buffer += "|------------------------------------------------|----------------|" & vbNewLine
            For j = StartAt To data.Length - 1 Step 16
                If (j + 16 > data.Length) Then
                    buffer += "|" & BitConverter.ToString(data, j, data.Length - j).Replace("-", " ")
                    buffer += New String(" ", ((j + 16) - data.Length) * 3)
                    buffer += " |" & FormatPacketStr(Text.Encoding.ASCII.GetString(data, j, data.Length - j))
                    buffer += New String(" ", ((j + 16) - data.Length))
                Else
                    buffer += "|" & BitConverter.ToString(data, j, 16).Replace("-", " ")
                    buffer += " |" & FormatPacketStr(Text.Encoding.ASCII.GetString(data, j, 16))
                End If
                buffer += "|" & vbNewLine
            Next
            buffer += "-------------------------------------------------------------------" & vbNewLine & vbNewLine

            File.AppendAllText("packets.log", buffer)
        Catch e As Exception
        End Try
    End Sub

    Private Function IgnorePacket(ByVal opcode As OPCODES) As Boolean
        Dim OpcodeName As String = String.Format("{0}", opcode)
        If OpcodeName.StartsWith("MSG_MOVE") Then Return True
        Select Case opcode
            Case OPCODES.SMSG_MONSTER_MOVE, OPCODES.SMSG_UPDATE_OBJECT
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Function FormatPacketStr(ByVal str As String) As String
        Dim tmpChar() As Char = str.ToCharArray
        Dim tmpStr As String = ""
        For i As Integer = 0 To tmpChar.Length - 1
            If tmpChar(i) < "A"c OrElse tmpChar(i) > "z"c Then
                tmpChar(i) = "."c
            End If
        Next i
        Return tmpChar
    End Function

    Public Class PacketClass
        Implements IDisposable

        Public Data() As Byte
        Public Offset As Integer = 4

        Public ReadOnly Property Length() As Integer
            Get
                Return (Data(1) + (Data(0) * 256))
            End Get
        End Property
        Public ReadOnly Property OpCode() As OPCODES
            Get
                Return (Data(2) + (Data(3) * 256))
            End Get
        End Property

        Public Sub New(ByVal opcode As OPCODES)
            ReDim Preserve Data(3)
            Data(0) = 0
            Data(1) = 2
            Data(2) = CType(opcode, Short) Mod 256
            Data(3) = CType(opcode, Short) \ 256
        End Sub
        Public Sub New(ByRef rawdata() As Byte)
            Data = rawdata
        End Sub

        'Public Sub AddBitArray(ByVal buffer As BitArray, ByVal Len As Integer)
        '    ReDim Preserve Data(Data.Length - 1 + Len)
        '    Data(0) = (Data.Length - 2) \ 256
        '    Data(1) = (Data.Length - 2) Mod 256

        '    Dim bufferarray(CType((buffer.Length + 8) / 8, Byte)) As Byte

        '    buffer.CopyTo(bufferarray, 0)
        '    Array.Copy(bufferarray, 0, Data, Data.Length - Len, Len)
        'End Sub
        Public Sub AddInt8(ByVal buffer As Byte)
            ReDim Preserve Data(Data.Length)
            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256
            Data(Data.Length - 1) = buffer
        End Sub
        Public Sub AddInt16(ByVal buffer As Short)
            ReDim Preserve Data(Data.Length + 1)
            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256

            Data(Data.Length - 2) = buffer And 255
            Data(Data.Length - 1) = (buffer >> 8) And 255
        End Sub
        Public Sub AddInt32(ByVal buffer As Integer, Optional ByVal position As Integer = 0)
            If position <= 0 OrElse position > (Data.Length - 3) Then
                position = Data.Length
                ReDim Preserve Data(Data.Length + 3)
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256
            End If

            Data(position) = buffer And 255
            Data(position + 1) = (buffer >> 8) And 255
            Data(position + 2) = (buffer >> 16) And 255
            Data(position + 3) = (buffer >> 24) And 255
        End Sub
        Public Sub AddInt64(ByVal buffer As Long)
            ReDim Preserve Data(Data.Length + 7)
            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256

            Data(Data.Length - 8) = buffer And 255
            Data(Data.Length - 7) = (buffer >> 8) And 255
            Data(Data.Length - 6) = (buffer >> 16) And 255
            Data(Data.Length - 5) = (buffer >> 24) And 255
            Data(Data.Length - 4) = (buffer >> 32) And 255
            Data(Data.Length - 3) = (buffer >> 40) And 255
            Data(Data.Length - 2) = (buffer >> 48) And 255
            Data(Data.Length - 1) = (buffer >> 56) And 255
        End Sub
        Public Sub AddString(ByVal buffer As String)
            If IsDBNull(buffer) Or buffer = "" Then
                AddInt8(0)
            Else
                Dim Bytes As Byte() = Text.Encoding.UTF8.GetBytes(buffer.ToCharArray)

                ReDim Preserve Data(Data.Length + Bytes.Length)
                Data(0) = (Data.Length - 2) \ 256
                Data(1) = (Data.Length - 2) Mod 256

                For i As Integer = 0 To Bytes.Length - 1
                    Data(Data.Length - 1 - Bytes.Length + i) = Bytes(i)
                Next i

                Data(Data.Length - 1) = 0
            End If
        End Sub
        Public Sub AddDouble(ByVal buffer2 As Double)
            Dim buffer1 As Byte() = BitConverter.GetBytes(buffer2)
            ReDim Preserve Data(Data.Length + buffer1.Length - 1)
            Buffer.BlockCopy(buffer1, 0, Data, Data.Length - buffer1.Length, buffer1.Length)

            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256
        End Sub
        Public Sub AddSingle(ByVal buffer2 As Single)
            Dim buffer1 As Byte() = BitConverter.GetBytes(buffer2)
            ReDim Preserve Data(Data.Length + buffer1.Length - 1)
            Buffer.BlockCopy(buffer1, 0, Data, Data.Length - buffer1.Length, buffer1.Length)

            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256
        End Sub
        Public Sub AddByteArray(ByVal buffer() As Byte)
            Dim tmp As Integer = Data.Length
            ReDim Preserve Data(Data.Length + buffer.Length - 1)
            Array.Copy(buffer, 0, Data, tmp, buffer.Length)

            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256
        End Sub
        Public Sub AddPackGUID(ByVal buffer As ULong)
            Dim GUID() As Byte = BitConverter.GetBytes(buffer)
            Dim flags As New BitArray(8)
            Dim offsetStart As Integer = Data.Length
            Dim offsetNewSize As Integer = offsetStart

            For i As Byte = 0 To 7
                flags(i) = (GUID(i) <> 0)
                If flags(i) Then offsetNewSize += 1
            Next

            ReDim Preserve Data(offsetNewSize)

            flags.CopyTo(Data, offsetStart)
            offsetStart += 1

            For i As Byte = 0 To 7
                If flags(i) Then
                    Data(offsetStart) = GUID(i)
                    offsetStart += 1
                End If
            Next
        End Sub

        'Public Sub AddUInt8(ByVal buffer As Byte)
        'ReDim Preserve Data(Data.Length + 1)
        '
        '   Data(Data.Length - 1) = CType(((buffer >> 8) And 255), Byte)
        'End Sub

        Public Function GetUInt8() As UShort
            Dim num1 As UShort = (Data.Length + 1)
            Offset = (Offset + 1)
            Return num1
        End Function

        Public Sub AddUInt16(ByVal buffer As UShort)
            ReDim Preserve Data(Data.Length + 1)
            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256

            Data(Data.Length - 2) = buffer And 255
            Data(Data.Length - 1) = (buffer >> 8) And 255
        End Sub

        Public Sub AddUInt32(ByVal buffer As UInteger)
            ReDim Preserve Data(Data.Length + 3)
            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256

            Data(Data.Length - 4) = buffer And 255
            Data(Data.Length - 3) = (buffer >> 8) And 255
            Data(Data.Length - 2) = (buffer >> 16) And 255
            Data(Data.Length - 1) = (buffer >> 24) And 255
        End Sub

        Public Sub AddUInt64(ByVal buffer As ULong)
            ReDim Preserve Data(Data.Length + 7)
            Data(0) = (Data.Length - 2) \ 256
            Data(1) = (Data.Length - 2) Mod 256

            Data(Data.Length - 8) = buffer And 255
            Data(Data.Length - 7) = (buffer >> 8) And 255
            Data(Data.Length - 6) = (buffer >> 16) And 255
            Data(Data.Length - 5) = (buffer >> 24) And 255
            Data(Data.Length - 4) = (buffer >> 32) And 255
            Data(Data.Length - 3) = (buffer >> 40) And 255
            Data(Data.Length - 2) = (buffer >> 48) And 255
            Data(Data.Length - 1) = (buffer >> 56) And 255
        End Sub

        Public Function GetInt8() As Byte
            Offset = Offset + 1
            Return Data(Offset - 1)
        End Function
        'Public Function GetInt8(ByVal Offset As Integer) As Byte
        '    Offset = Offset + 1
        '    Return Data(Offset - 1)
        'End Function
        Public Function GetInt16() As Short
            Dim num1 As Short = BitConverter.ToInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function
        'Public Function GetInt16(ByVal Offset As Integer) As Short
        '    Dim num1 As Short = BitConverter.ToInt16(Data, Offset)
        '    Offset = (Offset + 2)
        '    Return num1
        'End Function
        Public Function GetInt32() As Integer
            Dim num1 As Integer = BitConverter.ToInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function
        'Public Function GetInt32(ByVal Offset As Integer) As Integer
        '    Dim num1 As Integer = BitConverter.ToInt32(Data, Offset)
        '    Offset = (Offset + 4)
        '    Return num1
        'End Function
        Public Function GetInt64() As Long
            Dim num1 As Long = BitConverter.ToInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        'Public Function GetInt64(ByVal Offset As Integer) As Long
        '    Dim num1 As Long = BitConverter.ToInt64(Data, Offset)
        '    Offset = (Offset + 8)
        '    Return num1
        'End Function
        Public Function GetFloat() As Single
            Dim single1 As Single = BitConverter.ToSingle(Data, Offset)
            Offset = (Offset + 4)
            Return single1
        End Function
        'Public Function GetFloat(ByVal Offset_ As Integer) As Single
        '    Dim single1 As Single = BitConverter.ToSingle(Data, Offset)
        '    Offset = (Offset_ + 4)
        '    Return single1
        'End Function
        Public Function GetDouble() As Double
            Dim num1 As Double = BitConverter.ToDouble(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        'Public Function GetDouble(ByVal Offset As Integer) As Double
        '    Dim num1 As Double = BitConverter.ToDouble(Data, Offset)
        '    Offset = (Offset + 8)
        '    Return num1
        'End Function
        Public Function GetString() As String
            Dim start As Integer = Offset
            Dim i As Integer = 0

            While Data(start + i) <> 0
                i = i + 1
                Offset = Offset + 1
            End While
            Offset = Offset + 1

            Return Text.Encoding.UTF8.GetString(Data, start, i)
        End Function
        'Public Function GetString(ByVal Offset As Integer) As String
        '    Dim i As Integer = Offset
        '    Dim tmpString As String = ""
        '    While Data(i) <> 0
        '        tmpString = tmpString + Chr(Data(i))
        '        i = i + 1
        '        Offset = Offset + 1
        '    End While
        '    Offset = Offset + 1
        '    Return tmpString
        'End Function

        Public Function GetUInt16() As UShort
            Dim num1 As UShort = BitConverter.ToUInt16(Data, Offset)
            Offset = (Offset + 2)
            Return num1
        End Function
        'Public Function GetUInt16(ByVal Offset As Integer) As UShort
        '    Dim num1 As UShort = BitConverter.ToUInt16(Data, Offset)
        '    Offset = (Offset + 2)
        '    Return num1
        'End Function
        Public Function GetUInt32() As UInteger
            Dim num1 As UInteger = BitConverter.ToUInt32(Data, Offset)
            Offset = (Offset + 4)
            Return num1
        End Function
        'Public Function GetUInt32(ByVal Offset As Integer) As UInteger
        '    Dim num1 As UInteger = BitConverter.ToUInt32(Data, Offset)
        '    Offset = (Offset + 4)
        '    Return num1
        'End Function
        Public Function GetUInt64() As ULong
            Dim num1 As ULong = BitConverter.ToUInt64(Data, Offset)
            Offset = (Offset + 8)
            Return num1
        End Function
        'Public Function GetUInt64(ByVal Offset As Integer) As ULong
        '    Dim num1 As ULong = BitConverter.ToUInt64(Data, Offset)
        '    Offset = (Offset + 8)
        '    Return num1
        'End Function

        'Public Function GetPackGUID() As ULong
        '    Dim flags As Byte = Data(Offset)
        '    Dim GUID() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
        '    Offset += 1

        '    If (flags And 1) = 1 Then
        '        GUID(0) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 2) = 2 Then
        '        GUID(1) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 4) = 4 Then
        '        GUID(2) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 8) = 8 Then
        '        GUID(3) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 16) = 16 Then
        '        GUID(4) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 32) = 32 Then
        '        GUID(5) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 64) = 64 Then
        '        GUID(6) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 128) = 128 Then
        '        GUID(7) = Data(Offset)
        '        Offset += 1
        '    End If

        '    Return CType(BitConverter.ToUInt64(GUID, 0), ULong)
        'End Function
        'Public Function GetPackGUID(ByVal Offset As Integer) As ULong
        '    Dim flags As Byte = Data(Offset)
        '    Dim GUID() As Byte = {0, 0, 0, 0, 0, 0, 0, 0}
        '    Offset += 1

        '    If (flags And 1) = 1 Then
        '        GUID(0) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 2) = 2 Then
        '        GUID(1) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 4) = 4 Then
        '        GUID(2) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 8) = 8 Then
        '        GUID(3) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 16) = 16 Then
        '        GUID(4) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 32) = 32 Then
        '        GUID(5) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 64) = 64 Then
        '        GUID(6) = Data(Offset)
        '        Offset += 1
        '    End If
        '    If (flags And 128) = 128 Then
        '        GUID(7) = Data(Offset)
        '        Offset += 1
        '    End If

        '    Return CType(BitConverter.ToUInt64(GUID, 0), ULong)
        'End Function

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
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
End Module