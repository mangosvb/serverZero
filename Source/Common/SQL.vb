'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getmangos.eu>
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
Imports System.ComponentModel
Imports System.Threading
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
'Imports System.Data.OracleClient

<CLSCompliant(True), ComClass(Sql.ClassId, Sql.InterfaceId, Sql.EventsId)>
Public Class Sql
    Implements IDisposable

#Region "Class's used"
    <CLSCompliant(False)>
    Private _mySqlConn As MySqlConnection
    Private _mssqlConn As SqlConnection
    'Private OracleConn As OracleConnection
#End Region

#Region "Events and event ID's"
    Public Enum EMessages
        IdError = 0
        IdMessage = 1
    End Enum

    Public Event SqlMessage(ByVal messageId As EMessages, ByVal outBuf As String)
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

    '#Region "Version Info <Update VInfo and rvDate as needed>"
    '    Private VInfo As String = "2.1.0a"
    '    Private rvDate As String = "9:36 PM, Wednesday, September, 25, 2006"

    '    <Description("Class info version/last date updated.")> _
    '    Public ReadOnly Property Class_Version_Info() As String
    '        Get
    '            Return "Version: " + VInfo + ", Updated at: " + rvDate
    '        End Get
    '    End Property
    '#End Region

#Region "SQL startup Propertys, connections and disposal"
    'SQL Host name/password/etc..
    Private _vSqlHost As String = "localhost"
    Private _vSqlPort As String = "3306"
    Private _vSqlUser As String = ""
    Private _vSqlPass As String = ""
    Private _vSqldbName As String = ""
    Public Enum DbType
        MySql = 0
        Mssql = 1
        'Oracle = 2
        'SqLite = 3
    End Enum

    Public Enum ReturnState
        Success = 0
        MinorError = 1
        FatalError = 2
    End Enum

    Private _vSqlType As DbType

#Region "Main propertys"
#Region "Server type selection      MySQL|MSSQL|Oracle Supported"
    <Description("SQL Server selection.")>
    Public Property SqlTypeServer() As DbType
        Get
            SqlTypeServer = _vSqlType
        End Get
        Set(ByVal value As DbType)
            _vSqlType = value
        End Set
    End Property
#End Region
#Region "Server host ip"
    <Description("SQL Host name.")>
    Public Property SqlHost() As String
        Get
            SqlHost = _vSqlHost
        End Get
        Set(ByVal value As String)
            _vSqlHost = value
        End Set
    End Property
#End Region
#Region "Server host port"
    <Description("SQL Host port.")>
    Public Property SqlPort() As String
        Get
            SqlPort = _vSqlPort
        End Get
        Set(ByVal value As String)
            _vSqlPort = value
        End Set
    End Property
#End Region
#Region "Server username"
    <Description("SQL User name.")>
    Public Property SqlUser() As String
        Get
            SqlUser = _vSqlUser
        End Get
        Set(ByVal value As String)
            _vSqlUser = value
        End Set
    End Property
#End Region
#Region "Server Password"
    <Description("SQL Password.")>
    Public Property SqlPass() As String
        Get
            SqlPass = _vSqlPass
        End Get
        Set(ByVal value As String)
            _vSqlPass = value
        End Set
    End Property
#End Region
#Region "Database Name"
    <Description("SQL Database name.")>
    Public Property SqldbName() As String
        Get
            SqldbName = _vSqldbName
        End Get
        Set(ByVal value As String)
            _vSqldbName = value
        End Set
    End Property
