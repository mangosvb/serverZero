VERSION 5.00
Object = "{248DD890-BB45-11CF-9ABC-0080C7E7B78D}#1.0#0"; "MSWINSCK.ocx"
Object = "{3B7C8863-D78F-101B-B9B5-04021C009402}#1.2#0"; "RICHTX32.OCX"
Begin VB.Form frmMain 
   ClientHeight    =   5220
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   6015
   LinkTopic       =   "Form1"
   ScaleHeight     =   5220
   ScaleWidth      =   6015
   StartUpPosition =   3  'Windows Default
   Begin MSWinsockLib.Winsock sckPWorld 
      Left            =   2760
      Top             =   960
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin MSWinsockLib.Winsock sckPRealm 
      Left            =   0
      Top             =   960
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin RichTextLib.RichTextBox txtLog 
      Height          =   3375
      Left            =   120
      TabIndex        =   11
      Top             =   1800
      Width           =   5775
      _ExtentX        =   10186
      _ExtentY        =   5953
      _Version        =   393217
      Enabled         =   -1  'True
      ScrollBars      =   2
      Appearance      =   0
      TextRTF         =   $"Form1.frx":0000
   End
   Begin VB.Timer tmrFindWoW 
      Enabled         =   0   'False
      Interval        =   500
      Left            =   960
      Top             =   1440
   End
   Begin MSWinsockLib.Winsock sckWListen 
      Left            =   480
      Top             =   1440
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin MSWinsockLib.Winsock sckRListen 
      Left            =   0
      Top             =   1440
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin VB.CommandButton cmdListen 
      Caption         =   "Start Listen"
      Height          =   255
      Left            =   1920
      TabIndex        =   10
      Top             =   1440
      Width           =   1935
   End
   Begin VB.Frame frmOfficial 
      Caption         =   "Official"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1215
      Left            =   3000
      TabIndex        =   5
      Top             =   120
      Width           =   2895
      Begin VB.TextBox txtOPort 
         Appearance      =   0  'Flat
         Height          =   285
         Left            =   2280
         TabIndex        =   12
         Text            =   "3724"
         Top             =   360
         Width           =   495
      End
      Begin VB.TextBox txtORealm 
         Appearance      =   0  'Flat
         Height          =   285
         Left            =   720
         TabIndex        =   9
         Text            =   "vWoW"
         Top             =   720
         Width           =   2055
      End
      Begin VB.TextBox txtOIP 
         Appearance      =   0  'Flat
         Height          =   285
         Left            =   720
         TabIndex        =   7
         Text            =   "127.0.0.1"
         Top             =   360
         Width           =   1455
      End
      Begin VB.Label lblRealm 
         Caption         =   "Realm:"
         Height          =   255
         Left            =   120
         TabIndex        =   8
         Top             =   720
         Width           =   615
      End
      Begin VB.Label lblIP 
         Caption         =   "IP:"
         Height          =   255
         Left            =   120
         TabIndex        =   6
         Top             =   360
         Width           =   375
      End
   End
   Begin VB.Frame frmListen 
      Caption         =   "Listen"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1215
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   2775
      Begin VB.TextBox txtWPort 
         Appearance      =   0  'Flat
         Height          =   285
         Left            =   1080
         TabIndex        =   4
         Text            =   "3725"
         Top             =   720
         Width           =   1575
      End
      Begin VB.TextBox txtRPort 
         Appearance      =   0  'Flat
         Height          =   285
         Left            =   1080
         TabIndex        =   2
         Text            =   "3726"
         Top             =   360
         Width           =   1575
      End
      Begin VB.Label lblWorldPort 
         Caption         =   "World Port:"
         Height          =   255
         Left            =   120
         TabIndex        =   3
         Top             =   720
         Width           =   855
      End
      Begin VB.Label lblRealmPort 
         Caption         =   "Realm Port:"
         Height          =   255
         Left            =   120
         TabIndex        =   1
         Top             =   360
         Width           =   855
      End
   End
   Begin MSWinsockLib.Winsock sckOWorld 
      Left            =   2760
      Top             =   480
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin MSWinsockLib.Winsock sckFWorld 
      Left            =   0
      Top             =   480
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin MSWinsockLib.Winsock sckORealm 
      Left            =   2760
      Top             =   0
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
   Begin MSWinsockLib.Winsock sckFRealm 
      Left            =   0
      Top             =   0
      _ExtentX        =   741
      _ExtentY        =   741
      _Version        =   393216
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
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
 
