DROP TABLE IF EXISTS `dbc_TalentTab`;
CREATE TABLE `dbc_TalentTab` (
	`ID` INT NOT NULL DEFAULT '0',
	`Name` TEXT NOT NULL,
	`Name1` TEXT NOT NULL,
	`Name2` TEXT NOT NULL,
	`Name3` TEXT NOT NULL,
	`Name4` TEXT NOT NULL,
	`Name5` TEXT NOT NULL,
	`Name6` TEXT NOT NULL,
	`Name7` TEXT NOT NULL,
	`col9` BIGINT NOT NULL DEFAULT '0',
	`SpellIconID` INT NOT NULL DEFAULT '0',
	`raceMark` INT NOT NULL DEFAULT '0',
	`classMask` INT NOT NULL DEFAULT '0',
	`orderIndex` INT NOT NULL DEFAULT '0',
	`backgroundFile` TEXT NOT NULL)
 ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci COMMENT='Export of TalentTab';
 SET NAMES UTF8;

INSERT INTO `dbc_TalentTab` VALUES (261,"Elemental","0","0","0","0","0","0","0",4128894,1137,511,64,0,"ShamanElementalCombat");
INSERT INTO `dbc_TalentTab` VALUES (283,"Balance","0","0","0","0","0","0","0",4128894,62,511,1024,0,"DruidBalance");
INSERT INTO `dbc_TalentTab` VALUES (382,"Holy","0","0","0","0","0","0","0",4128894,70,511,2,0,"PaladinHoly");
INSERT INTO `dbc_TalentTab` VALUES (81,"Arcane","0","0","0","0","0","0","0",4128894,122,511,128,0,"MageArcane");
INSERT INTO `dbc_TalentTab` VALUES (182,"Assassination","0","0","0","0","0","0","0",4128894,498,511,8,0,"RogueAssassination");
INSERT INTO `dbc_TalentTab` VALUES (41,"Fire","0","0","0","0","0","0","0",4128894,11,511,128,0,"MageFire");
INSERT INTO `dbc_TalentTab` VALUES (201,"Discipline","0","0","0","0","0","0","0",4128894,555,511,16,0,"PriestDiscipline");
INSERT INTO `dbc_TalentTab` VALUES (161,"Arms","0","0","0","0","0","0","0",4128894,1462,511,1,0,"WarriorArms");
INSERT INTO `dbc_TalentTab` VALUES (302,"Affliction","0","0","0","0","0","0","0",4128894,150,511,256,0,"WarlockCurses");
INSERT INTO `dbc_TalentTab` VALUES (361,"Beast Mastery","0","0","0","0","0","0","0",4128894,255,511,4,0,"HunterBeastMastery");
INSERT INTO `dbc_TalentTab` VALUES (164,"Fury","0","0","0","0","0","0","0",4128894,456,511,1,1,"WarriorFury");
INSERT INTO `dbc_TalentTab` VALUES (263,"Enhancement","0","0","0","0","0","0","0",4128894,312,511,64,1,"ShamanEnhancement");
INSERT INTO `dbc_TalentTab` VALUES (181,"Combat","0","0","0","0","0","0","0",4128894,1501,511,8,1,"RogueCombat");
INSERT INTO `dbc_TalentTab` VALUES (383,"Protection","0","0","0","0","0","0","0",4128894,291,511,2,1,"PaladinProtection");
INSERT INTO `dbc_TalentTab` VALUES (363,"Marksmanship","0","0","0","0","0","0","0",4128894,126,511,4,1,"HunterMarksmanship");
INSERT INTO `dbc_TalentTab` VALUES (202,"Holy","0","0","0","0","0","0","0",4128894,79,511,16,1,"PriestHoly");
INSERT INTO `dbc_TalentTab` VALUES (303,"Demonology","0","0","0","0","0","0","0",4128894,692,511,256,1,"WarlockSummoning");
INSERT INTO `dbc_TalentTab` VALUES (281,"Feral Combat","0","0","0","0","0","0","0",4128894,201,511,1024,1,"DruidFeralCombat");
INSERT INTO `dbc_TalentTab` VALUES (163,"Protection","0","0","0","0","0","0","0",4128894,1463,511,1,2,"WarriorProtection");
INSERT INTO `dbc_TalentTab` VALUES (262,"Restoration","0","0","0","0","0","0","0",4128894,963,511,64,2,"ShamanRestoration");
INSERT INTO `dbc_TalentTab` VALUES (381,"Retribution","0","0","0","0","0","0","0",4128894,555,511,2,2,"PaladinCombat");
INSERT INTO `dbc_TalentTab` VALUES (203,"Shadow","0","0","0","0","0","0","0",4128894,98,511,16,2,"PriestShadow");
INSERT INTO `dbc_TalentTab` VALUES (362,"Survival","0","0","0","0","0","0","0",4128894,257,511,4,2,"HunterSurvival");
INSERT INTO `dbc_TalentTab` VALUES (61,"Frost","0","0","0","0","0","0","0",4128894,56,511,128,2,"MageFrost");
INSERT INTO `dbc_TalentTab` VALUES (183,"Subtlety","0","0","0","0","0","0","0",4128894,103,511,8,2,"RogueSubtlety");
INSERT INTO `dbc_TalentTab` VALUES (301,"Destruction","0","0","0","0","0","0","0",4128894,937,511,256,2,"WarlockDestruction");
INSERT INTO `dbc_TalentTab` VALUES (282,"Restoration","0","0","0","0","0","0","0",4128894,962,511,1024,2,"DruidRestoration");
