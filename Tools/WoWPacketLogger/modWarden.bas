Attribute VB_Name = "modWarden"
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

Public KeyIn() As Byte
Public KeyOut() As Byte

Private index As Long
Private source1() As Byte
Private source2() As Byte
Private data() As Byte

Public Sub InitWarden()
    Dim seedOut() As Byte
    Dim seedIn() As Byte
    
    NewKey SS_Hash
    
    seedOut = GetBytes(16)
    seedIn = GetBytes(16)
    KeyOut = Init(seedOut)
    KeyIn = Init(seedIn)
End Sub
    
Private Function Init(ByRef base() As Byte) As Byte()
    Dim val As Long
    Dim position As Long
    Dim baseLength As Long
    Dim i As Long
    Dim temp As Byte

    Dim key(255 + 2) As Byte

    For i = 0 To 256 - 1
        key(i) = i
    Next

    key(256) = 0
    key(257) = 0

    val = 0
    position = 0
    baseLength = UBound(base) + 1
    For i = 1 To 64
        val = val + key((i * 4) - 4) + base(position Mod baseLength)
        val = val And &HFF
        position = position + 1
        temp = key((i * 4) - 4)
        key((i * 4) - 4) = key(val And &HFF)
        key(val And &HFF) = temp

        val = val + key((i * 4) - 3) + base(position Mod baseLength)
        val = val And &HFF
        position = position + 1
        temp = key((i * 4) - 3)
        key((i * 4) - 3) = key(val And &HFF)
        key(val And &HFF) = temp

        val = val + key((i * 4) - 2) + base(position Mod baseLength)
        val = val And &HFF
        position = position + 1
        temp = key((i * 4) - 2)
        key((i * 4) - 2) = key(val And &HFF)
        key(val And &HFF) = temp

        val = val + key((i * 4) - 1) + base(position Mod baseLength)
        val = val And &HFF
        position = position + 1
        temp = key((i * 4) - 1)
        key((i * 4) - 1) = key(val And &HFF)
        key(val And &HFF) = temp
    Next

    Init = key
End Function
Public Sub Crypt(ByRef data() As Byte, ByRef key() As Byte)
    Dim temp As Byte
    Dim i As Long
    For i = 0 To UBound(data)
        key(256) = (CInt(key(256)) + 1) And &HFF
        key(257) = (CInt(key(257)) + CInt(key(key(256)))) And &HFF

        temp = key(key(257) And &HFF)
        key(key(257) And &HFF) = key(key(256) And &HFF)
        key(key(256) And &HFF) = temp

        data(i) = (data(i) Xor key((CInt(key(key(257))) + CInt(key(key(256)))) And &HFF))
    Next
End Sub

Private Sub NewKey(ByRef seed() As Byte)
    'Initialization
    source1 = ComputeHash(seed, 0, 20)
    source2 = ComputeHash(seed, 20, 20)
    Update
End Sub
Private Sub Update()
    Dim buffer1(20 * 3 - 1) As Byte
    CopyMemory buffer1(0), source1(0), 20
    CopyMemory buffer1(20), data(0), 20
    CopyMemory buffer1(40), source2(0), 20
    data = ComputeHash(buffer1)
End Sub

Private Function GetByte() As Byte
    Dim r As Byte
    r = data(index)
    index = index + 1
    If index >= &H14 Then
        Update
        index = 0
    End If
    GetByte = r
End Function
Private Function GetBytes(ByVal count As Integer) As Byte()
    Dim b() As Byte
    ReDim b(count - 1)
    Dim i As Long
    For i = 0 To count - 1
        b(i) = GetByte
    Next
    GetBytes = b
End Function

Private Function ComputeHash(ByRef data() As Byte, Optional ByVal index As Long = 0, Optional ByVal length As Long = -1) As Byte()
    Dim str As String, i As Long, returnStr As String, returnBytes() As Byte
    If length < 0 Then length = UBound(data) + 1
    str = ""
    For i = index To length - 1
        str = str & Chr(data(i))
    Next i
    returnStr = sha1(str)
    returnBytes = StrConv(returnStr, vbFromUnicode)
    ComputeHash = returnBytes
End Function
