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

Imports System.Xml.Serialization
Imports System.IO
Imports System.Reflection

Imports mangosVB.Common.Globals
Imports mangosVB.Common.Logging
Imports mangosVB.Common.Logging.BaseWriter
Imports mangosVB.Common.Global_Constants

Imports mangosVB.Shared

Public Module WorldServer

#Region "Global.Variables"
    'Players' containers
    Public CLIENTs As New Dictionary(Of UInteger, ClientClass)
    Public CHARACTERs As New Dictionary(Of ULong, CharacterObject)
    Public CHARACTERs_Lock As New ReaderWriterLock ' ReaderWriterLock_Debug("CHARACTERS")
    Public ALLQUESTS As New WS_Quests
    Public AllGraveYards As New WS_GraveYards

    Public CreatureQuestStarters As New Dictionary(Of Integer, List(Of Integer))
    Public CreatureQuestFinishers As New Dictionary(Of Integer, List(Of Integer))
    Public GameobjectQuestStarters As New Dictionary(Of Integer, List(Of Integer))
    Public GameobjectQuestFinishers As New Dictionary(Of Integer, List(Of Integer))

    'Worlds containers
    Public WORLD_CREATUREs_Lock As New ReaderWriterLock ' ReaderWriterLock_Debug("CREATURES")
    Public WORLD_CREATUREs As New Dictionary(Of ULong, CreatureObject)
    Public WORLD_CREATUREsKeys As New ArrayList()
    Public WORLD_GAMEOBJECTs As New Dictionary(Of ULong, GameObjectObject)
    Public WORLD_CORPSEOBJECTs As New Dictionary(Of ULong, CorpseObject)
    Public WORLD_DYNAMICOBJECTs_Lock As New ReaderWriterLock
    Public WORLD_DYNAMICOBJECTs As New Dictionary(Of ULong, DynamicObjectObject)
    Public WORLD_TRANSPORTs_Lock As New ReaderWriterLock
    Public WORLD_TRANSPORTs As New Dictionary(Of ULong, TransportObject)
    Public WORLD_ITEMs As New Dictionary(Of ULong, ItemObject)

    'Database's containers - READONLY
    Public ITEMDatabase As New Dictionary(Of Integer, ItemInfo)
    Public CREATURESDatabase As New Dictionary(Of Integer, CreatureInfo)
    Public GAMEOBJECTSDatabase As New Dictionary(Of Integer, GameObjectInfo)

    'Other
    Public itemGuidCounter As ULong = GUID_ITEM
    Public CreatureGUIDCounter As ULong = GUID_UNIT
    Public GameObjectsGUIDCounter As ULong = GUID_GAMEOBJECT
    Public CorpseGUIDCounter As ULong = GUID_CORPSE
    Public DynamicObjectsGUIDCounter As ULong = GUID_DYNAMICOBJECT
    Public TransportGUIDCounter As ULong = GUID_MO_TRANSPORT

    'System Things...
    Public Log As New BaseWriter
    Public PacketHandlers As New Dictionary(Of OPCODES, HandlePacket)
    Public Rnd As New Random
    Delegate Sub HandlePacket(ByRef Packet As PacketClass, ByRef client As ClientClass)

    'Scripting Support
    Public AreaTriggers As ScriptedObject
    Public AI As ScriptedObject
    'Public CharacterCreation As ScriptedObject

    Public ClsWorldServer As WorldServerClass

    Public Const SERVERSEED As Integer = &HDE133700

