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
Imports System.Threading
Imports System.Net.Sockets
Imports System.Xml.Serialization
Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Reflection
Imports System.Collections.Generic
Imports mangosVB.Common.SQL
Imports mangosVB.Common
Imports mangosVB.Common.BaseWriter

Public Module RS_Main
#Region "Global.Constants"

    Dim WS_STATUS() As String = {"ONLINE/G", "ONLINE/R", "OFFLINE "}
    Public ConsoleColor As New ConsoleColor


    '1.10.2 - 5302
    '1.11.0 - 5428
    '1.11.2 - 5464
    '1.12.0 - 5595
    '1.12.1 - 5875
    '1.12.2 - 6005
    '2.00.1 - 6180
    '2.00.3 - 6299
    '2.00.4 - 6314
    '2.00.5 - 6320
    '2.00.6 - 6337
    '2.00.7 - 6383
    '2.00.8 - 6403
    '2.00.10 - 6448
    '2.00.12 - 6546
    Const REQUIRED_VERSION_1 As Integer = 1
    Const REQUIRED_VERSION_2 As Integer = 12
    Const REQUIRED_VERSION_3 As Integer = 1
    Const REQUIRED_BUILD_LOW As Integer = 5875
    Const REQUIRED_BUILD_HIGH As Integer = 5875
    Const CONNETION_SLEEP_TIME As Integer = 100

    'RealmServ OP Codes
    Const CMD_AUTH_LOGON_CHALLENGE As Integer = &H0
    Const CMD_AUTH_LOGON_PROOF As Integer = &H1
    Const CMD_AUTH_RECONNECT_CHALLENGE As Integer = &H2
    Const CMD_AUTH_RECONNECT_PROOF As Integer = &H3
    Const CMD_AUTH_UPDATESRV As Integer = &H4
    Const CMD_AUTH_REALMLIST As Integer = &H10

    'UpdateServ OP Codes
    Const CMD_XFER_INITIATE As Integer = &H30  'client? from server
    Const CMD_XFER_DATA As Integer = &H31      'client? from server
    Const CMD_XFER_ACCEPT As Integer = &H32    'not official name, from client
    Const CMD_XFER_RESUME As Integer = &H33    'not official name, from client
    Const CMD_XFER_CANCEL As Integer = &H34    'not official name, from client

    'Unknown
    Const CMD_GRUNT_AUTH_CHALLENGE As Integer = &H0    'server
    Const CMD_GRUNT_AUTH_VERIFY As Integer = &H2       'server
    Const CMD_GRUNT_CONN_PING As Integer = &H10        'server
    Const CMD_GRUNT_CONN_PONG As Integer = &H11        'server
    Const CMD_GRUNT_HELLO As Integer = &H20            'server
    Const CMD_GRUNT_PROVESESSION As Integer = &H21     'server
    Const CMD_GRUNT_KICK As Integer = &H24             'server

    Public Log As New BaseWriter


    Public Enum AccountState As Byte
        'RealmServ Error Codes
        LOGIN_OK = &H0
        LOGIN_FAILED = &H1                     'Unable to connect
        LOGIN_BANNED = &H3                     'This World of Warcraft account has been closed and is no longer in service -- Please check the registered email address of this account for further information.
        LOGIN_UNKNOWN_ACCOUNT = &H4            'The information you have entered is not valid.  Please check the spelling of the account name and password.  If you need help in retrieving a lost or stolen password and account, see www.worldofwarcraft.com for more information.
        LOGIN_BAD_PASS = &H5                   'The information you have entered is not valid.  Please check the spelling of the account name and password.  If you need help in retrieving a lost or stolen password and account, see www.worldofwarcraft.com for more information.
        LOGIN_ALREADYONLINE = &H6              'This account is already logged into World of Warcraft.  Please check the spelling and try again.
        LOGIN_NOTIME = &H7                     'You have used up your prepaid time for this account. Please purchase more to continue playing.
        LOGIN_DBBUSY = &H8                     'Could not log in to World of Warcraft at this time.  Please try again later.
        LOGIN_BADVERSION = &H9                 'Unable to validate game version.  This may be caused by file corruption or the interference of another program.  Please visit www.blizzard.com/support/wow/ for more information and possible solutions to this issue.
        LOGIN_DOWNLOADFILE = &HA
        LOGIN_SUSPENDED = &HC                  'This World Of Warcraft account has been temporarily suspended. Please go to http://www.wow-europe.com/en/misc/banned.html for furhter information.
        LOGIN_PARENTALCONTROL = &HF            'Access to this account has been blocked by parental controls.  Your settings may be changed in your account preferences at http://www.worldofwarcraft.com.
    End Enum

#End Region

