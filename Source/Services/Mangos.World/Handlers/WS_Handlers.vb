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

Imports Mangos.Common.Enums
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Server

Namespace Handlers

    Public Module WS_Handlers

        Public Sub IntializePacketHandlers()
            'NOTE: These opcodes are not used in any way
            PacketHandlers(OPCODES.CMSG_FORCE_MOVE_ROOT_ACK) = AddressOf OnUnhandledPacket
            PacketHandlers(OPCODES.CMSG_FORCE_MOVE_UNROOT_ACK) = AddressOf OnUnhandledPacket
            PacketHandlers(OPCODES.CMSG_MOVE_WATER_WALK_ACK) = AddressOf OnUnhandledPacket
            PacketHandlers(OPCODES.MSG_MOVE_TELEPORT_ACK) = AddressOf OnUnhandledPacket

            'NOTE: These opcodes below must be exluded form Cluster
            PacketHandlers(OPCODES.CMSG_WARDEN_DATA) = AddressOf On_CMSG_WARDEN_DATA
            PacketHandlers(OPCODES.CMSG_NAME_QUERY) = AddressOf On_CMSG_NAME_QUERY
            PacketHandlers(OPCODES.CMSG_MESSAGECHAT) = AddressOf On_CMSG_MESSAGECHAT

            PacketHandlers(OPCODES.CMSG_LOGOUT_REQUEST) = AddressOf On_CMSG_LOGOUT_REQUEST
            PacketHandlers(OPCODES.CMSG_LOGOUT_CANCEL) = AddressOf On_CMSG_LOGOUT_CANCEL
            PacketHandlers(OPCODES.CMSG_CANCEL_TRADE) = AddressOf On_CMSG_CANCEL_TRADE
            PacketHandlers(OPCODES.CMSG_BEGIN_TRADE) = AddressOf On_CMSG_BEGIN_TRADE
            PacketHandlers(OPCODES.CMSG_UNACCEPT_TRADE) = AddressOf On_CMSG_UNACCEPT_TRADE
            PacketHandlers(OPCODES.CMSG_ACCEPT_TRADE) = AddressOf On_CMSG_ACCEPT_TRADE
            PacketHandlers(OPCODES.CMSG_INITIATE_TRADE) = AddressOf On_CMSG_INITIATE_TRADE
            PacketHandlers(OPCODES.CMSG_SET_TRADE_GOLD) = AddressOf On_CMSG_SET_TRADE_GOLD
            PacketHandlers(OPCODES.CMSG_SET_TRADE_ITEM) = AddressOf On_CMSG_SET_TRADE_ITEM
            PacketHandlers(OPCODES.CMSG_CLEAR_TRADE_ITEM) = AddressOf On_CMSG_CLEAR_TRADE_ITEM
            PacketHandlers(OPCODES.CMSG_IGNORE_TRADE) = AddressOf On_CMSG_IGNORE_TRADE
            PacketHandlers(OPCODES.CMSG_BUSY_TRADE) = AddressOf On_CMSG_BUSY_TRADE

            PacketHandlers(OPCODES.MSG_MOVE_START_FORWARD) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_BACKWARD) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_STOP) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_LEFT) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_RIGHT) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_STOP_STRAFE) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_JUMP) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_TURN_LEFT) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_TURN_RIGHT) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_STOP_TURN) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_UP) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_DOWN) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_STOP_PITCH) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_SET_RUN_MODE) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_SET_WALK_MODE) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_START_SWIM) = AddressOf OnStartSwim
            PacketHandlers(OPCODES.MSG_MOVE_STOP_SWIM) = AddressOf OnStopSwim
            PacketHandlers(OPCODES.MSG_MOVE_SET_FACING) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.MSG_MOVE_SET_PITCH) = AddressOf OnMovementPacket
            PacketHandlers(OPCODES.CMSG_MOVE_FALL_RESET) = AddressOf On_CMSG_MOVE_FALL_RESET

            PacketHandlers(OPCODES.MSG_MOVE_HEARTBEAT) = AddressOf On_MSG_MOVE_HEARTBEAT
            PacketHandlers(OPCODES.CMSG_AREATRIGGER) = AddressOf On_CMSG_AREATRIGGER
            PacketHandlers(OPCODES.MSG_MOVE_FALL_LAND) = AddressOf On_MSG_MOVE_FALL_LAND
            PacketHandlers(OPCODES.CMSG_ZONEUPDATE) = AddressOf On_CMSG_ZONEUPDATE
            PacketHandlers(OPCODES.CMSG_FORCE_RUN_SPEED_CHANGE_ACK) = AddressOf OnChangeSpeed
            PacketHandlers(OPCODES.CMSG_FORCE_RUN_BACK_SPEED_CHANGE_ACK) = AddressOf OnChangeSpeed
            PacketHandlers(OPCODES.CMSG_FORCE_SWIM_SPEED_CHANGE_ACK) = AddressOf OnChangeSpeed
            PacketHandlers(OPCODES.CMSG_FORCE_SWIM_BACK_SPEED_CHANGE_ACK) = AddressOf OnChangeSpeed
            PacketHandlers(OPCODES.CMSG_FORCE_TURN_RATE_CHANGE_ACK) = AddressOf OnChangeSpeed

            PacketHandlers(OPCODES.CMSG_STANDSTATECHANGE) = AddressOf On_CMSG_STANDSTATECHANGE
            PacketHandlers(OPCODES.CMSG_SET_SELECTION) = AddressOf On_CMSG_SET_SELECTION
            PacketHandlers(OPCODES.CMSG_REPOP_REQUEST) = AddressOf On_CMSG_REPOP_REQUEST
            PacketHandlers(OPCODES.MSG_CORPSE_QUERY) = AddressOf On_MSG_CORPSE_QUERY
            PacketHandlers(OPCODES.CMSG_SPIRIT_HEALER_ACTIVATE) = AddressOf On_CMSG_SPIRIT_HEALER_ACTIVATE
            PacketHandlers(OPCODES.CMSG_RECLAIM_CORPSE) = AddressOf On_CMSG_RECLAIM_CORPSE

            PacketHandlers(OPCODES.CMSG_TUTORIAL_FLAG) = AddressOf On_CMSG_TUTORIAL_FLAG
            PacketHandlers(OPCODES.CMSG_TUTORIAL_CLEAR) = AddressOf On_CMSG_TUTORIAL_CLEAR
            PacketHandlers(OPCODES.CMSG_TUTORIAL_RESET) = AddressOf On_CMSG_TUTORIAL_RESET
            PacketHandlers(OPCODES.CMSG_SET_ACTION_BUTTON) = AddressOf On_CMSG_SET_ACTION_BUTTON
            PacketHandlers(OPCODES.CMSG_SET_ACTIONBAR_TOGGLES) = AddressOf On_CMSG_SET_ACTIONBAR_TOGGLES
            PacketHandlers(OPCODES.CMSG_TOGGLE_HELM) = AddressOf On_CMSG_TOGGLE_HELM
            PacketHandlers(OPCODES.CMSG_TOGGLE_CLOAK) = AddressOf On_CMSG_TOGGLE_CLOAK
            PacketHandlers(OPCODES.CMSG_MOUNTSPECIAL_ANIM) = AddressOf On_CMSG_MOUNTSPECIAL_ANIM
            PacketHandlers(OPCODES.CMSG_EMOTE) = AddressOf On_CMSG_EMOTE
            PacketHandlers(OPCODES.CMSG_TEXT_EMOTE) = AddressOf On_CMSG_TEXT_EMOTE

            PacketHandlers(OPCODES.CMSG_ITEM_QUERY_SINGLE) = AddressOf On_CMSG_ITEM_QUERY_SINGLE
            PacketHandlers(OPCODES.CMSG_ITEM_NAME_QUERY) = AddressOf On_CMSG_ITEM_NAME_QUERY
            PacketHandlers(OPCODES.CMSG_SETSHEATHED) = AddressOf On_CMSG_SETSHEATHED
            PacketHandlers(OPCODES.CMSG_SWAP_INV_ITEM) = AddressOf On_CMSG_SWAP_INV_ITEM
            PacketHandlers(OPCODES.CMSG_SPLIT_ITEM) = AddressOf On_CMSG_SPLIT_ITEM
            PacketHandlers(OPCODES.CMSG_AUTOEQUIP_ITEM) = AddressOf On_CMSG_AUTOEQUIP_ITEM
            PacketHandlers(OPCODES.CMSG_AUTOSTORE_BAG_ITEM) = AddressOf On_CMSG_AUTOSTORE_BAG_ITEM
            PacketHandlers(OPCODES.CMSG_SWAP_ITEM) = AddressOf On_CMSG_SWAP_ITEM
            PacketHandlers(OPCODES.CMSG_DESTROYITEM) = AddressOf On_CMSG_DESTROYITEM
            PacketHandlers(OPCODES.CMSG_READ_ITEM) = AddressOf On_CMSG_READ_ITEM
            PacketHandlers(OPCODES.CMSG_PAGE_TEXT_QUERY) = AddressOf On_CMSG_PAGE_TEXT_QUERY
            PacketHandlers(OPCODES.CMSG_USE_ITEM) = AddressOf On_CMSG_USE_ITEM
            PacketHandlers(OPCODES.CMSG_OPEN_ITEM) = AddressOf On_CMSG_OPEN_ITEM
            PacketHandlers(OPCODES.CMSG_WRAP_ITEM) = AddressOf On_CMSG_WRAP_ITEM
            PacketHandlers(OPCODES.CMSG_SET_AMMO) = AddressOf On_CMSG_SET_AMMO

            PacketHandlers(OPCODES.CMSG_CREATURE_QUERY) = AddressOf On_CMSG_CREATURE_QUERY
            PacketHandlers(OPCODES.CMSG_GOSSIP_HELLO) = AddressOf On_CMSG_GOSSIP_HELLO
            PacketHandlers(OPCODES.CMSG_GOSSIP_SELECT_OPTION) = AddressOf On_CMSG_GOSSIP_SELECT_OPTION
            PacketHandlers(OPCODES.CMSG_NPC_TEXT_QUERY) = AddressOf On_CMSG_NPC_TEXT_QUERY
            PacketHandlers(OPCODES.CMSG_LIST_INVENTORY) = AddressOf On_CMSG_LIST_INVENTORY
            PacketHandlers(OPCODES.CMSG_BUY_ITEM_IN_SLOT) = AddressOf On_CMSG_BUY_ITEM_IN_SLOT
            PacketHandlers(OPCODES.CMSG_BUY_ITEM) = AddressOf On_CMSG_BUY_ITEM
            PacketHandlers(OPCODES.CMSG_BUYBACK_ITEM) = AddressOf On_CMSG_BUYBACK_ITEM
            PacketHandlers(OPCODES.CMSG_SELL_ITEM) = AddressOf On_CMSG_SELL_ITEM
            PacketHandlers(OPCODES.CMSG_REPAIR_ITEM) = AddressOf On_CMSG_REPAIR_ITEM

            PacketHandlers(OPCODES.CMSG_ATTACKSWING) = AddressOf On_CMSG_ATTACKSWING
            PacketHandlers(OPCODES.CMSG_ATTACKSTOP) = AddressOf On_CMSG_ATTACKSTOP

            PacketHandlers(OPCODES.CMSG_GAMEOBJECT_QUERY) = AddressOf On_CMSG_GAMEOBJECT_QUERY
            PacketHandlers(OPCODES.CMSG_GAMEOBJ_USE) = AddressOf On_CMSG_GAMEOBJ_USE

            PacketHandlers(OPCODES.CMSG_BATTLEFIELD_STATUS) = AddressOf On_CMSG_BATTLEFIELD_STATUS
            PacketHandlers(OPCODES.CMSG_SET_ACTIVE_MOVER) = AddressOf On_CMSG_SET_ACTIVE_MOVER
            PacketHandlers(OPCODES.CMSG_MEETINGSTONE_INFO) = AddressOf On_CMSG_MEETINGSTONE_INFO
            PacketHandlers(OPCODES.MSG_INSPECT_HONOR_STATS) = AddressOf On_MSG_INSPECT_HONOR_STATS

            PacketHandlers(OPCODES.MSG_PVP_LOG_DATA) = AddressOf On_MSG_PVP_LOG_DATA

            PacketHandlers(OPCODES.CMSG_MOVE_TIME_SKIPPED) = AddressOf On_CMSG_MOVE_TIME_SKIPPED

            PacketHandlers(OPCODES.CMSG_GET_MAIL_LIST) = AddressOf On_CMSG_GET_MAIL_LIST
            PacketHandlers(OPCODES.CMSG_SEND_MAIL) = AddressOf On_CMSG_SEND_MAIL
            PacketHandlers(OPCODES.CMSG_MAIL_CREATE_TEXT_ITEM) = AddressOf On_CMSG_MAIL_CREATE_TEXT_ITEM
            PacketHandlers(OPCODES.CMSG_ITEM_TEXT_QUERY) = AddressOf On_CMSG_ITEM_TEXT_QUERY
            PacketHandlers(OPCODES.CMSG_MAIL_DELETE) = AddressOf On_CMSG_MAIL_DELETE
            PacketHandlers(OPCODES.CMSG_MAIL_TAKE_ITEM) = AddressOf On_CMSG_MAIL_TAKE_ITEM
            PacketHandlers(OPCODES.CMSG_MAIL_TAKE_MONEY) = AddressOf On_CMSG_MAIL_TAKE_MONEY
            PacketHandlers(OPCODES.CMSG_MAIL_RETURN_TO_SENDER) = AddressOf On_CMSG_MAIL_RETURN_TO_SENDER
            PacketHandlers(OPCODES.CMSG_MAIL_MARK_AS_READ) = AddressOf On_CMSG_MAIL_MARK_AS_READ
            PacketHandlers(OPCODES.MSG_QUERY_NEXT_MAIL_TIME) = AddressOf On_MSG_QUERY_NEXT_MAIL_TIME

            PacketHandlers(OPCODES.CMSG_AUTOSTORE_LOOT_ITEM) = AddressOf On_CMSG_AUTOSTORE_LOOT_ITEM
            PacketHandlers(OPCODES.CMSG_LOOT_MONEY) = AddressOf On_CMSG_LOOT_MONEY
            PacketHandlers(OPCODES.CMSG_LOOT) = AddressOf On_CMSG_LOOT
            PacketHandlers(OPCODES.CMSG_LOOT_ROLL) = AddressOf On_CMSG_LOOT_ROLL
            PacketHandlers(OPCODES.CMSG_LOOT_RELEASE) = AddressOf On_CMSG_LOOT_RELEASE

            PacketHandlers(OPCODES.CMSG_TAXINODE_STATUS_QUERY) = AddressOf On_CMSG_TAXINODE_STATUS_QUERY
            PacketHandlers(OPCODES.CMSG_TAXIQUERYAVAILABLENODES) = AddressOf On_CMSG_TAXIQUERYAVAILABLENODES
            PacketHandlers(OPCODES.CMSG_ACTIVATETAXI) = AddressOf On_CMSG_ACTIVATETAXI
            PacketHandlers(OPCODES.CMSG_ACTIVATETAXI_FAR) = AddressOf On_CMSG_ACTIVATETAXI_FAR
            PacketHandlers(OPCODES.CMSG_MOVE_SPLINE_DONE) = AddressOf On_CMSG_MOVE_SPLINE_DONE

            PacketHandlers(OPCODES.CMSG_CAST_SPELL) = AddressOf On_CMSG_CAST_SPELL
            PacketHandlers(OPCODES.CMSG_CANCEL_CAST) = AddressOf On_CMSG_CANCEL_CAST
            PacketHandlers(OPCODES.CMSG_CANCEL_AURA) = AddressOf On_CMSG_CANCEL_AURA
            PacketHandlers(OPCODES.CMSG_CANCEL_AUTO_REPEAT_SPELL) = AddressOf On_CMSG_CANCEL_AUTO_REPEAT_SPELL
            PacketHandlers(OPCODES.CMSG_CANCEL_CHANNELLING) = AddressOf On_CMSG_CANCEL_CHANNELLING

            PacketHandlers(OPCODES.CMSG_TOGGLE_PVP) = AddressOf On_CMSG_TOGGLE_PVP
            PacketHandlers(OPCODES.MSG_BATTLEGROUND_PLAYER_POSITIONS) = AddressOf On_MSG_BATTLEGROUND_PLAYER_POSITIONS

            PacketHandlers(OPCODES.CMSG_QUESTGIVER_STATUS_QUERY) = AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_STATUS_QUERY
            PacketHandlers(OPCODES.CMSG_QUESTGIVER_HELLO) = AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_HELLO
            PacketHandlers(OPCODES.CMSG_QUESTGIVER_QUERY_QUEST) = AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_QUERY_QUEST
            PacketHandlers(OPCODES.CMSG_QUESTGIVER_ACCEPT_QUEST) = AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_ACCEPT_QUEST
            PacketHandlers(OPCODES.CMSG_QUESTLOG_REMOVE_QUEST) = AddressOf ALLQUESTS.On_CMSG_QUESTLOG_REMOVE_QUEST
            PacketHandlers(OPCODES.CMSG_QUEST_QUERY) = AddressOf ALLQUESTS.On_CMSG_QUEST_QUERY
            PacketHandlers(OPCODES.CMSG_QUESTGIVER_COMPLETE_QUEST) = AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_COMPLETE_QUEST
            PacketHandlers(OPCODES.CMSG_QUESTGIVER_REQUEST_REWARD) = AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_REQUEST_REWARD
            PacketHandlers(OPCODES.CMSG_QUESTGIVER_CHOOSE_REWARD) = AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_CHOOSE_REWARD
            PacketHandlers(OPCODES.MSG_QUEST_PUSH_RESULT) = AddressOf ALLQUESTS.On_MSG_QUEST_PUSH_RESULT
            PacketHandlers(OPCODES.CMSG_PUSHQUESTTOPARTY) = AddressOf ALLQUESTS.On_CMSG_PUSHQUESTTOPARTY

            PacketHandlers(OPCODES.CMSG_BINDER_ACTIVATE) = AddressOf On_CMSG_BINDER_ACTIVATE
            PacketHandlers(OPCODES.CMSG_BANKER_ACTIVATE) = AddressOf On_CMSG_BANKER_ACTIVATE
            PacketHandlers(OPCODES.CMSG_BUY_BANK_SLOT) = AddressOf On_CMSG_BUY_BANK_SLOT
            PacketHandlers(OPCODES.CMSG_AUTOBANK_ITEM) = AddressOf On_CMSG_AUTOBANK_ITEM
            PacketHandlers(OPCODES.CMSG_AUTOSTORE_BANK_ITEM) = AddressOf On_CMSG_AUTOSTORE_BANK_ITEM
            PacketHandlers(OPCODES.MSG_TALENT_WIPE_CONFIRM) = AddressOf On_MSG_TALENT_WIPE_CONFIRM
            PacketHandlers(OPCODES.CMSG_TRAINER_BUY_SPELL) = AddressOf On_CMSG_TRAINER_BUY_SPELL
            PacketHandlers(OPCODES.CMSG_TRAINER_LIST) = AddressOf On_CMSG_TRAINER_LIST

            PacketHandlers(OPCODES.MSG_AUCTION_HELLO) = AddressOf On_MSG_AUCTION_HELLO
            PacketHandlers(OPCODES.CMSG_AUCTION_SELL_ITEM) = AddressOf On_CMSG_AUCTION_SELL_ITEM
            PacketHandlers(OPCODES.CMSG_AUCTION_REMOVE_ITEM) = AddressOf On_CMSG_AUCTION_REMOVE_ITEM
            PacketHandlers(OPCODES.CMSG_AUCTION_LIST_ITEMS) = AddressOf On_CMSG_AUCTION_LIST_ITEMS
            PacketHandlers(OPCODES.CMSG_AUCTION_LIST_OWNER_ITEMS) = AddressOf On_CMSG_AUCTION_LIST_OWNER_ITEMS
            PacketHandlers(OPCODES.CMSG_AUCTION_PLACE_BID) = AddressOf On_CMSG_AUCTION_PLACE_BID
            PacketHandlers(OPCODES.CMSG_AUCTION_LIST_BIDDER_ITEMS) = AddressOf On_CMSG_AUCTION_LIST_BIDDER_ITEMS

            PacketHandlers(OPCODES.CMSG_PETITION_SHOWLIST) = AddressOf On_CMSG_PETITION_SHOWLIST
            PacketHandlers(OPCODES.CMSG_PETITION_BUY) = AddressOf On_CMSG_PETITION_BUY
            PacketHandlers(OPCODES.CMSG_PETITION_SHOW_SIGNATURES) = AddressOf On_CMSG_PETITION_SHOW_SIGNATURES
            PacketHandlers(OPCODES.CMSG_PETITION_QUERY) = AddressOf On_CMSG_PETITION_QUERY
            PacketHandlers(OPCODES.CMSG_OFFER_PETITION) = AddressOf On_CMSG_OFFER_PETITION
            PacketHandlers(OPCODES.CMSG_PETITION_SIGN) = AddressOf On_CMSG_PETITION_SIGN
            PacketHandlers(OPCODES.MSG_PETITION_RENAME) = AddressOf On_MSG_PETITION_RENAME
            PacketHandlers(OPCODES.MSG_PETITION_DECLINE) = AddressOf On_MSG_PETITION_DECLINE

            PacketHandlers(OPCODES.CMSG_BATTLEMASTER_HELLO) = AddressOf On_CMSG_BATTLEMASTER_HELLO
            PacketHandlers(OPCODES.CMSG_BATTLEFIELD_LIST) = AddressOf On_CMSG_BATTLEMASTER_HELLO

            PacketHandlers(OPCODES.CMSG_DUEL_CANCELLED) = AddressOf On_CMSG_DUEL_CANCELLED
            PacketHandlers(OPCODES.CMSG_DUEL_ACCEPTED) = AddressOf On_CMSG_DUEL_ACCEPTED
            PacketHandlers(OPCODES.CMSG_RESURRECT_RESPONSE) = AddressOf On_CMSG_RESURRECT_RESPONSE

            PacketHandlers(OPCODES.CMSG_LEARN_TALENT) = AddressOf On_CMSG_LEARN_TALENT

            PacketHandlers(OPCODES.CMSG_WORLD_TELEPORT) = AddressOf On_CMSG_WORLD_TELEPORT

            PacketHandlers(OPCODES.CMSG_SET_FACTION_ATWAR) = AddressOf On_CMSG_SET_FACTION_ATWAR
            PacketHandlers(OPCODES.CMSG_SET_FACTION_INACTIVE) = AddressOf On_CMSG_SET_FACTION_INACTIVE
            PacketHandlers(OPCODES.CMSG_SET_WATCHED_FACTION) = AddressOf On_CMSG_SET_WATCHED_FACTION

            PacketHandlers(OPCODES.CMSG_PET_NAME_QUERY) = AddressOf On_CMSG_PET_NAME_QUERY
            PacketHandlers(OPCODES.CMSG_REQUEST_PET_INFO) = AddressOf On_CMSG_REQUEST_PET_INFO
            PacketHandlers(OPCODES.CMSG_PET_ACTION) = AddressOf On_CMSG_PET_ACTION
            PacketHandlers(OPCODES.CMSG_PET_CANCEL_AURA) = AddressOf On_CMSG_PET_CANCEL_AURA
            PacketHandlers(OPCODES.CMSG_PET_ABANDON) = AddressOf On_CMSG_PET_ABANDON
            PacketHandlers(OPCODES.CMSG_PET_RENAME) = AddressOf On_CMSG_PET_RENAME
            PacketHandlers(OPCODES.CMSG_PET_SET_ACTION) = AddressOf On_CMSG_PET_SET_ACTION
            PacketHandlers(OPCODES.CMSG_PET_SPELL_AUTOCAST) = AddressOf On_CMSG_PET_SPELL_AUTOCAST
            PacketHandlers(OPCODES.CMSG_PET_STOP_ATTACK) = AddressOf On_CMSG_PET_STOP_ATTACK
            PacketHandlers(OPCODES.CMSG_PET_UNLEARN) = AddressOf On_CMSG_PET_UNLEARN

            'NOTE: These opcodes are partialy handled by cluster
            '   none

            'TODO:
            'CMSG_LOOT_MASTER_GIVE
        End Sub

        Public Sub OnUnhandledPacket(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            Log.WriteLine(GlobalEnum.LogType.WARNING, "[{0}:{1}] {2} [Unhandled Packet]", client.IP, client.Port, packet.OpCode)
        End Sub

        Public Sub OnWorldPacket(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Redirected Packet]", client.IP, client.Port, packet.OpCode)

            If client.Character Is Nothing OrElse client.Character.FullyLoggedIn = False Then
                Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", client.IP, client.Port, packet.OpCode, Environment.NewLine, packet.Length)
                DumpPacket(packet.Data, client)
            End If
        End Sub

    End Module
End NameSpace