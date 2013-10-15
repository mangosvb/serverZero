RENAME TABLE `bans` TO `ip_banned`;

ALTER TABLE `accounts`
	CHANGE COLUMN `account_id` `id` INT(11) NOT NULL AUTO_INCREMENT COMMENT 'Account Idenifer' FIRST,
	CHANGE COLUMN `plevel` `gmlevel` MEDIUMINT(3) UNSIGNED NOT NULL DEFAULT '0' AFTER `sha_pass_hash`,
	CHANGE COLUMN `banned` `ip_banned` TINYINT(1) UNSIGNED NOT NULL DEFAULT '0' AFTER `locked`,
	ADD COLUMN `expansion` TINYINT(1) NOT NULL DEFAULT '0' AFTER `ip_banned`;
RENAME TABLE `accounts` TO `account`;

ALTER TABLE `account`
	ADD COLUMN `sessionkey` LONGTEXT NOT NULL AFTER `gmlevel`;

ALTER TABLE `realmlist`
	CHANGE COLUMN `flags` `realmflags` TINYINT(3) UNSIGNED NOT NULL DEFAULT '2' COMMENT 'Flags - 0 = online, 1 = invaild, 2 = offline' AFTER `port`;

ALTER TABLE `realmlist`
	CHANGE COLUMN `security` `allowedSecurityLevel` TINYINT(1) NOT NULL DEFAULT '0' COMMENT 'GM Security Access' AFTER `timezone`;

CREATE TABLE `account_banned` (
  `id` INT( 11 ) UNSIGNED NOT NULL COMMENT  'Account identifier',
  `bandate` bigint(40) NOT NULL default '0',
  `unbandate` bigint(40) NOT NULL default '0',
  `bannedby` varchar(50) NOT NULL,
  `banreason` varchar(255) NOT NULL,
  `active` tinyint(4) NOT NULL default '1',
  PRIMARY KEY  (`id`,`bandate`)
) ENGINE=MyISAM;

CREATE TABLE `realmcharacters` (
  `realmid` INT( 11 ) UNSIGNED NOT NULL COMMENT  'Account identifier',
  `acctid` INT( 11 ) UNSIGNED NOT NULL COMMENT  'Realm identifier',
  `numchars` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`realmid`,`acctid`),
  KEY (acctid)
) ENGINE=MyISAM;