#End Region

    Public Config As XMLConfigFile
    <XmlRoot(ElementName:="WorldServer")>
    Public Class XMLConfigFile
        'Database Settings
        <XmlElement(ElementName:="AccountDatabase")> Public AccountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
        <XmlElement(ElementName:="CharacterDatabase")> Public CharacterDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
        <XmlElement(ElementName:="WorldDatabase")> Public WorldDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

        'Server Settings
        <XmlElement(ElementName:="ServerPlayerLimit")> Public ServerPlayerLimit As Integer = 10
        <XmlElement(ElementName:="CommandCharacter")> Public CommandCharacter As String = "."
        <XmlElement(ElementName:="XPRate")> Public XPRate As Single = 1.0
        <XmlElement(ElementName:="ManaRegenerationRate")> Public ManaRegenerationRate As Single = 1.0
        <XmlElement(ElementName:="HealthRegenerationRate")> Public HealthRegenerationRate As Single = 1.0
        <XmlElement(ElementName:="GlobalAuction")> Public GlobalAuction As Boolean = False
        <XmlElement(ElementName:="SaveTimer")> Public SaveTimer As Integer = 120000
        <XmlElement(ElementName:="WeatherTimer")> Public WeatherTimer As Integer = 600000
        <XmlElement(ElementName:="MapResolution")> Public MapResolution As Integer = 64
        <XmlArray(ElementName:="HandledMaps"), XmlArrayItem(GetType(String), ElementName:="Map")> Public Maps As New ArrayList

        'VMap Settings
        <XmlElement(ElementName:="VMaps")> Public VMapsEnabled As Boolean = False
        <XmlElement(ElementName:="VMapLineOfSightCalc")> Public LineOfSightEnabled As Boolean = False
        <XmlElement(ElementName:="VMapHeightCalc")> Public HeightCalcEnabled As Boolean = False

        'Logging Settings
        <XmlElement(ElementName:="LogType")> Public LogType As String = "FILE"
        <XmlElement(ElementName:="LogLevel")> Public LogLevel As LogType = GlobalEnum.LogType.NETWORK
        <XmlElement(ElementName:="LogConfig")> Public LogConfig As String = ""

        'Other Settings
        <XmlArray(ElementName:="ScriptsCompiler"), XmlArrayItem(GetType(String), ElementName:="Include")> Public CompilerInclude As New ArrayList
        <XmlElement(ElementName:="CreatePartyInstances")> Public CreatePartyInstances As Boolean = False
        <XmlElement(ElementName:="CreateRaidInstances")> Public CreateRaidInstances As Boolean = False
        <XmlElement(ElementName:="CreateBattlegrounds")> Public CreateBattlegrounds As Boolean = False
        <XmlElement(ElementName:="CreateArenas")> Public CreateArenas As Boolean = False
        <XmlElement(ElementName:="CreateOther")> Public CreateOther As Boolean = False

        'Cluster Settings
        <XmlElement(ElementName:="ClusterPassword")> Public ClusterPassword As String = ""
        <XmlElement(ElementName:="ClusterConnectMethod")> Public ClusterConnectMethod As String = "tcp"
        <XmlElement(ElementName:="ClusterConnectHost")> Public ClusterConnectHost As String = "127.0.0.1"
        <XmlElement(ElementName:="ClusterConnectPort")> Public ClusterConnectPort As Integer = 50001
        <XmlElement(ElementName:="LocalConnectHost")> Public LocalConnectHost As String = "127.0.0.1"
        <XmlElement(ElementName:="LocalConnectPort")> Public LocalConnectPort As Integer = 50002
    End Class

    Public Sub LoadConfig()
        Try
            Dim FileName As String = "configs/WorldServer.ini"

            'Get filename from console arguments
            Dim args As String() = Environment.GetCommandLineArgs()
            For Each arg As String In args
                If arg.IndexOf("config") <> -1 Then
                    FileName = Trim(arg.Substring(arg.IndexOf("=") + 1))
                End If
            Next
            'Make sure a config file exists
            If File.Exists(FileName) = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Cannot Continue. {1} does not exist.", Format(TimeOfDay, "hh:mm:ss"), FileName)
                Console.WriteLine("Please copy the ini files into the same directory as the Server exe files.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If
            'Load config
            Console.Write("[{0}] Loading Configuration from {1}...", Format(TimeOfDay, "hh:mm:ss"), FileName)

            Config = New XMLConfigFile
            Console.Write("...")

            Dim oXS As XmlSerializer = New XmlSerializer(GetType(XMLConfigFile))

            Console.Write("...")
            Dim oStmR As StreamReader
            oStmR = New StreamReader(FileName)
            Config = oXS.Deserialize(oStmR)
            oStmR.Close()

            Console.WriteLine(".[done]")

            'DONE: Make sure VMap functionality is disabled with VMaps
            If Not Config.VMapsEnabled Then
                Config.LineOfSightEnabled = False
                Config.HeightCalcEnabled = False
            End If

            'DONE: Setting SQL Connections
            Dim AccountDBSettings() As String = Split(Config.AccountDatabase, ";")
            If AccountDBSettings.Length = 6 Then
                AccountDatabase.SQLDBName = AccountDBSettings(4)
                AccountDatabase.SQLHost = AccountDBSettings(2)
                AccountDatabase.SQLPort = AccountDBSettings(3)
                AccountDatabase.SQLUser = AccountDBSettings(0)
                AccountDatabase.SqlPass = AccountDBSettings(1)
                AccountDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), AccountDBSettings(5))
            Else
                Console.WriteLine("Invalid connect string for the account database!")
            End If

            Dim CharacterDBSettings() As String = Split(Config.CharacterDatabase, ";")
            If CharacterDBSettings.Length = 6 Then
                CharacterDatabase.SQLDBName = CharacterDBSettings(4)
                CharacterDatabase.SQLHost = CharacterDBSettings(2)
                CharacterDatabase.SQLPort = CharacterDBSettings(3)
                CharacterDatabase.SQLUser = CharacterDBSettings(0)
                CharacterDatabase.SQLPass = CharacterDBSettings(1)
                CharacterDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), CharacterDBSettings(5))
            Else
                Console.WriteLine("Invalid connect string for the character database!")
            End If

            Dim WorldDBSettings() As String = Split(Config.WorldDatabase, ";")
            If WorldDBSettings.Length = 6 Then
                WorldDatabase.SQLDBName = WorldDBSettings(4)
                WorldDatabase.SQLHost = WorldDBSettings(2)
                WorldDatabase.SQLPort = WorldDBSettings(3)
                WorldDatabase.SQLUser = WorldDBSettings(0)
                WorldDatabase.SQLPass = WorldDBSettings(1)
                WorldDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), WorldDBSettings(5))
            Else
                Console.WriteLine("Invalid connect string for the world database!")
            End If

            RESOLUTION_ZMAP = Config.MapResolution - 1
            If RESOLUTION_ZMAP < 63 Then RESOLUTION_ZMAP = 63
            If RESOLUTION_ZMAP > 255 Then RESOLUTION_ZMAP = 255

            'DONE: Creating logger
            CreateLog(Config.LogType, Config.LogConfig, Log)
            Log.LogLevel = Config.LogLevel

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub

