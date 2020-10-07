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
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Server
Imports Mangos.World.Objects
Imports Mangos.World.Social
Imports Mangos.World.Loots
Imports Mangos.World.Spells
Imports Mangos.World.Auction

Namespace Handlers

    Public Class WS_Handlers

        Public Sub IntializePacketHandlers()
            'NOTE: These opcodes are not used in any way
            _WorldServer.PacketHandlers(OPCODES.CMSG_FORCE_MOVE_ROOT_ACK) = AddressOf OnUnhandledPacket
            _WorldServer.PacketHandlers(OPCODES.CMSG_FORCE_MOVE_UNROOT_ACK) = AddressOf OnUnhandledPacket
            _WorldServer.PacketHandlers(OPCODES.CMSG_MOVE_WATER_WALK_ACK) = AddressOf OnUnhandledPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_TELEPORT_ACK) = AddressOf OnUnhandledPacket

            'NOTE: These opcodes below must be exluded form Cluster
            _WorldServer.PacketHandlers(OPCODES.CMSG_WARDEN_DATA) = AddressOf _WS_Handlers_Warden.On_CMSG_WARDEN_DATA
            _WorldServer.PacketHandlers(OPCODES.CMSG_NAME_QUERY) = AddressOf _WS_Handlers_Misc.On_CMSG_NAME_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_MESSAGECHAT) = AddressOf _WS_Handlers_Chat.On_CMSG_MESSAGECHAT

            _WorldServer.PacketHandlers(OPCODES.CMSG_LOGOUT_REQUEST) = AddressOf _CharManagementHandler.On_CMSG_LOGOUT_REQUEST
            _WorldServer.PacketHandlers(OPCODES.CMSG_LOGOUT_CANCEL) = AddressOf _CharManagementHandler.On_CMSG_LOGOUT_CANCEL
            _WorldServer.PacketHandlers(OPCODES.CMSG_CANCEL_TRADE) = AddressOf _WS_Handlers_Trade.On_CMSG_CANCEL_TRADE
            _WorldServer.PacketHandlers(OPCODES.CMSG_BEGIN_TRADE) = AddressOf _WS_Handlers_Trade.On_CMSG_BEGIN_TRADE
            _WorldServer.PacketHandlers(OPCODES.CMSG_UNACCEPT_TRADE) = AddressOf _WS_Handlers_Trade.On_CMSG_UNACCEPT_TRADE
            _WorldServer.PacketHandlers(OPCODES.CMSG_ACCEPT_TRADE) = AddressOf _WS_Handlers_Trade.On_CMSG_ACCEPT_TRADE
            _WorldServer.PacketHandlers(OPCODES.CMSG_INITIATE_TRADE) = AddressOf _WS_Handlers_Trade.On_CMSG_INITIATE_TRADE
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_TRADE_GOLD) = AddressOf _WS_Handlers_Trade.On_CMSG_SET_TRADE_GOLD
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_TRADE_ITEM) = AddressOf _WS_Handlers_Trade.On_CMSG_SET_TRADE_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_CLEAR_TRADE_ITEM) = AddressOf _WS_Handlers_Trade.On_CMSG_CLEAR_TRADE_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_IGNORE_TRADE) = AddressOf _WS_Handlers_Trade.On_CMSG_IGNORE_TRADE
            _WorldServer.PacketHandlers(OPCODES.CMSG_BUSY_TRADE) = AddressOf _WS_Handlers_Trade.On_CMSG_BUSY_TRADE

            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_FORWARD) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_BACKWARD) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_STOP) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_LEFT) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_STRAFE_RIGHT) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_STOP_STRAFE) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_JUMP) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_TURN_LEFT) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_TURN_RIGHT) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_STOP_TURN) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_UP) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_PITCH_DOWN) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_STOP_PITCH) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_SET_RUN_MODE) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_SET_WALK_MODE) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_START_SWIM) = AddressOf _WS_CharMovement.OnStartSwim
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_STOP_SWIM) = AddressOf _WS_CharMovement.OnStopSwim
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_SET_FACING) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_SET_PITCH) = AddressOf _WS_CharMovement.OnMovementPacket
            _WorldServer.PacketHandlers(OPCODES.CMSG_MOVE_FALL_RESET) = AddressOf _WS_Handlers_Misc.On_CMSG_MOVE_FALL_RESET

            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_HEARTBEAT) = AddressOf _WS_CharMovement.On_MSG_MOVE_HEARTBEAT
            _WorldServer.PacketHandlers(OPCODES.CMSG_AREATRIGGER) = AddressOf _WS_CharMovement.On_CMSG_AREATRIGGER
            _WorldServer.PacketHandlers(OPCODES.MSG_MOVE_FALL_LAND) = AddressOf _WS_CharMovement.On_MSG_MOVE_FALL_LAND
            _WorldServer.PacketHandlers(OPCODES.CMSG_ZONEUPDATE) = AddressOf _WS_CharMovement.On_CMSG_ZONEUPDATE
            _WorldServer.PacketHandlers(OPCODES.CMSG_FORCE_RUN_SPEED_CHANGE_ACK) = AddressOf _WS_CharMovement.OnChangeSpeed
            _WorldServer.PacketHandlers(OPCODES.CMSG_FORCE_RUN_BACK_SPEED_CHANGE_ACK) = AddressOf _WS_CharMovement.OnChangeSpeed
            _WorldServer.PacketHandlers(OPCODES.CMSG_FORCE_SWIM_SPEED_CHANGE_ACK) = AddressOf _WS_CharMovement.OnChangeSpeed
            _WorldServer.PacketHandlers(OPCODES.CMSG_FORCE_SWIM_BACK_SPEED_CHANGE_ACK) = AddressOf _WS_CharMovement.OnChangeSpeed
            _WorldServer.PacketHandlers(OPCODES.CMSG_FORCE_TURN_RATE_CHANGE_ACK) = AddressOf _WS_CharMovement.OnChangeSpeed

            _WorldServer.PacketHandlers(OPCODES.CMSG_STANDSTATECHANGE) = AddressOf _CharManagementHandler.On_CMSG_STANDSTATECHANGE
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_SELECTION) = AddressOf _WS_Combat.On_CMSG_SET_SELECTION
            _WorldServer.PacketHandlers(OPCODES.CMSG_REPOP_REQUEST) = AddressOf _WS_Handlers_Misc.On_CMSG_REPOP_REQUEST
            _WorldServer.PacketHandlers(OPCODES.MSG_CORPSE_QUERY) = AddressOf _WS_Handlers_Misc.On_MSG_CORPSE_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_SPIRIT_HEALER_ACTIVATE) = AddressOf _WS_Creatures.On_CMSG_SPIRIT_HEALER_ACTIVATE
            _WorldServer.PacketHandlers(OPCODES.CMSG_RECLAIM_CORPSE) = AddressOf _WS_Handlers_Misc.On_CMSG_RECLAIM_CORPSE

            _WorldServer.PacketHandlers(OPCODES.CMSG_TUTORIAL_FLAG) = AddressOf _WS_Handlers_Misc.On_CMSG_TUTORIAL_FLAG
            _WorldServer.PacketHandlers(OPCODES.CMSG_TUTORIAL_CLEAR) = AddressOf _WS_Handlers_Misc.On_CMSG_TUTORIAL_CLEAR
            _WorldServer.PacketHandlers(OPCODES.CMSG_TUTORIAL_RESET) = AddressOf _WS_Handlers_Misc.On_CMSG_TUTORIAL_RESET
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_ACTION_BUTTON) = AddressOf _CharManagementHandler.On_CMSG_SET_ACTION_BUTTON
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_ACTIONBAR_TOGGLES) = AddressOf _WS_Handlers_Misc.On_CMSG_SET_ACTIONBAR_TOGGLES
            _WorldServer.PacketHandlers(OPCODES.CMSG_TOGGLE_HELM) = AddressOf _WS_Handlers_Misc.On_CMSG_TOGGLE_HELM
            _WorldServer.PacketHandlers(OPCODES.CMSG_TOGGLE_CLOAK) = AddressOf _WS_Handlers_Misc.On_CMSG_TOGGLE_CLOAK
            _WorldServer.PacketHandlers(OPCODES.CMSG_MOUNTSPECIAL_ANIM) = AddressOf _WS_Handlers_Misc.On_CMSG_MOUNTSPECIAL_ANIM
            _WorldServer.PacketHandlers(OPCODES.CMSG_EMOTE) = AddressOf _WS_Handlers_Misc.On_CMSG_EMOTE
            _WorldServer.PacketHandlers(OPCODES.CMSG_TEXT_EMOTE) = AddressOf _WS_Handlers_Misc.On_CMSG_TEXT_EMOTE

            _WorldServer.PacketHandlers(OPCODES.CMSG_ITEM_QUERY_SINGLE) = AddressOf _WS_Items.On_CMSG_ITEM_QUERY_SINGLE
            _WorldServer.PacketHandlers(OPCODES.CMSG_ITEM_NAME_QUERY) = AddressOf _WS_Items.On_CMSG_ITEM_NAME_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_SETSHEATHED) = AddressOf _WS_Combat.On_CMSG_SETSHEATHED
            _WorldServer.PacketHandlers(OPCODES.CMSG_SWAP_INV_ITEM) = AddressOf _WS_Items.On_CMSG_SWAP_INV_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_SPLIT_ITEM) = AddressOf _WS_Items.On_CMSG_SPLIT_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUTOEQUIP_ITEM) = AddressOf _WS_Items.On_CMSG_AUTOEQUIP_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUTOSTORE_BAG_ITEM) = AddressOf _WS_Items.On_CMSG_AUTOSTORE_BAG_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_SWAP_ITEM) = AddressOf _WS_Items.On_CMSG_SWAP_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_DESTROYITEM) = AddressOf _WS_Items.On_CMSG_DESTROYITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_READ_ITEM) = AddressOf _WS_Items.On_CMSG_READ_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_PAGE_TEXT_QUERY) = AddressOf _WS_Items.On_CMSG_PAGE_TEXT_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_USE_ITEM) = AddressOf _WS_Items.On_CMSG_USE_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_OPEN_ITEM) = AddressOf _WS_Items.On_CMSG_OPEN_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_WRAP_ITEM) = AddressOf _WS_Items.On_CMSG_WRAP_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_AMMO) = AddressOf _WS_Combat.On_CMSG_SET_AMMO

            _WorldServer.PacketHandlers(OPCODES.CMSG_CREATURE_QUERY) = AddressOf _WS_Creatures.On_CMSG_CREATURE_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_GOSSIP_HELLO) = AddressOf _WS_Creatures.On_CMSG_GOSSIP_HELLO
            _WorldServer.PacketHandlers(OPCODES.CMSG_GOSSIP_SELECT_OPTION) = AddressOf _WS_Creatures.On_CMSG_GOSSIP_SELECT_OPTION
            _WorldServer.PacketHandlers(OPCODES.CMSG_NPC_TEXT_QUERY) = AddressOf _WS_Creatures.On_CMSG_NPC_TEXT_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_LIST_INVENTORY) = AddressOf _WS_NPCs.On_CMSG_LIST_INVENTORY
            _WorldServer.PacketHandlers(OPCODES.CMSG_BUY_ITEM_IN_SLOT) = AddressOf _WS_NPCs.On_CMSG_BUY_ITEM_IN_SLOT
            _WorldServer.PacketHandlers(OPCODES.CMSG_BUY_ITEM) = AddressOf _WS_NPCs.On_CMSG_BUY_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_BUYBACK_ITEM) = AddressOf _WS_NPCs.On_CMSG_BUYBACK_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_SELL_ITEM) = AddressOf _WS_NPCs.On_CMSG_SELL_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_REPAIR_ITEM) = AddressOf _WS_NPCs.On_CMSG_REPAIR_ITEM

            _WorldServer.PacketHandlers(OPCODES.CMSG_ATTACKSWING) = AddressOf _WS_Combat.On_CMSG_ATTACKSWING
            _WorldServer.PacketHandlers(OPCODES.CMSG_ATTACKSTOP) = AddressOf _WS_Combat.On_CMSG_ATTACKSTOP

            _WorldServer.PacketHandlers(OPCODES.CMSG_GAMEOBJECT_QUERY) = AddressOf _WS_GameObjects.On_CMSG_GAMEOBJECT_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_GAMEOBJ_USE) = AddressOf _WS_GameObjects.On_CMSG_GAMEOBJ_USE

            _WorldServer.PacketHandlers(OPCODES.CMSG_BATTLEFIELD_STATUS) = AddressOf _WS_Handlers_Misc.On_CMSG_BATTLEFIELD_STATUS
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_ACTIVE_MOVER) = AddressOf _WS_Handlers_Misc.On_CMSG_SET_ACTIVE_MOVER
            _WorldServer.PacketHandlers(OPCODES.CMSG_MEETINGSTONE_INFO) = AddressOf _WS_Handlers_Misc.On_CMSG_MEETINGSTONE_INFO
            _WorldServer.PacketHandlers(OPCODES.MSG_INSPECT_HONOR_STATS) = AddressOf _WS_Handlers_Misc.On_MSG_INSPECT_HONOR_STATS

            _WorldServer.PacketHandlers(OPCODES.MSG_PVP_LOG_DATA) = AddressOf _WS_Handlers_Misc.On_MSG_PVP_LOG_DATA

            _WorldServer.PacketHandlers(OPCODES.CMSG_MOVE_TIME_SKIPPED) = AddressOf _WS_CharMovement.On_CMSG_MOVE_TIME_SKIPPED

            _WorldServer.PacketHandlers(OPCODES.CMSG_GET_MAIL_LIST) = AddressOf _WS_Mail.On_CMSG_GET_MAIL_LIST
            _WorldServer.PacketHandlers(OPCODES.CMSG_SEND_MAIL) = AddressOf _WS_Mail.On_CMSG_SEND_MAIL
            _WorldServer.PacketHandlers(OPCODES.CMSG_MAIL_CREATE_TEXT_ITEM) = AddressOf _WS_Mail.On_CMSG_MAIL_CREATE_TEXT_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_ITEM_TEXT_QUERY) = AddressOf _WS_Mail.On_CMSG_ITEM_TEXT_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_MAIL_DELETE) = AddressOf _WS_Mail.On_CMSG_MAIL_DELETE
            _WorldServer.PacketHandlers(OPCODES.CMSG_MAIL_TAKE_ITEM) = AddressOf _WS_Mail.On_CMSG_MAIL_TAKE_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_MAIL_TAKE_MONEY) = AddressOf _WS_Mail.On_CMSG_MAIL_TAKE_MONEY
            _WorldServer.PacketHandlers(OPCODES.CMSG_MAIL_RETURN_TO_SENDER) = AddressOf _WS_Mail.On_CMSG_MAIL_RETURN_TO_SENDER
            _WorldServer.PacketHandlers(OPCODES.CMSG_MAIL_MARK_AS_READ) = AddressOf _WS_Mail.On_CMSG_MAIL_MARK_AS_READ
            _WorldServer.PacketHandlers(OPCODES.MSG_QUERY_NEXT_MAIL_TIME) = AddressOf _WS_Mail.On_MSG_QUERY_NEXT_MAIL_TIME

            _WorldServer.PacketHandlers(OPCODES.CMSG_AUTOSTORE_LOOT_ITEM) = AddressOf _WS_Loot.On_CMSG_AUTOSTORE_LOOT_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_LOOT_MONEY) = AddressOf _WS_Loot.On_CMSG_LOOT_MONEY
            _WorldServer.PacketHandlers(OPCODES.CMSG_LOOT) = AddressOf _WS_Loot.On_CMSG_LOOT
            _WorldServer.PacketHandlers(OPCODES.CMSG_LOOT_ROLL) = AddressOf _WS_Loot.On_CMSG_LOOT_ROLL
            _WorldServer.PacketHandlers(OPCODES.CMSG_LOOT_RELEASE) = AddressOf _WS_Loot.On_CMSG_LOOT_RELEASE

            _WorldServer.PacketHandlers(OPCODES.CMSG_TAXINODE_STATUS_QUERY) = AddressOf _WS_Handlers_Taxi.On_CMSG_TAXINODE_STATUS_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_TAXIQUERYAVAILABLENODES) = AddressOf _WS_Handlers_Taxi.On_CMSG_TAXIQUERYAVAILABLENODES
            _WorldServer.PacketHandlers(OPCODES.CMSG_ACTIVATETAXI) = AddressOf _WS_Handlers_Taxi.On_CMSG_ACTIVATETAXI
            _WorldServer.PacketHandlers(OPCODES.CMSG_ACTIVATETAXI_FAR) = AddressOf _WS_Handlers_Taxi.On_CMSG_ACTIVATETAXI_FAR
            _WorldServer.PacketHandlers(OPCODES.CMSG_MOVE_SPLINE_DONE) = AddressOf _WS_Handlers_Taxi.On_CMSG_MOVE_SPLINE_DONE

            _WorldServer.PacketHandlers(OPCODES.CMSG_CAST_SPELL) = AddressOf _WS_Spells.On_CMSG_CAST_SPELL
            _WorldServer.PacketHandlers(OPCODES.CMSG_CANCEL_CAST) = AddressOf _WS_Spells.On_CMSG_CANCEL_CAST
            _WorldServer.PacketHandlers(OPCODES.CMSG_CANCEL_AURA) = AddressOf _WS_Spells.On_CMSG_CANCEL_AURA
            _WorldServer.PacketHandlers(OPCODES.CMSG_CANCEL_AUTO_REPEAT_SPELL) = AddressOf _WS_Spells.On_CMSG_CANCEL_AUTO_REPEAT_SPELL
            _WorldServer.PacketHandlers(OPCODES.CMSG_CANCEL_CHANNELLING) = AddressOf _WS_Spells.On_CMSG_CANCEL_CHANNELLING

            _WorldServer.PacketHandlers(OPCODES.CMSG_TOGGLE_PVP) = AddressOf _WS_Handlers_Misc.On_CMSG_TOGGLE_PVP
            _WorldServer.PacketHandlers(OPCODES.MSG_BATTLEGROUND_PLAYER_POSITIONS) = AddressOf _WS_Handlers_Battleground.On_MSG_BATTLEGROUND_PLAYER_POSITIONS

            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTGIVER_STATUS_QUERY) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTGIVER_STATUS_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTGIVER_HELLO) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTGIVER_HELLO
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTGIVER_QUERY_QUEST) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTGIVER_QUERY_QUEST
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTGIVER_ACCEPT_QUEST) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTGIVER_ACCEPT_QUEST
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTLOG_REMOVE_QUEST) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTLOG_REMOVE_QUEST
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUEST_QUERY) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUEST_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTGIVER_COMPLETE_QUEST) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTGIVER_COMPLETE_QUEST
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTGIVER_REQUEST_REWARD) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTGIVER_REQUEST_REWARD
            _WorldServer.PacketHandlers(OPCODES.CMSG_QUESTGIVER_CHOOSE_REWARD) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_QUESTGIVER_CHOOSE_REWARD
            _WorldServer.PacketHandlers(OPCODES.MSG_QUEST_PUSH_RESULT) = AddressOf _WorldServer.ALLQUESTS.On_MSG_QUEST_PUSH_RESULT
            _WorldServer.PacketHandlers(OPCODES.CMSG_PUSHQUESTTOPARTY) = AddressOf _WorldServer.ALLQUESTS.On_CMSG_PUSHQUESTTOPARTY

            _WorldServer.PacketHandlers(OPCODES.CMSG_BINDER_ACTIVATE) = AddressOf _WS_NPCs.On_CMSG_BINDER_ACTIVATE
            _WorldServer.PacketHandlers(OPCODES.CMSG_BANKER_ACTIVATE) = AddressOf _WS_NPCs.On_CMSG_BANKER_ACTIVATE
            _WorldServer.PacketHandlers(OPCODES.CMSG_BUY_BANK_SLOT) = AddressOf _WS_NPCs.On_CMSG_BUY_BANK_SLOT
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUTOBANK_ITEM) = AddressOf _WS_NPCs.On_CMSG_AUTOBANK_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUTOSTORE_BANK_ITEM) = AddressOf _WS_NPCs.On_CMSG_AUTOSTORE_BANK_ITEM
            _WorldServer.PacketHandlers(OPCODES.MSG_TALENT_WIPE_CONFIRM) = AddressOf _WS_NPCs.On_MSG_TALENT_WIPE_CONFIRM
            _WorldServer.PacketHandlers(OPCODES.CMSG_TRAINER_BUY_SPELL) = AddressOf _WS_NPCs.On_CMSG_TRAINER_BUY_SPELL
            _WorldServer.PacketHandlers(OPCODES.CMSG_TRAINER_LIST) = AddressOf _WS_NPCs.On_CMSG_TRAINER_LIST

            _WorldServer.PacketHandlers(OPCODES.MSG_AUCTION_HELLO) = AddressOf _WS_Auction.On_MSG_AUCTION_HELLO
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUCTION_SELL_ITEM) = AddressOf _WS_Auction.On_CMSG_AUCTION_SELL_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUCTION_REMOVE_ITEM) = AddressOf _WS_Auction.On_CMSG_AUCTION_REMOVE_ITEM
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUCTION_LIST_ITEMS) = AddressOf _WS_Auction.On_CMSG_AUCTION_LIST_ITEMS
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUCTION_LIST_OWNER_ITEMS) = AddressOf _WS_Auction.On_CMSG_AUCTION_LIST_OWNER_ITEMS
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUCTION_PLACE_BID) = AddressOf _WS_Auction.On_CMSG_AUCTION_PLACE_BID
            _WorldServer.PacketHandlers(OPCODES.CMSG_AUCTION_LIST_BIDDER_ITEMS) = AddressOf _WS_Auction.On_CMSG_AUCTION_LIST_BIDDER_ITEMS

            _WorldServer.PacketHandlers(OPCODES.CMSG_PETITION_SHOWLIST) = AddressOf _WS_Guilds.On_CMSG_PETITION_SHOWLIST
            _WorldServer.PacketHandlers(OPCODES.CMSG_PETITION_BUY) = AddressOf _WS_Guilds.On_CMSG_PETITION_BUY
            _WorldServer.PacketHandlers(OPCODES.CMSG_PETITION_SHOW_SIGNATURES) = AddressOf _WS_Guilds.On_CMSG_PETITION_SHOW_SIGNATURES
            _WorldServer.PacketHandlers(OPCODES.CMSG_PETITION_QUERY) = AddressOf _WS_Guilds.On_CMSG_PETITION_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_OFFER_PETITION) = AddressOf _WS_Guilds.On_CMSG_OFFER_PETITION
            _WorldServer.PacketHandlers(OPCODES.CMSG_PETITION_SIGN) = AddressOf _WS_Guilds.On_CMSG_PETITION_SIGN
            _WorldServer.PacketHandlers(OPCODES.MSG_PETITION_RENAME) = AddressOf _WS_Guilds.On_MSG_PETITION_RENAME
            _WorldServer.PacketHandlers(OPCODES.MSG_PETITION_DECLINE) = AddressOf _WS_Guilds.On_MSG_PETITION_DECLINE

            _WorldServer.PacketHandlers(OPCODES.CMSG_BATTLEMASTER_HELLO) = AddressOf _WS_Handlers_Battleground.On_CMSG_BATTLEMASTER_HELLO
            _WorldServer.PacketHandlers(OPCODES.CMSG_BATTLEFIELD_LIST) = AddressOf _WS_Handlers_Battleground.On_CMSG_BATTLEMASTER_HELLO

            _WorldServer.PacketHandlers(OPCODES.CMSG_DUEL_CANCELLED) = AddressOf _WS_Spells.On_CMSG_DUEL_CANCELLED
            _WorldServer.PacketHandlers(OPCODES.CMSG_DUEL_ACCEPTED) = AddressOf _WS_Spells.On_CMSG_DUEL_ACCEPTED
            _WorldServer.PacketHandlers(OPCODES.CMSG_RESURRECT_RESPONSE) = AddressOf _WS_Spells.On_CMSG_RESURRECT_RESPONSE

            _WorldServer.PacketHandlers(OPCODES.CMSG_LEARN_TALENT) = AddressOf _WS_Spells.On_CMSG_LEARN_TALENT

            _WorldServer.PacketHandlers(OPCODES.CMSG_WORLD_TELEPORT) = AddressOf _WS_Handlers_Gamemaster.On_CMSG_WORLD_TELEPORT

            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_FACTION_ATWAR) = AddressOf _WS_Handlers_Misc.On_CMSG_SET_FACTION_ATWAR
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_FACTION_INACTIVE) = AddressOf _WS_Handlers_Misc.On_CMSG_SET_FACTION_INACTIVE
            _WorldServer.PacketHandlers(OPCODES.CMSG_SET_WATCHED_FACTION) = AddressOf _WS_Handlers_Misc.On_CMSG_SET_WATCHED_FACTION

            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_NAME_QUERY) = AddressOf _WS_Pets.On_CMSG_PET_NAME_QUERY
            _WorldServer.PacketHandlers(OPCODES.CMSG_REQUEST_PET_INFO) = AddressOf _WS_Pets.On_CMSG_REQUEST_PET_INFO
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_ACTION) = AddressOf _WS_Pets.On_CMSG_PET_ACTION
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_CANCEL_AURA) = AddressOf _WS_Pets.On_CMSG_PET_CANCEL_AURA
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_ABANDON) = AddressOf _WS_Pets.On_CMSG_PET_ABANDON
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_RENAME) = AddressOf _WS_Pets.On_CMSG_PET_RENAME
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_SET_ACTION) = AddressOf _WS_Pets.On_CMSG_PET_SET_ACTION
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_SPELL_AUTOCAST) = AddressOf _WS_Pets.On_CMSG_PET_SPELL_AUTOCAST
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_STOP_ATTACK) = AddressOf _WS_Pets.On_CMSG_PET_STOP_ATTACK
            _WorldServer.PacketHandlers(OPCODES.CMSG_PET_UNLEARN) = AddressOf _WS_Pets.On_CMSG_PET_UNLEARN

            'NOTE: These opcodes are partialy handled by cluster
            '   none

            'TODO:
            'CMSG_LOOT_MASTER_GIVE
        End Sub

        Public Sub OnUnhandledPacket(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Unhandled Packet]", client.IP, client.Port, packet.OpCode)
        End Sub

        Public Sub OnWorldPacket(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] {2} [Redirected Packet]", client.IP, client.Port, packet.OpCode)

            If client.Character Is Nothing OrElse client.Character.FullyLoggedIn = False Then
                _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] Unknown Opcode 0x{2:X} [{2}], DataLen={4}", client.IP, client.Port, packet.OpCode, Environment.NewLine, packet.Length)
                _Packets.DumpPacket(packet.Data, client)
            End If
        End Sub

    End Class
End Namespace