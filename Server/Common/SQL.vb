' 
' Copyright (C) 2011 SpuriousZero <http://www.spuriousemu.com/>
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

Imports System.Data
Imports System.ComponentModel
Imports System.Threading
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.Data.OracleClient

<CLSCompliant(True), ComClass(SQL.ClassId, SQL.InterfaceId, SQL.EventsId)> _
Public Class SQL
    Implements IDisposable

#Region "Class's used"
    <CLSCompliant(False)> _
    Private MySQLConn As MySqlConnection
    Private MSSQLConn As SqlConnection
    Private OracleConn As OracleConnection
#End Region

#Region "Events and event ID's"
    Public Enum EMessages
        ID_Error = 0
        ID_Message = 1
    End Enum

    Public Event SQLMessage(ByVal MessageID As EMessages, ByVal OutBuf As String)
#End Region

#Region "COM GUIDs"
    ' These  GUIDs provide the COM identity for this class 
    ' and its COM interfaces. If you change them, existing 
    ' clients will no longer be able to access the class.
    Public Const ClassId As String = "ECC3DCA3-E394-4D4F-BCC9-FBC3A999B8D3"
    Public Const InterfaceId As String = "4A2A2AF5-39A2-44BA-9881-57AA6D867D33"
    Public Const EventsId As String = "BD50B4A8-D148-4C52-B8FC-469119FBB71D"
#End Region

    ' A creatable COM class must have a Public Sub New() 
    ' with no parameters, otherwise, the class will not be 
    ' registered in the COM registry and cannot be created 
    ' via CreateObject.
    Public Sub New()
        MyBase.New()
    End Sub

#Region "Version Info <Update VInfo and rvDate as needed>"
    Private VInfo As String = "2.1.0a"
    Private rvDate As String = "9:36 PM, Wednesday, September, 25, 2006"

    <Description("Class info version/last date updated.")> _
    Public ReadOnly Property Class_Version_Info() As String
        Get
            Return "Version: " + VInfo + ", Updated at: " + rvDate
        End Get
    End Property
#End Region

#Region "SQL startup Propertys, connections and disposal"
    'SQL Host name/password/etc..
    Private v_SQLHost As String = "localhost"
    Private v_SQLPort As String = "3306"
    Private v_SQLUser As String = ""
    Private v_SQLPass As String = ""
    Private v_SQLDBName As String = ""
    Public Enum DB_Type
        MySQL = 0
        MSSQL = 1
        Oracle = 2
        SQLite = 3
    End Enum
    Private v_SQLType As DB_Type

#Region "Main propertys"
#Region "Server type selection      MySQL|MSSQL|Oracle Supported"
    <Description("SQL Server selection.")> _
    Public Property SQLTypeServer() As DB_Type
        Get
            SQLTypeServer = v_SQLType
        End Get
        Set(ByVal value As DB_Type)
            v_SQLType = value
        End Set
    End Property
#End Region
#Region "Server host ip"
    <Description("SQL Host name.")> _
    Public Property SQLHost() As String
        Get
            SQLHost = v_SQLHost
        End Get
        Set(ByVal value As String)
            v_SQLHost = value
        End Set
    End Property
#End Region
#Region "Server host port"
    <Description("SQL Host port.")> _
    Public Property SQLPort() As String
        Get
            SQLPort = v_SQLPort
        End Get
        Set(ByVal value As String)
            v_SQLPort = value
        End Set
    End Property
#End Region
#Region "Server username"
    <Description("SQL User name.")> _
    Public Property SQLUser() As String
        Get
            SQLUser = v_SQLUser
        End Get
        Set(ByVal value As String)
            v_SQLUser = value
        End Set
    End Property
#End Region
#Region "Server Password"
    <Description("SQL Password.")> _
    Public Property SQLPass() As String
        Get
            SQLPass = v_SQLPass
        End Get
        Set(ByVal value As String)
            v_SQLPass = value
        End Set
    End Property
#End Region
#Region "Database Name"
    <Description("SQL Database name.")> _
    Public Property SQLDBName() As String
        Get
            SQLDBName = v_SQLDBName
        End Get
        Set(ByVal value As String)
            v_SQLDBName = value
        End Set
    End Property
