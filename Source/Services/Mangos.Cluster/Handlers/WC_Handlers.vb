'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports Mangos.Common.Globals
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Server
Imports Mangos.Common.Enums.Global


Namespace Handlers

    Public Class WC_Handlers

        Public Sub IntializePacketHandlers()
            'NOTE: These opcodes are not used in any way
            '_WorldCluster.PacketHandlers(OPCODES.CMSG_MOVE_TIME_SKIPPED) = AddressOf On_CMSG_MOVE_TIME_SKIPPED

            _WorldCluster.PacketHandlers(OPCODES.CMSG_NEXT_CINEMATIC_CAMERA) = AddressOf _WC_Handlers_Misc.On_CMSG_NEXT_CINEMATIC_CAMERA
            _WorldCluster.PacketHandlers(OPCODES.CMSG_COMPLETE_CINEMATIC) = AddressOf _WC_Handlers_Misc.On_CMSG_COMPLETE_CINEMATIC
            _WorldCluster.PacketHandlers(OPCODES.CMSG_UPDATE_ACCOUNT_DATA) = AddressOf _WC_Handlers_Auth.On_CMSG_UPDATE_ACCOUNT_DATA
            _WorldCluster.PacketHandlers(OPCODES.CMSG_REQUEST_ACCOUNT_DATA) = AddressOf _WC_Handlers_Auth.On_CMSG_REQUEST_ACCOUNT_DATA

            'NOTE: These opcodes are only partialy handled by Cluster and must be handled by WorldServer
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_HEARTBEAT) = AddressOf _WC_Handlers_Misc.On_MSG_MOVE_HEARTBEAT
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CANCEL_TRADE) = AddressOf _WC_Handlers_Misc.On_CMSG_CANCEL_TRADE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_LOGOUT_CANCEL) = AddressOf _WC_Handlers_Misc.On_CMSG_LOGOUT_CANCEL

            'NOTE: These opcodes below must be exluded form WorldServer
            _WorldCluster.PacketHandlers(OPCODES.CMSG_PING) = AddressOf _WC_Handlers_Auth.On_CMSG_PING
            _WorldCluster.PacketHandlers(OPCODES.CMSG_AUTH_SESSION) = AddressOf _WC_Handlers_Auth.On_CMSG_AUTH_SESSION

            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHAR_ENUM) = AddressOf _WC_Handlers_Auth.On_CMSG_CHAR_ENUM
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHAR_CREATE) = AddressOf _WC_Handlers_Auth.On_CMSG_CHAR_CREATE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHAR_DELETE) = AddressOf _WC_Handlers_Auth.On_CMSG_CHAR_DELETE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHAR_RENAME) = AddressOf _WC_Handlers_Auth.On_CMSG_CHAR_RENAME
            _WorldCluster.PacketHandlers(OPCODES.CMSG_PLAYER_LOGIN) = AddressOf _WC_Handlers_Auth.On_CMSG_PLAYER_LOGIN
            _WorldCluster.PacketHandlers(OPCODES.CMSG_PLAYER_LOGOUT) = AddressOf _WC_Handlers_Auth.On_CMSG_PLAYER_LOGOUT
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_WORLDPORT_ACK) = AddressOf _WC_Handlers_Auth.On_MSG_MOVE_WORLDPORT_ACK

            _WorldCluster.PacketHandlers(OPCODES.CMSG_QUERY_TIME) = AddressOf _WC_Handlers_Misc.On_CMSG_QUERY_TIME
            _WorldCluster.PacketHandlers(OPCODES.CMSG_INSPECT) = AddressOf _WC_Handlers_Misc.On_CMSG_INSPECT
            _WorldCluster.PacketHandlers(OPCODES.CMSG_WHO) = AddressOf _WC_Handlers_Social.On_CMSG_WHO
            _WorldCluster.PacketHandlers(OPCODES.CMSG_WHOIS) = AddressOf _WC_Handlers_Tickets.On_CMSG_WHOIS
            _WorldCluster.PacketHandlers(OPCODES.CMSG_PLAYED_TIME) = AddressOf _WC_Handlers_Misc.On_CMSG_PLAYED_TIME
            _WorldCluster.PacketHandlers(OPCODES.CMSG_NAME_QUERY) = AddressOf _WC_Handlers_Misc.On_CMSG_NAME_QUERY

            _WorldCluster.PacketHandlers(OPCODES.CMSG_BUG) = AddressOf _WC_Handlers_Tickets.On_CMSG_BUG
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GMTICKET_GETTICKET) = AddressOf _WC_Handlers_Tickets.On_CMSG_GMTICKET_GETTICKET
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GMTICKET_CREATE) = AddressOf _WC_Handlers_Tickets.On_CMSG_GMTICKET_CREATE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GMTICKET_SYSTEMSTATUS) = AddressOf _WC_Handlers_Tickets.On_CMSG_GMTICKET_SYSTEMSTATUS
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GMTICKET_DELETETICKET) = AddressOf _WC_Handlers_Tickets.On_CMSG_GMTICKET_DELETETICKET
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GMTICKET_UPDATETEXT) = AddressOf _WC_Handlers_Tickets.On_CMSG_GMTICKET_UPDATETEXT

            _WorldCluster.PacketHandlers(OPCODES.CMSG_BATTLEMASTER_JOIN) = AddressOf _WC_Handlers_Battleground.On_CMSG_BATTLEMASTER_JOIN
            _WorldCluster.PacketHandlers(OPCODES.CMSG_BATTLEFIELD_PORT) = AddressOf _WC_Handlers_Battleground.On_CMSG_BATTLEFIELD_PORT
            _WorldCluster.PacketHandlers(OPCODES.CMSG_LEAVE_BATTLEFIELD) = AddressOf _WC_Handlers_Battleground.On_CMSG_LEAVE_BATTLEFIELD
            _WorldCluster.PacketHandlers(OPCODES.MSG_BATTLEGROUND_PLAYER_POSITIONS) = AddressOf _WC_Handlers_Battleground.On_MSG_BATTLEGROUND_PLAYER_POSITIONS

            _WorldCluster.PacketHandlers(OPCODES.CMSG_FRIEND_LIST) = AddressOf _WC_Handlers_Social.On_CMSG_FRIEND_LIST
            _WorldCluster.PacketHandlers(OPCODES.CMSG_ADD_FRIEND) = AddressOf _WC_Handlers_Social.On_CMSG_ADD_FRIEND
            _WorldCluster.PacketHandlers(OPCODES.CMSG_ADD_IGNORE) = AddressOf _WC_Handlers_Social.On_CMSG_ADD_IGNORE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_DEL_FRIEND) = AddressOf _WC_Handlers_Social.On_CMSG_DEL_FRIEND
            _WorldCluster.PacketHandlers(OPCODES.CMSG_DEL_IGNORE) = AddressOf _WC_Handlers_Social.On_CMSG_DEL_IGNORE

            _WorldCluster.PacketHandlers(OPCODES.CMSG_REQUEST_RAID_INFO) = AddressOf _WC_Handlers_Group.On_CMSG_REQUEST_RAID_INFO

            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_INVITE) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_INVITE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_CANCEL) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_CANCEL
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_ACCEPT) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_ACCEPT
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_DECLINE) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_DECLINE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_UNINVITE) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_UNINVITE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_UNINVITE_GUID) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_UNINVITE_GUID
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_DISBAND) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_DISBAND
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_RAID_CONVERT) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_RAID_CONVERT
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_SET_LEADER) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_SET_LEADER
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_CHANGE_SUB_GROUP) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_CHANGE_SUB_GROUP
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GROUP_SWAP_SUB_GROUP) = AddressOf _WC_Handlers_Group.On_CMSG_GROUP_SWAP_SUB_GROUP
            _WorldCluster.PacketHandlers(OPCODES.CMSG_LOOT_METHOD) = AddressOf _WC_Handlers_Group.On_CMSG_LOOT_METHOD
            _WorldCluster.PacketHandlers(OPCODES.MSG_MINIMAP_PING) = AddressOf _WC_Handlers_Group.On_MSG_MINIMAP_PING
            _WorldCluster.PacketHandlers(OPCODES.MSG_RANDOM_ROLL) = AddressOf _WC_Handlers_Group.On_MSG_RANDOM_ROLL
            _WorldCluster.PacketHandlers(OPCODES.MSG_RAID_READY_CHECK) = AddressOf _WC_Handlers_Group.On_MSG_RAID_READY_CHECK
            _WorldCluster.PacketHandlers(OPCODES.MSG_RAID_ICON_TARGET) = AddressOf _WC_Handlers_Group.On_MSG_RAID_ICON_TARGET

            _WorldCluster.PacketHandlers(OPCODES.CMSG_REQUEST_PARTY_MEMBER_STATS) = AddressOf _WC_Handlers_Group.On_CMSG_REQUEST_PARTY_MEMBER_STATS

            _WorldCluster.PacketHandlers(OPCODES.CMSG_TURN_IN_PETITION) = AddressOf _WC_Handlers_Guild.On_CMSG_TURN_IN_PETITION

            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_QUERY) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_QUERY
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_CREATE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_CREATE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_DISBAND) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_DISBAND
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_ROSTER) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_ROSTER
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_INFO) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_INFO
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_RANK) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_RANK
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_ADD_RANK) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_ADD_RANK
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_DEL_RANK) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_DEL_RANK
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_PROMOTE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_PROMOTE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_DEMOTE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_DEMOTE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_LEADER) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_LEADER
            _WorldCluster.PacketHandlers(OPCODES.MSG_SAVE_GUILD_EMBLEM) = AddressOf _WC_Handlers_Guild.On_MSG_SAVE_GUILD_EMBLEM
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_SET_OFFICER_NOTE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_SET_OFFICER_NOTE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_SET_PUBLIC_NOTE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_SET_PUBLIC_NOTE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_MOTD) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_MOTD
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_INVITE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_INVITE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_ACCEPT) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_ACCEPT
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_DECLINE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_DECLINE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_REMOVE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_REMOVE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_GUILD_LEAVE) = AddressOf _WC_Handlers_Guild.On_CMSG_GUILD_LEAVE

            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHAT_IGNORED) = AddressOf _WC_Handlers_Chat.On_CMSG_CHAT_IGNORED
            _WorldCluster.PacketHandlers(OPCODES.CMSG_MESSAGECHAT) = AddressOf _WC_Handlers_Chat.On_CMSG_MESSAGECHAT

            _WorldCluster.PacketHandlers(OPCODES.CMSG_JOIN_CHANNEL) = AddressOf _WC_Handlers_Chat.On_CMSG_JOIN_CHANNEL
            _WorldCluster.PacketHandlers(OPCODES.CMSG_LEAVE_CHANNEL) = AddressOf _WC_Handlers_Chat.On_CMSG_LEAVE_CHANNEL
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_LIST) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_LIST
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_PASSWORD) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_PASSWORD
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_SET_OWNER) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_SET_OWNER
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_OWNER) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_OWNER
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_MODERATOR) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_MODERATOR
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_UNMODERATOR) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_UNMODERATOR
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_MUTE) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_MUTE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_UNMUTE) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_UNMUTE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_KICK) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_KICK
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_INVITE) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_INVITE
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_BAN) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_BAN
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_UNBAN) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_UNBAN
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_ANNOUNCEMENTS) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_ANNOUNCEMENTS
            _WorldCluster.PacketHandlers(OPCODES.CMSG_CHANNEL_MODERATE) = AddressOf _WC_Handlers_Chat.On_CMSG_CHANNEL_MODERATE

            'Opcodes redirected from the WorldServer
            'Commonly occurs while trying to peform actions while loading/transfering
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_BACKWARD) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_FORWARD) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_DOWN) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_UP) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_LEFT) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_RIGHT) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_SWIM) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_TURN_LEFT) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_START_TURN_RIGHT) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_STOP) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_STOP_PITCH) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_STOP_STRAFE) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_STOP_SWIM) = AddressOf OnClusterPacket
            _WorldCluster.PacketHandlers(OPCODES.MSG_MOVE_STOP_TURN) = AddressOf OnClusterPacket

            'NOTE: TODO Opcodes
            '   none

        End Sub

        Public Sub OnUnhandledPacket(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            _WorldCluster.Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Unhandled Packet]", client.IP, client.Port, packet.OpCode)
        End Sub

        Public Sub OnClusterPacket(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            _WorldCluster.Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Redirected Packet]", client.IP, client.Port, packet.OpCode)

            If client.Character Is Nothing OrElse client.Character.IsInWorld = False Then
                _WorldCluster.Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", client.IP, client.Port, packet.OpCode, vbCrLf, packet.Length)
                _Packets.DumpPacket(packet.Data, client)
            Else
                client.Character.GetWorld.ClientPacket(client.Index, packet.Data)
            End If
        End Sub

    End Class
End Namespace