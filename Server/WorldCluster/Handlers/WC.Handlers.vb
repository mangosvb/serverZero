'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getmangos.eu>
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


Imports mangosVB.Common

Public Module WC_Handlers

    Public Sub IntializePacketHandlers()
        'NOTE: These opcodes are not used in any way
        'PacketHandlers(OPCODES.CMSG_MOVE_TIME_SKIPPED) = AddressOf On_CMSG_MOVE_TIME_SKIPPED

        PacketHandlers(OPCODES.CMSG_NEXT_CINEMATIC_CAMERA) = AddressOf On_CMSG_NEXT_CINEMATIC_CAMERA
        PacketHandlers(OPCODES.CMSG_COMPLETE_CINEMATIC) = AddressOf On_CMSG_COMPLETE_CINEMATIC
        PacketHandlers(OPCODES.CMSG_UPDATE_ACCOUNT_DATA) = AddressOf On_CMSG_UPDATE_ACCOUNT_DATA
        PacketHandlers(OPCODES.CMSG_REQUEST_ACCOUNT_DATA) = AddressOf On_CMSG_REQUEST_ACCOUNT_DATA

        'NOTE: These opcodes are only partialy handled by Cluster and must be handled by WorldServer
        PacketHandlers(OPCODES.MSG_MOVE_HEARTBEAT) = AddressOf On_MSG_MOVE_HEARTBEAT
        PacketHandlers(OPCODES.CMSG_CANCEL_TRADE) = AddressOf On_CMSG_CANCEL_TRADE

        'NOTE: These opcodes below must be exluded form WorldServer
        PacketHandlers(OPCODES.CMSG_PING) = AddressOf On_CMSG_PING
        PacketHandlers(OPCODES.CMSG_AUTH_SESSION) = AddressOf On_CMSG_AUTH_SESSION

        PacketHandlers(OPCODES.CMSG_CHAR_ENUM) = AddressOf On_CMSG_CHAR_ENUM
        PacketHandlers(OPCODES.CMSG_CHAR_CREATE) = AddressOf On_CMSG_CHAR_CREATE
        PacketHandlers(OPCODES.CMSG_CHAR_DELETE) = AddressOf On_CMSG_CHAR_DELETE
        PacketHandlers(OPCODES.CMSG_CHAR_RENAME) = AddressOf On_CMSG_CHAR_RENAME
        PacketHandlers(OPCODES.CMSG_PLAYER_LOGIN) = AddressOf On_CMSG_PLAYER_LOGIN
        PacketHandlers(OPCODES.CMSG_PLAYER_LOGOUT) = AddressOf On_CMSG_PLAYER_LOGOUT
        PacketHandlers(OPCODES.MSG_MOVE_WORLDPORT_ACK) = AddressOf On_MSG_MOVE_WORLDPORT_ACK

        PacketHandlers(OPCODES.CMSG_QUERY_TIME) = AddressOf On_CMSG_QUERY_TIME
        PacketHandlers(OPCODES.CMSG_INSPECT) = AddressOf On_CMSG_INSPECT
        PacketHandlers(OPCODES.CMSG_WHO) = AddressOf On_CMSG_WHO
        PacketHandlers(OPCODES.CMSG_WHOIS) = AddressOf On_CMSG_WHOIS
        PacketHandlers(OPCODES.CMSG_PLAYED_TIME) = AddressOf On_CMSG_PLAYED_TIME
        PacketHandlers(OPCODES.CMSG_NAME_QUERY) = AddressOf On_CMSG_NAME_QUERY

        PacketHandlers(OPCODES.CMSG_BUG) = AddressOf On_CMSG_BUG
        PacketHandlers(OPCODES.CMSG_GMTICKET_GETTICKET) = AddressOf On_CMSG_GMTICKET_GETTICKET
        PacketHandlers(OPCODES.CMSG_GMTICKET_CREATE) = AddressOf On_CMSG_GMTICKET_CREATE
        PacketHandlers(OPCODES.CMSG_GMTICKET_SYSTEMSTATUS) = AddressOf On_CMSG_GMTICKET_SYSTEMSTATUS
        PacketHandlers(OPCODES.CMSG_GMTICKET_DELETETICKET) = AddressOf On_CMSG_GMTICKET_DELETETICKET
        PacketHandlers(OPCODES.CMSG_GMTICKET_UPDATETEXT) = AddressOf On_CMSG_GMTICKET_UPDATETEXT

        PacketHandlers(OPCODES.CMSG_BATTLEMASTER_JOIN) = AddressOf On_CMSG_BATTLEMASTER_JOIN
        PacketHandlers(OPCODES.CMSG_BATTLEFIELD_PORT) = AddressOf On_CMSG_BATTLEFIELD_PORT
        PacketHandlers(OPCODES.CMSG_LEAVE_BATTLEFIELD) = AddressOf On_CMSG_LEAVE_BATTLEFIELD

        PacketHandlers(OPCODES.CMSG_FRIEND_LIST) = AddressOf On_CMSG_FRIEND_LIST
        PacketHandlers(OPCODES.CMSG_ADD_FRIEND) = AddressOf On_CMSG_ADD_FRIEND
        PacketHandlers(OPCODES.CMSG_ADD_IGNORE) = AddressOf On_CMSG_ADD_IGNORE
        PacketHandlers(OPCODES.CMSG_DEL_FRIEND) = AddressOf On_CMSG_DEL_FRIEND
        PacketHandlers(OPCODES.CMSG_DEL_IGNORE) = AddressOf On_CMSG_DEL_IGNORE

        PacketHandlers(OPCODES.CMSG_REQUEST_RAID_INFO) = AddressOf On_CMSG_REQUEST_RAID_INFO

        PacketHandlers(OPCODES.CMSG_GROUP_INVITE) = AddressOf On_CMSG_GROUP_INVITE
        PacketHandlers(OPCODES.CMSG_GROUP_CANCEL) = AddressOf On_CMSG_GROUP_CANCEL
        PacketHandlers(OPCODES.CMSG_GROUP_ACCEPT) = AddressOf On_CMSG_GROUP_ACCEPT
        PacketHandlers(OPCODES.CMSG_GROUP_DECLINE) = AddressOf On_CMSG_GROUP_DECLINE
        PacketHandlers(OPCODES.CMSG_GROUP_UNINVITE) = AddressOf On_CMSG_GROUP_UNINVITE
        PacketHandlers(OPCODES.CMSG_GROUP_UNINVITE_GUID) = AddressOf On_CMSG_GROUP_UNINVITE_GUID
        PacketHandlers(OPCODES.CMSG_GROUP_DISBAND) = AddressOf On_CMSG_GROUP_DISBAND
        PacketHandlers(OPCODES.CMSG_GROUP_RAID_CONVERT) = AddressOf On_CMSG_GROUP_RAID_CONVERT
        PacketHandlers(OPCODES.CMSG_GROUP_SET_LEADER) = AddressOf On_CMSG_GROUP_SET_LEADER
        PacketHandlers(OPCODES.CMSG_GROUP_CHANGE_SUB_GROUP) = AddressOf On_CMSG_GROUP_CHANGE_SUB_GROUP
        PacketHandlers(OPCODES.CMSG_GROUP_SWAP_SUB_GROUP) = AddressOf On_CMSG_GROUP_SWAP_SUB_GROUP
        PacketHandlers(OPCODES.CMSG_LOOT_METHOD) = AddressOf On_CMSG_LOOT_METHOD
        PacketHandlers(OPCODES.MSG_MINIMAP_PING) = AddressOf On_MSG_MINIMAP_PING
        PacketHandlers(OPCODES.MSG_RANDOM_ROLL) = AddressOf On_MSG_RANDOM_ROLL
        PacketHandlers(OPCODES.MSG_RAID_READY_CHECK) = AddressOf On_MSG_RAID_READY_CHECK
        PacketHandlers(OPCODES.MSG_RAID_ICON_TARGET) = AddressOf On_MSG_RAID_ICON_TARGET

        PacketHandlers(OPCODES.CMSG_REQUEST_PARTY_MEMBER_STATS) = AddressOf On_CMSG_REQUEST_PARTY_MEMBER_STATS

        PacketHandlers(OPCODES.CMSG_TURN_IN_PETITION) = AddressOf On_CMSG_TURN_IN_PETITION

        PacketHandlers(OPCODES.CMSG_GUILD_QUERY) = AddressOf On_CMSG_GUILD_QUERY
        PacketHandlers(OPCODES.CMSG_GUILD_CREATE) = AddressOf On_CMSG_GUILD_CREATE
        PacketHandlers(OPCODES.CMSG_GUILD_DISBAND) = AddressOf On_CMSG_GUILD_DISBAND
        PacketHandlers(OPCODES.CMSG_GUILD_ROSTER) = AddressOf On_CMSG_GUILD_ROSTER
        PacketHandlers(OPCODES.CMSG_GUILD_INFO) = AddressOf On_CMSG_GUILD_INFO
        PacketHandlers(OPCODES.CMSG_GUILD_RANK) = AddressOf On_CMSG_GUILD_RANK
        PacketHandlers(OPCODES.CMSG_GUILD_ADD_RANK) = AddressOf On_CMSG_GUILD_ADD_RANK
        PacketHandlers(OPCODES.CMSG_GUILD_DEL_RANK) = AddressOf On_CMSG_GUILD_DEL_RANK
        PacketHandlers(OPCODES.CMSG_GUILD_PROMOTE) = AddressOf On_CMSG_GUILD_PROMOTE
        PacketHandlers(OPCODES.CMSG_GUILD_DEMOTE) = AddressOf On_CMSG_GUILD_DEMOTE
        PacketHandlers(OPCODES.CMSG_GUILD_LEADER) = AddressOf On_CMSG_GUILD_LEADER
        PacketHandlers(OPCODES.MSG_SAVE_GUILD_EMBLEM) = AddressOf On_MSG_SAVE_GUILD_EMBLEM
        PacketHandlers(OPCODES.CMSG_GUILD_SET_OFFICER_NOTE) = AddressOf On_CMSG_GUILD_SET_OFFICER_NOTE
        PacketHandlers(OPCODES.CMSG_GUILD_SET_PUBLIC_NOTE) = AddressOf On_CMSG_GUILD_SET_PUBLIC_NOTE
        PacketHandlers(OPCODES.CMSG_GUILD_MOTD) = AddressOf On_CMSG_GUILD_MOTD
        PacketHandlers(OPCODES.CMSG_GUILD_INVITE) = AddressOf On_CMSG_GUILD_INVITE
        PacketHandlers(OPCODES.CMSG_GUILD_ACCEPT) = AddressOf On_CMSG_GUILD_ACCEPT
        PacketHandlers(OPCODES.CMSG_GUILD_DECLINE) = AddressOf On_CMSG_GUILD_DECLINE
        PacketHandlers(OPCODES.CMSG_GUILD_REMOVE) = AddressOf On_CMSG_GUILD_REMOVE
        PacketHandlers(OPCODES.CMSG_GUILD_LEAVE) = AddressOf On_CMSG_GUILD_LEAVE

        PacketHandlers(OPCODES.CMSG_CHAT_IGNORED) = AddressOf On_CMSG_CHAT_IGNORED
        PacketHandlers(OPCODES.CMSG_MESSAGECHAT) = AddressOf On_CMSG_MESSAGECHAT

        PacketHandlers(OPCODES.CMSG_JOIN_CHANNEL) = AddressOf On_CMSG_JOIN_CHANNEL
        PacketHandlers(OPCODES.CMSG_LEAVE_CHANNEL) = AddressOf On_CMSG_LEAVE_CHANNEL
        PacketHandlers(OPCODES.CMSG_CHANNEL_LIST) = AddressOf On_CMSG_CHANNEL_LIST
        PacketHandlers(OPCODES.CMSG_CHANNEL_PASSWORD) = AddressOf On_CMSG_CHANNEL_PASSWORD
        PacketHandlers(OPCODES.CMSG_CHANNEL_SET_OWNER) = AddressOf On_CMSG_CHANNEL_SET_OWNER
        PacketHandlers(OPCODES.CMSG_CHANNEL_OWNER) = AddressOf On_CMSG_CHANNEL_OWNER
        PacketHandlers(OPCODES.CMSG_CHANNEL_MODERATOR) = AddressOf On_CMSG_CHANNEL_MODERATOR
        PacketHandlers(OPCODES.CMSG_CHANNEL_UNMODERATOR) = AddressOf On_CMSG_CHANNEL_UNMODERATOR
        PacketHandlers(OPCODES.CMSG_CHANNEL_MUTE) = AddressOf On_CMSG_CHANNEL_MUTE
        PacketHandlers(OPCODES.CMSG_CHANNEL_UNMUTE) = AddressOf On_CMSG_CHANNEL_UNMUTE
        PacketHandlers(OPCODES.CMSG_CHANNEL_KICK) = AddressOf On_CMSG_CHANNEL_KICK
        PacketHandlers(OPCODES.CMSG_CHANNEL_INVITE) = AddressOf On_CMSG_CHANNEL_INVITE
        PacketHandlers(OPCODES.CMSG_CHANNEL_BAN) = AddressOf On_CMSG_CHANNEL_BAN
        PacketHandlers(OPCODES.CMSG_CHANNEL_UNBAN) = AddressOf On_CMSG_CHANNEL_UNBAN
        PacketHandlers(OPCODES.CMSG_CHANNEL_ANNOUNCEMENTS) = AddressOf On_CMSG_CHANNEL_ANNOUNCEMENTS
        PacketHandlers(OPCODES.CMSG_CHANNEL_MODERATE) = AddressOf On_CMSG_CHANNEL_MODERATE

        'NOTE: TODO Opcodes
        '   none

    End Sub

    Public Sub OnUnhandledPacket(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Unhandled Packet]", client.IP, client.Port, packet.OpCode)
    End Sub

    Public Sub OnClusterPacket(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Redirected Packet]", client.IP, client.Port, packet.OpCode)

        If client.Character Is Nothing OrElse client.Character.IsInWorld = False Then
            Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", client.IP, client.Port, packet.OpCode, vbNewLine, packet.Length)
            DumpPacket(packet.Data, Client)
        Else
            client.Character.GetWorld.ClientPacket(Client.Index, packet.Data)
        End If
    End Sub

End Module