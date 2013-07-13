Attribute VB_Name = "modSRP6"
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

Option Explicit

Const G_Length = 1
Const G = 7
Const k = 3
Const Default_N = "894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7"
Global Default_B As String
Global Default_Salt As String
Dim AccV As String
Dim AccB As String
Dim AccountName As String
Dim SSHash As String
Public TrafficKey() As Byte
    
Sub Answer_CLIENT_AUTH_LOGON_CHALLENGE(AccName As String)
    
    Dim bigint1 As Integer
    bigint1 = 17
    Dim bigint2 As Integer
    bigint2 = 18
    Dim bigint3 As Integer
    bigint3 = 19
    Dim bigint4 As Integer
    bigint4 = 20
    Dim bigint5 As Integer
    bigint5 = 16
    
    Dim N As String
    Dim B As String
    Dim X As String
    Dim Salt As String
    Dim PublicB As String
    Dim CrcSalt As String
    'SHA1 Hash
    B = UCase("8692E3A6BA48B5B1004CEF76825127B7EB7D1AEF")
    Salt = UCase("33f140d46cb66e631fdbbbc9f029ad8898e05ee533876118185e56dde843674f")
    AccountName = AccName
    N = Default_N
    X = GenerateX(AccName, AccName, Salt)
    X = Reverse(X)
    N = Reverse(N)
    'SRP6 Calculation
    HexToBigInt N, bigint1
    HexToBigInt X, bigint2
    HexToBigInt G, bigint3
    ModExp bigint3, bigint2, bigint1, bigint4
    AccV = BigIntToHex(bigint4)
    HexToBigInt k, bigint1
    Mult bigint4, bigint1, bigint2
    B = Reverse(B)
    HexToBigInt N, bigint1
    HexToBigInt B, bigint2
    ModExp bigint3, bigint2, bigint1, bigint5
    Add bigint5, bigint4
    Divd bigint5, bigint1, bigint4
    Copyf bigint5, bigint1
    PublicB = Reverse(BigIntToHex(bigint1))
    AccB = PublicB
    N = Default_N
    CrcSalt = RandomHEX(16)
    'Sending packet
    
                    Dim i As Integer
                    Dim data_response(118) As Byte
                    data_response(0) = RS_LOGON_CHALLENGE
                    data_response(1) = 0
                    data_response(2) = login_no_error
                    For i = 3 To 34
                        data_response(i) = Val("&H" & Mid(PublicB, (i - 3) * 2 + 1, 2))
                    Next i
                    data_response(35) = G_Length
                    data_response(36) = Int(G)
                    data_response(37) = 32
                    For i = 38 To 69
                        data_response(i) = Val("&H" & Mid(N, (i - 38) * 2 + 1, 2))
                    Next i
                    For i = 70 To 101
                        data_response(i) = Val("&H" & Mid(Salt, (i - 70) * 2 + 1, 2))
                    Next i
                    For i = 102 To 117
                        data_response(i) = Val("&H" & Mid(CrcSalt, (i - 102) * 2 + 1, 2))
                    Next i
                    data_response(118) = 0
        
                    On Error Resume Next
                    frmMain.sckPRealm.SendData data_response
End Sub

Function Reverse(strString As String) As String
    Dim i As Integer
    Dim output As String
    output = ""
    For i = Len(strString) - 1 To 1 Step -2
        output = output + Mid(strString, i, 2)
    Next i
    Reverse = output
End Function

