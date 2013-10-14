'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
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
Imports System.Text
Imports System.Security.Cryptography
Imports mangosVB.Common
Imports mangosVB.Common.NativeMethods

Public Class AuthEngineClass
    Implements IDisposable
    Public Log As New BaseWriter

#Region "AuthEngine.Constructive"
    Shared Sub New()
        AuthEngineClass.CrcSalt = New Byte(16 - 1) {}
        RAND_bytes(CrcSalt, 16, "")

    End Sub
    Public Sub New()
        Dim buffer1 As Byte() = New Byte() {7}
        Me.g = buffer1
        Me.N = New Byte() {137, 75, 100, 94, 137, 225, 83, 91, 189, 173, 91, 139, 41, 6, 80, 83, 8, 1, 177, 142, 191, 191, 94, 143, 171, 60, 130, 135, 42, 62, 155, 183}
        Me.salt = New Byte() {173, 208, 58, 49, 210, 113, 20, 70, 117, 242, 112, 126, 80, 38, 182, 210, 241, 134, 89, 153, 118, 2, 80, 170, 185, 69, 224, 158, 221, 42, 163, 69}
        Dim buffer2 As Byte() = New Byte() {3}
        Me.k = buffer2
        Me.PublicB = New Byte(32 - 1) {}
        Me.b = New Byte(20 - 1) {}
    End Sub
    


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            Log.Dispose()
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region
#End Region

