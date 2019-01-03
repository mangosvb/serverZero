'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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
Imports System.Security.Cryptography

Imports mangosVB.Common

Public NotInheritable Class AuthEngineClass
    Implements IDisposable
    Private ReadOnly _log As New Logging.BaseWriter

    Shared Sub New()
        CrcSalt = New Byte(16 - 1) {}
        RAND_bytes(CrcSalt, 16, "")
    End Sub

    Public Sub New()
        Dim buffer1 As Byte() = New Byte() {7}
        g = buffer1
        N = New Byte() _
            {137, 75, 100, 94, 137, 225, 83, 91, 189, 173, 91, 139, 41, 6, 80, 83, 8, 1, 177, 142, 191, 191, 94, 143,
             171, 60, 130, 135, 42, 62, 155, 183}
        Salt = New Byte() _
            {173, 208, 58, 49, 210, 113, 20, 70, 117, 242, 112, 126, 80, 38, 182, 210, 241, 134, 89, 153, 118, 2, 80,
             170, 185, 69, 224, 158, 221, 42, 163, 69}
        Dim buffer2 As Byte() = New Byte() {3}
        _k = buffer2
        PublicB = New Byte(32 - 1) {}
        _b = New Byte(20 - 1) {}
    End Sub

    Private Sub CalculateB()
        ' Dim encoding1 As New UTF7Encoding
        RAND_bytes(_b, 20, "")
        Dim ptr1 As IntPtr = BN_new("")
        Dim ptr2 As IntPtr = BN_new("")
        Dim ptr3 As IntPtr = BN_new("")
        ' Dim ptr4 As IntPtr = BN_new("")
        _bnPublicB = BN_new("")
        Dim ptr5 As IntPtr = BN_CTX_new("")
        Array.Reverse(_b)
        _bNb = BN_bin2bn(_b, _b.Length, IntPtr.Zero, "")
        Array.Reverse(_b)
        BN_mod_exp(ptr1, _bNg, _bNb, _bNn, ptr5, "")
        BN_mul(ptr2, _bNk, _bNv, ptr5, "")
        BN_add(ptr3, ptr1, ptr2, "")
        BN_mod(_bnPublicB, ptr3, _bNn, ptr5, "")
        BN_bn2bin(_bnPublicB, PublicB, "")
        Array.Reverse(PublicB)
    End Sub

    Private Sub CalculateK()
        Dim algorithm1 As New SHA1Managed
        Dim list1 As New ArrayList
        list1 = Split(_s)
        list1.Item(0) = algorithm1.ComputeHash(CType(list1.Item(0), Byte()))
        list1.Item(1) = algorithm1.ComputeHash(CType(list1.Item(1), Byte()))
        SsHash = Combine(CType(list1.Item(0), Byte()), CType(list1.Item(1), Byte()))
    End Sub

    Public Sub CalculateM2(ByVal m1Loc As Byte())
        Dim algorithm1 As New SHA1Managed
        Dim buffer1 As Byte() = New Byte(_a.Length + m1Loc.Length + SsHash.Length - 1) {}
        Buffer.BlockCopy(_a, 0, buffer1, 0, _a.Length)
        Buffer.BlockCopy(m1Loc, 0, buffer1, _a.Length, m1Loc.Length)
        Buffer.BlockCopy(SsHash, 0, buffer1, (_a.Length + m1Loc.Length), SsHash.Length)
        M2 = algorithm1.ComputeHash(buffer1)
    End Sub

    Private Sub CalculateS()
        Dim ptr1 As IntPtr = BN_new("")
        Dim ptr2 As IntPtr = BN_new("")
        'Dim ptr3 As IntPtr = BN_new("")
        'Dim ptr4 As IntPtr = BN_new("")
        _bns = BN_new("")
        Dim ptr5 As IntPtr = BN_CTX_new("")
        _s = New Byte(32 - 1) {}
        BN_mod_exp(ptr1, _bNv, _bnu, _bNn, ptr5, "")
        BN_mul(ptr2, _bna, ptr1, ptr5, "")
        BN_mod_exp(_bns, ptr2, _bNb, _bNn, ptr5, "")
        BN_bn2bin(_bns, _s, "")
        Array.Reverse(_s)
        CalculateK()
    End Sub

    Public Sub CalculateU(ByVal a As Byte())
        _a = a
        Dim algorithm1 As New SHA1Managed
        Dim buffer1 As Byte() = New Byte(a.Length + PublicB.Length - 1) {}
        Buffer.BlockCopy(a, 0, buffer1, 0, a.Length)
        Buffer.BlockCopy(PublicB, 0, buffer1, a.Length, PublicB.Length)
        _u = algorithm1.ComputeHash(buffer1)
        Array.Reverse(_u)
        _bnu = BN_bin2bn(_u, _u.Length, IntPtr.Zero, "")
        Array.Reverse(_u)
        Array.Reverse(a)
        _bna = BN_bin2bn(a, a.Length, IntPtr.Zero, "")
        Array.Reverse(a)
        CalculateS()
    End Sub

    Private Sub CalculateV()
        _bNv = BN_new("")
        Dim ptr1 As IntPtr = BN_CTX_new("")
        BN_mod_exp(_bNv, _bNg, _bNx, _bNn, ptr1, "")
        CalculateB()
    End Sub

    Public Sub CalculateX(ByVal username As Byte(), ByVal pwHash As Byte())
        _username = username

        Dim algorithm1 As New SHA1Managed
        'Dim encoding1 As New UTF7Encoding
        Dim buffer3 As Byte()
        buffer3 = New Byte(20 - 1) {}
        Dim buffer5 As Byte()
        buffer5 = New Byte((Salt.Length + 20) - 1) {}
        Buffer.BlockCopy(pwHash, 0, buffer5, Salt.Length, 20)
        Buffer.BlockCopy(Salt, 0, buffer5, 0, Salt.Length)
        buffer3 = algorithm1.ComputeHash(buffer5)
        Array.Reverse(buffer3)
        _bNx = BN_bin2bn(buffer3, buffer3.Length, IntPtr.Zero, "")
        Array.Reverse(g)
        _bNg = BN_bin2bn(g, g.Length, IntPtr.Zero, "")
        Array.Reverse(g)
        Array.Reverse(_k)
        _bNk = BN_bin2bn(_k, _k.Length, IntPtr.Zero, "")
        Array.Reverse(_k)
        Array.Reverse(N)
        _bNn = BN_bin2bn(N, N.Length, IntPtr.Zero, "")
        Array.Reverse(N)
        CalculateV()
    End Sub

    Public Sub CalculateM1()
        Dim algorithm1 As New SHA1Managed
        Dim nHash As Byte()
        nHash = New Byte(20 - 1) {}
        Dim gHash As Byte()
        gHash = New Byte(20 - 1) {}
        Dim ngHash As Byte()
        ngHash = New Byte(20 - 1) {}
        Dim userHash As Byte()
        userHash = New Byte(20 - 1) {}

        nHash = algorithm1.ComputeHash(N)
        gHash = algorithm1.ComputeHash(g)
        userHash = algorithm1.ComputeHash(_username)
        For i As Integer = 0 To 19
            ngHash(i) = nHash(i) Xor gHash(i)
        Next i

        Dim temp As Byte() = Concat(ngHash, userHash)
        temp = Concat(temp, Salt)
        temp = Concat(temp, _a)
        temp = Concat(temp, PublicB)
        temp = Concat(temp, SsHash)
        M1 = algorithm1.ComputeHash(temp)
    End Sub

    'Public Sub CalculateM1_Full()
    '    Dim sha2 As New SHA1CryptoServiceProvider
    '    Dim i As Byte = 0

    '    'Calc S1/S2
    '    Dim s1 As Byte()
    '    s1 = New Byte(16 - 1) {}
    '    Dim s2 As Byte()
    '    s2 = New Byte(16 - 1) {}
    '    Do While (i < 16)
    '        s1(i) = _s((i * 2))
    '        s2(i) = _s(((i * 2) + 1))
    '        i += 1
    '    Loop

    '    'Calc SSHash
    '    Dim s1Hash As Byte()
    '    s1Hash = sha2.ComputeHash(s1)
    '    Dim s2Hash As Byte()
    '    s2Hash = sha2.ComputeHash(s2)
    '    ReDim SsHash(32 - 1)
    '    i = 0
    '    Do While (i < 16)
    '        SsHash((i * 2)) = s1Hash(i)
    '        SsHash(((i * 2) + 1)) = s2Hash(i)
    '        i += 1
    '    Loop

    '    'Calc M1
    '    Dim nHash As Byte()
    '    nHash = sha2.ComputeHash(N)
    '    Dim gHash As Byte()
    '    gHash = sha2.ComputeHash(g)
    '    Dim userHash As Byte()
    '    userHash = sha2.ComputeHash(_Username)

    '    Dim ngHash As Byte()
    '    ngHash = New Byte(20 - 1) {}
    '    i = 0
    '    Do While (i < 20)
    '        ngHash(i) = (nHash(i) Xor gHash(i))
    '        i += 1
    '    Loop

    '    Dim temp As Byte() = Concat(ngHash, userHash)
    '    temp = Concat(temp, Salt)
    '    temp = Concat(temp, _a)
    '    temp = Concat(temp, PublicB)
    '    temp = Concat(temp, SsHash)
    '    M1 = sha2.ComputeHash(temp)
    'End Sub

    Private Function Combine(ByVal Bytes1 As Byte(), ByVal Bytes2 As Byte()) As Byte()
        If (Bytes1.Length <> Bytes2.Length) Then Return Nothing

        Dim CombineBuffer As Byte() = New Byte(Bytes1.Length + Bytes2.Length - 1) {}
        Dim Counter As Integer = 0
        For i As Integer = 0 To CombineBuffer.Length - 1 Step 2
            CombineBuffer(i) = Bytes1(Counter)
            Counter += 1
        Next
        Counter = 0

        For i As Integer = 1 To CombineBuffer.Length - 1 Step 2
            CombineBuffer(i) = Bytes2(Counter)
            Counter += 1
        Next

        Return CombineBuffer
    End Function

    Public Function Concat(ByVal Buffer1 As Byte(), ByVal Buffer2 As Byte()) As Byte()
        Dim ConcatBuffer As Byte() = New Byte(Buffer1.Length + Buffer2.Length - 1) {}
        Array.Copy(Buffer1, ConcatBuffer, Buffer1.Length)
        Array.Copy(Buffer2, 0, ConcatBuffer, Buffer1.Length, Buffer2.Length)

        Return ConcatBuffer
    End Function

    Private Function Split(ByVal ByteBuffer As Byte()) As ArrayList
        Dim SplitBuffer1 As Byte() = New Byte(ByteBuffer.Length / 2 - 1) {}
        Dim SplitBuffer2 As Byte() = New Byte(ByteBuffer.Length / 2 - 1) {}
        Dim ReturnList As New ArrayList
        Dim Counter As Integer = 0

        For i As Integer = 0 To SplitBuffer1.Length - 1
            SplitBuffer1(i) = ByteBuffer(Counter)
            Counter += 2
        Next
        Counter = 1
        For i As Integer = 0 To SplitBuffer2.Length - 1
            SplitBuffer2(i) = ByteBuffer(Counter)
            Counter += 2
        Next

        ReturnList.Add(SplitBuffer1)
        ReturnList.Add(SplitBuffer2)

        Return ReturnList
    End Function

    Private _a As Byte()
    Private ReadOnly _b As Byte()
    Public ReadOnly PublicB As Byte()
    Public g As Byte()
    Private ReadOnly _k As Byte()
    'Private PublicK As Byte() = SS_Hash
    Public M2 As Byte()
    Public ReadOnly N As Byte()
    'Private Password As Byte()
    Private _s As Byte()
    Public ReadOnly Salt As Byte()
    Private _u As Byte()
    Public Shared ReadOnly CrcSalt As Byte()
    'Public CrcHash As Byte()
    Private _username As Byte()

    Public M1 As Byte()
    Public SsHash As Byte()

    Private _bna As IntPtr
    Private _bNb As IntPtr
    Private _bnPublicB As IntPtr
    Private _bNg As IntPtr
    Private _bNk As IntPtr
    Private _bNn As IntPtr
    Private _bns As IntPtr
    Private _bnu As IntPtr
    Private _bNv As IntPtr
    Private _bNx As IntPtr

#Region "IDisposable Support"

    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Private Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            _log.Dispose()
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
