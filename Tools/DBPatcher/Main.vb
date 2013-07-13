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

Imports SpuriousZero.Common
Imports System.IO
Imports System.Reflection

Module Main
    Public Database As New SQL
    Public FailMsg As String = ""

    Public Sub SLQEventHandler(ByVal MessageID As SQL.EMessages, ByVal OutBuf As String)
        Select Case MessageID
            Case SQL.EMessages.ID_Error
                If FailMsg <> "" Then FailMsg &= vbNewLine
                FailMsg &= OutBuf
        End Select
    End Sub

#Region "Main"
    Sub Main()
        AddHandler Database.SQLMessage, AddressOf SLQEventHandler

        Dim fs As FileStream = Nothing
        Dim fs2 As FileStream = Nothing

        Try
            Console.BackgroundColor = System.ConsoleColor.Black
            Console.Title = String.Format("{0} v{1}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title, [Assembly].GetExecutingAssembly().GetName().Version)

            Console.ForegroundColor = System.ConsoleColor.Yellow
            Console.WriteLine("{0}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyProductAttribute), False)(0), AssemblyProductAttribute).Product)
            Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)(0), AssemblyCopyrightAttribute).Copyright)
            Console.WriteLine()

            Console.ForegroundColor = System.ConsoleColor.Magenta
            Console.WriteLine("http://spuriousemu.com")
            Console.WriteLine()

            'DONE: Get all the sql info
            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("SQL Host [localhost]: ")
            Console.ForegroundColor = ConsoleColor.Cyan
            Dim SQLHost As String = Console.ReadLine()
            If SQLHost = "" Then SQLHost = "localhost"

            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("SQL Port [3306]: ")
            Console.ForegroundColor = ConsoleColor.Cyan
            Dim SQLPortDefault As String = Console.ReadLine()
            If SQLPortDefault = "" Then SQLPortDefault = "3306"
            Dim SQLPort As Integer = CType(SQLPortDefault, Integer)

            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("SQL User [root]: ")
            Console.ForegroundColor = ConsoleColor.Cyan
            Dim SQLUser As String = Console.ReadLine()
            If SQLUser = "" Then SQLUser = "root"

            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("SQL Pass [root]: ")
            Console.ForegroundColor = ConsoleColor.Cyan
            Dim SQLPass As String = Console.ReadLine()
            If SQLPass = "" Then SQLPass = "root"

            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("SQL Database [vwow]: ")
            Console.ForegroundColor = ConsoleColor.Cyan
            Dim SQLSourceDB As String = Console.ReadLine()
            If SQLSourceDB = "" Then SQLSourceDB = "vwow"

            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("Old Database File [vwow.sql]: ")
            Console.ForegroundColor = ConsoleColor.Cyan
            Dim DBFile As String = Console.ReadLine()
            If DBFile = "" Then DBFile = "vwow.sql"

            Console.ForegroundColor = ConsoleColor.Green
            Console.Write("Ignore data in tables [account,character,realms]: ")
            Console.ForegroundColor = ConsoleColor.Cyan
            Dim IgnoreDataInTables As String = Console.ReadLine()

            Dim TablesToIgnoreDataIn As New List(Of String)
            Dim IgnoreTableSplit() As String = IgnoreDataInTables.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
            For Each table As String In IgnoreTableSplit
                TablesToIgnoreDataIn.Add(table)
            Next

            'DONE: Setup the DB
            Database.SQLTypeServer = SQL.DB_Type.MySQL
            Database.SQLHost = SQLHost
            Database.SQLPort = SQLPort
            Database.SQLUser = SQLUser
            Database.SQLPass = SQLPass
            Database.SQLDBName = SQLSourceDB

            'DONE: Connect to the DB
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.Write("Connecting to databases... ")
            Database.Connect()
            If FailMsg <> "" Then
                Console.WriteLine("Failed.")
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(FailMsg)
                FailMsg = ""
            End If
            Console.WriteLine("Done.")

            Console.Write("Opening file... ")
            If File.Exists(DBFile) = False Then
                Console.WriteLine("Failed.")
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(DBFile & " does not exist.")
                GoTo ExitNow
            End If
            fs = New FileStream(DBFile, FileMode.Open, FileAccess.Read, FileShare.Read)
            Dim br As New IO.StreamReader(fs)
            fs.Seek(0, SeekOrigin.Begin)
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Done.")

            Console.Write("Creating file... ")
            fs2 = New FileStream("patch.sql", FileMode.Create, FileAccess.Write, FileShare.Read)
            Dim bw As New IO.StreamWriter(fs2)
            bw.AutoFlush = True
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Done.")

            bw.WriteLine("/*")
            bw.WriteLine("Auto-Generated patch file")
            bw.WriteLine("Made for SpuriousZero by UniX")
            bw.WriteLine("Date: " & Now)
            bw.WriteLine("*/")
            bw.WriteLine("")
            bw.WriteLine("SET FOREIGN_KEY_CHECKS=0;")
            bw.WriteLine("")

            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Checking for removed or changed tables.")

            Dim TablePos As New Dictionary(Of String, Long)
            Dim FoundTables As New Dictionary(Of String, List(Of String))
            Dim TableKeys As New Dictionary(Of String, List(Of String))
            Dim TableDefs As New Dictionary(Of String, String)
            Dim tmpLine As String = ""
            Dim TableName As String = ""
            Dim ColumnName As String = ""
            Dim tmpPos As Long = 0
            Do While br.EndOfStream = False
                tmpPos = br.BaseStream.Position
                tmpLine = br.ReadLine()

                If tmpLine.StartsWith("CREATE TABLE ") OrElse tmpLine.StartsWith("create table ") Then
                    TableName = GetTableName(tmpLine)
                    TablePos.Add(TableName, tmpPos)
                    FoundTables.Add(TableName, New List(Of String))
                    TableKeys.Add(TableName, New List(Of String))
                    TableDefs.Add(TableName, "")

                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write("Table: " & TableName & " - ")

                    'DONE: Remove old tables
                    If TableExists(TableName) = False Then
                        Console.WriteLine("Removed.")
                        bw.WriteLine("DROP TABLE IF EXISTS `{0}`;", TableName)
                        Continue Do
                    End If

                    Dim Changed As Boolean = False
                    Dim tmpLine2 As String = ""
                    Do While br.EndOfStream = False
                        tmpLine2 = br.ReadLine()
                        If tmpLine2.IndexOf(" KEY ") >= 0 Then
                            TableKeys(TableName).Add(tmpLine2)
                            Continue Do
                        ElseIf tmpLine2.IndexOf(";") > 0 Then
                            TableDefs(TableName) = tmpLine2
                            Exit Do
                        End If
                        ColumnName = GetTableName(tmpLine2)
                        FoundTables(TableName).Add(ColumnName)

                        'DONE: Remove old columns
                        If ColumnExists(TableName, ColumnName) = False Then
                            bw.WriteLine("ALTER TABLE `{0}` DROP COLUMN `{1}` CASCADE;", TableName, ColumnName)
                            Changed = True
                            Continue Do
                        End If

                        'DONE: Modify old columns
                        If CompareColumn(bw, TableName, ColumnName, tmpLine2) Then
                            Changed = True
                        End If
                    Loop

                    If Not Changed Then
                        Console.ForegroundColor = ConsoleColor.DarkCyan
                        Console.WriteLine("No change.")
                    Else
                        Console.ForegroundColor = ConsoleColor.Blue
                        Console.WriteLine("Changed.")
                    End If
                End If
            Loop

            'DONE: Modify table keys
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Checking for key changes in tables.")

            Dim KeyChanges As String = "" 'We will store key changes in this string to execute after data updates
            'This is because we want to drop our keys first, then update and THEN add the keys again
            'This way there will be no conflictions updating the data

            Dim result As New DataTable
            Database.Query("SHOW TABLES", result)
            For Each row As DataRow In result.Rows
                If FoundTables.ContainsKey(row.Item(0)) = False Then Continue For

                Dim primaryKeys As List(Of String) = Nothing
                Dim normalKeys As Dictionary(Of String, List(Of String)) = Nothing
                Dim uniqueKeys As Dictionary(Of String, List(Of String)) = Nothing
                GetKeys(row.Item(0), primaryKeys, normalKeys, uniqueKeys)
                Dim keyName As String = ""
                Dim PrimaryKeyChange As Boolean = False

                Dim PrimaryKeyCount As Integer = 0
                For Each key As String In TableKeys(row.Item(0))
                    If key.IndexOf("PRIMARY") <> -1 Then
                        PrimaryKeyCount += 1
                        If PrimaryKeyChange Then Continue For
                        'DONE: Check if a primary key is removed
                        keyName = GetTableName(key)
                        Dim keyParts As List(Of String) = GetKeyParts(key)
                        For Each part As String In keyParts
                            If primaryKeys.Contains(part) = False Then
                                'Console.WriteLine("Primary key not found: {0}", part)
                                PrimaryKeyChange = True
                                Exit For
                            End If
                        Next
                    ElseIf key.IndexOf("UNIQUE") <> -1 Then
                        keyName = GetTableName(key)
                        'DONE: Check if a unique key is removed
                        If uniqueKeys.ContainsKey(keyName) = False Then
                            'Console.WriteLine("Key not found: {0}", keyName)
                            bw.WriteLine("ALTER TABLE `{0}` DROP INDEX `{1}`;", row.Item(0), keyName)
                        End If
                    Else
                        keyName = GetTableName(key)
                        'DONE: Check if a unique key is removed
                        If normalKeys.ContainsKey(keyName) = False Then
                            'Console.WriteLine("Key not found: {0}", keyName)
                            bw.WriteLine("ALTER TABLE `{0}` DROP INDEX `{1}`;", row.Item(0), keyName)
                        End If
                    End If
                Next
                If PrimaryKeyChange = False Then
                    'DONE: Check if a primary key has been added
                    For Each key As String In primaryKeys
                        Dim foundIt As Boolean = False
                        For Each key2 As String In TableKeys(row.Item(0))
                            If key2.IndexOf("PRIMARY") = -1 Then Continue For
                            Dim tmpParts As List(Of String) = GetKeyParts(key2)
                            For Each part As String In tmpParts
                                If key = part Then
                                    foundIt = True
                                    Exit For
                                End If
                            Next
                        Next
                        If foundIt = False Then
                            'Console.WriteLine("Primary to be added: {0}", key)
                            PrimaryKeyChange = True
                            Exit For
                        End If
                    Next
                End If
                'DONE: Check if a unique key has been added
                For Each key As KeyValuePair(Of String, List(Of String)) In uniqueKeys
                    Dim foundIt As Boolean = False
                    For Each key2 As String In TableKeys(row.Item(0))
                        If key2.IndexOf("UNIQUE") = -1 Then Continue For
                        keyName = GetTableName(key2)
                        If key.Key = keyName Then
                            foundIt = True

                            'DONE: Check if the key is changed
                            Dim keyParts As List(Of String) = GetKeyParts(key2)
                            For Each part As String In key.Value
                                If keyParts.Contains(part) = False Then
                                    bw.WriteLine("ALTER TABLE `{0}` DROP INDEX `{1}`;", row.Item(0), keyName) 'First drop it
                                    foundIt = False 'And then we pretend we didn't find it so that it is added later
                                End If
                            Next
                            For Each part As String In keyParts
                                If key.Value.Contains(part) = False Then
                                    bw.WriteLine("ALTER TABLE `{0}` DROP INDEX `{1}`;", row.Item(0), keyName) 'First drop it
                                    foundIt = False 'And then we pretend we didn't find it so that it is added later
                                End If
                            Next
                            Exit For
                        End If
                    Next
                    If foundIt = False Then
                        'DONE: Add new unique key
                        KeyChanges &= String.Format("ALTER TABLE `{0}` ADD UNIQUE INDEX `{1}` (", row.Item(0), key.Key)
                        Dim isFirst As Boolean = True
                        For Each part As String In key.Value
                            If isFirst = False Then KeyChanges &= ","
                            KeyChanges &= "`" & part & "`"
                            isFirst = False
                        Next
                        KeyChanges &= ");"
                    End If
                Next
                'DONE: Check if a normal key has been added
                For Each key As KeyValuePair(Of String, List(Of String)) In normalKeys
                    Dim foundIt As Boolean = False
                    For Each key2 As String In TableKeys(row.Item(0))
                        If key2.IndexOf("PRIMARY") <> -1 OrElse key2.IndexOf("UNIQUE") <> -1 Then Continue For
                        keyName = GetTableName(key2)
                        If key.Key = keyName Then
                            foundIt = True

                            'DONE: Check if the key is changed
                            Dim keyParts As List(Of String) = GetKeyParts(key2)
                            For Each part As String In key.Value
                                If keyParts.Contains(part) = False Then
                                    bw.WriteLine("ALTER TABLE `{0}` DROP INDEX `{1}`;", row.Item(0), keyName) 'First drop it
                                    foundIt = False 'And then we pretend we didn't find it so that it is added later
                                End If
                            Next
                            For Each part As String In keyParts
                                If key.Value.Contains(part) = False Then
                                    bw.WriteLine("ALTER TABLE `{0}` DROP INDEX `{1}`;", row.Item(0), keyName) 'First drop it
                                    foundIt = False 'And then we pretend we didn't find it so that it is added later
                                End If
                            Next
                            Exit For
                        End If
                    Next
                    If foundIt = False Then
                        'DONE: Add new normal key
                        KeyChanges &= String.Format("ALTER TABLE `{0}` ADD INDEX `{1}` (", row.Item(0), key.Key)
                        Dim isFirst As Boolean = True
                        For Each part As String In key.Value
                            If isFirst = False Then KeyChanges &= ","
                            KeyChanges &= "`" & part & "`"
                            isFirst = False
                        Next
                        KeyChanges &= ");"
                    End If
                Next

                'DONE: If primary keys has changed
                If PrimaryKeyChange Then
                    If PrimaryKeyCount > 0 Then bw.WriteLine("ALTER TABLE `{0}` DROP PRIMARY KEY;", row.Item(0)) 'First drop all primary keys
                    'Then save the key changes until later
                    KeyChanges &= String.Format("ALTER TABLE `{0}` ADD PRIMARY KEY (", row.Item(0))
                    Dim isFirst As Boolean = True
                    For Each key As String In primaryKeys
                        If isFirst = False Then KeyChanges &= ","
                        KeyChanges &= "`" & key & "`"
                        isFirst = False
                    Next
                    KeyChanges &= ");" & vbNewLine
                End If
            Next

            'DONE: Add new tables
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Checking for added tables.")

            For Each row As DataRow In result.Rows
                If FoundTables.ContainsKey(row.Item(0)) = False Then
                    Console.ForegroundColor = ConsoleColor.Blue
                    Console.WriteLine("Found new table: " & row.Item(0))

                    CreateTable(bw, row.Item(0))
                End If
            Next

            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Checking for changed data in tables.")

            For Each row As DataRow In result.Rows
                If FoundTables.ContainsKey(row.Item(0)) Then
                    Console.ForegroundColor = ConsoleColor.Gray
                    Console.WriteLine("Looking in: " & row.Item(0))

                    Dim ignored As Boolean = False
                    For Each table As String In TablesToIgnoreDataIn
                        If CStr(row.Item(0)).Contains(table) Then
                            Console.ForegroundColor = ConsoleColor.Red
                            Console.WriteLine("Ignored.")
                            ignored = True
                        End If
                    Next
                    If Not ignored Then
                        ControlTable(DBFile, bw, row.Item(0), TablePos(row.Item(0)))
                        GC.Collect()
                        Threading.Thread.Sleep(50)
                    End If
                End If
            Next

            'DONE: Now write the key changes
            bw.WriteLine(KeyChanges)

