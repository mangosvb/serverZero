'
' Copyright (C) 2013 - 2014 getMaNGOS <http://www.getmangos.eu>
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
Public Module Converter

#Region "ToByte|ToBytes"
    Public Function ToByte(ByVal d As Byte(), ByRef offset As Integer) As Byte
        Return d(offset + 1)
    End Function
    'Byte
    Public Sub ToBytes(ByVal a As Byte, ByVal b As Byte(), ByRef t As Integer)
        b(t) = a
        t = t + 1
    End Sub
    'Float
    Public Sub ToBytes(ByVal a As Double, ByVal b As Byte(), ByRef t As Integer)
        Dim buffer1 As Byte() = BitConverter.GetBytes(a)
        Buffer.BlockCopy(buffer1, 0, b, t, buffer1.Length)
        t = (t + buffer1.Length)
    End Sub
    Public Sub ToBytes(ByVal a As Single, ByVal b As Byte(), ByRef t As Integer)
        Dim buffer1 As Byte() = BitConverter.GetBytes(a)
        Buffer.BlockCopy(buffer1, 0, b, t, buffer1.Length)
        t = (t + buffer1.Length)
    End Sub
    'Int16
    Public Sub ToBytes(ByVal a As Short, ByVal b As Byte(), ByRef t As Integer)
        b(t) = CType((a And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 8) And 255), Byte)
        t = t + 1
    End Sub
    'Int32
    Public Sub ToBytes(ByVal a As Integer, ByVal b As Byte(), ByRef t As Integer)
        b(t) = CType((a And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 8) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 16) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 24) And 255), Byte)
        t = t + 1
    End Sub
    'Int64
    Public Sub ToBytes(ByVal a As Long, ByVal b As Byte(), ByRef t As Integer)
        b(t) = CType((a And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 8) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 16) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 24) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 32) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 40) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 48) And 255), Byte)
        t = t + 1
        b(t) = CType(((a >> 56) And 255), Byte)
        t = t + 1
    End Sub
    'String
    Public Sub ToBytes(ByVal a As String, ByVal b As Byte(), ByRef t As Integer)
        Dim chArray1 As Char() = a.ToCharArray
        Dim chArray2 As Char() = chArray1
        Dim num1 As Integer
        For num1 = 0 To chArray2.Length - 1
            b(t) = CType(Asc(chArray2(num1)), Byte)
            t = t + 1
        Next num1
    End Sub
#End Region

#Region "ToDouble"
    Public Function ToDouble(ByVal d As Byte(), ByRef offset As Integer) As Double
        Dim num1 As Double = BitConverter.ToDouble(d, offset)
        offset = (offset + 8)
        Return num1
    End Function
#End Region

#Region "ToFloat"
    Public Function ToFloat(ByVal d As Byte(), ByRef offset As Integer) As Single
        Dim single1 As Single = BitConverter.ToSingle(d, offset)
        offset = (offset + 4)
        Return single1
    End Function
#End Region

#Region "ToInt16, ToUint16"
    Public Function ToInt16(ByVal d As Byte(), ByRef offset As Integer) As Short
        Dim num1 As Short = BitConverter.ToInt16(d, offset)
        offset = (offset + 2)
        Return num1
    End Function
    Public Function ToUInt16(ByVal d As Byte(), ByRef offset As Integer) As UInt16
        Dim num1 As UInt16 = BitConverter.ToUInt16(d, offset)
        offset = (offset + 2)
        Return num1
    End Function
#End Region

#Region "ToInt32, ToUint32"
    Public Function ToInt32(ByVal d As Byte(), ByRef offset As Integer) As Integer
        Dim num1 As Integer = BitConverter.ToInt32(d, offset)
        offset = (offset + 4)
        Return num1
    End Function
    Public Function ToUInt32(ByVal d As Byte(), ByRef offset As Integer) As UInt32
        Dim num1 As UInt32 = BitConverter.ToUInt32(d, offset)
        offset = (offset + 4)
        Return num1
    End Function
#End Region

#Region "ToInt64, ToUint64"
    Public Function ToInt64(ByVal d As Byte(), ByRef offset As Integer) As Long
        Dim num1 As Long = BitConverter.ToInt64(d, offset)
        offset = (offset + 8)
        Return num1
    End Function
    Public Function ToUInt64(ByVal d As Byte(), ByRef offset As Integer) As UInt64
        Dim num1 As UInt64 = BitConverter.ToUInt64(d, offset)
        offset = (offset + 8)
        Return num1
    End Function
#End Region

End Module