#Region "AuthEngine.Calculations"
    Private Sub CalculateB()
        Dim encoding1 As New UTF7Encoding
        RAND_bytes(Me.b, 20, "")
        Dim ptr1 As IntPtr = BN_new("")
        Dim ptr2 As IntPtr = BN_new("")
        Dim ptr3 As IntPtr = BN_new("")
        Dim ptr4 As IntPtr = BN_new("")
        Me.BNPublicB = BN_new("")
        Dim ptr5 As IntPtr = BN_CTX_new("")
        Array.Reverse(Me.b)
        Me.BNb = BN_bin2bn(Me.b, Me.b.Length, IntPtr.Zero, "")
        Array.Reverse(Me.b)
        BN_mod_exp(ptr1, Me.BNg, Me.BNb, Me.BNn, ptr5, "")
        BN_mul(ptr2, Me.BNk, Me.BNv, ptr5, "")
        BN_add(ptr3, ptr1, ptr2, "")
        BN_mod(Me.BNPublicB, ptr3, Me.BNn, ptr5, "")
        BN_bn2bin(Me.BNPublicB, Me.PublicB, "")
        Array.Reverse(Me.PublicB)
    End Sub
    Private Sub CalculateK()
        Dim algorithm1 As New SHA1Managed
        Dim list1 As New ArrayList
        list1 = AuthEngineClass.Split(Me.S)
        list1.Item(0) = algorithm1.ComputeHash(CType(list1.Item(0), Byte()))
        list1.Item(1) = algorithm1.ComputeHash(CType(list1.Item(1), Byte()))
        Me.SS_Hash = AuthEngineClass.Combine(CType(list1.Item(0), Byte()), CType(list1.Item(1), Byte()))
    End Sub
    Public Sub CalculateM2(ByVal m1 As Byte())
        Dim algorithm1 As New SHA1Managed
        Dim buffer1 As Byte() = New Byte(((Me.A.Length + m1.Length) + Me.SS_Hash.Length) - 1) {}
        Buffer.BlockCopy(Me.A, 0, buffer1, 0, Me.A.Length)
        Buffer.BlockCopy(m1, 0, buffer1, Me.A.Length, m1.Length)
        Buffer.BlockCopy(Me.SS_Hash, 0, buffer1, (Me.A.Length + m1.Length), Me.SS_Hash.Length)
        Me.M2 = algorithm1.ComputeHash(buffer1)
    End Sub
    Private Sub CalculateS()
        Dim ptr1 As IntPtr = BN_new("")
        Dim ptr2 As IntPtr = BN_new("")
        Dim ptr3 As IntPtr = BN_new("")
        Dim ptr4 As IntPtr = BN_new("")
        Me.BNS = BN_new("")
        Dim ptr5 As IntPtr = BN_CTX_new("")
        Me.S = New Byte(32 - 1) {}
        BN_mod_exp(ptr1, Me.BNv, Me.BNU, Me.BNn, ptr5, "")
        BN_mul(ptr2, Me.BNA, ptr1, ptr5, "")
        BN_mod_exp(Me.BNS, ptr2, Me.BNb, Me.BNn, ptr5, "")
        BN_bn2bin(Me.BNS, Me.S, "")
        Array.Reverse(Me.S)
        Me.CalculateK()
    End Sub
    Public Sub CalculateU(ByVal a As Byte())
        Me.A = a
        Dim algorithm1 As New SHA1Managed
        Dim buffer1 As Byte() = New Byte((a.Length + Me.PublicB.Length) - 1) {}
        Buffer.BlockCopy(a, 0, buffer1, 0, a.Length)
        Buffer.BlockCopy(Me.PublicB, 0, buffer1, a.Length, Me.PublicB.Length)
        Me.U = algorithm1.ComputeHash(buffer1)
        Array.Reverse(Me.U)
        Me.BNU = BN_bin2bn(Me.U, Me.U.Length, IntPtr.Zero, "")
        Array.Reverse(Me.U)
        Array.Reverse(Me.A)
        Me.BNA = BN_bin2bn(Me.A, Me.A.Length, IntPtr.Zero, "")
        Array.Reverse(Me.A)
        Me.CalculateS()
    End Sub
    Private Sub CalculateV()
        Me.BNv = BN_new("")
        Dim ptr1 As IntPtr = BN_CTX_new("")
        BN_mod_exp(Me.BNv, Me.BNg, Me.BNx, Me.BNn, ptr1, "")
        Me.CalculateB()
    End Sub
    Public Sub CalculateX(ByVal username As Byte(), ByVal pwHash As Byte())
        Me.Username = Username

        Dim algorithm1 As New SHA1Managed
        Dim encoding1 As New UTF7Encoding
        Dim buffer3 As Byte() = New Byte(20 - 1) {}
        Dim buffer5 As Byte() = New Byte((Me.salt.Length + 20) - 1) {}
        Buffer.BlockCopy(pwHash, 0, buffer5, Me.salt.Length, 20)
        Buffer.BlockCopy(Me.salt, 0, buffer5, 0, Me.salt.Length)
        buffer3 = algorithm1.ComputeHash(buffer5)
        Array.Reverse(buffer3)
        Me.BNx = BN_bin2bn(buffer3, buffer3.Length, IntPtr.Zero, "")
        Array.Reverse(Me.g)
        Me.BNg = BN_bin2bn(Me.g, Me.g.Length, IntPtr.Zero, "")
        Array.Reverse(Me.g)
        Array.Reverse(Me.k)
        Me.BNk = BN_bin2bn(Me.k, Me.k.Length, IntPtr.Zero, "")
        Array.Reverse(Me.k)
        Array.Reverse(Me.N)
        Me.BNn = BN_bin2bn(Me.N, Me.N.Length, IntPtr.Zero, "")
        Array.Reverse(Me.N)
        Me.CalculateV()
    End Sub


    Public Sub CalculateM1()
        Dim algorithm1 As New SHA1Managed
        Dim N_Hash As Byte() = New Byte(20 - 1) {}
        Dim G_Hash As Byte() = New Byte(20 - 1) {}
        Dim NG_Hash As Byte() = New Byte(20 - 1) {}
        Dim User_Hash As Byte() = New Byte(20 - 1) {}
        Dim i As Integer

        N_Hash = algorithm1.ComputeHash(N)
        G_Hash = algorithm1.ComputeHash(g)
        User_Hash = algorithm1.ComputeHash(Username)
        For i = 0 To 19
            NG_Hash(i) = CType(N_Hash(i) Xor G_Hash(i), Byte)
        Next i

        Dim temp As Byte() = AuthEngineClass.Concat(NG_Hash, User_Hash)
        temp = AuthEngineClass.Concat(temp, Me.salt)
        temp = AuthEngineClass.Concat(temp, Me.A)
        temp = AuthEngineClass.Concat(temp, Me.PublicB)
        temp = AuthEngineClass.Concat(temp, Me.SS_Hash)
        Me.M1 = algorithm1.ComputeHash(temp)
    End Sub
    Public Sub CalculateM1_Full()
        Dim sha2 As New SHA1CryptoServiceProvider
        Dim i As Byte = 0

        'Calc S1/S2
        Dim S1 As Byte() = New Byte(16 - 1) {}
        Dim S2 As Byte() = New Byte(16 - 1) {}
        Do While (i < 16)
            S1(i) = S((i * 2))
            S2(i) = S(((i * 2) + 1))
            i += 1
        Loop

        'Calc SSHash
        Dim S1_Hash As Byte() = sha2.ComputeHash(S1)
        Dim S2_Hash As Byte() = sha2.ComputeHash(S2)
        ReDim SS_Hash(32 - 1)
        i = 0
        Do While (i < 16)
            Me.SS_Hash((i * 2)) = S1_Hash(i)
            Me.SS_Hash(((i * 2) + 1)) = S2_Hash(i)
            i += 1
        Loop

        'Calc M1
        Dim N_Hash As Byte() = sha2.ComputeHash(Me.N)
        Dim G_Hash As Byte() = sha2.ComputeHash(Me.g)
        Dim User_Hash As Byte() = sha2.ComputeHash(Me.Username)

        Dim NG_Hash As Byte() = New Byte(20 - 1) {}
        i = 0
        Do While (i < 20)
            NG_Hash(i) = CType((N_Hash(i) Xor G_Hash(i)), Byte)
            i += 1
        Loop

        Dim temp As Byte() = AuthEngineClass.Concat(NG_Hash, User_Hash)
        temp = AuthEngineClass.Concat(temp, Me.salt)
        temp = AuthEngineClass.Concat(temp, Me.A)
        temp = AuthEngineClass.Concat(temp, Me.PublicB)
        temp = AuthEngineClass.Concat(temp, Me.SS_Hash)
        Me.M1 = sha2.ComputeHash(temp)
    End Sub
