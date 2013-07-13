Attribute VB_Name = "modBigInt"
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

'Author : Sjoerd.J.Schaper - vspickelen@zonnet.nl
'URL    : http://www.home.zonnet.nl/vspickelen/Largefiles/LargeInt.htm
'Date   : 05-05-2004
'Code   : Visual Basic for Windows 5.0
'`[..] there is a bound to the number of symbols which the computer
'can observe at one moment. If he wishes to observe more, he must use
'successive operations.' Alan Turing, On computable numbers [..], 1936.
'
Option Explicit

Public Const Fx = 2147483629, WrkD = "c:\temp\" ' set path working directory
Public Const MD = 10000, M2 = 99990000, MB = &H10000 'dec, bin storage bases
Public F() As Long, l() As Integer, sg() As Integer
'f() holds the numbers, l() their lengths, sg() the signs.
'Create an instance of the /LargeInt/ class with a REDIM statement at
'module level. Take care to initialize enough index variables to span the
'entire /LargeInt/ subscript range, so that each number may be associated
'with its own distinct row pointer. These pointers are passed as arguments
'to the procedures. Les(), Readst or Letf are necessarily the first to be
'called. To avoid crashes, never call by value and always beware of
'duplicate references.
Public t0 As Integer, t1 As Integer, t2 As Integer
'Rows t0, t1 and t2 are reserved for storing intermediate results,
'be careful not to pass any of these pointers as arguments
'if they're already in use (Shared) on the procedure-level.
Public Key As Boolean, Data As String
'Key is used to mimic the QuickBasic ON KEY event trap,
'Data is printed to TextBox "Slate.Box.Text" by routines Printf and Ratdec.
'
'Overview___________________________________________________________________________________
'Sub Add(i0, i1)
'  Add i0 and i1, the result is in i0.
'Sub Binf(i0)
'Shared t1, t2
'  Convert i0 to binary base MB, to be called before procedure Modpwr.
'Function Cmp(i0, i1)
'  Compare returns -1 if i0 < i1, 0 if i0 = i1, or 1 if i0 > i1.
'Sub Copyf(i0, i1)
'  Copy i0 to i1.
'Sub Cvl(g$)
'  Convert 4-byte Ascii string g$ to long integer.
'Sub Dcr(i0, a)
'Shared t2
'  Decrease i0 with small positive a.
'Sub Divd(i0, i1, i2)
'Shared t2
'  Divide i0 by i1. The quotient is in i2, the remainder in i0.
'Sub Euclid(i0, i1, i2)
'Shared t0, t1, t2
'  The extended Euclidean algorithm computes
'  i0^ -1 Mod i1 in i0, i1/ gcd in i1 and gcd(i0, i1) in i2.
'Sub Fctl(i0, n)
'Shared t1, t2
'  Compute factorial n(n-1)贩򈭽 in i0.
'Sub Inc(i0, a)
'Shared t2
'  Increase i0 with small positive a.
'Function Isf(i0, a)
'  Boolean: check if i0 equals one-word value a.
'Function Ismin1(i0, i1)
'  Boolean: check if i0 = -1 Mod i1.
'Function IsPPrm(i0, t3, t4, w)
'Shared t0, t1, t2; t3 & t4 are additional swap rows.
'  Boolean: check if i0 is probably prime with the Miller-Rabin test.
'  w is the number of witnesses, t3 returns a small factor, if any.
'Sub Isqrt(i0, i1)
'Shared t0, t1, t2
'  Integer square root of i0 with Heron's algorithm. Result is in i1.
'Function IsSqr(i0, i1)
'Shared t0, t1, t2
'  Boolean: check if i0 is square. If true, the root is in i1.
'Function Kronec(i0, i1, i2)
'Shared t0, t1, t2
'  Kronecker's quadratic residuosity symbol (i0/i1) = 0, 1, or -1.
'  Returns an odd gcd(i0, i1) in i2, or 2 if it's even.
'Function Les(g$)
'  Find the minimum length of array f() required to hold number g$.
'Sub Letf(i0, a)
'  Put one-word value a into i0.
'Sub LetfD(i0, c, cM)
'  Put double-word value c into i0 to base MD or MB.
'Sub Lsft(i0, r)
'  i0 is shifted left by r bits, 0 < r <= 4.
'Sub Mkl$(c)
'  Convert long integer c to 4-byte Ascii string.
'Sub Moddiv(i0, i1)
'Shared t1, t2
'  Compute positive residue i0 modulo i1.
'Sub Modmlt(i0, i1, i2)
'Shared t1, t2
'  Multiply i0 and i1, then reduce modulo i2. The result is in i0.
'Sub Modpwr(i0, i1, i2)
'Shared t0, t1, t2
'  Modular exponentiation: base i0, binary exponent i1, modulus i2,
'  the result is in i0. Modulo reduction is disabled if i2 is empty.
'Sub Modsqu(i0, i1)
'Shared t1, t2
'  Square i0, then reduce modulo i1.
'Sub Mult(i0, i1, i2)
'  Multiply i0 and i1. The result is in i0 by default,
'  swap pointers i0 and i2 to put the factor back.
'Function Nxtprm&(sw)
'  Loop through table PrimFlgs.bin, opened as binary file #2.
'  Initiate with sw = 0 and get next prime with each successive call.
'Sub Printf(i0, g$, h$, sw)
'  Add array f(i0,..) with optional prefix g$ and tail h$ to Data.
'  If sw = 1, length(i0) is also printed.
'Sub Pwr10(i0, k)
'  Set MSB(i0) to 1 and add abs(k) trailing zero's.
'Sub Ratdec(i0, i1, g$, lt)
'Shared t1, t2
'  Print the decimal representation of rational i0/i1 with length lt.
'  i0 is erased, g$ is an optional prefix.
'Sub Readst(i0, g$)
'  Read number string g$ in blocks of four digits into f(i0,..).
'Sub Rsft(i0, r)
'  i0 is shifted right by r bits, 0 < r <= 4.
'Sub Squ(i0, i1)
'  Returns the square of i0 in i1.
'Sub Subt(i0, i1)
'  Subtract i1 from i0, the result is in i0. To retain i0,
'  use -(i1 - i0) by putting 'Subt i1, i0: sg(i1) = -sg(i1)'.
'Sub Swp(i0, i1)
'  Exchange i1 and i0.

