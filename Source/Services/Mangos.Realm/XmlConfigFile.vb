Imports System.Xml.Serialization

<XmlRoot(ElementName:="RealmServer")>
Public Class XmlConfigFile
    'Server Configurations
    <XmlElement(ElementName:="RealmServerPort")>
    Private _realmServerPort As Integer = 3724

    <XmlElement(ElementName:="RealmServerAddress")>
    Private _realmServerAddress As String = "127.0.0.1"

    <XmlElement(ElementName:="AccountDatabase")>
    Private _accountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

    Property RealmServerPort As Integer
        Get
            Return _realmServerPort
        End Get
        Set(value As Integer)
            _realmServerPort = value
        End Set
    End Property

    Property RealmServerAddress As String
        Get
            Return _realmServerAddress
        End Get
        Set(value As String)

            If value Is Nothing Then
                Throw New ArgumentNullException(NameOf(value))
            End If

            _realmServerAddress = value
        End Set
    End Property

    Property AccountDatabase As String
        Get
            Return _accountDatabase
        End Get
        Set(value As String)

            If value Is Nothing Then
                Throw New ArgumentNullException(NameOf(value))
            End If

            _accountDatabase = value
        End Set
    End Property
End Class