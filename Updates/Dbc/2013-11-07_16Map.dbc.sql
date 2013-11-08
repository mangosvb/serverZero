DROP TABLE IF EXISTS `dbc_Map`;
CREATE TABLE `dbc_Map` (
	`ID` INT NOT NULL DEFAULT '0',
	`InternalMapName` TEXT NOT NULL,
	`InstanceType` INT NOT NULL DEFAULT '0',
	`isBattleground` TEXT NOT NULL,
	`mapName` TEXT NOT NULL,
	`mapName1` TEXT NOT NULL,
	`mapName2` TEXT NOT NULL,
	`mapName3` TEXT NOT NULL,
	`mapName4` TEXT NOT NULL,
	`mapName5` TEXT NOT NULL,
	`mapName6` TEXT NOT NULL,
	`mapName7` TEXT NOT NULL,
	`col12` BIGINT NOT NULL DEFAULT '0',
	`MinLvl` INT NOT NULL DEFAULT '0',
	`MaxLvl` INT NOT NULL DEFAULT '0',
	`MaxPlayers` INT NOT NULL DEFAULT '0',
	`ParentMapId` TEXT NOT NULL,
	`col17` BIGINT NOT NULL DEFAULT '0',
	`col18` BIGINT NOT NULL DEFAULT '0',
	`col19` INT NOT NULL DEFAULT '0',
	`HordeDescription` TEXT NOT NULL,
	`HordeDescription1` TEXT NOT NULL,
	`HordeDescription2` TEXT NOT NULL,
	`HordeDescription3` TEXT NOT NULL,
	`HordeDescription4` TEXT NOT NULL,
	`HordeDescription5` TEXT NOT NULL,
	`HordeDescription6` TEXT NOT NULL,
	`HordeDescription7` TEXT NOT NULL,
	`col28` BIGINT NOT NULL DEFAULT '0',
	`AllianceDescription` TEXT NOT NULL,
	`AllianceDescription1` TEXT NOT NULL,
	`AllianceDescription2` TEXT NOT NULL,
	`AllianceDescription3` TEXT NOT NULL,
	`AllianceDescription4` TEXT NOT NULL,
	`AllianceDescription5` TEXT NOT NULL,
	`AllianceDescription6` TEXT NOT NULL,
	`AllianceDescription7` TEXT NOT NULL,
	`col37` BIGINT NOT NULL DEFAULT '0',
	`loadingscreenId` INT NOT NULL DEFAULT '0',
	`raidOffset` INT NOT NULL DEFAULT '0',
	`ContinentName` TEXT NOT NULL,
	`col41` TEXT NOT NULL)
 ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci COMMENT='Export of Map';
 SET NAMES UTF8;