Sub Add(i0 As Integer, i1 As Integer)
  sg(i1) = -sg(i1)
  Subt i0, i1: sg(i1) = -sg(i1)
End Sub

Sub Binf(i0 As Integer)
Dim C As Long, c0 As Long, c2 As Long, cs As Long
Dim j As Integer, k As Integer, ll As Integer
Dim Q As Integer, r As Boolean, S As Integer
  ll = l(i0) - 1
  If ll = 0 Then Exit Sub
  Copyf i0, t2: Q = 625 '             MD / 16
  k = 0: Do
    cs = 0: c2 = 1
    For S = 1 To 4 '                  MB = 16^ 4
      For j = 0 To ll '               divide by 16
        F(t2, j) = F(t2, j) * Q
      Next j: C = 0
      For j = ll To 0 Step -1
        c0 = F(t2, j) Mod MD
        F(t2, j) = F(t2, j) \ MD + C: C = c0
      Next j
      cs = cs + (C \ Q) * c2: c2 = c2 * 16
      If F(t2, ll) = 0 And ll > 0 Then l(t2) = ll: ll = ll - 1
      r = Isf(t2, 0): If r Then Exit For
    Next S
    F(t1, k) = cs: k = k + 1 '        residue = base MB-digit
  Loop Until r: sg(t1) = sg(i0)
  l(t1) = k: F(t1, k) = 0: Swp t1, i0
End Sub

Function Cmp(i0 As Integer, i1 As Integer) As Integer
Dim j As Integer, S As Integer, X As Integer
  X = sg(i0)
  If X = sg(i1) Then
    S = l(i0) - l(i1)
    If S = 0 Then
      For j = l(i0) - 1 To 0 Step -1
        S = F(i0, j) - F(i1, j) '     L=>R check
        If S <> 0 Then Exit For
      Next j
    End If
    Select Case S
      Case Is < 0: X = -X
      Case 0: X = 0
    End Select
  End If
  Cmp = X
End Function

Function Cvl(G As String) As Long
Dim C As Long, k As Integer
  k = Len(G): C = 0
  If k < 4 Then G = G + String$(4 - k, Chr$(0))
  For k = 4 To 1 Step -1
    C = C * 256 + Asc(Mid$(G, k, 1))
  Next k: Cvl = C
End Function

Sub Copyf(i0 As Integer, i1 As Integer)
Dim j As Integer
  For j = 0 To l(i0)
    F(i1, j) = F(i0, j)
  Next j
  l(i1) = l(i0): sg(i1) = sg(i0)
End Sub

Sub Dcr(i0 As Integer, A As Integer)
Dim r As Integer
  A = Abs(A)
  r = F(i0, 0) - sg(i0) * A
  If r > 0 And r < MD Then
    F(i0, 0) = r
  Else
    If r = 0 Then
      F(i0, 0) = 0
      If Isf(i0, 0) Then sg(i0) = 1
    Else
      Letf t2, A: Subt i0, t2
    End If
  End If
End Sub