Private Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal length As Long)

Dim WorldIP As String, WorldPort As Long, RealmListSize As Integer, RealmListRecieved As Integer
Dim RealmListPacket() As Byte, RealmCount As Integer
Public GotSSHash As Boolean
Public GotPointer As Boolean
Public RealmConnected As Boolean
Public WorldConnected As Boolean
Dim Decoding As Boolean
Dim PacketFinishedF As Boolean
Dim LastPacketF() As Byte
Dim PacketFinishedO As Boolean
Dim LastPacketO() As Byte

Private Sub cmdListen_Click()
    If cmdListen.Caption = "Start Listen" Then
        txtRPort.Enabled = False
        txtWPort.Enabled = False
        txtOIP.Enabled = False
        txtOPort.Enabled = False
        txtORealm.Enabled = False
        sckRListen.LocalPort = txtRPort.Text
        sckWListen.LocalPort = txtWPort.Text
        sckRListen.Listen
        sckWListen.Listen
        cmdListen.Caption = "Stop Listen"
        txtLog.Text = txtLog.Text & "Started listening..." & vbNewLine
    Else
        sckFRealm.Close
        sckORealm.Close
        sckFWorld.Close
        sckOWorld.Close
        sckRListen.Close
        sckWListen.Close
        txtRPort.Enabled = True
        txtWPort.Enabled = True
        txtOIP.Enabled = True
        txtOPort.Enabled = True
        txtORealm.Enabled = True
        cmdListen.Caption = "Start Listen"
        txtLog.Text = txtLog.Text & "Closed connections and stopped listening..." & vbNewLine
    End If
End Sub

Private Sub Form_Load()
    'This are indexes for SRP6 calculations
    ReDim F(20, 4095), sg(20), l(20)
    
    ReDim LastPacketO(0)
    ReDim LastPacketF(0)
    PacketFinishedO = True
    PacketFinishedF = True
    
    GotPointer = False
    GotSSHash = False
    SSHashPointer = 0
    
    timeBeginPeriod 1
    
    If Dir(App.Path & "/Packets.log") <> "" Then Kill App.Path & "/Packets.log"
End Sub

Private Sub Form_Unload(Cancel As Integer)
    End
End Sub

Private Sub sckFRealm_Close()
    sckORealm.Close
    txtLog.Text = txtLog.Text & "Client closed the proxy realm connection." & vbNewLine
End Sub

Private Sub sckFRealm_DataArrival(ByVal bytesTotal As Long)
    Dim data() As Byte, i As Long, Response() As Byte
    sckFRealm.GetData data, vbByte + vbArray
    
    If sckORealm.State <> sckConnected Then sckFRealm.Close: Exit Sub
    sckORealm.SendData data
    
    LogPacket CInt(data(0)), UBound(data), data, False, True
End Sub

Private Sub sckFRealm_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    sckFRealm.Close
    sckORealm.Close
    txtLog.Text = txtLog.Text & "Error FRealm (" & Number & "): " & Description
End Sub

Private Sub sckFWorld_Close()
    sckOWorld.Close
    txtLog.Text = txtLog.Text & "Client closed the world connection." & vbNewLine
End Sub

