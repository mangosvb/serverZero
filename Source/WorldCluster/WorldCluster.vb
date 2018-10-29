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

Imports System.Threading
Imports System.Xml.Serialization
Imports System.IO
Imports System.Reflection

Imports mangosVB.Common
Imports mangosVB.Common.Globals
Imports mangosVB.Common.Logging
Imports mangosVB.Common.Logging.BaseWriter
Imports mangosVB.Shared

Imports WorldCluster.Globals
Imports WorldCluster.Handlers
Imports WorldCluster.DataStores
Imports WorldCluster.Server

Public Module WorldCluster
    'Players' containers
    Public CLIETNIDs As Long = 0

    Public CLIENTs As New Dictionary(Of UInteger, ClientClass)

    Public CHARACTERs_Lock As New ReaderWriterLock
    Public CHARACTERs As New Dictionary(Of ULong, CharacterObject)
    'Public CHARACTER_NAMEs As New Hashtable

    'System Things...
    Public Log As New BaseWriter
    Public PacketHandlers As New Dictionary(Of OPCODES, HandlePacket)
    Public Rnd As New Random
    Delegate Sub HandlePacket(ByRef packet As PacketClass, ByRef client As ClientClass)

    Public _config As XMLConfigFile

    <XmlRoot(ElementName:="WorldCluster")>
    Public Class XMLConfigFile
        <XmlElement(ElementName:="WorldClusterPort")> Public WorldClusterPort As Integer = 8085
        <XmlElement(ElementName:="WorldClusterAddress")> Public WorldClusterAddress As String = "127.0.0.1"
        <XmlElement(ElementName:="ServerPlayerLimit")> Public ServerPlayerLimit As Integer = 10

        'Database Settings
        <XmlElement(ElementName:="AccountDatabase")> Public AccountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
        <XmlElement(ElementName:="CharacterDatabase")> Public CharacterDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
        <XmlElement(ElementName:="WorldDatabase")> Public WorldDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

        'Cluster Settings
        <XmlElement(ElementName:="ClusterPassword")> Public ClusterPassword As String = ""
        <XmlElement(ElementName:="ClusterListenMethod")> Public ClusterListenMethod As String = "tcp"
        <XmlElement(ElementName:="ClusterListenAddress")> Public ClusterListenAddress As String = "127.0.0.1"
        <XmlElement(ElementName:="ClusterListenPort")> Public ClusterListenPort As Integer = 50001
        <XmlArray(ElementName:="ClusterFirewall"), XmlArrayItem(GetType(String), ElementName:="IP")> Public Firewall As New ArrayList

        'Stats Settings
        <XmlElement(ElementName:="StatsEnabled")> Public StatsEnabled As Boolean = True
        <XmlElement(ElementName:="StatsTimer")> Public StatsTimer As Integer = 120000
        <XmlElement(ElementName:="StatsLocation")> Public StatsLocation As String = "stats.xml"

        'Logging Settings
        <XmlElement(ElementName:="LogType")> Public LogType As String = "FILE"
        <XmlElement(ElementName:="LogLevel")> Public LogLevel As LogType = GlobalEnum.LogType.NETWORK
        <XmlElement(ElementName:="LogConfig")> Public LogConfig As String = ""
        <XmlElement(ElementName:="PacketLogging")> Public PacketLogging As Boolean = False
        <XmlElement(ElementName:="GMLogging")> Public GMLogging As Boolean = False
    End Class

    Public Sub LoadConfig()
        Try
            'Make sure WorldCluster.ini exists
            If File.Exists("configs/WorldCluster.ini") = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Cannot Continue. {1} does not exist.", Format(TimeOfDay, "hh:mm:ss"), "configs/WorldCluster.ini")
                Console.WriteLine("Please copy the ini files into the same directory as the mangosVB exe files.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If

            Console.Write("[{0}] Loading Configuration...", Format(TimeOfDay, "hh:mm:ss"))

            _config = New XMLConfigFile
            Console.Write("...")

            Dim oXS As XmlSerializer = New XmlSerializer(GetType(XMLConfigFile))
            Console.Write("...")

            Dim ostream As StreamReader
            ostream = New StreamReader("configs/WorldCluster.ini")
            _config = oXS.Deserialize(ostream)
            ostream.Close()

            Console.WriteLine(".[done]")

            'DONE: Setting SQL Connections
            Dim AccountDBSettings() As String = Split(_config.AccountDatabase, ";")
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

            Dim CharacterDBSettings() As String = Split(_config.CharacterDatabase, ";")
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

            Dim WorldDBSettings() As String = Split(_config.WorldDatabase, ";")
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

            'DONE: Creating logger
            CreateLog(_config.LogType, _config.LogConfig, Log)
            Log.LogLevel = _config.LogLevel

            'DONE: Cleaning up the packet log
            If _config.PacketLogging Then
                File.Delete("packets.log")
            End If

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub

#Region "WS.DataAccess"
    Public AccountDatabase As New SQL
    Public CharacterDatabase As New SQL
    Public WorldDatabase As New SQL

    Public Sub AccountSQLEventHandler(messageId As SQL.EMessages, outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[ACCOUNT] " & outBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[ACCOUNT] " & outBuf)
        End Select
    End Sub

    Public Sub CharacterSQLEventHandler(messageId As SQL.EMessages, outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[CHARACTER] " & outBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[CHARACTER] " & outBuf)
        End Select
    End Sub

    Public Sub WorldSQLEventHandler(messageId As SQL.EMessages, outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[WORLD] " & outBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[WORLD] " & outBuf)
        End Select
    End Sub
#End Region

    <MTAThread()>
    Sub Main()
        timeBeginPeriod(1, "")  'Set timeGetTime("") to a accuracy of 1ms

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
        Console.WriteLine(" Website / Forum / Support: https://getmangos.eu/             ")
        Console.WriteLine("")

        If DoesSharedDllExist() = False Then
            End
        End If

        Console.ForegroundColor = ConsoleColor.Magenta

        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title)
        Console.WriteLine("version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.ForegroundColor = ConsoleColor.White

        Console.WriteLine("")
        Console.ForegroundColor = ConsoleColor.Gray

        Dim dateTimeStarted As Date = Now
        Log.WriteLine(LogType.INFORMATION, "[{0}] World Cluster Starting...", Format(TimeOfDay, "hh:mm:ss"))

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

        InitializeInternalDatabase()
        IntializePacketHandlers()

        Dim areDbVersionsOk As Boolean = True
        If CheckRequiredDbVersion(AccountDatabase, ServerDb.Realm) = False Then areDbVersionsOk = False
        If CheckRequiredDbVersion(WorldDatabase, ServerDb.World) = False Then areDbVersionsOk = False
        If CheckRequiredDbVersion(CharacterDatabase, ServerDb.Character) = False Then areDbVersionsOk = False

        If areDbVersionsOk = False Then
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            End
        End If

        WorldServer = New WorldServerClass
        GC.Collect()

        If Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High Then
            Log.WriteLine(LogType.WARNING, "Setting Process Priority to HIGH..[done]")
        Else
            Log.WriteLine(LogType.WARNING, "Setting Process Priority to NORMAL..[done]")
        End If

        Log.WriteLine(LogType.INFORMATION, "Load Time: {0}", Format(DateDiff(DateInterval.Second, dateTimeStarted, Now), "0 seconds"))
        Log.WriteLine(LogType.INFORMATION, "Used memory: {0}", Format(GC.GetTotalMemory(False), "### ### ##0 bytes"))

        WaitConsoleCommand()
    End Sub

    Public Sub WaitConsoleCommand()
        Dim tmp As String = "", CommandList() As String, cmds() As String
        Dim cmd() As String = {}
        Dim varList As Integer
        While Not WorldServer.m_flagStopListen
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
                                WorldServer.m_flagStopListen = True

                            Case "info"
                                Log.WriteLine(LogType.INFORMATION, "Used memory: {0}", Format(GC.GetTotalMemory(False), "### ### ##0 bytes"))

                            Case "help"
                                Console.ForegroundColor = ConsoleColor.Blue
                                Console.WriteLine("'WorldCluster' Command list:")
                                Console.ForegroundColor = ConsoleColor.White
                                Console.WriteLine("---------------------------------")
                                Console.WriteLine("")
                                Console.WriteLine("'help' - Brings up the 'WorldCluster' Command list (this).")
                                Console.WriteLine("")
                                Console.WriteLine("'info' - Displays used memory.")
                                Console.WriteLine("")
                                Console.WriteLine("'shutdown' - Shuts down WorldCluster.")
                            Case Else
                                Console.ForegroundColor = ConsoleColor.DarkRed
                                Console.WriteLine("Error! Cannot find specified command. Please type 'help' for information on console for commands.")
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

    Private Sub GenericExceptionHandler(sender As Object, e As UnhandledExceptionEventArgs)
        Dim ex As Exception = e.ExceptionObject

        Log.WriteLine(LogType.CRITICAL, ex.ToString & vbNewLine)
        Log.WriteLine(LogType.FAILED, "Unexpected error has occured. An 'WorldCluster-Error-yyyy-mmm-d-h-mm.log' file has been created. Check your log folder for more information.")

        Dim tw As TextWriter
        tw = New StreamWriter(New FileStream(String.Format("WorldCluster-Error-{0}.log", Format(Now, "yyyy-MMM-d-H-mm")), FileMode.Create))
        tw.Write(ex.ToString)
        tw.Close()
    End Sub

End Module
