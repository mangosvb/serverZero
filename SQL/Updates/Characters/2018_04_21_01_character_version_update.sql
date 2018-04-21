-- Adding Character DB Version Table
drop table if exists character_db_version;
create table character_db_version (
	required_2018_04_21_01_version_updates bit(1) null default null
)
comment='Last applied sql update to DB'
collate='utf8_general_ci'
engine=MyISAM
row_format=FIXED
;