Private Sub sckFWorld_DataArrival(ByVal bytesTotal As Long)
    Dim data() As Byte, Packets() As tPacketData, PacketNum As Integer, packet_len As Integer, OPCode As Integer
    sckFWorld.GetData data, vbByte + vbArray
    
    If sckOWorld.State <> sckConnected Then sckFWorld.Close: Exit Sub
    
    OPCode = GetInt16FromPacket(data, 2)
    If PacketFinishedF = False Then
        PacketFinishedF = True
        Dim PacketIndex As Long
        PacketIndex = UBound(LastPacketF)
        ReDim Preserve LastPacketF(PacketIndex + 1 + UBound(data))
        CopyMemory LastPacketF(PacketIndex + 1), data(0), UBound(data) + 1
        data = LastPacketF
        packet_len = GetInt16FromPacket(data, 0, True)
        ReDim LastPacketF(0)
    Else
        If Decoding Then
            sckOWorld.SendData data
            DecodeC data
        ElseIf OPCode = CMSG_AUTH_SESSION Then
            'Lets wait until we got the SS Hash
            Do While GotSSHash = False
                DoEvents
            Loop
            sckOWorld.SendData data
            
            ClientKey(0) = 0: ClientKey(1) = 0: ClientKey(2) = 0: ClientKey(3) = 0
            ServerKey(0) = 0: ServerKey(1) = 0: ServerKey(2) = 0: ServerKey(3) = 0
            Decoding = True
            txtLog.Text = txtLog.Text & "Starting decoding." & vbNewLine
        End If
    End If
    
    packet_len = GetInt16FromPacket(data, 0, True)
    If packet_len > (UBound(data) - 1) Then
        PacketFinishedF = False
        LastPacketF = data
        Exit Sub
    End If
    If packet_len < 0 Then Exit Sub
    
    ReDim Packets(0)
    While packet_len < UBound(data) - 1
        If packet_len > 0 Then
            Dim tmpBuff() As Byte
            ReDim tmpBuff(UBound(data) - packet_len - 2)
            CopyMemory tmpBuff(0), data(packet_len + 2), UBound(tmpBuff) + 1
            ReDim Preserve data(packet_len + 1)
            AddPacketToQueue Packets, data
            data = tmpBuff
            If UBound(data) < 5 Then
                PacketFinishedF = False
                LastPacketF = sckData
                GoTo PacketsDone
            End If
            If Decoding Then DecodeC data
            packet_len = GetInt16FromPacket(data, 0, True)
            If packet_len < 0 Then GoTo PacketsDone
            If packet_len > (UBound(data) - 1) Then
                PacketFinishedF = False
                LastPacketF = data
                GoTo PacketsDone
            End If
        End If
    Wend
    AddPacketToQueue Packets, data
    
PacketsDone:
    For PacketNum = 0 To UBound(Packets) - 1
        With Packets(PacketNum)
            packet_len = GetInt16FromPacket(.data, 0, True)
            OPCode = GetInt16FromPacket(.data, 2)
            
            If OPCode = CMSG_WARDEN_DATA Then
                Dim b() As Byte
                ReDim b(UBound(.data) - 6)
                CopyMemory b(0), .data(6), UBound(b) + 1
                Crypt .data, KeyOut
                CopyMemory .data(6), b(0), UBound(b) + 1
            End If
            
            LogPacket OPCode, packet_len, .data, False
        End With
    Next PacketNum
End Sub

Private Sub sckFWorld_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    sckFWorld.Close
    sckOWorld.Close
    txtLog.Text = txtLog.Text & "Error FWorld (" & Number & "): " & Description
End Sub

Private Sub sckORealm_Close()
    sckFRealm.Close
    sckORealm.Close
    txtLog.Text = txtLog.Text & "Official realm server closed the connection." & vbNewLine
End Sub

Private Sub sckORealm_Connect()
    RealmConnected = True
    txtLog.Text = txtLog.Text & "Connected to official realm server." & vbNewLine
End Sub

Private Sub sckORealm_DataArrival(ByVal bytesTotal As Long)
    Dim data() As Byte
    sckORealm.GetData data, vbByte + vbArray
    
    If RealmListRecieved < RealmListSize Then 'Realmlist continues
        RealmListRecieved = RealmListRecieved + bytesTotal
        AddPacketByteArray RealmListPacket, data
        
        If RealmListRecieved = RealmListSize Then HandleRealmList RealmListPacket
        Exit Sub
    ElseIf data(0) = &H10 Then 'Realmlist
        RealmListSize = (GetInt16FromPacket(data, 1) + 3)
        RealmListRecieved = bytesTotal
        RealmCount = GetInt16FromPacket(data, 7)
        RealmListPacket = data
        
        If RealmListRecieved = RealmListSize Then HandleRealmList RealmListPacket
        Exit Sub
    End If
    
    If sckFRealm.State <> sckConnected Then sckORealm.Close: Exit Sub
    sckFRealm.SendData data
    
    LogPacket CInt(data(0)), UBound(data), data, True, True
