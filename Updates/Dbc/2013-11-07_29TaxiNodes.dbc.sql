DROP TABLE IF EXISTS `dbc_TaxiNodes`;
CREATE TABLE `dbc_TaxiNodes` (
	`ID` INT NOT NULL DEFAULT '0',
	`MapId` BIGINT NOT NULL DEFAULT '0',
	`LocationX` BIGINT NOT NULL DEFAULT '0',
	`LocationY` FLOAT NOT NULL DEFAULT '0',
	`LocationZ` FLOAT NOT NULL DEFAULT '0',
	`Name` TEXT NOT NULL,
	`Name1` TEXT NOT NULL,
	`Name2` TEXT NOT NULL,
	`Name3` TEXT NOT NULL,
	`Name4` TEXT NOT NULL,
	`Name5` TEXT NOT NULL,
	`Name6` TEXT NOT NULL,
	`Name7` TEXT NOT NULL,
	`col13` BIGINT NOT NULL DEFAULT '0',
	`mountCreatureDisplayInfoID1` INT NOT NULL DEFAULT '0',
	`mountCreatureDisplayInfoID2` INT NOT NULL DEFAULT '0')
 ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci COMMENT='Export of TaxiNodes';
 SET NAMES UTF8;

INSERT INTO `dbc_TaxiNodes` VALUES (1,0,-8888.98,-0.54,94.39,"Northshire Abbey","0","0","0","0","0","0","0",8323198,308,0);
INSERT INTO `dbc_TaxiNodes` VALUES (2,0,-8840.56,489.7,109.61,"Stormwind, Elwynn","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (3,0,16391.81,16341.21,69.44,"Programmer Isle","0","0","0","0","0","0","0",4128894,308,0);
INSERT INTO `dbc_TaxiNodes` VALUES (4,0,-10628.89,1036.68,34.06,"Sentinel Hill, Westfall","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (5,0,-9429.1,-2231.4,68.65,"Lakeshire, Redridge","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (6,0,-4821.78,-1155.44,502.21,"Ironforge, Dun Morogh","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (7,0,-3792.26,-783.29,9.06,"Menethil Harbor, Wetlands","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (8,0,-5421.91,-2930.01,347.25,"Thelsamar, Loch Modan","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (9,0,-14271.77,299.87,31.09,"Booty Bay, Stranglethorn","0","0","0","0","0","0","0",8323198,541,0);
INSERT INTO `dbc_TaxiNodes` VALUES (10,0,478.86,1536.59,131.32,"The Sepulcher, Silverpine Forest","0","0","0","0","0","0","0",4128894,3574,0);
INSERT INTO `dbc_TaxiNodes` VALUES (11,0,1568.62,267.97,-43.1,"Undercity, Tirisfal","0","0","0","0","0","0","0",4128894,3574,0);
INSERT INTO `dbc_TaxiNodes` VALUES (12,0,-10515.46,-1261.65,41.34,"Darkshire, Duskwood","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (13,0,-0.06,-859.91,58.83,"Tarren Mill, Hillsbrad","0","0","0","0","0","0","0",8323198,3574,0);
INSERT INTO `dbc_TaxiNodes` VALUES (14,0,-711.48,-515.48,26.11,"Southshore, Hillsbrad","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (15,0,2253.4,-5344.9,83.38,"Eastern Plaguelands","0","0","0","0","0","0","0",8323198,541,0);
INSERT INTO `dbc_TaxiNodes` VALUES (16,0,-1240.53,-2515.11,22.16,"Refuge Pointe, Arathi","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (17,0,-916.29,-3496.89,70.45,"Hammerfall, Arathi","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (18,0,-14444.29,509.62,26.2,"Booty Bay, Stranglethorn","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (19,0,-14473.05,464.15,36.43,"Booty Bay, Stranglethorn","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (20,0,-12414.18,146.29,3.28,"Grom\'gol, Stranglethorn","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (21,0,-6633.99,-2180.05,244.14,"Kargath, Badlands","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (22,1,-1197.21,29.71,176.95,"Thunder Bluff, Mulgore","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (23,1,1677.59,-4315.71,61.17,"Orgrimmar, Durotar","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (24,0,-6666,-2222.3,278.6,"Generic, World target for Zeppelin Paths","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (25,1,-441.8,-2596.08,96.06,"Crossroads, The Barrens","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (26,1,6341.38,557.68,16.29,"Auberdine, Darkshore","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (27,1,8643.59,841.05,23.3,"Rut\'theran Village, Teldrassil","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (28,1,2827.34,-289.24,107.16,"Astranaar, Ashenvale","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (29,1,966.57,1040.32,104.27,"Sun Rock Retreat, Stonetalon Mountains","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (30,1,-5407.71,-2414.3,90.32,"Freewind Post, Thousand Needles","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (31,1,-4491.88,-775.89,-39.52,"Thalanaar, Feralas","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (32,1,-3825.37,-4516.58,10.44,"Theramore, Dustwallow Marsh","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (33,1,2681.13,1461.68,232.88,"Stonetalon Peak, Stonetalon Mountains","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (34,1,-1965.17,-5824.29,-1.06,"Transport, Booty Bay - Ratchet","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (35,1,1320.07,-4649.2,21.57,"Transport, Orgrimmar Zepplins","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (36,0,-8644.62,433.28,59.46,"Generic, World target","0","0","0","0","0","0","0",4128894,15665,0);
INSERT INTO `dbc_TaxiNodes` VALUES (37,1,139.24,1325.82,193.5,"Nijel\'s Point, Desolace","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (38,1,-1767.64,3263.89,4.94,"Shadowprey Village, Desolace","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (39,1,-7223.97,-3734.59,8.39,"Gadgetzan, Tanaris","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (40,1,-7048.89,-3780.36,10.19,"Gadgetzan, Tanaris","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (41,1,-4373.8,3338.65,12.27,"Feathermoon, Feralas","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (42,1,-4419.86,199.31,25.06,"Camp Mojache, Feralas","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (43,0,283.74,-2002.76,194.74,"Aerie Peak, The Hinterlands","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (44,1,3661.52,-4390.38,113.05,"Valormok, Azshara","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (45,0,-11112.25,-3435.74,79.09,"Nethergarde Keep, Blasted Lands","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (46,0,-986.43,-547.86,-3.86,"Southshore Ferry, Hillsbrad","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (47,0,-12418.77,235.43,1.12,"Transport, Grom\'gol - Orgrimmar","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (48,1,5068.4,-337.22,367.41,"Bloodvenom Post, Felwood","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (49,1,7458.45,-2487.21,462.33,"Moonglade","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (50,0,0,0,0,"Transport, Menethil Ships","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (51,0,0,0,0,"Transport, Rut\'theran - Auberdine","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (52,1,6799.24,-4742.44,701.5,"Everlook, Winterspring","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (53,1,6813.06,-4611.12,710.67,"Everlook, Winterspring","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (54,1,-4203.87,3284,-12.86,"Transport, Feathermoon - Feralas","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (55,1,-3147.39,-2842.18,34.61,"Brackenwall Village, Dustwallow Marsh","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (56,0,-10456.97,-3279.25,21.35,"Stonard, Swamp of Sorrows","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (57,1,8701.51,991.37,14.21,"Fishing Village, Teldrassil","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (58,1,3374.71,996.97,5.19,"Zoram\'gar Outpost, Ashenvale","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (59,30,574.21,-46.65,37.61,"Dun Baldar, Alterac Valley","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (60,30,-1335.44,-319.69,90.66,"Frostwolf Keep, Alterac Valley","0","0","0","0","0","0","0",4128894,3574,0);
INSERT INTO `dbc_TaxiNodes` VALUES (61,1,2302.39,-2524.55,104.4,"Splintertree Post, Ashenvale","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (62,1,7793.61,-2403.47,489.32,"Nighthaven, Moonglade","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (63,1,7787.72,-2404.1,489.56,"Nighthaven, Moonglade","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (64,1,2721.99,-3880.64,100.87,"Talrendis Point, Azshara","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (65,1,6205.88,-1949.63,571.29,"Talonbranch Glade, Felwood","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (66,0,931.32,-1430.11,64.67,"Chillwind Camp, Western Plaguelands","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (67,0,2271.09,-5340.8,87.11,"Light\'s Hope Chapel, Eastern Plaguelands","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (68,0,2327.41,-5286.89,81.78,"Light\'s Hope Chapel, Eastern Plaguelands","0","0","0","0","0","0","0",4128894,3574,0);
INSERT INTO `dbc_TaxiNodes` VALUES (69,1,7470.39,-2123.38,492.34,"Moonglade","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (70,0,-7504.03,-2187.54,165.53,"Flame Crest, Burning Steppes","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (71,0,-8364.61,-2738.35,185.46,"Morgan\'s Vigil, Burning Steppes","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (72,1,-6811.39,836.74,49.81,"Cenarion Hold, Silithus","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (73,1,-6761.83,772.03,88.91,"Cenarion Hold, Silithus","0","0","0","0","0","0","0",4128894,0,3837);
INSERT INTO `dbc_TaxiNodes` VALUES (74,0,-6552.59,-1168.27,309.31,"Thorium Point, Searing Gorge","0","0","0","0","0","0","0",4128894,0,541);
INSERT INTO `dbc_TaxiNodes` VALUES (75,0,-6554.93,-1100.05,309.57,"Thorium Point, Searing Gorge","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (76,0,-635.26,-4720.5,5.38,"Revantusk Village, The Hinterlands","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (77,1,-2380.67,-1882.67,95.85,"Camp Taurajo, The Barrens","0","0","0","0","0","0","0",4128894,2224,0);
INSERT INTO `dbc_TaxiNodes` VALUES (78,0,3133.31,-3399.93,139.53,"Naxxramas","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (79,1,-6113.82,-1142.7,-187.63,"Marshal\'s Refuge, Un\'Goro Crater","0","0","0","0","0","0","0",4128894,2224,541);
INSERT INTO `dbc_TaxiNodes` VALUES (80,1,-894.59,-3773.01,11.48,"Ratchet, The Barrens","0","0","0","0","0","0","0",4128894,2224,541);
INSERT INTO `dbc_TaxiNodes` VALUES (81,131074,0,0,0,"Filming","0","0","0","0","0","0","0",4128894,0,0);
INSERT INTO `dbc_TaxiNodes` VALUES (84,0,2998.71,-3050.1,117.19,"Plaguewood Tower, Eastern Plaguelands","0","0","0","0","0","0","0",4128894,17660,17660);
INSERT INTO `dbc_TaxiNodes` VALUES (85,0,3109.31,-4285.13,109.45,"Northpass Tower, Eastern Plaguelands","0","0","0","0","0","0","0",4128894,3574,541);
INSERT INTO `dbc_TaxiNodes` VALUES (86,0,2499.23,-4742.85,93.5,"Eastwall Tower, Eastern Plaguelands","0","0","0","0","0","0","0",4128894,3574,541);
INSERT INTO `dbc_TaxiNodes` VALUES (87,0,1857.56,-3658.47,143.73,"Crown Guard Tower, Eastern Plaguelands","0","0","0","0","0","0","0",4128894,3574,541);
