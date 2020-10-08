Imports System.Xml.Serialization
Imports Mangos.Common.Enums.Global

<XmlRoot(ElementName:="WorldCluster")>
Public Class ClusterServerConfiguration
    <XmlElement(ElementName:="WorldClusterPort")>
    Public Property WorldClusterPort As Integer = 8085

    <XmlElement(ElementName:="WorldClusterAddress")>
    Public Property WorldClusterAddress As String = "127.0.0.1"

    <XmlElement(ElementName:="ServerPlayerLimit")>
    Public Property ServerPlayerLimit As Integer = 10

    'Database Settings
    <XmlElement(ElementName:="AccountDatabase")>
    Public Property AccountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

    <XmlElement(ElementName:="CharacterDatabase")>
    Public Property CharacterDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

    <XmlElement(ElementName:="WorldDatabase")>
    Public Property WorldDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

    'Cluster Settings

    <XmlElement(ElementName:="ClusterListenAddress")>
    Public Property ClusterListenAddress As String = "127.0.0.1"

    <XmlElement(ElementName:="ClusterListenPort")>
    Public Property ClusterListenPort As Integer = 50001

    'Stats Settings
    <XmlElement(ElementName:="StatsEnabled")>
    Public Property StatsEnabled As Boolean = True

    <XmlElement(ElementName:="StatsTimer")>
    Public Property StatsTimer As Integer = 120000

    <XmlElement(ElementName:="StatsLocation")>
    Public Property StatsLocation As String = "stats.xml"

    'Logging Settings
    <XmlElement(ElementName:="LogType")>
    Public Property LogType As String = "FILE"

    <XmlElement(ElementName:="LogLevel")>
    Public Property LogLevel As LogType = Mangos.Common.Enums.Global.LogType.NETWORK

    <XmlElement(ElementName:="LogConfig")>
    Public Property LogConfig As String = ""

    <XmlElement(ElementName:="PacketLogging")>
    Public Property PacketLogging As Boolean = False

    <XmlElement(ElementName:="GMLogging")>
    Public Property GMLogging As Boolean = False
End Class