End Sub

Private Sub sckORealm_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    sckFRealm.Close
    sckORealm.Close
    txtLog.Text = txtLog.Text & "Error ORealm (" & Number & "): " & Description
End Sub

Private Sub sckOWorld_Close()
    sckFWorld.Close
    sckOWorld.Close
    txtLog.Text = txtLog.Text & "Official world server closed the connection." & vbNewLine
End Sub

Private Sub sckOWorld_Connect()
    WorldConnected = True
    txtLog.Text = txtLog.Text & "Connected to official world server." & vbNewLine
End Sub

Private Sub sckOWorld_DataArrival(ByVal bytesTotal As Long)
    Dim data() As Byte, Packets() As tPacketData, PacketNum As Integer, packet_len As Integer, OPCode As Integer
    sckOWorld.GetData data, vbByte + vbArray
    
    If sckFWorld.State <> sckConnected Then sckOWorld.Close: Exit Sub
    sckFWorld.SendData data
    
    OPCode = GetInt16FromPacket(data, 2)
    If PacketFinishedO = False Then
        PacketFinishedO = True
        Dim PacketIndex As Long
        PacketIndex = UBound(LastPacketO)
        ReDim Preserve LastPacketO(PacketIndex + 1 + UBound(data))
        CopyMemory LastPacketO(PacketIndex + 1), data(0), UBound(data) + 1
        data = LastPacketO
        ReDim LastPacketO(0)
    Else
        If Decoding Then
            DecodeS data
        ElseIf OPCode = SMSG_AUTH_CHALLENGE Then
            tmrFindWoW.Enabled = True
        End If
    End If
    
    packet_len = GetInt16FromPacket(data, 0, True)
    If packet_len > (UBound(data) - 1) Then
        PacketFinishedO = False
        LastPacketO = data
        Exit Sub
    End If
    If packet_len < 0 Then Exit Sub
    
    ReDim Packets(0)
    While packet_len < UBound(data) - 1
        If packet_len > 0 Then
            Dim tmpBuff() As Byte
            ReDim tmpBuff(UBound(data) - packet_len - 2)
            CopyMemory tmpBuff(0), data(packet_len + 2), UBound(tmpBuff) + 1
            ReDim Preserve data(packet_len + 1)
            AddPacketToQueue Packets, data
            data = tmpBuff
            If UBound(data) < 3 Then
                PacketFinishedO = False
                LastPacketO = data
                GoTo PacketsDone
            End If
            If Decoding Then DecodeS data
            packet_len = GetInt16FromPacket(data, 0, True)
            If packet_len < 0 Then GoTo PacketsDone
            If packet_len > (UBound(data) - 1) Then
                PacketFinishedO = False
                LastPacketO = data
                GoTo PacketsDone
            End If
        End If
    Wend
    AddPacketToQueue Packets, data
    
PacketsDone:
    For PacketNum = 0 To UBound(Packets) - 1
        With Packets(PacketNum)
            packet_len = GetInt16FromPacket(.data, 0, True)
            OPCode = GetInt16FromPacket(.data, 2)
            
            If OPCode = SMSG_WARDEN_DATA Then
                Dim b() As Byte
                ReDim b(UBound(.data) - 4)
                CopyMemory b(0), .data(4), UBound(b) + 1
                Crypt .data, KeyIn
                CopyMemory .data(4), b(0), UBound(b) + 1
            End If
            
            LogPacket OPCode, packet_len, .data, True
        End With
    Next PacketNum
End Sub

Private Sub sckOWorld_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    sckFWorld.Close
    sckOWorld.Close
    txtLog.Text = txtLog.Text & "Error OWorld (" & Number & "): " & Description
End Sub