ExitNow:
        Catch e As Exception
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("Error.")
            Console.WriteLine(e.ToString)
        End Try

        If fs IsNot Nothing Then
            fs.Close()
            fs.Dispose()
            fs = Nothing

            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Closed input.")
        End If

        If fs2 IsNot Nothing Then
            fs2.Close()
            fs2.Dispose()
            fs2 = Nothing

            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("Closed output.")
        End If

        Console.ForegroundColor = ConsoleColor.DarkMagenta
        Console.WriteLine("Press any key to close this window.")
        Console.ReadKey()
    End Sub
#End Region

#Region "Small Functions"
    Public Function GetTableName(ByVal s As String) As String
        Dim index1 As Integer = s.IndexOf("`"c)
        If index1 = -1 Then Return ""
        Dim index2 As Integer = s.IndexOf("`"c, index1 + 1)
        If index2 = -1 Then Return ""
        Return s.Substring(index1 + 1, index2 - index1 - 1)
    End Function

    Public Function GetColumns(ByVal s As String) As List(Of String)
        Dim newList As New List(Of String)
        Dim index As Integer = s.IndexOf("`"c)
        If index = -1 Then Return newList
        index = s.IndexOf("`"c, index + 1)
        If index = -1 Then Return newList
        index = s.IndexOf("(", index + 1)
        If index = -1 Then Return newList
        Dim index2 As Integer = s.IndexOf(")", index + 1)
        If index2 = -1 Then Return newList
        Dim tmpStr As String = s.Substring(index + 1, index2 - index - 1)

        Dim sSplit() As String = tmpStr.Replace("`", "").Split(",")
        If sSplit.Length = 0 Then
            newList.Add(tmpStr)
        Else
            For Each str As String In sSplit
                newList.Add(str)
            Next
        End If

        Return newList
    End Function

    Public Function TableExists(ByVal table As String) As Boolean
        Dim result As New DataTable
        Database.Query("SHOW TABLES;", result)
        For Each tableRow As DataRow In result.Rows
            If CStr(tableRow.Item(0)) = table Then Return True
        Next
        Return False
    End Function

    Public Function ColumnExists(ByVal table As String, ByVal column As String) As Boolean
        Dim result As New DataTable
        Database.Query(String.Format("SHOW COLUMNS IN `{0}` WHERE Field = '{1}';", table, column), result)
        Return (result.Rows.Count > 0)
    End Function

    Public Function IsHex(ByVal str As String) As Boolean
        If str.StartsWith("0x") = False Then Return False
        For i As Integer = 0 To str.Length - 1
            If i > 10 Then Return True

            If (str(i) < "0"c OrElse str(i) > "9"c) AndAlso (str(i) < "A"c OrElse str(i) > "F"c) Then Return False
        Next

        Return True
    End Function

    Public Function ToHex(ByVal bBytes() As Byte) As String
        If bBytes.Length = 0 Then Return ""
        Dim tmpStr As String = "0x"
        For i As Integer = 0 To bBytes.Length - 1
            If bBytes(i) < 16 Then
                tmpStr &= "0" & Hex(bBytes(i))
            Else
                tmpStr &= Hex(bBytes(i))
            End If
        Next
        Return tmpStr
    End Function

    Public Function ToStr(ByVal bBytes() As Byte) As String
        If bBytes.Length = 0 Then Return ""
        Dim tmpStr As String = ""
        For i As Integer = 0 To bBytes.Length - 1
            If bBytes(i) = 0 Then
                tmpStr &= "\0"
            Else
                tmpStr &= Chr(bBytes(i))
            End If
        Next
        Return tmpStr

        'NOTE: The function below ignored "zero-terminators"
        'Return System.Text.Encoding.ASCII.GetString(bBytes)
    End Function
