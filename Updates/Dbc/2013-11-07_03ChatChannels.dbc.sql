DROP TABLE IF EXISTS `dbc_ChatChannels`;
CREATE TABLE `dbc_ChatChannels` (
	`ID` INT NOT NULL DEFAULT '0',
	`flags` BIGINT NOT NULL DEFAULT '0',
	`factionGroup` TEXT NOT NULL,
	`Name` TEXT NOT NULL,
	`Name1` TEXT NOT NULL,
	`Name2` TEXT NOT NULL,
	`Name3` TEXT NOT NULL,
	`Name4` TEXT NOT NULL,
	`Name5` TEXT NOT NULL,
	`Name6` TEXT NOT NULL,
	`Name7` TEXT NOT NULL,
	`col11` BIGINT NOT NULL DEFAULT '0',
	`ShortcutName` TEXT NOT NULL,
	`ShortcutName1` TEXT NOT NULL,
	`ShortcutName2` TEXT NOT NULL,
	`ShortcutName3` TEXT NOT NULL,
	`ShortcutName4` TEXT NOT NULL,
	`ShortcutName5` TEXT NOT NULL,
	`ShortcutName6` TEXT NOT NULL,
	`ShortcutName7` TEXT NOT NULL,
	`col20` BIGINT NOT NULL DEFAULT '0')
 ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci COMMENT='Export of ChatChannels';
 SET NAMES UTF8;

INSERT INTO `dbc_ChatChannels` VALUES (1,3,"0","General - \%s","0","0","0","0","0","0","0",4128894,"General","0","0","0","0","0","0","0",4128894);
INSERT INTO `dbc_ChatChannels` VALUES (2,59,"0","Trade - \%s","0","0","0","0","0","0","0",4128894,"Trade","0","0","0","0","0","0","0",4128894);
INSERT INTO `dbc_ChatChannels` VALUES (22,65539,"0","LocalDefense - \%s","0","0","0","0","0","0","0",4128894,"LocalDefense","0","0","0","0","0","0","0",4128894);
INSERT INTO `dbc_ChatChannels` VALUES (23,65540,"0","WorldDefense","0","0","0","0","0","0","0",4128894,"WorldDefense","0","0","0","0","0","0","0",4128894);
INSERT INTO `dbc_ChatChannels` VALUES (24,0,"0","LookingForGroup","0","0","0","0","0","0","0",4128894,"LookingForGroup","0","0","0","0","0","0","0",4128894);
INSERT INTO `dbc_ChatChannels` VALUES (25,131122,"0","GuildRecruitment - \%s","0","0","0","0","0","0","0",4128894,"GuildRecruitment","0","0","0","0","0","0","0",4128894);