Private Sub HandleRealmList(data() As Byte)
    Dim i As Long, RealmName As String, RealmIP As String, PckPos As Long, IPSplit() As String
    If RealmCount <= 0 Then MsgBox "I'm sorry but the realmlist is empty. Try again later.", vbExclamation, "Empty realmlist": Exit Sub
    
    PckPos = 8
    For i = 1 To RealmCount
        PckPos = PckPos + 5
        If PckPos > UBound(data) Then Exit Sub
        RealmName = GetStringFromPacket(data, PckPos)
        If PckPos > UBound(data) Then Exit Sub
        RealmIP = GetStringFromPacket(data, PckPos)
        PckPos = PckPos + 7
        
        If LCase(Replace(RealmName, "'", "")) = LCase(Replace(txtORealm.Text, "'", "")) Then
            'Found the realm we were looking for
            IPSplit = Split(RealmIP, ":")
            WorldIP = IPSplit(0)
            WorldPort = CLng(IPSplit(1))
            
            AddPacketHeader data, &H10
            AddPacketInt16 data, 0
            
            AddPacketInt32 data, 0
            
            AddPacketInt8 data, 1
            AddPacketInt8 data, 1
            AddPacketInt8 data, 0
            AddPacketInt8 data, 0
            AddPacketInt8 data, 0
            AddPacketInt8 data, 1
            AddPacketString data, RealmName
            AddPacketString data, "127.0.0.1:" & CLng(txtWPort.Text)
            AddPacketFloat data, -1.6
            AddPacketInt8 data, 1
            AddPacketInt8 data, 1
            AddPacketInt8 data, 0
            AddPacketInt8 data, 2
            AddPacketInt8 data, 0
            FinishPacket data
            sckFRealm.SendData data
            
            LogPacket CInt(data(0)), UBound(data), data, True, True
            txtLog.Text = txtLog.Text & "Sending fake realmlist." & vbNewLine
            Exit Sub
        End If
    Next i
    
    sckFRealm.SendData data
    LogPacket CInt(data(0)), UBound(data), data, True, True
    MsgBox "The realm was not found in the list, sending the official one instead." & vbNewLine & "The rest will not be logged.", vbExclamation, "Realm not found"
End Sub

Private Sub sckPRealm_Close()
    txtLog.Text = txtLog.Text & "Client closed the fake realm connection." & vbNewLine
End Sub

Private Sub sckPRealm_DataArrival(ByVal bytesTotal As Long)
    Dim data() As Byte, data_response() As Byte
    sckPRealm.GetData data, vbByte + vbArray
    
    Select Case data(0)
        Case RS_LOGON_CHALLENGE, RS_RECONNECT_CHALLENGE
            txtLog.Text = txtLog.Text & "RS_LOGON_CHALLENGE" & vbNewLine
            Dim AccountName As String
            AccountName = GetStringFromPacket(data, 34)
            txtLog.Text = txtLog.Text & "Login with: " & AccountName & vbNewLine
            Answer_CLIENT_AUTH_LOGON_CHALLENGE AccountName
            
        Case RS_LOGON_PROOF, RS_RECONNECT_PROOF
            txtLog.Text = txtLog.Text & "RS_LOGON_PROOF" & vbNewLine
            
            Dim A As String
            Dim M1 As String
            A = ""
            M1 = ""
            For i = 1 To 32
                If Len(hex(data(i))) < 2 Then
                    A = A + "0" + hex(data(i))
                Else
                    A = A + hex(data(i))
                End If
            Next i
            For i = 33 To 33 + 19
                If Len(hex(data(i))) < 2 Then
                    M1 = M1 + "0" + hex(data(i))
                Else
                    M1 = M1 + hex(data(i))
                End If
            Next i
            
            Answer_CLIENT_LOGON_PROOF A, M1
            
        Case RS_REALMLIST
            txtLog.Text = txtLog.Text & "RS_REALMLIST" & vbNewLine
            AddPacketHeader data_response, &H10
            AddPacketInt16 data_response, 0
            
            AddPacketInt8 data_response, data(1)
            AddPacketInt8 data_response, data(2)
            AddPacketInt8 data_response, data(3)
            AddPacketInt8 data_response, data(4)
            
            AddPacketInt8 data_response, 1
            AddPacketInt8 data_response, 1
            AddPacketInt8 data_response, 0
            AddPacketInt8 data_response, 0
            AddPacketInt8 data_response, 0
            AddPacketInt8 data_response, 1
            AddPacketString data_response, "Join this realm"
            AddPacketString data_response, "127.0.0.1:" & CLng(txtWPort.Text)
            AddPacketFloat data_response, -1.6
            AddPacketInt8 data_response, 1
            AddPacketInt8 data_response, 1
            AddPacketInt8 data_response, 0
            AddPacketInt8 data_response, 2
            AddPacketInt8 data_response, 0
            FinishPacket data_response
            sckPRealm.SendData data_response
    End Select
