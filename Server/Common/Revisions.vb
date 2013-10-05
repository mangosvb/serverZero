Public Class Revisions
    Public Const REVISION_DB_CHARACTERS As String = "required_s1350_11716_09_characters_mail"
    Public Const REVISION_DB_MANGOS As String = "required_s2034_12522_01_mangos_db_script_string"
    Public Const REVISION_DB_REALMD As String = "required_c12484_02_realmd_account_access"

    Public Shared Function CheckRequiredDBVersion(ByRef ThisDatabase As SQL, ByRef DatabaseName As String) As Boolean
        Dim thisTableName As String

        Select Case DatabaseName.ToUpper()
            Case "REALM"
                thisTableName = "realmd_db_version"
            Case "CHARACTERS"
                thisTableName = "character_db_version"
            Case "WORLD"
                thisTableName = "db_version"
            Case Else
                thisTableName = "error"
        End Select
        Dim MySQLQuery As New DataTable
        ThisDatabase.Query(String.Format("SELECT column_name FROM information_schema.columns WHERE table_name='realmd_db_version'  AND TABLE_SCHEMA='" & ThisDatabase.SQLDBName & "'"), MySQLQuery)

        'Check database version against code version 
        Dim dtVersion As String = ""

        For Each row As DataRow In MySQLQuery.Rows
            dtVersion = row.Item("column_name").ToString
        Next

        If dtVersion = "" Then
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] The table `" & "realmd_db_version" & "` in your [" & ThisDatabase.SQLDBName & "]", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] database is missing its version info.", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] MaNGOSVB cannot find the version info required", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] to check that the db is up to date.", Format(TimeOfDay, "hh:mm:ss"))
            '            Console.WriteLine("[{0}]", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] This revision of MaNGOSVB requires a database updated to:", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] `" & Common.Revisions.REVISION_DB_REALMD.Replace("required_", "") & ".sql`", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("*************************")
            Console.WriteLine("* Press any key to exit *")
            Console.WriteLine("*************************")
            Console.ReadKey()
            Return False
        ElseIf dtVersion = Common.Revisions.REVISION_DB_REALMD Then 'They Match
            Console.WriteLine("[{0}] Db Version Matched", Format(TimeOfDay, "hh:mm:ss"))
            Return True
        Else 'Oh no they do not match
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] The table `" & "realmd_db_version" & "` in your [" & ThisDatabase.SQLDBName & "] database", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] indicates that this database is out of date!", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] --------------------------------------------------------------", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}]  [A] You have: --> `" & dtVersion & ".sql`", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}] ", Format(TimeOfDay, "hh:mm:ss"))
            Console.WriteLine("[{0}]  [B] You need: --> `" & Common.Revisions.REVISION_DB_REALMD & ".sql`", Format(TimeOfDay, "hh:mm:ss"))
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
    End Function
End Class
