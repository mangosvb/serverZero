-- -----------------------------------
-- Add missing fields to table account
-- -----------------------------------
ALTER TABLE account
  ADD COLUMN `os` VARCHAR(3) DEFAULT '' COMMENT 'Client OS Version' AFTER `locale`;

ALTER TABLE account
  ADD COLUMN `playerBot` BIT(1) NOT NULL DEFAULT b'0' COMMENT 'Whether the account is a playerbot account' AFTER `os`;

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- -----------------------------------
-- Create New db_Version table
-- -----------------------------------
DROP TABLE IF EXISTS `db_version`;

CREATE TABLE `db_version` (
  `version` INT(3) NOT NULL,
  `structure` INT(3) NOT NULL,
  `content` INT(3) NOT NULL,
  `description` VARCHAR(30) NOT NULL DEFAULT '',
  `comment` VARCHAR(150) DEFAULT '',
  PRIMARY KEY (`version`,`structure`,`content`)
) ENGINE=INNODB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT COMMENT='Used DB version notes';

/*Data for the table `db_version` */

LOCK TABLES `db_version` WRITE;

INSERT  INTO `db_version`(`version`,`structure`,`content`,`description`,`comment`) VALUES 
(1,0,0,'Initial Release','Initial Release for Rel1_00_000');

UNLOCK TABLES;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- -----------------------------------
-- Create New warden_log table
-- -----------------------------------
CREATE TABLE `warden_log` (
  `entry` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Log entry ID',
  `check` SMALLINT(5) UNSIGNED NOT NULL COMMENT 'Failed Warden check ID',
  `action` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0' COMMENT 'Action taken (enum WardenActions)',
  `account` INT(11) UNSIGNED NOT NULL COMMENT 'Account ID',
  `guid` INT(11) UNSIGNED NOT NULL DEFAULT '0' COMMENT 'Player GUID',
  `map` INT(11) UNSIGNED DEFAULT NULL COMMENT 'Map ID',
  `position_x` FLOAT DEFAULT NULL COMMENT 'Player position X',
  `position_y` FLOAT DEFAULT NULL COMMENT 'Player position Y',
  `position_z` FLOAT DEFAULT NULL COMMENT 'Player position Z',
  `date` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date of the log entry',
  PRIMARY KEY (`entry`)
) ENGINE=MYISAM DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='Warden log of failed checks';


-- -----------------------------------
-- Add missing fields to table account
-- -----------------------------------
ALTER TABLE realmlist
  ADD COLUMN `localAddress` VARCHAR(255) NOT NULL DEFAULT '127.0.0.1' AFTER `Address`;
ALTER TABLE realmlist
  ADD COLUMN `localSubnetMask` VARCHAR(255) NOT NULL DEFAULT '255.255.255.0' AFTER `localAddress`;
  
-- -----------------------------------
-- Remove old realmd_db_version table
-- -----------------------------------
DROP TABLE `realmd_db_version`;