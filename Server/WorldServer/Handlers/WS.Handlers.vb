'
' Copyright (C) 2013 - 2014 getMaNGOS <http://www.getmangos.eu>
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

Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common

Public Module WS_Handlers

    Public Sub IntializePacketHandlers()
        'NOTE: These opcodes are not used in any way
        PacketHandlers(OPCODES.CMSG_FORCE_MOVE_ROOT_ACK) = CType(AddressOf OnUnhandledPacket, HandlePacket)
        PacketHandlers(OPCODES.CMSG_FORCE_MOVE_UNROOT_ACK) = CType(AddressOf OnUnhandledPacket, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MOVE_WATER_WALK_ACK) = CType(AddressOf OnUnhandledPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_TELEPORT_ACK) = CType(AddressOf OnUnhandledPacket, HandlePacket)

        'NOTE: These opcodes below must be exluded form Cluster
        PacketHandlers(OPCODES.CMSG_WARDEN_DATA) = CType(AddressOf On_CMSG_WARDEN_DATA, HandlePacket)
        PacketHandlers(OPCODES.CMSG_NAME_QUERY) = CType(AddressOf On_CMSG_NAME_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MESSAGECHAT) = CType(AddressOf On_CMSG_MESSAGECHAT, HandlePacket)

        PacketHandlers(OPCODES.CMSG_LOGOUT_REQUEST) = CType(AddressOf On_CMSG_LOGOUT_REQUEST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LOGOUT_CANCEL) = CType(AddressOf On_CMSG_LOGOUT_CANCEL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CANCEL_TRADE) = CType(AddressOf On_CMSG_CANCEL_TRADE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BEGIN_TRADE) = CType(AddressOf On_CMSG_BEGIN_TRADE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_UNACCEPT_TRADE) = CType(AddressOf On_CMSG_UNACCEPT_TRADE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ACCEPT_TRADE) = CType(AddressOf On_CMSG_ACCEPT_TRADE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_INITIATE_TRADE) = CType(AddressOf On_CMSG_INITIATE_TRADE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_TRADE_GOLD) = CType(AddressOf On_CMSG_SET_TRADE_GOLD, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_TRADE_ITEM) = CType(AddressOf On_CMSG_SET_TRADE_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CLEAR_TRADE_ITEM) = CType(AddressOf On_CMSG_CLEAR_TRADE_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_IGNORE_TRADE) = CType(AddressOf On_CMSG_IGNORE_TRADE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BUSY_TRADE) = CType(AddressOf On_CMSG_BUSY_TRADE, HandlePacket)

        PacketHandlers(OPCODES.MSG_MOVE_START_FORWARD) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_BACKWARD) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_STOP) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_LEFT) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_RIGHT) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_STOP_STRAFE) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_JUMP) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_TURN_LEFT) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_TURN_RIGHT) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_STOP_TURN) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_UP) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_DOWN) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_STOP_PITCH) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_SET_RUN_MODE) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_SET_WALK_MODE) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_START_SWIM) = CType(AddressOf OnStartSwim, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_STOP_SWIM) = CType(AddressOf OnStopSwim, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_SET_FACING) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_SET_PITCH) = CType(AddressOf OnMovementPacket, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MOVE_FALL_RESET) = CType(AddressOf On_CMSG_MOVE_FALL_RESET, HandlePacket)

        PacketHandlers(OPCODES.MSG_MOVE_HEARTBEAT) = CType(AddressOf On_MSG_MOVE_HEARTBEAT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AREATRIGGER) = CType(AddressOf On_CMSG_AREATRIGGER, HandlePacket)
        PacketHandlers(OPCODES.MSG_MOVE_FALL_LAND) = CType(AddressOf On_MSG_MOVE_FALL_LAND, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ZONEUPDATE) = CType(AddressOf On_CMSG_ZONEUPDATE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_FORCE_RUN_SPEED_CHANGE_ACK) = CType(AddressOf OnChangeSpeed, HandlePacket)
        PacketHandlers(OPCODES.CMSG_FORCE_RUN_BACK_SPEED_CHANGE_ACK) = CType(AddressOf OnChangeSpeed, HandlePacket)
        PacketHandlers(OPCODES.CMSG_FORCE_SWIM_SPEED_CHANGE_ACK) = CType(AddressOf OnChangeSpeed, HandlePacket)
        PacketHandlers(OPCODES.CMSG_FORCE_SWIM_BACK_SPEED_CHANGE_ACK) = CType(AddressOf OnChangeSpeed, HandlePacket)
        PacketHandlers(OPCODES.CMSG_FORCE_TURN_RATE_CHANGE_ACK) = CType(AddressOf OnChangeSpeed, HandlePacket)

        PacketHandlers(OPCODES.CMSG_STANDSTATECHANGE) = CType(AddressOf On_CMSG_STANDSTATECHANGE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_SELECTION) = CType(AddressOf On_CMSG_SET_SELECTION, HandlePacket)
        PacketHandlers(OPCODES.CMSG_REPOP_REQUEST) = CType(AddressOf On_CMSG_REPOP_REQUEST, HandlePacket)
        PacketHandlers(OPCODES.MSG_CORPSE_QUERY) = CType(AddressOf On_MSG_CORPSE_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SPIRIT_HEALER_ACTIVATE) = CType(AddressOf On_CMSG_SPIRIT_HEALER_ACTIVATE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_RECLAIM_CORPSE) = CType(AddressOf On_CMSG_RECLAIM_CORPSE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_TUTORIAL_FLAG) = CType(AddressOf On_CMSG_TUTORIAL_FLAG, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TUTORIAL_CLEAR) = CType(AddressOf On_CMSG_TUTORIAL_CLEAR, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TUTORIAL_RESET) = CType(AddressOf On_CMSG_TUTORIAL_RESET, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_ACTION_BUTTON) = CType(AddressOf On_CMSG_SET_ACTION_BUTTON, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_ACTIONBAR_TOGGLES) = CType(AddressOf On_CMSG_SET_ACTIONBAR_TOGGLES, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TOGGLE_HELM) = CType(AddressOf On_CMSG_TOGGLE_HELM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TOGGLE_CLOAK) = CType(AddressOf On_CMSG_TOGGLE_CLOAK, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MOUNTSPECIAL_ANIM) = CType(AddressOf On_CMSG_MOUNTSPECIAL_ANIM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_EMOTE) = CType(AddressOf On_CMSG_EMOTE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TEXT_EMOTE) = CType(AddressOf On_CMSG_TEXT_EMOTE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_ITEM_QUERY_SINGLE) = CType(AddressOf On_CMSG_ITEM_QUERY_SINGLE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ITEM_NAME_QUERY) = CType(AddressOf On_CMSG_ITEM_NAME_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SETSHEATHED) = CType(AddressOf On_CMSG_SETSHEATHED, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SWAP_INV_ITEM) = CType(AddressOf On_CMSG_SWAP_INV_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SPLIT_ITEM) = CType(AddressOf On_CMSG_SPLIT_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUTOEQUIP_ITEM) = CType(AddressOf On_CMSG_AUTOEQUIP_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUTOSTORE_BAG_ITEM) = CType(AddressOf On_CMSG_AUTOSTORE_BAG_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SWAP_ITEM) = CType(AddressOf On_CMSG_SWAP_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_DESTROYITEM) = CType(AddressOf On_CMSG_DESTROYITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_READ_ITEM) = CType(AddressOf On_CMSG_READ_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PAGE_TEXT_QUERY) = CType(AddressOf On_CMSG_PAGE_TEXT_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_USE_ITEM) = CType(AddressOf On_CMSG_USE_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_OPEN_ITEM) = CType(AddressOf On_CMSG_OPEN_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_WRAP_ITEM) = CType(AddressOf On_CMSG_WRAP_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_AMMO) = CType(AddressOf On_CMSG_SET_AMMO, HandlePacket)

        PacketHandlers(OPCODES.CMSG_CREATURE_QUERY) = CType(AddressOf On_CMSG_CREATURE_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GOSSIP_HELLO) = CType(AddressOf On_CMSG_GOSSIP_HELLO, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GOSSIP_SELECT_OPTION) = CType(AddressOf On_CMSG_GOSSIP_SELECT_OPTION, HandlePacket)
        PacketHandlers(OPCODES.CMSG_NPC_TEXT_QUERY) = CType(AddressOf On_CMSG_NPC_TEXT_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LIST_INVENTORY) = CType(AddressOf On_CMSG_LIST_INVENTORY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BUY_ITEM_IN_SLOT) = CType(AddressOf On_CMSG_BUY_ITEM_IN_SLOT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BUY_ITEM) = CType(AddressOf On_CMSG_BUY_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BUYBACK_ITEM) = CType(AddressOf On_CMSG_BUYBACK_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SELL_ITEM) = CType(AddressOf On_CMSG_SELL_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_REPAIR_ITEM) = CType(AddressOf On_CMSG_REPAIR_ITEM, HandlePacket)

        PacketHandlers(OPCODES.CMSG_ATTACKSWING) = CType(AddressOf On_CMSG_ATTACKSWING, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ATTACKSTOP) = CType(AddressOf On_CMSG_ATTACKSTOP, HandlePacket)

        PacketHandlers(OPCODES.CMSG_GAMEOBJECT_QUERY) = CType(AddressOf On_CMSG_GAMEOBJECT_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_GAMEOBJ_USE) = CType(AddressOf On_CMSG_GAMEOBJ_USE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_BATTLEFIELD_STATUS) = CType(AddressOf On_CMSG_BATTLEFIELD_STATUS, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_ACTIVE_MOVER) = CType(AddressOf On_CMSG_SET_ACTIVE_MOVER, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MEETINGSTONE_INFO) = CType(AddressOf On_CMSG_MEETINGSTONE_INFO, HandlePacket)
        PacketHandlers(OPCODES.MSG_INSPECT_HONOR_STATS) = CType(AddressOf On_MSG_INSPECT_HONOR_STATS, HandlePacket)

        PacketHandlers(OPCODES.MSG_PVP_LOG_DATA) = CType(AddressOf On_MSG_PVP_LOG_DATA, HandlePacket)

        PacketHandlers(OPCODES.CMSG_GET_MAIL_LIST) = CType(AddressOf On_CMSG_GET_MAIL_LIST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SEND_MAIL) = CType(AddressOf On_CMSG_SEND_MAIL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MAIL_CREATE_TEXT_ITEM) = CType(AddressOf On_CMSG_MAIL_CREATE_TEXT_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ITEM_TEXT_QUERY) = CType(AddressOf On_CMSG_ITEM_TEXT_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MAIL_DELETE) = CType(AddressOf On_CMSG_MAIL_DELETE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MAIL_TAKE_ITEM) = CType(AddressOf On_CMSG_MAIL_TAKE_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MAIL_TAKE_MONEY) = CType(AddressOf On_CMSG_MAIL_TAKE_MONEY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MAIL_RETURN_TO_SENDER) = CType(AddressOf On_CMSG_MAIL_RETURN_TO_SENDER, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MAIL_MARK_AS_READ) = CType(AddressOf On_CMSG_MAIL_MARK_AS_READ, HandlePacket)
        PacketHandlers(OPCODES.MSG_QUERY_NEXT_MAIL_TIME) = CType(AddressOf On_MSG_QUERY_NEXT_MAIL_TIME, HandlePacket)

        PacketHandlers(OPCODES.CMSG_AUTOSTORE_LOOT_ITEM) = CType(AddressOf On_CMSG_AUTOSTORE_LOOT_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LOOT_MONEY) = CType(AddressOf On_CMSG_LOOT_MONEY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LOOT) = CType(AddressOf On_CMSG_LOOT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LOOT_ROLL) = CType(AddressOf On_CMSG_LOOT_ROLL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_LOOT_RELEASE) = CType(AddressOf On_CMSG_LOOT_RELEASE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_TAXINODE_STATUS_QUERY) = CType(AddressOf On_CMSG_TAXINODE_STATUS_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TAXIQUERYAVAILABLENODES) = CType(AddressOf On_CMSG_TAXIQUERYAVAILABLENODES, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ACTIVATETAXI) = CType(AddressOf On_CMSG_ACTIVATETAXI, HandlePacket)
        PacketHandlers(OPCODES.CMSG_ACTIVATETAXI_FAR) = CType(AddressOf On_CMSG_ACTIVATETAXI_FAR, HandlePacket)
        PacketHandlers(OPCODES.CMSG_MOVE_SPLINE_DONE) = CType(AddressOf On_CMSG_MOVE_SPLINE_DONE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_CAST_SPELL) = CType(AddressOf On_CMSG_CAST_SPELL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CANCEL_CAST) = CType(AddressOf On_CMSG_CANCEL_CAST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CANCEL_AURA) = CType(AddressOf On_CMSG_CANCEL_AURA, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CANCEL_AUTO_REPEAT_SPELL) = CType(AddressOf On_CMSG_CANCEL_AUTO_REPEAT_SPELL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_CANCEL_CHANNELLING) = CType(AddressOf On_CMSG_CANCEL_CHANNELLING, HandlePacket)

        PacketHandlers(OPCODES.CMSG_TOGGLE_PVP) = CType(AddressOf On_CMSG_TOGGLE_PVP, HandlePacket)

        PacketHandlers(OPCODES.CMSG_QUESTGIVER_STATUS_QUERY) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_STATUS_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUESTGIVER_HELLO) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_HELLO, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUESTGIVER_QUERY_QUEST) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_QUERY_QUEST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUESTGIVER_ACCEPT_QUEST) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_ACCEPT_QUEST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUESTLOG_REMOVE_QUEST) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTLOG_REMOVE_QUEST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUEST_QUERY) = CType(AddressOf ALLQUESTS.On_CMSG_QUEST_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUESTGIVER_COMPLETE_QUEST) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_COMPLETE_QUEST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUESTGIVER_REQUEST_REWARD) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_REQUEST_REWARD, HandlePacket)
        PacketHandlers(OPCODES.CMSG_QUESTGIVER_CHOOSE_REWARD) = CType(AddressOf ALLQUESTS.On_CMSG_QUESTGIVER_CHOOSE_REWARD, HandlePacket)
        PacketHandlers(OPCODES.MSG_QUEST_PUSH_RESULT) = CType(AddressOf ALLQUESTS.On_MSG_QUEST_PUSH_RESULT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PUSHQUESTTOPARTY) = CType(AddressOf ALLQUESTS.On_CMSG_PUSHQUESTTOPARTY, HandlePacket)

        PacketHandlers(OPCODES.CMSG_BINDER_ACTIVATE) = CType(AddressOf On_CMSG_BINDER_ACTIVATE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BANKER_ACTIVATE) = CType(AddressOf On_CMSG_BANKER_ACTIVATE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BUY_BANK_SLOT) = CType(AddressOf On_CMSG_BUY_BANK_SLOT, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUTOBANK_ITEM) = CType(AddressOf On_CMSG_AUTOBANK_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUTOSTORE_BANK_ITEM) = CType(AddressOf On_CMSG_AUTOSTORE_BANK_ITEM, HandlePacket)
        PacketHandlers(OPCODES.MSG_TALENT_WIPE_CONFIRM) = CType(AddressOf On_MSG_TALENT_WIPE_CONFIRM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TRAINER_BUY_SPELL) = CType(AddressOf On_CMSG_TRAINER_BUY_SPELL, HandlePacket)
        PacketHandlers(OPCODES.CMSG_TRAINER_LIST) = CType(AddressOf On_CMSG_TRAINER_LIST, HandlePacket)

        PacketHandlers(OPCODES.MSG_AUCTION_HELLO) = CType(AddressOf On_MSG_AUCTION_HELLO, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUCTION_SELL_ITEM) = CType(AddressOf On_CMSG_AUCTION_SELL_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUCTION_REMOVE_ITEM) = CType(AddressOf On_CMSG_AUCTION_REMOVE_ITEM, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUCTION_LIST_ITEMS) = CType(AddressOf On_CMSG_AUCTION_LIST_ITEMS, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUCTION_LIST_OWNER_ITEMS) = CType(AddressOf On_CMSG_AUCTION_LIST_OWNER_ITEMS, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUCTION_PLACE_BID) = CType(AddressOf On_CMSG_AUCTION_PLACE_BID, HandlePacket)
        PacketHandlers(OPCODES.CMSG_AUCTION_LIST_BIDDER_ITEMS) = CType(AddressOf On_CMSG_AUCTION_LIST_BIDDER_ITEMS, HandlePacket)

        PacketHandlers(OPCODES.CMSG_PETITION_SHOWLIST) = CType(AddressOf On_CMSG_PETITION_SHOWLIST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PETITION_BUY) = CType(AddressOf On_CMSG_PETITION_BUY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PETITION_SHOW_SIGNATURES) = CType(AddressOf On_CMSG_PETITION_SHOW_SIGNATURES, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PETITION_QUERY) = CType(AddressOf On_CMSG_PETITION_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_OFFER_PETITION) = CType(AddressOf On_CMSG_OFFER_PETITION, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PETITION_SIGN) = CType(AddressOf On_CMSG_PETITION_SIGN, HandlePacket)
        PacketHandlers(OPCODES.MSG_PETITION_RENAME) = CType(AddressOf On_MSG_PETITION_RENAME, HandlePacket)
        PacketHandlers(OPCODES.MSG_PETITION_DECLINE) = CType(AddressOf On_MSG_PETITION_DECLINE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_BATTLEMASTER_HELLO) = CType(AddressOf On_CMSG_BATTLEMASTER_HELLO, HandlePacket)
        PacketHandlers(OPCODES.CMSG_BATTLEFIELD_LIST) = CType(AddressOf On_CMSG_BATTLEMASTER_HELLO, HandlePacket)

        PacketHandlers(OPCODES.CMSG_DUEL_CANCELLED) = CType(AddressOf On_CMSG_DUEL_CANCELLED, HandlePacket)
        PacketHandlers(OPCODES.CMSG_DUEL_ACCEPTED) = CType(AddressOf On_CMSG_DUEL_ACCEPTED, HandlePacket)
        PacketHandlers(OPCODES.CMSG_RESURRECT_RESPONSE) = CType(AddressOf On_CMSG_RESURRECT_RESPONSE, HandlePacket)

        PacketHandlers(OPCODES.CMSG_LEARN_TALENT) = CType(AddressOf On_CMSG_LEARN_TALENT, HandlePacket)

        PacketHandlers(OPCODES.CMSG_WORLD_TELEPORT) = CType(AddressOf On_CMSG_WORLD_TELEPORT, HandlePacket)

        PacketHandlers(OPCODES.CMSG_SET_FACTION_ATWAR) = CType(AddressOf On_CMSG_SET_FACTION_ATWAR, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_FACTION_INACTIVE) = CType(AddressOf On_CMSG_SET_FACTION_INACTIVE, HandlePacket)
        PacketHandlers(OPCODES.CMSG_SET_WATCHED_FACTION) = CType(AddressOf On_CMSG_SET_WATCHED_FACTION, HandlePacket)

        PacketHandlers(OPCODES.CMSG_PET_NAME_QUERY) = CType(AddressOf On_CMSG_PET_NAME_QUERY, HandlePacket)
        PacketHandlers(OPCODES.CMSG_REQUEST_PET_INFO) = CType(AddressOf On_CMSG_REQUEST_PET_INFO, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_ACTION) = CType(AddressOf On_CMSG_PET_ACTION, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_CANCEL_AURA) = CType(AddressOf On_CMSG_PET_CANCEL_AURA, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_ABANDON) = CType(AddressOf On_CMSG_PET_ABANDON, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_RENAME) = CType(AddressOf On_CMSG_PET_RENAME, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_SET_ACTION) = CType(AddressOf On_CMSG_PET_SET_ACTION, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_SPELL_AUTOCAST) = CType(AddressOf On_CMSG_PET_SPELL_AUTOCAST, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_STOP_ATTACK) = CType(AddressOf On_CMSG_PET_STOP_ATTACK, HandlePacket)
        PacketHandlers(OPCODES.CMSG_PET_UNLEARN) = CType(AddressOf On_CMSG_PET_UNLEARN, HandlePacket)

        'NOTE: These opcodes are partialy handled by cluster
        '   none

        'TODO:
        'CMSG_LOOT_MASTER_GIVE
    End Sub

    Public Sub OnUnhandledPacket(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Unhandled Packet]", client.IP, client.Port, CType(packet.OpCode, OPCODES))
    End Sub

End Module