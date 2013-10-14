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
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common


Public Module WorldCluster


#Region "Global.Variables"
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
    Delegate Sub HandlePacket(ByRef Packet As PacketClass, ByRef Client As ClientClass)

#End Region
#Region "Global.Config"
    Public Config As XMLConfigFile
    <XmlRoot(ElementName:="WorldCluster")> _
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
        <XmlElement(ElementName:="LogType")> Public LogType As String = "COLORCONSOLE"
        <XmlElement(ElementName:="LogLevel")> Public LogLevel As LogType = mangosVB.Common.BaseWriter.LogType.NETWORK
        <XmlElement(ElementName:="LogConfig")> Public LogConfig As String = ""
        <XmlElement(ElementName:="PacketLogging")> Public PacketLogging As Boolean = False
        <XmlElement(ElementName:="GMLogging")> Public GMLogging As Boolean = False
    End Class

    Public Sub LoadConfig()
        Try
            'Make sure WorldCluster.ini exists
            If System.IO.File.Exists("WorldCluster.ini") = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Cannot Continue. {1} does not exist.", Format(TimeOfDay, "HH:mm:ss"), "WorldCluster.ini")
                Console.WriteLine("Please copy the ini files into the same directory as the mangosVB exe files.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If

            Console.Write("[{0}] Loading Configuration...", Format(TimeOfDay, "HH:mm:ss"))

            Config = New XMLConfigFile
            Console.Write("...")

            Dim oXS As XmlSerializer = New XmlSerializer(GetType(XMLConfigFile))

            Console.Write("...")
            Dim oStmR As StreamReader
            oStmR = New StreamReader("WorldCluster.ini")
            Config = oXS.Deserialize(oStmR)
            oStmR.Close()


            Console.WriteLine(".[done]")


            'DONE: Setting SQL Connections
            Dim AccountDBSettings() As String = Split(Config.AccountDatabase, ";")
            If AccountDBSettings.Length = 6 Then
                AccountDatabase.SQLDBName = AccountDBSettings(4)
                AccountDatabase.SQLHost = AccountDBSettings(2)
                AccountDatabase.SQLPort = AccountDBSettings(3)
                AccountDatabase.SQLUser = AccountDBSettings(0)
                AccountDatabase.SQLPass = AccountDBSettings(1)
                AccountDatabase.SQLTypeServer = CType([Enum].Parse(GetType(SQL.DB_Type), AccountDBSettings(5)), SQL.DB_Type)
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
                CharacterDatabase.SQLTypeServer = CType([Enum].Parse(GetType(SQL.DB_Type), CharacterDBSettings(5)), SQL.DB_Type)
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
                WorldDatabase.SQLTypeServer = CType([Enum].Parse(GetType(SQL.DB_Type), WorldDBSettings(5)), SQL.DB_Type)
            Else
                Console.WriteLine("Invalid connect string for the world database!")
            End If

            'DONE: Creating logger
            BaseWriter.CreateLog(Config.LogType, Config.LogConfig, Log)
            Log.LogLevel = Config.LogLevel

            'DONE: Cleaning up the packet log
            If Config.PacketLogging Then
                File.Delete("packets.log")
            End If

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub
#End Region

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

    <System.MTAThreadAttribute()> _
    Sub Main()
        timeBeginPeriod(1)  'Set timeGetTime to a accuracy of 1ms

        Console.BackgroundColor = System.ConsoleColor.Black
        Console.Title = String.Format("{0} v{1}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title, [Assembly].GetExecutingAssembly().GetName().Version)

        Console.ForegroundColor = System.ConsoleColor.Yellow
        Console.WriteLine("{0}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyProductAttribute), False)(0), AssemblyProductAttribute).Product)
        Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)(0), AssemblyCopyrightAttribute).Copyright)
        Console.WriteLine()

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
        Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title)
        Console.WriteLine("version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.WriteLine("svn-reversion {0} ({1})", svnRevision, svnDate)
        Console.ForegroundColor = System.ConsoleColor.White


        Console.WriteLine("")
        Console.ForegroundColor = System.ConsoleColor.Gray

        Dim dateTimeStarted As Date = Now
        Log.WriteLine(LogType.INFORMATION, "[{0}] World Cluster Starting...", Format(TimeOfDay, "HH:mm:ss"))

        Dim currentDomain As AppDomain = AppDomain.CurrentDomain
        AddHandler currentDomain.UnhandledException, AddressOf GenericExceptionHandler

        LoadConfig()
        Console.ForegroundColor = System.ConsoleColor.Gray
        AddHandler AccountDatabase.SQLMessage, AddressOf AccountSQLEventHandler
        AddHandler CharacterDatabase.SQLMessage, AddressOf CharacterSQLEventHandler
        AddHandler WorldDatabase.SQLMessage, AddressOf WorldSQLEventHandler
        AccountDatabase.Connect()
        AccountDatabase.Update("SET NAMES 'utf8';")
        CharacterDatabase.Connect()
        CharacterDatabase.Update("SET NAMES 'utf8';")
        WorldDatabase.Connect()
        WorldDatabase.Update("SET NAMES 'utf8';")

#If DEBUG Then
        Log.WriteLine(LogType.DEBUG, "Setting MySQL into debug mode..[done]")
        AccountDatabase.Update("SET SESSION sql_mode='STRICT_ALL_TABLES';")
        CharacterDatabase.Update("SET SESSION sql_mode='STRICT_ALL_TABLES';")
        WorldDatabase.Update("SET SESSION sql_mode='STRICT_ALL_TABLES';")
#End If
        InitializeInternalDatabase()
        IntializePacketHandlers()
        WorldServer = New WorldServerClass
        GC.Collect()

        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High
        Log.WriteLine(LogType.WARNING, "Setting Process Priority to HIGH..[done]")

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
                        If cmds(1).Trim().Length > 0 Then cmd = Split(cmds(1).Trim, " ")
                        '<<<<<<<<<<<COMMAND STRUCTURE>>>>>>>>>>
                        Select Case cmds(0).ToLower
                            Case "createaccount", "/createaccount"
                                If cmd.Length <> 3 Then
                                    Console.ForegroundColor = System.ConsoleColor.Yellow
                                    Console.WriteLine("[{0}] USAGE: createaccount <account> <password> <email>", Format(TimeOfDay, "HH:mm:ss"))
                                Else
                                    Dim passwordStr() As Byte = System.Text.Encoding.ASCII.GetBytes(cmd(0).ToUpper & ":" & cmd(1).ToUpper)
                                    Dim passwordHash() As Byte = New System.Security.Cryptography.SHA1Managed().ComputeHash(passwordStr)
                                    Dim hashStr As String = BitConverter.ToString(passwordHash).Replace("-", "")

                                    AccountDatabase.InsertSQL([String].Format("INSERT INTO accounts (username, sha_pass_hash, email, joindate, last_ip) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", cmd(0), hashStr, cmd(2), Format(Now, "yyyy-MM-dd"), "0.0.0.0"))
                                    If AccountDatabase.QuerySQL("SELECT * FROM accounts WHERE username = """ & cmd(0) & """;") Then
                                        Console.ForegroundColor = System.ConsoleColor.Green
                                        Console.WriteLine("[Account: " & cmd(0) & " Password: " & cmd(1) & " Email: " & cmd(2) & "] has been created.")
                                        Console.ForegroundColor = System.ConsoleColor.Gray
                                    Else
                                        Console.ForegroundColor = System.ConsoleColor.Red
                                        Console.WriteLine("[Account: " & cmd(0) & " Password: " & cmd(1) & " Email: " & cmd(2) & "] could not be created.")
                                        Console.ForegroundColor = System.ConsoleColor.Gray
                                    End If
                                End If
                            Case "gccollect"
                                GC.Collect()
                                Console.ForegroundColor = System.ConsoleColor.Blue
                                Console.WriteLine("'WorldCluster' Command list:")
                                Console.ForegroundColor = System.ConsoleColor.White
                                Console.WriteLine("---------------------------------")
                                Console.WriteLine("")
                                Console.WriteLine("'createaccount <user> <password> <email>' or '/createaccount <user> <password> <email>' - Creates an account with the specified username <user>, password <password>, and email <email>.")
                            Case Else
                                Console.ForegroundColor = System.ConsoleColor.Red
                                Console.WriteLine("Error! Cannot find specified command. Please type 'help' for information on 'WorldCluster' console commands.")
                                Console.ForegroundColor = System.ConsoleColor.White
                        End Select
                        '<<<<<<<<<<</END COMMAND STRUCTURE>>>>>>>>>>>>
                    End If
                Next
            Catch e As Exception
                'Needed to be rewritten do to an unknown call from this line.
                'Log.WriteLine(LogType.FAILED, "Error executing command [{0}]. {2}{1}", Format(TimeOfDay, "HH:mm:ss"), tmp, e.ToString, vbNewLine)
            End Try
        End While
    End Sub

    Private Sub GenericExceptionHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim EX As Exception
        EX = e.ExceptionObject

        Log.WriteLine(LogType.CRITICAL, EX.ToString & vbNewLine)
        Log.WriteLine(LogType.FAILED, "Unexpected error has occured. An 'Error-yyyy-mmm-d-h-mm.log' file has been created. Please post the file in the HELP SECTION at getMangos.com (http://www.getMangos.co.uk)!")

        Dim tw As TextWriter
        tw = New StreamWriter(New FileStream(String.Format("Error-{0}.log", Format(Now, "yyyy-MMM-d-H-mm")), FileMode.Create))
        tw.Write(EX.ToString)
        tw.Close()
    End Sub


End Module
