-- Update Default Value for item_guid
alter table characters_mail
	change column item_guid item_guid bigint(20) not null default '0' after mail_read;

-- Create Mail Items Table
drop table if exists mail_items;
create table mail_items (
	mail_id int(11) null default null,
	item_guid int(11) null default null
)
comment='Mail System' 
collate='latin1_swedish_ci' 
engine=MyISAM
;