#End Region
#End Region
#Region "Main Functions"
#Region "Connect()                  MySQL|MSSQL|Oracle Supported"
    <Description("Start up the SQL connection.")>
    Public Function Connect() As Integer
        Try
            If SQLHost.Length < 1 Then
                RaiseEvent SqlMessage(EMessages.IdError, "You have to set the SQLHost cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLPort.Length < 1 Then
                RaiseEvent SqlMessage(EMessages.IdError, "You have to set the SQLPort cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLUser.Length < 1 Then
                RaiseEvent SqlMessage(EMessages.IdError, "You have to set the SQLUser cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLPass.Length < 1 Then
                RaiseEvent SqlMessage(EMessages.IdError, "You have to set the SQLPassword cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLDBName.Length < 1 Then
                RaiseEvent SqlMessage(EMessages.IdError, "You have to set the SQLDatabaseName cannot be empty")
                Return ReturnState.FatalError
            End If

            Select Case _vSqlType
                Case DbType.MySql
                    _mySqlConn = New MySqlConnection(String.Format("Server={0};Port={4};User ID={1};Password={2};Database={3};Compress=false;Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    _mySqlConn.Open()
                    RaiseEvent SqlMessage(EMessages.IdMessage, "MySQL Connection Opened Successfully [" & SQLUser & "@" & SQLHost & "]")

                Case DbType.Mssql
                    _mssqlConn = New SqlConnection(String.Format("Server={0},{4};User ID={1};Password={2};Database={3};Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    If _mssqlConn.State = ConnectionState.Closed Then
                        _mssqlConn.Open()
                    End If
                    RaiseEvent SqlMessage(EMessages.IdMessage, "MS-SQL Connection Opened Successfully [" & SQLUser & "@" & SQLHost & "]")

                    'Case DB_Type.Oracle
                    '    Dim OracleConn As New OracleConnection([String].Format("Host={0};Port={4};User Id={1};Password={2};Data Source={3};", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    '    OracleConn.Open()
                    '    RaiseEvent SQLMessage(EMessages.ID_Message, "Oracle Connection Opened Successfully [" & SQLUser & "@" & SQLHost & "]")

            End Select

        Catch e As MySqlException
            RaiseEvent SqlMessage(EMessages.IdError, "MySQL Connection Error [" & e.Message & "]")
            Return ReturnState.FatalError
        Catch e As SqlException
            RaiseEvent SqlMessage(EMessages.IdError, "MSSQL Connection Error [" & e.Message & "]")
            Return ReturnState.FatalError
            'Catch e As OracleException
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Connection Error [" & e.Message & "]")
        End Try
        Return ReturnState.Success
    End Function
#End Region
#Region "Restart()                  MySQL|MSSQL|Oracle Supported"
    <Description("Restart the SQL connection.")>
    Public Sub Restart()
        Try
            Select Case _vSqlType
                Case DbType.MySql
                    _mySqlConn.Close()
                    _mySqlConn.Dispose()
                    _mySqlConn = New MySqlConnection(String.Format("Server={0};Port={4};User ID={1};Password={2};Database={3};Compress=false;Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    _mySqlConn.Open()

                    If _mySqlConn.State = ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdMessage, "MySQL Connection restarted!")
                    Else
                        RaiseEvent SqlMessage(EMessages.IdError, "Unable to restart MySQL connection.")
                    End If
                Case DbType.Mssql
                    _mssqlConn.Close()
                    _mssqlConn.Dispose()
                    _mssqlConn = New SqlConnection(String.Format("Server={0},{4};User ID={1};Password={2};Database={3};Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    _mssqlConn.Open()

                    If _mssqlConn.State = ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdMessage, "MSSQL Connection restarted!")
                    Else
                        RaiseEvent SqlMessage(EMessages.IdError, "Unable to restart MSSQL connection.")
                    End If
                    'Case DB_Type.Oracle
                    '    OracleConn.Close()
                    '    OracleConn.Dispose()
                    '    OracleConn = New OracleConnection([String].Format("Host={0};Port={4};User Id={1};Password={2};Data Source={3};", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    '    OracleConn.Open()

                    '    If OracleConn.State = ConnectionState.Open Then
                    '        RaiseEvent SQLMessage(EMessages.ID_Message, "Oracle Connection restarted!")
                    '    Else
                    '        RaiseEvent SQLMessage(EMessages.ID_Error, "Unable to restart Oracle connection.")
                    '    End If
            End Select

        Catch e As MySqlException
            RaiseEvent SqlMessage(EMessages.IdError, "MySQL Connection Error [" & e.Message & "]")
        Catch e As SqlException
            RaiseEvent SqlMessage(EMessages.IdError, "MSSQL Connection Error [" & e.Message & "]")
            'Catch e As OracleException
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Connection Error [" & e.Message & "]")
        End Try
    End Sub
#End Region

#Region "IDisposable Support"
    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    <Description("Close file and dispose the wdb reader.")>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            Select Case _vSqlType
                Case DbType.MySql
                    _mySqlConn.Close()
                    _mySqlConn.Dispose()
                Case DbType.Mssql
                    _mssqlConn.Close()
                    _mssqlConn.Dispose()
                    'Case DB_Type.Oracle
                    '    OracleConn.Close()
                    '    OracleConn.Dispose()
            End Select
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

#End Region
#End Region

#Region "SQL Wraper for VB 6.0."
    Private _mQuery As String = ""
    Private _mResult As DataTable

#Region "Query Wraper"
    <Description("SQLQuery. EG.: (SELECT * FROM db_accounts WHERE account = 'name';')")>
    Public Function QuerySql(ByVal sqlQuery As String) As Boolean
        _mQuery = sqlQuery
        Query(_mQuery, _mResult)
        If _mResult.Rows.Count > 0 Then
            'Table gathered
            Return True
        Else
            'Table dosent exist
            Return False
        End If
    End Function
#End Region
    '#Region "SQL Get [STRING], [DataTable]"
    '    <Description("SQLGet. Used after the query to get a section value")>
    '    Public Function GetSql(ByVal tableSection As String) As String
    '        Return (mResult.Rows(0).Item(tableSection)).ToString
    '    End Function
    '    Public Function GetDataTableSql() As DataTable
    '        Return mResult
    '    End Function
    '#End Region
#Region "Insert Wraper"
    <Description("SQLInsert. EG.: (INSERT INTO db_textpage (pageid, text, nextpageid, wdbversion, checksum) VALUES ('pageid DWORD', 'pagetext STRING', 'nextpage DWORD', 'version DWORD', 'checksum DWORD')")>
    Public Sub InsertSql(ByVal sqlInsertionQuery As String)
        Insert(sqlInsertionQuery)
    End Sub
#End Region
    '#Region "Update Wraper"
    '    <Description("SQLUpdate. EG.: (UPDATE db_textpage SET pagetext='pagetextstring' WHERE pageid = 'pageiddword';")>
    '    Public Sub UpdateSQL(ByVal SQLUpdateQuery As String)
    '        Update(SQLUpdateQuery)
    '    End Sub
    '#End Region
#End Region

#Region "Main SQL Functions Used."
#Region "Query      MySQL|MSSQL|Oracle Supported       [SELECT * FROM db_accounts WHERE account = 'name';']"
    Public Function Query(ByVal sqlquery As String, ByRef result As DataTable) As Integer
        If result Is Nothing Then
            Throw New ArgumentNullException(NameOf(result))
        End If

        Select Case _vSqlType
            Case DbType.MySql
                If _mySqlConn.State <> ConnectionState.Open Then
                    Restart()
                    If _mySqlConn.State <> ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdError, "MySQL Database Request Failed!")
                        Return ReturnState.MinorError
                    End If
                End If
            Case DbType.Mssql
                If _mssqlConn.State <> ConnectionState.Open Then
                    Restart()
                    If _mssqlConn.State <> ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdError, "MSSQL Database Request Failed!")
                        Return ReturnState.MinorError
                    End If
                End If
                'Case DB_Type.Oracle
                '    If OracleConn.State <> ConnectionState.Open Then
                '        Restart()
                '        If OracleConn.State <> ConnectionState.Open Then
                '            RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Database Request Failed!")
                '            Exit Sub
                '        End If
                '    End If
        End Select

        Dim exitCode As Integer = ReturnState.Success
        Try
            Select Case _vSqlType
                Case DbType.MySql
                    Monitor.Enter(_mySqlConn)
                    Dim mySqlCommand As New MySqlCommand(sqlquery, _mySqlConn)
                    Dim mySqlAdapter As New MySqlDataAdapter(mySqlCommand)

                    If result Is Nothing Then
                        result = New DataTable
                    Else
                        result.Clear()
                    End If
                    mySqlAdapter.Fill(result)

                Case DbType.Mssql
                    Monitor.Enter(_mssqlConn)
                    Dim mssqlCommand As SqlCommand = New SqlCommand(sqlquery, _mssqlConn)
                    Dim mssqlAdapter As New SqlDataAdapter(mssqlCommand)

                    If result Is Nothing Then
                        result = New DataTable
                    Else
                        result.Clear()
                    End If
                    mssqlAdapter.Fill(result)

                    'Case DB_Type.Oracle
                    '    Monitor.Enter(OracleConn)
                    '    Dim OracleCommand As OracleCommand = New OracleCommand(sqlquery, OracleConn)
                    '    Dim OracleAdapter As New OracleDataAdapter(OracleCommand)

                    '    If Result Is Nothing Then
                    '        Result = New DataTable
                    '    Else
                    '        Result.Clear()
                    '    End If
                    '    OracleAdapter.Fill(Result)
            End Select

        Catch e As MySqlException
            RaiseEvent SqlMessage(EMessages.IdError, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SqlMessage(EMessages.IdError, "Query string was: " & sqlquery)
            exitCode = ReturnState.FatalError
        Catch e As SqlException
            RaiseEvent SqlMessage(EMessages.IdError, "Error Reading From MSSQL Database " & e.Message)
            RaiseEvent SqlMessage(EMessages.IdError, "Query string was: " & sqlquery)
            exitCode = ReturnState.FatalError
            'Catch e As OracleException
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From Oracle Database " & e.Message)
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Finally
            Select Case _vSqlType
                Case DbType.MySql
                    Monitor.Exit(_mySqlConn)
                Case DbType.Mssql
                    Monitor.Exit(_mssqlConn)
                    'Case DB_Type.Oracle
                    '    Monitor.Exit(OracleConn)
            End Select
        End Try
        Return exitCode
    End Function
#End Region
#Region "Insert     MySQL|MSSQL|Oracle Supported       [INSERT INTO db_textpage (pageid, text, nextpageid, wdbversion, checksum) VALUES ('pageid DWORD', 'pagetext STRING', 'nextpage DWORD', 'version DWORD', 'checksum DWORD']"
    Public Sub Insert(ByVal sqlquery As String)
        Select Case _vSqlType
            Case DbType.MySql
                If _mySqlConn.State <> ConnectionState.Open Then
                    Restart()
                    If _mySqlConn.State <> ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdError, "MySQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DbType.Mssql
                If _mssqlConn.State <> ConnectionState.Open Then
                    Restart()
                    If _mssqlConn.State <> ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdError, "MSSQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
                'Case DB_Type.Oracle
                '    If OracleConn.State <> ConnectionState.Open Then
                '        Restart()
                '        If OracleConn.State <> ConnectionState.Open Then
                '            RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Database Request Failed!")
                '            Exit Sub
                '        End If
                '    End If
        End Select

        Try
            Select Case _vSqlType
                Case DbType.MySql
                    Monitor.Enter(_mySqlConn)
                    Dim mySqlTransaction As MySqlTransaction = _mySqlConn.BeginTransaction
                    Dim mySqlCommand As New MySqlCommand(sqlquery, _mySqlConn, mySqlTransaction)

                    mySqlCommand.ExecuteNonQuery()
                    mySqlTransaction.Commit()
                    Console.WriteLine("transaction completed")

                Case DbType.Mssql
                    Monitor.Enter(_mssqlConn)
                    Dim mssqlTransaction As SqlTransaction = _mssqlConn.BeginTransaction()
                    Dim mssqlCommand As New SqlCommand(sqlquery, _mssqlConn, mssqlTransaction)

                    mssqlCommand.ExecuteNonQuery()
                    mssqlTransaction.Commit()
                    Console.WriteLine("transaction completed")

                    'Case DB_Type.Oracle
                    '    Monitor.Enter(OracleConn)
                    '    Dim OracleTransaction As OracleTransaction = OracleConn.BeginTransaction()
                    '    Dim OracleCommand As New OracleCommand(sqlquery, OracleConn, OracleTransaction)

                    '    OracleCommand.ExecuteNonQuery()
                    '    OracleTransaction.Commit()
                    '    Console.WriteLine("transaction completed")
            End Select

        Catch e As MySqlException
            RaiseEvent SqlMessage(EMessages.IdError, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SqlMessage(EMessages.IdError, "Insert string was: " & sqlquery)
        Catch e As SqlException
            RaiseEvent SqlMessage(EMessages.IdError, "Error Reading From MSSQL Database " & e.Message)
            RaiseEvent SqlMessage(EMessages.IdError, "Query string was: " & sqlquery)
            'Catch e As OracleException
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From Oracle Database " & e.Message)
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Finally
            Select Case _vSqlType
                Case DbType.MySql
                    Monitor.Exit(_mySqlConn)
                Case DbType.Mssql
                    Monitor.Exit(_mssqlConn)
                    'Case DB_Type.Oracle
                    '    Monitor.Exit(OracleConn)
            End Select
        End Try
    End Sub
#End Region
#Region "Update     MySQL|MSSQL|Oracle Supported       [UPDATE db_textpage SET pagetext='pagetextstring' WHERE pageid = 'pageiddword';]"
    Public Sub Update(ByVal sqlquery As String)
        Select Case _vSqlType
            Case DbType.MySql
                If _mySqlConn.State <> ConnectionState.Open Then
                    Restart()
                    If _mySqlConn.State <> ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdError, "MySQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
            Case DbType.Mssql
                If _mssqlConn.State <> ConnectionState.Open Then
                    Restart()
                    If _mssqlConn.State <> ConnectionState.Open Then
                        RaiseEvent SqlMessage(EMessages.IdError, "MSSQL Database Request Failed!")
                        Exit Sub
                    End If
                End If
                'Case DB_Type.Oracle
                '    If OracleConn.State <> ConnectionState.Open Then
                '        Restart()
                '        If OracleConn.State <> ConnectionState.Open Then
                '            RaiseEvent SQLMessage(EMessages.ID_Error, "Oracle Database Request Failed!")
                '            Exit Sub
                '        End If
                '    End If
        End Select

        Try
            Select Case _vSqlType
                Case DbType.MySql
                    Monitor.Enter(_mySqlConn)
                    Dim mySqlCommand As New MySqlCommand(sqlquery, _mySqlConn)
                    Dim mySqlAdapter As New MySqlDataAdapter(mySqlCommand)

                    Dim result As New DataTable
                    mySqlAdapter.Fill(result)

                Case DbType.Mssql
                    Monitor.Enter(_mssqlConn)
                    Dim mssqlCommand As New SqlCommand(sqlquery, _mssqlConn)
                    Dim mssqlAdapter As New SqlDataAdapter(mssqlCommand)

                    Dim result As New DataTable
                    mssqlAdapter.Fill(result)

                    'Case DB_Type.Oracle
                    '    Monitor.Enter(OracleConn)
                    '    Dim OracleCommand As New OracleCommand(sqlquery, OracleConn)
                    '    Dim OracleAdapter As New OracleDataAdapter(OracleCommand)

                    '    Dim result As New DataTable
                    '    OracleAdapter.Fill(result)
            End Select

        Catch e As MySqlException
            RaiseEvent SqlMessage(EMessages.IdError, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SqlMessage(EMessages.IdError, "Update string was: " & sqlquery)
        Catch e As SqlException
            RaiseEvent SqlMessage(EMessages.IdError, "Error Reading From MSSQL Database " & e.Message)
            RaiseEvent SqlMessage(EMessages.IdError, "Query string was: " & sqlquery)
            'Catch e As OracleException
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From Oracle Database " & e.Message)
            '    RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
        Finally
            Select Case _vSqlType
                Case DbType.MySql
                    Monitor.Exit(_mySqlConn)
                Case DbType.Mssql
                    Monitor.Exit(_mssqlConn)
                    'Case DB_Type.Oracle
                    '    Monitor.Exit(OracleConn)
            End Select
        End Try
    End Sub
#End Region
#End Region

End Class
