'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.Data
Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports Mangos.Common
Imports Mangos.Common.Enums.Authentication
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Globals
Imports Mangos.Common.Logging
Imports Mangos.Configuration
Imports Mangos.Realm.Factories

Public Class RealmServer
    Private ReadOnly _configurationProvider As IConfigurationProvider(Of RealmServerConfiguration)

    Private ReadOnly _CommonGlobalFunctions As Common.Globals.Functions
    Private ReadOnly _Converter As Converter
    Private ReadOnly _Global_Constants As Global_Constants
    Private ReadOnly _RealmServerClassFactory As RealmServerClassFactory

    Private Const RealmPath As String = "configs/RealmServer.ini"

    Public Log As New BaseWriter

    Public Sub New(
                   commonGlobalFunctions As Common.Globals.Functions,
                   converter As Converter,
                   globalConstants As Global_Constants,
                   realmServerClassFactory As RealmServerClassFactory,
                   configurationProvider As IConfigurationProvider(Of RealmServerConfiguration))
        _CommonGlobalFunctions = commonGlobalFunctions
        _Converter = converter
        _Global_Constants = globalConstants
        _RealmServerClassFactory = realmServerClassFactory
        _configurationProvider = configurationProvider
    End Sub

    Private Async Function LoadConfig() As Task
        Try
            'Make sure RealmServer.ini exists
            If File.Exists(RealmPath) = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Cannot Continue. {1} does not exist.", Format(TimeOfDay, "hh:mm:ss"), RealmPath)
                Console.WriteLine("Please make sure your ini files are inside config folder where the mangosvb executables are located.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If

            Console.Write("[{0}] Loading Configuration...", Format(TimeOfDay, "hh:mm:ss"))

            Console.WriteLine(".[done]")

            'DONE: Setting SQL Connection
            Dim configuration = Await _configurationProvider.GetConfigurationAsync()
            Dim accountDbSettings() As String = Split(configuration.AccountDatabase, ";")
            If accountDbSettings.Length <> 6 Then
                Console.WriteLine("Invalid connect string for the account database!")
            Else
                AccountDatabase.SQLDBName = accountDbSettings(4)
                AccountDatabase.SQLHost = accountDbSettings(2)
                AccountDatabase.SQLPort = accountDbSettings(3)
                AccountDatabase.SQLUser = accountDbSettings(0)
                AccountDatabase.SQLPass = accountDbSettings(1)
                AccountDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), accountDbSettings(5))
            End If

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Function

    Public Property RealmServer As RealmServerClass
    Public ReadOnly Property LastSocketConnection As New Dictionary(Of UInteger, Date)
    Public Property AccountDatabase As New SQL

    Private Sub SqlEventHandler(ByVal messageId As SQL.EMessages, ByVal outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Console.ForegroundColor = ConsoleColor.Red
            Case SQL.EMessages.ID_Message
                Console.ForegroundColor = ConsoleColor.DarkGreen
            Case Else
                Exit Select
        End Select

        Console.WriteLine("[" & Format(TimeOfDay, "hh:mm:ss") & "] " & outBuf)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    'Public Enum WoWLanguage As Byte
    '    EnGb = 0
    '    EnUs = 1
    '    DeDe = 2
    '    FrFr = 3
    'End Enum

    Public Sub On_RS_LOGON_CHALLENGE(ByRef data() As Byte, ByRef client As ClientClass)
        Dim iUpper As Integer = (data(33) - 1)
        Dim packetSize As Integer = BitConverter.ToInt16(New Byte() {data(3), data(2)}, 0)
        Dim packetAccount As String
        Dim packetIp As String
        Dim accState As AccountState '= AccountState.LOGIN_DBBUSY

        'Read account name from packet
        packetAccount = ""
        For i As Integer = 0 To iUpper
            packetAccount += Chr(data(34 + i))
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
        ElseIf (clientBuild = _Global_Constants.Required_Build_1_12_1) Or (clientBuild = _Global_Constants.Required_Build_1_12_2) Or (clientBuild = _Global_Constants.Required_Build_1_12_3) Then
            'TODO: in the far future should check if the account is expired too
            Dim result As DataTable = Nothing
            Try
                'Get Account info
                AccountDatabase.Query($"SELECT id, sha_pass_hash, gmlevel, expansion FROM account WHERE username = ""{packetAccount }"";", result)

                'Check Account state
                If result.Rows.Count > 0 Then
                    accState = If(AccountDatabase.QuerySQL("SELECT id FROM account_banned WHERE id = '" & result.Rows(0).Item("id") & "';"),
                        AccountState.LOGIN_BANNED,
                        AccountState.LOGIN_OK)
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
                    If result.Rows(0).Item("sha_pass_hash").Length <> 40 Then 'Invalid password type, should always be 40 characters
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("[{0}] [{1}:{2}] Not a valid SHA1 password for account: '{3}' SHA1 Hash: '{4}'", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount, result.Rows(0).Item("sha_pass_hash"))
                        Console.ForegroundColor = ConsoleColor.White

                        Dim dataResponse(1) As Byte
                        dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                        dataResponse(1) = AccountState.LOGIN_BAD_PASS
                        client.Send(dataResponse, "RS_LOGON_CHALLENGE-FAIL-BADPWFORMAT")
                    Else 'Bail out with something meaningful

                        client.Access = result.Rows(0).Item("gmlevel")

                        Dim hash() As Byte = New Byte(19) {}
                        For i As Integer = 0 To 39 Step 2
                            hash(i \ 2) = CInt("&H" & result.Rows(0).Item("sha_pass_hash").Substring(i, 2))
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
                            client.Send(dataResponse, "RS_LOGON_CHALLENGE OK")
                        Catch ex As Exception
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.WriteLine("[{0}] [{1}:{2}] Error loading AuthEngine: {3}{4}", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, Environment.NewLine, ex)
                            Console.ForegroundColor = ConsoleColor.White
                        End Try
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

                Case AccountState.LOGIN_FAILED
                    Exit Select

                Case AccountState.LOGIN_BAD_PASS
                    Exit Select

                Case AccountState.LOGIN_DBBUSY
                    Exit Select

                Case AccountState.LOGIN_BADVERSION
                    Exit Select

                Case AccountState.LOGIN_DOWNLOADFILE
                    Exit Select

                Case AccountState.LOGIN_SUSPENDED
                    Exit Select

                Case AccountState.LOGIN_PARENTALCONTROL
                    Exit Select

                Case Else
                    Console.WriteLine("[{0}] [{1}:{2}] Account error [{3}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, packetAccount)
                    Dim dataResponse(1) As Byte
                    dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                    dataResponse(1) = AccountState.LOGIN_FAILED
                    client.Send(dataResponse, "RS_LOGON_CHALLENGE-FAILED")
                    Exit Sub
            End Select
        Else
            If Dir("Updates/wow-patch-" & Val("&H" & Hex(data(12)) & Hex(data(11))) & "-" & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & ".mpq") <> "" Then
                'Send UPDATE_MPQ
                Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_INITIATE [" & Chr(data(6)) & Chr(data(5)) & Chr(data(4)) & " " & data(8) & "." & data(9) & "." & data(10) & "." & Val("&H" & Hex(data(12)) & Hex(data(11))) & " " & Chr(data(15)) & Chr(data(14)) & Chr(data(13)) & " " & Chr(data(19)) & Chr(data(18)) & Chr(data(17)) & " " & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & "]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)

                client.UpdateFile = "Updates/wow-patch-" & Val("&H" & Hex(data(12)) & Hex(data(11))) & "-" & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & ".mpq"
                Dim dataResponse(30) As Byte

                dataResponse(0) = AuthCMD.CMD_XFER_INITIATE
                'Name Len 0x05 -> sizeof(Patch)
                Dim i As Integer = 1
                _Converter.ToBytes(CType(5, Byte), dataResponse, i)
                'Name 'Patch'
                _Converter.ToBytes("Patch", dataResponse, i)
                'Size 0x34 C4 0D 00 = 902,196 byte (180->181 enGB)
                _Converter.ToBytes(CType(FileLen(client.UpdateFile), Integer), dataResponse, i)
                'Unknown 0x0 always
                _Converter.ToBytes(0, dataResponse, i)
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
                Console.WriteLine("[{0}] [{1}:{2}] WRONG_VERSION [" & Chr(data(6)) & Chr(data(5)) & Chr(data(4)) & " " & data(8) & "." & data(9) & "." & data(10) & "." & Val("&H" & Hex(data(12)) & Hex(data(11))) & " " & Chr(data(15)) & Chr(data(14)) & Chr(data(13)) & " " & Chr(data(19)) & Chr(data(18)) & Chr(data(17)) & " " & Chr(data(24)) & Chr(data(23)) & Chr(data(22)) & Chr(data(21)) & "]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
                Dim dataResponse(1) As Byte
                dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
                dataResponse(1) = AccountState.LOGIN_BADVERSION
                client.Send(dataResponse, "RS_LOGON_CHALLENGE-WRONG-VERSION")
            End If
        End If
    End Sub

    Public Sub On_RS_LOGON_PROOF(ByRef data() As Byte, ByRef client As ClientClass)
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

        If Not passCheck Then
            'Wrong pass
            Console.WriteLine("[{0}] [{1}:{2}] Wrong password for user {3}.", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, client.Account)
            Dim dataResponse(1) As Byte
            dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
            dataResponse(1) = AccountState.LOGIN_BAD_PASS
            client.Send(dataResponse, "RS_LOGON_PROOF WRONGPASS")
        Else
            client.AuthEngine.CalculateM2(m1)

            Dim dataResponse(25) As Byte
            dataResponse(0) = AuthCMD.CMD_AUTH_LOGON_PROOF
            dataResponse(1) = AccountState.LOGIN_OK
            Array.Copy(client.AuthEngine.M2, 0, dataResponse, 2, 20)
            dataResponse(22) = 0
            dataResponse(23) = 0
            dataResponse(24) = 0
            dataResponse(25) = 0

            client.Send(dataResponse, "RS_LOGON_PROOF OK")

            'Set SSHash in DB
            Dim sshash As String = ""

            'For i as Integer = 0 To client.AuthEngine.SS_Hash.Length - 1
            For i As Integer = 0 To 40 - 1
                sshash = If(client.AuthEngine.SsHash(i) < 16, sshash + "0" + Hex(client.AuthEngine.SsHash(i)), sshash + Hex(client.AuthEngine.SsHash(i)))
            Next
            AccountDatabase.Update($"UPDATE account SET sessionkey = '{sshash }', last_ip = '{client.Ip}', last_login = '{Format(Now, "yyyy-MM-dd") }' WHERE username = '{client.Account }';")

            Console.WriteLine("[{0}] [{1}:{2}] Auth success for user {3}. [{4}]", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, client.Account, sshash)
        End If
    End Sub

    Public Sub On_RS_REALMLIST(ByRef data() As Byte, ByRef client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_REALM_LIST", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)

        Dim packetLen As Integer = 0
        Dim characterCount As Integer = 0
        Dim result As DataTable = Nothing
        Dim countresult As DataTable = Nothing

        ' Retrieve the Account ID
        AccountDatabase.Query($"SELECT id FROM account WHERE username = ""{client.Account }"";", result)

        'Fetch RealmList Data
        AccountDatabase.Query(String.Format("SELECT * FROM realmlist;"), result)

        For Each row As DataRow In result.Rows
            packetLen = packetLen + Len(row.Item("address")) + Len(row.Item("name")) + 1 + Len(Format(row.Item("port"), "0")) + 14
        Next

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
        dataResponse(8) = 0

        Dim tmp As Integer = 8
        For Each host As DataRow In result.Rows

            ' Get Number of Characters for the Realm
            AccountDatabase.Query($"SELECT * FROM realmcharacters WHERE realmid = ""{CInt(host.Item("id"))}"" AND acctid = ""{CInt(result.Rows(0).Item("id"))}"";", countresult)

            If (countresult.Rows.Count > 0) Then
                characterCount = countresult.Rows(0).Item("numchars")
            End If

            '(uint8) Realm Icon
            '	0 -> Normal; 1 -> PvP; 6 -> RP; 8 -> RPPvP;
            _Converter.ToBytes(CType(host.Item("icon"), Byte), dataResponse, tmp)
            '(uint8) IsLocked
            '	0 -> none; 1 -> locked
            _Converter.ToBytes(CType(0, Byte), dataResponse, tmp)
            '(uint8) unk
            _Converter.ToBytes(CType(0, Byte), dataResponse, tmp)
            '(uint8) unk
            _Converter.ToBytes(CType(0, Byte), dataResponse, tmp)
            '(uint8) Realm Color
            '   0 -> Green; 1 -> Red; 2 -> Offline;
            _Converter.ToBytes(CType(host.Item("realmflags"), Byte), dataResponse, tmp)
            '(string) Realm Name (zero terminated)
            _Converter.ToBytes(CType(host.Item("name"), String), dataResponse, tmp)
            _Converter.ToBytes(CType(0, Byte), dataResponse, tmp) '\0
            '(string) Realm Address ("ip:port", zero terminated)
            _Converter.ToBytes(host.Item("address") & ":" & host.Item("port"), dataResponse, tmp)
            _Converter.ToBytes(CType(0, Byte), dataResponse, tmp) '\0
            '(float) Population
            '   400F -> Full; 5F -> Medium; 1.6F -> Low; 200F -> New; 2F -> High
            '   00 00 48 43 -> Recommended
            '   00 00 C8 43 -> Full
            '   9C C4 C0 3F -> Low
            '   BC 74 B3 3F -> Low
            _Converter.ToBytes(CType(host.Item("population"), Single), dataResponse, tmp)
            '(byte) Number of character at this realm for this account
            _Converter.ToBytes(CType(characterCount, Byte), dataResponse, tmp)
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
            _Converter.ToBytes(CType(host.Item("timezone"), Byte), dataResponse, tmp)
            '(byte) Unknown (may be 2 -> TestRealm, / 6 -> ?)
            _Converter.ToBytes(CType(0, Byte), dataResponse, tmp)
        Next

        dataResponse(tmp) = 2 '2=list of realms 0=wizard
        dataResponse(tmp + 1) = 0

        client.Send(dataResponse, "RS-REALMLIST")
    End Sub

    Public Sub On_CMD_XFER_CANCEL(ByRef data() As Byte, ByRef client As ClientClass)
        'TODO: data parameter is never used
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_CANCEL", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
        client.Socket.Close()
    End Sub

    Public Sub On_CMD_XFER_ACCEPT(ByRef data() As Byte, ByRef client As ClientClass)
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

        If filelen <= 1500 Then
            tmp = 1
            Dim dataResponse(filelen + 2) As Byte
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            _Converter.ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, 0, dataResponse, 3, filelen)
            client.Send(dataResponse, "CMD-XFER-ACCEPT-3")
        Else
            Dim dataResponse() As Byte

            While filelen > 1500
                tmp = 1
                ReDim dataResponse(1500 + 2)
                dataResponse(0) = AuthCMD.CMD_XFER_DATA
                _Converter.ToBytes(CType(1500, Short), dataResponse, tmp)
                Array.Copy(buffer, fileOffset, dataResponse, 3, 1500)
                filelen -= 1500
                fileOffset += 1500
                client.Send(dataResponse, "CMD-XFER-ACCEPT-1")
            End While
            tmp = 1
            ReDim dataResponse(filelen + 2)
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            _Converter.ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, fileOffset, dataResponse, 3, filelen)
            client.Send(dataResponse, "CMD-XFER-ACCEPT-2")
        End If
        'Client.Socket.Close()
    End Sub

    Public Sub On_CMD_XFER_RESUME(ByRef data() As Byte, ByRef client As ClientClass)
        Console.WriteLine("[{0}] [{1}:{2}] CMD_XFER_RESUME", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port)
        Dim tmp As Integer = 1
        Dim buffer() As Byte
        Dim filelen As Integer
        filelen = FileSystem.FileLen(client.UpdateFile)
        Dim fileOffset As Integer
        fileOffset = _Converter.ToInt32(data, tmp)
        filelen -= fileOffset

        Dim fs As FileStream = New FileStream(client.UpdateFile, FileMode.Open, FileAccess.Read)
        Dim r As BinaryReader = New BinaryReader(fs)

        r.ReadBytes(fileOffset)
        buffer = r.ReadBytes(filelen)
        r.Close()
        '        fs.Close()
        fileOffset = 0

        If filelen <= 1500 Then
            tmp = 1
            Dim dataResponse(filelen + 2) As Byte
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            _Converter.ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, 0, dataResponse, 3, filelen)
            client.Send(dataResponse, "XFER-RESUME-XFER-DATA")
        Else
            Dim dataResponse() As Byte

            While filelen > 1500
                tmp = 1
                ReDim dataResponse(1500 + 2)
                dataResponse(0) = AuthCMD.CMD_XFER_DATA
                _Converter.ToBytes(CType(1500, Short), dataResponse, tmp)
                Array.Copy(buffer, fileOffset, dataResponse, 3, 1500)
                filelen -= 1500
                fileOffset += 1500
                client.Send(dataResponse, "XFER-RESUME")
            End While
            tmp = 1
            ReDim dataResponse(filelen + 2)
            dataResponse(0) = AuthCMD.CMD_XFER_DATA
            _Converter.ToBytes(CType(filelen, Short), dataResponse, tmp)
            Array.Copy(buffer, fileOffset, dataResponse, 3, filelen)
            client.Send(dataResponse, "XFER-RESUME-XFER-DATALARGER")
        End If
        'Client.Socket.Close()
    End Sub

    Public Sub DumpPacket(ByRef data() As Byte, ByRef client As ClientClass)
        Dim buffer As String = ""
        If client Is Nothing Then
            buffer += String.Format("[{0}] DEBUG: Packet Dump{1}", Format(TimeOfDay, "hh:mm:ss"), Environment.NewLine)
        Else
            buffer += String.Format("[{0}] [{1}:{2}] DEBUG: Packet Dump{3}", Format(TimeOfDay, "hh:mm:ss"), client.Ip, client.Port, Environment.NewLine)
        End If

        Dim j As Integer
        If data.Length Mod 16 = 0 Then
            For j = 0 To data.Length - 1 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & Environment.NewLine
            Next
        Else
            For j = 0 To data.Length - 1 - 16 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & Environment.NewLine
            Next

            buffer += "|  " & BitConverter.ToString(data, j, data.Length Mod 16).Replace("-", " ")
            buffer += New String(" ", (16 - data.Length Mod 16) * 3)
            buffer += " |  " & Encoding.ASCII.GetString(data, j, data.Length Mod 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?")
            buffer += New String(" ", 16 - data.Length Mod 16)
            buffer += " |" & Environment.NewLine
        End If

        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(buffer)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Private Sub WorldServer_Status_Report()
        Dim result1 As DataTable = New DataTable
        Dim returnValues As Integer
        returnValues = AccountDatabase.Query(String.Format("SELECT * FROM realmlist WHERE allowedSecurityLevel < '1';"), result1)
        If returnValues > SQL.ReturnState.Success Then 'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        Console.WriteLine()
        Console.WriteLine("[{0}] Loading known game servers...", Format(TimeOfDay, "hh:mm:ss"))

        Console.ForegroundColor = ConsoleColor.DarkGreen
        For Each row As DataRow In result1.Rows
            Console.WriteLine("     [{1}] at {0}:{2} - {3}", row.Item("address").PadRight(6), row.Item("name").PadRight(6), Format(row.Item("port")).PadRight(6), _Global_Constants.WorldServerStatus(Int(row.Item("realmflags"))).PadRight(6))
        Next
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Async Function StartAsync() As Task

        Console.BackgroundColor = ConsoleColor.Black
        Dim assemblyTitleAttribute As AssemblyTitleAttribute = CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute)
        Console.Title = $"{assemblyTitleAttribute.Title } v{[Assembly].GetExecutingAssembly().GetName().Version }"

        Console.ForegroundColor = ConsoleColor.Yellow
        Dim assemblyProductAttribute As AssemblyProductAttribute = CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyProductAttribute), False)(0), AssemblyProductAttribute)
        Console.WriteLine("{0}", assemblyProductAttribute.Product)

        Dim assemblyCopyrightAttribute As AssemblyCopyrightAttribute = CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)(0), AssemblyCopyrightAttribute)
        Console.WriteLine(assemblyCopyrightAttribute.Copyright)

        Console.WriteLine()

        Console.ForegroundColor = ConsoleColor.Yellow

        Console.WriteLine("  __  __      _  _  ___  ___  ___   __   __ ___               ")
        Console.WriteLine(" |  \/  |__ _| \| |/ __|/ _ \/ __|  \ \ / /| _ )      We Love ")
        Console.WriteLine(" | |\/| / _` | .` | (_ | (_) \__ \   \ V / | _ \   Vanilla Wow")
        Console.WriteLine(" |_|  |_\__,_|_|\_|\___|\___/|___/    \_/  |___/              ")
        Console.WriteLine("                                                              ")
        Console.WriteLine(" Website / Forum / Support: https://getmangos.eu/             ")
        Console.WriteLine("")

        Console.ForegroundColor = ConsoleColor.Magenta

        Console.ForegroundColor = ConsoleColor.White
        Dim attributeType As Type = GetType(AssemblyTitleAttribute)
        Console.Write([Assembly].GetExecutingAssembly().GetCustomAttributes(attributeType, False)(0).Title)

        Console.WriteLine(" version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Gray

        Console.WriteLine("[{0}] Realm Server Starting...", Format(TimeOfDay, "hh:mm:ss"))

        LoadConfig()

        Console.ForegroundColor = ConsoleColor.Yellow
        Log.WriteLine(LogType.INFORMATION, "Running from: {0}", AppDomain.CurrentDomain.BaseDirectory)
        Console.ForegroundColor = ConsoleColor.Gray

        AddHandler AccountDatabase.SQLMessage, AddressOf SqlEventHandler

        Dim ReturnValues As Integer
        ReturnValues = AccountDatabase.Connect()
        If ReturnValues > SQL.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        If _CommonGlobalFunctions.CheckRequiredDbVersion(AccountDatabase, ServerDb.Realm) = False Then 'Check the Database version, exit if its wrong

            If True Then
                Console.WriteLine("*************************")
                Console.WriteLine("* Press any key to exit *")
                Console.WriteLine("*************************")
                Console.ReadKey()
                End
            End If
        End If

        RealmServer = _RealmServerClassFactory.Create(Me)
        Await RealmServer.StartAsync()
        GC.Collect()

        WorldServer_Status_Report()
    End Function

    Public Function Ip2Int(ByVal ip As String) As UInteger
        If ip.Split(".").Length <> 4 Then Return 0

        Try
            Dim ipBytes(3) As Byte
            ipBytes(0) = ip.Split(".")(3)
            ipBytes(1) = ip.Split(".")(2)
            ipBytes(2) = ip.Split(".")(1)
            ipBytes(3) = ip.Split(".")(0)
            Return BitConverter.ToUInt32(ipBytes, 0)
        Catch
            Return 0
        End Try
    End Function

    Private Sub GenericExceptionHandler(sender As Object, e As UnhandledExceptionEventArgs)
        Dim ex As Exception = e.ExceptionObject

        Log.WriteLine(LogType.CRITICAL, ex.ToString & Environment.NewLine)
        Log.WriteLine(LogType.FAILED, "Unexpected error has occured. An 'RealmServer-Error-yyyy-mmm-d-h-mm.log' file has been created. Check your log folder for more information.")

        Dim tw As TextWriter
        tw = New StreamWriter(New FileStream(String.Format("RealmServer-Error-{0}.log", Format(Now, "yyyy-MMM-d-H-mm")), FileMode.Create))
        tw.Write(ex.ToString)
        tw.Close()
    End Sub
End Class