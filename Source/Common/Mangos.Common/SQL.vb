'
' Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
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

Imports System.ComponentModel
Imports System.Threading
Imports MySql.Data.MySqlClient
Imports System.Data

Public Class SQL
    Implements IDisposable

    Private MySQLConn As MySqlConnection

#Region "Events and event ID's"
    Public Enum EMessages
        ID_Error = 0
        ID_Message = 1
    End Enum

    Public Event SQLMessage(ByVal MessageID As EMessages, ByVal OutBuf As String)
#End Region
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
    Private v_SQLHost As String = "localhost"
    Private v_SQLPort As String = "3306"
    Private v_SQLUser As String = ""
    Private v_SQLPass As String = ""
    Private v_SQLDBName As String = ""
    Public Enum DB_Type
        MySQL = 0
    End Enum

    Public Enum ReturnState
        Success = 0
        MinorError = 1
        FatalError = 2
    End Enum

    Private v_SQLType As DB_Type

#Region "Main propertys"
#Region "Server type selection      MySQL Supported"
    <Description("SQL Server selection.")>
    Public Property SQLTypeServer As DB_Type
        Get
            SQLTypeServer = v_SQLType
        End Get
        Set
            v_SQLType = Value
        End Set
    End Property
#End Region
#Region "Server host ip"
    <Description("SQL Host name.")>
    Public Property SQLHost As String
        Get
            SQLHost = v_SQLHost
        End Get
        Set
            v_SQLHost = Value
        End Set
    End Property
#End Region
#Region "Server host port"
    <Description("SQL Host port.")>
    Public Property SQLPort As String
        Get
            SQLPort = v_SQLPort
        End Get
        Set
            v_SQLPort = Value
        End Set
    End Property
#End Region
#Region "Server username"
    <Description("SQL User name.")>
    Public Property SQLUser As String
        Get
            SQLUser = v_SQLUser
        End Get
        Set
            v_SQLUser = Value
        End Set
    End Property
#End Region
#Region "Server Password"
    <Description("SQL Password.")>
    Public Property SQLPass As String
        Get
            SQLPass = v_SQLPass
        End Get
        Set
            v_SQLPass = Value
        End Set
    End Property
#End Region
#Region "Database Name"
    <Description("SQL Database name.")>
    Public Property SQLDBName As String
        Get
            SQLDBName = v_SQLDBName
        End Get
        Set
            v_SQLDBName = Value
        End Set
    End Property