INSERT INTO `dbc_Map` VALUES (0,"Azeroth",0,"","Eastern Kingdoms","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128894,"","0","0","0","0","0","0","0",4128894,4,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (1,"Kalimdor",0,"","Kalimdor","0","0","0","0","0","0","0",8323198,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,3,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (13,"test",0,"","Testing","0","0","0","0","0","0","0",8323198,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,0,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (25,"ScottTest",0,"","Scott Test","0","0","0","0","0","0","0",8323198,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,0,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (29,"Test",1,"","CashTest","0","0","0","0","0","0","0",8323198,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,0,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (30,"PVPZone01",3,"Azeroth","Alterac Valley","0","0","0","0","0","0","0",4128894,51,60,40,"0",0.74,0.34,0,"Hidden within the Alterac Mountains, Alterac Valley is the home of Thrall�s own clan of orcs, the Frostwolves. The Stormpike dwarves have established a foothold in the valley and seek to plumb its depths for riches, and links to their ancestral past. The territorial Frostwolves, unwilling to suffer the dwarven incursion, have rallied an army... an army eager for righteous slaughter.","0","0","0","0","0","0","0",4128894,"Hidden within the Alterac Mountains, Alterac Valley is the home of Thrall�s own clan of orcs, the Frostwolves. The Stormpike dwarves have established a foothold in the valley and seek to plumb its depths for riches, and links to their ancestral past. The territorial Frostwolves, unwilling to suffer the dwarven incursion, have rallied an army... an army eager for righteous slaughter.","0","0","0","0","0","0","0",4128894,104,0,"","Azeroth");
INSERT INTO `dbc_Map` VALUES (33,"Shadowfang",1,"","Shadowfang Keep","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,204,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (34,"StormwindJail",1,"","Stormwind Stockade","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,717,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,194,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (35,"StormwindPrison",0,"","<unused>StormwindPrison","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,717,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,23,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (36,"DeadminesInstance",1,"","Deadmines","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,142,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (37,"PVPZone02",0,"Azeroth","Azshara Crater","0","0","0","0","0","0","0",4128894,10,20,30,"0",-5409,-2884,0,"Crush the Alliance!","0","0","0","0","0","0","0",4128894,"Defend yourself from the onslaught of the Horde!","0","0","0","0","0","0","0",4128895,25,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (42,"Collin",0,"","Collin\'s Test","0","0","0","0","0","0","0",8323198,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,0,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (43,"WailingCaverns",1,"","Wailing Caverns","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,718,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,143,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (44,"Monastery",1,"","<unused> Monastery","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,42,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (47,"RazorfenKraulInstance",1,"","Razorfen Kraul","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,188,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (48,"Blackfathom",1,"","Blackfathom Deeps","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,719,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,196,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (70,"Uldaman",1,"","Uldaman","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,1337,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,144,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (90,"GnomeragonInstance",1,"","Gnomeregan","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,721,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,193,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (109,"SunkenTemple",1,"","Sunken Temple","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,1477,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,191,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (129,"RazorfenDowns",1,"","Razorfen Downs","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,145,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (169,"EmeraldDream",2,"","Emerald Dream","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,0,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (189,"MonasteryInstances",1,"","Scarlet Monastery","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,190,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (209,"TanarisInstance",1,"","Zul\'Farrak","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,146,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (229,"BlackRockSpire",1,"","Blackrock Spire","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,1583,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,189,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (230,"BlackrockDepths",1,"","Blackrock Depths","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,1584,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,103,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (249,"OnyxiaLairInstance",2,"","Onyxia\'s Lair","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2159,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,61,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (269,"CavernsOfTime",1,"","Caverns of Time","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,4,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (289,"SchoolofNecromancy",1,"","Scholomance","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,102,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (309,"Zul\'gurub",2,"","Zul\'Gurub","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,1977,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,161,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (329,"Stratholme",1,"","Stratholme","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,101,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (349,"Mauradon",1,"","Maraudon","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2100,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,81,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (369,"DeeprunTram",0,"","Deeprun Tram","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2257,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,205,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (389,"OrgrimmarInstance",1,"","Ragefire Chasm","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2437,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,195,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (409,"MoltenCore",2,"","Molten Core","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2717,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,192,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (429,"DireMaul",1,"","Dire Maul","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2557,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,82,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (449,"AlliancePVPBarracks",0,"","Alliance PVP Barracks","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2918,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,181,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (450,"HordePVPBarracks",0,"","Horde PVP Barracks","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2917,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,182,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (451,"development",0,"","Development Land","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,0,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,21,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (469,"BlackwingLair",2,"","Blackwing Lair","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,2677,"","0","0","0","0","0","0","0",4128892,"","0","0","0","0","0","0","0",4128892,141,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (489,"PVPZone03",3,"Azeroth","Warsong Gulch","0","0","0","0","0","0","0",4128894,10,60,10,"0",0,0,3277,"A valley bordering Ashenvale Forest and the Barrens, Warsong Gulch hosts a constant battle between the Horde and the Alliance. Orcs use their Warsong Mill to cut lumber reaped from Ashenvale, provoking the night elves of Silverwing Hold. Eager to aid their allies, members of every race in Azeroth rush to the Gulch to lend sword, or spell, to the conflict.","0","0","0","0","0","0","0",4128894,"A valley bordering Ashenvale Forest and the Barrens, Warsong Gulch hosts a constant battle between the Horde and the Alliance. Orcs use their Warsong Mill to cut lumber reaped from Ashenvale, provoking the night elves of Silverwing Hold. Eager to aid their allies, members of every race in Azeroth rush to the Gulch to lend sword, or spell, to the conflict.","0","0","0","0","0","0","0",4128894,122,10,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (509,"AhnQiraj",2,"","Ruins of Ahn\'Qiraj","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,3429,"","0","0","0","0","0","0","0",4128876,"","0","0","0","0","0","0","0",4128892,184,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (529,"PVPZone04",3,"Azeroth","Arathi Basin","0","0","0","0","0","0","0",4128894,20,60,15,"0",0,0,3358,"The Arathi Basin is one of the main staging points of war between the Humans and the Forsaken in Azeroth. The League of Arathor seek to reclaim lost lands for their benefactors in Stormwind, while the Defilers – elite troopers under the watchful eye of Varimathras – seek to sever the vital connection between the Humans and their Dwarven allies to the south.","0","0","0","0","0","0","0",4128894,"The Arathi Basin is one of the main staging points of war between the Humans and the Forsaken in Azeroth. The League of Arathor seek to reclaim lost lands for their benefactors in Stormwind, while the Defilers – elite troopers under the watchful eye of Varimathras – seek to sever the vital connection between the Humans and their Dwarven allies to the south.","0","0","0","0","0","0","0",4128894,183,10,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (531,"AhnQirajTemple",2,"","Ahn\'Qiraj Temple","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,3428,"","0","0","0","0","0","0","0",4128876,"","0","0","0","0","0","0","0",4128892,185,0,"Azeroth","Azeroth");
INSERT INTO `dbc_Map` VALUES (533,"Stratholme Raid",2,"","Naxxramas","0","0","0","0","0","0","0",4128894,0,0,0,"0",0,0,3456,"","0","0","0","0","0","0","0",4128844,"","0","0","0","0","0","0","0",4128844,197,0,"Azeroth","Azeroth");
