'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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
Imports System.Collections.Generic
Imports System.IO
Imports System.Security.Cryptography
Imports System.Xml.Serialization
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text
Imports System.Reflection
Imports mangosVB.Common.Global_Enums
Imports mangosVB.Common
Imports mangosVB.Common.Logging
Imports mangosVB.Shared

Public Module RealmServer
#Region "Global.Variables"
    Dim Log As New BaseWriter
#End Region

#Region "Global.Config"
    Private _config As XmlConfigFile

    <XmlRoot(ElementName:="RealmServer")>
    Public Class XmlConfigFile
        'Server Configurations
        <XmlElement(ElementName:="RealmServerPort")> Public RealmServerPort As Int32 = 3724
        <XmlElement(ElementName:="RealmServerAddress")> Public RealmServerAddress As String = "127.0.0.1"
        <XmlElement(ElementName:="AccountDatabase")> Public AccountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
    End Class

    Private Sub LoadConfig()
        Try
            'Make sure RealmServer.ini exists
            If File.Exists("configs/RealmServer.ini") = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Cannot Continue. {1} does not exist.", Format(TimeOfDay, "hh:mm:ss"), "configs/RealmServer.ini")
                Console.WriteLine("Please copy the ini files into the same directory as the MangosVB exe files.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If

            Console.Write("[{0}] Loading Configuration...", Format(TimeOfDay, "hh:mm:ss"))

            _config = New XmlConfigFile
            Console.Write("...")

            Dim oXs As XmlSerializer = New XmlSerializer(GetType(XmlConfigFile))
            Console.Write("...")
            Dim oStmR As StreamReader
            oStmR = New StreamReader("configs/RealmServer.ini")
            _config = oXs.Deserialize(oStmR)
            oStmR.Close()

            Console.WriteLine(".[done]")

            'DONE: Creating logger
            'Logger.CreateLog(Config.LogType, Config.LogConfig, Log)
            'Log.LogLevel = Config.LogLevel

            'DONE: Setting SQL Connection
            Dim accountDbSettings() As String
            accountDbSettings = Split(_config.AccountDatabase, ";")
            If accountDbSettings.Length = 6 Then
                _accountDatabase.SQLDBName = accountDbSettings(4)
                _accountDatabase.SQLHost = accountDbSettings(2)
                _accountDatabase.SQLPort = accountDbSettings(3)
                _accountDatabase.SQLUser = accountDbSettings(0)
                _accountDatabase.SQLPass = accountDbSettings(1)
                _accountDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), accountDbSettings(5))
            Else
                Console.WriteLine("Invalid connect string for the account database!")
            End If

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub

#End Region

#Region "RS.Sockets"

    Private ReadOnly LastConnections As New Dictionary(Of UInteger, Date)
    Private _realmServer As RealmServerClass

    Private Class RealmServerClass
        Implements IDisposable

        Public FlagStopListen As Boolean = False
        Private ReadOnly _lstHost As IPAddress = IPAddress.Parse(_config.RealmServerAddress)
        Private ReadOnly _lstConnection As TcpListener
        'Private lstThreadPool As ThreadPool

        Public Sub New()
            Try
                _lstConnection = New TcpListener(_lstHost, _config.RealmServerPort)
                _lstConnection.Start()
                Dim rsListenThread As Thread
                rsListenThread = New Thread(AddressOf AcceptConnection)
                rsListenThread.Name = "Realm Server, Listening"
                rsListenThread.Start()

                Console.WriteLine("[{0}] Listening on {1} on port {2}", Format(TimeOfDay, "hh:mm:ss"), _lstHost, _config.RealmServerPort)
            Catch e As Exception
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Error in {2}: {1}.", Format(TimeOfDay, "hh:mm:ss"), e.Message, e.Source)
                Console.ForegroundColor = ConsoleColor.Gray
            End Try
        End Sub

        Private Sub AcceptConnection()
            Do While Not FlagStopListen
                Thread.Sleep(ConnectionSleepTime)
                If _lstConnection.Pending() Then
                    Dim client As New ClientClass
                    client.Socket = _lstConnection.AcceptSocket
                    'lstThreadPool.QueueUserWorkItem(New System.Threading.WaitCallback(AddressOf client.Process))

                    Dim newThread As New Thread(AddressOf client.Process)
                    newThread.Start()
                End If
            Loop
        End Sub

#Region "IDisposable Support"

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Default Functions
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                FlagStopListen = True
                _lstConnection.Stop()
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

#End Region

