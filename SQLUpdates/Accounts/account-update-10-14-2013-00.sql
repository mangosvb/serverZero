DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
	`account_id` INT(11) NOT NULL AUTO_INCREMENT COMMENT 'Account Idenifer',
	`username` VARCHAR(32) NOT NULL DEFAULT '',
	`sha_pass_hash` VARCHAR(40) NOT NULL DEFAULT '',
	`plevel` MEDIUMINT(3) UNSIGNED NOT NULL DEFAULT '0',
	`email` VARCHAR(50) NOT NULL DEFAULT '',
	`joindate` VARCHAR(10) NOT NULL DEFAULT '00-00-0000',
	`last_sshash` VARCHAR(90) NOT NULL DEFAULT '',
	`last_ip` VARCHAR(15) NOT NULL DEFAULT '',
	`last_login` VARCHAR(100) NOT NULL DEFAULT '0000-00-00',
	`mutetime` BIGINT(40) NULL DEFAULT '0',
	`locked` TINYINT(3) NULL DEFAULT '0',
	`banned` TINYINT(1) UNSIGNED NOT NULL DEFAULT '0',
	PRIMARY KEY (`account_id`),
	INDEX `username` (`username`),
	INDEX `plevel` (`plevel`)
)
COLLATE='utf8_general_ci'
ENGINE=MyISAM