Sub Answer_CLIENT_LOGON_PROOF(ClientA As String, ClientM1 As String)
    
    Dim i As Integer
    Dim bigint1 As Integer
    bigint1 = 15
    Dim bigint2 As Integer
    bigint2 = 14
    Dim bigint3 As Integer
    bigint3 = 13
    Dim bigint4 As Integer
    bigint4 = 7
    Dim bigint5 As Integer
    bigint5 = 8
    
    Dim U As String
    Dim N As String
    Dim A As String
    Dim B As String
    Dim Salt As String
    Dim S As String
    Dim M1 As String
    Dim PublicB As String
    Dim V As String
    
    'DEBUG Values:
    '****************************************************************************
    N = Default_N
    B = UCase("8692E3A6BA48B5B1004CEF76825127B7EB7D1AEF")
    Salt = UCase("33f140d46cb66e631fdbbbc9f029ad8898e05ee533876118185e56dde843674f")
    'B = Default_B
    'Salt = Default_Salt
    '****************************************************************************
    A = ClientA
    PublicB = AccB
    V = AccV
    
    Dim temp1 As String
    Dim temp2 As String
    
    U = modSHA1.sha1(HexToStr(A) + HexToStr(PublicB))
    U = Reverse(U)
    
    N = Reverse(N)
    A = Reverse(A)
    B = Reverse(B)
    
    HexToBigInt V, bigint1
    HexToBigInt U, bigint2
    HexToBigInt N, bigint3
    
    ModExp bigint1, bigint2, bigint3, bigint4
        
    HexToBigInt A, bigint1
    Mult bigint4, bigint1, bigint3
    
    HexToBigInt B, bigint1
    HexToBigInt N, bigint3
    ModExp bigint4, bigint1, bigint3, bigint2
    S = BigIntToHex(bigint2)
    S = Reverse(S)
    temp1 = ""
    temp2 = ""
    For i = 1 To Len(S) Step 2
        If i \ 2 Mod 2 = 0 Then
            temp1 = temp1 + Mid(S, i, 2)
        Else
            temp2 = temp2 + Mid(S, i, 2)
        End If
    Next i

    temp1 = HexToStr(temp1)
    temp2 = HexToStr(temp2)
    temp1 = modSHA1.sha1(temp1)
    temp2 = modSHA1.sha1(temp2)

    SSHash = ""
    For i = 1 To Len(temp1) Step 2
        SSHash = SSHash + Mid(temp1, i, 2) + Mid(temp2, i, 2)
    Next i
        
    'Calc M1 to verify it matches the M1 supplied by the client
    Dim N_Hash As String
    Dim G_Hash As String
    Dim NG_Hash As String
    Dim User_Hash As String
    
    User_Hash = modSHA1.sha1(UCase(AccountName))
    N = Reverse(N)
    N_Hash = modSHA1.sha1(HexToStr(N))
    G_Hash = modSHA1.sha1(Chr(7))
    NG_Hash = ""
    For i = 1 To Len(N_Hash)
        NG_Hash = NG_Hash + hex(Val("&H" & Mid(N_Hash, i, 1)) Xor Val("&H" & Mid(G_Hash, i, 1)))
    Next i
    
    temp1 = NG_Hash & User_Hash & Salt & Reverse(A) & PublicB & SSHash
    M1 = modSHA1.sha1(HexToStr(temp1))
    
    '!!! Verify here client M1 and calculated M1
        Dim data_response() As Byte
        If UCase(M1) = UCase(ClientM1) Then
                'pass accepted, calculate M2 and send it
                temp1 = Reverse(A) & M1 & SSHash
                M1 = modSHA1.sha1(HexToStr(temp1))
                TrafficKey = HexToBytes(SSHash)
                ReDim data_response(25) As Byte
                data_response(0) = RS_LOGON_PROOF
                data_response(1) = login_no_error
                For i = 2 To 21
                    data_response(i) = Val("&H" & Mid(M1, (i - 2) * 2 + 1, 2))
                Next i
                data_response(22) = 0
                data_response(23) = 0
                data_response(24) = 0
                data_response(25) = 0
                frmMain.sckPRealm.SendData data_response
                
            Else
                'wrong pass
                AddPacketHeader data_response, RS_LOGON_PROOF
                AddPacketInt8 data_response, login_bad_user
                AddPacketInt8 data_response, 3
                AddPacketInt8 data_response, 0
                frmMain.sckPRealm.SendData data_response
                
                frmMain.txtLog.Text = frmMain.txtLog.Text & "Please use the same password as the username in this phase." & vbNewLine
        End If
End Sub
