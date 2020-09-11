'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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

Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports System.Xml.Serialization

Imports mangosVB.Common
Imports mangosVB.Common.Globals
Imports mangosVB.Common.Logging
Imports mangosVB.Common.Logging.BaseWriter

Imports mangosVB.Shared

Imports WorldCluster.DataStores
Imports WorldCluster.Globals
Imports WorldCluster.Handlers
Imports WorldCluster.Server

Public Module WorldCluster
    Private Const ClusterPath As String = "configs/WorldCluster.ini"

    'Players' containers
    Public CLIETNIDs As Long = 0

    Public CLIENTs As New Dictionary(Of UInteger, ClientClass)

    Public CHARACTERs_Lock As New ReaderWriterLock
    Public CHARACTERs As New Dictionary(Of ULong, CharacterObject)
    'Public CHARACTER_NAMEs As New Hashtable

    'System Things...
    Public Log As New BaseWriter
    Public Rnd As New Random
    Delegate Sub HandlePacket(ByRef packet As PacketClass, ByRef client As ClientClass)

    <XmlRoot(ElementName:="WorldCluster")>
    Public Class XMLConfigFile
        <XmlElement(ElementName:="WorldClusterPort")>
        Private _worldClusterPort As Integer = 8085

        <XmlElement(ElementName:="WorldClusterAddress")>
        Private _worldClusterAddress As String = "127.0.0.1"

        <XmlElement(ElementName:="ServerPlayerLimit")>
        Private _serverPlayerLimit As Integer = 10

        'Database Settings
        <XmlElement(ElementName:="AccountDatabase")>
        Private _accountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

        <XmlElement(ElementName:="CharacterDatabase")>
        Private _characterDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

        <XmlElement(ElementName:="WorldDatabase")>
        Private _worldDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

        'Cluster Settings
        <XmlElement(ElementName:="ClusterPassword")>
        Private _clusterPassword As String = ""

        <XmlElement(ElementName:="ClusterListenMethod")>
        Private _clusterListenMethod As String = "tcp"

        <XmlElement(ElementName:="ClusterListenAddress")>
        Private _clusterListenAddress As String = "127.0.0.1"

        <XmlElement(ElementName:="ClusterListenPort")>
        Private _clusterListenPort As Integer = 50001

        <XmlArray(ElementName:="ClusterFirewall"), XmlArrayItem(GetType(String), ElementName:="IP")>
        Private _firewall As New ArrayList

        'Stats Settings
        <XmlElement(ElementName:="StatsEnabled")>
        Private _statsEnabled As Boolean = True

        <XmlElement(ElementName:="StatsTimer")>
        Private _statsTimer As Integer = 120000

        <XmlElement(ElementName:="StatsLocation")>
        Private _statsLocation As String = "stats.xml"

        'Logging Settings
        <XmlElement(ElementName:="LogType")>
        Private _logType As String = "FILE"

        <XmlElement(ElementName:="LogLevel")>
        Private _logLevel As LogType = GlobalEnum.LogType.NETWORK

        <XmlElement(ElementName:="LogConfig")>
        Private _logConfig As String = ""

        <XmlElement(ElementName:="PacketLogging")>
        Private _packetLogging As Boolean = False

        <XmlElement(ElementName:="GMLogging")>
        Private _gMLogging As Boolean = False

        Property WorldClusterPort As Integer
            Get
                Return _worldClusterPort
            End Get
            Set(value As Integer)
                _worldClusterPort = value
            End Set
        End Property

        Property WorldClusterAddress As String
            Get
                Return _worldClusterAddress
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _worldClusterAddress = value
            End Set
        End Property

        Property ServerPlayerLimit As Integer
            Get
                Return _serverPlayerLimit
            End Get
            Set(value As Integer)
                _serverPlayerLimit = value
            End Set
        End Property

        Property AccountDatabase As String
            Get
                Return _accountDatabase
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _accountDatabase = value
            End Set
        End Property

        Property CharacterDatabase As String
            Get
                Return _characterDatabase
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _characterDatabase = value
            End Set
        End Property

        Property WorldDatabase As String
            Get
                Return _worldDatabase
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _worldDatabase = value
            End Set
        End Property

        Property ClusterPassword As String
            Get
                Return _clusterPassword
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _clusterPassword = value
            End Set
        End Property

        Property ClusterListenMethod As String
            Get
                Return _clusterListenMethod
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _clusterListenMethod = value
            End Set
        End Property

        Property ClusterListenAddress As String
            Get
                Return _clusterListenAddress
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _clusterListenAddress = value
            End Set
        End Property

        Property ClusterListenPort As Integer
            Get
                Return _clusterListenPort
            End Get
            Set(value As Integer)
                _clusterListenPort = value
            End Set
        End Property

        Property Firewall As ArrayList
            Get
                Return _firewall
            End Get
            Set(value As ArrayList)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _firewall = value
            End Set
        End Property

        Property StatsEnabled As Boolean
            Get
                Return _statsEnabled
            End Get
            Set(value As Boolean)
                _statsEnabled = value
            End Set
        End Property

        Property StatsTimer As Integer
            Get
                Return _statsTimer
            End Get
            Set(value As Integer)
                _statsTimer = value
            End Set
        End Property

        Property StatsLocation As String
            Get
                Return _statsLocation
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _statsLocation = value
            End Set
        End Property

        Property LogType As String
            Get
                Return _logType
            End Get
            Set(value As String)

                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If

                _logType = value
            End Set
        End Property

        Property LogLevel As LogType
            Get
                Return _logLevel
            End Get
            Set(value As LogType)
                _logLevel = value
            End Set
        End Property

        Public Property LogConfig As String
            Get
                Return _logConfig
            End Get
            Set(value As String)
                _logConfig = value
            End Set
        End Property

        Property PacketLogging As Boolean
            Get
                Return _packetLogging
            End Get
            Set(value As Boolean)
                _packetLogging = value
            End Set
        End Property

        Property GMLogging As Boolean
            Get
                Return _gMLogging
            End Get
            Set(value As Boolean)
                _gMLogging = value
            End Set
        End Property
    End Class

    Public Sub LoadConfig()
        Try
            'Make sure WorldCluster.ini exists
            If File.Exists(ClusterPath) = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Cannot Continue. {1} does not exist.", Format(TimeOfDay, "hh:mm:ss"), ClusterPath)
                Console.WriteLine("Please make sure your ini files are inside config folder where the mangosvb executables are located.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If

            Console.Write("[{0}] Loading Configuration...", Format(TimeOfDay, "hh:mm:ss"))

            Dim xmlConfigFile As XMLConfigFile = New XMLConfigFile
            Config = xmlConfigFile
            Console.Write("...")

            Dim ostream As StreamReader
            ostream = New StreamReader(ClusterPath)
            Config = New XmlSerializer(GetType(XMLConfigFile)).Deserialize(ostream)
            ostream.Close()

            Console.WriteLine(".[done]")

            'DONE: Setting SQL Connections
            Dim AccountDBSettings() As String = Split(Config.AccountDatabase, ";")
            If AccountDBSettings.Length <> 6 Then
                Console.WriteLine("Invalid connect string for the account database!")
            Else
                AccountDatabase.SQLDBName = AccountDBSettings(4)
                AccountDatabase.SQLHost = AccountDBSettings(2)
                AccountDatabase.SQLPort = AccountDBSettings(3)
                AccountDatabase.SQLUser = AccountDBSettings(0)
                AccountDatabase.SQLPass = AccountDBSettings(1)
                AccountDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), AccountDBSettings(5))
            End If

            Dim CharacterDBSettings() As String = Split(Config.CharacterDatabase, ";")
            If CharacterDBSettings.Length <> 6 Then
                Console.WriteLine("Invalid connect string for the character database!")
            Else
                CharacterDatabase.SQLDBName = CharacterDBSettings(4)
                CharacterDatabase.SQLHost = CharacterDBSettings(2)
                CharacterDatabase.SQLPort = CharacterDBSettings(3)
                CharacterDatabase.SQLUser = CharacterDBSettings(0)
                CharacterDatabase.SQLPass = CharacterDBSettings(1)
                CharacterDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), CharacterDBSettings(5))
            End If

            Dim WorldDBSettings() As String = Split(Config.WorldDatabase, ";")
            If WorldDBSettings.Length <> 6 Then
                Console.WriteLine("Invalid connect string for the world database!")
            Else
                WorldDatabase.SQLDBName = WorldDBSettings(4)
                WorldDatabase.SQLHost = WorldDBSettings(2)
                WorldDatabase.SQLPort = WorldDBSettings(3)
                WorldDatabase.SQLUser = WorldDBSettings(0)
                WorldDatabase.SQLPass = WorldDBSettings(1)
                WorldDatabase.SQLTypeServer = [Enum].Parse(GetType(SQL.DB_Type), WorldDBSettings(5))
            End If

            'DONE: Creating logger
            CreateLog(Config.LogType, Config.LogConfig, Log)
            Log.LogLevel = Config.LogLevel

            'DONE: Cleaning up the packet log
            If Config.PacketLogging Then
                File.Delete("packets.log")
            End If

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub

