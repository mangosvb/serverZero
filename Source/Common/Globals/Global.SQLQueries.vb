'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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

Public NotInheritable Class SQLQueries

#Region "Shared Queries"
    Public Const GetAccountIdByName As String = "SELECT id FROM account WHERE username = ""{UserName}"";"
#End Region

#Region "Realm Server Queries"
    Public Const DBVersionCheck As String = "SELECT `version`,`structure`,`content` FROM {DatabaseName}.db_version"
    Public Const KnownWorldServers As String = "SELECT * FROM realmlist WHERE allowedSecurityLevel < '1';"
    Public Const OnlineWorldServers As String = "SELECT * FROM realmlist WHERE realmflags < 2 && allowedSecurityLevel < '1';"
    Public Const GMOnlyServers As String = "SELECT * FROM realmlist WHERE realmflags < 2 && allowedSecurityLevel >= '1';"
    Public Const IPBanned As String = "SELECT ip FROM ip_banned WHERE ip = '{IpAddress}';"
    Public Const LoginAccountInfo As String = "SELECT id, sha_pass_hash, gmlevel, expansion FROM account WHERE username = ""{UserName}"";"
    Public Const LoginAccountBanned As String = "SELECT id FROM account_banned WHERE id = '{Id}';"
    Public Const LogonUpdateAccount As String = "UPDATE account SET sessionkey = '{SessionKey}', last_ip='{LastIp}', last_login='{LastLogin}' WHERE username = '{UserName}';"
    Public Const GetNonGMOnlyRealms As String = "SELECT * FROM realmlist WHERE allowedSecurityLevel = '0';"
    Public Const GetAllRealms As String = "SELECT * FROM realmlist;"
    Public Const GetNumberCharactersForRealm As String = "SELECT * FROM realmcharacters WHERE realmid = '{HostRealmId}' AND acctid = '{AccountId}';"
#End Region