#End Region

#Region "AuthEngine.Functions"
    Private Shared Function Combine(ByVal b1 As Byte(), ByVal b2 As Byte()) As Byte()
        If (b1.Length = b2.Length) Then
            Dim buffer1 As Byte() = New Byte((b1.Length + b2.Length) - 1) {}
            Dim num1 As Integer = 0
            Dim num2 As Integer = 1
            Dim num3 As Integer
            For num3 = 0 To b1.Length - 1
                buffer1(num1) = b1(num3)
                num1 += 1
                num1 += 1
            Next num3
            Dim num4 As Integer
            For num4 = 0 To b2.Length - 1
                buffer1(num2) = b2(num4)
                num2 += 1
                num2 += 1
            Next num4
            Return buffer1
        End If
        Return Nothing
    End Function
    Public Shared Function Concat(ByVal a As Byte(), ByVal b As Byte()) As Byte()
        Dim buffer1 As Byte() = New Byte((a.Length + b.Length) - 1) {}
        Dim num1 As Integer
        For num1 = 0 To a.Length - 1
            buffer1(num1) = a(num1)
        Next num1
        Dim num2 As Integer
        For num2 = 0 To b.Length - 1
            buffer1((num2 + a.Length)) = b(num2)
        Next num2
        Return buffer1
    End Function


    Private Shared Function Split(ByVal bo As Byte()) As ArrayList
        Dim buffer1 As Byte() = New Byte((bo.Length - 1) - 1) {}
        If (((bo.Length Mod 2) <> 0) AndAlso (bo.Length > 2)) Then
            Buffer.BlockCopy(bo, 1, buffer1, 0, bo.Length)
        End If
        Dim buffer2 As Byte() = New Byte((bo.Length / 2) - 1) {}
        Dim buffer3 As Byte() = New Byte((bo.Length / 2) - 1) {}
        Dim num1 As Integer = 0
        Dim num2 As Integer = 1
        Dim num3 As Integer
        For num3 = 0 To buffer2.Length - 1
            buffer2(num3) = bo(num1)
            num1 += 1
            num1 += 1
        Next num3
        Dim num4 As Integer
        For num4 = 0 To buffer3.Length - 1
            buffer3(num4) = bo(num2)
            num2 += 1
            num2 += 1
        Next num4
        Dim list1 As New ArrayList
        list1.Add(buffer2)
        list1.Add(buffer3)
        Return list1
    End Function
#End Region

#Region "AuthEngine.Variables"


    Private A As Byte()
    Private b As Byte()
    Public PublicB As Byte()
    Public g As Byte()
    Private k As Byte()
    'Private PublicK As Byte() = SS_Hash
    Public M2 As Byte()
    Public N As Byte()
    'Private Password As Byte()
    Private S As Byte()
    Public salt As Byte()
    Private U As Byte()
    Public Shared CrcSalt As Byte()
    Public CrcHash As Byte()
    Public Username As Byte()

    Public M1 As Byte()
    Public SS_Hash As Byte()
#End Region

#Region "AuthEngine.BigIntegers"
    Private BNA As IntPtr
    Private BNb As IntPtr
    Private BNPublicB As IntPtr
    Private BNg As IntPtr
    Private BNk As IntPtr
    Private BNn As IntPtr
    Private BNS As IntPtr
    Private BNU As IntPtr
    Private BNv As IntPtr
    Private BNx As IntPtr
#End Region

End Class