#End Region

#Region "Compare Columns"
    Public Function CompareColumn(ByRef bw As StreamWriter, ByVal table As String, ByVal column As String, ByVal str As String) As Boolean
        Dim result As New DataTable
        Database.Query(String.Format("SHOW COLUMNS IN `{0}` WHERE Field = '{1}';", table, column), result)
        Dim columnInfo As DataRow = result.Rows(0)

        If str.EndsWith(","c) Then str = str.Substring(0, str.Length - 1)
        Dim sSplit() As String = str.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)
        Dim ColumnHaveDefault As Boolean = False
        Dim ColumnDefault As String = ""
        Dim ColumnType As String = ""
        Dim ColumnAllowNull As Boolean = True
        Dim ColumnUnsigned As Boolean = False
        Dim ColumnExtra As String = ""

        Dim bNextType As Boolean = False
        Dim bNot As Boolean = False
        Dim bDefault As Boolean = False
        For Each part As String In sSplit
            If bNextType Then
                bNextType = False
                ColumnType = part
            ElseIf part.Length > 2 AndAlso part(0) = "`"c AndAlso part(part.Length - 1) = "`"c Then
                bNextType = True
            ElseIf part = "NOT" Then
                bNot = True
            ElseIf part = "NULL" Then
                If bNot Then
                    bNot = False
                    ColumnAllowNull = False
                Else
                    ColumnAllowNull = True
                End If
            ElseIf part.ToLower = "auto_increment" Then
                ColumnExtra &= part
            ElseIf part.ToLower = "unsigned" Then
                ColumnUnsigned = True
            ElseIf part.ToLower = "default" Then
                bDefault = True
            ElseIf bDefault Then
                If part.EndsWith("'"c) Then bDefault = False
                If ColumnDefault <> "" Then ColumnDefault &= " "
                ColumnDefault &= part.Replace("'", "")
                ColumnHaveDefault = True
            End If
        Next

        Dim Changed As Boolean = False
        If columnInfo.Item("Type") <> (ColumnType & If(ColumnUnsigned, " unsigned", "")) Then
            Changed = True
        End If
        If columnInfo.Item("Null") <> If(ColumnAllowNull, "YES", "NO") Then
            Changed = True
        End If
        If TypeOf columnInfo.Item("Default") Is DBNull Then
            If ColumnHaveDefault Then
                Changed = True
            End If
        Else
            If columnInfo.Item("Default") <> ColumnDefault Then
                Changed = True
            End If
        End If
        If CStr(columnInfo.Item("Extra")).ToLower <> ColumnExtra.ToLower Then
            Changed = True
        End If

        If Changed = False Then Return False

        Dim tmpStr As String = ""
        If columnInfo.Item("Null") = "YES" Then tmpStr = "NULL" Else tmpStr = "NOT NULL"
        If Not TypeOf columnInfo.Item("Default") Is DBNull Then
            tmpStr &= " default '" & columnInfo.Item("Default") & "'"
        End If
        If columnInfo.Item("Extra") <> "" Then tmpStr &= " " & columnInfo.Item("Extra")
        bw.WriteLine("ALTER TABLE `{0}` MODIFY `{1}` {2} {3};", table, column, columnInfo.Item("Type"), tmpStr)

        Return True
    End Function