#Region "WS.DataAccess"
    Public AccountDatabase As New SQL
    Public CharacterDatabase As New SQL
    Public WorldDatabase As New SQL
    Public Sub AccountSQLEventHandler(ByVal MessageID As SQL.EMessages, ByVal OutBuf As String)
        Select Case MessageID
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[ACCOUNT] " & OutBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[ACCOUNT] " & OutBuf)
        End Select
    End Sub

    Public Sub CharacterSQLEventHandler(ByVal MessageID As Sql.EMessages, ByVal OutBuf As String)
        Select Case MessageID
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[CHARACTER] " & OutBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[CHARACTER] " & OutBuf)
        End Select
    End Sub

    Public Sub WorldSQLEventHandler(ByVal MessageID As Sql.EMessages, ByVal OutBuf As String)
        Select Case MessageID
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[WORLD] " & OutBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[WORLD] " & OutBuf)
        End Select
    End Sub
#End Region

    <MTAThread()>
    Sub Main()
        TimeBeginPeriod(1, "")  'Set timeGetTime("") to a accuracy of 1ms

        Console.BackgroundColor = ConsoleColor.Black
        Console.Title = String.Format("{0} v{1}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title, [Assembly].GetExecutingAssembly().GetName().Version)

        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine("{0}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyProductAttribute), False)(0), AssemblyProductAttribute).Product)
        Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)(0), AssemblyCopyrightAttribute).Copyright)
        Console.WriteLine()

        Console.ForegroundColor = ConsoleColor.Yellow

        Console.WriteLine("  __  __      _  _  ___  ___  ___   __   __ ___               ")
        Console.WriteLine(" |  \/  |__ _| \| |/ __|/ _ \/ __|  \ \ / /| _ )      We Love ")
        Console.WriteLine(" | |\/| / _` | .` | (_ | (_) \__ \   \ V / | _ \   Vanilla Wow")
        Console.WriteLine(" |_|  |_\__,_|_|\_|\___|\___/|___/    \_/  |___/              ")
        Console.WriteLine("                                                              ")
        Console.WriteLine(" Website / Forum / Support: https://getmangos.eu/          ")
        Console.WriteLine("")

        If DoesSharedDllExist() = False Then
            End
        End If

        Console.ForegroundColor = ConsoleColor.Magenta

        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title)
        Console.WriteLine(" version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.ForegroundColor = ConsoleColor.White

        Console.WriteLine("")
        Console.ForegroundColor = ConsoleColor.Gray

        Dim dateTimeStarted As Date = Now
        Log.WriteLine(LogType.INFORMATION, "[{0}] World Server Starting...", Format(TimeOfDay, "hh:mm:ss"))

        Dim currentDomain As AppDomain = AppDomain.CurrentDomain
        AddHandler currentDomain.UnhandledException, AddressOf GenericExceptionHandler

        LoadConfig()

        Console.ForegroundColor = ConsoleColor.Gray
        AddHandler AccountDatabase.SqlMessage, AddressOf AccountSQLEventHandler
        AddHandler CharacterDatabase.SqlMessage, AddressOf CharacterSQLEventHandler
        AddHandler WorldDatabase.SqlMessage, AddressOf WorldSQLEventHandler

        Dim ReturnValues As Integer
        ReturnValues = AccountDatabase.Connect()
        If ReturnValues > Sql.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If
        AccountDatabase.Update("SET NAMES 'utf8';")

        ReturnValues = CharacterDatabase.Connect()
        If ReturnValues > Sql.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If
        CharacterDatabase.Update("SET NAMES 'utf8';")

        ReturnValues = WorldDatabase.Connect()
        If ReturnValues > Sql.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If
        WorldDatabase.Update("SET NAMES 'utf8';")

        'Check the Database version, exit if its wrong
        Dim areDbVersionsOk As Boolean = True
        If CheckRequiredDbVersion(AccountDatabase, ServerDb.Realm) = False Then areDbVersionsOk = False
        If CheckRequiredDbVersion(CharacterDatabase, ServerDb.Character) = False Then areDbVersionsOk = False
        If CheckRequiredDbVersion(WorldDatabase, ServerDb.World) = False Then areDbVersionsOk = False

        If areDbVersionsOk = False Then
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        InitializeInternalDatabase()
        IntializePacketHandlers()

#If WARDEN Then
        Maiev.InitWarden()
#End If

        'Log.WriteLine(LogType.WARNING, "Loading Quests...")
        ALLQUESTS.LoadAllQuests()
        'Log.WriteLine(LogType.WARNING, "Loading Quests...Complete")

        AllGraveYards.InitializeGraveyards()

        LoadTransports()

        ClsWorldServer = New WorldServerClass
        GC.Collect()

        If Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High Then
            Log.WriteLine(LogType.WARNING, "Setting Process Priority to HIGH..[done]")
        Else
            Log.WriteLine(LogType.WARNING, "Setting Process Priority to NORMAL..[done]")
        End If

        Log.WriteLine(LogType.INFORMATION, " Load Time:   {0}", Format(DateDiff(DateInterval.Second, dateTimeStarted, Now), "0 seconds"))
        Log.WriteLine(LogType.INFORMATION, " Used Memory: {0}", Format(GC.GetTotalMemory(False), "### ### ##0 bytes"))

        WaitConsoleCommand()

        Try
        Catch ex As Exception
            Regenerator.Dispose()
            AreaTriggers.Dispose()
        End Try
    End Sub

    Public Sub WaitConsoleCommand()
        Dim tmp As String = "", CommandList() As String, cmds() As String
        Dim cmd() As String = {}
        Dim varList As Integer
        While Not ClsWorldServer._flagStopListen
            Try
                tmp = Log.ReadLine()
                CommandList = tmp.Split(";")

                For varList = LBound(CommandList) To UBound(CommandList)
                    cmds = Split(CommandList(varList), " ", 2)
                    If CommandList(varList).Length > 0 Then
                        '<<<<<<<<<<<COMMAND STRUCTURE>>>>>>>>>>
                        Select Case cmds(0).ToLower
                            Case "shutdown"
                                Log.WriteLine(LogType.WARNING, "Server shutting down...")
                                ClsWorldServer._flagStopListen = True

                            Case "debug"
                                Dim conn As New ClientClass With {
                                    .DEBUG_CONNECTION = True
                                }

                                Dim test As New CharacterObject(conn, CType(cmds(1), Long))
                                CHARACTERs(CType(cmds(1), Long)) = test
                                AddToWorld(test)
                                Log.WriteLine(LogType.DEBUG, "Spawned character " & test.Name)

                            Case "createaccount" 'ToDo: Fix create account command.
                                AccountDatabase.InsertSQL([String].Format("INSERT INTO accounts (account, password, email, joindate, last_ip) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", cmd(1), cmd(2), cmd(3), Format(Now, "yyyy-MM-dd"), "0.0.0.0"))
                                If AccountDatabase.QuerySQL("SELECT * FROM accounts WHERE account = "" & packet_account & "";") Then
                                    Console.ForegroundColor = ConsoleColor.DarkGreen
                                    Console.WriteLine("[Account: " & cmd(1) & " Password: " & cmd(2) & " Email: " & cmd(3) & "] has been created.")
                                    Console.ForegroundColor = ConsoleColor.Gray
                                Else
                                    Console.ForegroundColor = ConsoleColor.DarkRed
                                    Console.WriteLine("[Account: " & cmd(1) & " Password: " & cmd(2) & " Email: " & cmd(3) & "] could not be created.")
                                    Console.ForegroundColor = ConsoleColor.Gray
                                End If

                            Case "gccollect"
                                GC.Collect()

                            Case "ban account"
                                Console.ForegroundColor = ConsoleColor.DarkYellow
                                Console.WriteLine("[{0}] Specify the Account Name :", Format(TimeOfDay, "hh:mm:ss"))

                                Dim AccountName As String
                                AccountName = Console.ReadLine

                                Dim result As New DataTable
                                Dim result1 As New DataTable
                                AccountDatabase.Query("SELECT banned FROM accounts WHERE account = """ & AccountName & """;", result)
                                AccountDatabase.Query("SELECT last_ip FROM accounts WHERE account = """ & AccountName & """;", result1)

                                Dim IP As String
                                IP = result1.Rows(0).Item("last_ip")
                                If result.Rows.Count > 0 Then
                                    If result.Rows(0).Item("banned") = 1 Then
                                        Console.ForegroundColor = ConsoleColor.Green
                                        Console.WriteLine(String.Format("[{1}] Account [{0}] is already banned.", AccountName, Format(TimeOfDay, "hh:mm:ss")))

                                        Console.ForegroundColor = ConsoleColor.Yellow

                                    ElseIf IP = "127.0.0.1" Then
                                        Console.WriteLine("[{1}] Account [{0}] has the same IP Adress as the host.", AccountName, Format(TimeOfDay, "hh:mm:ss"))
                                    ElseIf IP = "0.0.0.0" Then
                                        Console.WriteLine("[{1}] Account [{0}] does not have an IP Address.", AccountName, Format(TimeOfDay, "hh:mm:ss"))
                                    Else
                                        AccountDatabase.Query("SELECT last_ip FROM accounts WHERE account = """ & AccountName & """;", result)

                                        Dim IpAddress As String
                                        IpAddress = result.Rows(0).Item("last_ip")
                                        AccountDatabase.Update("UPDATE accounts SET banned = 1 WHERE account = """ & AccountName & """;")
                                        Console.WriteLine(String.Format("[{1}] IP Address [{0}] is now banned.", IP, Format(TimeOfDay, "hh:mm:ss")))
                                        AccountDatabase.Update(String.Format("INSERT INTO `bans` VALUES ('{0}', '{1}', '{2}', '{3}');", IpAddress, Format(Now, "yyyy-MM-dd"), "No Reason Specified.", AccountName))
                                        Console.WriteLine(String.Format("[{1}] Account [{0}] is now banned.", AccountName, Format(TimeOfDay, "hh:mm:ss")))
                                    End If
                                Else
                                    Console.ForegroundColor = ConsoleColor.DarkGray
                                    Console.WriteLine(String.Format("[{1}] Account [{0}] is not found.", AccountName, Format(TimeOfDay, "hh:mm:ss")))
                                End If

                            Case "unban account"
                                Console.ForegroundColor = ConsoleColor.DarkCyan
                                Console.WriteLine("[{0}] Account Name?", Format(TimeOfDay, "hh:mm:ss"))
                                Dim AccountName As String
                                Console.ForegroundColor = ConsoleColor.Magenta
                                AccountName = Console.ReadLine
                                Dim result As New DataTable
                                AccountDatabase.Query("SELECT banned FROM accounts WHERE account = """ & AccountName & """;", result)

                                If result.Rows.Count > 0 Then
                                    If result.Rows(0).Item("banned") = 0 Then
                                        Console.ForegroundColor = ConsoleColor.Green
                                        Console.WriteLine(String.Format("[{1}] Account [{0}] is not banned.", AccountName, Format(TimeOfDay, "hh:mm:ss")))
                                    Else
                                        Console.ForegroundColor = ConsoleColor.Red
                                        AccountDatabase.Update("UPDATE accounts SET banned = 0 WHERE account = """ & AccountName & """;")
                                        AccountDatabase.Query("SELECT last_ip FROM accounts WHERE account = """ & AccountName & """;", result)
                                        Dim IP As String
                                        IP = result.Rows(0).Item("last_ip")
                                        AccountDatabase.Update([String].Format("DELETE FROM `bans` WHERE `ip` = '{0}';", IP))
                                        Console.WriteLine(String.Format("[{1}] Account [{0}] has been unbanned.", AccountName, Format(TimeOfDay, "hh:mm:ss")))

                                    End If
                                Else
                                    Console.WriteLine(String.Format("[{1}] Account [{0}] is not found.", AccountName, Format(TimeOfDay, "hh:mm:ss")))
                                End If

                            Case "info"
                                Log.WriteLine(LogType.INFORMATION, "Used memory: {0}", Format(GC.GetTotalMemory(False), "### ### ##0 bytes"))

                            Case "help"
                                Console.ForegroundColor = ConsoleColor.Blue
                                Console.WriteLine("'WorldServer' Command list:")
                                Console.ForegroundColor = ConsoleColor.White
                                Console.WriteLine("---------------------------------")
                                Console.WriteLine("")
                                Console.WriteLine("")
                                Console.WriteLine("'help' - Brings up the 'WorldServer' Command list (this).")
                                Console.WriteLine("")
                                Console.WriteLine("'create account <user> <password> <email>' - Creates an account with the specified username <user>, password <password>, and email <email>.")
                                Console.WriteLine("")
                                Console.WriteLine("'debug' - Creates a 'test' character.")
                                Console.WriteLine("")
                                Console.WriteLine("'info' - Brings up a context menu showing server information (such as memory used).")
                                Console.WriteLine("")
                                Console.WriteLine("'shutdown' - Shuts down 'WorldServer'.")
                                Console.WriteLine("")
                                Console.WriteLine("'ban account'- Adds a Ban and IP Ban on an account.")
                                Console.WriteLine("")
                                Console.WriteLine("'unban account'- Removes a Ban and IP Ban on an account.")
                            Case Else
                                Console.ForegroundColor = ConsoleColor.DarkRed
                                Console.WriteLine("Error! Cannot find specified command. Please type 'help' for information on 'WorldServer' console commands.")
                                Console.ForegroundColor = ConsoleColor.Gray
                        End Select
                        '<<<<<<<<<<</END COMMAND STRUCTURE>>>>>>>>>>>>
                    End If
                Next
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error executing command [{0}]. {2}{1}", Format(TimeOfDay, "hh:mm:ss"), tmp, e.ToString, vbNewLine)
            End Try
        End While
    End Sub

    Private Sub GenericExceptionHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim EX As Exception
        EX = e.ExceptionObject

        Log.WriteLine(LogType.CRITICAL, EX.ToString & vbNewLine)
        Log.WriteLine(LogType.FAILED, "Unexpected error has occured. An 'WorldServer-Error-yyyy-mmm-d-h-mm.log' file has been created. Check your log folder for more information.")

        Dim tw As TextWriter
        tw = New StreamWriter(New FileStream(String.Format("WorldServer-Error-{0}.log", Format(Now, "yyyy-MMM-d-H-mm")), FileMode.Create))
        tw.Write(EX.ToString)
        tw.Close()
    End Sub

End Module