End Sub

Private Sub sckPRealm_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    sckPRealm.Close
    txtLog.Text = txtLog.Text & "Error PRealm (" & Number & "): " & Description
End Sub

Private Sub sckPWorld_Close()
    txtLog.Text = txtLog.Text & "Client closed the world connection." & vbNewLine
End Sub

Private Sub sckPWorld_DataArrival(ByVal bytesTotal As Long)
    On Error GoTo ErrorData
    Dim data() As Byte, OPCode As Integer, lngPid As Long, i As Long, j As Long
    sckPWorld.GetData data, vbByte + vbArray
    
    OPCode = GetInt16FromPacket(data, 2)
    If OPCode = CMSG_AUTH_SESSION Then
        'Search the memory for the ss hash
        'Find the window
        lngHWnd = FindWindow(vbNullString, "World of Warcraft")
        
        If lngHWnd Then
            GetWindowThreadProcessId lngHWnd, lngPid
            'Open process:
            lngPHandle = OpenSecureProcess("World of Warcraft", Me.hWnd, PROCESS_ALL_ACCESS)
            If lngPHandle Then
                txtLog.Text = txtLog.Text & "Starting scan of memory. Please wait." & vbNewLine
                If SetPriorityClass(GetCurrentProcess, &H80) = 0 Then
                    txtLog.Text = txtLog.Text & "Could not set CPU priority to high. This may take a while now." & vbNewLine
                End If
                
                txtLog.Text = txtLog.Text & "Looking for: " & BytesToHex(TrafficKey) & vbNewLine
                i = &H5000000
                Do While SSHashPointer = 0
                    If GetMemByte(i) = TrafficKey(0) Then
                        For j = 1 To 39
                            If GetMemByte(i + j) <> TrafficKey(j) Then Exit For
                            If j = 39 Then SSHashPointer = i
                        Next j
                    End If
                    i = i + 1
                    If i > &HD000000 Then GoTo ErrorData
                    If (i Mod 100) = 0 Then
                        DoEvents
                    End If
                Loop
                txtLog.Text = txtLog.Text & "SS Hash Pointer found at: 0x" & FormatHex(hex(SSHashPointer), 8) & "." & vbNewLine
                GotPointer = True
                sckPWorld.Close
            Else
                txtLog.Text = txtLog.Text & "Could not hook into the WoW process." & vbNewLine
                sckPWorld.Close
            End If
        Else
            txtLog.Text = txtLog.Text & "WoW window could not be found." & vbNewLine
            sckPWorld.Close
        End If
    End If
    
    Exit Sub
ErrorData:
    txtLog.Text = txtLog.Text & "Scan finished, the memory address could not be found." & vbNewLine
    sckPWorld.Close
End Sub

Private Sub sckPWorld_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    sckPWorld.Close
    txtLog.Text = txtLog.Text & "Error PWorld (" & Number & "): " & Description
End Sub

Private Sub sckRListen_ConnectionRequest(ByVal requestID As Long)
    txtLog.Text = txtLog.Text & "Connection request for realm server." & vbNewLine
    If GotPointer = False Then
        txtLog.Text = txtLog.Text & "Redirecting to the fake realm." & vbNewLine
        sckPRealm.Close
        sckPRealm.Accept requestID
    Else
        txtLog.Text = txtLog.Text & "Redirecting to the official realm." & vbNewLine
        sckORealm.Close
        
        RealmConnected = False
        sckORealm.Connect txtOIP.Text, txtOPort.Text
        
        Do
            DoEvents
        Loop Until RealmConnected
        
        sckFRealm.Close
        sckFRealm.Accept requestID
        
        RealmListSize = 0
        RealmListRecieved = 0
        WorldIP = ""
        WorldPort = 0
        GotSSHash = False
    End If
    txtLog.Text = txtLog.Text & "Realm server connection accepted." & vbNewLine
End Sub

Private Sub sckRListen_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    txtLog.Text = txtLog.Text & "Error RListen (" & Number & "): " & Description
End Sub