#End Region

#Region "Get keys"
    Public Sub GetKeys(ByVal table As String, ByRef primaryKeys As List(Of String), ByRef normalKeys As Dictionary(Of String, List(Of String)), ByRef uniqueKeys As Dictionary(Of String, List(Of String)))
        Dim keyResult As New DataTable
        Database.Query(String.Format("SHOW INDEX FROM `{0}`;", table), keyResult)

        primaryKeys = New List(Of String)
        normalKeys = New Dictionary(Of String, List(Of String))
        uniqueKeys = New Dictionary(Of String, List(Of String))
        For Each keyRow As DataRow In keyResult.Rows
            If keyRow.Item("Key_name") = "PRIMARY" Then
                primaryKeys.Add(keyRow.Item("Column_name"))
            Else
                If keyRow.Item("Non_unique") = 0 Then
                    If uniqueKeys.ContainsKey(keyRow.Item("Key_name")) = False Then
                        uniqueKeys.Add(keyRow.Item("Key_name"), New List(Of String))
                    End If
                    uniqueKeys(keyRow.Item("Key_name")).Add(keyRow.Item("Column_name"))
                Else
                    If normalKeys.ContainsKey(keyRow.Item("Key_name")) = False Then
                        normalKeys.Add(keyRow.Item("Key_name"), New List(Of String))
                    End If
                    normalKeys(keyRow.Item("Key_name")).Add(keyRow.Item("Column_name"))
                End If
            End If
        Next
        keyResult.Clear()
    End Sub

    Public Function GetKeyParts(ByVal str As String) As List(Of String)
        Dim newList As New List(Of String)
        Dim index1 As Integer = str.IndexOf("(")
        If index1 = -1 Then Return newList
        Dim index2 As Integer = str.IndexOf(")", index1 + 1)
        If index2 = -1 Then Return newList

        str = str.Substring(index1 + 1, index2 - index1 - 1).Replace(" ", "").Replace("`", "")
        Dim sSplit() As String = str.Split(",")
        If sSplit.Length = 0 Then
            newList.Add(str)
        Else
            For Each tmpStr As String In sSplit
                newList.Add(tmpStr)
            Next
        End If

        Return newList
    End Function