Sub Divd(i0 As Integer, i1 As Integer, i2 As Integer)
Dim C As Long, c0 As Long, cs As Long, j As Integer
Dim k As Integer, l0 As Integer, l1 As Integer, l2 As Integer
Dim ll As Integer, m As Integer, nr As Double, Q As Integer
  If Isf(i1, 0) Then
    j = MsgBox("div. by zero in Sub Divd", 277, "Error")
    If j = 2 Then Close: End '        [Cancel]
    Letf i2, 0: Exit Sub '            [Retry]
  End If
  l0 = l(i0): l1 = l(i1): m = l0 - l1
  sg(i2) = sg(i0) * sg(i1): Q = sg(i2)
  If m < 0 Then Letf i2, 0: sg(i2) = Q: Exit Sub
  For j = 0 To m
    F(t2, j) = F(i0, j)
  Next j: sg(t2) = sg(i0)
  ll = l1 - 1: cs = F(i1, ll) '       MSB(divisor)
  If ll > 0 Then cs = cs * MD + F(i1, ll - 1)
  '
  For k = m To 0 Step -1
    nr = F(i0, l1 + k) '              MSB(intermediate dividend)
    For j = ll To ll - 1 Step -1
      If j < 0 Then Exit For
      nr = nr * MD + F(i0, j + k)
    Next j
    Q = Int(nr / cs): F(i2, k) = Q '  estimate partial quotient
    If Q > 0 Then
      Do
        C = MD: For j = 0 To l1 '     subtract multiplied divisor,
          c0 = C + F(i0, j + k) + M2 - Q * F(i1, j)
          C = c0 \ MD: F(t2, j + k) = c0 - C * MD
        Next j
        If C = MD Then '              swap dividend & remainder
          Swp i0, t2: Exit Do
        Else
          If Q > 1 Then '             or decrease quotient
            Q = Q - 1: F(i2, k) = Q
          Else
            F(i2, k) = 0: Exit Do
          End If
        End If
      Loop
    End If
  Next k: l0 = 1: l2 = 1
  For j = ll To 0 Step -1
    If F(i0, j) > 0 Then l0 = j + 1: Exit For
  Next j: l(i0) = l0: F(i0, l0) = 0
  For j = m To 0 Step -1
    If F(i2, j) > 0 Then l2 = j + 1: Exit For
  Next j: l(i2) = l2: F(i2, l2) = 0
End Sub

Sub Euclid(i0 As Integer, i1 As Integer, i2 As Integer)
Dim S As Integer
  If Isf(i1, 0) Then Copyf i0, i2: Exit Sub
  S = sg(i1): sg(i1) = 1
  If sg(i0) = -1 Then Moddiv i0, i1
  If Isf(i0, 0) Then Swp i1, i2: Letf i1, 1: Exit Sub
  Letf i2, 1: Letf t0, 0
  Do
    Swp i0, i1: Swp i2, t0
    Divd i0, i1, t1 '                 Euclidean division & inversion
    Mult t1, t0, t2: Subt i2, t1 '    r_t = r_t-2 - q_t * r_t-1
  Loop Until Isf(i0, 0)
  Swp i0, t0: Swp i1, i2
  If sg(i0) = -1 Then Add i0, i1
  sg(i1) = S: sg(i2) = 1
End Sub

Sub Fctl(i0 As Integer, N As Integer)
Dim C As Long, j As Integer, lx As Integer
Dim nr As Double, r As Integer
  If N < 3 Then
    If N < 0 Then
      j = MsgBox("illegal argument Fctl", 277, "Error")
      If j = 2 Then Close: End '      [Cancel]
      Letf i0, 0: Exit Sub '          [Retry]
    End If
    If N < 2 Then N = 1
    Letf i0, N: Exit Sub
  End If
  lx = UBound(F, 2)
  Letf i0, 1: r = 1
  Do: nr = r
    Do: r = r + 1
      C = CLng(nr): nr = nr * r
    Loop Until nr > Fx Or r > N
    LetfD t1, C, MD '                 partial product
    If l(i0) + l(t1) > lx Then
      j = MsgBox("overflow in Sub Fctl", 277, "Error")
      If j = 2 Then Close: End '      [Cancel]
      Letf i0, 0: Exit Sub '          [Retry]
    End If
    Mult i0, t1, t2
    DoEvents: If Key Then Exit Sub
  Loop Until r > N
End Sub

Sub Inc(i0 As Integer, A As Integer)
Dim r As Integer
  A = Abs(A)
  r = F(i0, 0) + sg(i0) * A
  If r > 0 And r < MD Then
    F(i0, 0) = r
  Else
    If r = 0 Then
      F(i0, 0) = 0
      If Isf(i0, 0) Then sg(i0) = 1
    Else
      Letf t2, A: Add i0, t2
    End If
  End If
End Sub

Function Isf(i0 As Integer, A As Integer) As Boolean
Dim r As Integer, S As Integer
  S = Sgn(A): If A = 0 Then S = sg(i0)
  r = F(i0, 1) + F(i0, 0) = Abs(A)
  Isf = r And (sg(i0) = S) And (l(i0) = 1)
End Function

Function Ismin1(i0 As Integer, i1 As Integer) As Boolean
  Ismin1 = 0
  If l(i0) > l(i1) And sg(i0) = 1 Then Exit Function
  Inc i0, 1: Ismin1 = Cmp(i0, i1) = 0: Dcr i0, 1
End Function

