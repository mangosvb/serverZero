Public Class QuestMenu
    Public Sub AddMenu(ByVal QuestName As String, ByVal ID As Short, ByVal Level As Short, Optional ByVal Icon As Byte = 0)
        Names.Add(QuestName)
        IDs.Add(ID)
        Icons.Add(Icon)
        Levels.Add(Level)
    End Sub
    Public IDs As ArrayList = New ArrayList
    Public Names As ArrayList = New ArrayList
    Public Icons As ArrayList = New ArrayList
    Public Levels As ArrayList = New ArrayList
End Class