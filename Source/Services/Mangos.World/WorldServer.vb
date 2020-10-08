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

Imports System.Xml.Serialization
Imports System.IO
Imports System.Reflection
Imports System.Net
Imports System.Threading
Imports Mangos.Common
Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Globals
Imports Mangos.Common.Logging
Imports Mangos.Common.Logging.BaseWriter
Imports Mangos.SignalR
Imports Mangos.World.Globals
Imports Mangos.World.Maps
Imports Mangos.World.Objects
Imports Mangos.World.Player
Imports Mangos.World.Quests
Imports Mangos.World.Server

Public Class WorldServer
    Private server As ProxyServer(Of WS_Network.WorldServerClass)

#Region "Global.Variables"
    'Players' containers
    Public CLIENTs As New Dictionary(Of UInteger, WS_Network.ClientClass)
    Public CHARACTERs As New Dictionary(Of ULong, WS_PlayerData.CharacterObject)
    Public CHARACTERs_Lock As New ReaderWriterLock ' ReaderWriterLock_Debug("CHARACTERS")
    Public ALLQUESTS As New WS_Quests
    Public AllGraveYards As New WS_GraveYards

    Public CreatureQuestStarters As New Dictionary(Of Integer, List(Of Integer))
    Public CreatureQuestFinishers As New Dictionary(Of Integer, List(Of Integer))
    Public GameobjectQuestStarters As New Dictionary(Of Integer, List(Of Integer))
    Public GameobjectQuestFinishers As New Dictionary(Of Integer, List(Of Integer))

    'Worlds containers
    Public WORLD_CREATUREs_Lock As New ReaderWriterLock ' ReaderWriterLock_Debug("CREATURES")
    Public WORLD_CREATUREs As New Dictionary(Of ULong, WS_Creatures.CreatureObject)
    Public WORLD_CREATUREsKeys As New ArrayList()
    Public WORLD_GAMEOBJECTs As New Dictionary(Of ULong, WS_GameObjects.GameObjectObject)
    Public WORLD_CORPSEOBJECTs As New Dictionary(Of ULong, WS_Corpses.CorpseObject)
    Public WORLD_DYNAMICOBJECTs_Lock As New ReaderWriterLock
    Public WORLD_DYNAMICOBJECTs As New Dictionary(Of ULong, WS_DynamicObjects.DynamicObjectObject)
    Public WORLD_TRANSPORTs_Lock As New ReaderWriterLock
    Public WORLD_TRANSPORTs As New Dictionary(Of ULong, WS_Transports.TransportObject)
    Public WORLD_ITEMs As New Dictionary(Of ULong, ItemObject)

    'Database's containers - READONLY
    Public ITEMDatabase As New Dictionary(Of Integer, WS_Items.ItemInfo)
    Public CREATURESDatabase As New Dictionary(Of Integer, CreatureInfo)
    Public GAMEOBJECTSDatabase As New Dictionary(Of Integer, WS_GameObjects.GameObjectInfo)

    'Other
    Public itemGuidCounter As ULong = _Global_Constants.GUID_ITEM
    Public CreatureGUIDCounter As ULong = _Global_Constants.GUID_UNIT
    Public GameObjectsGUIDCounter As ULong = _Global_Constants.GUID_GAMEOBJECT
    Public CorpseGUIDCounter As ULong = _Global_Constants.GUID_CORPSE
    Public DynamicObjectsGUIDCounter As ULong = _Global_Constants.GUID_DYNAMICOBJECT
    Public TransportGUIDCounter As ULong = _Global_Constants.GUID_MO_TRANSPORT

    'System Things...
    Public Log As New BaseWriter
    Public PacketHandlers As New Dictionary(Of OPCODES, HandlePacket)
    Public Rnd As New Random
    Delegate Sub HandlePacket(ByRef Packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)

    'Scripting Support
    Public AreaTriggers As ScriptedObject
    Public AI As ScriptedObject
    'Public CharacterCreation As ScriptedObject

    Public ClsWorldServer As WS_Network.WorldServerClass

    Public Const SERVERSEED As Integer = &HDE133700

#End Region

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
            Dim configuration = _ConfigurationProvider.GetConfiguration()


            Console.WriteLine(".[done]")

            'DONE: Make sure VMap functionality is disabled with VMaps
            If Not configuration.VMapsEnabled Then
                configuration.LineOfSightEnabled = False
                configuration.HeightCalcEnabled = False
            End If

            'DONE: Setting SQL Connections
            Dim AccountDBSettings() As String = Split(configuration.AccountDatabase, ";")
            If AccountDBSettings.Length = 6 Then
                AccountDatabase.SQLDBName = AccountDBSettings(4)
                AccountDatabase.SQLHost = AccountDBSettings(2)
                AccountDatabase.SQLPort = AccountDBSettings(3)
                AccountDatabase.SQLUser = AccountDBSettings(0)
                AccountDatabase.SQLPass = AccountDBSettings(1)
                AccountDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), AccountDBSettings(5))
            Else
                Console.WriteLine("Invalid connect string for the account database!")
            End If

            Dim CharacterDBSettings() As String = Split(configuration.CharacterDatabase, ";")
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

            Dim WorldDBSettings() As String = Split(configuration.WorldDatabase, ";")
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

            _WS_Maps.RESOLUTION_ZMAP = configuration.MapResolution - 1
            If _WS_Maps.RESOLUTION_ZMAP < 63 Then _WS_Maps.RESOLUTION_ZMAP = 63
            If _WS_Maps.RESOLUTION_ZMAP > 255 Then _WS_Maps.RESOLUTION_ZMAP = 255

            'DONE: Creating logger
            Log = CreateLog(configuration.LogType, configuration.LogConfig)
            Log.LogLevel = configuration.LogLevel

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

    Public Sub CharacterSQLEventHandler(ByVal MessageID As SQL.EMessages, ByVal OutBuf As String)
        Select Case MessageID
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[CHARACTER] " & OutBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[CHARACTER] " & OutBuf)
        End Select
    End Sub

    Public Sub WorldSQLEventHandler(ByVal MessageID As SQL.EMessages, ByVal OutBuf As String)
        Select Case MessageID
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[WORLD] " & OutBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[WORLD] " & OutBuf)
        End Select
    End Sub
