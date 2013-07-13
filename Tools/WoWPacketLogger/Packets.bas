Attribute VB_Name = "Packets"
'
' Copyright (C) 2009 vWoW <http://www.vanilla-wow.com/>
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

Type tPacketData
    Data() As Byte
End Type

Sub AddPacketHeader(data_array() As Byte, OPCode As Byte)
    ReDim data_array(0) As Byte
    data_array(0) = OPCode
End Sub

Sub InitPacket(data_array() As Byte, OPCode As Integer)
    ReDim data_array(1) As Byte
    data_array(0) = 0
    data_array(1) = 0
    AddPacketInt16 data_array, OPCode
End Sub

Sub AddPacketInt8(data_array() As Byte, iValue As Byte, Optional iPos As Long = -1)
    Dim Index As Integer
    If iPos < 0 Or iPos > (UBound(data_array) - 1) Then
        Index = UBound(data_array) + 1
        ReDim Preserve data_array(Index) As Byte
    Else
        Index = iPos
    End If
    data_array(Index) = iValue
End Sub

Sub AddPacketInt16(data_array() As Byte, iValue As Integer, Optional iPos As Long = -1)
    Dim Index As Integer
    If iPos < 0 Or iPos > (UBound(data_array) - 1) Then
        Index = UBound(data_array) + 1
        ReDim Preserve data_array(Index + 1) As Byte
    Else
        Index = iPos
    End If
    CopyMemory data_array(Index), iValue, 2
End Sub

Sub AddPacketInt32(data_array() As Byte, iValue As Long, Optional iPos As Long = -1)
    Dim Index As Integer
    If iPos < 0 Or iPos > (UBound(data_array) - 3) Then
        Index = UBound(data_array) + 1
        ReDim Preserve data_array(Index + 3) As Byte
    Else
        Index = iPos
    End If
    CopyMemory data_array(Index), iValue, 4
End Sub

Sub AddPacketFloat(data_array() As Byte, iValue As Single)
    Dim Index As Integer
    Index = UBound(data_array) + 1
    ReDim Preserve data_array(Index + 3) As Byte
    CopyMemory data_array(Index), iValue, 4
End Sub

Sub AddPacketString(data_array() As Byte, sValue As String, Optional EndZero As Boolean = True, Optional Reversed As Boolean = False)
    Dim Index As Integer, j As Integer
    Index = UBound(data_array) + 1
    
    If sValue = "" Then
        ReDim Preserve data_array(Index)
        data_array(Index) = 0
    Else
        If EndZero Then
            ReDim Preserve data_array(Index + Len(sValue))
        Else
            ReDim Preserve data_array(Index + Len(sValue) - 1)
        End If
        
        If Reversed Then
            For j = Len(sValue) - 1 To 0 Step -1
                data_array(Index) = Asc(Mid(sValue, j + 1, 1))
                Index = Index + 1
            Next j
        Else
            For j = 0 To Len(sValue) - 1
                data_array(Index) = Asc(Mid(sValue, j + 1, 1))
                Index = Index + 1
            Next j
        End If
        If EndZero Then data_array(Index) = 0
    End If
End Sub

Sub AddPacketByteArray(data_array() As Byte, byte_array() As Byte, Optional startOffset As Long = 0, Optional endOffset As Long = 0, Optional InsertAt As Long = -1)
    Dim Index As Integer, i As Integer, j As Integer
    If InsertAt = -1 Then
        Index = UBound(data_array) + 1
    Else
        Index = InsertAt
    End If
    
    If endOffset > UBound(byte_array) Then endOffset = UBound(byte_array)
    If endOffset = 0 Then endOffset = UBound(byte_array)
    If startOffset > endOffset Then startOffset = 0
    
    If (Index + (endOffset - startOffset)) > UBound(data_array) Then
        ReDim Preserve data_array(Index + ((endOffset - startOffset) - 1))
    End If
    
    j = 0
    For i = startOffset To endOffset
        If (Index + j) > UBound(data_array) Then ReDim Preserve data_array(Index + j)
        data_array(Index + j) = byte_array(i)
        j = j + 1
    Next i
End Sub

Sub FinishPacket(data_array() As Byte)
    Dim packet_len As Long

    packet_len = UBound(data_array)
    AddPacketInt16 data_array, packet_len - 2, 1
End Sub

Sub EndPacket(data_array() As Byte)
    Dim packet_len As Long, tmpByte As Byte
    
    packet_len = UBound(data_array)
    AddPacketInt16 data_array, packet_len - 1, 0
    tmpByte = data_array(1)
    data_array(1) = data_array(0)
    data_array(0) = tmpByte
End Sub

Function GetInt16FromPacket(data_array() As Byte, iPos As Long, Optional Mirror As Boolean = False) As Integer
    Dim tmpInt As Integer, tmpBytes(1) As Byte
    If Mirror Then
        tmpBytes(0) = data_array(iPos + 1)
        tmpBytes(1) = data_array(iPos)
    Else
        tmpBytes(0) = data_array(iPos)
        tmpBytes(1) = data_array(iPos + 1)
    End If
    CopyMemory tmpInt, tmpBytes(0), 2
    iPos = iPos + 2
    GetInt16FromPacket = tmpInt
End Function

Function GetInt32FromPacket(data_array() As Byte, iPos As Long) As Long
    Dim tmpLng As Long
    CopyMemory tmpLng, data_array(iPos), 4
    iPos = iPos + 4
    GetInt32FromPacket = tmpLng
End Function

Function GetFloatFromPacket(data_array() As Byte, iPos As Long) As Single
    Dim tmpSng As Single
    CopyMemory tmpSng, data_array(iPos), 4
    iPos = iPos + 4
    GetFloatFromPacket = tmpSng
End Function

Function GetStringFromPacket(data_array() As Byte, iPos As Long) As String
    Dim tmpLng As Long, i As Integer, tmpStr As String

    tmpStr = ""
    For i = iPos To UBound(data_array)
        If data_array(i) = 0 Then Exit For
        tmpStr = tmpStr & Chr(data_array(i))
    Next i
    iPos = iPos + Len(tmpStr) + 1
    
    GetStringFromPacket = tmpStr
End Function

Sub AddPacketToQueue(PacketData() As tPacketData, Data() As Byte)
    Dim Index As Long
    Index = UBound(PacketData)
    PacketData(Index).Data = Data
    ReDim Preserve PacketData(Index + 1)
End Sub