#End Region
#End Region
#Region "Main Functions"
#Region "Connect()                  MySQL|MSSQL|Oracle Supported"
    <Description("Start up the SQL connection.")> _
    Public Sub Connect()
        Try
            If SQLHost.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLHost cannot be empty")
                Exit Sub
            End If
            If SQLPort.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLPort cannot be empty")
                Exit Sub
            End If
            If SQLUser.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLUser cannot be empty")
                Exit Sub
            End If
            If SQLPass.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLPassword cannot be empty")
                Exit Sub
            End If
            If SQLDBName.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLDatabaseName cannot be empty")
                Exit Sub
            End If

            Select Case v_SQLType
                Case DB_Type.MySQL
                    MySQLConn = New MySqlConnection([String].Format("Server={0};Port={4};User ID={1};Password={2};Database={3};Compress=false;Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    MySQLConn.Open()
                    RaiseEvent SQLMessage(EMessages.ID_Message, "MySQL Connection Opened Successfully [" & SQLUser & "@" & SQLHost & "]")

                Case DB_Type.MSSQL
                    MSSQLConn = New SqlConnection([String].Format("Server={0},{4};User ID={1};Password={2};Database={3};Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    If MSSQLConn.State = ConnectionState.Closed Then
                        MSSQLConn.Open()
                    End If
                    RaiseEvent SQLMessage(EMessages.ID_Message, "MS-SQL Connection Opened Successfully [" & SQLUser & "@" & SQLHost & "]")

                Case DB_Type.Oracle
                    Dim OracleConn As New OracleConnection([String].Format("Host={0};Port={4};User Id={1};Password={2};Data Source={3};", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    OracleConn.Open()
                    RaiseEvent SQLMessage(EMessages.ID_Message, "Oracle Connection Opened Successfully [" & SQLUser & "@" & SQLHost & "]")


            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Connection Error [" & e.Message & "]")
        Catch e As SqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "MSSQL Connection Error [" & e.Message & "]")
        Catch e As OracleException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Connection Error [" & e.Message & "]")
        End Try
    End Sub
#End Region
#Region "Restart()                  MySQL|MSSQL|Oracle Supported"
    <Description("Restart the SQL connection.")> _
    Public Sub Restart()
        Try
            Select Case v_SQLType
                Case DB_Type.MySQL
                    MySQLConn.Close()
                    MySQLConn.Dispose()
                    MySQLConn = New MySqlConnection([String].Format("Server={0};Port={4};User ID={1};Password={2};Database={3};Compress=false;Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    MySQLConn.Open()

                    If MySQLConn.State = ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Message, "MySQL Connection restarted!")
                    Else
                        RaiseEvent SQLMessage(EMessages.ID_Error, "Unable to restart MySQL connection.")
                    End If
                Case DB_Type.MSSQL
                    MSSQLConn.Close()
                    MSSQLConn.Dispose()
                    MSSQLConn = New SqlConnection([String].Format("Server={0},{4};User ID={1};Password={2};Database={3};Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    MSSQLConn.Open()

                    If MSSQLConn.State = ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Message, "MSSQL Connection restarted!")
                    Else
                        RaiseEvent SQLMessage(EMessages.ID_Error, "Unable to restart MSSQL connection.")
                    End If
                Case DB_Type.Oracle
                    OracleConn.Close()
                    OracleConn.Dispose()
                    OracleConn = New OracleConnection([String].Format("Host={0};Port={4};User Id={1};Password={2};Data Source={3};", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    OracleConn.Open()

                    If OracleConn.State = ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Message, "Oracle Connection restarted!")
                    Else
                        RaiseEvent SQLMessage(EMessages.ID_Error, "Unable to restart Oracle connection.")
                    End If
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Connection Error [" & e.Message & "]")
        Catch e As SqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "MSSQL Connection Error [" & e.Message & "]")
        Catch e As OracleException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Connection Error [" & e.Message & "]")
        End Try
    End Sub
#End Region
#Region "Dispose()                  MySQL|MSSQL|Oracle Supported"
    Public Sub Dispose() Implements System.IDisposable.Dispose
        Select Case v_SQLType
            Case DB_Type.MySQL
                MySQLConn.Close()
                MySQLConn.Dispose()
            Case DB_Type.MSSQL
                MSSQLConn.Close()
                MSSQLConn.Dispose()
            Case DB_Type.Oracle
                OracleConn.Close()
                OracleConn.Dispose()
        End Select
    End Sub
#End Region
#End Region
#End Region

#Region "SQL Wraper for VB 6.0."
    Private mQuery As String = ""
    Private mResult As DataTable

#Region "Query Wraper"
    <Description("SQLQuery. EG.: (SELECT * FROM db_accounts WHERE account = 'name';')")> _
    Public Function QuerySQL(ByVal SQLQuery As String) As Boolean
        mQuery = SQLQuery
        Query(mQuery, mResult)
        If mResult.Rows.Count > 0 Then
            'Table gathered
            Return True
        Else
            'Table dosent exist
            Return False
        End If
    End Function
#End Region
#Region "SQL Get [STRING], [DataTable]"
    <Description("SQLGet. Used after the query to get a section value")> _
    Public Function GetSQL(ByVal TableSection As String) As String
        Return (mResult.Rows(0).Item(TableSection)).ToString
    End Function
    Public Function GetDataTableSQL() As DataTable
        Return mResult
    End Function
#End Region
#Region "Insert Wraper"
    <Description("SQLInsert. EG.: (INSERT INTO db_textpage (pageid, text, nextpageid, wdbversion, checksum) VALUES ('pageid DWORD', 'pagetext STRING', 'nextpage DWORD', 'version DWORD', 'checksum DWORD')")> _
    Public Sub InsertSQL(ByVal SQLInsertionQuery As String)
        Insert(SQLInsertionQuery)
    End Sub
#End Region
#Region "Update Wraper"
    <Description("SQLUpdate. EG.: (UPDATE db_textpage SET pagetext='pagetextstring' WHERE pageid = 'pageiddword';")> _
    Public Sub UpdateSQL(ByVal SQLUpdateQuery As String)
        Update(SQLUpdateQuery)
    End Sub
#End Region
#End Region

#Region "Main SQL Functions Used."
#Region "Query      MySQL|MSSQL|Oracle Supported       [SELECT * FROM db_accounts WHERE account = 'name';']"
    Public Sub Query(ByVal sqlquery As String, ByRef Result As DataTable)
        Select Case v_SQLType
            Case DB_Type.MySQL
                If MySQLConn.State <> ConnectionState.Open Then
                    Restart()
                    If MySQLConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DB_Type.MSSQL
                If MSSQLConn.State <> ConnectionState.Open Then
                    Restart()
                    If MSSQLConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "MSSQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DB_Type.Oracle
                If OracleConn.State <> ConnectionState.Open Then
                    Restart()
                    If OracleConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Database Request Failed!")
                        Exit Sub
                    End If
                End If
        End Select

        Try
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Enter(MySQLConn)
                    Dim MySQLCommand As New MySQLCommand(sqlquery, MySQLConn)
                    Dim MySQLAdapter As New MySqlDataAdapter(MySQLCommand)

                    If Result Is Nothing Then
                        Result = New DataTable
                    Else
                        Result.Clear()
                    End If
                    MySQLAdapter.Fill(Result)


                Case DB_Type.MSSQL
                    Monitor.Enter(MSSQLConn)
                    Dim MSSQLCommand As SqlCommand = New SqlCommand(sqlquery, MSSQLConn)
                    Dim MSSQLAdapter As New SqlDataAdapter(MSSQLCommand)

                    If Result Is Nothing Then
                        Result = New DataTable
                    Else
                        Result.Clear()
                    End If
                    MSSQLAdapter.Fill(Result)


                Case DB_Type.Oracle
                    Monitor.Enter(OracleConn)
                    Dim OracleCommand As OracleCommand = New OracleCommand(sqlquery, OracleConn)
                    Dim OracleAdapter As New OracleDataAdapter(OracleCommand)

                    If Result Is Nothing Then
                        Result = New DataTable
                    Else
                        Result.Clear()
                    End If
                    OracleAdapter.Fill(Result)
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Catch e As SqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MSSQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Catch e As OracleException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From Oracle Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Finally
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Exit(MySQLConn)
                Case DB_Type.MSSQL
                    Monitor.Exit(MSSQLConn)
                Case DB_Type.Oracle
                    Monitor.Exit(OracleConn)
            End Select
        End Try
    End Sub
#End Region
#Region "Insert     MySQL|MSSQL|Oracle Supported       [INSERT INTO db_textpage (pageid, text, nextpageid, wdbversion, checksum) VALUES ('pageid DWORD', 'pagetext STRING', 'nextpage DWORD', 'version DWORD', 'checksum DWORD']"
    Public Sub Insert(ByVal sqlquery As String)
        Select Case v_SQLType
            Case DB_Type.MySQL
                If MySQLConn.State <> ConnectionState.Open Then
                    Restart()
                    If MySQLConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DB_Type.MSSQL
                If MSSQLConn.State <> ConnectionState.Open Then
                    Restart()
                    If MSSQLConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "MSSQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DB_Type.Oracle
                If OracleConn.State <> ConnectionState.Open Then
                    Restart()
                    If OracleConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Database Request Failed!")
                        Exit Sub
                    End If
                End If
        End Select

        Try
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Enter(MySQLConn)
                    Dim MySQLTransaction As MySqlTransaction = MySQLConn.BeginTransaction
                    Dim MySQLCommand As New MySqlCommand(sqlquery, MySQLConn, MySQLTransaction)

                    MySQLCommand.ExecuteNonQuery()
                    MySQLTransaction.Commit()
                    Console.WriteLine("transaction completed")


                Case DB_Type.MSSQL
                    Monitor.Enter(MSSQLConn)
                    Dim MSSQLTransaction As SqlTransaction = MSSQLConn.BeginTransaction()
                    Dim MSSQLCommand As New SqlCommand(sqlquery, MSSQLConn, MSSQLTransaction)

                    MSSQLCommand.ExecuteNonQuery()
                    MSSQLTransaction.Commit()
                    Console.WriteLine("transaction completed")


                Case DB_Type.Oracle
                    Monitor.Enter(OracleConn)
                    Dim OracleTransaction As OracleTransaction = OracleConn.BeginTransaction()
                    Dim OracleCommand As New OracleCommand(sqlquery, OracleConn, OracleTransaction)

                    OracleCommand.ExecuteNonQuery()
                    OracleTransaction.Commit()
                    Console.WriteLine("transaction completed")
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Insert string was: " & sqlquery)
        Catch e As SqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MSSQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Catch e As OracleException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From Oracle Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Finally
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Exit(MySQLConn)
                Case DB_Type.MSSQL
                    Monitor.Exit(MSSQLConn)
                Case DB_Type.Oracle
                    Monitor.Exit(OracleConn)
            End Select
        End Try
    End Sub
#End Region
#Region "Update     MySQL|MSSQL|Oracle Supported       [UPDATE db_textpage SET pagetext='pagetextstring' WHERE pageid = 'pageiddword';]"
    Public Sub Update(ByVal sqlquery As String)
        Select Case v_SQLType
            Case DB_Type.MySQL
                If MySQLConn.State <> ConnectionState.Open Then
                    Restart()
                    If MySQLConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DB_Type.MSSQL
                If MSSQLConn.State <> ConnectionState.Open Then
                    Restart()
                    If MSSQLConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "MSSQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DB_Type.Oracle
                If OracleConn.State <> ConnectionState.Open Then
                    Restart()
                    If OracleConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Database Request Failed!")
                        Exit Sub
                    End If
                End If
        End Select

        Try
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Enter(MySQLConn)
                    Dim MySQLCommand As New MySqlCommand(sqlquery, MySQLConn)
                    Dim MySQLAdapter As New MySqlDataAdapter(MySQLCommand)

                    Dim result As New DataTable
                    MySQLAdapter.Fill(result)


                Case DB_Type.MSSQL
                    Monitor.Enter(MSSQLConn)
                    Dim MSSQLCommand As New SqlCommand(sqlquery, MSSQLConn)
                    Dim MSSQLAdapter As New SqlDataAdapter(MSSQLCommand)

                    Dim result As New DataTable
                    MSSQLAdapter.Fill(result)


                Case DB_Type.Oracle
                    Monitor.Enter(OracleConn)
                    Dim OracleCommand As New OracleCommand(sqlquery, OracleConn)
                    Dim OracleAdapter As New OracleDataAdapter(OracleCommand)

                    Dim result As New DataTable
                    OracleAdapter.Fill(result)
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Update string was: " & sqlquery)
        Catch e As SqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MSSQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Catch e As OracleException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From Oracle Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Finally
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Exit(MySQLConn)
                Case DB_Type.MSSQL
                    Monitor.Exit(MSSQLConn)
                Case DB_Type.Oracle
                    Monitor.Exit(OracleConn)
            End Select
        End Try
    End Sub
#End Region
#End Region

End Class


