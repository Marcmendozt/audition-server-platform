#----------------------------
# Table structure for baddefaultavatarlog
#----------------------------
drop table if exists baddefaultavatarlog;
CREATE TABLE `baddefaultavatarlog` (
  `Num` int(4) unsigned NOT NULL auto_increment,
  `UserSN` int(4) unsigned NOT NULL default '0',
  `AvatarCnt` int(4) unsigned NOT NULL default '0',
  PRIMARY KEY  (`Num`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table baddefaultavatarlog
#----------------------------

#----------------------------
# Table structure for dayusercount
#----------------------------
drop table if exists dayusercount;
CREATE TABLE `dayusercount` (
  `Time` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
  `ChannelNum` tinyint(3) unsigned default NULL,
  `UserCount` smallint(5) unsigned NOT NULL default '0',
  `ServerNum` smallint(2) unsigned NOT NULL default '0'
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table dayusercount
#----------------------------

#----------------------------
# Table structure for ddr_current
#----------------------------
drop table if exists ddr_current;
CREATE TABLE `ddr_current` (
  `AREA_ID` int(4) default '1',
  `SET_ID` int(4) default NULL,
  `NUM` int(4) default NULL,
  `DATE` datetime default NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table ddr_current
#----------------------------

#----------------------------
# Table structure for round
#----------------------------
drop table if exists `round`;
CREATE TABLE `round` (
  `RoundSN` int(11) NOT NULL auto_increment,
  `Mode` enum('S','T') NOT NULL default 'S',
  `JoinUserSize` tinyint(1) unsigned NOT NULL default '0',
  `EndTime` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
  PRIMARY KEY  (`RoundSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table round
#----------------------------

#----------------------------
# Table structure for userroundresult
#----------------------------
drop table if exists userroundresult;
CREATE TABLE `userroundresult` (
  `ResultSN` int(11) NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `RoundSN` int(11) NOT NULL default '0',
  `Rank` tinyint(1) unsigned NOT NULL default '0',
  `Exp` int(10) unsigned NOT NULL default '0',
  `GotExp` int(10) unsigned NOT NULL default '0',
  `BonusExp` int(10) unsigned NOT NULL default '0',
  `GotMoney` int(10) unsigned NOT NULL default '0',
  `EndTime` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`ResultSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table userroundresult
#----------------------------