Private Sub sckWListen_ConnectionRequest(ByVal requestID As Long)
    txtLog.Text = txtLog.Text & "Connection request for world server." & vbNewLine
    
    If GotPointer = False Then
        sckPWorld.Close
        sckPWorld.Accept requestID
        
        DoEvents
        Do
            DoEvents
        Loop Until sckPWorld.State = sckConnected
        
        Dim data() As Byte
        InitPacket data, SMSG_AUTH_CHALLENGE
        AddPacketInt32 data, &HDE1337
        EndPacket data
        sckPWorld.SendData data
    Else
        PacketFinishedO = True
        PacketFinishedF = True
        Decoding = False
        sckFWorld.Close
        sckFWorld.Accept requestID
        
        DoEvents
        Do
            DoEvents
        Loop Until sckFWorld.State = sckConnected
        
        sckOWorld.Close
        WorldConnected = False
        sckOWorld.Connect WorldIP, WorldPort
        txtLog.Text = txtLog.Text & "Connecting to " & WorldIP & ":" & WorldPort & "." & vbNewLine
        
        Do
            DoEvents
        Loop Until WorldConnected
        
        GotSSHash = False
        DoEvents
    End If
    
    txtLog.Text = txtLog.Text & "World server connection accepted." & vbNewLine
    DoEvents
End Sub

Private Sub sckWListen_Error(ByVal Number As Integer, Description As String, ByVal Scode As Long, ByVal Source As String, ByVal HelpFile As String, ByVal HelpContext As Long, CancelDisplay As Boolean)
    txtLog.Text = txtLog.Text & "Error WListen (" & Number & "): " & Description
End Sub

Private Sub tmrFindWoW_Timer()
    Dim lngPid As Long, ProcessID As Long, SS_HashHex As String
    If GotSSHash = True Then tmrFindWoW.Enabled = False: Exit Sub
    'Find the window
    lngHWnd = FindWindow(vbNullString, "World of Warcraft")
    
    If lngHWnd Then
        GetWindowThreadProcessId lngHWnd, lngPid
        'Open process:
        lngPHandle = OpenSecureProcess("World of Warcraft", Me.hWnd, PROCESS_ALL_ACCESS)
        'The SS_Hash Pointer keeps changing all the time
        '----------
        '0x0578A347
        '0x0579AF97
        '0x05AF124F
        '0x05B6E46F
        '0x05B881EF
        '0x07275137
        '0x0C67BCD7
        '----------
        SS_HashHex = StringToHex(GetMemString(SSHashPointer, 40))
        If SS_HashHex <> "00000000000000000000000000000000000000000000000000000000000000000000000000000000" Then
            SS_Hash = HexToBytes(SS_HashHex)
            InitWarden
            txtLog.Text = txtLog.Text & "SS_Hash = " & SS_HashHex & vbNewLine
            GotSSHash = True
            tmrFindWoW.Enabled = False
            CloseHandle lngPHandle 'Close it so we don't get busted as a memory cheater!
        End If
    Else
        'Game is not found
        lngPHandle = 0
    End If
End Sub

Private Sub DecodeC(data() As Byte)
' Decoding client messages
Dim T As Integer
Dim A As Integer
Dim b As Integer
Dim d As Integer

    For T = 0 To 5
        A = ClientKey(0)
        ClientKey(0) = data(T)
        b = data(T)
        
        b = CByte((b - A) And &HFF)
        d = ClientKey(1)
        A = SS_Hash(d)
        A = CByte((A Xor b) And &HFF)
        data(T) = A

        A = ClientKey(1)
        A = A + 1
        ClientKey(1) = A Mod 40
    Next T
End Sub

Private Sub DecodeS(data() As Byte)
'Decoding server messages
Dim T As Integer
Dim A As Integer
Dim b As Integer
Dim d As Integer

    For T = 0 To 3
        A = ServerKey(0)
        ServerKey(0) = data(T)
        b = data(T)
        
        b = CByte((b - A) And &HFF)
        d = ServerKey(1)
        A = SS_Hash(d)
        A = CByte((A Xor b) And &HFF)
        data(T) = A

        A = ServerKey(1)
        A = A + 1
        ServerKey(1) = A Mod 40
    Next T
End Sub