#Region "Global.Config"
    Public Config As XMLConfigFile
    <XmlRoot(ElementName:="RealmServer")> _
    Public Class XMLConfigFile
        'Server Configurations
        <XmlElement(ElementName:="RSPort")> Public RSPort As Int32 = 3724
        <XmlElement(ElementName:="RSHost")> Public RSHost As String = "127.0.0.1"
        <XmlElement(ElementName:="AccountDatabase")> Public AccountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
    End Class

    Public Sub LoadConfig()
        Try
            'Make sure RealmServer.ini exists
            If System.IO.File.Exists("RealmServer.ini") = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Cannot Continue. {1} does not exist.", Format(TimeOfDay, "hh:mm:ss"), "RealmServer.ini")
                Console.WriteLine("Please copy the ini files into the same directory as the MangosVB exe files.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If

            Console.Write("[{0}] Loading Configuration...", Format(TimeOfDay, "hh:mm:ss"))

            Config = New XMLConfigFile
            Console.Write("...")

            Dim oXS As XmlSerializer = New XmlSerializer(GetType(XMLConfigFile))
            Console.Write("...")
            Dim oStmR As StreamReader
            oStmR = New StreamReader("RealmServer.ini")
            Config = oXS.Deserialize(oStmR)
            oStmR.Close()


            Console.WriteLine(".[done]")

            'DONE: Creating logger
            'BaseWriter.CreateLog(Config.LogType, Config.LogConfig, Log)
            'Log.LogLevel = Config.LogLevel

            'DONE: Setting SQL Connection
            Dim AccountDBSettings() As String = Split(Config.AccountDatabase, ";")
            If AccountDBSettings.Length = 6 Then
                Database.SQLDBName = AccountDBSettings(4)
                Database.SQLHost = AccountDBSettings(2)
                Database.SQLPort = AccountDBSettings(3)
                Database.SQLUser = AccountDBSettings(0)
                Database.SQLPass = AccountDBSettings(1)
                Database.SQLTypeServer = CType([Enum].Parse(GetType(SQL.DB_Type), AccountDBSettings(5)), SQL.DB_Type)
            Else
                Console.WriteLine("Invalid connect string for the account database!")
            End If

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub
#End Region

#Region "RS.Sockets"
    Public LastConnections As New Dictionary(Of UInteger, Date)
    Public RS As RealmServerClass
    Class RealmServerClass
        Public _flagStopListen As Boolean = False
        Private lstHost As Net.IPAddress = Net.IPAddress.Parse(Config.RSHost)
        Private lstConnection As TcpListener
        'Private lstThreadPool As ThreadPool

        Public Sub New()
            Try
                lstConnection = New TcpListener(lstHost, Config.RSPort)
                lstConnection.Start()
                Dim RSListenThread As Thread
                RSListenThread = New Thread(AddressOf AcceptConnection)
                RSListenThread.Name = "Realm Server, Listening"
                RSListenThread.Start()

                Console.WriteLine("[{0}] Listening on {1} on port {2}", Format(TimeOfDay, "hh:mm:ss"), lstHost, Config.RSPort)
            Catch e As Exception
                Console.WriteLine()
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] Error in {2}: {1}.", Format(TimeOfDay, "hh:mm:ss"), e.Message, e.Source)
                Console.ForegroundColor = System.ConsoleColor.Gray
            End Try
        End Sub
        Protected Sub AcceptConnection()
            Do While Not _flagStopListen
                Thread.Sleep(CONNETION_SLEEP_TIME)
                If lstConnection.Pending() Then
                    Dim Client As New ClientClass
                    Client.Socket = lstConnection.AcceptSocket
                    'lstThreadPool.QueueUserWorkItem(New System.Threading.WaitCallback(AddressOf Client.Process))

                    Dim NewThread As New Thread(AddressOf Client.Process)
                    NewThread.Start()
                End If
            Loop
        End Sub
        Protected Overloads Sub Dispose(ByVal disposing As Boolean)
            _flagStopListen = True
            lstConnection.Stop()
        End Sub
    End Class
#End Region
#Region "RS.Data Access"
    Public Database As New SQL
    Public Sub SLQEventHandler(ByVal MessageID As SQL.EMessages, ByVal OutBuf As String)
        Select Case MessageID
            Case SQL.EMessages.ID_Error
                Console.ForegroundColor = System.ConsoleColor.Red
            Case SQL.EMessages.ID_Message
                Console.ForegroundColor = System.ConsoleColor.DarkGreen
        End Select

        Console.WriteLine("[" & Format(TimeOfDay, "hh:mm:ss") & "] " & OutBuf)
        Console.ForegroundColor = System.ConsoleColor.Gray
    End Sub