#End Region

#Region "Create Table"
    Public Sub CreateTable(ByRef bw As StreamWriter, ByVal table As String)
        bw.WriteLine("")
        bw.WriteLine("/*Table structure for table `{0}` */", table)
        bw.WriteLine("")
        bw.WriteLine("DROP TABLE IF EXISTS `{0}`;", table)
        bw.WriteLine("CREATE TABLE `{0}` (", table)

        Dim tableResult As New DataTable
        Database.Query(String.Format("SELECT ENGINE, AUTO_INCREMENT, CREATE_OPTIONS, TABLE_COMMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' LIMIT 1;", Database.SQLDBName, table), tableResult)

        Dim primaryKeys As List(Of String) = Nothing
        Dim normalKeys As Dictionary(Of String, List(Of String)) = Nothing
        Dim uniqueKeys As Dictionary(Of String, List(Of String)) = Nothing
        GetKeys(table, primaryKeys, normalKeys, uniqueKeys)

        Dim result As New DataTable
        Database.Query(String.Format("SHOW COLUMNS IN `{0}`;", table), result)

        Dim Columns As New List(Of String)
        Dim isFirst As Boolean = True
        For Each row As DataRow In result.Rows
            If isFirst = False Then bw.WriteLine(",")
            Dim tmpStr As String = ""
            If row.Item("Null") = "YES" Then tmpStr = "NULL" Else tmpStr = "NOT NULL"
            If Not TypeOf row.Item("Default") Is DBNull Then
                tmpStr &= " default '" & row.Item("Default") & "'"
            End If
            If row.Item("Extra") <> "" Then tmpStr &= " " & row.Item("Extra")
            Columns.Add(row.Item("Field"))
            bw.Write("  `{0}` {1} {2}", row.Item("Field"), row.Item("Type"), tmpStr)
            isFirst = False
        Next
        If primaryKeys.Count > 0 OrElse uniqueKeys.Count > 0 OrElse normalKeys.Count > 0 Then bw.WriteLine(",") Else bw.WriteLine("")
        result.Clear()

        If primaryKeys.Count > 0 Then
            bw.Write("  PRIMARY KEY  (")
            isFirst = True
            For Each key As String In primaryKeys
                If isFirst = False Then bw.Write(",")
                bw.Write("`" & key & "`")
                isFirst = False
            Next
            bw.Write(")")
            If uniqueKeys.Count > 0 Then bw.WriteLine(",") Else bw.WriteLine("")
        End If

        If uniqueKeys.Count > 0 Then
            Dim isFirst2 As Boolean = True
            For Each key As KeyValuePair(Of String, List(Of String)) In uniqueKeys
                If isFirst2 = False Then bw.WriteLine(",")
                bw.Write("  UNIQUE KEY `" & key.Key & "` (")
                isFirst = True
                For Each tmpKey As String In key.Value
                    If isFirst = False Then bw.Write(",")
                    bw.Write("`" & tmpKey & "`")
                    isFirst = False
                Next
                bw.Write(")")
                isFirst2 = False
            Next
            If normalKeys.Count > 0 Then bw.WriteLine(",") Else bw.WriteLine("")
        End If

        If normalKeys.Count > 0 Then
            Dim isFirst2 As Boolean = True
            For Each key As KeyValuePair(Of String, List(Of String)) In normalKeys
                If isFirst2 = False Then bw.WriteLine(",")
                bw.Write("  KEY `" & key.Key & "` (")
                isFirst = True
                For Each tmpKey As String In key.Value
                    If isFirst = False Then bw.Write(",")
                    bw.Write("`" & tmpKey & "`")
                    isFirst = False
                Next
                bw.Write(")")
                isFirst2 = False
            Next
            bw.WriteLine("")
        End If

        Dim tmpStr2 As String = ""
        Dim tmpStr3 As String = ""
        If Not TypeOf tableResult.Rows(0).Item("AUTO_INCREMENT") Is DBNull Then tmpStr3 &= " AUTO_INCREMENT=" & tableResult.Rows(0).Item("AUTO_INCREMENT")
        If tableResult.Rows(0).Item("CREATE_OPTIONS") <> "" Then tmpStr2 &= " " & tableResult.Rows(0).Item("CREATE_OPTIONS")
        If tableResult.Rows(0).Item("TABLE_COMMENT") <> "" Then tmpStr2 &= " COMMENT='" & tableResult.Rows(0).Item("TABLE_COMMENT") & "'"
        bw.WriteLine(") ENGINE={0}{1} DEFAULT CHARSET=utf8{2};", tableResult.Rows(0).Item("ENGINE"), tmpStr3, tmpStr2)
        bw.WriteLine("")
        tableResult.Clear()

        bw.WriteLine("/*Data for the table `{0}` */", table)
        bw.WriteLine("")

        DumpTable(bw, table, Columns)

        bw.WriteLine("")
    End Sub
#End Region

