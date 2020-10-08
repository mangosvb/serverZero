Imports System.Xml.Serialization
Imports Mangos.Common.Enums.Global

<XmlRoot(ElementName:="WorldServer")>
Public Class WorldServerConfiguration
    'Database Settings
    <XmlElement(ElementName:="AccountDatabase")>
    Public AccountDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
    <XmlElement(ElementName:="CharacterDatabase")>
    Public CharacterDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"
    <XmlElement(ElementName:="WorldDatabase")>
    Public WorldDatabase As String = "root;mangosVB;localhost;3306;mangosVB;MySQL"

    'Server Settings
    <XmlElement(ElementName:="ServerPlayerLimit")>
    Public ServerPlayerLimit As Integer = 10
    <XmlElement(ElementName:="CommandCharacter")>
    Public CommandCharacter As String = "."
    <XmlElement(ElementName:="XPRate")>
    Public XPRate As Single = 1.0
    <XmlElement(ElementName:="ManaRegenerationRate")>
    Public ManaRegenerationRate As Single = 1.0
    <XmlElement(ElementName:="HealthRegenerationRate")>
    Public HealthRegenerationRate As Single = 1.0
    <XmlElement(ElementName:="GlobalAuction")>
    Public GlobalAuction As Boolean = False
    <XmlElement(ElementName:="SaveTimer")>
    Public SaveTimer As Integer = 120000
    <XmlElement(ElementName:="WeatherTimer")>
    Public WeatherTimer As Integer = 600000
    <XmlElement(ElementName:="MapResolution")>
    Public MapResolution As Integer = 64
    <XmlArray(ElementName:="HandledMaps"), XmlArrayItem(GetType(String), ElementName:="Map")>
    Public Maps As New List(Of String)

    'VMap Settings
    <XmlElement(ElementName:="VMaps")>
    Public VMapsEnabled As Boolean = False
    <XmlElement(ElementName:="VMapLineOfSightCalc")>
    Public LineOfSightEnabled As Boolean = False
    <XmlElement(ElementName:="VMapHeightCalc")>
    Public HeightCalcEnabled As Boolean = False

    'Logging Settings
    <XmlElement(ElementName:="LogType")>
    Public LogType As String = "FILE"
    <XmlElement(ElementName:="LogLevel")>
    Public LogLevel As LogType = Mangos.Common.Enums.Global.LogType.NETWORK
    <XmlElement(ElementName:="LogConfig")>
    Public LogConfig As String = ""

    'Other Settings
    <XmlArray(ElementName:="ScriptsCompiler"), XmlArrayItem(GetType(String), ElementName:="Include")>
    Public CompilerInclude As New ArrayList
    <XmlElement(ElementName:="CreatePartyInstances")>
    Public CreatePartyInstances As Boolean = False
    <XmlElement(ElementName:="CreateRaidInstances")>
    Public CreateRaidInstances As Boolean = False
    <XmlElement(ElementName:="CreateBattlegrounds")>
    Public CreateBattlegrounds As Boolean = False
    <XmlElement(ElementName:="CreateArenas")>
    Public CreateArenas As Boolean = False
    <XmlElement(ElementName:="CreateOther")>
    Public CreateOther As Boolean = False

    'Cluster Settings
    <XmlElement(ElementName:="ClusterConnectHost")>
    Public ClusterConnectHost As String = "127.0.0.1"
    <XmlElement(ElementName:="ClusterConnectPort")>
    Public ClusterConnectPort As Integer = 50001
    <XmlElement(ElementName:="LocalConnectHost")>
    Public LocalConnectHost As String = "127.0.0.1"
    <XmlElement(ElementName:="LocalConnectPort")>
    Public LocalConnectPort As Integer = 50002
End Class