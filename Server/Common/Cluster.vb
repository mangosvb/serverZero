Imports System.ComponentModel
Imports System.Collections.Generic

Public Enum ExpansionLevel As Byte
    NORMAL = 0          'WoW
End Enum

Public Enum AccessLevel As Byte
    Trial = 0
    Player = 1
    GameMaster = 2
    Developer = 3
    Admin = 4
End Enum

Public Class Authenticator
    Inherits MarshalByRefObject

    Private aObject As Object = Nothing
    Private aPassword As String = ""

    Public Sub New(ByVal pObject As Object, ByVal pPassword As String)
        aPassword = pPassword
        aObject = pObject
    End Sub

    Public Function Login(ByVal Password As String) As Object
        If aPassword = Password Then
            Return aObject
        Else
            Return Nothing
        End If
    End Function

    ' Make object live forever
    Public Overrides Function InitializeLifetimeService() As Object
        Return Nothing
    End Function
End Class

Public Interface ICluster

    <Description("Signal realm server for new world server.")> _
    Function Connect(ByVal URI As String, ByVal Maps As ICollection) As Boolean
    <Description("Signal realm server for disconected world server.")> _
    Sub Disconnect(ByVal URI As String, ByVal Maps As ICollection)

    <Description("Send data packet to client.")> _
    Sub ClientSend(ByVal ID As UInteger, ByVal Data As Byte())
    <Description("Notify client drop.")> _
    Sub ClientDrop(ByVal ID As UInteger)
    <Description("Notify client transfer.")> _
    Sub ClientTransfer(ByVal ID As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As UInteger)
    <Description("Notify client update.")> _
    Sub ClientUpdate(ByVal ID As UInteger, ByVal Zone As UInteger, ByVal Level As Byte)
    <Description("Set client chat flag.")> _
    Sub ClientSetChatFlag(ByVal ID As UInteger, ByVal Flag As Byte)
    <Description("Get client crypt key.")> _
    Function ClientGetCryptKey(ByVal ID As UInteger) As Byte()

    Function BattlefieldList(ByVal Type As Byte) As List(Of Integer)
    Sub BattlefieldFinish(ByVal BattlefieldID As Integer)

    <Description("Send data packet to all clients online.")> _
    Sub Broadcast(ByVal Data() As Byte)
    <Description("Send data packet to all clients in specified client's group.")> _
    Sub BroadcastGroup(ByVal GroupID As Long, ByVal Data() As Byte)
    <Description("Send data packet to all clients in specified client's raid.")> _
    Sub BroadcastRaid(ByVal GroupID As Long, ByVal Data() As Byte)
    <Description("Send data packet to all clients in specified client's guild.")> _
    Sub BroadcastGuild(ByVal GuildID As Long, ByVal Data() As Byte)
    <Description("Send data packet to all clients in specified client's guild officers.")> _
    Sub BroadcastGuildOfficers(ByVal GuildID As Long, ByVal Data() As Byte)

    <Description("Send update for the requested group.")> _
    Sub GroupRequestUpdate(ByVal ID As UInteger)

End Interface
Public Interface IWorld

    <Description("Initialize client object.")> _
    Sub ClientConnect(ByVal ID As UInteger, ByVal Client As ClientInfo)
    <Description("Destroy client object.")> _
    Sub ClientDisconnect(ByVal ID As UInteger)
    <Description("Assing particular client to this world server (Use client ID).")> _
    Sub ClientLogin(ByVal ID As UInteger, ByVal GUID As ULong)
    <Description("Remove particular client from this world server (Use client ID).")> _
    Sub ClientLogout(ByVal ID As UInteger)
    <Description("Transfer packet from Realm to World using client's ID.")> _
    Sub ClientPacket(ByVal ID As UInteger, ByVal Data() As Byte)

    <Description("Create CharacterObject.")> _
    Function ClientCreateCharacter(ByVal Account As String, ByVal Name As String, ByVal Race As Byte, ByVal Classe As Byte, ByVal Gender As Byte, ByVal Skin As Byte, _
                                   ByVal Face As Byte, ByVal HairStyle As Byte, ByVal HairColor As Byte, ByVal FacialHair As Byte, ByVal OutfitID As Byte) As Integer

    <Description("Respond to world server if still alive.")> _
    Function Ping(ByVal Timestamp As Integer, ByVal Latency As Integer) As Integer

    <Description("Tell the cluster about your CPU & Memory Usage")> _
    Sub ServerInfo(ByRef CPUUsage As Single, ByRef MemoryUsage As ULong)

    <Description("Make world create specific map.")> _
    Sub InstanceCreate(ByVal Map As UInteger)
    <Description("Make world destroy specific map.")> _
    Sub InstanceDestroy(ByVal Map As UInteger)
    <Description("Check world configuration.")> _
    Function InstanceCanCreate(ByVal Type As Integer) As Boolean

    <Description("Set client's group.")> _
    Sub ClientSetGroup(ByVal ID As UInteger, ByVal GroupID As Long)
    <Description("Update group information.")> _
    Sub GroupUpdate(ByVal GroupID As Long, ByVal GroupType As Byte, ByVal GroupLeader As ULong, ByVal Members() As ULong)
    <Description("Update group information about looting.")> _
    Sub GroupUpdateLoot(ByVal GroupID As Long, ByVal Difficulty As Byte, ByVal Method As Byte, ByVal Threshold As Byte, ByVal Master As ULong)

    <Description("Request party member stats.")> _
    Function GroupMemberStats(ByVal GUID As ULong, ByVal Flag As Integer) As Byte()

    <Description("Update guild information.")> _
    Sub GuildUpdate(ByVal GUID As ULong, ByVal GuildID As UInteger, ByVal GuildRank As Byte)

    Sub BattlefieldCreate(ByVal BattlefieldID As Integer, ByVal BattlefieldMapType As Byte, ByVal Map As UInteger)
    Sub BattlefieldDelete(ByVal BattlefieldID As Integer)
    Sub BattlefieldJoin(ByVal BattlefieldID As Integer, ByVal GUID As ULong)
    Sub BattlefieldLeave(ByVal BattlefieldID As Integer, ByVal GUID As ULong)

End Interface

<Serializable()> _
Public Class ClientInfo
    Public Index As UInteger
    Public IP As Net.IPAddress
    Public Port As UInteger
    Public Account As String
    Public Access As AccessLevel = AccessLevel.Player
    Public Expansion As ExpansionLevel = ExpansionLevel.NORMAL
End Class