#Region "Dump Table"
    Public Sub DumpTable(ByRef bw As StreamWriter, ByVal table As String, ByVal Columns As List(Of String))
        If Columns Is Nothing Then
            Dim result As New DataTable
            Database.Query(String.Format("SHOW COLUMNS IN `{0}`;", table), result)

            Columns = New List(Of String)
            For Each row As DataRow In result.Rows
                Columns.Add(row.Item("Field"))
            Next
            result.Clear()
        End If

        Dim valueResult As New DataTable
        Database.Query(String.Format("SELECT * FROM `{0}`;", table), valueResult)
        If valueResult.Rows.Count = 0 Then Exit Sub

        Dim isFirst As Boolean
        Dim tmpColumns As String = ""
        isFirst = True
        For Each col As String In Columns
            If isFirst = False Then tmpColumns &= ","
            tmpColumns &= "`" & col & "`"
            isFirst = False
        Next
        bw.Write("INSERT INTO `{0}`({1}) VALUES ", table, tmpColumns)

        isFirst = True
        For Each valueRow As DataRow In valueResult.Rows
            If isFirst = False Then bw.Write(",")
            bw.Write("(")
            Dim isFirst2 As Boolean = True
            For Each col As Object In valueRow.ItemArray
                If isFirst2 = False Then bw.Write(",")
                If (TypeOf col Is Single) OrElse (TypeOf col Is Double) Then
                    bw.Write(col.ToString().Replace(",", "."))
                ElseIf TypeOf col Is String Then
                    bw.Write("'" & CStr(col).Replace("'", "\'") & "'")
                ElseIf TypeOf col Is Byte() Then
                    If CType(col, Byte()).Length = 0 Then
                        bw.Write("''")
                    Else
                        bw.Write(ToHex(CType(col, Byte())))
                    End If
                ElseIf TypeOf col Is DBNull Then
                    bw.Write("NULL")
                Else
                    bw.Write(col)
                End If
                isFirst2 = False
            Next
            bw.Write(")")
            isFirst = False
        Next

        bw.WriteLine(";")
    End Sub
#End Region

