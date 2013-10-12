DROP TABLE IF EXISTS `realms`;
CREATE TABLE `realmlist` (
	`id` TINYINT(3) UNSIGNED NOT NULL AUTO_INCREMENT,
	`name` VARCHAR(50) NOT NULL DEFAULT 'MangosVB Development',
	`address` VARCHAR(50) NOT NULL DEFAULT '127.0.0.1' COMMENT 'Realm Host IP Address',
	`port` INT(5) NOT NULL DEFAULT '8085',
	`flags` TINYINT(3) UNSIGNED NOT NULL DEFAULT '2' COMMENT 'Flags - 0 = online, 1 = invaild, 2 = offline',
	`icon` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
	`population` FLOAT(3,0) UNSIGNED NOT NULL DEFAULT '0' COMMENT 'Total Population on the realm',
	`timezone` TINYINT(3) NOT NULL DEFAULT '1',
	`security` TINYINT(1) NOT NULL DEFAULT '0' COMMENT 'GM Security Access',
	PRIMARY KEY (`id`)
)
COLLATE='utf8_general_ci'
ENGINE=MyISAM;