#Region "World Cluster Queries"
    Public Const SetCharacterSet As String = "SET NAMES 'utf8';"
    Public Const SetSqlMode As String = "SET SESSION sql_mode='STRICT_ALL_TABLES';"
    Public Const CreateAccount As String = "INSERT INTO account (username, sha_pass_hash, email, joindate, last_ip) VALUES ('{UserName}', '{ShaPassHash}', '{Email}', '{JoinDate}', '{LastIp}')"
    Public Const GetAllBattleGrounds As String = "SELECT * FROM battleground_template"
    Public Const SetAllPlayersOffline As String = "UPDATE characters SET char_online = 0;"
    Public Const GetAccountToBanByName As String = "SELECT id, username, last_ip FROM account WHERE username = {UserName};"
    Public Const GetBannedAccountById As String = "SELECT id, active FROM account_banned WHERE id = {Id};"
    Public Const UpdateAccountBanned As String = "UPDATE account_banned SET active = 1 WHERE id = '{Id}';"
    Public Const InsertBannedAccount As String = "INSERT INTO `account_banned` VALUES ('{Id}', UNIX_TIMESTAMP('{BanDate}'), UNIX_TIMESTAMP('{UnBanDate}'), '{BannedBy}', '{BanReason}', active = 1);"

    #Region "Auth Handlers Queries"
    Public Const GetSessionKeyGMLevelByName As String = "SELECT sessionkey, gmlevel FROM account WHERE username = '{UserName}';"
    Public Const GetAllCharactersForAccountId As String = "SELECT * FROM characters WHERE account_id = '{AccountId}' ORDER BY char_guid;"
    Public Const GetCorpseCountForPlayer As String = "SELECT COUNT(*) FROM corpse WHERE player = {CharGuid};"
    Public Const GetPetForCharacterEnum As String = "SELECT modelid, level, entry FROM character_pet WHERE owner = '{CharGuid}';"
    Public Const GetPetFamily As String = "SELECT family FROM creature_template WHERE entry = '{Entry}'"
    Public Const GetPlayerInventoryForEnum As String = "SELECT item_slot, displayid, inventorytype FROM {CharacterDB}.characters_inventory, {WorldDB}.item_template WHERE item_bag = {ItemBag} AND item_slot <> 255 AND entry = item_id  ORDER BY item_slot;"
    Public Const GetItemGuidFromPlayerInventory As String = "SELECT item_guid FROM characters_inventory WHERE item_bag = {ItemBag};"
    Public Const DeleteFromPlayerInventoryByItemGuid As String = "DELETE FROM characters_inventory WHERE item_guid = ""{ItemGuid}"";"
    Public Const DeleteFromPlayerInventoryByItemBag As String = "DELETE FROM characters_inventory WHERE item_bag = ""{ItemBag}"";"
    Public Const GetItemGuidFromPlayerInventoryByItemOwner As String = "SELECT item_guid FROM characters_inventory WHERE item_owner = {ItemOwner};"
    Public Const GetMailIdFromPlayerMailByReceiver As String = "SELECT mail_id FROM characters_mail WHERE mail_receiver = ""{MailReceiver}"";"
    Public Const DeletePlayerMailByMailId As String = "DELETE FROM characters_mail WHERE mail_id = ""{MailId}"";"
    Public Const DeletePlayerMailItemByMailId As String = "DELETE FROM mail_items WHERE mail_id = ""{MailId}"";"
    Public Const DeleteCharacterByGuid As String = "DELETE FROM characters WHERE char_guid = ""{CharGuid}"";"
    Public Const DeleteCharacterHonorByGuid As String = "DELETE FROM characters_honor WHERE char_guid = ""{CharGuid}"";"
    Public Const DeleteCharacterQuestsByGuid As String = "DELETE FROM characters_quests WHERE char_guid = ""{CharGuid}"";"
    Public Const DeleteCharacterSocialByGuid As String = "DELETE FROM character_social WHERE guid = '{Guid}' OR friend = '{Friend}';"
    Public Const DeleteCharacterSpellsByGuid As String = "DELETE FROM characters_spells WHERE guid = ""{Guid}"";"
    Public Const DeletePetitionsByOwner As String = "DELETE FROM petitions WHERE petition_owner = ""{PetitionOwner}"";"
    Public Const DeleteAuctionsByOwner As String = "DELETE FROM auctionhouse WHERE auction_owner = ""{AuctionOwner}"";"
    Public Const DeleteCharacterTicketsByGuid As String = "DELETE FROM characters_tickets WHERE char_guid = ""{CharGuid}"";"
    Public Const DeleteCorpseByGuid As String = "DELETE FROM corpse WHERE guid = ""{Guid}"";"
    Public Const GetGuildIdByLeader As String = "SELECT guild_id FROM guilds WHERE guild_leader = ""{GuildLeader}"";"
    Public Const RemoveGuildFromPlayers As String = "UPDATE characters SET char_guildid=0, char_guildrank=0, char_guildpnote='', charguildoffnote='' WHERE char_guildid=""{CharGuildId}"";"
    Public Const DeleteGuildById As String = "DELETE FROM guild WHERE guild_id=""{GuildId}"";"
    Public Const GetCharacterNameByLike As String = "SELECT char_name FROM characters WHERE char_name LIKE ""{CharName}"";"
    Public Const CharacterRename As String = "UPDATE characters SET char_name = ""{CharName}"", force_restrictions = 0 WHERE char_guid = {CharGuid};"
    #End Region

    #Region "Character Handlers Queries"
    Public Const SelectAllFromCharactersByGuid As String = "SELECT * FROM characters WHERE char_guid = {CharGuid};"
    Public Const TransferPlayer As String = "UPDATE characters SET char_positionX = {CharPositionX}, char_positionY = {CharPositionY}, char_positionZ = {CharPositionZ}, char_orientation = {CharOrientation}, char_map_id = {CharMapId} WHERE char_guid = {CharGuid};"
    Public Const SetCharacterOnlineByGuid As String = "UPDATE characters SET char_online = 1 WHERE char_guid = {CharGuid};"
    Public Const TriggerCinematic As String = "SELECT char_moviePlayed FROM characters WHERE char_guid = {CharGuid} AND char_moviePlayed = 0;"
    Public Const SetCinematicPlayed As String = "UPDATE characters SET char_moviePlayed = 1 WHERE char_guid = {CharGuid};"
    Public Const GetCharacterGuidByName As String = "SELECT char_guid FROM characters WHERE char_name = ""{CharName}"";"
    Public Const GetCharacterNameByGuid As String = "SELECT char_name FROM characters WHERE char_guid = ""{CharGuid}"";"
    #End Region

    #Region "Group Handlers Queries"
    Public Const GetCharacterInstancesByGuid As String = "SELECT * FROM characters_instances WHERE char_guid = {CharGuid};"
    #End Region
    
    #Region "Guild Handlers Queries"
    Public Const CreateGuild As String = "INSERT INTO guilds (guild_name, guild_leader, guild_cYear, guild_cMonth, guild_cDay) VALUES (""{GuildName}"", {GuildLeader}, {GuildCreationYear}, {GuildCreationMonth}, {GuildCreationDay}); SELECT guild_id FROM guilds WHERE guild_name = ""{GuildName}"";"
    Public Const UpdateGuildRank As String = "UPDATE guilds SET guild_rank{GuildRankId} = ""{GuildRank}"", guild_rank{GuildRightId}_Rights = {GuildRights} WHERE guild_id = {GuildId};"
    Public Const AddGuildRank As String = "UPDATE guilds SET guild_rank{GuildRankId} = '{GuildRank}', guild_rank{GuildRightId}_Rights = '{GuildRights}' WHERE guild_id = {GuildId};"
    Public Const DeleteGuildRank As String = "UPDATE guilds SET guild_rank{GuildRankId} = '{GuildRank}', guild_rank{GuildRightId}_Rights = '{GuildRights}' WHERE guild_id = {GuildId};"
    Public Const FindNewGuildLeadersGuid As String = "SELECT char_guid, char_guildId, char_guildrank FROM characters WHERE char_name = '{CharName}';"
    Public Const SetGuildLeader As String = "UPDATE guilds SET guild_leader = ""{GuildLeader}"" WHERE guild_id = {GuildId};"
    Public Const SetCharacterGuildRank As String = "UPDATE characters SET char_guildRank = {GuildRank} WHERE char_guid = {CharGuid};"
    Public Const SaveGuildEmblem As String = "UPDATE guilds SET guild_tEmblemStyle = {GuildtEmblemStyle}, guild_tEmblemColor = {GuildtEmblemColor}, guild_tBorderStyle = {GuildtBorderStyle}, guild_tBorderColor = {GuildtBorderColor}, guild_tBackgroundColor = {GuildtBackgroundColor} WHERE guild_id = {GuildId};"
    Public Const DeleteGuildInformationById As String = "DELETE FROM guilds WHERE guild_id = {GuildId};"
    public Const SetGuildMOTD As String = "UPDATE guilds SET guild_MOTD = '{GuildMOTD}' WHERE guild_id = '{GuildId}';"
    Public Const SetGuildOfficerNote As String = "UPDATE characters SET char_guildOffNote = ""{CharGuildOffNote}"" WHERE char_name = ""{CharName}"";"
    Public Const SetGuildPublicNote As String = "UPDATE characters SET char_guildPNote = ""{CharGuildPNote}"" WHERE char_name = ""{CharName}"";"
    ' Also Uses GetCharacterGuidByName Defined in Character Handlers Queries
    Public Const GetAllPetitionInfoByGuid As String = "SELECT * FROM petitions WHERE petition_itemGuid = {ItemGuid} LIMIT 1;"
    Public Const GetGuildByGuildId As String = "SELECT * FROM guilds WHERE guild_id = {GuildId};"
    Public Const GetCharacterGuildByGuildId As String = "SELECT char_guid FROM characters WHERE char_guildId = {GuildId};"
    Public Const AddCharacterToGuild As String = "UPDATE characters SET char_guildId = {GuildId}, char_guildRank = {GuildRank}, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {CharGuid};"
    Public Const CountGuildMembers As String = "SELECT char_online, char_guid, char_name, char_class, char_level, char_zone_id, char_logouttime, char_guildRank, char_guildPNote, char_guildOffNote FROM characters WHERE char_guildId = {GuildId};"
    #End Region
    
    #Region "Social Handlers Queries"
    Public Const GetSocialIgnoreList As String = "SELECT * FROM character_social WHERE guid = {CharGuid} AND flags = {SocialFlags};"
    Public Const GetSocialFriendList As String = "SELECT * FROM character_social WHERE guid = {CharGuid} AND (flags & {SocialFlags}) > 0;"
    Public Const GetNotifyFriendStatus As String = "SELECT guid FROM character_social WHERE friend = {FriendGuid} AND (flags & {SocialFlags}) > 0;"
    Public Const GetCharacterGuidAndRaceByName As String = "SELECT char_guid, char_race FROM characters WHERE char_name = ""{CharName}"";"
    Public Const GetSocialFlagsByFlags As String = "SELECT flags FROM character_social WHERE flags = {SocialFlags}"
    Public Const GetSocialFlagsByCharacterGuidFriendGuidFlags As String = "SELECT flags FROM character_social WHERE guid = {CharGuid} AND friend = {FriendGuid} AND flags = {SocialFlags};"
    Public Const InsertCharacterSocial As String = "INSERT INTO character_social (guid, friend, flags) VALUES ({CharGuid}, {FriendGuid}, {SocialFlags});"
    Public Const GetAllByCharacterGuidFriendGuidFlags As String = "SELECT * FROM character_social WHERE guid = {CharGuid} AND friend = {FriendGuid} AND flags = {SocialFlags};"
    Public Const GetSocialFlagsByCharacterGuidFriendGuid As String = "SELECT flags FROM character_social WHERE guid = {CharGuid} AND friend = {FriendGuid};"
    Public Const DeleteCharacterSocialByFrindGuidCharacterGuid As String = "DELETE FROM character_social WHERE friend = {FriendGuid} AND guid = {CharGuid};"
    Public Const UpdateSocialFlagsByFriendGuidCharacterGuid As String = "UPDATE character_social SET flags = {SocialFlags} WHERE friend = {FriendGuid} AND guid = {CharGuid};"
    #End Region

    #Region "Tickets Handlers Queries"
    Public Const GetTicketsByCharacterGuid As String = "SELECT * FROM characters_tickets WHERE char_guid = {CharGuid};"
    Public Const CreateTicket As String = "INSERT INTO characters_tickets (char_guid, ticket_text, ticket_x, ticket_y, ticket_z, ticket_map) VALUES ({CharGuid} , ""{TicketText}"", {TicketX}, {TicketY}, {TicketZ}, {TicketMap});"
    Public Const DeleteTicket As String = "DELETE FROM characters_tickets WHERE char_guid = {CharGuid};"
    Public Const SetTicketCharacterGuidText As String = "UPDATE characters_tickets SET char_guid={CharGuid}, ticket_text=""{TicketText}"";"
    #End Region
    