#Region "RS.Data Access"

    Private _accountDatabase As New SQL

    Private Sub SqlEventHandler(ByVal messageId As SQL.EMessages, ByVal outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Console.ForegroundColor = ConsoleColor.Red
            Case SQL.EMessages.ID_Message
                Console.ForegroundColor = ConsoleColor.DarkGreen
        End Select

        Console.WriteLine("[" & Format(TimeOfDay, "hh:mm:ss") & "] " & outBuf)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

#End Region

#Region "RS.Analyzer"

    'Public Enum WoWLanguage As Byte
    '    EnGb = 0
    '    EnUs = 1
    '    DeDe = 2
    '    FrFr = 3
    'End Enum

    Private NotInheritable Class ClientClass
        Implements IDisposable

        Public Socket As Socket
        Public Ip As IPAddress = IPAddress.Parse("127.0.0.1")
        Public Port As Int32 = 0
        Public AuthEngine As AuthEngineClass
        Public Account As String = ""
        'Public Language As String = "enGB"
        'Public Expansion As ExpansionLevel = ExpansionLevel.NORMAL
        Public UpdateFile As String = ""
        Public Access As AccessLevel = AccessLevel.Player

        Private Sub OnData(ByVal data() As Byte)
            Select Case data(0)
                Case AuthCMD.CMD_AUTH_LOGON_CHALLENGE
                    'Console.WriteLine("[{0}] [{1}:{2}] RS_LOGON_CHALLENGE", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                    On_RS_LOGON_CHALLENGE(data, Me)
                Case AuthCMD.CMD_AUTH_LOGON_PROOF
                    'Console.WriteLine("[{0}] [{1}:{2}] RS_LOGON_PROOF", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                    On_RS_LOGON_PROOF(data, Me)
                Case AuthCMD.CMD_AUTH_REALMLIST
                    'Console.WriteLine("[{0}] [{1}:{2}] RS_REALMLIST", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                    On_RS_REALMLIST(data, Me)

                'TODO: No Value listed for AuthCMD
                'Case CMD_AUTH_UPDATESRV
                '    Console.WriteLine("[{0}] [{1}:{2}] RS_UPDATESRV", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)

                Case AuthCMD.CMD_XFER_ACCEPT
                    'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_ACCEPT", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                    On_CMD_XFER_ACCEPT(data, Me)
                Case AuthCMD.CMD_XFER_RESUME
                    'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_RESUME", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                    On_CMD_XFER_RESUME(data, Me)
                Case AuthCMD.CMD_XFER_CANCEL
                    'Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_CANCEL", Format(TimeOfDay, "hh:mm:ss"), IP, Port)
                    On_CMD_XFER_CANCEL(data, Me)
                Case Else
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("[{0}] [{1}:{2}] Unknown Opcode 0x{3}", Format(TimeOfDay, "hh:mm:ss"), Ip, Port, data(0))
                    Console.ForegroundColor = ConsoleColor.Gray
                    DumpPacket(data, Me)
            End Select
        End Sub

        Public Sub Process()
            Ip = CType(Socket.RemoteEndPoint, IPEndPoint).Address
            Port = CType(Socket.RemoteEndPoint, IPEndPoint).Port

            'DONE: Connection spam protection
            Dim ipInt As UInteger
            ipInt = Ip2Int(Ip.ToString)
            If LastConnections.ContainsKey(ipInt) Then
                If Now > LastConnections(ipInt) Then
                    LastConnections(ipInt) = Now.AddSeconds(5)
                Else
                    Socket.Close()
                    Dispose()
                    Exit Sub
                End If
            Else
                LastConnections.Add(ipInt, Now.AddSeconds(5))
            End If

            Dim buffer() As Byte


            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Incoming connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
            Console.WriteLine("[{0}] [{1}:{2}] Checking for banned IP.", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
            Console.ForegroundColor = ConsoleColor.Gray
            If Not _accountDatabase.QuerySql("SELECT ip FROM ip_banned WHERE ip = '" & Ip.ToString & "';") Then

                While Not _realmServer.FlagStopListen
                    Thread.Sleep(ConnectionSleepTime)
                    If Socket.Available > 0 Then
                        If Socket.Available > 500 Then 'DONE: Data flood protection
                            Exit While
                        End If
                        ReDim buffer(Socket.Available - 1)
                        Dim dummyBytes As Integer = Socket.Receive(buffer, buffer.Length, 0)
                        Console.WriteLine("[{0}] Incoming connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
                        Console.WriteLine("[{0}] Data Packet Flood:", dummyBytes)

                        OnData(buffer)
                    End If
                    If Not Socket.Connected Then Exit While
                    If (Socket.Poll(100, SelectMode.SelectRead)) And (Socket.Available = 0) Then Exit While
                End While

            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] [{1}:{2}] This ip is banned.", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
                Console.ForegroundColor = ConsoleColor.Gray
            End If

            Socket.Close()

            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Connection from [{1}:{2}] closed", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
            Console.ForegroundColor = ConsoleColor.Gray

            Dispose()
        End Sub

        Public Sub Send(ByVal data() As Byte, ByVal packetName As String)
            Try
                Dim i As Integer = Socket.Send(data, 0, data.Length, SocketFlags.None)
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("[{0}] [{1}:{2}] ({4}) Data sent, result code={3}", Format(TimeOfDay, "hh:mm:ss"), Ip, Port, i, packetName)
                Console.ForegroundColor = ConsoleColor.Gray

            Catch err As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Connection from [{1}:{2}] do not exist - ERROR!!!", Format(TimeOfDay, "hh:mm:ss"), Ip, Port)
                Console.ForegroundColor = ConsoleColor.Gray
                Socket.Close()
            End Try
        End Sub

#Region "IDisposable Support"

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Private Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
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

#End Region

#Region "RS_OPCODES"

    Private Sub On_RS_LOGON_CHALLENGE(ByRef data() As Byte, ByRef client As ClientClass)
        Dim iUpper As Integer = (data(33) - 1)
        'Dim packetSize As Integer = BitConverter.ToInt16(New Byte() {data(3), data(2)}, 0)
        Dim packetAccount As String
        Dim packetIp As String
        Dim accState As AccountState '= AccountState.LOGIN_DBBUSY

        'Read account name from packet
        packetAccount = ""
        For i As Integer = 0 To iUpper
            packetAccount = packetAccount + Chr(data(34 + i))
        Next i
        client.Account = packetAccount

        'Read users ip from packet
        packetIp = CInt(data(29)).ToString & "." & CInt(data(30)).ToString & "." & CInt(data(31)).ToString & "." & CInt(data(32)).ToString

        'Get the client build from packet.
        Dim bMajor As Integer = data(8)
        Dim bMinor As Integer = data(9)
        Dim bRevision As Integer = data(10)
        Dim clientBuild As Integer = BitConverter.ToInt16((New Byte() {data(11), data(12)}), 0)
        Dim clientLanguage As String = Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21))

        Console.WriteLine("[{0}] [{1}:{2}] CMD_AUTH_LOGON_CHALLENGE [{3}] [{4}], WoW Version [{5}.{6}.{7}.{8}] [{9}].", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount, packetIp, bMajor.ToString, bMinor.ToString, bRevision.ToString, clientBuild.ToString, clientLanguage)

        'DONE: Check if our build can join the server
        'If ((RequiredVersion1 = 0 AndAlso RequiredVersion2 = 0 AndAlso RequiredVersion3 = 0) OrElse
        '    (bMajor = RequiredVersion1 AndAlso bMinor = RequiredVersion2 AndAlso bRevision = RequiredVersion3)) AndAlso
        '   clientBuild >= RequiredBuildLow AndAlso clientBuild <= RequiredBuildHigh Then
        If bMajor = 0 And bMinor = 0 And bRevision = 0 Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("[{0}] [{1}:{2}] Invalid Client", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
            Console.ForegroundColor = ConsoleColor.White
            Dim dataResponse(1) As Byte
            dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
            dataResponse(1) = AccountState.LOGIN_BADVERSION
            client.Send(dataResponse, "RS_LOGON_CHALLENGE-FAIL-BADVERSION")
        ElseIf (clientBuild = Required_Build_1_12_1) Or (clientBuild = Required_Build_1_12_2) Or (clientBuild = Required_Build_1_12_3) Then
            'TODO: in the far future should check if the account is expired too
            Dim result As DataTable = Nothing
            Try
                'Get Account info
                _accountDatabase.Query(String.Format("SELECT id, sha_pass_hash, gmlevel, expansion FROM account WHERE username = ""{0}"";", packetAccount), result)

                'Check Account state
                If result.Rows.Count > 0 Then
                    If _accountDatabase.QuerySQL("SELECT id FROM account_banned WHERE id = '" & result.Rows(0).Item("id") & "';") Then
                        accState = AccountState.LOGIN_BANNED
                    Else
                        accState = AccountState.LOGIN_OK
                    End If
                Else
                    accState = AccountState.LOGIN_UNKNOWN_ACCOUNT
                End If
            Catch
                accState = AccountState.LOGIN_DBBUSY
            End Try

            'DONE: Send results to client
            Select Case accState
                Case AccountState.LOGIN_OK
                    Console.WriteLine("[{0}] [{1}:{2}] Account found [{3}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount)

                    Dim account(data(33) - 1) As Byte
                    Array.Copy(data, 34, account, 0, data(33))
                    Dim pwHash As String = result.Rows(0).Item("sha_pass_hash")
                    If pwHash.Length = 40 Then 'Invalid password type, should always be 40 characters

                        client.Access = result.Rows(0).Item("gmlevel")

                        Dim hash() As Byte = New Byte(19) {}
                        For i As Integer = 0 To 39 Step 2
                            hash(i \ 2) = CInt("&H" & pwHash.Substring(i, 2))
                        Next

                        'client.Language = clientLanguage
                        'If Not IsDBNull(result.Rows(0).Item("expansion")) Then
                        '    client.Expansion = result.Rows(0).Item("expansion")
                        'Else
                        '    client.Expansion = ExpansionLevel.NORMAL
                        'End If

                        Try
                            client.AuthEngine = New AuthEngineClass
                            client.AuthEngine.CalculateX(account, hash)

                            Dim dataResponse(118) As Byte
                            dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_CHALLENGE
                            dataResponse(1) = AccountState.LOGIN_OK
                            dataResponse(2) = Val("&H00")
                            Array.Copy(client.AuthEngine.PublicB, 0, dataResponse, 3, 32)
                            dataResponse(35) = client.AuthEngine.g.Length
                            dataResponse(36) = client.AuthEngine.g(0)
                            dataResponse(37) = 32
                            Array.Copy(client.AuthEngine.N, 0, dataResponse, 38, 32)
                            Array.Copy(client.AuthEngine.Salt, 0, dataResponse, 70, 32)
                            Array.Copy(AuthEngineClass.CrcSalt, 0, dataResponse, 102, 16)
                            dataResponse(118) = 0 ' Added in 1.12.x client branch? Security Flags (&H0...&H4)?
                            client.Send(dataResponse, "RS_LOGON_CHALLENGE")
                        Catch ex As Exception
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.WriteLine("[{0}] [{1}:{2}] Error loading AuthEngine: {3}{4}", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, vbNewLine, ex)
                            Console.ForegroundColor = ConsoleColor.White
                        End Try
                    Else 'Bail out with something meaningful
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("[{0}] [{1}:{2}] Not a valid SHA1 password for account: '{3}' SHA1 Hash: '{4}'", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount, pwHash)
                        Console.ForegroundColor = ConsoleColor.White
                        Dim dataResponse(1) As Byte
                        dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                        dataResponse(1) = AccountState.LOGIN_BAD_PASS
                        client.Send(dataResponse, "RS_LOGON_CHALLENGE-FAIL-BADPWFORMAT")
                    End If

                    Exit Sub
                Case AccountState.LOGIN_UNKNOWN_ACCOUNT
                    Console.WriteLine("[{0}] [{1}:{2}] Account not found [{3}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount)
                    Dim dataResponse(1) As Byte
                    dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                    dataResponse(1) = AccountState.LOGIN_UNKNOWN_ACCOUNT
                    client.Send(dataResponse, "RS_LOGON_CHALLENGE-UNKNOWN_ACCOUNT")
                    Exit Sub
                Case AccountState.LOGIN_BANNED
                    Console.WriteLine("[{0}] [{1}:{2}] Account banned [{3}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount)
                    Dim dataResponse(1) As Byte
                    dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                    dataResponse(1) = AccountState.LOGIN_BANNED
                    client.Send(dataResponse, "RS_LOGON_CHALLENGE-BANNED")
                    Exit Sub
                Case AccountState.LOGIN_NOTIME
                    Console.WriteLine("[{0}] [{1}:{2}] Account prepaid time used [{3}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount)
                    Dim dataResponse(1) As Byte
                    dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                    dataResponse(1) = AccountState.LOGIN_NOTIME
                    client.Send(dataResponse, "RS_LOGON_CHALLENGE-NOTIME")
                    Exit Sub
                Case AccountState.LOGIN_ALREADYONLINE
                    Console.WriteLine("[{0}] [{1}:{2}] Account already logged in the game [{3}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount)
                    Dim dataResponse(1) As Byte
                    dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                    dataResponse(1) = AccountState.LOGIN_ALREADYONLINE
                    client.Send(dataResponse, "RS_LOGON_CHALLENGE-ALREADYONLINE")
                    Exit Sub
                Case Else
                    Console.WriteLine("[{0}] [{1}:{2}] Account error [{3}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount)
                    Dim dataResponse(1) As Byte
                    dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                    dataResponse(1) = AccountState.LOGIN_FAILED
                    client.Send(dataResponse, "RS_LOGON_CHALLENGE-FAILED")
                    Exit Sub
            End Select
        Else
            If Dir("Updates/wow-patch-" & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & "-" & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & ".mpq") <> "" Then
                'Send UPDATE_MPQ
                Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_INITIATE [" & Chr(data(6)) & Chr(data(5)) & Chr(data(4)) & " " & data(8) & "." & data(9) & "." & data(10) & "." & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & " " & Chr(data(15)) & Chr(data(14)) & Chr(data(13)) & " " & Chr(data(19)) & Chr(data(18)) & Chr(data(17)) & " " & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & "]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)

                client.UpdateFile = "Updates/wow-patch-" & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & "-" & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & ".mpq"
                Dim dataResponse(30) As Byte

                dataResponse(0) = AuthCMD.CMD_XFER_INITIATE
                'Name Len 0x05 -> sizeof(Patch)
                Dim i As Integer = 1
                ToBytes(CType(5, Byte), dataResponse, i)
                'Name 'Patch'
                ToBytes("Patch", dataResponse, i)
                'Size 0x34 C4 0D 00 = 902,196 byte (180->181 enGB)
                ToBytes(CType(FileLen(client.UpdateFile), Integer), dataResponse, i)
                'Unknown 0x0 always
                ToBytes(0, dataResponse, i)
                'MD5 CheckSum
                Dim md5 As New MD5CryptoServiceProvider
                Dim buffer() As Byte
                Dim fs As FileStream = New FileStream(client.UpdateFile, FileMode.Open)
                Dim r As BinaryReader = New BinaryReader(fs)
                buffer = r.ReadBytes(FileLen(client.UpdateFile))
                r.Close()
                '                fs.Close()
                Dim result As Byte() = md5.ComputeHash(buffer)
                Array.Copy(result, 0, dataResponse, 15, 16)
                client.Send(dataResponse, "RS_LOGON_CHALLENGE-CMD-XFER-INITIATE")
            Else
                'Send BAD_VERSION
                Console.WriteLine("[{0}] [{1}:{2}] WRONG_VERSION [" & Chr(data(6)) & Chr(data(5)) & Chr(data(4)) & " " & data(8) & "." & data(9) & "." & data(10) & "." & (Val("&H" & Hex(data(12)) & Hex(data(11)))) & " " & Chr(data(15)) & Chr(data(14)) & Chr(data(13)) & " " & Chr(data(19)) & Chr(data(18)) & Chr(data(17)) & " " & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & "]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
                Dim dataResponse(1) As Byte
                dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                dataResponse(1) = AccountState.LOGIN_BADVERSION
                client.Send(dataResponse, "RS_LOGON_CHALLENGE-WRONG-VERSION")
            End If
        End If
    End Sub

    Private Sub On_RS_LOGON_PROOF(ByRef data() As Byte, ByRef client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_AUTH_LOGON_PROOF", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
        Dim a(31) As Byte
        Array.Copy(data, 1, a, 0, 32)
        Dim m1(19) As Byte
        Array.Copy(data, 33, m1, 0, 20)
        'Dim CRC_Hash(19) As Byte
        'Array.Copy(data, 53, CRC_Hash, 0, 20)
        'Dim NumberOfKeys as Byte = data(73)
        'Dim unk as Byte = data(74)

        'Calculate U and M1
        client.AuthEngine.CalculateU(a)
        client.AuthEngine.CalculateM1()
        'Client.AuthEngine.CalculateCRCHash()

        'Check M1=ClientM1
        Dim passCheck As Boolean = True
        For i As Byte = 0 To 19
            If m1(i) <> client.AuthEngine.M1(i) Then
                passCheck = False
                Exit For
            End If
        Next

        If passCheck Then
            client.AuthEngine.CalculateM2(m1)

            Dim dataResponse(25) As Byte
            dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
            dataResponse(1) = AccountState.LOGIN_OK
            Array.Copy(client.AuthEngine.M2, 0, dataResponse, 2, 20)
            dataResponse(22) = 0
            dataResponse(23) = 0
            dataResponse(24) = 0
            dataResponse(25) = 0

            client.Send(dataResponse, "RS_LOGON_PROOF-OK")
            'Set SSHash in DB
            Dim sshash As String = ""
            'For i as Integer = 0 To client.AuthEngine.SS_Hash.Length - 1
            For i As Integer = 0 To 40 - 1
                If client.AuthEngine.SsHash(i) < 16 Then
                    sshash = sshash + "0" + Hex(client.AuthEngine.SsHash(i))
                Else
                    sshash = sshash + Hex(client.AuthEngine.SsHash(i))
                End If
            Next
            _accountDatabase.Update(String.Format("UPDATE account SET sessionkey = '{1}', last_ip='{2}', last_login='{3}' WHERE username = '{0}';", client.Account, sshash, client.Ip.ToString, Format(Now, "yyyy-MM-dd")))

            Console.WriteLine("[{0}] [{1}:{2}] Auth success for user {3}. [{4}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, client.Account, sshash)
        Else
            'Wrong pass
            Console.WriteLine("[{0}] [{1}:{2}] Wrong password for user {3}.", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, client.Account)
            Dim dataResponse(1) As Byte
            dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
            dataResponse(1) = AccountState.LOGIN_UNKNOWN_ACCOUNT
            client.Send(dataResponse, "RS_LOGON_PROOF-WRONGPASS")
        End If
    End Sub

    Private Sub On_RS_REALMLIST(ByRef data() As Byte, ByRef client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_REALM_LIST", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)

        Dim packetLen As Integer = 0
        Dim characterCount As Integer = 0
        Dim result As DataTable = Nothing
        Dim countresult As DataTable = Nothing

        ' Retrieve the Account ID
        _accountDatabase.Query(String.Format("SELECT id FROM account WHERE username = ""{0}"";", client.Account), result)
        Dim accountId As Integer = result.Rows(0).Item("id")

        If client.Access < AccessLevel.GameMaster Then
            'Console.WriteLine("[{0}] [{1}:{2}] Player is not a Gamemaster, only listing non-GMonly realms", Format(TimeOfDay, "hh:mm:ss"), client.IP, client.Port)
            _accountDatabase.Query(String.Format("SELECT * FROM realmlist WHERE allowedSecurityLevel = '0';"), result)
        Else
            'Console.WriteLine("[{0}] [{1}:{2}] Player is a Gamemaster, listing all realms", Format(TimeOfDay, "hh:mm:ss"), client.IP, client.Port)
            _accountDatabase.Query(String.Format("SELECT * FROM realmlist;"), result)
        End If

        For Each row As DataRow In result.Rows
            packetLen = packetLen + Len(row.Item("address")) + Len(row.Item("name")) + 1 + Len(Format(row.Item("port"), "0")) + 14
        Next

        Dim tmp As Integer = 8
        Dim dataResponse(packetLen + 9) As Byte

        '(byte) Opcode
        dataResponse(0) = AuthCMD.CMD_AUTH_REALMLIST

        '(uint16) Packet Length
        dataResponse(2) = (packetLen + 7) \ 256
        dataResponse(1) = (packetLen + 7) Mod 256

        '(uint32) Unk
        dataResponse(3) = data(1)
        dataResponse(4) = data(2)
        dataResponse(5) = data(3)
        dataResponse(6) = data(4)

        '(uint16) Realms Count
        dataResponse(7) = result.Rows.Count

        For Each host As DataRow In result.Rows
            Dim hostRealmId As Integer = host.Item("id")

            ' Get Number of Characters for the Realm
            _accountDatabase.Query(String.Format("SELECT * FROM realmcharacters WHERE realmid = '" & hostRealmId & "' AND acctid = '" & accountId & "';"), countresult)

            If (countresult.Rows.Count > 0) Then
                characterCount = countresult.Rows(0).Item("numchars")
            End If

            '(uint8) Realm Icon
            '	0 -> Normal; 1 -> PvP; 6 -> RP; 8 -> RPPvP;
            ToBytes(CType(host.Item("icon"), Byte), dataResponse, tmp)
            '(uint8) IsLocked
            '	0 -> none; 1 -> locked
            ToBytes(CType(0, Byte), dataResponse, tmp)
            '(uint8) unk
            ToBytes(CType(0, Byte), dataResponse, tmp)
            '(uint8) unk
            ToBytes(CType(0, Byte), dataResponse, tmp)
            '(uint8) Realm Color
            '   0 -> Green; 1 -> Red; 2 -> Offline;
            ToBytes(CType(host.Item("realmflags"), Byte), dataResponse, tmp)
            '(string) Realm Name (zero terminated)
            ToBytes(CType(host.Item("name"), String), dataResponse, tmp)
            ToBytes(CType(0, Byte), dataResponse, tmp) '\0
            '(string) Realm Address ("ip:port", zero terminated)
            ToBytes(host.Item("address") & ":" & host.Item("port"), dataResponse, tmp)
            ToBytes(CType(0, Byte), dataResponse, tmp) '\0
            '(float) Population
            '   400F -> Full; 5F -> Medium; 1.6F -> Low; 200F -> New; 2F -> High
            '   00 00 48 43 -> Recommended
            '   00 00 C8 43 -> Full
            '   9C C4 C0 3F -> Low
            '   BC 74 B3 3F -> Low
            ToBytes(CType(host.Item("population"), Single), dataResponse, tmp)
            '(byte) Number of character at this realm for this account
            ToBytes(CType(characterCount, Byte), dataResponse, tmp)
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
            ToBytes(CType(host.Item("timezone"), Byte), dataResponse, tmp)
            '(byte) Unknown (may be 2 -> TestRealm, / 6 -> ?)
            ToBytes(CType(0, Byte), dataResponse, tmp)
        Next

        dataResponse(tmp) = 2 '2=list of realms 0=wizard
        dataResponse(tmp + 1) = 0

        client.Send(dataResponse, "RS-REALMLIST")
    End Sub

    Private Sub On_CMD_XFER_CANCEL(ByRef data() As Byte, ByRef client As ClientClass)
        'TODO: data parameter is never used
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_CANCEL", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
        client.Socket.Close()
    End Sub

    Private Sub On_CMD_XFER_ACCEPT(ByRef data() As Byte, ByRef client As ClientClass)
        'TODO: data parameter is never used
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_ACCEPT", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
        Dim tmp As Integer '= 1
        Dim buffer() As Byte
        Dim filelen As Integer
        filelen = FileSystem.FileLen(client.UpdateFile)
        Dim fileOffset As Integer = 0
        Dim fs As FileStream = New FileStream(client.UpdateFile, FileMode.Open, FileAccess.Read)
        Dim r As BinaryReader = New BinaryReader(fs)
        buffer = r.ReadBytes(filelen)
        r.Close()
        ' fs.Close()

        Const maxUpdatePacketSize As Integer = 1500

        If filelen > maxUpdatePacketSize Then
            Dim dataResponse() As Byte

            While filelen > maxUpdatePacketSize
                tmp = 1
                ReDim dataResponse(maxUpdatePacketSize + 2)
                dataResponse(0) = AuthCMD.CMD_XFER_DATA
                ToBytes(CType(maxUpdatePacketSize, Short), dataResponse, tmp)
                Array.Copy(buffer, fileOffset, dataResponse, 3, maxUpdatePacketSize)
                filelen = filelen - maxUpdatePacketSize
                fileOffset = fileOffset + maxUpdatePacketSize
                client.Send(dataResponse, "CMD-XFER-ACCEPT-1")
            End While
            tmp = 1
            ReDim dataResponse(filelen + 2)
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, fileOffset, dataResponse, 3, filelen)
            client.Send(dataResponse, "CMD-XFER-ACCEPT-2")
        Else
            tmp = 1
            Dim dataResponse(filelen + 2) As Byte
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, 0, dataResponse, 3, filelen)
            client.Send(dataResponse, "CMD-XFER-ACCEPT-3")
        End If
        'Client.Socket.Close()
    End Sub

    Private Sub On_CMD_XFER_RESUME(ByRef data() As Byte, ByRef client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_RESUME", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
        Dim tmp As Integer = 1
        Dim buffer() As Byte
        Dim filelen As Integer
        filelen = FileSystem.FileLen(client.UpdateFile)
        Dim fileOffset As Integer
        fileOffset = ToInt32(data, tmp)
        filelen = filelen - fileOffset

        Dim fs As FileStream = New FileStream(client.UpdateFile, FileMode.Open, FileAccess.Read)
        Dim r As BinaryReader = New BinaryReader(fs)

        r.ReadBytes(fileOffset)
        buffer = r.ReadBytes(filelen)
        r.Close()
        '        fs.Close()
        fileOffset = 0

        Const maxUpdatePacketSize As Integer = 1500

        If filelen > maxUpdatePacketSize Then
            Dim dataResponse() As Byte

            While filelen > maxUpdatePacketSize
                tmp = 1
                ReDim dataResponse(maxUpdatePacketSize + 2)
                dataResponse(0) = AuthCMD.CMD_XFER_DATA
                ToBytes(CType(maxUpdatePacketSize, Short), dataResponse, tmp)
                Array.Copy(buffer, fileOffset, dataResponse, 3, maxUpdatePacketSize)
                filelen = filelen - maxUpdatePacketSize
                fileOffset = fileOffset + maxUpdatePacketSize
                client.Send(dataResponse, "XFER-RESUME")
            End While
            tmp = 1
            ReDim dataResponse(filelen + 2)
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, fileOffset, dataResponse, 3, filelen)
            client.Send(dataResponse, "XFER-RESUME-XFER-DATALARGER")
        Else
            tmp = 1
            Dim dataResponse(filelen + 2) As Byte
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, 0, dataResponse, 3, filelen)
            client.Send(dataResponse, "XFER-RESUME-XFER-DATA")
        End If
        'Client.Socket.Close()
    End Sub

    Private Sub DumpPacket(ByRef data() As Byte, ByRef client As ClientClass)
        Dim j As Integer
        Dim buffer As String = ""
        If client Is Nothing Then
            buffer = buffer + String.Format("[{0}] DEBUG: Packet Dump{1}", Format(TimeOfDay, "hh:mm:ss"), vbNewLine)
        Else
            buffer = buffer + String.Format("[{0}] [{1}:{2}] DEBUG: Packet Dump{3}", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, vbNewLine)
        End If

        If data.Length Mod 16 = 0 Then
            For j = 0 To data.Length - 1 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
            Next
        Else
            For j = 0 To data.Length - 1 - 16 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
            Next

            buffer += "|  " & BitConverter.ToString(data, j, data.Length Mod 16).Replace("-", " ")
            buffer += New String(" ", (16 - data.Length Mod 16) * 3)
            buffer += " |  " & Encoding.ASCII.GetString(data, j, data.Length Mod 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?")
            buffer += New String(" ", 16 - data.Length Mod 16)
            buffer += " |" & vbNewLine
        End If

        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(buffer)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

#End Region

    Private Sub WorldServer_Status_Report()
        Dim result1 As DataTable = New DataTable
        Dim returnValues As Integer
        returnValues = _accountDatabase.Query(String.Format("SELECT * FROM realmlist WHERE allowedSecurityLevel < '1';"), result1)
        If returnValues > SQL.ReturnState.Success Then 'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        Dim result2 As DataTable = New DataTable
        returnValues = _accountDatabase.Query(String.Format("SELECT * FROM realmlist WHERE realmflags < 2 && allowedSecurityLevel < '1';"), result2)
        If returnValues > SQL.ReturnState.Success Then 'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        Dim result3 As DataTable = New DataTable
        returnValues = _accountDatabase.Query(String.Format("SELECT * FROM realmlist WHERE realmflags < 2 && allowedSecurityLevel >= '1';"), result3)
        If returnValues > SQL.ReturnState.Success Then 'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        Console.WriteLine()
        Console.WriteLine("[{0}] Known World Servers are {1}, Online World Servers are {2}", Format(TimeOfDay, "hh:mm:ss"), result1.Rows.Count, result2.Rows.Count)
        Console.WriteLine("[{0}] GM Only World Servers are {1}", Format(TimeOfDay, "hh:mm:ss"), result3.Rows.Count)

        Console.ForegroundColor = ConsoleColor.DarkGreen
        For Each row As DataRow In result1.Rows
            Console.WriteLine("     [{1}] at {0}:{2} - {3}", row.Item("address").PadRight(6), row.Item("name").PadRight(6), Format(row.Item("port")).PadRight(6), WorldServerStatus(Int(row.Item("realmflags"))).PadRight(6))
        Next
        Console.ForegroundColor = ConsoleColor.Yellow
        For Each row As DataRow In result3.Rows
            Console.WriteLine("     [{1}] at {0}:{2} - {3}", row.Item("address").PadRight(6), row.Item("name").PadRight(20), Format(row.Item("port")), WorldServerStatus(Int(row.Item("realmflags"))).PadRight(10))
        Next
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Sub Main()
        Dim log As New BaseWriter

        Console.Title = String.Format("{0} v{1}", [Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0).Title, [Assembly].GetExecutingAssembly().GetName().Version)

        Console.ForegroundColor = ConsoleColor.Yellow

        Console.WriteLine("  __  __      _  _  ___  ___  ___   __   __ ___               ")
        Console.WriteLine(" |  \/  |__ _| \| |/ __|/ _ \/ __|  \ \ / /| _ )      We Love ")
        Console.WriteLine(" | |\/| / _` | .` | (_ | (_) \__ \   \ V / | _ \   Vanilla Wow")
        Console.WriteLine(" |_|  |_\__,_|_|\_|\___|\___/|___/    \_/  |___/              ")
        Console.WriteLine("                                                              ")
        Console.WriteLine(" Website / Forum / Support: https://getmangos.eu/          ")
        Console.WriteLine("")

        Console.ForegroundColor = ConsoleColor.Magenta

        Console.ForegroundColor = ConsoleColor.White
        Console.Write([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0).Title)
        Console.WriteLine(" version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Gray

        Console.WriteLine("[{0}] Realm Server Starting...", Format(TimeOfDay, "hh:mm:ss"))

        If System.IO.File.Exists("Shared.dll") = False Then
            log.WriteLine(LogType.CRITICAL, "Failed to find Shared.dll, server startup aborted")
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("[{0}] Failed to find Shared.dll, server startup aborted", Format(TimeOfDay, "hh:mm:ss"))
            Console.ReadKey()
            End
        End If
        LoadConfig()

        Console.ForegroundColor = ConsoleColor.Yellow
        log.WriteLine(LogType.INFORMATION, "Running from: {0}", AppDomain.CurrentDomain.BaseDirectory)
        Console.ForegroundColor = ConsoleColor.Gray

        AddHandler _accountDatabase.SqlMessage, AddressOf SqlEventHandler
        _accountDatabase.Connect()

        _realmServer = New RealmServerClass

        'Check the Database version, exit if its wrong
        Dim areDbVersionsOk As Boolean = True
        If Globals.CheckRequiredDbVersion(_accountDatabase, ServerDb.Realm) = False Then areDbVersionsOk = False
        'If CheckRequiredDbVersion(WorldDatabase, ServerDb.World) = False Then areDbVersionsOk = False
        'If CheckRequiredDbVersion(CharacterDatabase, ServerDb.Character) = False Then areDbVersionsOk = False

        If areDbVersionsOk = False Then
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If
        WorldServer_Status_Report()
    End Sub

    Private Function Ip2Int(ByVal ip As String) As UInteger
        Dim ipSplit() As String = ip.Split(".")
        If ipSplit.Length <> 4 Then Return 0
        Dim ipBytes(3) As Byte
        Try
            ipBytes(0) = ipSplit(3)
            ipBytes(1) = ipSplit(2)
            ipBytes(2) = ipSplit(1)
            ipBytes(3) = ipSplit(0)
            Return BitConverter.ToUInt32(ipBytes, 0)
        Catch
            Return 0
        End Try
    End Function

    Private Sub GenericExceptionHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim EX As Exception
        EX = e.ExceptionObject

        Log.WriteLine(LogType.CRITICAL, EX.ToString & vbNewLine)
        Log.WriteLine(LogType.FAILED, "Unexpected error has occured. An 'RealmServer-Error-yyyy-mmm-d-h-mm.log' file has been created. Check your log folder for more information.")

        Dim tw As TextWriter
        tw = New StreamWriter(New FileStream(String.Format("RealmServer-Error-{0}.log", Format(Now, "yyyy-MMM-d-H-mm")), FileMode.Create))
        tw.Write(EX.ToString)
        tw.Close()
    End Sub
End Module
