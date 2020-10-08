Imports System.Xml.Serialization

<XmlRoot(ElementName:="RealmServer")>
Public Class RealmServerConfiguration
    Public Property RealmServerPort As Integer = 3724

    Public Property RealmServerAddress As String = "127.0.0.1"

    Public Property AccountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
End Class