#End Region
#End Region
#Region "Main Functions"
#Region "Connect()                  MySQL Supported"
    <Description("Start up the SQL connection.")> _
    Public Function Connect() As Integer
        Try
            If SQLHost.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLHost cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLPort.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLPort cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLUser.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLUser cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLPass.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLPassword cannot be empty")
                Return ReturnState.FatalError
            End If
            If SQLDBName.Length < 1 Then
                RaiseEvent SQLMessage(EMessages.ID_Error, "You have to set the SQLDatabaseName cannot be empty")
                Return ReturnState.FatalError
            End If

            Select Case v_SQLType
                Case DB_Type.MySQL
                    MySQLConn = New MySqlConnection(String.Format("Server={0};Port={4};User ID={1};Password={2};Database={3};Compress=false;Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    MySQLConn.Open()
                    RaiseEvent SQLMessage(EMessages.ID_Message, "MySQL Connection Opened Successfully [" & SQLUser & "@" & SQLHost & "]")
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Connection Error [" & e.Message & "]")
            Return ReturnState.FatalError
        End Try
        Return ReturnState.Success
    End Function
#End Region
#Region "Restart()                  MySQL Supported"
    <Description("Restart the SQL connection.")> _
    Public Sub Restart()
        Try
            Select Case v_SQLType
                Case DB_Type.MySQL
                    MySQLConn.Close()
                    MySQLConn.Dispose()
                    MySQLConn = New MySqlConnection(String.Format("Server={0};Port={4};User ID={1};Password={2};Database={3};Compress=false;Connection Timeout=1;", SQLHost, SQLUser, SQLPass, SQLDBName, SQLPort))
                    MySQLConn.Open()

                    If MySQLConn.State = ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Message, "MySQL Connection restarted!")
                    Else
                        RaiseEvent SQLMessage(EMessages.ID_Error, "Unable to restart MySQL connection.")
                    End If
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Connection Error [" & e.Message & "]")
        End Try
    End Sub
#End Region

#Region "IDisposable Support"
    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    <Description("Close file and dispose the wdb reader.")> _
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            Select Case v_SQLType
                Case DB_Type.MySQL
                    MySQLConn.Close()
                    MySQLConn.Dispose()
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
#Region "Query      MySQL Supported       [SELECT * FROM db_accounts WHERE account = 'name';']"
    Public Function Query(ByVal sqlquery As String, ByRef Result As DataTable) As Integer
        Select Case v_SQLType
            Case DB_Type.MySQL
                If MySQLConn.State <> ConnectionState.Open Then
                    Restart()
                    If MySQLConn.State <> ConnectionState.Open Then
                        RaiseEvent SQLMessage(EMessages.ID_Error, "MySQL Database Request Failed!")
                        Return ReturnState.MinorError
                    End If
                End If
        End Select

        Dim ExitCode As Integer = ReturnState.Success
        Try
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Enter(MySQLConn)
                    Dim MySQLCommand As New MySqlCommand(sqlquery, MySQLConn)
                    Dim MySQLAdapter As New MySqlDataAdapter(MySQLCommand)

                    If Result Is Nothing Then
                        Result = New DataTable
                    Else
                        Result.Clear()
                    End If
                    MySQLAdapter.Fill(Result)
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Query string was: " & sqlquery)
            ExitCode = ReturnState.FatalError
        Finally
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Exit(MySQLConn)
            End Select
        End Try
        Return ExitCode
    End Function
#End Region
#Region "Insert     MySQL       [INSERT INTO db_textpage (pageid, text, nextpageid, wdbversion, checksum) VALUES ('pageid DWORD', 'pagetext STRING', 'nextpage DWORD', 'version DWORD', 'checksum DWORD']"
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
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Insert string was: " & sqlquery)
        Finally
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Exit(MySQLConn)
            End Select
        End Try
    End Sub

    'TODO: Apply proper implementation as needed
    Public Function TableInsert(tablename As String, dbField1 As String, dbField1Value As String, dbField2 As String, dbField2Value As Integer) As Integer
        Dim cmd As New MySqlCommand("", MySQLConn)
        cmd.Connection.Open()
        cmd.CommandText = "insert into `" & tablename & "`(`" & dbField1 & "`,`" & dbField2 & "`) " &
                                          "VALUES (@field1value, @field2value)"

        cmd.Parameters.AddWithValue("@field1value", dbField1Value)
        cmd.Parameters.AddWithValue("@field2value", dbField2Value)

        Try
            cmd.ExecuteScalar()
            cmd.Connection.Close()
            Return 0
        Catch ex As Exception
            cmd.Connection.Close()
            Return -1
        End Try
    End Function

    'TODO: Apply proper implementation as needed
    Public Function TableSelect(tablename As String, returnfields As String, dbField1 As String, dbField1Value As String) As DataSet
        Dim cmd As New MySqlCommand("", MySQLConn)
        cmd.Connection.Open()
        cmd.CommandText = "select " & returnfields & " FROM `" & tablename & "` WHERE `" & dbField1 & "` = '@dbField1value';"

        cmd.Parameters.AddWithValue("@dbfield1value", dbField1Value)

        Try
            Dim adapter As New MySqlDataAdapter()
            Dim myDataset As New DataSet()
            adapter.SelectCommand = cmd

            adapter.Fill(myDataset)
            cmd.ExecuteScalar()
            cmd.Connection.Close()
            Return myDataset
        Catch ex As Exception
            cmd.Connection.Close()
            Return Nothing
        End Try
    End Function

#End Region
#Region "Update     MySQL Supported       [UPDATE db_textpage SET pagetext='pagetextstring' WHERE pageid = 'pageiddword';]"
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
        End Select

        Try
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Enter(MySQLConn)
                    Dim MySQLCommand As New MySqlCommand(sqlquery, MySQLConn)
                    Dim MySQLAdapter As New MySqlDataAdapter(MySQLCommand)

                    Dim result As New DataTable
                    MySQLAdapter.Fill(result)
            End Select

        Catch e As MySqlException
            RaiseEvent SQLMessage(EMessages.ID_Error, "Error Reading From MySQL Database " & e.Message)
            RaiseEvent SQLMessage(EMessages.ID_Error, "Update string was: " & sqlquery)
        Finally
            Select Case v_SQLType
                Case DB_Type.MySQL
                    Monitor.Exit(MySQLConn)
            End Select
        End Try
    End Sub
#End Region
#End Region

End Class
