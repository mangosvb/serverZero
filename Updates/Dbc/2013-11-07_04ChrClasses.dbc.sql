DROP TABLE IF EXISTS `dbc_ChrClasses`;
CREATE TABLE `dbc_ChrClasses` (
	`ID` INT NOT NULL DEFAULT '0',
	`col1` TEXT NOT NULL,
	`col2` TEXT NOT NULL,
	`powerType` INT NOT NULL DEFAULT '0',
	`petNameToken` TEXT NOT NULL,
	`Name` TEXT NOT NULL,
	`Name1` TEXT NOT NULL,
	`Name2` TEXT NOT NULL,
	`Name3` TEXT NOT NULL,
	`Name4` TEXT NOT NULL,
	`Name5` TEXT NOT NULL,
	`Name6` TEXT NOT NULL,
	`Name7` TEXT NOT NULL,
	`col13` BIGINT NOT NULL DEFAULT '0',
	`fileName` TEXT NOT NULL,
	`col15` INT NOT NULL DEFAULT '0',
	`col16` TEXT NOT NULL)
 ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci COMMENT='Export of ChrClasses';
 SET NAMES UTF8;

INSERT INTO `dbc_ChrClasses` VALUES (1,"PET","",1,"PET","Warrior","0","0","0","0","0","0","0",8323199,"WARRIOR",4,"");
INSERT INTO `dbc_ChrClasses` VALUES (2,"PET","",0,"PET","Paladin","0","0","0","0","0","0","0",8323199,"PALADIN",10,"PET");
INSERT INTO `dbc_ChrClasses` VALUES (3,"PET","PET",0,"PET","Hunter","0","0","0","0","0","0","0",8323199,"HUNTER",9,"");
INSERT INTO `dbc_ChrClasses` VALUES (4,"PET","PET",3,"PET","Rogue","0","0","0","0","0","0","0",8323199,"ROGUE",8,"");
INSERT INTO `dbc_ChrClasses` VALUES (5,"PET","",0,"PET","Priest","0","0","0","0","0","0","0",8323199,"PRIEST",6,"");
INSERT INTO `dbc_ChrClasses` VALUES (7,"PET","",0,"PET","Shaman","0","0","0","0","0","0","0",8323199,"SHAMAN",11,"PET");
INSERT INTO `dbc_ChrClasses` VALUES (8,"PET","",0,"PET","Mage","0","0","0","0","0","0","0",8323199,"MAGE",3,"");
INSERT INTO `dbc_ChrClasses` VALUES (9,"PET","",0,"DEMON","Warlock","0","0","0","0","0","0","0",8323199,"WARLOCK",5,"");
INSERT INTO `dbc_ChrClasses` VALUES (11,"PET","",0,"PET","Druid","0","0","0","0","0","0","0",8323199,"DRUID",7,"PET");
