Module Program
    Public frmLoad As New frmLoad
    Public frmDbcCompare As New frmDbcCompare

    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New frmMain())
    End Sub
End Module