#End Region

#Region "World Server Queries"
    Public Const UpdateAccountUnBanned As String = "UPDATE account_banned SET active = 0 WHERE id = '{Id}';"
    Public Const DeleteIPBanned As String = "DELETE FROM ip_banned WHERE ip = '{IpAddress}';"

    #Region "World Server Auction Queries"
    Public Const GetItemGuidByItemId As String = "SELECT entry FROM item_template WHERE entry = {ItemId};"
    Public Const AuctionCreateMail As String = "INSERT INTO characters_mail (mail_sender,mail_receiver,mail_type,mail_stationary,mail_subject,mail_body,mail_money,mail_COD,mail_time,mail_read,item_guid) VALUES ({MailSender},{MailReceiver},2,62,{MailSubject},"""","""",0,30,0,{ItemGuid});"
    Public Const GetAuctionByOwner As String = "SELECT * FROM auctionhouse WHERE auction_owner = ""{AuctionOwner}"";"
    Public Const GetAuctionByBidder As String = "SELECT * FROM auctionhouse WHERE auction_bidder = ""{AuctionBidder}"";"
    Public Const CreateAuction As String = "INSERT INTO auctionhouse (auction_bid, auction_buyout, auction_timeleft, auction_bidder, auction_owner, auction_itemId, auction_itemGuid, auction_itemCount) VALUES ({AuctionBid},{AuctionBuyout},{AuctionTimeLeft},{AuctionBidder},{AuctionOwner},{AuctionItemid},{AuctionItemGuid},{AuctionItemCount});"
    Public Const GetAuctionIdByItemGuid As String = "SELECT auction_id FROM auctionhouse WHERE auction_itemGuid = {AuctionItemGuid};"
    Public Const GetAuctionByAuctionId As String = "SELECT * FROM auctionhouse WHERE auction_id = {AuctionId};"
    Public Const CreateCharacterMailForAuction As String = "INSERT INTO characters_mail (mail_sender, mail_receiver, mail_type, mail_stationary, mail_subject, mail_body, mail_money, mail_COD, mail_time, mail_read, item_guid) VALUES ({MailSender},{MailReceiver},{MailType},{MailStationary},'{MailSubject}','{MailBody}',{MailMoney},{MailCOD},{MailTime},{MailRead},{ItemGuid});"
    Public Const GetCharacterMailByReceiver As String = "SELECT mail_id FROM characters_mail WHERE mail_receiver = {MailReceiver};"
    Public Const CreateMailItems As String = "INSERT INTO mail_items (mail_id, item_guid) VALUES ({MailId}, {ItemGuid});"
    Public Const DeleteAuctionByAuctionId As String = "DELETE FROM auctionhouse WHERE auction_id = {AuctionId};"
    Public Const UpdateAuctionBidder As String = "UPDATE auctionhouse SET auction_bidder = {AuctionBidder}, auction_bid = {AuctionBid} WHERE auction_id = {AuctionId};"
    Public Const GetAuctionListItems As String = "SELECT auctionhouse.* FROM {CharacterDB}.auctionhouse, {WorldrDB}.item_template WHERE item_template.entry = auctionhouse.auction_itemId"
    Public Const GetAuctionListItemsAndName As String = " AND item_template.name LIKE '%{Name}%'"
    Public Const GetAuctionListItemsAndLevelMin As String = " AND item_template.itemlevel > {LevelMin}"
    Public Const GetAuctionListItemsAndLevelMax As String = " AND item_template.itemlevel < {LevelMax}"
    Public Const GetAuctionListItemsAndSlot As String = " AND item_template.inventoryType = {InventoryType}"
    Public Const GetAuctionListItemsAndClass As String = " AND item_template.class = {ItemClass}"
    Public Const GetAuctionListItemsAndSubClass As String = " AND item_template.subclass = {ItemSubClass}"
    Public Const GetAuctionListItemsAndQuantity As String = " AND item_template.quality = {ItemQuality}"
    #End Region

    #Region "World Server DBC Database Queries"
    Public Const GetPlayerXPLevel As String = "SELECT * FROM player_xp_for_level order by lvl;"
    Public Const GetBattleMasters As String = "SELECT * FROM battlemaster_entry"
    Public Const GetBattleGrounds As String = "SELECT * FROM battleground_template"
    Public Const GetTeleportCoords As String = "SELECT * FROM spells_teleport_coords"
    Public Const GetMaxItemGuidCharacterInventory As String = "SELECT MAX(item_guid) FROM characters_inventory;"
    Public Const GetMaxSpawnIdCreatures As String = "SELECT MAX(spawn_id) FROM spawns_creatures;"
    Public Const GetMaxSpawnIdGameObjects As String = "SELECT MAX(spawn_id) FROM spawns_gameobjects;"
    Public Const GetMaxGuidForCorpse As String = "SELECT MAX(guid) FROM corpse"
    #End Region

    #Region "World Server DBC Load Queries"
    Public Const GetSpellChains As String = "SELECT spell_id, prev_spell FROM spell_chain"
    Public Const GetAllCreatureGossip As String = "SELECT * FROM npc_gossip;"
    Public Const GetAllWaypointData As String = "SELECT * FROM waypoint_data ORDER BY id, point;"
    Public Const GetAllCreatureEquipTemplateRaw As String = "SELECT * FROM creature_equip_template_raw;"
    Public Const GetAllCreatureModelInfo As String = "SELECT * FROM creature_model_info;"
    Public Const GetAllCreatureQuestRealtion As String = "SELECT * FROM creature_questrelation;"
    Public Const GetAllGameObjectQuestRelation As String = "SELECT * FROM gameobject_questrelation;"
    Public Const GetAllCreatureInvolvedRelation As String = "SELECT * FROM creature_involvedrelation;"
    Public Const GetAllGameObjectInvolvedRelation As String = "SELECT * FROM gameobject_involvedrelation;"
    Public Const GetAllGameWeather As String = "SELECT * FROM game_weather;"
    #End Region

    #Region "World Server Character Movement Handler Queries"
    Public Const GetAllAreaTriggerInvolvedRelationById As String = "SELECT * FROM areatrigger_involvedrelation WHERE id = {Id};"
    Public Const GetAllAreaTriggerTavernById As String = "SELECT * FROM areatrigger_tavern WHERE id = {Id};"
    Public Const GetAllAreaTriggerTeleportById As String = "SELECT * FROM areatrigger_teleport WHERE id = {Id};"
    #End Region

    #Region "World Server Commands Handler Queries"
    Public Const GetAllGameTeleportLocations As String = "SELECT * FROM game_tele order by name"
    Public Const GetGameTeleportLocationByNameLike As String = "SELECT * FROM game_tele WHERE name like '{Name}%' order by name;"
    Public Const GetGameTaleportLocationByName As String = "SELECT * FROM game_tele WHERE name = '{Name}' order by name LIMIT 1;"
    Public Const ForceCharacterRename As String = "UPDATE characters SET force_restrictions = 1 WHERE char_guid = {CharGuid};"
    Public Const BanCharacter As String = "UPDATE characters SET force_restrictions = 2 WHERE char_guid = {CharGuid};"
    Public Const GetActiveFromAccountBannedById As String = "SELECT active FROM account_banned WHERE id = {AccountId};"
    Public Const InsertIpBannedAccount As String = "INSERT INTO `ip_banned` VALUES ('{Ip}', UNIX_TIMESTAMP({BanDate}), UNIX_TIMESTAMP({UnBanDate}), '{BannedBy}', '{BanReason}');"
    Public Const GetUserNameByName As String = "SELECT username FROM account WHERE username = ""{UserName}"";"
    Public Const GetIdGMLevelByName As String = "SELECT id, gmlevel FROM account WHERE username = ""{UserName}"";"
    Public Const ChangePasswordForAccount As String = "UPDATE account SET sha_pass_hash='{ShaPassHash}' WHERE id={Id}"
    Public Const SetGmLevelForAccount As String = "UPDATE account SET gmlevel={GMLevel} WHERE id={Id}"
    #End Region

    #Region "World Server Instance Handler Queries"
    Public Const InstanceMapExpireLessThan As String = "SELECT * FROM characters_instances WHERE expire < {Expire};"
    Public Const GetMaxInstanceFromCharacterInstance As String = "SELECT MAX(instance) FROM characters_instances WHERE map = {Map};"
    Public Const DeleteCharacterInstance As String = "DELETE FROM characters_instances WHERE instance = {Instance} AND map = {Map};"
    Public Const DeleteCharacterInstanceGroup As String = "DELETE FROM characters_instances_group WHERE instance = {Instance} AND map = {Map};"
    Public Const ExtendExpireCharacterInstance As String = "UPDATE characters_instances SET expire = {Expire} WHERE instance = {Instance} AND map = {Map};"
    Public Const ExtendExpireCharacterInstanceGroup As String = "UPDATE characters_instances_group SET expire = {Expire} WHERE instance = {Instance} AND map = {Map};"
    Public Const GetCharacterInstanceinfo As String = "SELECT * FROM characters_instances WHERE char_guid = {CharGuid} AND map = {Map};"
    Public Const GetGroupInstanceInfo As String = "SELECT * FROM characters_instances_group WHERE group_id = {GroupId} AND map = {Map};"
    Public Const InsertGroupInstanceInfo As String = "INSERT INTO characters_instances_group (group_id, map, instance, expire) VALUES ({GroupId}, {Map}, {Instance}, {Expire});"
    Public Const GetCharacterInstanceInfoByGuid As String = "SELECT * FROM characters_instances WHERE char_guid = {CharGuid};"
    #End Region

    #Region "World Server Miscellaneous Handler Queries"
    Public Const CharacterNameQueryResponse As String = "SELECT char_name, char_race, char_class, char_gender FROM characters WHERE char_guid = ""{CharGuid}"";"
    Public Const DeleteCorpseByPlayer As String = "DELETE FROM corpse WHERE player = ""{Player}"";"
    #End Region

    #Region "World Server Loot Queries"
    Public Const CreateLootTemplate As String = "SELECT * FROM {Name} WHERE entry = {Entry};"
    #End Region

    #Region "World Server Map Queries"
    Public Const GetCreatureSpawns As String = "SELECT * FROM spawns_creatures LEFT OUTER JOIN game_event_creature ON spawns_creatures.spawn_id = game_event_creature.guid WHERE spawn_map={SpawnMap} AND spawn_positionX BETWEEN '{PostionX1}' AND '{PositionX2}' AND spawn_positionY BETWEEN '{PositionY1}' AND '{PositionY2}';"
    Public Const GetGameObjectSpawns As String = "SELECT * FROM spawns_gameobjects LEFT OUTER JOIN game_event_gameobject ON spawns_gameobjects.spawn_id = game_event_gameobject.guid WHERE spawn_map={SpawnMap} AND spawn_spawntime>=0 AND spawn_positionX BETWEEN '{PostionX1}' AND '{PositionX2}' AND spawn_positionY BETWEEN '{PositionY1}' AND '{PositionY2}';"
    Public Const GetCorpseSpawns As String = "SELECT * FROM corpse WHERE map={Map} AND instance={Instance} AND position_x BETWEEN '{PostionX1}' AND '{PostionX2}' AND position_y BETWEEN '{PositionY1}' AND '{PositionY2}';"
    #End Region

    #Region "World Server Graveyard Queries"
    Public Const GetIdFactionFromGraveyardZone As String = "SELECT id, faction FROM game_graveyard_zone WHERE ghost_zone = {GhostZone} and (faction = 0 or faction = {Faction}) "
    Public Const GetIdFactionFromGraveyardZoneByGhostZone As String = "SELECT id, faction FROM game_graveyard_zone WHERE ghost_zone = {GhostZone}"
    #End Region

    #Region "World Server Corpses Queries"
    Public Const CreateCorpse As String = "INSERT INTO corpse (guid, player, position_x, position_y, position_z, orientation, map, time, corpse_type, instance)  VALUES ({Guid}, {Player}, {PositionX}, {PositionY}, {PositionZ}, {Orientation}, {Map}, UNIX_TIMESTAMP({Time}), {CorpseType}, {Instance});"
    Public Const GetCorpseInfoByGuid As String = "SELECT * FROM corpse WHERE guid = {Guid};"
    #End Region

    #Region"World Server Creature Info Queries"
    Public Const GetCreatureByEntry As String = "SELECT * FROM creatures WHERE entry = {Entry};"
    #End Region

    #Region "World Server Creature Queries"
    Public Const GetCreatureSpawnsBySpawnId As String = "SELECT * FROM spawns_creatures LEFT OUTER JOIN game_event_creature ON spawns_creatures.spawn_id = game_event_creature.guid WHERE spawn_id = {SpawnId};"
    Public Const GetCreatureSpawnsAddonBySpawnId As String = "SELECT * FROM spawns_creatures_addon WHERE spawn_id = {SpawnId};"
    Public Const GetNpcTextById As String = "SELECT * FROM npc_text WHERE ID = {Id};"
    #End Region

    #Region "World Server Game Object Queries"
    Public Const GetGameObjectTemplateByEntry As String = "SELECT * FROM gameobject_template WHERE entry = {Entry};"
    Public Const GetGameObjectSpawnsBySpawnId As String = "SELECT * FROM spawns_gameobjects LEFT OUTER JOIN game_event_gameobject ON spawns_gameobjects.spawn_id = game_event_gameobject.guid WHERE spawn_id = {SpawnId};"
    Public Const GetSkillFishingBaseLevelByEntry As String = "SELECT * FROM skill_fishing_base_level WHERE entry = {Entry};"
    #End Region

    #Region "World Server Item Object Queries"
    Public Const GetItemLootByEntry As String = "SELECT * FROM item_loot WHERE entry = {Entry};"
    Public Const GetCharacterInventoryByItemGuid As String = "SELECT * FROM characters_inventory WHERE item_guid = ""{ItemGuid}"";"
    Public Const GetCharacterInventoryByItemBag As String = "SELECT * FROM characters_inventory WHERE item_bag = {ItemBag};"
    Public Const SaveNewCharacterInventory As String = "INSERT INTO characters_inventory (item_guid, item_id, item_owner, item_creator, item_giftCreator, item_stackCount, item_durability, item_flags, item_chargesLeft, item_textId, item_enchantment, item_random_properties) VALUES ({ItemGuid}, {ItemId}, {ItemOwner}, {ItemCreator}, {ItemGiftCreator}, {ItemStackCount}, {ItemDurability}, {ItemFlags}, {ItemChargesLeft}, {ItemTextId}, ""{ItemEnchantment}"", {ItemRandomProperties});"
    Public Const UpdateCharacterInventory As String = "UPDATE characters_inventory SET  item_owner=""{ItemOwner}"", item_creator={ItemCreator}, item_giftCreator={ItemGiftCreator}, item_stackCount={ItemStackCount}, item_durability={ItemDurability}, item_chargesLeft={ItemChargesLeft}, item_random_properties={ItemRandomProperties}, item_flags={ItemFlags}, item_enchantment=""{ItemEnchantment}"", item_textId={ItemTextId} WHERE item_guid = ""{ItemGuid}"";"
    Public Const DeletePetitionByItemGuid As String = "DELETE FROM petitions WHERE petition_itemGuid = {PetitionItemGuid};"
    #End Region

    #Region "World Server Item Queries"
    Public Const GetAllItemTemplateByEntry As String = "SELECT * FROM item_template WHERE entry = {Entry};"
    Public Const GetAllPageTextByEntry As String = "SELECT * FROM page_text WHERE entry = ""{Entry}"";"
    #End Region

    #Region "World Server NPC Queries"
    Public Const TrainerBuySpell As String = "SELECT * FROM npc_trainer WHERE entry = {Entry} AND spell = {Spell};"
    Public Const GetTrainerByEntry As String = "SELECT * FROM npc_trainer WHERE entry = {Entry};"
    Public Const GetVendorByEntry As String = "SELECT * FROM npc_vendor WHERE entry = {Entry};"
    Public Const BuyBankSlot As String = "UPDATE characters SET char_bankSlots = {BankSlots}, char_copper = {Copper};"
    #End Region

    #Region "World Server Pet Queries"
    Public Const GetPetByOwner As String = "SELECT * FROM character_pet WHERE owner = '{Owner}';"
    #End Region

    #Region "World Server Transports Queries"
    Public Const GetAllTransports As String = "SELECT * FROM transports"
    #End Region

    #Region "World Server Player Creation Queries"
    Public Const GetCharacterName As String = "SELECT char_name FROM characters WHERE char_name = ""{CharName}"";"
    Public Const GetCharacterRaceByAccountId As String = "SELECT char_race FROM characters WHERE account_id = ""{AccountId}"" LIMIT 1;"
    Public Const GetCharacterNamesByAccountId As String = "SELECT char_name FROM characters WHERE account_id = ""{AccountId}"";"
    Public Const GetPlayerCreateInfoByRaceAndClass As String = "SELECT * FROM playercreateinfo WHERE race = {Race} AND class = {Class};"
    Public Const GetPlayerCreateInfoActionByRaceAndClass As String = "SELECT * FROM playercreateinfo_action WHERE race = {Race} AND class = {Class} ORDER BY button;"
    Public Const GetPlayerCreateInfoSkillByRaceAndClass As String = "SELECT * FROM playercreateinfo_skill WHERE race = {Race} AND class = {Class};"
    Public Const GetPlayerLevelStats As String = "SELECT * FROM player_levelstats WHERE race = {Race} AND class = {Class} AND level = {Level};"
    Public Const GetPlayerClassLevelStats As String = "SELECT * FROM player_classlevelstats WHERE class = {Class} AND level = {Level};"
    Public Const GetPlayerCreateInfoSpellByRaceAndClass As String = "SELECT * FROM playercreateinfo_spell WHERE race = {Race} AND class = {Class};"
    Public Const GetPlayerCreateInfoItemByRaceAndClass As String = "SELECT * FROM playercreateinfo_item WHERE race = {Race} AND class = {Class};"
    #End Region

    #Region "World Server Player Data Type Queries"
    Public Const HonorSave As String = "INSERT INTO characters_honor (char_guid) VALUES ({CharGuid});"
    Public Const HonorUpdate As String = "UPDATE characters_honor SET honor_points = {HonorPoints}, kills_honor = {HonorKills}, kills_dishonor = {DisHonorKills}, honor_yesterday = {HonorYesterday}, honor_thisWeek = {HonorThisWeek}, kills_thisWeek = {KillsThisWeek}, kills_today = {KillsToday}, kills_dishonortoday = {DisHonorKillsToday} WHERE char_guid = ""{CharGuid}"";"
    Public Const GetCharacterHonorByGuid As String = "SELECT * FROM characters_honor WHERE char_guid = {CharGuid};"
    Public Const LearnCharacterSpells As String = "INSERT INTO characters_spells (guid, spellid, active, cooldown, cooldownitem) VALUES ({Guid},{SpellId},{Active},{CoolDown},{CoolDownItem});"
    Public Const DeactivateCharacterSpells As String = "UPDATE characters_spells SET active = 0 WHERE guid = {Guid} AND spellid = {SpellId};"
    Public Const UnLearnCharacterSpells As String = "DELETE FROM characters_spells WHERE guid = {Guid} AND spellid = {SpellId};"
    Public Const UpdateItemSlotItemBagByGuid As String = "UPDATE characters_inventory SET item_slot = {ItemSlot}, item_bag = {ItemBag} WHERE item_guid = {ItemGuid};"
    Public Const SetItemSlotItemBagStackCountByGuid As String = "UPDATE characters_inventory SET item_slot = {ItemSlot}, item_bag = {ItemBag}, item_stackCount = {ItemStackCount} WHERE item_guid = {ItemGuid};"
    Public Const SetCharacterOnline As String = "SELECT * FROM characters WHERE char_guid = {CharaterGuid}; UPDATE characters SET char_online = 1 WHERE char_guid = {CharGuid};"
    Public Const ResetSpellCoolDown As String = "UPDATE characters_spells SET cooldown = 0, cooldownitem = 0 WHERE guid = {CharGuid} AND cooldown > 0 AND cooldown < {CoolDown}; 
    SELECT * FROM characters_spells WHERE guid = {CharSpellGuid}; 
    UPDATE characters_spells SET cooldown = 0, cooldownitem = 0 WHERE guid = {CharSpellsGuid} AND cooldown > 0 AND cooldown < {SpellCoolDown};"
    Public Const GetAllCorpseByPlayer As String = "SELECT * FROM corpse WHERE player = {Player};"
    Public Const SaveAsNewCharacter As String = "INSERT INTO characters (account_id, char_name, char_race, char_class, char_gender, char_skin, char_face, 
    char_hairStyle, char_hairColor, char_facialHair, char_level, char_manaType, char_mana, char_rage, char_energy, char_life, char_positionX, char_positionY, 
    char_positionZ, char_map_id, char_zone_id, char_orientation, bindpoint_positionX, bindpoint_positionY, bindpoint_positionZ, bindpoint_map_id, bindpoint_zone_id,
    char_copper, char_xp, char_xp_rested, char_skillList, char_auraList, char_tutorialFlags, char_mapExplored, char_reputation, char_actionBar, char_strength,
    char_agility, char_stamina, char_intellect, char_spirit, force_restrictions) VALUES ({AccountId}, ""{CharName}"", {CharRace}, {CharClass}, {CharGender}, {CharSkin}, {CharFace},
    {CharHairStyle}, {CharHairColor}, {CharFacialHair}, {CharLevel}, {CharManaType}, {CharMana}, {CharRage}, {CharEnergy}, {CharLife}, {CharPositionX}, {CharPositionY},
    {CharPositionZ}, {CharMapId}, {CharZoneId}, {CharOrientation}, {BindpointPositionX}, {BindpointPositionY}, {BindpointPositionZ}, {BindpointMapId}, {BindpointZoneId},
    {CharCopper}, {CharXp}, {CharXpRested}, ""{CharSkillList}"", ""{CharAuraList}"", ""{CharTutorialFlags}"", ""{CharMapExplored}"", ""{CharReputation}"", 
    ""{CharActionBar}"", {CharStrength}, {CharAgility}, {CharStamina}, {CharIntellect}, {CharSpirit}, {ForceRestrictions});"
    Public Const UpdateCharacterByGuid As String = "UPDATE characters SET char_name=""{CharName}"", char_race={CharRace}, char_class={CharClass}, char_gender={CharGender},
    char_skin={CharSkin}, char_face={CharFace}, char_hairStyle={CharHairStyle}, char_hairColor={CharHairColor}, char_facialHair={CharFacialHair}, char_level={CharLevel},
    char_manaType={CharManaType}, char_life={CharLife}, char_rage={CharRage}, char_mana={CharMana}, char_energy={CharEnergy}, char_strength={CharStrength},
    char_agility={CharAgility}, char_stamina={CharStamina}, char_intellect={CharIntellect}, char_spirit={CharSpirit}, char_map_id={CharMapId}, char_zone_id={CharZoneId},
    char_positionX={CharPositionX}, char_positionY={CharPositionY}, char_positionZ={CharPositionZ}, char_orientation={CharOrientation}, char_transportGuid={CharTransportGuid},
    bindpoint_positionX={BindpointPositionX}, bindpoint_positionY={BindpointPositionY}, bindpoint_positionZ={BindpointPositionZ}, bindpoint_map_id={BindpointMapId},
    bindpoint_zone_id={BindpointZoneId}, char_copper={CharCopper}, char_xp={CharXp}, char_xp_rested={CharXpRested}, char_guildId={CharGuildId}, char_guildRank={CharGuildRank},
    char_skillList=""{CharSkillList}"", char_auraList=""{CharAuraList}"", char_tutorialFlags=""{CharTutorialFlags}"", char_taxiFlags=""{CharTaxiFlags}"",
    char_mapExplored=""{CharMapExplored}"", char_reputation=""{CharReputation}"", char_actionBar=""{CharActionBar}"", char_talentpoints={CharTalentpoints},
    force_restrictions={ForceRestrictions} WHERE char_guid = ""{CharGuid}"";"
    Public Const SavePosition As String = "UPDATE characters SET char_positionX={CharPositionX}, char_positionY={CharPositionY}, char_positionZ={CharPositionZ}, char_orientation={CharOrientation}, char_map_id={CharMapId} WHERE char_guid = ""{CharGuid}"";"
    Public Const AddQuest As String = "INSERT INTO characters_quests (char_guid, quest_id, quest_status) VALUES ({CharGuid}, {QuestId}, {QuestStatus});"
    Public Const DeleteQuest As String = "DELETE FROM characters_quests WHERE char_guid = {CharGuid} AND quest_id = {QuestId};"
    Public Const CompleteQuest As String = "UPDATE characters_quests SET quest_status = -1 WHERE char_guid = {CharGuid} AND quest_id = {QuestId};"
    Public Const UpdateQuest As String = "UPDATE characters_quests SET quest_status = {QuestStatus} WHERE char_guid = {CharGuid} AND quest_id = {QuestId};"
    Public Const CanAcceptQuest As String = "SELECT quest_status FROM characters_quests WHERE char_guid = {CharGuid} AND quest_id = {QuestId} LIMIT 1;"
    Public Const CheckQuestCompletion As String = "SELECT quest_id FROM characters_quests WHERE char_guid = {CharGuid} AND quest_status = -1 AND quest_id = {QuestId};"
    #End Region

    #Region "Wolrd Server Player Helper Queries"
    Public Const CharacterHonorSave As String = "UPDATE characters_honor SET honor_points=""{HonorPoints}"", honor_rank={HonorRank}, 
    honor_hightestRank={HonorHightestRank}, honor_standing={HonorStanding}, honor_lastWeek={HonorLastWeek}, honor_thisWeek={HonorThisWeek}, 
    honor_yesterday={HonorYesterday}, kills_lastWeek={KillsLastWeek}, kills_thisWeek={KillsThisWeek}, kills_yesterday={KillsYesterday},
    kills_dishonorableToday={KillsDishonorableToday}, kills_honorableToday={KillsHonorableToday}, kills_dishonorableLifetime={KillsDishonorableLifetime},
    kills_honorableLifetime={KillsHonorableLifetime} WHERE char_guid = ""{CharGuid}"";"
    #End Region

    #Region "World Server Quest Info Queries"
    Public Const GetAllQuestsByEntry As String = "SELECT * FROM quests WHERE entry = {Entry};"
    #End Region

    #Region "World Server Quests Queries"
    Public Const GetAllEntryFromQuests As String = "SELECT entry FROM quests;"
    Public Const GetAllCharacterQuests As String = "SELECT quest_id, quest_status FROM characters_quests q WHERE q.char_guid = {CharGuid};"
    #End Region

    #Region "World Server Guilds Queries"
    Public Const GetGuildIdByGuildName As String = "SELECT guild_id FROM guilds WHERE guild_name = '{GuildName}'"
    Public Const SaveGuildPetition As String = "INSERT INTO petitions (petition_id, petition_itemGuid, petition_owner, petition_name, petition_type, petition_signedMembers) VALUES ({PetitionId}, {PetitionItemGuid}, {PetitionOwner}, '{PetitionName}', {PetitionType}, 0);"
    Public Const GetPetitionInfoByGuid As String = "SELECT * FROM petitions WHERE petition_itemGuid = {ItemGuid};"
    Public Const PetitionRename As String = "UPDATE petitions SET petition_name = '{PetitionName}' WHERE petition_itemGuid = {ItemGuid};"
    Public Const GetPetitionSignedMembersAndOwnerByGuid As String = "SELECT petition_signedMembers, petition_owner FROM petitions WHERE petition_itemGuid = {ItemGuid};"
    Public Const UpdatePetitionSignedMembers As String = "UPDATE petitions SET petition_signedMembers = petition_signedMembers + 1, petition_signedMember{PetitionSignedMemberId} = {PetitionSignedMember} WHERE petition_itemGuid = {ItemGuid};"
    Public Const GetPetitionOwnerByGuid As String = "SELECT petition_owner FROM petitions WHERE petition_itemGuid = {ItemGuid} LIMIT 1;"
    #End Region

    #Region "World Server Mail Queries"
    Public Const MailReturnToSender As String = "UPDATE characters_mail SET mail_time = {MailTime}, mail_read = 0, mail_receiver = (mail_receiver + mail_sender), mail_sender = (mail_receiver - mail_sender), mail_receiver = (mail_receiver - mail_sender) WHERE mail_id = {MailId};"
    Public Const MailDeleteById As String = "DELETE FROM characters_mail WHERE mail_id = {MailId};"
    Public Const MailMarkAsRead As String = "UPDATE characters_mail SET mail_read = 1, mail_time = {MailTime} WHERE mail_id = {MailId} AND mail_read < 2;"
    Public Const MailQueryNextMailTime As String = "SELECT COUNT(*) FROM characters_mail WHERE mail_read = 0 AND mail_receiver = {MailReceiver} AND mail_time > {MailTime};"
    Public Const MailGetMailIdByMailTimeLessThan As String = "SELECT mail_id FROM characters_mail WHERE mail_time < {MailTime};"
    Public Const MailGetAllByReceiver As String = "SELECT * FROM characters_mail WHERE mail_receiver = {MailReceiver};"
    Public Const MailTakeItemCheckReceiver As String = "SELECT mail_cod, mail_sender, item_guid FROM characters_mail WHERE mail_id = {MailId} AND mail_receiver = {MailReceiver};"
    Public Const MailPayCod As String = "UPDATE characters_mail SET mail_cod = 0 WHERE mail_id = {MailId};"
    Public Const MailSendCodToSender As String = "INSERT INTO characters_mail (mail_sender, mail_receiver, mail_subject, mail_body, mail_item_guid, mail_money, 
    mail_COD, mail_time, mail_read, mail_type) VALUES ({MailSender},{MailReceiver},'{MailSubject}','{MailBody}',{MailItemGuid},{MailMoney},{MailCOD},{MailTime},
    {MailRead},{MailType});"
    Public Const MailSendNoSlotError As String = "UPDATE characters_mail SET item_guid = 0 WHERE mail_id = {MailId}; DELETE FROM mail_items WHERE mail_id = {MailItemsMailId};"
    Public Const MailTakeMoney As String = "SELECT mail_money FROM characters_mail WHERE mail_id = {MailId}; UPDATE characters_mail SET mail_money = 0 WHERE mail_id = {UpdateMailId};"
    Public Const MailGetMailBodyById As String = "SELECT mail_body FROM characters_mail WHERE mail_id = {MailId};"
    Public Const GetCharacterGuidAndRaceNameLike As String = "SELECT char_guid, char_race FROM characters WHERE char_name Like '{CharName}';"
    Public Const MailSendMail As String = "INSERT INTO characters_mail (mail_sender, mail_receiver, mail_type, mail_stationary, mail_subject, mail_body, mail_money,
    mail_COD, mail_time, mail_read, item_guid) VALUES ({MailSender},{MailReceiver},{MailType},{MailStationary},'{MailSubject}','{MailBody}',{MailMoney},{MailCOD},
    {MailTime},{MailRead},{ItemGuid});"
    #End Region

    #Region "World Server Spells Queries"
    Public Const SaveSpellCooldowns As String = "UPDATE characters_spells SET cooldown={CoolDown}, cooldownitem={CoolDownItem} WHERE guid = {Guid} AND spellid = {SpellId};"
    #End Region

#End Region

End Class
