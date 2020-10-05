Namespace Objects
    Public Class GossipMenu
        Public Sub AddMenu(ByVal menu As String, Optional ByVal icon As Byte = 0, Optional ByVal isCoded As Byte = 0, Optional ByVal cost As Integer = 0, Optional ByVal WarningMessage As String = "")
            Icons.Add(icon)
            Menus.Add(menu)
            Coded.Add(isCoded)
            Costs.Add(cost)
            WarningMessages.Add(WarningMessage)
        End Sub
        Public Icons As New ArrayList
        Public Menus As New ArrayList
        Public Coded As New ArrayList
        Public Costs As New ArrayList
        Public WarningMessages As New ArrayList
    End Class
End NameSpace