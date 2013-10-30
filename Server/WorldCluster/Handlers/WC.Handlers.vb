'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.Threading
Imports System.Net.Sockets
Imports System.Xml.Serialization
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common


Public Module WC_Handlers

    Public Sub IntializePacketHandlers()
        'NOTE: These opcodes are not used in any way
        PacketHandlers(OPCODES.CMSG_MOVE_TIME_SKIPPED) = CType(AddressOf OnUnhandledPacket, HandlePacket)

        PacketHandlers(OPCODES.CMSG_NEXT_CINEMATIC_CAMERA) = CType(AddressOf On_CMSG_NEXT_CINEMATIC_CAMERA, HandlePacket)
        PacketHandlers(OPCODES.CMSG_COMPLETE_CINEMATIC) = CType(AddressOf On_CMSG_COMPLETE_CINEMATIC, HandlePacket)
        PacketHandlers(OPCODES.CMSG_UPDATE_ACCOUNT_DATA) = CType(AddressOf On_CMSG_UPDATE_ACCOUNT_DATA, HandlePacket)
        PacketHandlers(OPCODES.CMSG_REQUEST_ACCOUNT_DATA) = CType(AddressOf On_CMSG_REQUEST_ACCOUNT_DATA, HandlePacket)


        'NOTE: These opcodes are only partialy handled by Cluster and must be handled by WorldServer
        PacketHandlers(OPCODES.MSG_MOVE_HEARTBEAT) = CType(AddressOf On_MSG_MOVE_HEARTBEAT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CANCEL_TRADE) = CType(AddressOf On_CMSG_CANCEL_TRADE, HandlePacket)



        'NOTE: These opcodes below must be exluded form WorldServer
        PacketHandlers(OPCODES.CMSG_PING) = CType(AddressOf On_CMSG_PING, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUTH_SESSION) = CType(AddressOf On_CMSG_AUTH_SESSION, HandlePacket)

        PacketHandlers(OPCODES.CMSG_CHAR_ENUM) = CType(AddressOf On_CMSG_CHAR_ENUM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHAR_CREATE) = CType(AddressOf On_CMSG_CHAR_CREATE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHAR_DELETE) = CType(AddressOf On_CMSG_CHAR_DELETE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHAR_RENAME) = CType(AddressOf On_CMSG_CHAR_RENAME, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PLAYER_LOGIN) = CType(AddressOf On_CMSG_PLAYER_LOGIN, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PLAYER_LOGOUT) = CType(AddressOf On_CMSG_PLAYER_LOGOUT, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_WORLDPORT_ACK) = CType(AddressOf On_MSG_MOVE_WORLDPORT_ACK, HandlePacket)

        PacketHandlers(OPCODES.CMSG_QUERY_TIME) = CType(AddressOf On_CMSG_QUERY_TIME, HandlePacket)
        PacketHandlers(OPCODES.CMSG_INSPECT) = CType(AddressOf On_CMSG_INSPECT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_WHO) = CType(AddressOf On_CMSG_WHO, HandlePacket)
        PacketHandlers(OPCODES.CMSG_WHOIS) = CType(AddressOf On_CMSG_WHOIS, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PLAYED_TIME) = CType(AddressOf On_CMSG_PLAYED_TIME, HandlePacket)
        PacketHandlers(OPCODES.CMSG_NAME_QUERY) = CType(AddressOf On_CMSG_NAME_QUERY, HandlePacket)

        PacketHandlers(OPCODES.CMSG_BUG) = CType(AddressOf On_CMSG_BUG, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GMTICKET_GETTICKET) = CType(AddressOf On_CMSG_GMTICKET_GETTICKET, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GMTICKET_CREATE) = CType(AddressOf On_CMSG_GMTICKET_CREATE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GMTICKET_SYSTEMSTATUS) = CType(AddressOf On_CMSG_GMTICKET_SYSTEMSTATUS, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GMTICKET_DELETETICKET) = CType(AddressOf On_CMSG_GMTICKET_DELETETICKET, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GMTICKET_UPDATETEXT) = CType(AddressOf On_CMSG_GMTICKET_UPDATETEXT, HandlePacket)

        PacketHandlers(OPCODES.CMSG_BATTLEMASTER_JOIN) = CType(AddressOf On_CMSG_BATTLEMASTER_JOIN, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BATTLEFIELD_PORT) = CType(AddressOf On_CMSG_BATTLEFIELD_PORT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LEAVE_BATTLEFIELD) = CType(AddressOf On_CMSG_LEAVE_BATTLEFIELD, HandlePacket)

        PacketHandlers(OPCODES.CMSG_FRIEND_LIST) = CType(AddressOf On_CMSG_FRIEND_LIST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ADD_FRIEND) = CType(AddressOf On_CMSG_ADD_FRIEND, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ADD_IGNORE) = CType(AddressOf On_CMSG_ADD_IGNORE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_DEL_FRIEND) = CType(AddressOf On_CMSG_DEL_FRIEND, HandlePacket)
        PacketHandlers(OPCODES.CMSG_DEL_IGNORE) = CType(AddressOf On_CMSG_DEL_IGNORE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_REQUEST_RAID_INFO) = CType(AddressOf On_CMSG_REQUEST_RAID_INFO, HandlePacket)

        PacketHandlers(OPCODES.CMSG_GROUP_INVITE) = CType(AddressOf On_CMSG_GROUP_INVITE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_CANCEL) = CType(AddressOf On_CMSG_GROUP_CANCEL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_ACCEPT) = CType(AddressOf On_CMSG_GROUP_ACCEPT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_DECLINE) = CType(AddressOf On_CMSG_GROUP_DECLINE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_UNINVITE) = CType(AddressOf On_CMSG_GROUP_UNINVITE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_UNINVITE_GUID) = CType(AddressOf On_CMSG_GROUP_UNINVITE_GUID, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_DISBAND) = CType(AddressOf On_CMSG_GROUP_DISBAND, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_RAID_CONVERT) = CType(AddressOf On_CMSG_GROUP_RAID_CONVERT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_SET_LEADER) = CType(AddressOf On_CMSG_GROUP_SET_LEADER, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_CHANGE_SUB_GROUP) = CType(AddressOf On_CMSG_GROUP_CHANGE_SUB_GROUP, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GROUP_SWAP_SUB_GROUP) = CType(AddressOf On_CMSG_GROUP_SWAP_SUB_GROUP, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LOOT_METHOD) = CType(AddressOf On_CMSG_LOOT_METHOD, HandlePacket)
        PacketHandlers(OPCODES.MSG_MINIMAP_PING) = CType(AddressOf On_MSG_MINIMAP_PING, HandlePacket)
        PacketHandlers(OPCODES.MSG_RANDOM_ROLL) = CType(AddressOf On_MSG_RANDOM_ROLL, HandlePacket)
        PacketHandlers(OPCODES.MSG_RAID_READY_CHECK) = CType(AddressOf On_MSG_RAID_READY_CHECK, HandlePacket)
        PacketHandlers(OPCODES.MSG_RAID_ICON_TARGET) = CType(AddressOf On_MSG_RAID_ICON_TARGET, HandlePacket)

        PacketHandlers(OPCODES.CMSG_REQUEST_PARTY_MEMBER_STATS) = CType(AddressOf On_CMSG_REQUEST_PARTY_MEMBER_STATS, HandlePacket)

        PacketHandlers(OPCODES.CMSG_TURN_IN_PETITION) = CType(AddressOf On_CMSG_TURN_IN_PETITION, HandlePacket)

        PacketHandlers(OPCODES.CMSG_GUILD_QUERY) = CType(AddressOf On_CMSG_GUILD_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_CREATE) = CType(AddressOf On_CMSG_GUILD_CREATE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_DISBAND) = CType(AddressOf On_CMSG_GUILD_DISBAND, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_ROSTER) = CType(AddressOf On_CMSG_GUILD_ROSTER, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_INFO) = CType(AddressOf On_CMSG_GUILD_INFO, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_RANK) = CType(AddressOf On_CMSG_GUILD_RANK, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_ADD_RANK) = CType(AddressOf On_CMSG_GUILD_ADD_RANK, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_DEL_RANK) = CType(AddressOf On_CMSG_GUILD_DEL_RANK, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_PROMOTE) = CType(AddressOf On_CMSG_GUILD_PROMOTE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_DEMOTE) = CType(AddressOf On_CMSG_GUILD_DEMOTE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_LEADER) = CType(AddressOf On_CMSG_GUILD_LEADER, HandlePacket)
        PacketHandlers(OPCODES.MSG_SAVE_GUILD_EMBLEM) = CType(AddressOf On_MSG_SAVE_GUILD_EMBLEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_SET_OFFICER_NOTE) = CType(AddressOf On_CMSG_GUILD_SET_OFFICER_NOTE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_SET_PUBLIC_NOTE) = CType(AddressOf On_CMSG_GUILD_SET_PUBLIC_NOTE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_MOTD) = CType(AddressOf On_CMSG_GUILD_MOTD, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_INVITE) = CType(AddressOf On_CMSG_GUILD_INVITE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_ACCEPT) = CType(AddressOf On_CMSG_GUILD_ACCEPT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_DECLINE) = CType(AddressOf On_CMSG_GUILD_DECLINE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_REMOVE) = CType(AddressOf On_CMSG_GUILD_REMOVE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GUILD_LEAVE) = CType(AddressOf On_CMSG_GUILD_LEAVE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_CHAT_IGNORED) = CType(AddressOf On_CMSG_CHAT_IGNORED, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MESSAGECHAT) = CType(AddressOf On_CMSG_MESSAGECHAT, HandlePacket)

        PacketHandlers(OPCODES.CMSG_JOIN_CHANNEL) = CType(AddressOf On_CMSG_JOIN_CHANNEL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LEAVE_CHANNEL) = CType(AddressOf On_CMSG_LEAVE_CHANNEL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_LIST) = CType(AddressOf On_CMSG_CHANNEL_LIST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_PASSWORD) = CType(AddressOf On_CMSG_CHANNEL_PASSWORD, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_SET_OWNER) = CType(AddressOf On_CMSG_CHANNEL_SET_OWNER, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_OWNER) = CType(AddressOf On_CMSG_CHANNEL_OWNER, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_MODERATOR) = CType(AddressOf On_CMSG_CHANNEL_MODERATOR, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_UNMODERATOR) = CType(AddressOf On_CMSG_CHANNEL_UNMODERATOR, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_MUTE) = CType(AddressOf On_CMSG_CHANNEL_MUTE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_UNMUTE) = CType(AddressOf On_CMSG_CHANNEL_UNMUTE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_KICK) = CType(AddressOf On_CMSG_CHANNEL_KICK, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_INVITE) = CType(AddressOf On_CMSG_CHANNEL_INVITE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_BAN) = CType(AddressOf On_CMSG_CHANNEL_BAN, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_UNBAN) = CType(AddressOf On_CMSG_CHANNEL_UNBAN, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_ANNOUNCEMENTS) = CType(AddressOf On_CMSG_CHANNEL_ANNOUNCEMENTS, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CHANNEL_MODERATE) = CType(AddressOf On_CMSG_CHANNEL_MODERATE, HandlePacket)


        'NOTE: TODO Opcodes
        '   none

    End Sub

    Public Sub OnUnhandledPacket(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Unhandled Packet]", Client.IP, Client.Port, CType(packet.OpCode, OPCODES))
    End Sub

    Public Sub OnClusterPacket(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Redirected Packet]", Client.IP, Client.Port, CType(packet.OpCode, OPCODES))

        If Client.Character Is Nothing OrElse Client.Character.IsInWorld = False Then
            Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", Client.IP, Client.Port, packet.OpCode, vbNewLine, packet.Length)
            DumpPacket(packet.Data, Client)
        Else
            Client.Character.GetWorld.ClientPacket(Client.Index, packet.Data)
        End If
    End Sub

End Module