#Region "Control Table"
    Public Sub ControlTable(ByVal DBFile As String, ByRef bw As StreamWriter, ByVal table As String, ByVal start As Long)
        Dim fs As New FileStream(DBFile, FileMode.Open, FileAccess.Read, FileShare.Read)
        Dim br As New IO.StreamReader(fs)

        br.BaseStream.Seek(0, SeekOrigin.Begin)

        Dim keyResult As New DataTable
        Database.Query(String.Format("SELECT CONSTRAINT_NAME, COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}';", Database.SQLDBName, table), keyResult)

        Dim primaryKeys As New List(Of String)
        For Each keyRow As DataRow In keyResult.Rows
            If keyRow.Item("CONSTRAINT_NAME") = "PRIMARY" Then
                primaryKeys.Add(keyRow.Item("COLUMN_NAME"))
            End If
        Next
        keyResult.Clear()

        Dim newLines As New List(Of Object())
        Dim oldLines As New List(Of Object())
        Dim updateLines As New List(Of Object())

        Dim tmpLine As String = ""
        Dim foundIt As Boolean = False
        Do While br.EndOfStream = False
            If foundIt = False Then
                tmpLine = br.ReadLine()
            Else
                tmpLine &= br.ReadLine()
            End If
            If foundIt OrElse tmpLine.StartsWith("INSERT ") OrElse tmpLine.StartsWith("insert ") Then
                Dim TableName As String = GetTableName(tmpLine)
                If TableName = table Then
                    foundIt = True
                    If tmpLine.EndsWith(";") = False Then Continue Do

                    'Check to see if there's more insert lines under this one
                    Dim lastLineIsDone As Boolean = True
                    Dim linesToSkip As Integer = 2
                    Do While br.EndOfStream = False AndAlso linesToSkip > 0
                        Dim sTmpLine1 As String = br.ReadLine()
                        If lastLineIsDone Then
                            If (sTmpLine1.StartsWith("INSERT ") OrElse sTmpLine1.StartsWith("insert ")) AndAlso GetTableName(tmpLine) = table Then
                                lastLineIsDone = False
                                tmpLine = tmpLine.Substring(0, tmpLine.Length - 1) ' Remove the ; sign at the end
                                tmpLine &= ","
                                Dim valuesPos As Integer = -1
                                valuesPos = sTmpLine1.IndexOf(") values (")
                                If valuesPos = -1 Then valuesPos = sTmpLine1.IndexOf(") VALUES (")
                                If valuesPos = -1 Then Console.Write("Values not found!") : Continue Do
                                valuesPos += 9
                                tmpLine &= sTmpLine1.Substring(valuesPos, sTmpLine1.Length - valuesPos)
                                If sTmpLine1.EndsWith(";") = False Then Continue Do
                                lastLineIsDone = True
                                linesToSkip = 2
                            Else
                                linesToSkip -= 1
                            End If
                        Else
                            tmpLine &= sTmpLine1
                            If sTmpLine1.EndsWith(";") Then
                                lastLineIsDone = True
                                linesToSkip = 2
                            End If
                        End If
                    Loop

                    Dim Columns As List(Of String) = GetColumns(tmpLine)
                    Dim fileData As New DataTable(table)

                    For Each col As String In Columns
                        fileData.Columns.Add(col)
                    Next

                    Dim index As Integer = tmpLine.IndexOf(" VALUES (")
                    If index = -1 Then index = tmpLine.IndexOf(" values (")
                    If index = -1 Then Exit Do
                    tmpLine = tmpLine.Substring(index + " VALUES (".Length)
                    tmpLine = tmpLine.Substring(0, tmpLine.LastIndexOf(")"))
                    Dim sSplit() As String = tmpLine.Split(New String() {"),("}, StringSplitOptions.RemoveEmptyEntries)
                    If sSplit.Length = 0 Then
                        ReDim sSplit(0)
                        sSplit(0) = tmpLine
                    End If
                    Dim j As Integer = 0
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write("Loading values from file         ")
                    Console.ForegroundColor = ConsoleColor.DarkCyan
                    For Each tmp As String In sSplit
                        Dim values As New List(Of Object)

                        Dim StartedString As Boolean = False
                        Dim tmpStr As String
                        Dim tmpInt As Integer
                        Dim tmpUint As UInteger
                        Dim tmpSng As Single
                        Dim tmpDbl As Double
                        Dim tmpLng As Long
                        Dim tmpUlng As ULong
                        index = 0
                        For i = index To tmp.Length - 1
                            If StartedString = False AndAlso (tmp(i) = ","c OrElse i = (tmp.Length - 1)) Then
                                tmpStr = tmp.Substring(index, i - index + 1)
                                If tmpStr.StartsWith(" ") Then tmpStr = tmpStr.Substring(1)
                                If tmpStr.EndsWith(",") Then tmpStr = tmpStr.Substring(0, tmpStr.Length - 1)
                                If Integer.TryParse(tmpStr, tmpInt) Then
                                    values.Add(tmpInt)
                                ElseIf UInteger.TryParse(tmpStr, tmpUint) Then
                                    values.Add(tmpUint)
                                ElseIf Long.TryParse(tmpStr, tmpLng) Then
                                    values.Add(tmpLng)
                                ElseIf ULong.TryParse(tmpStr, tmpUlng) Then
                                    values.Add(tmpUlng)
                                ElseIf Single.TryParse(tmpStr.Replace(".", ","), tmpSng) Then
                                    values.Add(tmpSng)
                                ElseIf Double.TryParse(tmpStr.Replace(".", ","), tmpDbl) Then
                                    values.Add(tmpDbl)
                                Else
                                    If tmpStr = "NULL" Then
                                        values.Add(Nothing)
                                    Else
                                        tmpStr = tmpStr.Replace("\'", "'").Replace("\""", """")
                                        values.Add(tmpStr)
                                    End If
                                End If
                                index = i + 1
                            ElseIf tmp(i) = "'"c AndAlso (i = 0 OrElse tmp(i - 1) <> "\"c) Then
                                StartedString = Not StartedString
                                If StartedString Then
                                    index = i + 1
                                Else
                                    If index = i Then
                                        tmpStr = ""
                                    Else
                                        tmpStr = tmp.Substring(index, i - index)
                                    End If
                                    tmpStr = tmpStr.Replace("\'", "'").Replace("\""", """").Replace("\r", vbCr).Replace("\n", vbLf)
                                    values.Add(tmpStr)
                                    i += 1
                                    index = i + 1
                                End If
                            End If
                        Next
                        Try
                            fileData.Rows.Add(values.ToArray)
                        Catch
                            Console.WriteLine(Columns.Count & " - " & values.ToArray.Count)
                            Console.WriteLine(tmp)
                            Throw New Exception("FAILBOT!")
                        End Try
                        j += 1
                        If (j Mod Int(sSplit.Length / 30)) = 0 Then
                            Console.Write(".")
                        End If
                    Next
                    Console.ForegroundColor = ConsoleColor.DarkGreen
                    Console.WriteLine(" Done")

                    Console.ForegroundColor = ConsoleColor.DarkYellow
                    Console.WriteLine("Rows: {0}", fileData.Rows.Count)

                    Dim DBResult As New DataTable
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write("Loading values from database     ")
                    Console.ForegroundColor = ConsoleColor.DarkCyan
                    Console.Write("...")
                    Dim theLines As New Dictionary(Of Integer, Boolean)
                    Database.Query(String.Format("SELECT * FROM `{0}`;", table), DBResult)
                    Console.Write("...")
                    Console.ForegroundColor = ConsoleColor.DarkGreen
                    Console.WriteLine(" Done")

                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write("Initializing stuff               ")
                    Console.ForegroundColor = ConsoleColor.DarkCyan
                    For i As Integer = 0 To DBResult.Rows.Count - 1
                        theLines.Add(i, False)

                        If ((i + 1) Mod Int(DBResult.Rows.Count / 30)) = 0 Then
                            Console.Write(".")
                        End If
                    Next
                    Console.ForegroundColor = ConsoleColor.DarkGreen
                    Console.WriteLine(" Done")

                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write("Checking for changed/removed rows")
                    Console.ForegroundColor = ConsoleColor.DarkCyan
                    For i As Integer = 0 To fileData.Rows.Count - 1
                        If primaryKeys.Count = 0 Then Exit For
                        Dim row As DataRow = fileData.Rows(i)
                        Dim found As Boolean = False
                        For k As Integer = i To 0 Step -1
                            If k >= DBResult.Rows.Count Then Continue For
                            If k < (i - 1000) Then Exit For
                            Dim row2 As DataRow = DBResult.Rows(k)
                            For Each key As String In primaryKeys
                                If TypeOf row.Item(key) Is DBNull Then GoTo NextRow
                                If row.Item(key) <> row2.Item(key) Then
                                    GoTo NextRow
                                End If
                            Next

                            'DONE: Check if the row has changed
                            found = True
                            theLines(k) = True
                            For Each col As String In Columns
                                Dim tmpValue As Object = row2.Item(col)
                                Dim tmpValue2 As Object = row.Item(col)
                                If TypeOf tmpValue Is Single OrElse TypeOf tmpValue Is Double Then
                                    tmpValue = tmpValue.ToString.ToUpper.Replace("E-000", "E-").Replace("E-00", "E-").Replace("E-0", "E-")
                                    tmpValue2 = tmpValue2.ToString.ToUpper.Replace("E-000", "E-").Replace("E-00", "E-").Replace("E-0", "E-")
                                ElseIf TypeOf tmpValue Is Byte() Then
                                    If IsHex(tmpValue2) Then
                                        tmpValue = ToHex(CType(tmpValue, Byte()))
                                    Else
                                        tmpValue = ToStr(CType(tmpValue, Byte()))
                                    End If
                                End If
                                Try
                                    If (TypeOf tmpValue2 Is DBNull AndAlso Not (TypeOf tmpValue Is DBNull)) OrElse (TypeOf tmpValue Is DBNull AndAlso Not (TypeOf tmpValue2 Is DBNull)) OrElse (tmpValue2 <> tmpValue) Then
                                        'Console.WriteLine(col & ": """ & tmpValue2 & """ <> """ & tmpValue & """")
                                        updateLines.Add(row2.ItemArray)
                                        Exit For
                                    End If
                                Catch
                                    'Console.WriteLine("ERROR: " & col & ": """ & tmpValue2 & """ <> """ & tmpValue & """")
                                End Try

                            Next
                            Exit For
NextRow:
                        Next
                        If found = False Then
                            oldLines.Add(row.ItemArray)
                        End If

                        If ((i + 1) Mod Int(fileData.Rows.Count / 30)) = 0 Then
                            Console.Write(".")
                        End If
                    Next
                    Console.ForegroundColor = ConsoleColor.DarkGreen
                    Console.WriteLine(" Done")

                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write("Checking for added rows          ")
                    Console.ForegroundColor = ConsoleColor.DarkCyan

                    If theLines.Count = 0 Then
                        Console.Write(".")
                    Else
                        For Each line As KeyValuePair(Of Integer, Boolean) In theLines
                            If Not line.Value Then newLines.Add(DBResult.Rows(line.Key).ItemArray)

                            If ((line.Key + 1) Mod Int(theLines.Count / 30)) = 0 Then
                                Console.Write(".")
                            End If
                        Next
                    End If
                    DBResult.Clear()
                    fileData.Clear()

                    Console.ForegroundColor = ConsoleColor.DarkGreen
                    Console.WriteLine(" Done")

                    Console.ForegroundColor = ConsoleColor.DarkYellow
                    Console.WriteLine("New rows: {0}", newLines.Count)
                    Console.WriteLine("Removed rows: {0}", oldLines.Count)
                    Console.WriteLine("Changed rows: {0}", updateLines.Count)

                    'DONE: Write the new rows to the file
                    Dim isFirst As Boolean
                    If newLines.Count > 0 Then
                        Dim tmpColumns As String = ""
                        isFirst = True
                        For Each col As String In Columns
                            If isFirst = False Then tmpColumns &= ","
                            tmpColumns &= "`" & col & "`"
                            isFirst = False
                        Next
                        bw.Write("INSERT INTO `{0}`({1}) VALUES ", table, tmpColumns)

                        isFirst = True
                        For Each row As Object() In newLines
                            If isFirst = False Then bw.Write(",")
                            bw.Write("(")
                            Dim isFirst2 As Boolean = True
                            For Each col As Object In row
                                If isFirst2 = False Then bw.Write(",")
                                If (TypeOf col Is Single) OrElse (TypeOf col Is Double) Then
                                    bw.Write(col.ToString().Replace(",", "."))
                                ElseIf TypeOf col Is String Then
                                    bw.Write("'" & CStr(col).Replace("'", "\'") & "'")
                                ElseIf TypeOf col Is Byte() Then
                                    If CType(col, Byte()).Length = 0 Then
                                        bw.Write("''")
                                    Else
                                        bw.Write(ToHex(CType(col, Byte())))
                                    End If
                                Else
                                    bw.Write(col)
                                End If
                                isFirst2 = False
                            Next
                            bw.Write(")")
                            isFirst = False
                        Next
                        newLines.Clear()
                        bw.WriteLine(";")
                    End If

                    'DONE: Write the removed lines to the file
                    For Each row As Object() In oldLines
                        Dim tmpColumns As String = ""
                        isFirst = True
                        If primaryKeys.Count = 0 Then
                            For Each col As String In Columns
                                If isFirst = False Then tmpColumns &= " AND "
                                For i As Integer = 0 To Columns.Count - 1
                                    If Columns(i) = col Then
                                        tmpColumns &= "`" & col & "`='" & row(i) & "'"
                                    End If
                                Next
                                isFirst = False
                            Next
                        Else
                            For Each key As String In primaryKeys
                                If isFirst = False Then tmpColumns &= " AND "
                                For i As Integer = 0 To Columns.Count - 1
                                    If Columns(i) = key Then
                                        tmpColumns &= "`" & key & "`='" & row(i) & "'"
                                    End If
                                Next
                                isFirst = False
                            Next
                        End If

                        bw.WriteLine("DELETE FROM `{0}` WHERE {1};", table, tmpColumns)
                    Next
                    oldLines.Clear()

                    'DONE: Write the changed lines to the file
                    For Each row As Object() In updateLines
                        Dim tmpColumns As String = ""
                        isFirst = True
                        If primaryKeys.Count = 0 Then
                            For Each col As String In Columns
                                If isFirst = False Then tmpColumns &= " AND "
                                For i As Integer = 0 To Columns.Count - 1
                                    If Columns(i) = col Then
                                        tmpColumns &= "`" & col & "`='" & row(i) & "'"
                                    End If
                                Next
                                isFirst = False
                            Next
                        Else
                            For Each key As String In primaryKeys
                                If isFirst = False Then tmpColumns &= " AND "
                                For i As Integer = 0 To Columns.Count - 1
                                    If Columns(i) = key Then
                                        tmpColumns &= "`" & key & "`='" & row(i) & "'"
                                    End If
                                Next
                                isFirst = False
                            Next
                        End If

                        Dim tmpStr As String = ""
                        isFirst = True
                        For Each col As String In Columns
                            If primaryKeys.Contains(col) Then Continue For
                            If isFirst = False Then tmpStr &= ", "
                            For i As Integer = 0 To Columns.Count - 1
                                If Columns(i) = col Then
                                    tmpStr &= "`" & col & "`="
                                    If (TypeOf row(i) Is Single) OrElse (TypeOf row(i) Is Double) Then
                                        tmpStr &= row(i).ToString().Replace(",", ".")
                                    ElseIf TypeOf row(i) Is String Then
                                        tmpStr &= "'" & CStr(row(i)).Replace("'", "\'") & "'"
                                    ElseIf TypeOf row(i) Is Byte() Then
                                        tmpStr &= "'" & ToStr(CType(row(i), Byte())) & "'"
                                        If CType(row(i), Byte()).Length = 0 Then
                                            tmpStr &= "''"
                                        Else
                                            tmpStr &= ToHex(CType(row(i), Byte()))
                                        End If
                                    ElseIf TypeOf row(i) Is DBNull Then
                                        tmpStr &= "NULL"
                                    Else
                                        tmpStr &= row(i).ToString()
                                    End If
                                End If
                            Next
                            isFirst = False
                        Next

                        bw.WriteLine("UPDATE `{0}` SET {1} WHERE {2};", table, tmpStr, tmpColumns)
                    Next
                    updateLines.Clear()

                    GoTo ExitNow
                End If
            End If
        Loop

        'TODO: Insert all rows we find in the new db
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine("No data found in table: {0}", table)
        DumpTable(bw, table, Nothing)

ExitNow:
        If fs IsNot Nothing Then
            fs.Close()
            fs.Dispose()
            fs = Nothing
            br = Nothing
        End If
    End Sub
#End Region

End Module