Function IsPPrm(i0 As Integer, t3 As Integer, t4 As Integer, w As Integer) As Boolean
Dim A As Integer, df As Integer, j As Integer, k As Integer
Dim Sw As Boolean, r As Integer, X As Boolean
  X = -1
  If sg(i0) = -1 Or Isf(i0, 1) Then
    X = 0: GoTo jmp
  End If
  Sw = F(t3, 0) = -32768 And l(i0) > 1
  If Not Sw Then '                      try small divisors
    Letf t3, 2: df = 1
    k = l(i0) * 15: If k > 2668 Then k = 2668
    For r = 1 To k
      Copyf i0, t0: Divd t0, t3, t1
      If Isf(t0, 0) Then '              a | N
        X = Isf(t1, 1): Sw = 0: GoTo jmp
      End If
      If Cmp(t3, t1) = 1 Then GoTo jmp 'a > sqrt(N)
      If Sw Then
        Do: df = 6 - df: Inc t3, df '   sieve multiples of 2, 3 and 5
        Loop Until F(t3, 0) Mod 5 > 0
      Else
        Inc t3, df: df = df * 2: Sw = F(t3, 0) = 5
      End If
      DoEvents: If Key Then Exit Function
    Next r
  End If
  '
  Copyf i0, t3: Dcr t3, 1 '           Miller-Rabin test
  k = 0: Do '                         determine N - 1 = (2^ k) * odd(m)
    k = k + 1: Rsft t3, 1
  Loop Until -(F(t3, 0) And 1)
  Binf t3: Randomize Timer
  A = 5 + Int(Rnd * 313): df = 4
  Do Until A Mod 6 = 5 '              random witness
    A = A + 1: Loop
  For r = 1 To w
    Letf t4, A
    Modpwr t4, t3, i0 '               if N is prime then
    If Key Then Exit Function
    X = Isf(t4, 1) Or Ismin1(t4, i0) 'a^ m = 1 Mod N or
    If X = 0 Then '         (a^ m)^ (2^ j) = -1 Mod N
      For j = 1 To k - 1 '     for some j in [0, k - 1]
        Modsqu t4, i0
        If Ismin1(t4, i0) Then X = -1: Exit For
      Next j
      If X = 0 Then GoTo jmp
    End If
    Do: df = 6 - df: A = A + df
    Loop Until A Mod 5 > 0
  Next r
jmp:
  If X Or Sw Then Letf t3, 1 '        Else return small factor t3
  IsPPrm = X
End Function

Sub Isqrt(i0 As Integer, i1 As Integer)
Dim C As Long, j As Integer, l1 As Integer, ll As Integer
Dim r As Single, S As Integer, tr As Single
  If sg(i0) = -1 Then
    j = MsgBox("illegal argument Isqrt", 277, "Error")
    If j = 2 Then Close: End '        [Cancel]
    Letf i1, 0: Exit Sub '            [Retry]
  End If
  If Isf(i0, 0) Or Isf(i0, 1) Then
    Copyf i0, i1: Exit Sub
  End If
  ll = l(i0) - 1: C = F(i0, ll) '     argument a in i0
  r = 21.54: Do: tr = C / r
    r = (r + tr) / 2 '                compute MSB(sqrt)
  Loop Until Abs(tr - r) < 1
  l1 = ll \ 2: l(i1) = l1 + 1
  For j = 0 To l(i1): F(i1, j) = 0: Next
  S = (ll And 1) * 99: sg(i1) = 1 '   compose seed x0 in i1
  F(i1, l1) = CLng(r * (1 + S))
  '
  Do '                                Heron's algorithm
    Copyf i0, t0: Divd t0, i1, t1 '   a / x0
    Add i1, t1: Rsft i1, 1 '          x1:= (x0 + a/x0) / 2
    Subt t1, i1
    S = F(t1, 1) + F(t1, 0) < 2
    DoEvents: If Key Then Exit Sub
  Loop Until S And l(t1) = sg(t1) '   If a/x0 - x1 = 0 Or 1 Then Exit
End Sub

