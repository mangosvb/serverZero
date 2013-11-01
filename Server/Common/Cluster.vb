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

    Private ReadOnly _aObject As Object = Nothing
    Private ReadOnly _aPassword As String = ""

    Public Sub New(ByVal pObject As Object, ByVal pPassword As String)
        _aPassword = pPassword
        _aObject = pObject
    End Sub

    Public Function Login(ByVal password As String) As Object
        If _aPassword = password Then
            Return _aObject
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
    Function Connect(ByVal uri As String, ByVal maps As ICollection) As Boolean
    <Description("Signal realm server for disconected world server.")> _
    Sub Disconnect(ByVal uri As String, ByVal maps As ICollection)

    <Description("Send data packet to client.")> _
    Sub ClientSend(ByVal id As UInteger, ByVal data As Byte())
    <Description("Notify client drop.")> _
    Sub ClientDrop(ByVal id As UInteger)
    <Description("Notify client transfer.")> _
    Sub ClientTransfer(ByVal id As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As UInteger)
    <Description("Notify client update.")> _
    Sub ClientUpdate(ByVal id As UInteger, ByVal zone As UInteger, ByVal level As Byte)
    <Description("Set client chat flag.")> _
    Sub ClientSetChatFlag(ByVal id As UInteger, ByVal flag As Byte)
    <Description("Get client crypt key.")> _
    Function ClientGetCryptKey(ByVal id As UInteger) As Byte()

    Function BattlefieldList(ByVal type As Byte) As List(Of Integer)
    Sub BattlefieldFinish(ByVal battlefieldId As Integer)

    <Description("Send data packet to all clients online.")> _
    Sub Broadcast(ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's group.")> _
    Sub BroadcastGroup(ByVal groupId As Long, ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's raid.")> _
    Sub BroadcastRaid(ByVal groupId As Long, ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's guild.")> _
    Sub BroadcastGuild(ByVal guildId As Long, ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's guild officers.")> _
    Sub BroadcastGuildOfficers(ByVal guildId As Long, ByVal data() As Byte)

    <Description("Send update for the requested group.")> _
    Sub GroupRequestUpdate(ByVal id As UInteger)

End Interface
Public Interface IWorld

    <Description("Initialize client object.")> _
    Sub ClientConnect(ByVal id As UInteger, ByVal client As ClientInfo)
    <Description("Destroy client object.")> _
    Sub ClientDisconnect(ByVal id As UInteger)
    <Description("Assing particular client to this world server (Use client ID).")> _
    Sub ClientLogin(ByVal id As UInteger, ByVal guid As ULong)
    <Description("Remove particular client from this world server (Use client ID).")> _
    Sub ClientLogout(ByVal id As UInteger)
    <Description("Transfer packet from Realm to World using client's ID.")> _
    Sub ClientPacket(ByVal id As UInteger, ByVal data() As Byte)

    <Description("Create CharacterObject.")> _
    Function ClientCreateCharacter(ByVal account As String, ByVal name As String, ByVal race As Byte, ByVal classe As Byte, ByVal gender As Byte, ByVal skin As Byte, _
                                   ByVal face As Byte, ByVal hairStyle As Byte, ByVal hairColor As Byte, ByVal facialHair As Byte, ByVal outfitId As Byte) As Integer

    <Description("Respond to world server if still alive.")> _
    Function Ping(ByVal timestamp As Integer, ByVal latency As Integer) As Integer

    <Description("Tell the cluster about your CPU & Memory Usage")> _
    Sub ServerInfo(ByRef cpuUsage As Single, ByRef memoryUsage As ULong)

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
