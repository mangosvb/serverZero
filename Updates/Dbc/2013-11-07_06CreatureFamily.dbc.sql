DROP TABLE IF EXISTS `dbc_CreatureFamily`;
CREATE TABLE `dbc_CreatureFamily` (
	`ID` INT NOT NULL DEFAULT '0',
	`minScale` TEXT NOT NULL,
	`minScaleLevel` TEXT NOT NULL,
	`maxScale` TEXT NOT NULL,
	`maxScaleLevel` INT NOT NULL DEFAULT '0',
	`petFoodMask` INT NOT NULL DEFAULT '0',
	`SkillLine2` INT NOT NULL DEFAULT '0',
	`petTalentType` INT NOT NULL DEFAULT '0',
	`categoryEnumID` TEXT NOT NULL,
	`Name` TEXT NOT NULL,
	`Name1` TEXT NOT NULL,
	`Name2` TEXT NOT NULL,
	`Name3` TEXT NOT NULL,
	`Name4` TEXT NOT NULL,
	`Name5` TEXT NOT NULL,
	`Name6` TEXT NOT NULL,
	`Name7` BIGINT NOT NULL DEFAULT '0',
	`inconFile` TEXT NOT NULL)
 ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci COMMENT='Export of CreatureFamily';
 SET NAMES UTF8;

INSERT INTO `dbc_CreatureFamily` VALUES (1,"Wolf","Wolf","Wolf",60,208,270,1,"Wolf","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Wolf");
INSERT INTO `dbc_CreatureFamily` VALUES (2,"Wolf","Wolf","Wolf",60,209,270,3,"Cat","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Cat");
INSERT INTO `dbc_CreatureFamily` VALUES (3,"","Wolf","Wolf",60,203,270,1,"Spider","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Spider");
INSERT INTO `dbc_CreatureFamily` VALUES (4,"Wolf","Wolf","Wolf",60,210,270,63,"Bear","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Bear");
INSERT INTO `dbc_CreatureFamily` VALUES (5,"Wolf","Wolf","Wolf",60,211,270,63,"Boar","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Boar");
INSERT INTO `dbc_CreatureFamily` VALUES (6,"","Wolf","Wolf",60,212,270,3,"Crocolisk","0","0","0","0","0","0","0",4128894,"Interface\\Icons\\Ability\_Hunter\_Pet\_Crocolisk");
INSERT INTO `dbc_CreatureFamily` VALUES (7,"","Wolf","Wolf",60,213,270,3,"Carrion Bird","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Vulture");
INSERT INTO `dbc_CreatureFamily` VALUES (8,"Wolf","Wolf","Wolf",60,214,270,58,"Crab","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Crab");
INSERT INTO `dbc_CreatureFamily` VALUES (9,"Wolf","Wolf","Wolf",60,215,270,48,"Gorilla","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Gorilla");
INSERT INTO `dbc_CreatureFamily` VALUES (11,"","Wolf","Wolf",60,217,270,1,"Raptor","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Raptor");
INSERT INTO `dbc_CreatureFamily` VALUES (12,"","Wolf","Wolf",60,218,270,52,"Tallstrider","0","0","0","0","0","0","0",4128894,"Interface\\Icons\\Ability\_Hunter\_Pet\_TallStrider");
INSERT INTO `dbc_CreatureFamily` VALUES (15,"Wolf","Wolf","Wolf",60,189,0,0,"Felhunter","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Druid\_CatForm");
INSERT INTO `dbc_CreatureFamily` VALUES (16,"Wolf","Wolf","Wolf",60,204,0,0,"Voidwalker","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Druid\_CatForm");
INSERT INTO `dbc_CreatureFamily` VALUES (17,"Wolf","Wolf","Wolf",60,205,0,0,"Succubus","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Druid\_CatForm");
INSERT INTO `dbc_CreatureFamily` VALUES (19,"Wolf","Wolf","Wolf",60,207,0,0,"Doomguard","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Druid\_CatForm");
INSERT INTO `dbc_CreatureFamily` VALUES (20,"Wolf","Wolf","Wolf",60,236,270,1,"Scorpid","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Scorpid");
INSERT INTO `dbc_CreatureFamily` VALUES (21,"","Wolf","Wolf",60,251,270,178,"Turtle","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Hunter\_Pet\_Turtle");
INSERT INTO `dbc_CreatureFamily` VALUES (23,"","Wolf","",60,188,270,0,"Imp","0","0","0","0","0","0","0",8323198,"Interface\\Icons\\Ability\_Druid\_CatForm");
INSERT INTO `dbc_CreatureFamily` VALUES (24,"","Wolf","Wolf",60,653,270,48,"Bat","0","0","0","0","0","0","0",4128894,"Interface\\Icons\\Ability\_Hunter\_Pet\_Bat");
INSERT INTO `dbc_CreatureFamily` VALUES (25,"Wolf","Wolf","Wolf",60,654,270,33,"Hyena","0","0","0","0","0","0","0",4128894,"Interface\\Icons\\Ability\_Hunter\_Pet\_Hyena");
INSERT INTO `dbc_CreatureFamily` VALUES (26,"","Wolf","Wolf",60,655,270,1,"Owl","0","0","0","0","0","0","0",4128894,"Interface\\Icons\\Ability\_Hunter\_Pet\_Owl");
INSERT INTO `dbc_CreatureFamily` VALUES (27,"","Wolf","Wolf",60,656,270,14,"Wind Serpent","0","0","0","0","0","0","0",4128894,"Interface\\Icons\\Ability\_Hunter\_Pet\_WindSerpent");
INSERT INTO `dbc_CreatureFamily` VALUES (28,"","","",0,758,0,0,"Remote Control","0","0","0","0","0","0","0",4128894,"");
