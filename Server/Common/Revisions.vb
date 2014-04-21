'
' Copyright (C) 2013 - 2014 getMaNGOS <http://www.getmangos.eu>
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

Public Class Revisions
    Private Const RevisionDbCharacters As String = "required_s1350_11716_09_characters_mail"
    Private Const RevisionDbMangos As String = "required_s2034_12522_01_mangos_db_script_string"
    Private Const RevisionDbRealmd As String = "required_c12484_02_realmd_account_access"

    Public Shared Function CheckRequiredDbVersion(ByRef thisDatabase As SQL, ByRef databaseName As String) As Boolean
        Dim thisTableName As String
        Dim thisDbTableName As String = ""
        Select Case databaseName.ToUpper()
            Case "REALM"
                thisTableName = "realmd_db_version"
                thisDbTableName = RevisionDbRealmd
            Case "CHARACTERS"
                thisTableName = "character_db_version"
                thisDbTableName = RevisionDbCharacters
            Case "WORLD"
                thisTableName = "db_version"
                thisDbTableName = RevisionDbMangos
            Case Else
                thisTableName = "error"
        End Select
        Dim mySqlQuery As New DataTable
        thisDatabase.Query(String.Format("SELECT column_name FROM information_schema.columns WHERE table_name='" & thisTableName & "'  AND TABLE_SCHEMA='" & thisDatabase.SQLDBName & "'"), mySqlQuery)

        'Check database version against code version
        Dim dtVersion As String = ""
        If mySqlQuery.Rows.Count > 0 Then
            For Each row As DataRow In mySqlQuery.Rows
                dtVersion = row.Item("column_name").ToString
            Next

            If dtVersion = "" Then
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] The table `" & thisDbTableName & "` in your [" & thisDatabase.SQLDBName & "]", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] database is missing its version info.", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] MaNGOSVB cannot find the version info required", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] to check that the db is up to date.", Format(TimeOfDay, "hh:mm:ss"))
                '            Console.WriteLine("[{0}]", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] This revision of MaNGOSVB requires a database updated to:", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] `" & thisDbTableName.Replace("required_", "") & ".sql`", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("*************************")
                Console.WriteLine("* Press any key to exit *")
                Console.WriteLine("*************************")
                Console.ReadKey()
                Return False
            ElseIf dtVersion = RevisionDbRealmd Then 'They Match
                Console.WriteLine("[{0}] Db Version Matched", Format(TimeOfDay, "hh:mm:ss"))
                Return True
            Else 'Oh no they do not match
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] The table `" & thisDbTableName & "` in your [" & thisDatabase.SQLDBName & "] database", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] indicates that this database is out of date!", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}]  [A] You have: --> `" & dtVersion & ".sql`", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}]  [B] You need: --> `" & thisDbTableName & ".sql`", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] You must apply all updates after [A] to [B] to use mangos with this", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] database. These updates are included in the sql/updates folder.", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] Please read the included [README] in sql/updates for instructions", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("[{0}] on how to update.", Format(TimeOfDay, "hh:mm:ss"))
                Console.WriteLine("*************************")
                Console.WriteLine("* Press any key to exit *")
                Console.WriteLine("*************************")
                Console.ReadKey()
                Return False
            End If
        Else
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] The table `" & thisDbTableName & "` in your [" & thisDatabase.SQLDBName & "]", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] database is missing ", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] MaNGOSVB cannot find the version info required", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] to check that the db is up to date.", Format(TimeOfDay, "hh:mm:ss"))
            '            Console.WriteLine("[{0}]", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] This revision of MaNGOSVB requires a database updated to:", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] `" & thisDbTableName.Replace("required_", "") & ".sql`", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            Return False
        End If
    End Function
End Class