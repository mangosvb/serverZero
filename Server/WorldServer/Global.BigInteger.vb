Imports mangosVB.Common.NativeMethods

Public Class BigInteger
    Private bigint As IntPtr = IntPtr.Zero
    Private Shared ctx As IntPtr = BN_CTX_new()

    Public Sub New()
        bigint = BN_new()
    End Sub

    Public Sub New(ByVal IntPtr As IntPtr)
        bigint = IntPtr
    End Sub

    Public Sub New(ByVal input() As Byte)
        bigint = BN_bin2bn(input, input.Length, IntPtr.Zero)
    End Sub

    Public Sub New(ByVal input As Integer)
        Dim buffer() As Byte = BitConverter.GetBytes(input)
        Array.Reverse(buffer)
        bigint = BN_bin2bn(buffer, buffer.Length, IntPtr.Zero)
    End Sub

    Public Sub New(ByVal input As Long)
        Dim buffer() As Byte = BitConverter.GetBytes(input)
        Array.Reverse(buffer)
        bigint = BN_bin2bn(buffer, buffer.Length, IntPtr.Zero)
    End Sub

    Public ReadOnly Property IntPtr() As IntPtr
        Get
            Return bigint
        End Get
    End Property

    Public Function GetBytes() As Byte()
        If BN_num_bits(bigint) = 0 Then Return New Byte() {}

        Dim numBytes As Integer = (BN_num_bits(bigint) - 1) \ 8
        Dim bytes(numBytes) As Byte
        BN_bn2bin(bigint, bytes)
        Return bytes
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If Not TypeOf obj Is BigInteger Then Return False
        Dim bi As BigInteger = DirectCast(obj, BigInteger)

        Dim bytes1() As Byte = Me.GetBytes()
        Dim bytes2() As Byte = bi.GetBytes()

        If bytes1.Length <> bytes2.Length Then Return False

        For i As Integer = 0 To bytes1.Length - 1
            If bytes1(i) <> bytes2(i) Then Return False
        Next

        Return True
    End Function

    Public Function ModPow(ByVal p As BigInteger, ByVal m As BigInteger) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_mod_exp(newBN, bigint, p.IntPtr, m.IntPtr, ctx)
        Return New BigInteger(newBN)
    End Function

    Public Shared Operator +(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_add(newBN, a.IntPtr, b.IntPtr)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator -(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_sub(newBN, a.IntPtr, b.IntPtr)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator *(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_mul(newBN, a.IntPtr, b.IntPtr, ctx)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator \(ByVal a As BigInteger, ByVal d As BigInteger) As BigInteger
        Dim newBN As IntPtr = BN_new()
        Dim remainder As IntPtr = BN_new()
        BN_div(newBN, remainder, a.IntPtr, d.IntPtr, ctx)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator Mod(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_mod(newBN, a.IntPtr, b.IntPtr, ctx)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator ^(ByVal a As BigInteger, ByVal p As BigInteger) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_exp(newBN, a.IntPtr, p.IntPtr, ctx)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator <<(ByVal a As BigInteger, ByVal b As Integer) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_lshift(newBN, a.IntPtr, b)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator >>(ByVal a As BigInteger, ByVal b As Integer) As BigInteger
        Dim newBN As IntPtr = BN_new()
        BN_rshift(newBN, a.IntPtr, b)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator Not(ByVal a As BigInteger) As BigInteger
        Dim bytes() As Byte = a.GetBytes()
        For i As Integer = 0 To bytes.Length - 1
            bytes(i) = Not bytes(i)
        Next

        Dim newBN As IntPtr = BN_bin2bn(bytes, bytes.Length, IntPtr.Zero)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator And(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
        Dim bytes1() As Byte = a.GetBytes()
        Dim bytes2() As Byte = b.GetBytes()

        Dim len As Integer = If(bytes1.Length > bytes2.Length, bytes2.Length, bytes1.Length)
        For i As Integer = 0 To len - 1
            bytes1(i) = bytes1(i) And bytes2(i)
        Next

        Dim newBN As IntPtr = BN_bin2bn(bytes1, bytes1.Length, IntPtr.Zero)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator Or(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
        Dim bytes1() As Byte = a.GetBytes()
        Dim bytes2() As Byte = b.GetBytes()

        Dim len As Integer = If(bytes1.Length > bytes2.Length, bytes2.Length, bytes1.Length)
        For i As Integer = 0 To len - 1
            bytes1(i) = bytes1(i) Or bytes2(i)
        Next

        Dim newBN As IntPtr = BN_bin2bn(bytes1, bytes1.Length, IntPtr.Zero)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator Xor(ByVal a As BigInteger, ByVal b As BigInteger) As BigInteger
        Dim bytes1() As Byte = a.GetBytes()
        Dim bytes2() As Byte = b.GetBytes()

        Dim len As Integer = If(bytes1.Length > bytes2.Length, bytes2.Length, bytes1.Length)
        For i As Integer = 0 To len - 1
            bytes1(i) = bytes1(i) Xor bytes2(i)
        Next

        Dim newBN As IntPtr = BN_bin2bn(bytes1, bytes1.Length, IntPtr.Zero)
        Return New BigInteger(newBN)
    End Operator

    Public Shared Operator =(ByVal a As BigInteger, ByVal b As BigInteger) As Boolean
        Return a.Equals(b)
    End Operator

    Public Shared Operator <>(ByVal a As BigInteger, ByVal b As BigInteger) As Boolean
        Return Not a.Equals(b)
    End Operator

    Public Shared Operator >(ByVal a As BigInteger, ByVal b As BigInteger) As Boolean
        Dim bytes1() As Byte = a.GetBytes()
        Dim bytes2() As Byte = b.GetBytes()

        'If one of them is negative
        If (bytes1(bytes1.Length - 1) And &H80) > 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) = 0 Then Return False
        If (bytes1(bytes1.Length - 1) And &H80) = 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) > 0 Then Return True

        'If they are same signed
        Dim len As Integer = If(bytes1.Length > bytes2.Length, bytes1.Length, bytes2.Length)
        For i As Integer = len - 1 To 0 Step -1
            If bytes1(i) < bytes2(i) Then Return False
            If bytes1(i) > bytes2(i) Then Return True
        Next
        Return False
    End Operator

    Public Shared Operator <(ByVal a As BigInteger, ByVal b As BigInteger) As Boolean
        Dim bytes1() As Byte = a.GetBytes()
        Dim bytes2() As Byte = b.GetBytes()

        'If one of them is negative
        If (bytes1(bytes1.Length - 1) And &H80) > 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) = 0 Then Return True
        If (bytes1(bytes1.Length - 1) And &H80) = 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) > 0 Then Return False

        'If they are same signed
        Dim len As Integer = If(bytes1.Length > bytes2.Length, bytes1.Length, bytes2.Length)
        For i As Integer = len - 1 To 0 Step -1
            If bytes1(i) < bytes2(i) Then Return True
            If bytes1(i) > bytes2(i) Then Return False
        Next
        Return False
    End Operator

    Public Shared Operator >=(ByVal a As BigInteger, ByVal b As BigInteger) As Boolean
        Dim bytes1() As Byte = a.GetBytes()
        Dim bytes2() As Byte = b.GetBytes()

        'If one of them is negative
        If (bytes1(bytes1.Length - 1) And &H80) > 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) = 0 Then Return False
        If (bytes1(bytes1.Length - 1) And &H80) = 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) > 0 Then Return True

        'If they are same signed
        Dim len As Integer = If(bytes1.Length > bytes2.Length, bytes1.Length, bytes2.Length)
        For i As Integer = len - 1 To 0 Step -1
            If bytes1(i) < bytes2(i) Then Return False
            If bytes1(i) > bytes2(i) Then Return True
        Next
        Return True
    End Operator

    Public Shared Operator <=(ByVal a As BigInteger, ByVal b As BigInteger) As Boolean
        Dim bytes1() As Byte = a.GetBytes()
        Dim bytes2() As Byte = b.GetBytes()

        'If one of them is negative
        If (bytes1(bytes1.Length - 1) And &H80) > 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) = 0 Then Return False
        If (bytes1(bytes1.Length - 1) And &H80) = 0 AndAlso (bytes2(bytes2.Length - 1) And &H80) > 0 Then Return True

        'If they are same signed
        Dim len As Integer = If(bytes1.Length > bytes2.Length, bytes1.Length, bytes2.Length)
        For i As Integer = len - 1 To 0 Step -1
            If bytes1(i) < bytes2(i) Then Return False
            If bytes1(i) > bytes2(i) Then Return True
        Next
        Return True
    End Operator

    Public Shared Operator IsTrue(ByVal a As BigInteger) As Boolean
        Dim bytes1() As Byte = a.GetBytes()
        If bytes1.Length = 1 AndAlso bytes1(0) = 0 Then Return False
        Return True
    End Operator

    Public Shared Operator IsFalse(ByVal a As BigInteger) As Boolean
        Dim bytes1() As Byte = a.GetBytes()
        If bytes1.Length = 1 AndAlso bytes1(0) = 0 Then Return True
        Return False
    End Operator

    Public Overrides Function GetHashCode() As Integer
        Return Me.GetBytes().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return ToString(10)
    End Function

    Public Overloads Function ToString(ByVal radix As Integer) As String
        Dim bytes() As Byte = Me.GetBytes()
        If bytes.Length = 0 Then Return "0"

        Dim a As BigInteger = Me

        Dim quotient As New BigInteger()
        Dim remainder As New BigInteger()
        Dim biRadix As New BigInteger(radix)

        Dim charSet As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim result As New System.Text.StringBuilder()

        While bytes.Length > 0
            BN_div(quotient.IntPtr, remainder.IntPtr, a.IntPtr, biRadix.IntPtr, ctx)

            Dim remainderBytes() As Byte = remainder.GetBytes()
            If remainderBytes.Length = 0 Then
                result.Append("0")
            Else
                If remainderBytes(0) < 10 Then
                    result.Append(remainderBytes(0))
                Else
                    result.Append(charSet(remainderBytes(0) - 10))
                End If
            End If

            a = quotient
            bytes = a.GetBytes()
        End While

        Return result.ToString()
    End Function

    Public Function ToHEX() As String
        Dim bytes() As Byte = Me.GetBytes()
        If bytes.Length = 0 Then Return "00"

        Dim sHEX As New System.Text.StringBuilder(bytes.Length * 2)
        For i As Integer = bytes.Length - 1 To 0 Step -1
            sHEX.Append(bytes(i).ToString("X2"))
        Next

        Return sHEX.ToString()
    End Function

End Class