#Region "WS.DataAccess"

    Public Property PacketHandlers As New Dictionary(Of OPCODES, HandlePacket)

    Public Property Config As XMLConfigFile

    Public Property AccountDatabase As New SQL
    Public Property CharacterDatabase As New SQL
    Public Property WorldDatabase As New SQL

    Public Sub AccountSQLEventHandler(messageId As SQL.EMessages, outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[ACCOUNT] " & outBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[ACCOUNT] " & outBuf)
            Case Else
                Exit Select
        End Select
    End Sub

    Public Sub CharacterSQLEventHandler(messageId As SQL.EMessages, outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[CHARACTER] " & outBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[CHARACTER] " & outBuf)
            Case Else
                Exit Select
        End Select
    End Sub

    Public Sub WorldSQLEventHandler(messageId As SQL.EMessages, outBuf As String)
        Select Case messageId
            Case SQL.EMessages.ID_Error
                Log.WriteLine(LogType.FAILED, "[WORLD] " & outBuf)
            Case SQL.EMessages.ID_Message
                Log.WriteLine(LogType.SUCCESS, "[WORLD] " & outBuf)
            Case Else
                Exit Select
        End Select
    End Sub
#End Region

    <MTAThread()>
    Public Sub Main()
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

        If DoesSharedDllExist() = False Then
            End
        End If

        Console.ForegroundColor = ConsoleColor.Magenta

        Console.ForegroundColor = ConsoleColor.White
        Dim assemblyTitleAttribute1 As AssemblyTitleAttribute = CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute)
        Console.WriteLine(assemblyTitleAttribute1.Title)

        Console.WriteLine("version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.ForegroundColor = ConsoleColor.White

        Console.WriteLine("")
        Console.ForegroundColor = ConsoleColor.Gray

        Log.WriteLine(LogType.INFORMATION, "[{0}] World Cluster Starting...", Format(TimeOfDay, "hh:mm:ss"))

        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf GenericExceptionHandler

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

        If CheckRequiredDbVersion(AccountDatabase, ServerDb.Realm) = False Then         'Check the Database version, exit if its wrong

            If True Then
                Console.WriteLine("*************************")
                Console.WriteLine("* Press any key to exit *")
                Console.WriteLine("*************************")
                Console.ReadKey()
                End
            End If
        End If

        If CheckRequiredDbVersion(CharacterDatabase, ServerDb.Character) = False Then         'Check the Database version, exit if its wrong

            If True Then
                Console.WriteLine("*************************")
                Console.WriteLine("* Press any key to exit *")
                Console.WriteLine("*************************")
                Console.ReadKey()
                End
            End If
        End If

        If CheckRequiredDbVersion(WorldDatabase, ServerDb.World) = False Then         'Check the Database version, exit if its wrong

            If True Then
                Console.WriteLine("*************************")
                Console.WriteLine("* Press any key to exit *")
                Console.WriteLine("*************************")
                Console.ReadKey()
                End
            End If
        End If

        WorldServer = New WorldServerClass
        GC.Collect()

        If Process.GetCurrentProcess().PriorityClass <> ProcessPriorityClass.High Then
            Log.WriteLine(LogType.WARNING, "Setting Process Priority to NORMAL..[done]")
        Else
            Log.WriteLine(LogType.WARNING, "Setting Process Priority to HIGH..[done]")
        End If

        Log.WriteLine(LogType.INFORMATION, "Load Time: {0}", Format(DateDiff(DateInterval.Second, Now, Now), "0 seconds"))
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
