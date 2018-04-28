
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