#End Region
#Region "RS.Analyzer"

    Public Enum WoWLanguage As Byte
        enGB = 0
        enUS = 1
        deDE = 2
        frFR = 3
    End Enum
    Class ClientClass
        Implements IDisposable

        Public Socket As Socket
        Public IP As Net.IPAddress = Net.IPAddress.Parse("0.0.0.0")
        Public Port As Int32 = 0
        Public AuthEngine As AuthEngineClass
        Public Account As String = ""
        Public Language As String = "enGB"
        Public UpdateFile As String = ""
        Public Access As AccessLevel = AccessLevel.Player

        Public Sub OnData(ByVal data() As Byte)
            Select Case data(0)
                Case CMD_AUTH_LOGON_CHALLENGE, CMD_AUTH_RECONNECT_CHALLENGE
                    'Console.WriteLine("[{0}] [{1}:{2}] RS_LOGON_CHALLENGE", Format(TimeOfDay, "HH:mm:ss"), IP, Port)
                    On_RS_LOGON_CHALLENGE(data, Me)
                Case CMD_AUTH_LOGON_PROOF, CMD_AUTH_RECONNECT_PROOF
                    'Console.WriteLine("[{0}] [{1}:{2}] RS_LOGON_PROOF", Format(TimeOfDay, "HH:mm:ss"), IP, Port)
                    On_RS_LOGON_PROOF(data, Me)
                Case CMD_AUTH_REALMLIST
                    'Console.WriteLine("[{0}] [{1}:{2}] RS_REALMLIST", Format(TimeOfDay, "HH:mm:ss"), IP, Port)
                    On_RS_REALMLIST(data, Me)

                Case CMD_AUTH_UPDATESRV
                    Console.WriteLine("[{0}] [{1}:{2}] RS_UPDATESRV", Format(TimeOfDay, "hh:mm:ss"), IP, Port)

                Case CMD_XFER_ACCEPT
                    'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_ACCEPT", Format(TimeOfDay, "HH:mm:ss"), IP, Port)
                    On_CMD_XFER_ACCEPT(data, Me)
                Case CMD_XFER_RESUME
                    'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_RESUME", Format(TimeOfDay, "HH:mm:ss"), IP, Port)
                    On_CMD_XFER_RESUME(data, Me)
                Case CMD_XFER_CANCEL
                    'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_CANCEL", Format(TimeOfDay, "HH:mm:ss"), IP, Port)
                    On_CMD_XFER_CANCEL(data, Me)
                Case Else
                    Console.ForegroundColor = System.ConsoleColor.Red
                    Console.WriteLine("[{0}] [{1}:{2}] Unknown Opcode 0x{3}", Format(TimeOfDay, "hh:mm:ss"), IP, Port, data(0))
                    Console.ForegroundColor = System.ConsoleColor.Gray
                    DumpPacket(data, Me)
            End Select
        End Sub
        Public Sub Process()
            IP = CType(Socket.RemoteEndPoint, IPEndPoint).Address
            Port = CType(Socket.RemoteEndPoint, IPEndPoint).Port

            'DONE: Connection spam protection
            Dim IpInt As UInteger = IP2Int(IP.ToString)
            If LastConnections.ContainsKey(IpInt) Then
                If Now > LastConnections(IpInt) Then
                    LastConnections(IpInt) = Now.AddSeconds(5)
                Else
                    Socket.Close()
                    Me.Dispose()
                    Exit Sub
                End If
            Else
                LastConnections.Add(IpInt, Now.AddSeconds(5))
            End If

            Dim Buffer() As Byte
            Dim bytes As Integer

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Incoming connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
            Console.WriteLine("[{0}] [{1}:{2}] Checking for banned IP.", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
            Console.ForegroundColor = System.ConsoleColor.Gray
            If Not Database.QuerySQL("SELECT ip FROM bans WHERE ip = """ & IP.ToString & """;") Then

                While Not RS._flagStopListen
                    Thread.Sleep(CONNETION_SLEEP_TIME)
                    If Socket.Available > 0 Then
                        If Socket.Available > 500 Then 'DONE: Data flood protection
                            Exit While
                        End If
                        ReDim Buffer(Socket.Available - 1)
                        bytes = Socket.Receive(Buffer, Buffer.Length, 0)
                        OnData(Buffer)
                    End If
                    If Not Socket.Connected Then Exit While
                    If (Socket.Poll(100, SelectMode.SelectRead)) And (Socket.Available = 0) Then Exit While
                End While

            Else
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] [{1}:{2}] This ip is banned.", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                Console.ForegroundColor = System.ConsoleColor.Gray
            End If

            Socket.Close()

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Connection from [{1}:{2}] closed", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
            Console.ForegroundColor = System.ConsoleColor.Gray

            Me.Dispose()
        End Sub
        Public Sub Send(ByVal data() As Byte)
            Try
                Dim i As Integer = Socket.Send(data, 0, data.Length, SocketFlags.None)

#If DEBUG Then
                Console.ForegroundColor = System.ConsoleColor.DarkGray
                Console.WriteLine("[{0}] [{1}:{2}] Data sent, result code={3}", Format(TimeOfDay, "hh:mm:ss"), IP, Port, i)
                Console.ForegroundColor = System.ConsoleColor.Gray
#End If
            Catch Err As Exception
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] Connection from [{1}:{2}] do not exist - ERROR!!!", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                Console.ForegroundColor = System.ConsoleColor.Gray
                Socket.Close()
            End Try
        End Sub
        Public Sub Dispose() Implements System.IDisposable.Dispose
            'Console.ForegroundColor = System.ConsoleColor.DarkGray
            'Console.WriteLine("[{0}] Connection from [{1}:{2}] deleted", Format(TimeOfDay, "HH:mm:ss"), IP, Port)
            'Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub
    End Class

#End Region

#Region "RS_OPCODES"
    Public Sub On_RS_LOGON_CHALLENGE(ByRef data() As Byte, ByRef Client As ClientClass)
        Dim i As Integer
        Dim iUpper As Integer = (CInt(data(33)) - 1)
        Dim packet_size As Integer = BitConverter.ToInt16(New Byte() {data(3), data(2)}, 0)
        Dim packet_account As String
        Dim packet_ip As String
        Dim acc_state As AccountState = AccountState.LOGIN_DBBUSY

        'Read account name from packet
        packet_account = ""
        For i = 0 To iUpper
            packet_account = packet_account + Chr(data(34 + i))
        Next i
        Client.Account = packet_account

        'Read users ip from packet
        packet_ip = CInt(data(29)).ToString & "." & CInt(data(30)).ToString & "." & CInt(data(31)).ToString & "." & CInt(data(32)).ToString

        'Get the client build from packet.
        Dim bMajor As Integer = CInt(data(8))
        Dim bMinor As Integer = CInt(data(9))
        Dim bRevision As Integer = CInt(data(10))
        Dim ClientBuild As Integer = BitConverter.ToInt16(New Byte() {data(11), data(12)}, 0)
        Dim ClientLanguage As String = Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21))

        Console.WriteLine("[{0}] [{1}:{2}] CMD_AUTH_LOGON_CHALLENGE [{3}] [{4}], WoW Version [{5}.{6}.{7}.{8}] [{9}].", Format(TimeOfDay, "HH:mm:ss"), Client.IP, Client.Port, packet_account, packet_ip, bMajor.ToString, bMinor.ToString, bRevision.ToString, ClientBuild.ToString, ClientLanguage)

        'DONE: Check if our build can join the server
        If ((REQUIRED_VERSION_1 = 0 AndAlso REQUIRED_VERSION_2 = 0 AndAlso REQUIRED_VERSION_3 = 0) OrElse _
            (bMajor = REQUIRED_VERSION_1 AndAlso bMinor = REQUIRED_VERSION_2 AndAlso bRevision = REQUIRED_VERSION_3)) AndAlso _
            ClientBuild >= REQUIRED_BUILD_LOW AndAlso ClientBuild <= REQUIRED_BUILD_HIGH Then
            'TODO: in the far future should check if the account is expired too
            Dim result As DataTable = Nothing
            Try
                'Get Account info
                Database.Query([String].Format("SELECT * FROM accounts WHERE account = ""{0}"";", packet_account), result)

                'Check Account state
                If result.Rows.Count > 0 Then
                    If result.Rows(0).Item("banned") = 1 Then acc_state = AccountState.LOGIN_BANNED Else acc_state = AccountState.LOGIN_OK
                Else
                    acc_state = AccountState.LOGIN_UNKNOWN_ACCOUNT
                End If
            Catch
                acc_state = AccountState.LOGIN_DBBUSY
            End Try


            'DONE: Send results to client
            Select Case acc_state
                Case AccountState.LOGIN_OK
                    Console.WriteLine("[{0}] [{1}:{2}] Account found [{3}]", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, packet_account)

                    Dim account(data(33) - 1) As Byte
                    Array.Copy(data, 34, account, 0, data(33))
                    Dim pwHash As String = result.Rows(0).Item("password")
                    Client.Access = result.Rows(0).Item("plevel")

                    Dim Hash() As Byte = New Byte(19) {}
                    For i = 0 To 39 Step 2
                        Hash(i \ 2) = CInt("&H" & pwHash.Substring(i, 2))
                    Next

                    Client.Language = ClientLanguage
                    Client.Expansion = result.Rows(0).Item("expansion")
                    Try
                        Client.AuthEngine = New AuthEngineClass
                    Catch ex As Exception
                        Console.ForegroundColor = System.ConsoleColor.Red
                        Console.WriteLine("[{0}] [{1}:{2}] Error loading AuthEngine: {3}{4}", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, vbNewLine, ex)
                        Console.ForegroundColor = System.ConsoleColor.White
                    End Try
                    Client.AuthEngine.CalculateX(account, Hash)

                    Dim data_response(118) As Byte
                    data_response(0) = CMD_AUTH_LOGON_CHALLENGE
                    data_response(1) = AccountState.LOGIN_OK
                    data_response(2) = Val("&H00")
                    Array.Copy(Client.AuthEngine.PublicB, 0, data_response, 3, 32)
                    data_response(35) = Client.AuthEngine.g.Length
                    data_response(36) = Client.AuthEngine.g(0)
                    data_response(37) = 32
                    Array.Copy(Client.AuthEngine.N, 0, data_response, 38, 32)
                    Array.Copy(Client.AuthEngine.salt, 0, data_response, 70, 32)
                    Array.Copy(AuthEngineClass.CrcSalt, 0, data_response, 102, 16)
                    data_response(118) = 0 ' Added in 1.12.x client branch? Security Flags (&H0...&H4)?
                    Client.Send(data_response)
                    Exit Sub
                Case AccountState.LOGIN_UNKNOWN_ACCOUNT
                    Console.WriteLine("[{0}] [{1}:{2}] Account not found [{3}]", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, packet_account)
                Case AccountState.LOGIN_BANNED
                    Console.WriteLine("[{0}] [{1}:{2}] Account banned [{3}]", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, packet_account)
                Case AccountState.LOGIN_NOTIME
                    Console.WriteLine("[{0}] [{1}:{2}] Account prepaid time used [{3}]", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, packet_account)
                Case AccountState.LOGIN_ALREADYONLINE
                    Console.WriteLine("[{0}] [{1}:{2}] Account already logged in the game [{3}]", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, packet_account)
                Case Else
                    Console.WriteLine("[{0}] [{1}:{2}] Account error [{3}]", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, packet_account)
            End Select
            Dim data_response_error(1) As Byte
            data_response_error(0) = CMD_AUTH_LOGON_PROOF
            data_response_error(1) = acc_state
            Client.Send(data_response_error)

        Else
            If Dir("Updates/wow-patch-" & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & "-" & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & ".mpq") <> "" Then
                'Send UPDATE_MPQ
                Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_INITIATE [" & Chr(data(6)) & Chr(data(5)) & Chr(data(4)) & " " & data(8) & "." & data(9) & "." & data(10) & "." & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & " " _
                    & Chr(data(15)) & Chr(data(14)) & Chr(data(13)) & " " & Chr(data(19)) & Chr(data(18)) & Chr(data(17)) & " " & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & "]" _
                                  , Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port)

                Client.UpdateFile = "Updates/wow-patch-" & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & "-" & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & ".mpq"
                Dim data_response(30) As Byte

                data_response(0) = CMD_XFER_INITIATE
                'Name Len 0x05 -> sizeof(Patch)
                i = 1
                Converter.ToBytes(CType(5, Byte), data_response, i)
                'Name 'Patch'
                Converter.ToBytes("Patch", data_response, i)
                'Size 0x34 C4 0D 00 = 902,196 byte (180->181 enGB)
                Converter.ToBytes(CType(FileLen(Client.UpdateFile), Integer), data_response, i)
                'Unknown 0x0 always
                Converter.ToBytes(CType(0, Integer), data_response, i)
                'MD5 CheckSum
                Dim md5 As New MD5CryptoServiceProvider
                Dim buffer() As Byte
                Dim fs As FileStream = New FileStream(Client.UpdateFile, FileMode.Open)
                Dim r As BinaryReader = New BinaryReader(fs)
                buffer = r.ReadBytes(FileLen(Client.UpdateFile))
                r.Close()
                fs.Close()
                Dim result As Byte() = md5.ComputeHash(buffer)
                Array.Copy(result, 0, data_response, 15, 16)
                Client.Send(data_response)
            Else
                'Send BAD_VERSION
                Console.WriteLine("[{0}] [{1}:{2}] WRONG_VERSION [" & Chr(data(6)) & Chr(data(5)) & Chr(data(4)) & " " & data(8) & "." & data(9) & "." & data(10) & "." & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & " " _
                                    & Chr(data(15)) & Chr(data(14)) & Chr(data(13)) & " " & Chr(data(19)) & Chr(data(18)) & Chr(data(17)) & " " & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & "]" _
                                  , Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port)
                Dim data_response(1) As Byte
                data_response(0) = CMD_AUTH_LOGON_PROOF
                data_response(1) = AccountState.LOGIN_BADVERSION
                Client.Send(data_response)
            End If
        End If
    End Sub
    Public Sub On_RS_LOGON_PROOF(ByRef data() As Byte, ByRef Client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_AUTH_LOGON_PROOF", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port)
        Dim A(31) As Byte
        Array.Copy(data, 1, A, 0, 32)
        Dim M1(19) As Byte
        Array.Copy(data, 33, M1, 0, 20)
        'Dim CRC_Hash(19) As Byte
        'Array.Copy(data, 53, CRC_Hash, 0, 20)
        'Dim NumberOfKeys as Byte = data(73)
        'Dim unk as Byte = data(74)

        'Calculate U and M1
        Client.AuthEngine.CalculateU(A)
        Client.AuthEngine.CalculateM1()
        'Client.AuthEngine.CalculateCRCHash()

        'Check M1=ClientM1
        Dim pass_check As Boolean = True
        Dim i As Byte
        For i = 0 To 19
            If M1(i) <> Client.AuthEngine.M1(i) Then
                pass_check = False
                Exit For
            End If
        Next

        If pass_check Then
            Client.AuthEngine.CalculateM2(M1)

            Dim data_response(25) As Byte
            data_response(0) = CMD_AUTH_LOGON_PROOF
            data_response(1) = AccountState.LOGIN_OK
            Array.Copy(Client.AuthEngine.M2, 0, data_response, 2, 20)
            data_response(22) = 0
            data_response(23) = 0
            data_response(24) = 0
            data_response(25) = 0

            Client.Send(data_response)
            'Set SSHash in DB
            Dim sshash As String = ""
            'For i = 0 To Client.AuthEngine.SS_Hash.Length - 1
            For i = 0 To 40 - 1
                If Client.AuthEngine.SS_Hash(i) < 16 Then
                    sshash = sshash + "0" + Hex(Client.AuthEngine.SS_Hash(i))
                Else
                    sshash = sshash + Hex(Client.AuthEngine.SS_Hash(i))
                End If
            Next
            Database.Update([String].Format("UPDATE accounts SET last_sshash = '{1}', last_ip='{2}', last_login='{3}' WHERE account = '{0}';", Client.Account, sshash, Client.IP.ToString, Format(Now, "yyyy-MM-dd")))

            Console.WriteLine("[{0}] [{1}:{2}] Auth success for user {3}. [{4}]", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, Client.Account, sshash)
        Else
            'Wrong pass
            Console.WriteLine("[{0}] [{1}:{2}] Wrong password for user {3}.", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, Client.Account)
            Dim data_response(1) As Byte
            data_response(0) = CMD_AUTH_LOGON_PROOF
            data_response(1) = AccountState.LOGIN_UNKNOWN_ACCOUNT
            Client.Send(data_response)
        End If
    End Sub
    Public Sub On_RS_REALMLIST(ByRef data() As Byte, ByRef Client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_REALM_LIST", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port)

        Dim packet_len As Integer = 0
        Dim result As DataTable = Nothing
        If Client.Access < AccessLevel.GameMaster Then
            'Console.WriteLine("[{0}] [{1}:{2}] Player is not a Gamemaster, only listing non-GMonly realms", Format(TimeOfDay, "HH:mm:ss"), Client.IP, Client.Port)
            Database.Query([String].Format("SELECT * FROM realms WHERE gmonly = '0';"), result)
        Else
            'Console.WriteLine("[{0}] [{1}:{2}] Player is a Gamemaster, listing all realms", Format(TimeOfDay, "HH:mm:ss"), Client.IP, Client.Port)
            Database.Query([String].Format("SELECT * FROM realms;"), result)
        End If

        For Each Row As System.Data.DataRow In result.Rows
            packet_len = packet_len + Len(Row.Item("ws_host")) + Len(Row.Item("ws_name")) + 1 + Len(Format(Row.Item("ws_port"), "0")) + 14
        Next

        Dim tmp As Integer = 8
        Dim data_response(packet_len + 9) As Byte

        '(byte) Opcode
        data_response(0) = CMD_AUTH_REALMLIST

        '(uint16) Packet Length
        data_response(2) = (packet_len + 7) \ 256
        data_response(1) = (packet_len + 7) Mod 256

        '(uint32) Unk
        data_response(3) = data(1)
        data_response(4) = data(2)
        data_response(5) = data(3)
        data_response(6) = data(4)

        '(uint16) Realms Count
        data_response(7) = result.Rows.Count

        For Each Host As System.Data.DataRow In result.Rows
            '(uint8) Realm Icon
            '	0 -> Normal; 1 -> PvP; 6 -> RP; 8 -> RPPvP;
            Converter.ToBytes(CType(Host.Item("ws_type"), Byte), data_response, tmp)
            '(uint8) IsLocked
            '	0 -> none; 1 -> locked
            Converter.ToBytes(CType(0, Byte), data_response, tmp)
            '(uint8) unk
            Converter.ToBytes(CType(0, Byte), data_response, tmp)
            '(uint8) unk
            Converter.ToBytes(CType(0, Byte), data_response, tmp)
            '(uint8) Realm Color 
            '   0 -> Green; 1 -> Red; 2 -> Offline;
            Converter.ToBytes(CType(Host.Item("ws_status"), Byte), data_response, tmp)
            '(string) Realm Name (zero terminated)
            Converter.ToBytes(CType(Host.Item("ws_name"), String), data_response, tmp)
            Converter.ToBytes(CType(0, Byte), data_response, tmp) '\0
            '(string) Realm Address ("ip:port", zero terminated)
            Converter.ToBytes(CType(Host.Item("ws_host") & ":" & Host.Item("ws_port"), String), data_response, tmp)
            Converter.ToBytes(CType(0, Byte), data_response, tmp) '\0
            '(float) Population 
            '   400F -> Full; 5F -> Medium; 1.6F -> Low; 200F -> New; 2F -> High
            '   00 00 48 43 -> Recommended
            '   00 00 C8 43 -> Full
            '   9C C4 C0 3F -> Low
            '   BC 74 B3 3F -> Low
            Converter.ToBytes(CType(Host.Item("ws_population"), Single), data_response, tmp)
            '(byte) Number of character at this realm for this account
            Converter.ToBytes(CType(1, Byte), data_response, tmp)
            '(byte) Timezone 
            '   0x01 - Development
            '   0x02 - USA
            '   0x03 - Oceania
            '   0x04 - LatinAmerica
            '   0x05 - Tournament
            '   0x06 - Korea
            '   0x07 - Tournament
            '   0x08 - UnitedKingdom
            '   0x09 - Germany
            '   0x0A - France
            '   0x0B - Spain
            '   0x0C - Russian
            '   0x0D - Tournament
            '   0x0E - Taiwan
            '   0x0F - Tournament
            '   0x10 - China
            '   0x11 - CN1
            '   0x12 - CN2
            '   0x13 - CN3
            '   0x14 - CN4
            '   0x15 - CN5
            '   0x16 - CN6
            '   0x17 - CN7
            '   0x18 - CN8
            '   0x19 - Tournament
            '   0x1A - Test Server
            '   0x1B - Tournament
            '   0x1C - QA Server
            '   0x1D - CN9
            '   0x1E - Test Server 2
            '     >  - off the list :)
            Converter.ToBytes(CType(Host.Item("ws_timezone"), Byte), data_response, tmp)
            '(byte) Unknown (may be 2 -> TestRealm, / 6 -> ?)
            Converter.ToBytes(CType(0, Byte), data_response, tmp)
        Next

        data_response(tmp) = 2 '2=list of realms 0=wizard
        data_response(tmp + 1) = 0

        Client.Send(data_response)

    End Sub
    Public Sub On_CMD_XFER_CANCEL(ByRef data() As Byte, ByRef Client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_CANCEL", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port)
        Client.Socket.Close()
    End Sub
    Public Sub On_CMD_XFER_ACCEPT(ByRef data() As Byte, ByRef Client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_ACCEPT", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port)
        Dim tmp As Integer = 1
        Dim buffer() As Byte
        Dim file_len As Integer = FileLen(Client.UpdateFile)
        Dim file_offset As Integer = 0
        Dim fs As FileStream = New FileStream(Client.UpdateFile, FileMode.Open, FileAccess.Read)
        Dim r As BinaryReader = New BinaryReader(fs)
        buffer = r.ReadBytes(file_len)
        r.Close()
        fs.Close()

        Const MAX_UPDATE_PACKET_SIZE As Integer = 1500

        If file_len > MAX_UPDATE_PACKET_SIZE Then
            Dim data_response() As Byte

            While file_len > MAX_UPDATE_PACKET_SIZE
                tmp = 1
                ReDim data_response(MAX_UPDATE_PACKET_SIZE + 2)
                data_response(0) = CMD_XFER_DATA
                Converter.ToBytes(CType(MAX_UPDATE_PACKET_SIZE, Short), data_response, tmp)
                Array.Copy(buffer, file_offset, data_response, 3, MAX_UPDATE_PACKET_SIZE)
                file_len = file_len - MAX_UPDATE_PACKET_SIZE
                file_offset = file_offset + MAX_UPDATE_PACKET_SIZE
                Client.Send(data_response)
            End While
            tmp = 1
            ReDim data_response(file_len + 2)
            data_response(0) = CMD_XFER_DATA
            Converter.ToBytes(CType(file_len, Short), data_response, tmp)
            Array.Copy(buffer, file_offset, data_response, 3, file_len)
            Client.Send(data_response)
        Else
            tmp = 1
            Dim data_response(file_len + 2) As Byte
            data_response(0) = CMD_XFER_DATA
            Converter.ToBytes(CType(file_len, Short), data_response, tmp)
            Array.Copy(buffer, 0, data_response, 3, file_len)
            Client.Send(data_response)
        End If
        'Client.Socket.Close()
    End Sub
    Public Sub On_CMD_XFER_RESUME(ByRef data() As Byte, ByRef Client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_RESUME", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port)
        Dim tmp As Integer = 1
        Dim buffer() As Byte
        Dim file_len As Integer = FileLen(Client.UpdateFile)
        Dim file_offset As Integer = Converter.ToInt32(data, tmp)
        file_len = file_len - file_offset

        Dim fs As FileStream = New FileStream(Client.UpdateFile, FileMode.Open, FileAccess.Read)
        Dim r As BinaryReader = New BinaryReader(fs)

        r.ReadBytes(file_offset)
        buffer = r.ReadBytes(file_len)
        r.Close()
        fs.Close()
        file_offset = 0

        Const MAX_UPDATE_PACKET_SIZE As Integer = 1500

        If file_len > MAX_UPDATE_PACKET_SIZE Then
            Dim data_response() As Byte

            While file_len > MAX_UPDATE_PACKET_SIZE
                tmp = 1
                ReDim data_response(MAX_UPDATE_PACKET_SIZE + 2)
                data_response(0) = CMD_XFER_DATA
                Converter.ToBytes(CType(MAX_UPDATE_PACKET_SIZE, Short), data_response, tmp)
                Array.Copy(buffer, file_offset, data_response, 3, MAX_UPDATE_PACKET_SIZE)
                file_len = file_len - MAX_UPDATE_PACKET_SIZE
                file_offset = file_offset + MAX_UPDATE_PACKET_SIZE
                Client.Send(data_response)
            End While
            tmp = 1
            ReDim data_response(file_len + 2)
            data_response(0) = CMD_XFER_DATA
            Converter.ToBytes(CType(file_len, Short), data_response, tmp)
            Array.Copy(buffer, file_offset, data_response, 3, file_len)
            Client.Send(data_response)
        Else
            tmp = 1
            Dim data_response(file_len + 2) As Byte
            data_response(0) = CMD_XFER_DATA
            Converter.ToBytes(CType(file_len, Short), data_response, tmp)
            Array.Copy(buffer, 0, data_response, 3, file_len)
            Client.Send(data_response)
        End If
        'Client.Socket.Close()
    End Sub
    Public Sub DumpPacket(ByRef data() As Byte, ByRef Client As ClientClass)
        Dim j As Integer
        Dim buffer As String = ""
        If Client Is Nothing Then
            buffer = buffer + [String].Format("[{0}] DEBUG: Packet Dump{1}", Format(TimeOfDay, "hh:mm:ss"), vbNewLine)
        Else
            buffer = buffer + [String].Format("[{0}] [{1}:{2}] DEBUG: Packet Dump{3}", Format(TimeOfDay, "hh:mm:ss"), Client.IP, Client.Port, vbNewLine)
        End If

        If data.Length Mod 16 = 0 Then
            For j = 0 To data.Length - 1 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
            Next
        Else
            For j = 0 To data.Length - 1 - 16 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
            Next

            buffer += "|  " & BitConverter.ToString(data, j, data.Length Mod 16).Replace("-", " ")
            buffer += New String(" ", (16 - data.Length Mod 16) * 3)
            buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, data.Length Mod 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?")
            buffer += New String(" ", 16 - data.Length Mod 16)
            buffer += " |" & vbNewLine
        End If

        Console.ForegroundColor = System.ConsoleColor.Red
        Console.WriteLine(buffer)
        Console.ForegroundColor = System.ConsoleColor.Gray

    End Sub
#End Region


    Sub WS_Status_Report()
        Dim result1 As DataTable = New DataTable
        Dim ReturnValues As Integer
        ReturnValues = Database.Query([String].Format("SELECT * FROM realms WHERE gmonly !='1';"), result1)
        If ReturnValues > SQL.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        Dim result2 As DataTable = New DataTable
        ReturnValues = Database.Query([String].Format("SELECT * FROM realms WHERE ws_status < 2 && gmonly != '1';"), result2)
        If ReturnValues > SQL.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        Dim result3 As DataTable = New DataTable
        ReturnValues = Database.Query([String].Format("SELECT * FROM realms WHERE ws_status < 2 && gmonly = '1';"), result3)
        If ReturnValues > SQL.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        Console.WriteLine()
        Console.WriteLine("[{0}] Known World Servers are {1}, Online World Servers are {2}", Format(TimeOfDay, "hh:mm:ss"), result1.Rows.Count, result2.Rows.Count)
        Console.WriteLine("[{0}] GM Only World Servers are {1}", Format(TimeOfDay, "HH:mm:ss"), result3.Rows.Count)

        Console.ForegroundColor = System.ConsoleColor.DarkGreen
        For Each Row As System.Data.DataRow In result1.Rows
            Console.WriteLine("           {3} [{1}] at {0}:{2}", Row.Item("ws_host").PadRight(20), Row.Item("ws_name").PadRight(20), Format(Row.Item("ws_port")).PadRight(6), WS_STATUS(Int(Row.Item("ws_status"))).PadRight(10))
        Next
        Console.ForegroundColor = System.ConsoleColor.Yellow
        For Each Row As System.Data.DataRow In result3.Rows
            Console.WriteLine("           {3} [{1}] at {0}           :{2}", Row.Item("ws_host").PadRight(6), Row.Item("ws_name").PadRight(20), Format(Row.Item("ws_port")), WS_STATUS(Int("3")).PadRight(10))
        Next
        Console.ForegroundColor = System.ConsoleColor.Gray
    End Sub
    Sub Main()
        Console.Title = String.Format("{0} v{1}", [Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyTitleAttribute), False)(0).Title, [Assembly].GetExecutingAssembly().GetName().Version)

        Console.ForegroundColor = System.ConsoleColor.Yellow

        Console.WriteLine(" ####       ####            ###     ###   ########    #######     ######## ")
        Console.WriteLine(" #####     #####            ####    ###  ##########  #########   ##########")
        Console.WriteLine(" #####     #####            #####   ###  ##########  #########   ##########")
        Console.WriteLine(" ######   ######            #####   ###  ###        ####   ####  ###       ")
        Console.WriteLine(" ######   ######    ####    ######  ###  ###        ###     ###  ###       ")
        Console.WriteLine(" ####### #######   ######   ######  ###  ###  ##### ###     ###  ########  ")
        Console.WriteLine(" ### ### ### ###   ######   ####### ###  ###  ##### ###     ###  ######### ")
        Console.WriteLine(" ### ### ### ###  ###  ###  ### ### ###  ###  ##### ###     ###   #########")
        Console.WriteLine(" ### ####### ###  ###  ###  ###  ######  ###    ### ###     ###        ####")
        Console.WriteLine(" ### ####### ###  ###  ###  ###  ######  ###    ### ###     ###         ###")
        Console.WriteLine(" ###  #####  ### ########## ###   #####  ###   #### ####   ####        ####")
        Console.WriteLine(" ###  #####  ### ########## ###   #####  #########   #########   ##########")
        Console.WriteLine(" ###  #####  ### ###    ### ###    ####  #########   #########   ######### ")
        Console.WriteLine(" ###   ###   ### ###    ### ###     ###   #######     #######     #######  ")
        Console.WriteLine("")
        Console.WriteLine(" Website: http://www.getmangos.co.uk                         ##  ##  ##### ")
        Console.WriteLine("                                                             ##  ##  ##  ##")
        Console.WriteLine("    Wiki: http://github.com/mangoswiki/wiki                  ##  ##  ##### ")
        Console.WriteLine("                                                              ####   ##  ##")
        Console.WriteLine("   Forum: http://community.getmangos.co.uk                     ##    ##### ")
        Console.WriteLine("")

        Console.ForegroundColor = System.ConsoleColor.Magenta

        Console.ForegroundColor = System.ConsoleColor.White
        Console.Write([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyTitleAttribute), False)(0).Title)
        Console.WriteLine(" version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.WriteLine()
        Console.ForegroundColor = System.ConsoleColor.Gray

        Console.WriteLine("[{0}] Realm Server Starting...", Format(TimeOfDay, "hh:mm:ss"))
        LoadConfig()

#If DEBUG Then
        Console.ForegroundColor = System.ConsoleColor.Yellow
        Log.WriteLine(LogType.INFORMATION, "Running from: {0}", System.AppDomain.CurrentDomain.BaseDirectory)
        Console.ForegroundColor = System.ConsoleColor.Gray
#End If

        AddHandler Database.SQLMessage, AddressOf SLQEventHandler
        Database.Connect()

        RS = New RealmServerClass

        WS_Status_Report()

        Dim tmp As String, CommandList() As String, cmd() As String
        Dim varList As Integer
        While True
            tmp = Console.ReadLine()
            CommandList = tmp.Split(";")

            For varList = LBound(CommandList) To UBound(CommandList)
                cmd = CommandList(varList).Split(" ")
                If CommandList(varList).Length > 0 Then
                    Select Case cmd(0).ToLower
                        Case "/quit", "/shutdown", "/off", "/kill", "/exit", "quit", "shutdown", "off", "kill"
                            Console.ForegroundColor = System.ConsoleColor.DarkGreen
                            Console.WriteLine("Server shutting down...")
                            Console.ForegroundColor = System.ConsoleColor.Gray
                            Thread.Sleep(1000)
                            End
                        Case "help", "/help"
                            Console.ForegroundColor = System.ConsoleColor.Blue
                            Console.WriteLine("'RealmServer' Command list:")
                            Console.ForegroundColor = System.ConsoleColor.White
                            Console.WriteLine("---------------------------------")
                            Console.WriteLine("")
                            Console.WriteLine("")
                            Console.WriteLine("'help' or '/help' - Brings up the RealmServer' Command list (this).")
                            Console.WriteLine("")
                            Console.WriteLine("'/quit' or '/shutdown' or 'off' or 'kill' or 'exit' - Shutsdown 'RealmServer'.")
                        Case Else
                            Console.ForegroundColor = System.ConsoleColor.DarkRed
                            Console.WriteLine("Error!. Cannot find specified command. Please type 'help' for information on 'RealmServer' console commands.")
                            Console.ForegroundColor = System.ConsoleColor.White
                    End Select
                End If
            Next
        End While
    End Sub

    Function IP2Int(ByVal IP As String) As UInteger
        Dim IpSplit() As String = IP.Split(".")
        If IpSplit.Length <> 4 Then Return 0
        Dim IpBytes(3) As Byte
        Try
            IpBytes(0) = CByte(IpSplit(3))
            IpBytes(1) = CByte(IpSplit(2))
            IpBytes(2) = CByte(IpSplit(1))
            IpBytes(3) = CByte(IpSplit(0))
            Return BitConverter.ToUInt32(IpBytes, 0)
        Catch
            Return 0
        End Try
    End Function
End Module