Function IsSqr(i0 As Integer, i1 As Integer) As Boolean
'Square test from Henri Cohen's `Computational Number Theory', 1.7.2
Dim C As Long, k As Integer, k2 As Integer
Static q11(10) As Integer, q63(62) As Integer
Static q64(63) As Integer, q65(64) As Integer, Sw As Boolean
If Not Sw Then
  For k = 0 To 32
    k2 = k * k
    q11(k2 Mod 11) = -1
    q63(k2 Mod 63) = -1
    q64(k2 And 63) = -1
    q65(k2 Mod 65) = -1
  Next k: Sw = -1
End If
  IsSqr = 0
  k = (F(i0, 1) * MD + F(i0, 0)) And 63
  If q64(k) Then
    Copyf i0, t0: LetfD i1, 45045, MD
    Divd t0, i1, t1
    C = F(t0, 1) * MD + F(t0, 0)
    If q63(C Mod 63) Then
      If q65(C Mod 65) Then
        If q11(C Mod 11) Then '       i0 is possibly square
          Isqrt i0, i1: Squ i1, t2
          IsSqr = Cmp(i0, t2) = 0
        End If
      End If
    End If
  End If
End Function

Function Kronec(i0 As Integer, i1 As Integer, i2 As Integer) As Integer
Dim r As Integer, S As Integer, X As Integer
  X = 1
  Copyf i0, t0: Copyf i1, t1
  If sg(t1) = -1 Then
    sg(t1) = 1: X = sg(t0)
  End If
  If (F(t1, 0) And 1) = 0 Then
    If Isf(t1, 0) Then '              (a/0) undefined
      Swp i2, t0: X = -32768: GoTo jump
    End If
    If (F(t0, 0) And 1) = 0 Then
      Letf i2, 2: If Isf(t0, 0) Then Swp i2, t1
      X = 0: GoTo jump
    End If
    If Isf(t1, 2) Then
      Letf i2, 1: GoTo jump
    End If
    r = 0: Do
      r = r + 1: Rsft t1, 1
    Loop Until -(F(t1, 0) And 1)
    If -(r And 1) Then
      r = F(t0, 0) And 7
      If r = 3 Or r = 5 Then X = -X
    End If
  End If
  If sg(t0) = -1 Then
    sg(t0) = 1: If (F(t1, 0) And 3) = 3 Then X = -X
  End If
  If Isf(t1, 1) Then Swp i2, t1: GoTo jump
  '
  Do
    Divd t0, t1, i2
    If (F(t0, 0) And 1) = 0 Then
      If Isf(t0, 0) Then
        Swp i2, t1: X = 0: Exit Do
      End If
      r = 0: Do
        r = r + 1: Rsft t0, 1
      Loop Until -(F(t0, 0) And 1)
      If -(r And 1) Then
        r = F(t1, 0) And 7
        If r = 3 Or r = 5 Then X = -X
      End If
    End If '                          (1/N) = 1 for all N
    If Isf(t0, 1) Then Swp i2, t0: Exit Do
    r = (F(t0, 0) And 3) = 3
    S = (F(t1, 0) And 3) = 3
    Swp t0, t1: If r And S Then X = -X
  Loop
jump:
  Kronec = X
End Function

Function Les(G As String) As Integer
  G = Trim$(G)
  Les = (1 + (Len(G) - 1) \ 4) * 2
End Function

Sub Letf(i0 As Integer, A As Integer)
  F(i0, 1) = 0: F(i0, 0) = Abs(A) '   abs(a) < MD
  l(i0) = 1: sg(i0) = Sgn(A + 0.5) '  SGN(0) = 1
End Sub

Sub LetfD(i0 As Integer, C As Long, cM As Long)
Dim cs As Long, j As Integer
  sg(i0) = Sgn(C + 0.5): cs = Abs(C)
  j = 0: Do
    F(i0, j) = cs Mod cM '            split DWord c
    cs = cs \ cM: j = j + 1
  Loop Until cs = 0
  l(i0) = j: F(i0, j) = 0
End Sub

Sub Lsft(i0 As Integer, r As Integer)
Dim C As Long, c0 As Long, j As Integer
Dim l0 As Integer, Q As Integer
  Select Case r
    Case 1: Q = 2
    Case 2: Q = 4
    Case 3: Q = 8
    Case 4: Q = 16
    Case Else: Exit Sub
  End Select
  l0 = l(i0): C = 0
  For j = 0 To l0
    c0 = C + F(i0, j) * Q
    C = c0 \ MD: F(i0, j) = c0 - C * MD
  Next j
  If F(i0, l0) > 0 Then l0 = l0 + 1
  l(i0) = l0: F(i0, l0) = 0
End Sub

Function Mkl(C As Long) As String
Dim ct As Long, S As String
  S = "": ct = C: Do
    S = S + Chr$(ct And 255)
    ct = ct \ 256
  Loop Until ct = 0
  Mkl = S + String$(4 - Len(S), Chr$(0))
End Function

Sub Moddiv(i0 As Integer, i1 As Integer)
Dim S As Integer
  Divd i0, i1, t1
  If sg(i0) = -1 Then '               make positive residue
    If Isf(i0, 0) Then
      sg(i0) = 1
    Else
      S = sg(i1): sg(i1) = -1
      Subt i0, i1: sg(i1) = S
    End If
  End If
End Sub

Sub Modmlt(i0 As Integer, i1 As Integer, i2 As Integer)
  Mult i0, i1, t1
  Divd i0, i2, t1
End Sub

Sub Modpwr(i0 As Integer, i1 As Integer, i2 As Integer)
Dim C As Long, c0 As Long, c2 As Long, j As Integer
Dim k As Integer, S As Integer, Sw As Boolean
  If Isf(i1, 0) Then Letf i0, 1: Exit Sub
  Sw = Not Isf(i2, 0) '               enable reduction mod i2
  If Sw Then
    S = sg(i2): sg(i2) = 1
    Moddiv i0, i2 '                   initial reduction
  Else
    S = sg(i0): sg(i0) = 1
    If (F(i1, 0) And 1) = 0 Then S = 1
  End If
  Copyf i0, t0
  k = l(i1) - 1: c0 = MB \ 2
  C = F(i1, k): c2 = c0
  Do Until (C And c2) > 0 '           find MSB(i1)
    c2 = c2 \ 2: Loop
  C = C And (c2 - 1): c2 = c2 \ 2
  '
  For j = k To 0 Step -1 '            L=>R binary exponentiation
    If j < k Then C = F(i1, j) '      unsigned bitvector i1
    Do While c2 > 0
      Squ t0, t1: Swp t0, t1 '        square t0
      If Sw Then Divd t0, i2, t1 '    reduce mod i2
      If (C And c2) > 0 Then
        Mult t0, i0, t1 '             t0 times base i0
        If Sw Then Divd t0, i2, t1
      End If
      C = C And (c2 - 1): c2 = c2 \ 2
    Loop: c2 = c0
    DoEvents: If Key Then Exit Sub
  Next j: Swp i0, t0
  If Sw Then
    sg(i2) = S
  Else
    sg(i0) = S
  End If
End Sub

Sub Modsqu(i0 As Integer, i1 As Integer)
  Squ i0, t1: Swp i0, t1
  Divd i0, i1, t1
End Sub

Sub Mult(i0 As Integer, i1 As Integer, i2 As Integer)
Dim C As Long, c0 As Long, c2 As Long, j As Integer, k As Integer
Dim l0 As Integer, l1 As Integer, l2 As Integer, ll As Integer
Dim lx As Integer, m As Integer, Sw As Boolean
  l0 = l(i0): l1 = l(i1): lx = l0 + l1 - 1
  If lx >= UBound(F, 2) Then
    j = MsgBox("overflow in Sub Mult", 277, "Error")
    If j = 2 Then Close: End '        [Cancel]
    Letf i2, 0: Exit Sub '            [Retry]
  End If
  Sw = l0 < l1: If Sw Then Swp i0, i1
  l0 = l(i0): l1 = l(i1)
  '
  C = 0: c2 = F(i1, 0)
  For j = 0 To l0
    c0 = C + c2 * F(i0, j) '          initiate dest.
    C = c0 \ MD: F(i2, j) = c0 - C * MD
  Next j
  For j = l0 + 1 To lx: F(i2, j) = 0: Next
  '
  For m = 1 To l1 - 1 Step 22
    For ll = 0 To 21
      k = ll + m: C = F(i1, k)
      For j = 0 To l0 - 1 '           multiply,
        F(i2, j + k) = F(i2, j + k) + C * F(i0, j)
      Next j
      If k = l1 - 1 Then Exit For
    Next ll: C = 0
    For j = m To k + l0 '             normalize,
      c0 = C + F(i2, j): C = c0 \ MD
      F(i2, j) = c0 - C * MD
    Next j
  Next m: l2 = 1
  For j = lx To 0 Step -1 '           and resize
    If F(i2, j) > 0 Then l2 = j + 1: Exit For
  Next j: l(i2) = l2
  F(i2, l2) = 0: sg(i2) = sg(i0) * sg(i1)
  Swp i0, i2: If Sw Then Swp i2, i1
End Sub

Function Nxtprm(Sw As Boolean) As Long
Dim b4 As String * 4, r As Integer
Static cp As Long, cpl  As Long, C As Long, cb As Long
Static dc As Integer, fl As Boolean, i As Integer
If Sw = 0 Then
  cp = 1: cpl = LOF(2): C = 2 '       initiate
  dc = 1: i = 30: fl = 0
Else
  If fl Then
    Do: dc = 6 - dc: C = C + dc
      If i = 30 Then
        i = 0
        If cp < cpl Then '            next bitvector in PrimFlgs.bin
          Get #2, cp, b4
          cb = Cvl(b4): cp = cp + 4
        Else
          cb = 469171647
        End If
      End If
      r = cb And 1: cb = cb \ 2: i = i + 1
    Loop Until -r
  Else
    C = C + dc: dc = dc * 2: fl = C = 5
  End If
End If
  Nxtprm = C
End Function

Sub Printf(i0 As Integer, G As String, h As String, Sw As Boolean)
Dim j As Integer, ll As Integer, S As String
  S = LTrim$(Str$(F(i0, l(i0) - 1)))
  ll = Len(S): If sg(i0) = -1 Then S = "-" + S
  S = G + S: Data = Data + S
  For j = l(i0) - 2 To 0 Step -1
    S = LTrim$(Str$(F(i0, j))): ll = ll + 4
    S = String$(4 - Len(S), "0") + S
    Data = Data + S
  Next j
  If Sw = 0 Then
    Data = Data + h
  Else
    h = h + "  [" & ll & "]"
    Data = Data + h + vbCrLf
  End If
  'Slate.Box.Text = Data
End Sub

Sub Pwr10(i0 As Integer, k As Integer)
Dim j As Integer, r As Integer, S As Integer
  sg(i0) = 1: k = Abs(k)
  l(i0) = 1 + k \ 4
  If l(i0) >= UBound(F, 2) Then
    j = MsgBox("overflow in Sub Pwr10", 277, "Error")
    If j = 2 Then Close: End '        [Cancel]
    Letf i0, 0: Exit Sub '            [Retry]
  End If
  For j = 0 To l(i0)
    F(i0, j) = 0
  Next j
  r = k Mod 4: S = 1
  For j = 1 To r
    S = S * 10
  Next j
  F(i0, l(i0) - 1) = S
End Sub

Sub Ratdec(i0 As Integer, i1 As Integer, G As String, lt As Integer)
Dim j As Integer, k As Integer, lx As Integer, S As String
  Divd i0, i1, t1
  If Isf(i0, 0) Then
    Printf t1, G, "", 1
  Else
    Printf t1, G, ".", 0 '            integral part
    lx = 1 + (lt - 1) \ 4
    For k = 1 To lx
      For j = l(i0) To 1 Step -1 '    dividend * MD
        F(i0, j) = F(i0, j - 1)
      Next j: l(i0) = l(i0) + 1
      F(i0, 0) = 0: F(i0, l(i0)) = 0
      Divd i0, i1, t1 '               partial quotient
      S = LTrim$(Str$(F(t1, 0)))
      S = String$(4 - Len(S), "0") + S
      Data = Data + S '               decimal digit
    Next k
    S = "  [" & 4 * lx & "]"
    Data = Data + S + vbCrLf
    'Slate.Box.Text = Data
  End If
End Sub

Sub Readst(i0 As Integer, G As String)
Dim k As Integer, l0 As Integer, ll As Integer
  G = Trim$(G): sg(i0) = 1
  If G = "" Then G = " "
  If Left$(G, 1) = "-" Then
    G = Mid$(G, 2): sg(i0) = -1
  End If
  l0 = Len(G): ll = 4 - (l0 And 3)
  If ll < 4 Then
    G = String$(ll, " ") + G: l0 = l0 + ll
  End If
  ll = l0 - 3: l0 = l0 \ 4
  For k = ll To 1 Step -4
    F(i0, (ll - k) \ 4) = Val(Mid$(G, k, 4))
  Next k: l(i0) = l0: F(i0, l0) = 0
  G = LTrim$(G)
  If sg(i0) = -1 Then G = "-" + G
End Sub

Sub Rsft(i0 As Integer, r As Integer)
Dim C As Long, c0 As Long, j As Integer
Dim l0 As Integer, Q As Integer
  Select Case r
    Case 1: Q = 5000
    Case 2: Q = 2500
    Case 3: Q = 1250
    Case 4: Q = 625
    Case Else: Exit Sub
  End Select
  l0 = l(i0) - 1
  For j = 0 To l0
    F(i0, j) = F(i0, j) * Q
  Next j: C = 0
  For j = l0 To 0 Step -1
    c0 = F(i0, j) Mod MD
    F(i0, j) = F(i0, j) \ MD + C: C = c0
  Next j
  If F(i0, l0) = 0 And l0 > 0 Then l(i0) = l0
  If Isf(i0, 0) Then sg(i0) = 1
End Sub

Sub Squ(i0 As Integer, i1 As Integer)
Dim C As Long, c0 As Long, j As Integer
Dim k As Integer, l0 As Integer, l1 As Integer
Dim ll As Integer, lx As Integer, m As Integer
  l0 = l(i0): lx = l0 + l0 - 1
  If lx >= UBound(F, 2) Then
    j = MsgBox("overflow in Sub Squ", 277, "Error")
    If j = 2 Then Close: End '        [Cancel]
    Letf i1, 0: Exit Sub '            [Retry]
  End If
  If l0 = 1 Then
    C = F(i0, 0) * F(i0, 0)
    LetfD i1, C, MD
  Else
    j = 0: For k = 0 To l0 - 1
      C = F(i0, k) * F(i0, k) '       initiate dest.
      F(i1, j) = C Mod MD
      F(i1, j + 1) = C \ MD: j = j + 2
    Next k: sg(i1) = 1
    '
    For m = 1 To l0 - 1 Step 11
      For ll = 0 To 10 '              add mixed terms
        k = ll + m: C = F(i0, k) * 2
        For j = 0 To k - 1
          F(i1, j + k) = F(i1, j + k) + C * F(i0, j)
        Next j
        If k = l0 - 1 Then Exit For
      Next ll: C = 0
      For j = m To k + k + 1
        c0 = C + F(i1, j): C = c0 \ MD
        F(i1, j) = c0 - C * MD
      Next j
    Next m: l1 = 1
    For j = lx To 0 Step -1
      If F(i1, j) > 0 Then l1 = j + 1: Exit For
    Next j: l(i1) = l1: F(i1, l1) = 0
  End If
End Sub

Sub Subt(i0 As Integer, i1 As Integer)
Dim im As Integer, ix As Integer, j As Integer
Dim lm As Integer, lx As Integer, S As Integer
  If sg(i0) = sg(i1) Then '           subtract
    S = l(i0) - l(i1): ix = i0: im = i1
    If S = 0 Then
      For j = l(i0) - 1 To 0 Step -1
        S = F(i0, j) - F(i1, j)
        If S <> 0 Then Exit For
      Next j
    End If
    Select Case S
    Case Is < 0 '                     i0 := -(i1 - i0)
      sg(i0) = -sg(i0): Swp ix, im
    Case 0
      Letf i0, 0: Exit Sub
    End Select
    lx = l(ix): lm = l(im)
    For j = lm + 1 To lx: F(im, j) = 0: Next
    S = 0: For j = 0 To lx
      S = S + F(ix, j) - F(im, j)
      If S < 0 Then
        F(i0, j) = S + MD: S = -1
      Else
        F(i0, j) = S: S = 0
      End If
    Next j
  Else '                              add
    lx = l(i0): lm = l(i1): im = i1
    If lx < lm Then Swp lx, lm: im = i0
    For j = lm + 1 To lx: F(im, j) = 0: Next
      S = 0: For j = 0 To lx
      S = S + F(i0, j) + F(i1, j)
      If S < MD Then
        F(i0, j) = S: S = 0
      Else
        F(i0, j) = S - MD: S = 1
      End If
    Next j
  End If
  lm = 1: For j = lx To 0 Step -1
    If F(i0, j) > 0 Then lm = j + 1: Exit For
  Next j: l(i0) = lm: F(i0, lm) = 0
End Sub

Sub Swp(i0 As Integer, i1 As Integer)
Dim r As Integer
  r = i0: i0 = i1: i1 = r
End Sub


Public Sub HexToBigInt(Tekst As String, bigint As Integer)
    '!!! uses 0,1,2,3 registers
    Dim i As Integer
    Dim A As Integer
    Dim B As Integer
    Dim C As Integer
    Dim E As Integer
    t0 = 0
    'Open "log.txt" For Output As #9
    
        A = 1
        Readst A, "1"
        'Data = ""
        'Printf A, "", "", False
        'Print #9, "A=1=" & Data
        B = 2
        Readst B, "16"
        'Data = ""
        'Printf B, "", "", False
        'Print #9, "B=16=" & Data
        C = bigint
        Readst C, "0"
        'Data = ""
        'Printf C, "", "", False
        'Print #9, "C=0=" & Data
        E = 3
        For i = 0 To Len(Tekst) - 1
            Readst E, Str(Val("&H" & Mid(Tekst, Len(Tekst) - i, 1)))
                'Data = ""
                'Printf E, "", "", False
                'Print #9, "E=" & Data
            Mult E, A, t0
                'Data = ""
                'Printf E, "", "", False
                'Print #9, "E=E*A=" & Data
            Add C, E
                'Data = ""
                'Printf C, "", "", False
                'Print #9, "C=C+E=" & Data
            Mult A, B, t0
                'Data = ""
                'Printf A, "", "", False
                'Print #9, "A=B*A=" & Data
        Next i
        'c = Tekst as BigInt
        'Printf C, "", "", False
        'MsgBox Data
    'bigint = C
    'Close #9
End Sub

Public Function BigIntToHex(ByVal bigint As Integer) As String
    Dim i As Integer
    Dim A As Integer
    Dim B As Integer
    Dim C As Integer
    Dim temp As String
    
    A = 1
    B = 2
    C = 3
    t0 = 12
    t1 = 11
    t2 = 10
    Copyf bigint, A
    Readst B, "16"
    temp = ""
    
    While Cmp(A, B) > 0
        Copyf A, C
            'Data = ""
            'Printf B, "", "", False
            'Log "debug info = " & Data
        Divd C, B, A

        Data = ""
        Printf C, "", "", False
        temp = hex(Val(Data)) + temp
    Wend
    
    Data = ""
    Printf A, "", "", False
    temp = hex(Val(Data)) + temp
    BigIntToHex = temp
End Function

Public Function ModExp(Base As Integer, exp As Integer, modulus As Integer, Result As Integer)
'  Modular exponentiation: base i0, binary exponent i1, modulus i2,
'  the result is in i0. Modulo reduction is disabled if i2 is empty.
    
'!!! uses 4,5,6 registers
'!!! modpwr uses 10,11,12 registers

    Dim temp1 As Integer
    Dim temp2 As Integer
    Dim temp3 As Integer
    temp1 = 1: temp2 = 2: temp3 = 3
    Copyf Base, temp3
    Copyf exp, temp2
    Copyf modulus, temp1

    t0 = 12
    t1 = 11
    t2 = 10
    Binf temp2
    Modpwr temp3, temp2, temp1
    If Ismin1(temp1, temp3) Then Letf temp1, -1
    Swp temp1, temp2
    
    Copyf temp3, Result
End Function