#End Region

    <MTAThread()>
    Sub Start()
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
        AddHandler AccountDatabase.SQLMessage, AddressOf AccountSQLEventHandler
        AddHandler CharacterDatabase.SQLMessage, AddressOf CharacterSQLEventHandler
        AddHandler WorldDatabase.SQLMessage, AddressOf WorldSQLEventHandler

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
        AccountDatabase.Update("SET NAMES 'utf8';")

        ReturnValues = CharacterDatabase.Connect()
        If ReturnValues > SQL.ReturnState.Success Then   'Ok, An error occurred
            Console.WriteLine("[{0}] An SQL Error has occurred", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If
        CharacterDatabase.Update("SET NAMES 'utf8';")

        ReturnValues = WorldDatabase.Connect()
        If ReturnValues > SQL.ReturnState.Success Then   'Ok, An error occurred
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
        If _CommonGlobalFunctions.CheckRequiredDbVersion(AccountDatabase, ServerDb.Realm) = False Then areDbVersionsOk = False
        If _CommonGlobalFunctions.CheckRequiredDbVersion(CharacterDatabase, ServerDb.Character) = False Then areDbVersionsOk = False
        If _CommonGlobalFunctions.CheckRequiredDbVersion(WorldDatabase, ServerDb.World) = False Then areDbVersionsOk = False

        If areDbVersionsOk = False Then
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        _WS_DBCDatabase.InitializeInternalDatabase()
        _WS_Handlers.IntializePacketHandlers()

#If WARDEN Then
        Maiev.InitWarden()
#End If

        ALLQUESTS.LoadAllQuests()

        AllGraveYards.InitializeGraveyards()

        _WS_Transports.LoadTransports()

        ClsWorldServer = New WS_Network.WorldServerClass
        Dim configuration = _ConfigurationProvider.GetConfiguration()
        Server = New ProxyServer(Of WS_Network.WorldServerClass)(Dns.GetHostAddresses(configuration.LocalConnectHost)(0), configuration.LocalConnectPort, ClsWorldServer)
        ClsWorldServer.ClusterConnect()
        Log.WriteLine(LogType.INFORMATION, "Interface UP at: {0}", ClsWorldServer.LocalURI)
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
            _WS_TimerBasedEvents.Regenerator.Dispose()
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

                            'Case "createaccount" 'ToDo: Fix create account command.
                            '    AccountDatabase.InsertSQL([String].Format("INSERT INTO accounts (account, password, email, joindate, last_ip) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", cmd(1), cmd(2), cmd(3), Format(Now, "yyyy-MM-dd"), "0.0.0.0"))
                            '    If AccountDatabase.QuerySQL("SELECT * FROM accounts WHERE account = "" & packet_account & "";") Then
                            '        Console.ForegroundColor = ConsoleColor.DarkGreen
                            '        Console.WriteLine("[Account: " & cmd(1) & " Password: " & cmd(2) & " Email: " & cmd(3) & "] has been created.")
                            '        Console.ForegroundColor = ConsoleColor.Gray
                            '    Else
                            '        Console.ForegroundColor = ConsoleColor.Red
                            '        Console.WriteLine("[Account: " & cmd(1) & " Password: " & cmd(2) & " Email: " & cmd(3) & "] could not be created.")
                            '        Console.ForegroundColor = ConsoleColor.Gray
                            '    End If

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
                                'Console.WriteLine("'create account <user> <password> <email>' - Creates an account with the specified username <user>, password <password>, and email <email>.")
                                'Console.WriteLine("")
                                Console.WriteLine("'info' - Brings up a context menu showing server information (such as memory used).")
                                Console.WriteLine("")
                                Console.WriteLine("'shutdown' - Shuts down 'WorldServer'.")
                            Case Else
                                Console.ForegroundColor = ConsoleColor.Red
                                Console.WriteLine("Error! Cannot find specified command. Please type 'help' for information on 'WorldServer' console commands.")
                                Console.ForegroundColor = ConsoleColor.Gray
                        End Select
                        '<<<<<<<<<<</END COMMAND STRUCTURE>>>>>>>>>>>>
                    End If
                Next
            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error executing command [{0}]. {2}{1}", Format(TimeOfDay, "hh:mm:ss"), tmp, e.ToString, Environment.NewLine)
            End Try
        End While
    End Sub

    Private Sub GenericExceptionHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim EX As Exception
        EX = e.ExceptionObject

        Log.WriteLine(LogType.CRITICAL, EX.ToString & Environment.NewLine)
        Log.WriteLine(LogType.FAILED, "Unexpected error has occured. An 'WorldServer-Error-yyyy-mmm-d-h-mm.log' file has been created. Check your log folder for more information.")

        Dim tw As TextWriter
        tw = New StreamWriter(New FileStream(String.Format("WorldServer-Error-{0}.log", Format(Now, "yyyy-MMM-d-H-mm")), FileMode.Create))
        tw.Write(EX.ToString)
        tw.Close()
    End Sub

End Class
