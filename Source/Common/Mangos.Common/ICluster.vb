Imports System.ComponentModel

Public Interface ICluster

    <Description("Signal realm server for new world server.")>
    Function Connect(ByVal uri As String, ByVal maps As List(Of UInteger)) As Boolean
    <Description("Signal realm server for disconected world server.")>
    Sub Disconnect(ByVal uri As String, ByVal maps As List(Of UInteger))

    <Description("Send data packet to client.")>
    Sub ClientSend(ByVal id As UInteger, ByVal data As Byte())
    <Description("Notify client drop.")>
    Sub ClientDrop(ByVal id As UInteger)
    <Description("Notify client transfer.")>
    Sub ClientTransfer(ByVal id As UInteger, ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal map As UInteger)
    <Description("Notify client update.")>
    Sub ClientUpdate(ByVal id As UInteger, ByVal zone As UInteger, ByVal level As Byte)
    <Description("Set client chat flag.")>
    Sub ClientSetChatFlag(ByVal id As UInteger, ByVal flag As Byte)
    <Description("Get client crypt key.")>
    Function ClientGetCryptKey(ByVal id As UInteger) As Byte()

    Function BattlefieldList(ByVal type As Byte) As List(Of Integer)
    Sub BattlefieldFinish(ByVal battlefieldId As Integer)

    <Description("Send data packet to all clients online.")>
    Sub Broadcast(ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's group.")>
    Sub BroadcastGroup(ByVal groupId As Long, ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's raid.")>
    Sub BroadcastRaid(ByVal groupId As Long, ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's guild.")>
    Sub BroadcastGuild(ByVal guildId As Long, ByVal data() As Byte)
    <Description("Send data packet to all clients in specified client's guild officers.")>
    Sub BroadcastGuildOfficers(ByVal guildId As Long, ByVal data() As Byte)

    <Description("Send update for the requested group.")>
    Sub GroupRequestUpdate(ByVal id As UInteger)

End Interface