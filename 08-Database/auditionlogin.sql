#----------------------------
# Table structure for logininfo
#----------------------------
drop table if exists logininfo;
CREATE TABLE `logininfo` (
  `UserSN` int(10) unsigned NOT NULL default '0',
  `ServerNum` mediumint(7) unsigned NOT NULL default '0',
  `Time` datetime NOT NULL default '0000-00-00 00:00:00',
  `Type` enum('LOGIN','LOGOUT') NOT NULL default 'LOGIN',
  `IP` varchar(16) NOT NULL default '000.000.000.000',
  KEY `IDX_ServerNum` (`ServerNum`),
  KEY `UserSN` (`UserSN`),
  KEY `Time` (`Time`),
  KEY `IP` (`IP`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table logininfo
#----------------------------

#----------------------------
# Table structure for member
#----------------------------
drop table if exists member;
CREATE TABLE `member` (
  `id` int(11) unsigned NOT NULL auto_increment,
  `userid` varchar(16) NOT NULL default '0',
  `usernick` varchar(20) NOT NULL default '0',
  `sex` int(1) NOT NULL default '0',
  `passwd` varchar(20) NOT NULL default '0',
  `registdate` datetime NOT NULL default '0000-00-00 00:00:00',
  `Id9you` int(11) NOT NULL default '0',
  `State` int(2) unsigned NOT NULL default '0',
  `PopCard` varchar(20) NOT NULL default '0',
  `UserName` varchar(20) NOT NULL default '0',
  PRIMARY KEY  (`id`),
  UNIQUE KEY `usernick` (`usernick`),
  UNIQUE KEY `Id9you` (`Id9you`),
  UNIQUE KEY `userid` (`userid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table member
#----------------------------
insert  into member values (1, 'CiMed', 'CiMed', 0, 'testau', '2006-10-10 23:00:00', 1, 1, '0', 'CiMed') ;

#----------------------------
# Table structure for notallowednicklist
#----------------------------
drop table if exists notallowednicklist;
CREATE TABLE `notallowednicklist` (
  `BanNick` varchar(16) NOT NULL default '',
  PRIMARY KEY  (`BanNick`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table notallowednicklist
#----------------------------

#----------------------------
# Table structure for version
#----------------------------
drop table if exists `version`;
CREATE TABLE `version` (
  `Version` varchar(50) NOT NULL default ''
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table version
#----------------------------
insert  into `version` values ('01.01.18.cn') ;
insert  into `version` values ('01.02.10.cn') ;
insert  into `version` values ('01.02.13.cn') ;
insert  into `version` values ('01.02.18.cn') ;
insert  into `version` values ('01.03.11.cn') ;
