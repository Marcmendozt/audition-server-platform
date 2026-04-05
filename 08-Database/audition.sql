#----------------------------
# Table structure for admininfo
#----------------------------
drop table if exists admininfo;
CREATE TABLE `admininfo` (
  `UserSN` int(10) unsigned NOT NULL auto_increment,
  `UserIP` varchar(16) NOT NULL default '',
  `UserID` varchar(22) NOT NULL default '',
  `Password` varchar(23) NOT NULL default '',
  `Power` tinyint(2) unsigned NOT NULL default '0',
  PRIMARY KEY  (`UserSN`),
  UNIQUE KEY `UserID` (`UserID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table admininfo
#----------------------------
insert  into admininfo values (1, '127.0.0.1', 'CiMed', 'testau', 127) ;

#----------------------------
# Table structure for adminlog
#----------------------------
drop table if exists adminlog;
CREATE TABLE `adminlog` (
  `LogSN` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `Time` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
  `Log` varchar(255) NOT NULL default '',
  PRIMARY KEY  (`LogSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table adminlog
#----------------------------

#----------------------------
# Table structure for avataritemlist
#----------------------------
drop table if exists avataritemlist;
CREATE TABLE `avataritemlist` (
  `UserSN` int(10) NOT NULL default '0',
  `EquipState` enum('INV','EQP','DEF') NOT NULL default 'DEF',
  `AvatarItem` smallint(6) NOT NULL default '0',
  `BuyNick` varchar(42) NOT NULL default '',
  `BuyDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `ExpireDate` datetime NOT NULL default '2080-01-01 00:00:00',
  `BuyType` enum('M','G','D') NOT NULL default 'D',
  UNIQUE KEY `UserSN` (`UserSN`,`AvatarItem`),
  KEY `ExpireDate` (`ExpireDate`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table avataritemlist
#----------------------------

#----------------------------
# Table structure for avatarlist
#----------------------------
drop table if exists avatarlist;
CREATE TABLE `avatarlist` (
  `ItemID` int(10) NOT NULL default '0',
  `ItemName` varchar(30) NOT NULL default '0',
  `ItemStyle` varchar(16) NOT NULL default '0',
  `Sex` tinyint(2) NOT NULL default '0',
  `ItemKind` varchar(2) NOT NULL default 'A',
  `Cash` int(4) NOT NULL default '0',
  `a` varchar(16) NOT NULL default '0',
  `b` varchar(16) NOT NULL default '0',
  `c` varchar(16) NOT NULL default '0',
  `d` varchar(16) NOT NULL default '0',
  PRIMARY KEY  (`ItemID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table avatarlist
#----------------------------
insert  into avatarlist values (0, '', '0', 1, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (1, '', '0', 1, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (2, '', '0', 1, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (3, '', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (4, '', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (5, '', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (6, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (7, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (8, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (9, '', '0', 1, 'A', 180, '0', '0', '0', '0') ;
insert  into avatarlist values (10, '', '0', 1, 'A', 180, '0', '0', '0', '0') ;
insert  into avatarlist values (11, '', '0', 1, 'A', 180, '0', '0', '0', '0') ;
insert  into avatarlist values (12, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (13, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (14, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (15, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (16, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (17, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (18, '', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (19, '', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (20, '', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (21, '', '0', 0, 'A', 180, '0', '0', '0', '0') ;
insert  into avatarlist values (22, '', '0', 0, 'A', 180, '0', '0', '0', '0') ;
insert  into avatarlist values (23, '', '0', 0, 'A', 180, '0', '0', '0', '0') ;
insert  into avatarlist values (24, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (25, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (26, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (27, '', '0', 1, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (28, '', '0', 1, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (29, '', '0', 1, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (30, 'è', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (31, 'è', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (32, 'è', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (33, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (34, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (35, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (36, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (37, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (38, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (39, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (40, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (41, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (42, '', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (43, '', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (44, '', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (45, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (46, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (47, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (48, 'Բ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (49, 'Բ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (50, 'Բ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (51, '', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (52, '', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (53, '', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (54, '˧', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (55, '˧', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (56, '˧', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (57, '37', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (58, '37', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (59, '37', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (60, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (61, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (62, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (63, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (64, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (65, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (66, '', '0', 1, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (67, '', '0', 1, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (68, '', '0', 1, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (69, '', '0', 1, 'A', 200, '0', '0', '0', '0') ;
insert  into avatarlist values (70, '', '0', 1, 'A', 200, '0', '0', '0', '0') ;
insert  into avatarlist values (71, '', '0', 1, 'A', 200, '0', '0', '0', '0') ;
insert  into avatarlist values (72, '', '0', 1, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (73, '', '0', 1, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (74, '', '0', 1, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (75, '', '0', 0, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (76, '', '0', 0, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (77, '', '0', 0, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (78, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (79, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (80, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (81, 'ǰ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (82, 'ǰ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (83, 'ǰ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (84, 'XiaoX', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (85, 'XiaoX', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (86, 'XiaoX', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (87, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (88, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (89, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (90, '', '0', 1, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (91, 'ǳ', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (92, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (93, '', '0', 1, 'A', 140, '0', '0', '0', '0') ;
insert  into avatarlist values (94, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (95, '', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (96, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (97, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (98, '', '0', 1, 'A', 290, '0', '0', '0', '0') ;
insert  into avatarlist values (99, '', '0', 1, 'A', 180, '0', '0', '0', '0') ;
insert  into avatarlist values (100, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (101, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (102, '87T', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (103, '', '0', 1, 'A', 140, '0', '0', '0', '0') ;
insert  into avatarlist values (104, 'NIKE', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (105, 'Ƥ', '0', 1, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (106, '', '0', 1, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (107, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (108, 'ESPRIT', '0', 0, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (109, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (110, 'ǳ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (111, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (112, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (113, 'GUCCI', '0', 0, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (114, '', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (115, 'U2', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (116, '', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (117, '', '0', 0, 'A', 200, '0', '0', '0', '0') ;
insert  into avatarlist values (118, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (119, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (120, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (121, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (122, 'MNG', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (123, 'U2ӡ', '0', 0, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (124, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (125, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (126, '', '0', 0, 'A', 130, '0', '0', '0', '0') ;
insert  into avatarlist values (127, 'һ', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (128, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (129, 'GUCCI', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (130, '', '0', 0, 'A', 520, '0', '0', '0', '0') ;
insert  into avatarlist values (131, '', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (132, 'ESPRIT', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (133, 'ǳ', '0', 0, 'A', 170, '0', '0', '0', '0') ;
insert  into avatarlist values (134, 'ESPRIT', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (135, '', '0', 0, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (136, 'ߡʽţ', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (137, '', '0', 1, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (138, '', '0', 1, 'A', 270, '0', '0', '0', '0') ;
insert  into avatarlist values (139, '', '0', 1, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (140, '', '0', 1, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (141, 'JOJO', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (142, '', '0', 1, 'A', 240, '0', '0', '0', '0') ;
insert  into avatarlist values (143, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (144, 'ǳ', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (145, 'HIP-HOP', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (146, 'ǳɫţ', '0', 1, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (147, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (148, '', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (149, '', '0', 1, 'A', 740, '0', '0', '0', '0') ;
insert  into avatarlist values (150, '', '0', 1, 'A', 150, '0', '0', '0', '0') ;
insert  into avatarlist values (151, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (152, 'NIKE', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (153, '', '0', 1, 'A', 590, '0', '0', '0', '0') ;
insert  into avatarlist values (154, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (155, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (156, 'U2ȹ', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (157, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (158, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (159, '', '0', 0, 'A', 330, '0', '0', '0', '0') ;
insert  into avatarlist values (160, 'ѵ', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (161, 'DIOR', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (162, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (163, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (164, '˫', '0', 0, 'A', 470, '0', '0', '0', '0') ;
insert  into avatarlist values (165, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (166, '', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (167, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (168, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (169, 'LEE', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (170, '', '0', 0, 'A', 240, '0', '0', '0', '0') ;
insert  into avatarlist values (171, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (172, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (173, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (174, '', '0', 0, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (175, 'MNG Ұ', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (176, 'Hip-Hop', '0', 0, 'A', 140, '0', '0', '0', '0') ;
insert  into avatarlist values (177, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (178, 'Hip-Hop', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (179, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (180, 'LEVI\'S', '0', 0, 'A', 1100, '0', '0', '0', '0') ;
insert  into avatarlist values (181, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (182, '', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (183, '', '0', 2, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (184, 'PUMA', '0', 2, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (185, 'Ƥ', '0', 2, 'A', 230, '0', '0', '0', '0') ;
insert  into avatarlist values (186, 'NIKE', '0', 2, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (187, '', '0', 2, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (188, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (189, '', '0', 2, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (190, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (191, '', '0', 0, 'A', 290, '0', '0', '0', '0') ;
insert  into avatarlist values (192, 'JOJO', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (193, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (194, '', '0', 1, 'A', 630, '0', '0', '0', '0') ;
insert  into avatarlist values (195, '', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (196, '', '0', 1, 'A', 140, '0', '0', '0', '0') ;
insert  into avatarlist values (197, 'ELAND У', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (198, 'Ů', '0', 0, 'A', 630, '0', '0', '0', '0') ;
insert  into avatarlist values (199, 'Ů', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (200, 'Ů', '0', 0, 'A', 140, '0', '0', '0', '0') ;
insert  into avatarlist values (201, 'ELAND', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (202, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (203, '', '0', 1, 'A', 160, '0', '0', '0', '0') ;
insert  into avatarlist values (204, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (205, 'ELAND', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (206, 'Ů', '0', 0, 'A', 630, '0', '0', '0', '0') ;
insert  into avatarlist values (207, 'Ů', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (208, 'Ů', '0', 0, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (209, 'NY', '0', 1, 'A', 330, '0', '0', '0', '0') ;
insert  into avatarlist values (210, 'NY', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (211, 'NY', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (212, 'NY', '0', 1, 'A', 250, '0', '0', '0', '0') ;
insert  into avatarlist values (213, 'NY', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (214, 'NY', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (215, 'NY', '0', 0, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (216, 'NY', '0', 0, 'A', 110, '0', '0', '0', '0') ;
insert  into avatarlist values (217, '', '0', 1, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (218, '', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (219, '', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (220, '', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (221, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (222, '', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (223, 'Levi\'s', '0', 1, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (224, 'Levi\'s', '0', 1, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (225, 'Levi\'s', '0', 1, 'A', 590, '0', '0', '0', '0') ;
insert  into avatarlist values (226, 'Versaceħ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (227, 'Versaceħ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (228, 'Jordon', '0', 1, 'A', 330, '0', '0', '0', '0') ;
insert  into avatarlist values (229, 'Jordon ӡ', '0', 1, 'A', 890, '0', '0', '0', '0') ;
insert  into avatarlist values (230, 'BaerTwo', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (231, 'BaerTwo', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (232, 'YOYO', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (233, 'Gucci', '0', 0, 'A', 1100, '0', '0', '0', '0') ;
insert  into avatarlist values (234, 'BellVolles', '0', 0, 'A', 790, '0', '0', '0', '0') ;
insert  into avatarlist values (235, 'BellVolles', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (236, 'BellVolles', '0', 0, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (237, 'ŮӶ', '0', 0, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (238, 'Valentino', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (239, 'Valentino', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (240, 'Valentino', '0', 1, 'A', 249, '0', '0', '0', '0') ;
insert  into avatarlist values (241, 'Valentino', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (242, 'C&K', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (243, 'C&K ѵ', '0', 0, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (244, 'C&K ѵ', '0', 0, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (245, 'C&K', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (246, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (247, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (248, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (249, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (250, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (251, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (252, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (253, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (254, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (255, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (256, 'ǳ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (257, 'ǳ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (258, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (259, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (260, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (261, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (262, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (263, '', '0', 1, 'A', 390, '0', '0', '0', '0') ;
insert  into avatarlist values (264, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (265, 'Gucci', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (266, 'Gucci', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (267, '', '0', 0, 'A', 790, '0', '0', '0', '0') ;
insert  into avatarlist values (268, 'CD', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (269, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (270, '', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (271, '', '0', 1, 'A', 1300, '0', '0', '0', '0') ;
insert  into avatarlist values (272, '', '0', 1, 'A', 890, '0', '0', '0', '0') ;
insert  into avatarlist values (273, 'Dior', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (274, 'Dior', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (275, 'Dior', '0', 0, 'A', 230, '0', '0', '0', '0') ;
insert  into avatarlist values (276, 'Dior', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (277, 'Dior', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (278, 'Dior', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (279, 'Dior ǳţ', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (280, 'Dior Ͱʽţ', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (281, 'Dior ëѥ', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (282, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (283, '', '0', 1, 'A', 130, '0', '0', '0', '0') ;
insert  into avatarlist values (284, '', '0', 1, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (285, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (286, '', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (287, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (288, '', '0', 0, 'A', 330, '0', '0', '0', '0') ;
insert  into avatarlist values (289, '', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (290, '', '0', 1, 'A', 670, '0', '0', '0', '0') ;
insert  into avatarlist values (291, '', '0', 1, 'A', 130, '0', '0', '0', '0') ;
insert  into avatarlist values (292, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (293, '', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (294, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (295, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (296, 'Levi\'s', '0', 1, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (297, 'Levi\'s', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (298, 'Lee', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (299, 'Lee', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (300, 'ESPRIT Բ', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (301, 'ESPRIT', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (302, 'Levi\'s Ů', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (303, 'Levi\'s', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (304, '', '0', 0, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (305, '', '0', 0, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (306, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (307, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (308, 'DIOR', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (309, 'ʨ', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (310, 'Benetton', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (311, 'Benetton', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (312, 'Benetton', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (313, '', '0', 0, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (314, 'Apple ţ', '0', 0, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (315, 'Apple', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (316, 'Belle', '0', 0, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (317, 'PUMA33', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (318, 'PUMA33', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (319, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (320, 'ONLY ʮ', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (321, 'ONLY Ϳѻţ', '0', 1, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (322, 'ONLY', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (323, '', '0', 0, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (324, 'ONLY', '0', 0, 'A', 670, '0', '0', '0', '0') ;
insert  into avatarlist values (325, 'ONLY ˫', '0', 0, 'A', 670, '0', '0', '0', '0') ;
insert  into avatarlist values (326, 'ONLY', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (327, 'ʥ', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (328, 'ʥ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (329, '', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (330, 'ET-boite', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (331, 'ET-boite ӡ', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (332, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (333, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (334, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (335, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (336, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (337, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (338, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (339, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (340, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (341, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (342, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (343, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (344, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (345, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (346, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (347, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (348, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (349, 'Parad', '0', 0, 'A', 3000, '0', '0', '0', '0') ;
insert  into avatarlist values (350, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (351, '', '0', 1, 'A', 330, '0', '0', '0', '0') ;
insert  into avatarlist values (352, 'U2', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (353, 'U2', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (354, 'MOC', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (355, '', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (356, 'YSL', '0', 0, 'A', 1500, '0', '0', '0', '0') ;
insert  into avatarlist values (357, 'YSL', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (358, 'YSL', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (359, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (360, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (361, '', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (362, 'adiads', '0', 1, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (363, 'adiads', '0', 1, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (364, 'Levi\'s', '0', 1, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (365, 'adiads', '0', 1, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (366, '', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (367, 'Weekend ţ', '0', 0, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (368, 'Azona', '0', 0, 'A', 1500, '0', '0', '0', '0') ;
insert  into avatarlist values (369, 'DBSK-1', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (370, 'DBSK-1', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (371, 'DBSK-1', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (372, 'DBSK-1', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (373, '', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (374, 'Jessica', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (375, 'Jessica', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (376, 'Converse', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (377, 'DBSK-2', '0', 1, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (378, 'DBSK-2', '0', 1, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (379, 'DBSK-2', '0', 1, 'A', 1100, '0', '0', '0', '0') ;
insert  into avatarlist values (380, 'DBSK-2', '0', 1, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (381, '', '0', 0, 'A', 810, '0', '0', '0', '0') ;
insert  into avatarlist values (382, '9YOU', '0', 0, 'A', 1300, '0', '0', '0', '0') ;
insert  into avatarlist values (383, '9YOU', '0', 0, 'A', 1300, '0', '0', '0', '0') ;
insert  into avatarlist values (384, '', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (385, 'Armani ʱ', '0', 1, 'A', 330, '0', '0', '0', '0') ;
insert  into avatarlist values (386, 'Armani', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (387, 'Armani', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (388, 'Armani', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (389, '', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (390, 'AVV', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (391, 'AVV', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (392, 'AVV', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (393, 'B-1', '0', 1, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (394, 'B-1', '0', 1, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (395, 'B-1', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (396, 'B-1', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (397, 'KENñ', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (398, 'KEN ͼ', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (399, 'KEN ӡ', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (400, 'KENƤЬ', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (401, 'Dior', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (402, 'Dior', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (403, 'Dior', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (404, 'Dior', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (405, 'DBSK-3', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (406, 'DBSK-3', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (407, 'DBSK-3', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (408, 'DBSK-3', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (409, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (410, 'Annasui ӡ', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (411, 'Annasui', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (412, 'JOJO ѣĿ', '0', 0, 'A', 230, '0', '0', '0', '0') ;
insert  into avatarlist values (413, 'JOJO ѣĿ', '0', 0, 'A', 230, '0', '0', '0', '0') ;
insert  into avatarlist values (414, 'DBSK-4', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (415, 'DBSK-4', '0', 1, 'A', 1500, '0', '0', '0', '0') ;
insert  into avatarlist values (416, 'DBSK-4', '0', 1, 'A', 910, '0', '0', '0', '0') ;
insert  into avatarlist values (417, 'DBSK-4', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (418, 'DBSK-G1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (419, 'DBSK-G1', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (420, 'DBSK-G1', '0', 0, 'A', 330, '0', '0', '0', '0') ;
insert  into avatarlist values (421, 'DBSK-G1', '0', 0, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (422, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (423, 'SIN-1', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (424, 'SIN-1 Χ', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (425, 'Armani', '0', 1, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (426, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (427, 'DBSK-5', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (428, 'DBSK-5', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (429, 'DBSK-5', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (430, 'DBSK-G2', '0', 0, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (431, 'DBSK-G2', '0', 0, 'A', 940, '0', '0', '0', '0') ;
insert  into avatarlist values (432, 'DBSK-G2', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (433, 'DBSK-G2', '0', 0, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (434, 'SoryLove', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (435, 'SoryLove', '0', 1, 'A', 1530, '0', '0', '0', '0') ;
insert  into avatarlist values (436, 'SoryLove', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (437, 'SoryLove', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (438, 'SoryLove Ů', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (439, 'SoryLove Ů', '0', 0, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (440, 'SoryLove Ů', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (441, 'SoryLove Ů', '0', 0, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (442, 'Sex Ů', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (443, 'Sex Χ', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (444, 'Sex', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (445, 'Sex', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (446, 'B-2', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (447, 'RAIN', '0', 1, 'A', 2500, '0', '0', '0', '0') ;
insert  into avatarlist values (448, 'RAIN', '0', 1, 'A', 2500, '0', '0', '0', '0') ;
insert  into avatarlist values (449, '', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (450, '', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (451, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (452, '', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (453, '', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (454, 'MAT-GIRL2', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (455, 'MAT-GIRL2', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (456, 'MAT-GIRL1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (457, 'MAT-GIRL1', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (458, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (459, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (460, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (461, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (462, 'MAT-MAN2', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (463, 'MAT-MAN2', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (464, 'MAT-GIRL1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (465, 'MAT-GIRL2', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (466, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (467, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (468, 'ESPRIT', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (469, 'ESPRIT', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (470, 'ESPRIT', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (471, 'MAT-MAN1', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (472, 'MAT-MAN1', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (473, '', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (474, '', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (475, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (476, '', '0', 0, 'A', 1500, '0', '0', '0', '0') ;
insert  into avatarlist values (477, '', '0', 0, 'A', 1500, '0', '0', '0', '0') ;
insert  into avatarlist values (478, 'LEE Ůʽ', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (479, 'LEE', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (480, '', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (481, 'ESPRIT', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (482, 'ESPRIT', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (483, 'ESPRIT', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (484, 'ESPRIT', '0', 1, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (485, '', '0', 1, 'A', 300, '0', '0', '0', '0') ;
insert  into avatarlist values (486, '', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (487, '', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (488, '', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (489, 'Didgeno б', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (490, '', '0', 0, 'A', 1500, '0', '0', '0', '0') ;
insert  into avatarlist values (491, 'õ', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (492, 'ONLY', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (493, 'ONLY ţ', '0', 0, 'A', 890, '0', '0', '0', '0') ;
insert  into avatarlist values (494, 'Meto', '0', 1, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (495, 'TAXAS', '0', 1, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (496, 'NIKE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (497, 'NIKE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (498, 'NIKE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (499, 'NIKE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (500, 'NIKE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (501, 'NIKE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (502, 'NIKE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (503, 'NIKE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (504, 'NIKE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (505, 'NIKE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (506, 'NIKE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (507, 'NIKE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (508, 'Armani ְҵ', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (509, 'Armani ְҵ', '0', 1, 'A', 630, '0', '0', '0', '0') ;
insert  into avatarlist values (510, 'Armani ְҵ', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (511, 'Armani', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (512, '', '0', 1, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (513, '', '0', 1, 'A', 810, '0', '0', '0', '0') ;
insert  into avatarlist values (514, '', '0', 1, 'A', 930, '0', '0', '0', '0') ;
insert  into avatarlist values (515, '', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (516, 'ELLE', '0', 0, 'A', 790, '0', '0', '0', '0') ;
insert  into avatarlist values (517, 'ELLE', '0', 0, 'A', 910, '0', '0', '0', '0') ;
insert  into avatarlist values (518, 'ELLE', '0', 0, 'A', 740, '0', '0', '0', '0') ;
insert  into avatarlist values (519, 'ELLE', '0', 0, 'A', 1100, '0', '0', '0', '0') ;
insert  into avatarlist values (520, '', '0', 0, 'A', 1100, '0', '0', '0', '0') ;
insert  into avatarlist values (521, '', '0', 0, 'A', 930, '0', '0', '0', '0') ;
insert  into avatarlist values (522, '', '0', 0, 'A', 830, '0', '0', '0', '0') ;
insert  into avatarlist values (523, 'Dior', '0', 0, 'A', 2500, '0', '0', '0', '0') ;
insert  into avatarlist values (524, 'Dior', '0', 0, 'A', 2500, '0', '0', '0', '0') ;
insert  into avatarlist values (525, 'ZENK', '0', 0, 'A', 830, '0', '0', '0', '0') ;
insert  into avatarlist values (526, 'ZENK', '0', 0, 'A', 1300, '0', '0', '0', '0') ;
insert  into avatarlist values (527, 'ZENK', '0', 0, 'A', 1400, '0', '0', '0', '0') ;
insert  into avatarlist values (528, 'ZENK', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (529, '', '0', 0, 'A', 740, '0', '0', '0', '0') ;
insert  into avatarlist values (530, 'D&G', '0', 0, 'A', 1100, '0', '0', '0', '0') ;
insert  into avatarlist values (531, 'D&G', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (532, 'BELLE', '0', 0, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (533, 'D&G', '0', 0, 'A', 970, '0', '0', '0', '0') ;
insert  into avatarlist values (534, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (535, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (536, 'DBSK-1', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (537, 'DBSK-1', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (538, 'DBSK-3', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (539, 'DBSK-3', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (540, 'DBSK-4', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (541, 'DBSK-4', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (542, 'SIN-1', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (543, 'SIN-1', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (544, 'SoryLove', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (545, 'SoryLove', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (546, '', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (547, '', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (548, 'SoryLove Ů', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (549, 'SoryLove Ů', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (550, 'Sexy Ů', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (551, 'Sexy Ů', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (552, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (553, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (554, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (555, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (556, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (557, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (558, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (559, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (560, '', '0', 1, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (561, 'Ů', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (562, 'ONLY ë', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (563, 'ONLY 31', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (564, 'BELLE', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (565, 'GUCCI', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (566, 'JACKJONES', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (567, 'JACKJONES', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (568, 'JACKJONES', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (569, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (570, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (571, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (572, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (573, '', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (574, '', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (575, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (576, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (577, 'B-1', '0', 1, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (578, 'B-1', '0', 1, 'A', 530, '0', '0', '0', '0') ;
insert  into avatarlist values (579, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (580, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (581, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (582, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (583, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (584, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (585, '', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (586, '', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (587, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (588, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (589, 'B-1', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (590, 'B-1', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (591, 'ESPRIT', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (592, 'ESPRIT', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (593, 'Ů', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (594, 'Ů', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (595, 'ONLY', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (596, 'ONLY', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (597, '9YOU', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (598, '9YOU', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (599, '', '0', 0, 'A', 1510, '0', '0', '0', '0') ;
insert  into avatarlist values (600, '', '0', 0, 'A', 1510, '0', '0', '0', '0') ;
insert  into avatarlist values (601, 'Ů', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (602, 'Ů', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (603, '9YOU', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (604, '9YOU', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (605, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (606, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (607, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (608, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (609, 'PUMA', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (610, 'PUMA', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (611, 'PUMA', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (612, 'PUMA', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (613, 'PUMA', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (614, 'PUMA', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (615, 'HYO-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (616, 'HYO-1', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (617, 'HYO-1', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (618, 'HYO-1', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (619, 'HYO-2', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (620, 'HYO-2', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (621, 'HYO-2', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (622, 'HYO-2', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (623, 'HYO-3 ī', '0', 0, 'A', 250, '0', '0', '0', '0') ;
insert  into avatarlist values (624, 'HYO-3 ͸', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (625, 'HYO-3', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (626, 'HYO-3', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (627, 'ER-1', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (628, 'ER-1', '0', 1, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (629, 'ER-1', '0', 1, 'A', 640, '0', '0', '0', '0') ;
insert  into avatarlist values (630, 'ER-1', '0', 1, 'A', 360, '0', '0', '0', '0') ;
insert  into avatarlist values (631, 'JEWEL-1', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (632, 'JEWEL-1', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (633, 'JEWEL-1', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (634, 'JEWEL-1', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (635, '', '0', 0, 'A', 640, '0', '0', '0', '0') ;
insert  into avatarlist values (636, 'YOYO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (637, 'HIP-HOP ñ', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (638, 'HIP-HOP ñ', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (639, 'HIP-HOP ñ', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (640, 'HIP-HOP', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (641, 'HIP-HOP', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (642, 'HIP-HOP', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (643, 'HIP-HOP', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (644, 'HIP-HOP', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (645, 'HIP-HOP', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (646, 'Sex', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (647, 'Sex', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (648, '', '0', 1, 'A', 810, '0', '0', '0', '0') ;
insert  into avatarlist values (649, 'ONLY', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (650, 'ONLY', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (651, 'ONLY', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (652, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (653, 'YOYO', '0', 0, 'A', 670, '0', '0', '0', '0') ;
insert  into avatarlist values (654, 'YOYO', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (655, 'YOYO', '0', 0, 'A', 440, '0', '0', '0', '0') ;
insert  into avatarlist values (656, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (657, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (658, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (659, 'adaids', '0', 1, 'A', 340, '0', '0', '0', '0') ;
insert  into avatarlist values (660, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (661, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (662, 'BBOA-1', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (663, 'BBOA-1', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (664, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (665, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (666, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (667, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (668, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (669, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (670, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (671, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (672, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (673, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (674, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (675, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (676, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (677, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (678, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (679, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (680, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (681, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (682, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (683, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (684, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (685, 'BBOA-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (686, 'BBOA-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (687, 'BBOA-1', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (688, 'б', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (689, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (690, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (691, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (692, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (693, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (694, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (695, 'ESPRIT', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (696, '', '0', 1, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (697, 'LAPERS', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (698, 'NIKE', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (699, 'NIKE', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (700, 'NIKE', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (701, 'JACKJONES', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (702, 'JACKJONES', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (703, 'JACKJONES', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (704, 'YSL', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (705, 'YSL', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (706, 'YSL', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (707, 'Levi\'s ţ', '0', 1, 'A', 1200, '0', '0', '0', '0') ;
insert  into avatarlist values (708, 'EAKORS T', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (709, 'ESPRIT', '0', 1, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (710, 'ESPRIT', '0', 1, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (711, 'ESPRIT ţ', '0', 1, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (712, 'ETAM ʨ', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (713, 'ETAM ӡ', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (714, 'Dior', '0', 0, 'A', 1380, '0', '0', '0', '0') ;
insert  into avatarlist values (715, 'Dior', '0', 0, 'A', 1380, '0', '0', '0', '0') ;
insert  into avatarlist values (716, 'adaids', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (717, 'adaids', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (718, 'adaids', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (719, 'BELLE', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (720, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (721, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (722, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (723, 'APPLE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (724, 'ELLE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (725, '', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (726, 'Levi\'s ţ', '0', 1, 'A', 1200, '0', '0', '0', '0') ;
insert  into avatarlist values (727, 'Levi\'s ţ', '0', 1, 'A', 1200, '0', '0', '0', '0') ;
insert  into avatarlist values (728, '', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (729, 'YSL', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (730, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (731, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (732, '', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (733, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (734, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (735, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (736, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (737, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (738, 'BELLE', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (739, 'X-', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (740, 'X-', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (741, 'X-', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (742, '', '0', 0, 'A', 820, '0', '0', '0', '0') ;
insert  into avatarlist values (743, 'U2 ʨ', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (744, 'U2', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (745, 'Reddteamrrr', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (746, 'Reddteamrrr T', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (747, 'Reddteamrrr', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (748, 'Reddteamrrr', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (749, '˫', '0', 1, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (750, 'Sex', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (751, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (752, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (753, '', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (754, 'ESPRIT', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (755, 'ESPRIT', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (756, 'ESPRIT', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (757, 'Dior', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (758, 'YOORA', '0', 0, 'A', 750, '0', '0', '0', '0') ;
insert  into avatarlist values (759, 'Xiao-X', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (760, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (761, 'U2', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (762, 'U2', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (763, 'U2', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (764, '', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (765, '', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (766, 'JACKJONES', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (767, 'JACKJONES', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (768, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (769, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (770, 'PUMA', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (771, 'Ryzme T-shirt', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (772, 'Ryzme T-shirt', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (773, '??? ?????', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (774, '????? ????', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (775, '????? ?????', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (776, '????? ??5?', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (777, '???? ??', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (778, '????? 5???', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (779, '???? ?????', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (780, 'E-LAND У԰', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (781, 'DJ', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (782, 'Eight', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (783, 'Eight', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (784, 'ɳ̲', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (785, '', '0', 2, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (786, 'E-LAND ¶', '0', 0, 'A', 660, '0', '0', '0', '0') ;
insert  into avatarlist values (787, 'M2N Ĩ', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (788, 'M2N Ĩ', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (789, '', '0', 0, 'A', 1510, '0', '0', '0', '0') ;
insert  into avatarlist values (790, '', '0', 0, 'A', 660, '0', '0', '0', '0') ;
insert  into avatarlist values (791, '', '0', 0, 'A', 660, '0', '0', '0', '0') ;
insert  into avatarlist values (792, '', '0', 0, 'A', 660, '0', '0', '0', '0') ;
insert  into avatarlist values (793, 'Xiao-X', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (794, 'Xiao-X', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (795, 'Zenki', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (796, 'Xiao-X', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (797, 'Xiao-X', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (798, '', '0', 0, 'A', 1510, '0', '0', '0', '0') ;
insert  into avatarlist values (799, '', '0', 0, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (800, 'LEE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (801, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (802, 'LEE', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (803, '', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (804, '???? ???', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (805, '??? ?????', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (806, '????? ???', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (807, '????? ????', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (808, '?????', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (809, '????? 7???', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (810, '2??? ?????', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (811, '???? ???', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (812, 'BOY', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (813, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (814, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (815, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (816, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (817, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (818, '', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (819, 'LEE', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (820, 'Dior', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (821, 'Dior', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (822, 'ESPRIT', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (823, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (824, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (825, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (826, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (827, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (828, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (829, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (830, 'С', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (831, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (832, 'Armani', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (833, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (834, 'U2', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (835, 'Armani', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (836, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (837, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (838, '', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (839, '', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (840, 'E-LAND', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (841, 'Dior ͸', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (842, 'Dior ͸', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (843, 'Dior', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (844, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (845, 'COSPLAY', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (846, 'COSPLAY', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (847, 'ESPRIT', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (848, 'Zenk', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (849, 'Zenk', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (850, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (851, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (852, '', '0', 1, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (853, '', '0', 1, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (854, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (855, '', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (856, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (857, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (858, 'Dior', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (859, 'Dior', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (860, 'Dior', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (861, 'Dior', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (862, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (863, 'ţ', '0', 0, 'A', 1200, '0', '0', '0', '0') ;
insert  into avatarlist values (864, 'YOYO', '0', 0, 'A', 1510, '0', '0', '0', '0') ;
insert  into avatarlist values (865, 'YOYO', '0', 0, 'A', 1510, '0', '0', '0', '0') ;
insert  into avatarlist values (866, 'ZENK', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (867, 'ZENK ͸', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (868, 'ZENK ͸', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (869, 'һ', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (870, 'һ', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (871, 'PHOO', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (872, 'Dior', '0', 0, 'A', 1510, '0', '0', '0', '0') ;
insert  into avatarlist values (873, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (874, 'ADAIDS', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (875, 'ADAIDS', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (876, 'ADAIDS', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (877, 'ADAIDS', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (878, 'CONYERSE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (879, 'CONYERSE LOGO', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (880, 'CONYERSE', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (881, 'CONYERSE', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (882, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (883, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (884, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (885, '', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (886, 'ELLE', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (887, 'ELLE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (888, 'ELLE', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (889, 'ELLE', '0', 0, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (890, 'AQUA', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (891, 'AQUA', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (892, 'AQUA', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (893, 'AQUA', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (894, 'JACKJONES', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (895, 'JACKJONES', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (896, 'JACKJONES', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (897, 'JACKJONES', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (898, 'JACKJONES', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (899, 'JACKJONES', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (900, '', '0', 1, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (901, '', '0', 1, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (902, 'GUCCI', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (903, 'YOORA', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (904, 'YOORA', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (905, 'YOORA', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (906, 'ETRO ¶', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (907, 'ETRO ¶', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (908, 'XIAO ʮ', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (909, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (910, 'DIOR ˿', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (911, 'DIOR ˿', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (912, 'ELAND', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (913, 'DIOR', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (914, 'COOL ⨺', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (915, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (916, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (917, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (918, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (919, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (920, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (921, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (922, 'XIAO', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (923, 'ETRO', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (924, 'ETRO', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (925, '9YOU', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (926, 'LINING', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (927, 'LINING', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (928, 'LINING', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (929, 'LINING', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (930, '9YOU', '0', 1, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (931, '9YOU', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (932, '9YOU', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (933, '9YOU', '0', 1, 'A', 280, '0', '0', '0', '0') ;
insert  into avatarlist values (934, 'AU', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (935, 'AU', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (936, 'AU', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (937, 'CLOTICS', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (938, 'CLOTICS', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (939, 'DIOR ɳ̲', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (940, 'DIOR ɳ̲', '0', 1, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (941, 'ETRO', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (942, 'ETRO', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (943, 'ETRO', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (944, 'ETRO', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (945, 'ETRO', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (946, 'ETRO', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (947, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (948, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (949, 'GUCCI', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (950, 'GUCCI', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (951, 'GUCCI', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (952, 'AU', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (953, 'AU', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (954, 'GUCCI', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (955, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (956, '', '0', 0, 'A', 990, '0', '0', '0', '0') ;
insert  into avatarlist values (957, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (958, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (959, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (960, 'RAIN', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (961, 'RAIN', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (962, 'RAIN', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (963, 'RAIN', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (964, 'SexyYeon-2', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (965, 'SexyYeon-2', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (966, 'SexyYeon-2', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (967, '', '0', 1, 'A', 810, '0', '0', '0', '0') ;
insert  into avatarlist values (968, 'GUCCI ɢ', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (969, 'GUCCI', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (970, 'GUCCI', '0', 1, 'A', 910, '0', '0', '0', '0') ;
insert  into avatarlist values (971, 'GUCCI', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (972, 'ADAIDS', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (973, 'HYOL-4', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (974, 'HYOL-4', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (975, 'HYOL-4', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (976, 'XIAO', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (977, 'DIOR', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (978, 'XIAO', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (979, 'DIOR èŮ', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (980, 'DIOR', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (981, 'EBACE', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (982, 'CHANEL', '0', 0, 'A', 810, '0', '0', '0', '0') ;
insert  into avatarlist values (983, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (984, '', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (985, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (986, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (987, '', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (988, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (989, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (990, 'ʥ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (991, 'ʥ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (992, 'ʥ', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (993, 'ʥ', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (994, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (995, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (996, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (997, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (998, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (999, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1000, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1001, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1002, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1003, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1004, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1005, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1006, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1007, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1008, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1009, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1010, '', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1011, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1012, '', '0', 1, 'A', 980, '0', '0', '0', '0') ;
insert  into avatarlist values (1013, '', '0', 1, 'A', 980, '0', '0', '0', '0') ;
insert  into avatarlist values (1014, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1015, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1016, 'ָ', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1017, 'ICECREAM T', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1018, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1019, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1020, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1021, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1022, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1023, '', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1024, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1025, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1026, 'JEWEL-2', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1027, 'JEWEL-2', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1028, 'JEWEL-2', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1029, 'JEWEL-2', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1030, '', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1031, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1032, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1033, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1034, 'Ħ', '0', 1, 'A', 810, '0', '0', '0', '0') ;
insert  into avatarlist values (1035, 'Ħ', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1036, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1037, 'ָ', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1038, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1039, 'DIOR ¶', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1040, 'DIOR Ů', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1041, 'XIAO', '0', 0, 'A', 980, '0', '0', '0', '0') ;
insert  into avatarlist values (1042, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1043, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1044, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1045, 'DIOR', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1046, 'DIOR', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1047, 'DIOR', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1048, 'DIOR', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1049, 'DIOR', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1050, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1051, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1052, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1053, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1054, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1055, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1056, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1057, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1058, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1059, '', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1060, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1061, '', '0', 1, 'A', 660, '0', '0', '0', '0') ;
insert  into avatarlist values (1062, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1063, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1064, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1065, '', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1066, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1067, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1068, 'JOLIN', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1069, 'DIOR', '0', 0, 'A', 1500, '0', '0', '0', '0') ;
insert  into avatarlist values (1070, 'DIOR', '0', 0, 'A', 980, '0', '0', '0', '0') ;
insert  into avatarlist values (1071, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1072, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1073, 'DIOR', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1074, 'DIOR ţ', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1075, 'JACKJONES', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1076, 'JACKJONES', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1077, 'ʷ', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1078, 'ʷ', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1079, 'ʷ', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1080, 'ʷ', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1081, 'ʷ', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1082, 'ʷ', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1083, 'ʷ', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1084, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1085, 'BYJ-1', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1086, 'BYJ-1', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1087, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1088, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1089, 'JACKJONES', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1090, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1091, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1092, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1093, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1094, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1095, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1096, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1097, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1098, 'DIOR ˫', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1099, 'DIOR ˫', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1100, 'GSTAR', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1101, 'GSTAR', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1102, 'GSTAR', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1103, 'DIOR', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1104, 'PUBGIRL', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1105, 'PUBGIRL', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1106, 'PUBGIRL', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1107, 'ӡ', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1108, 'ӡ', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1109, 'ӡ', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1110, 'ӡ', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1111, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1112, '', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1113, '', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1114, '', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1115, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1116, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1117, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1118, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1119, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1120, '', '0', 0, 'A', 980, '0', '0', '0', '0') ;
insert  into avatarlist values (1121, 'DIOR', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1122, 'DIOR', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1123, '', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1124, 'DIOR', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1125, 'DIOR', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1126, '', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1127, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1128, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1129, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1130, '', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1131, 'YOORA', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1132, 'YOORA', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1133, 'YOORA', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1134, 'YOORA', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1135, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1136, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1137, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1138, '', '0', 1, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1139, '', '0', 1, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1140, '', '0', 1, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1141, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1142, 'DIOR', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1143, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1144, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1145, 'D&G', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1146, 'D&G', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1147, 'D&G', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1148, '', '0', 1, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1149, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1150, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1151, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1152, 'CHANEL', '0', 0, 'A', 980, '0', '0', '0', '0') ;
insert  into avatarlist values (1153, 'CHANEL', '0', 0, 'A', 980, '0', '0', '0', '0') ;
insert  into avatarlist values (1154, 'CHANEL', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1155, 'HYOL-5', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1156, 'HYOL-5', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1157, 'HYOL-5', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1158, 'HYOL-5', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1159, 'EBACE', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1160, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1161, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1162, 'DIOR', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1163, 'DIOR', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1164, 'DIOR', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1165, 'PD', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1166, '', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1167, '', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1168, '', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1169, '', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1170, 'Ǭ', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1171, 'Ǭ', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1172, 'Ǭ', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1173, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1174, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1175, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1176, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1177, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1178, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1179, '', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1180, '', '0', 1, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1181, 'BZ', '0', 1, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1182, 'BZ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1183, 'BZ', '0', 1, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1184, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1185, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1186, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1187, '', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1188, '', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1189, 'DIOR', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1190, 'LV', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1191, 'LV Ƥ', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1192, 'LV', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1193, 'DIOR', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1194, 'DIOR', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1195, 'DIOR', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1196, 'DIOR', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1197, 'DBSK-6', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1198, 'DBSK-6', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1199, 'DBSK-6', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1200, 'HYOL-6', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1201, 'HYOL-6', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1202, 'KTH-1', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1203, 'KTH-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1204, 'KTH-1', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1205, 'XIAO í', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1206, 'XIAO í', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1207, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1208, 'KHJ-1', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1209, 'LV', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1210, '', '0', 0, 'A', 5000, '0', '0', '0', '0') ;
insert  into avatarlist values (1211, '', '0', 1, 'A', 5000, '0', '0', '0', '0') ;
insert  into avatarlist values (1212, 'LV', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1213, 'EBACE', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1214, 'EBACE', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1215, 'HYOL-6', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1216, 'FHATHER Ů', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1217, 'FHATHER Ů', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1218, 'FHATHER Ů', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1219, 'FHATHER Ů', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1220, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1221, 'ZENKI', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1222, 'ZENKI', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1223, 'ZENKI', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1224, 'ZENKI', '0', 1, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1225, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1226, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1227, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1228, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1229, 'GUCCI ͯ', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1230, 'GUCCI ͯ', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1231, 'GUCCI ͯ', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1232, 'MGY-2', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1233, 'MGY-2', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1234, 'MGY-2', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (1235, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1236, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1237, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1238, '', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1239, 'MGY-3', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1240, 'MGY-3', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1241, 'MGY-3', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1242, 'MGY-3', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1243, 'ANSON-1', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1244, 'ANSON-1', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1245, 'SE-1', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1246, 'SE-1', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1247, 'SE-1', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1248, 'SE-1', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1249, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1250, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1251, '', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1252, '', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1253, '', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1254, '', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1255, '', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1256, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1257, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1258, 'GINO Ұ', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1259, 'GINO Ұ', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1260, 'GINO Ұ', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1261, 'GINO Ұ', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1262, 'COUNTER STRIKE', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1263, 'COUNTER STRIKE', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1264, 'COUNTER STRIKE', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1265, 'COUNTER STRIKE', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1266, '', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1267, '', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1268, '', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1269, 'HJW-1', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1270, 'HJW-1', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1271, 'HJW-1', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1272, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1273, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1274, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1275, 'PUB Ů', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1276, 'PUB Ů', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1277, 'PUB Ů', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1278, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1279, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1280, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1281, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1282, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1283, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1284, 'EBACE', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1285, 'EBACE', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1286, 'EBACE', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1287, 'DBSK-7', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1288, 'DBSK-7', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1289, 'DBSK-7', '0', 1, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1290, '', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (1291, 'HJW-2', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1292, 'HJW-2', '0', 0, 'A', 480, '0', '0', '0', '0') ;
insert  into avatarlist values (1293, 'HJW-2', '0', 0, 'A', 380, '0', '0', '0', '0') ;
insert  into avatarlist values (1294, '', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1295, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1296, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1297, '', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1298, 'BIN-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1299, 'BIN-1', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1300, 'BIN-1', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1301, 'J', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1302, '', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1303, '', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1304, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1305, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1306, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1307, '', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1308, '', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1309, 'ŷ', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1310, 'ŷ', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1311, 'ŷ', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1312, 'ŷ', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1313, 'CHANEL', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1314, 'CHANEL', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1315, 'DANNI ʱװ', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1316, 'DANNI ʱװ', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1317, 'DANNI ʱװ', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (1318, 'DANNI ʱװ', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (1319, 'BINI', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1320, 'BINI', '0', 1, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (1321, 'BINI', '0', 1, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (1322, 'BINI', '0', 1, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (1323, 'CD', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1324, 'CD', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1325, 'CD', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1326, 'ANSON-2', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1327, 'ANSON-2', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1328, 'ANSON-2', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1329, 'ANSON-2', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1330, 'EBACE', '0', 1, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1331, 'EBACE', '0', 1, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1332, 'EBACE', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1333, 'EBACE', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1334, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1335, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1336, '', '0', 0, 'A', 800, '0', '0', '0', '0') ;
insert  into avatarlist values (1337, '', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1338, 'HCY-1', '0', 0, 'A', 900, '0', '0', '0', '0') ;
insert  into avatarlist values (1339, 'HCY-1', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1340, 'Alana', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1341, 'Alana', '0', 0, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (1342, 'KMJ-1', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1343, 'KMJ-1', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1344, 'KMJ-1', '0', 1, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1345, 'CY-1', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1346, 'CY-1', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1347, 'CY-1', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1348, 'CY-1', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1349, 'GHJ-1', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1350, 'GHJ-1', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1351, 'GHJ-1', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1352, 'LJH-1', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1353, 'LJH-1', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1354, 'LJH-1', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1355, 'LJH-1', '0', 0, 'A', 400, '0', '0', '0', '0') ;
insert  into avatarlist values (1356, '', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1357, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1358, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1359, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1360, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1361, '', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1362, 'DORAJI', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1363, 'DORAJI', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1364, 'DORAJI', '0', 1, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1365, 'DORAJI', '0', 1, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1366, 'AYM-1', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1367, 'AYM-1', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1368, 'AYM-1', '0', 0, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (1369, 'AYM-1', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (1370, 'С', '0', 0, 'A', 750, '0', '0', '0', '0') ;
insert  into avatarlist values (1371, 'С', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1372, 'С', '0', 0, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (1373, 'С', '0', 0, 'A', 350, '0', '0', '0', '0') ;
insert  into avatarlist values (1374, '', '0', 0, 'A', 750, '0', '0', '0', '0') ;
insert  into avatarlist values (1375, '', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1376, '', '0', 0, 'A', 450, '0', '0', '0', '0') ;
insert  into avatarlist values (1377, '', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1378, 'ʥ', '0', 1, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (1379, 'ʥ', '0', 0, 'A', 50, '0', '0', '0', '0') ;
insert  into avatarlist values (1380, 'PARDA', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1381, 'PARDA', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1382, 'PARDA', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1383, 'PRADA', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1384, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1385, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1386, '', '0', 1, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1387, '', '0', 1, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1388, '', '0', 0, 'A', 810, '0', '0', '0', '0') ;
insert  into avatarlist values (1389, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1390, '', '0', 0, 'A', 780, '0', '0', '0', '0') ;
insert  into avatarlist values (1391, 'Dior', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1392, 'Dior', '0', 0, 'A', 880, '0', '0', '0', '0') ;
insert  into avatarlist values (1393, 'Dior', '0', 0, 'A', 680, '0', '0', '0', '0') ;
insert  into avatarlist values (1394, 'ʥ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1395, 'ʥ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1396, 'ʥ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1397, 'ʥ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1398, 'ʥ', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1399, 'ʥ', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1400, 'ʥ', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1401, 'ʥ', '0', 0, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1402, 'JEDI', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1403, 'JEDI', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1404, 'JEDI', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1405, 'JEDI', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1406, 'Nina Ricci', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1407, 'Nina Ricci', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1408, 'Nina Ricci', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1409, 'DIOR', '0', 0, 'A', 2000, '0', '0', '0', '0') ;
insert  into avatarlist values (1410, 'Nina Ricci', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1411, 'EBC', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1412, 'EBC', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1413, 'EBC ԭҰ֮', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1414, 'EBC ī', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1415, '???? ???', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1416, '???? ?????', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1417, '????? ???', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1418, '??? ???', '0', 2, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1419, 'Givenchy', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1420, 'Givenchy', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1421, '???? ????', '0', 1, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1422, 'Givenchy', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1423, 'ALEX', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1424, 'ALEX', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1425, 'ALEX', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1426, 'ALEX', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1427, 'Cerruti', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1428, 'Cerruti', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1429, 'Cerruti', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1430, 'Cerruti', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1431, 'Issey Miyake', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1432, 'Issey Miyake', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1433, 'Issey Miyake', '0', 0, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1434, 'Issey Miyake', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1435, 'WITCH', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1436, 'WITCH ¶', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1437, 'WITCH', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1438, 'Anna sui', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1439, 'Anna sui', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1440, 'Anna sui', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1441, 'BOSS', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1442, 'BOSS', '0', 1, 'A', 390, '0', '0', '0', '0') ;
insert  into avatarlist values (1443, 'BOSS', '0', 1, 'A', 390, '0', '0', '0', '0') ;
insert  into avatarlist values (1444, 'BOSS', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1445, 'GUCCI', '0', 0, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (1446, 'GUCCI', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1447, 'GUCCI', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1448, 'GUCCI', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1449, 'ELEVEN', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1450, 'ELEVEN ˫', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1451, 'ELEVEN ǰ', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1452, 'ELEVEN', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1453, '??? ???', '0', 2, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1454, 'GHOST', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1455, 'GHOST', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1456, 'GHOST', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1457, 'GHOST ƤЬ', '0', 1, 'A', 390, '0', '0', '0', '0') ;
insert  into avatarlist values (1458, 'JIM', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1459, 'JIM', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1460, 'JIM', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1461, 'JIM', '0', 1, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1462, 'E ǰ׹', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1463, 'E', '0', 1, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1464, 'E', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1465, '', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1466, '', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1467, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1468, '', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1469, 'XIAOXIAO', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1470, 'XIAOXIAO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1471, 'XIAOXIAO', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1472, 'XIAOXIAO ëëѥ', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1473, 'CHANEL', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1474, 'CHANEL', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1475, 'CHANEL', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1476, 'CHANEL', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1477, '', '0', 0, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1478, '', '0', 0, 'A', 730, '0', '0', '0', '0') ;
insert  into avatarlist values (1479, '', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1480, 'ZENKI', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1481, 'ZENKI', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1482, 'ZENKI', '0', 1, 'A', 210, '0', '0', '0', '0') ;
insert  into avatarlist values (1483, 'ZENKI', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1484, 'JOLIN', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1485, 'JOLIN', '0', 0, 'A', 590, '0', '0', '0', '0') ;
insert  into avatarlist values (1486, 'JOLIN', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1487, 'GHOST', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1488, 'GHOST', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1489, 'GHOST Ƥ', '0', 1, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1490, 'GHOST', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1491, 'ANSWER ɢ', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1492, 'ANSWER', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1493, 'ANSWER', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1494, 'HIPHOP', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1495, 'ZENKI', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1496, 'ZENKI', '0', 1, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1497, 'ZENKI', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1498, 'ANSWER ë', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1499, 'ANSWER', '0', 1, 'A', 550, '0', '0', '0', '0') ;
insert  into avatarlist values (1500, 'ANSWER', '0', 1, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1501, 'ANSWER', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1502, 'XIAOXIAO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1503, 'AUDITION ӡ', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1504, 'XIAOXIAO', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1505, 'XIAOXIAO', '0', 0, 'A', 660, '0', '0', '0', '0') ;
insert  into avatarlist values (1506, 'XIAOXIAO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1507, 'XIAOXIAO ţ', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1508, 'XIAOXIAO', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1509, 'Sonia Rykiel', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1510, 'Sonia Rykiel', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1511, 'Sonia Rykiel', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1512, 'Sonia Rykiel', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (1513, 'STROIT', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1514, 'STROIT', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1515, 'STROIT', '0', 1, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1516, 'STROIT', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1517, 'HERO', '0', 1, 'A', 590, '0', '0', '0', '0') ;
insert  into avatarlist values (1518, 'HERO', '0', 1, 'A', 590, '0', '0', '0', '0') ;
insert  into avatarlist values (1519, 'HERO', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1520, 'HERO', '0', 1, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1521, 'Benetton', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1522, 'Benetton', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1523, 'Benetton', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1524, 'Benetton', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1525, 'XIAOXIAO', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1526, 'XIAOXIAO ë', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1527, 'XIAOXIAO', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1528, 'XIAOXIAO', '0', 0, 'A', 430, '0', '0', '0', '0') ;
insert  into avatarlist values (1529, 'Fendi', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1530, 'Fendi', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1531, 'Fendi', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1532, '', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1533, '', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1534, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1535, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1536, 'Burberry', '0', 1, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1537, 'Burberry', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1538, 'Burberry', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1539, 'Burberry', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1540, 'XIAOXIAO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1541, 'XIAOXIAO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1542, 'XIAOXIAO', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1543, 'SUMMER', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1544, 'SUMMER Ϸ', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1545, 'SUMMER', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1546, 'SUMMER', '0', 0, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1547, 'CLEAN', '0', 0, 'A', 580, '0', '0', '0', '0') ;
insert  into avatarlist values (1548, 'CLEAN ˿', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1549, 'CLEAN', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1550, 'DORAJI', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1551, 'DORAJI', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1552, 'DORAJI', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1553, 'DORAJI', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1554, 'XIAOXIAO', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1555, 'XIAOXIAO', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1556, 'Burberry', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1557, 'XIAOXIAO', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1558, 'XIAOXIAO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1559, 'XIAOXIAO Բ', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1560, 'XIAOXIAO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1561, 'XIAOXIAO', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1562, 'XIAOXIO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1563, 'XIAOXIO', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1564, 'XIAOXIO', '0', 0, 'A', 590, '0', '0', '0', '0') ;
insert  into avatarlist values (1565, 'XIAO', '0', 0, 'A', 410, '0', '0', '0', '0') ;
insert  into avatarlist values (1566, 'XIAO', '0', 0, 'A', 510, '0', '0', '0', '0') ;
insert  into avatarlist values (1567, 'XIAO', '0', 0, 'A', 490, '0', '0', '0', '0') ;
insert  into avatarlist values (1568, 'XIAO', '0', 0, 'A', 310, '0', '0', '0', '0') ;
insert  into avatarlist values (1569, 'ETAM', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1570, 'ETAM', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1571, 'ETAM', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1572, 'ETAM', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1573, 'C&K Ƥ', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1574, 'C&K', '0', 1, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1575, 'C&K ţ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1576, 'C&K', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1577, 'FREE ɢ', '0', 1, 'A', 1000, '0', '0', '0', '0') ;
insert  into avatarlist values (1578, 'FREE ţ', '0', 1, 'A', 500, '0', '0', '0', '0') ;
insert  into avatarlist values (1579, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1580, '', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1581, '', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1582, '', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1583, 'JONES Ů', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1584, 'JONES ţ', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1585, 'JONES ţ', '0', 0, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1586, 'JONES ţ', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1587, 'XIAOXIAO', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1588, 'XIAOXIAO', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1589, 'XIAOXIAO', '0', 0, 'A', 100, '0', '0', '0', '0') ;
insert  into avatarlist values (1590, 'XIAOXIAO', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1591, 'XIAOXIAO', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1592, 'XIAOXIAO', '0', 0, 'A', 600, '0', '0', '0', '0') ;
insert  into avatarlist values (1593, 'XIAOXIAO', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1594, 'JACKJONES Ƥñ', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1595, 'JACKJONES Ƥ', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1596, 'JACKJONES ȫ', '0', 1, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1597, 'JACKJONES', '0', 1, 'A', 590, '0', '0', '0', '0') ;
insert  into avatarlist values (1598, '', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1599, '', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1600, '', '0', 1, 'A', 700, '0', '0', '0', '0') ;
insert  into avatarlist values (1601, '', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1602, '', '0', 1, 'A', 710, '0', '0', '0', '0') ;
insert  into avatarlist values (1603, '', '0', 1, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1604, '', '0', 1, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1605, 'EBACE', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1606, 'EBACE', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1607, 'EBACE ţ', '0', 0, 'A', 650, '0', '0', '0', '0') ;
insert  into avatarlist values (1608, 'EBACE', '0', 0, 'A', 610, '0', '0', '0', '0') ;
insert  into avatarlist values (1609, 'XIAOXIAO', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1610, 'XIAOXIAO', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1611, 'XIAOXIAO', '0', 0, 'A', 690, '0', '0', '0', '0') ;
insert  into avatarlist values (1612, 'XIAOXIAO', '0', 0, 'A', 610, '0', '0', '0', '0') ;

#----------------------------
# Table structure for battleround
#----------------------------
drop table if exists battleround;
CREATE TABLE `battleround` (
  `RoundSN` int(11) NOT NULL auto_increment,
  `Server` smallint(6) NOT NULL default '0',
  `Channel` smallint(6) NOT NULL default '0',
  `RoomNum` smallint(6) NOT NULL default '0',
  `RoomName` varchar(32) NOT NULL default '',
  `JoinUserCount` tinyint(4) NOT NULL default '0',
  `EntryFee` int(11) NOT NULL default '0',
  `Prize` int(11) NOT NULL default '0',
  `MasterSN` int(11) unsigned NOT NULL default '0',
  `MusicCode` varchar(16) NOT NULL default '0',
  `StageCode` smallint(6) NOT NULL default '0',
  `GameMode` tinyint(4) NOT NULL default '0',
  `StartTime` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`RoundSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table battleround
#----------------------------

#----------------------------
# Table structure for battleroundresult
#----------------------------
drop table if exists battleroundresult;
CREATE TABLE `battleroundresult` (
  `ResultSN` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `RoundSN` int(11) NOT NULL default '0',
  `AfterMoney` int(11) NOT NULL default '0',
  `Win` tinyint(4) NOT NULL default '0',
  `EndTime` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`ResultSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table battleroundresult
#----------------------------

#----------------------------
# Table structure for battleuser
#----------------------------
drop table if exists battleuser;
CREATE TABLE `battleuser` (
  `UserSN` int(10) unsigned NOT NULL default '0',
  `DM00` tinyint(3) unsigned NOT NULL default '0',
  `DM01` tinyint(3) unsigned NOT NULL default '0',
  `DM02` tinyint(3) unsigned NOT NULL default '0',
  `DM03` tinyint(3) unsigned NOT NULL default '0',
  `DM04` tinyint(3) unsigned NOT NULL default '0',
  `DM05` tinyint(3) unsigned NOT NULL default '0',
  `DM06` tinyint(3) unsigned NOT NULL default '0',
  `DM07` tinyint(3) unsigned NOT NULL default '0',
  `DM08` tinyint(3) unsigned NOT NULL default '0',
  `DM09` tinyint(3) unsigned NOT NULL default '0',
  `DM10` tinyint(3) unsigned NOT NULL default '0',
  `DM11` tinyint(3) unsigned NOT NULL default '0',
  `DM12` tinyint(3) unsigned NOT NULL default '0',
  `DM13` tinyint(3) unsigned NOT NULL default '0',
  `DM14` tinyint(3) unsigned NOT NULL default '0',
  `DM15` tinyint(3) unsigned NOT NULL default '0',
  `DM16` tinyint(3) unsigned NOT NULL default '0',
  `DM17` tinyint(3) unsigned NOT NULL default '0',
  `DM18` tinyint(3) unsigned NOT NULL default '0',
  `DM19` tinyint(3) unsigned NOT NULL default '0',
  `DM20` tinyint(3) unsigned NOT NULL default '0',
  `DM21` tinyint(3) unsigned NOT NULL default '0',
  `DM22` tinyint(3) unsigned NOT NULL default '0',
  `DM23` tinyint(3) unsigned NOT NULL default '0',
  `DM24` tinyint(3) unsigned NOT NULL default '0',
  `DM25` tinyint(3) unsigned NOT NULL default '0',
  `DM26` tinyint(3) unsigned NOT NULL default '0',
  `DM27` tinyint(3) unsigned NOT NULL default '0',
  `DM28` tinyint(3) unsigned NOT NULL default '0',
  `DM29` tinyint(3) unsigned NOT NULL default '0',
  `DM30` tinyint(3) unsigned NOT NULL default '0',
  `DM31` tinyint(3) unsigned NOT NULL default '0',
  `DM32` tinyint(3) unsigned NOT NULL default '0',
  `DM33` tinyint(3) unsigned NOT NULL default '0',
  `DM34` tinyint(3) unsigned NOT NULL default '0',
  `DM35` tinyint(3) unsigned NOT NULL default '0',
  `DM36` tinyint(3) unsigned NOT NULL default '0',
  `DM37` tinyint(3) unsigned NOT NULL default '0',
  `DM38` tinyint(3) unsigned NOT NULL default '0',
  `DM39` tinyint(3) unsigned NOT NULL default '0',
  `DM40` tinyint(3) unsigned NOT NULL default '0',
  `DM41` tinyint(3) unsigned NOT NULL default '0',
  `DM42` tinyint(3) unsigned NOT NULL default '0',
  `DM43` tinyint(3) unsigned NOT NULL default '0',
  `DM44` tinyint(3) unsigned NOT NULL default '0',
  `DM45` tinyint(3) unsigned NOT NULL default '0',
  `DM46` tinyint(3) unsigned NOT NULL default '0',
  `DM47` tinyint(3) unsigned NOT NULL default '0',
  `DM48` tinyint(3) unsigned NOT NULL default '0',
  `DM49` tinyint(3) unsigned NOT NULL default '0',
  `DM50` tinyint(3) unsigned NOT NULL default '0',
  `DM51` tinyint(3) unsigned NOT NULL default '0',
  `DM52` tinyint(3) unsigned NOT NULL default '0',
  `DM53` tinyint(3) unsigned NOT NULL default '0',
  `DM54` tinyint(3) unsigned NOT NULL default '0',
  `DM55` tinyint(3) unsigned NOT NULL default '0',
  `DM56` tinyint(3) unsigned NOT NULL default '0',
  `DM57` tinyint(3) unsigned NOT NULL default '0',
  `DM58` tinyint(3) unsigned NOT NULL default '0',
  `DM59` tinyint(3) unsigned NOT NULL default '0',
  `DM60` tinyint(3) unsigned NOT NULL default '0',
  `DM61` tinyint(3) unsigned NOT NULL default '0',
  `DM62` tinyint(3) unsigned NOT NULL default '0',
  `DM63` tinyint(3) unsigned NOT NULL default '0',
  `DM64` tinyint(3) unsigned NOT NULL default '0',
  `DM65` tinyint(3) unsigned NOT NULL default '0',
  `DM66` tinyint(3) unsigned NOT NULL default '0',
  `DM67` tinyint(3) unsigned NOT NULL default '0',
  `DM68` tinyint(3) unsigned NOT NULL default '0',
  `DM69` tinyint(3) unsigned NOT NULL default '0',
  `DM70` tinyint(3) unsigned NOT NULL default '0',
  `DM71` tinyint(3) unsigned NOT NULL default '0',
  `DM72` tinyint(3) unsigned NOT NULL default '0',
  `DM73` tinyint(3) unsigned NOT NULL default '0',
  `DM74` tinyint(3) unsigned NOT NULL default '0',
  `DM75` tinyint(3) unsigned NOT NULL default '0',
  `DM76` tinyint(3) unsigned NOT NULL default '0',
  `DM77` tinyint(3) unsigned NOT NULL default '0',
  `DM78` tinyint(3) unsigned NOT NULL default '0',
  `DM79` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`UserSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table battleuser
#----------------------------

#----------------------------
# Table structure for board_item_info
#----------------------------
drop table if exists board_item_info;
CREATE TABLE `board_item_info` (
  `BoardSN` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `Type` tinyint(3) unsigned NOT NULL default '0',
  `Color` int(10) NOT NULL default '0',
  `BeforeMoney` int(10) unsigned NOT NULL default '0',
  `Price` int(10) unsigned NOT NULL default '0',
  `AfterMoney` int(10) unsigned NOT NULL default '0',
  `UseTime` datetime NOT NULL default '0000-00-00 00:00:00',
  `BoardMsg` varchar(64) NOT NULL default '',
  `DestServer` int(10) NOT NULL default '-1',
  `SendConfirm` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`BoardSN`),
  KEY `UserSN` (`UserSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table board_item_info
#----------------------------

#----------------------------
# Table structure for buylog
#----------------------------
drop table if exists buylog;
CREATE TABLE `buylog` (
  `BuySN` int(11) NOT NULL auto_increment,
  `UserSN` int(11) unsigned NOT NULL default '0',
  `GoodsType` enum('A','M','D') NOT NULL default 'A',
  `GoodsIndex` int(11) unsigned NOT NULL default '0',
  `GoodsPrice` int(11) unsigned NOT NULL default '0',
  `BeforeMoney` int(11) unsigned NOT NULL default '0',
  `AfterMoney` int(11) unsigned NOT NULL default '0',
  `Time` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
  `Period` smallint(6) NOT NULL default '0',
  `BuyDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `ApplyDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`BuySN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table buylog
#----------------------------

#----------------------------
# Table structure for channellist
#----------------------------
drop table if exists channellist;
CREATE TABLE `channellist` (
  `ServerNum` smallint(2) unsigned NOT NULL default '0',
  `Num` smallint(2) unsigned NOT NULL default '0',
  `Name` varchar(32) NOT NULL default '',
  `UserCount` smallint(2) unsigned NOT NULL default '200',
  `RoomCount` smallint(2) unsigned NOT NULL default '100',
  `MinLevel` smallint(2) unsigned NOT NULL default '0',
  `MaxLevel` smallint(2) unsigned NOT NULL default '0',
  `EventNum` tinyint(2) unsigned NOT NULL default '0',
  PRIMARY KEY  (`ServerNum`,`Num`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table channellist
#----------------------------
insert  into channellist values (101, 0, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 1, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 2, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 3, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 4, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 5, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 6, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 7, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 8, 'RageZone', 200, 100, 0, 61, 0) ;
insert  into channellist values (101, 9, 'RageZone', 200, 100, 0, 61, 0) ;

#----------------------------
# Table structure for couple_info
#----------------------------
drop table if exists couple_info;
CREATE TABLE `couple_info` (
  `CoupleSN` int(10) unsigned NOT NULL default '0',
  `MaleSN` int(10) unsigned NOT NULL auto_increment,
  `FemaleSN` int(10) unsigned NOT NULL default '0',
  `CoupleDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`MaleSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table couple_info
#----------------------------

#----------------------------
# Table structure for couple_log
#----------------------------
drop table if exists couple_log;
CREATE TABLE `couple_log` (
  `LogSN` int(10) unsigned NOT NULL auto_increment,
  `FemaleSN` int(10) unsigned NOT NULL default '0',
  `MaleSN` int(10) unsigned NOT NULL default '0',
  `CoupleDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `SeparateDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `ConfirmDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `SeparateSN` int(10) unsigned NOT NULL default '0',
  `FemaleLevel` int(10) unsigned NOT NULL default '0',
  `MaleLevel` int(10) unsigned NOT NULL default '0',
  `BeforeDen` int(10) unsigned NOT NULL default '0',
  `AfterDen` int(10) unsigned NOT NULL default '0',
  PRIMARY KEY  (`LogSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table couple_log
#----------------------------

#----------------------------
# Table structure for couplepresentden
#----------------------------
drop table if exists couplepresentden;
CREATE TABLE `couplepresentden` (
  `PresentSN` int(10) unsigned NOT NULL auto_increment,
  `RoundSN` int(10) unsigned NOT NULL default '0',
  `FromSN` int(10) unsigned NOT NULL default '0',
  `ToSN` int(10) unsigned NOT NULL default '0',
  `Money` smallint(5) unsigned NOT NULL default '0',
  PRIMARY KEY  (`PresentSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table couplepresentden
#----------------------------

#----------------------------
# Table structure for coupleround
#----------------------------
drop table if exists coupleround;
CREATE TABLE `coupleround` (
  `RoundSN` int(10) unsigned NOT NULL auto_increment,
  `Server` smallint(5) unsigned NOT NULL default '0',
  `Channel` smallint(5) unsigned NOT NULL default '0',
  `RoomNum` smallint(5) unsigned NOT NULL default '0',
  `RoomName` varchar(32) NOT NULL default '',
  `GameMode` tinyint(3) unsigned NOT NULL default '0',
  `UserSN_A_M` int(10) unsigned NOT NULL default '0',
  `UserSN_A_F` int(10) unsigned NOT NULL default '0',
  `UserSN_B_M` int(10) unsigned NOT NULL default '0',
  `UserSN_B_F` int(10) unsigned NOT NULL default '0',
  `StartTime` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`RoundSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table coupleround
#----------------------------

#----------------------------
# Table structure for hack
#----------------------------
drop table if exists hack;
CREATE TABLE `hack` (
  `HackIndex` int(12) unsigned NOT NULL auto_increment,
  `HackSN` int(12) unsigned NOT NULL default '0',
  `HackNick` varchar(42) NOT NULL default '',
  `ServerName` varchar(32) NOT NULL default '',
  `RoomName` varchar(40) NOT NULL default '',
  `RoomNum` smallint(2) unsigned NOT NULL default '0',
  `Exp` int(4) NOT NULL default '0',
  `Money` int(4) NOT NULL default '0',
  `Cash` int(4) NOT NULL default '0',
  `HackDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `Kind` varchar(16) NOT NULL default 'speed',
  PRIMARY KEY  (`HackIndex`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table hack
#----------------------------

#----------------------------
# Table structure for item_use_info
#----------------------------
drop table if exists item_use_info;
CREATE TABLE `item_use_info` (
  `UseSN` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `UseCount` int(10) unsigned NOT NULL default '0',
  `LastTime` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`UseSN`),
  KEY `UserSN` (`UserSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table item_use_info
#----------------------------

#----------------------------
# Table structure for jjang
#----------------------------
drop table if exists jjang;
CREATE TABLE `jjang` (
  `JJangIndex` int(4) unsigned NOT NULL auto_increment,
  `jjangnick` varchar(42) NOT NULL default '',
  `JJangDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`JJangIndex`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table jjang
#----------------------------

#----------------------------
# Table structure for level_quest
#----------------------------
drop table if exists level_quest;
CREATE TABLE `level_quest` (
  `Level` tinyint(1) unsigned NOT NULL default '1',
  `Perfect` tinyint(1) unsigned NOT NULL default '0',
  `ConsecutivePerfect` tinyint(1) unsigned NOT NULL default '0',
  `Score` int(4) unsigned NOT NULL default '0',
  `GameMode` tinyint(1) unsigned NOT NULL default '0',
  `MusicCode` smallint(2) unsigned NOT NULL default '0',
  `StageCode` tinyint(1) unsigned NOT NULL default '0',
  `Fee` smallint(2) unsigned NOT NULL default '0',
  `WinDen` smallint(2) unsigned NOT NULL default '0',
  `WinExp` smallint(2) unsigned NOT NULL default '0',
  PRIMARY KEY  (`Level`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table level_quest
#----------------------------
insert  into level_quest values (1, 0, 0, 0, 0, 0, 0, 0, 0, 0) ;
insert  into level_quest values (2, 0, 0, 0, 0, 0, 0, 0, 0, 0) ;
insert  into level_quest values (3, 0, 0, 0, 0, 0, 0, 0, 0, 0) ;
insert  into level_quest values (4, 0, 0, 0, 0, 0, 0, 0, 0, 0) ;
insert  into level_quest values (5, 5, 2, 145000, 0, 17, 0, 150, 600, 0) ;
insert  into level_quest values (6, 5, 2, 160000, 0, 17, 0, 200, 800, 0) ;
insert  into level_quest values (7, 5, 2, 190000, 0, 17, 0, 250, 1000, 0) ;
insert  into level_quest values (8, 6, 2, 225000, 0, 17, 0, 300, 1200, 0) ;
insert  into level_quest values (9, 6, 2, 240000, 0, 14, 0, 550, 2800, 0) ;
insert  into level_quest values (10, 6, 2, 265000, 0, 14, 0, 600, 3000, 0) ;
insert  into level_quest values (11, 7, 2, 280000, 0, 14, 0, 700, 3500, 0) ;
insert  into level_quest values (12, 7, 2, 295000, 0, 14, 0, 750, 3800, 0) ;
insert  into level_quest values (13, 7, 2, 300000, 0, 14, 0, 850, 5100, 0) ;
insert  into level_quest values (14, 8, 2, 315000, 0, 14, 0, 900, 5400, 0) ;
insert  into level_quest values (15, 8, 2, 240000, 1, 9, 0, 1000, 6000, 0) ;
insert  into level_quest values (16, 8, 2, 265000, 1, 9, 2, 1050, 6300, 0) ;
insert  into level_quest values (17, 8, 2, 280000, 1, 9, 2, 1500, 10500, 0) ;
insert  into level_quest values (18, 9, 2, 295000, 1, 9, 2, 1600, 11200, 0) ;
insert  into level_quest values (19, 9, 2, 280000, 1, 10, 2, 1700, 11900, 0) ;
insert  into level_quest values (20, 13, 2, 295000, 1, 10, 2, 1800, 12600, 0) ;
insert  into level_quest values (21, 14, 2, 315000, 1, 10, 2, 1900, 13300, 0) ;
insert  into level_quest values (22, 12, 2, 330000, 1, 10, 2, 2000, 14000, 0) ;
insert  into level_quest values (23, 10, 2, 280000, 1, 5, 2, 2100, 16800, 0) ;
insert  into level_quest values (24, 11, 2, 290000, 1, 5, 2, 2200, 17600, 0) ;
insert  into level_quest values (25, 12, 3, 295000, 1, 5, 2, 2300, 18400, 0) ;
insert  into level_quest values (26, 13, 3, 305000, 1, 5, 2, 2400, 19200, 0) ;
insert  into level_quest values (27, 10, 3, 400000, 1, 16, 2, 2500, 20000, 0) ;
insert  into level_quest values (28, 10, 3, 415000, 1, 16, 2, 2600, 20800, 0) ;
insert  into level_quest values (29, 15, 3, 430000, 1, 16, 2, 3400, 30600, 0) ;
insert  into level_quest values (30, 16, 3, 450000, 1, 16, 2, 3500, 31500, 0) ;
insert  into level_quest values (31, 14, 3, 330000, 3, 11, 2, 3650, 32900, 0) ;
insert  into level_quest values (32, 15, 3, 350000, 3, 11, 2, 3750, 33800, 0) ;
insert  into level_quest values (33, 16, 3, 350000, 3, 11, 6, 3900, 35100, 0) ;
insert  into level_quest values (34, 16, 3, 360000, 3, 11, 6, 4000, 36000, 0) ;
insert  into level_quest values (35, 15, 3, 375000, 3, 13, 6, 4150, 37400, 0) ;
insert  into level_quest values (36, 16, 3, 390000, 3, 13, 6, 4250, 32300, 0) ;
insert  into level_quest values (37, 17, 3, 405000, 3, 13, 6, 4400, 39600, 0) ;
insert  into level_quest values (38, 18, 3, 410000, 3, 13, 6, 4500, 45000, 0) ;
insert  into level_quest values (39, 16, 3, 425000, 3, 7, 6, 4650, 46500, 0) ;
insert  into level_quest values (40, 17, 3, 430000, 3, 7, 6, 4750, 47500, 0) ;
insert  into level_quest values (41, 18, 3, 440000, 3, 7, 6, 4900, 49000, 0) ;
insert  into level_quest values (42, 19, 3, 450000, 3, 7, 6, 5000, 50000, 0) ;
insert  into level_quest values (43, 16, 3, 370000, 3, 18, 6, 5150, 51500, 0) ;
insert  into level_quest values (44, 17, 3, 375000, 3, 18, 6, 5250, 52500, 0) ;
insert  into level_quest values (45, 18, 3, 385000, 3, 18, 6, 6450, 65535, 0) ;
insert  into level_quest values (46, 19, 3, 390000, 3, 18, 6, 6600, 65535, 0) ;
insert  into level_quest values (47, 18, 4, 385000, 3, 6, 6, 6750, 65535, 0) ;
insert  into level_quest values (48, 19, 4, 390000, 3, 6, 6, 6900, 65535, 0) ;
insert  into level_quest values (49, 20, 4, 400000, 3, 6, 6, 7050, 65535, 0) ;
insert  into level_quest values (50, 22, 5, 410000, 3, 6, 6, 7200, 65535, 0) ;
insert  into level_quest values (51, 22, 6, 515000, 3, 6, 6, 7350, 65535, 0) ;
insert  into level_quest values (52, 22, 6, 520000, 3, 6, 6, 7500, 65535, 0) ;
insert  into level_quest values (53, 22, 6, 530000, 3, 6, 6, 7650, 65535, 0) ;
insert  into level_quest values (54, 23, 6, 535000, 3, 6, 6, 7800, 65535, 0) ;
insert  into level_quest values (55, 23, 7, 540000, 3, 6, 6, 7950, 65535, 0) ;
insert  into level_quest values (56, 23, 7, 545000, 3, 6, 6, 8100, 65535, 0) ;
insert  into level_quest values (57, 23, 7, 550000, 3, 6, 6, 8250, 65535, 0) ;
insert  into level_quest values (58, 24, 7, 555000, 3, 6, 6, 8400, 65535, 0) ;
insert  into level_quest values (59, 24, 8, 560000, 3, 6, 6, 8550, 65535, 0) ;
insert  into level_quest values (60, 24, 8, 565000, 3, 6, 6, 8700, 65535, 0) ;

#----------------------------
# Table structure for levelinfo
#----------------------------
drop table if exists levelinfo;
CREATE TABLE `levelinfo` (
  `Level` tinyint(2) unsigned NOT NULL default '0',
  `Exp` int(10) unsigned NOT NULL default '0',
  PRIMARY KEY  (`Level`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table levelinfo
#----------------------------
insert  into levelinfo values (1, 0) ;
insert  into levelinfo values (2, 220) ;
insert  into levelinfo values (3, 1260) ;
insert  into levelinfo values (4, 3480) ;
insert  into levelinfo values (5, 7140) ;
insert  into levelinfo values (6, 12500) ;
insert  into levelinfo values (7, 19695) ;
insert  into levelinfo values (8, 29000) ;
insert  into levelinfo values (9, 40500) ;
insert  into levelinfo values (10, 54500) ;
insert  into levelinfo values (11, 71000) ;
insert  into levelinfo values (12, 90000) ;
insert  into levelinfo values (13, 112000) ;
insert  into levelinfo values (14, 137000) ;
insert  into levelinfo values (15, 165000) ;
insert  into levelinfo values (16, 206000) ;
insert  into levelinfo values (17, 230500) ;
insert  into levelinfo values (18, 268500) ;
insert  into levelinfo values (19, 310000) ;
insert  into levelinfo values (20, 354500) ;
insert  into levelinfo values (21, 403500) ;
insert  into levelinfo values (22, 456000) ;
insert  into levelinfo values (23, 512000) ;
insert  into levelinfo values (24, 572500) ;
insert  into levelinfo values (25, 637000) ;
insert  into levelinfo values (26, 705500) ;
insert  into levelinfo values (27, 778000) ;
insert  into levelinfo values (28, 855000) ;
insert  into levelinfo values (29, 937000) ;
insert  into levelinfo values (30, 1023000) ;
insert  into levelinfo values (31, 1113000) ;
insert  into levelinfo values (32, 1209000) ;
insert  into levelinfo values (33, 1308000) ;
insert  into levelinfo values (34, 1413500) ;
insert  into levelinfo values (35, 1523000) ;
insert  into levelinfo values (36, 1638000) ;
insert  into levelinfo values (37, 1757500) ;
insert  into levelinfo values (38, 1882500) ;
insert  into levelinfo values (39, 2012000) ;
insert  into levelinfo values (40, 2147500) ;
insert  into levelinfo values (41, 2288000) ;
insert  into levelinfo values (42, 2434000) ;
insert  into levelinfo values (43, 2585000) ;
insert  into levelinfo values (44, 2742000) ;
insert  into levelinfo values (45, 2904500) ;
insert  into levelinfo values (46, 3072500) ;
insert  into levelinfo values (47, 3246500) ;
insert  into levelinfo values (48, 3426000) ;
insert  into levelinfo values (49, 3611000) ;
insert  into levelinfo values (50, 3802500) ;
insert  into levelinfo values (51, 4000000) ;
insert  into levelinfo values (52, 4300000) ;
insert  into levelinfo values (53, 4650000) ;
insert  into levelinfo values (54, 5000000) ;
insert  into levelinfo values (55, 5400000) ;
insert  into levelinfo values (56, 5850000) ;
insert  into levelinfo values (57, 6350000) ;
insert  into levelinfo values (58, 6900000) ;
insert  into levelinfo values (59, 7500000) ;
insert  into levelinfo values (60, 12000000) ;

#----------------------------
# Table structure for levelquestlog
#----------------------------
drop table if exists levelquestlog;
CREATE TABLE `levelquestlog` (
  `LevelQuestSN` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `ProcLevel` smallint(2) unsigned NOT NULL default '0',
  `BeforeDen` int(10) unsigned NOT NULL default '0',
  `AfterDen` int(10) unsigned NOT NULL default '0',
  `BeforeExp` int(10) unsigned NOT NULL default '0',
  `AfterExp` int(10) unsigned NOT NULL default '0',
  `ProcTime` timestamp NOT NULL default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
  `Pass` tinyint(2) unsigned NOT NULL default '0',
  PRIMARY KEY  (`LevelQuestSN`),
  KEY `UserSN` (`UserSN`,`ProcLevel`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table levelquestlog
#----------------------------

#----------------------------
# Table structure for music
#----------------------------
drop table if exists music;
CREATE TABLE `music` (
  `MusicIndex` int(10) unsigned NOT NULL auto_increment,
  `MusicFile` varchar(64) NOT NULL default '',
  PRIMARY KEY  (`MusicIndex`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table music
#----------------------------
insert  into music values (1, 'k1001.tbm') ;
insert  into music values (2, 'k1002.tbm') ;
insert  into music values (3, 'k1003.tbm') ;
insert  into music values (4, 'k1004.tbm') ;
insert  into music values (5, 'k1005.tbm') ;
insert  into music values (6, 'k1007.tbm') ;
insert  into music values (7, 'k1008.tbm') ;
insert  into music values (8, 'k1009.tbm') ;
insert  into music values (9, 'k1011.tbm') ;
insert  into music values (10, 'k1012.tbm') ;
insert  into music values (11, 'k1013.tbm') ;
insert  into music values (12, 'k1014.tbm') ;
insert  into music values (13, 'k1015.tbm') ;
insert  into music values (14, 'k1016.tbm') ;
insert  into music values (15, 'k1017.tbm') ;
insert  into music values (16, 'k1018.tbm') ;
insert  into music values (17, 'k1010.tbm') ;
insert  into music values (18, 'k1006.tbm') ;
insert  into music values (19, 'c0004.tbm') ;
insert  into music values (20, 'e0003.tbm') ;
insert  into music values (21, 'e0004.tbm') ;
insert  into music values (22, 'e0006.tbm') ;
insert  into music values (23, 'e0007.tbm') ;
insert  into music values (24, 'e0009.tbm') ;
insert  into music values (25, 'e0008.tbm') ;
insert  into music values (26, 'e0010.tbm') ;
insert  into music values (27, 'e0014.tbm') ;
insert  into music values (28, 'e0001.tbm') ;
insert  into music values (29, 'e0002.tbm') ;
insert  into music values (30, 'e0005.tbm') ;
insert  into music values (31, 'c0036.tbm') ;
insert  into music values (32, 'e0012.tbm') ;
insert  into music values (33, 'e0013.tbm') ;
insert  into music values (34, 'e0015.tbm') ;
insert  into music values (35, 'e0019.tbm') ;
insert  into music values (36, 'c0037.tbm') ;
insert  into music values (37, 'c0038.tbm') ;
insert  into music values (38, 'c0039.tbm') ;
insert  into music values (39, 'c0040.tbm') ;
insert  into music values (40, 'e0011.tbm') ;
insert  into music values (41, 'c0043.tbm') ;
insert  into music values (42, 'e0016.tbm') ;
insert  into music values (43, 'e0017.tbm') ;
insert  into music values (44, 'e0018.tbm') ;
insert  into music values (45, 'e0020.tbm') ;
insert  into music values (46, 'c0041.tbm') ;
insert  into music values (47, 'c0042.tbm') ;
insert  into music values (48, 'e0021.tbm') ;
insert  into music values (49, 'c0044.tbm') ;
insert  into music values (50, 'c0013.tbm') ;
insert  into music values (51, 'c0035.tbm') ;
insert  into music values (52, 'c1006.tbm') ;
insert  into music values (53, 'c1006.tbm') ;
insert  into music values (54, 'c1007.tbm') ;
insert  into music values (55, 'c1009.tbm') ;
insert  into music values (56, 'c0056.tbm') ;
insert  into music values (57, 'c0020.tbm') ;
insert  into music values (58, 'c0027.tbm') ;
insert  into music values (59, 'c0030.tbm') ;
insert  into music values (60, 'c0018.tbm') ;
insert  into music values (61, 'c0055.tbm') ;
insert  into music values (62, 'c0006.tbm') ;
insert  into music values (63, 'c0031.tbm') ;
insert  into music values (64, 'c1000.tbm') ;
insert  into music values (65, 'c1022.tbm') ;
insert  into music values (66, 'c1001.tbm') ;
insert  into music values (67, 'c1004.tbm') ;
insert  into music values (68, 'c1002.tbm') ;
insert  into music values (69, 'c1003.tbm') ;
insert  into music values (70, 'c1005.tbm') ;
insert  into music values (71, 'c0001.tbm') ;
insert  into music values (72, 'c1010.tbm') ;
insert  into music values (73, 'c1008.tbm') ;
insert  into music values (74, 'c1011.tbm') ;
insert  into music values (75, 'c1012.tbm') ;
insert  into music values (76, 'c0046.tbm') ;
insert  into music values (77, 'c1013.tbm') ;
insert  into music values (78, 'c1014.tbm') ;
insert  into music values (79, 'c1018.tbm') ;
insert  into music values (80, 'c0057.tbm') ;
insert  into music values (81, 'c0058.tbm') ;
insert  into music values (82, 'c1015.tbm') ;
insert  into music values (83, 'c1015.tbm') ;
insert  into music values (84, 'c1016.tbm') ;
insert  into music values (85, 'c1017.tbm') ;
insert  into music values (86, 'c1019.tbm') ;
insert  into music values (87, 'c1020.tbm') ;
insert  into music values (88, 'c1021.tbm') ;
insert  into music values (89, 'c1023.tbm') ;
insert  into music values (90, 'c1024.tbm') ;
insert  into music values (91, 'c1025.tbm') ;
insert  into music values (92, 'c1026.tbm') ;
insert  into music values (93, 'c0045.tbm') ;
insert  into music values (94, 'c0045.tbm') ;
insert  into music values (95, 'c0047.tbm') ;
insert  into music values (96, 'c0048.tbm') ;
insert  into music values (97, 'c0049.tbm') ;
insert  into music values (98, 'c0050.tbm') ;
insert  into music values (99, 'c0051.tbm') ;

#----------------------------
# Table structure for musicplaycount
#----------------------------
drop table if exists musicplaycount;
CREATE TABLE `musicplaycount` (
  `MusicIndex` int(11) NOT NULL default '0',
  `GamePlayCount` int(10) unsigned NOT NULL default '0',
  `ShopPlayCount` int(10) unsigned NOT NULL default '0',
  `PrePlayCount` int(10) unsigned NOT NULL default '0',
  `LastUpdateDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`MusicIndex`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table musicplaycount
#----------------------------

#----------------------------
# Table structure for musicticket
#----------------------------
drop table if exists musicticket;
CREATE TABLE `musicticket` (
  `TicketIndex` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `BuyDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `ExpireDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`TicketIndex`),
  UNIQUE KEY `UserSN` (`UserSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table musicticket
#----------------------------

#----------------------------
# Table structure for musicticketlog
#----------------------------
drop table if exists musicticketlog;
CREATE TABLE `musicticketlog` (
  `LogIndex` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `BuyDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `AddDate` smallint(6) NOT NULL default '0',
  PRIMARY KEY  (`LogIndex`),
  KEY `UserSN` (`UserSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table musicticketlog
#----------------------------

#----------------------------
# Table structure for present
#----------------------------
drop table if exists present;
CREATE TABLE `present` (
  `PresentID` int(12) unsigned NOT NULL auto_increment,
  `SendSN` int(10) NOT NULL default '0',
  `SendNick` varchar(23) NOT NULL default '',
  `RecvSN` int(10) NOT NULL default '0',
  `RecvNick` varchar(23) NOT NULL default '',
  `Kind` varchar(2) NOT NULL default '0',
  `ItemID` int(12) unsigned NOT NULL default '0',
  `Period` smallint(6) NOT NULL default '0',
  `BeforeCash` int(4) unsigned NOT NULL default '0',
  `AfterCash` int(4) unsigned NOT NULL default '0',
  `SendDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `RecvDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `Memo` varchar(120) NOT NULL default '',
  PRIMARY KEY  (`PresentID`),
  KEY `SendSN` (`SendSN`),
  KEY `RecvSN` (`RecvSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table present
#----------------------------

#----------------------------
# Table structure for rank
#----------------------------
drop table if exists rank;
CREATE TABLE `rank` (
  `RankSN` int(10) unsigned NOT NULL auto_increment,
  `UserNick` varchar(42) NOT NULL default '',
  `Exp` int(10) unsigned NOT NULL default '0',
  PRIMARY KEY  (`RankSN`),
  UNIQUE KEY `UserNick` (`UserNick`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table rank
#----------------------------

#----------------------------
# Table structure for rank_temp
#----------------------------
drop table if exists rank_temp;
CREATE TABLE `rank_temp` (
  `RankSN` int(10) unsigned NOT NULL auto_increment,
  `UserNick` varchar(42) NOT NULL default '',
  `Exp` int(10) unsigned NOT NULL default '0',
  PRIMARY KEY  (`RankSN`),
  KEY `UserNick` (`UserNick`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table rank_temp
#----------------------------

#----------------------------
# Table structure for serverlist
#----------------------------
drop table if exists serverlist;
CREATE TABLE `serverlist` (
  `ServerGroup` smallint(2) unsigned NOT NULL default '0',
  `Num` smallint(2) unsigned NOT NULL default '0',
  `Name` varchar(16) NOT NULL default '',
  `IPAddr` varchar(16) NOT NULL default '0',
  `Port` smallint(5) unsigned NOT NULL default '0',
  `CurUser` int(10) unsigned NOT NULL default '0',
  `MaxUser` int(10) unsigned NOT NULL default '0',
  `Grade` tinyint(1) unsigned NOT NULL default '0',
  `IPRestriction` tinyint(1) unsigned NOT NULL default '0',
  `DoubleDen` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`Num`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table serverlist
#----------------------------
insert  into serverlist values (1, 101, 'RZ', '127.0.0.1', 25511, 0, 2000, 0, 0, 1) ;

#----------------------------
# Table structure for tournament
#----------------------------
drop table if exists tournament;
CREATE TABLE `tournament` (
  `TournamentSN` int(10) unsigned NOT NULL auto_increment,
  `Grade` tinyint(4) unsigned NOT NULL default '0',
  `JoinCount` smallint(6) NOT NULL default '0',
  `WinSN1` int(10) unsigned NOT NULL default '0',
  `WinSN2` int(10) unsigned NOT NULL default '0',
  `WinSN3` int(10) unsigned NOT NULL default '0',
  `Prize1` int(11) NOT NULL default '0',
  `Prize2` int(11) NOT NULL default '0',
  `Prize3` int(11) NOT NULL default '0',
  `StartDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `EndDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`TournamentSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table tournament
#----------------------------

#----------------------------
# Table structure for tournament_crown
#----------------------------
drop table if exists tournament_crown;
CREATE TABLE `tournament_crown` (
  `Grade` int(10) unsigned NOT NULL default '0',
  `UserSN` int(10) unsigned NOT NULL default '0',
  `UpdateDate` datetime NOT NULL default '0000-00-00 00:00:00'
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table tournament_crown
#----------------------------

#----------------------------
# Table structure for tournament_luckey
#----------------------------
drop table if exists tournament_luckey;
CREATE TABLE `tournament_luckey` (
  `LuckeySN` int(10) unsigned NOT NULL auto_increment,
  `GotSN` int(10) unsigned NOT NULL default '0',
  `TournamentSN` int(10) unsigned NOT NULL default '0',
  `UserSN` int(10) unsigned NOT NULL default '0',
  `LuckeyMoney` int(10) unsigned NOT NULL default '0',
  `GotDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`LuckeySN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table tournament_luckey
#----------------------------

#----------------------------
# Table structure for tournament_point
#----------------------------
drop table if exists tournament_point;
CREATE TABLE `tournament_point` (
  `PointSN` int(10) unsigned NOT NULL auto_increment,
  `TournamentSN` int(10) unsigned NOT NULL default '0',
  `UserSN` int(10) unsigned NOT NULL default '0',
  `GotPoint` int(11) NOT NULL default '0',
  `GotDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`PointSN`),
  KEY `UserSN` (`UserSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table tournament_point
#----------------------------

#----------------------------
# Table structure for tournament_rank
#----------------------------
drop table if exists tournament_rank;
CREATE TABLE `tournament_rank` (
  `Rank` int(10) unsigned NOT NULL auto_increment,
  `UserSN` int(10) unsigned NOT NULL default '0',
  `UserNick` varchar(42) NOT NULL default '',
  `Point` int(11) NOT NULL default '0',
  `WinCount` smallint(6) NOT NULL default '0',
  `TotalPoint` int(11) NOT NULL default '0',
  PRIMARY KEY  (`Rank`),
  UNIQUE KEY `UserSN` (`UserSN`,`UserNick`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table tournament_rank
#----------------------------

#----------------------------
# Table structure for tournament_user
#----------------------------
drop table if exists tournament_user;
CREATE TABLE `tournament_user` (
  `TJoinSN` int(10) unsigned NOT NULL auto_increment,
  `TournamentSN` int(10) unsigned NOT NULL default '0',
  `UserSN` int(10) unsigned NOT NULL default '0',
  `JoinDate` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`TJoinSN`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table tournament_user
#----------------------------

#----------------------------
# Table structure for user_web_cart
#----------------------------
drop table if exists user_web_cart;
CREATE TABLE `user_web_cart` (
  `nKey` int(10) unsigned NOT NULL auto_increment,
  `UserSn` int(11) unsigned NOT NULL default '0',
  `ItemIdx` smallint(5) unsigned NOT NULL default '0',
  `Flag` enum('NEW','LOCK','SPEND','BACK') NOT NULL default 'NEW',
  `UserID` varchar(20) NOT NULL default '',
  `SubType` enum('ITEM','MUSIC','DANCE') NOT NULL default 'ITEM',
  `BuyType` enum('M','G') NOT NULL default 'G',
  `Qty` smallint(5) unsigned NOT NULL default '1',
  `RegDate` datetime NOT NULL default '0000-00-00 00:00:00',
  `ReceiptDate` datetime NOT NULL default '0000-00-00 00:00:00',
  KEY `game_idx` (`UserSn`,`nKey`,`UserID`,`ItemIdx`),
  KEY `UserSn` (`UserSn`),
  KEY `Flag` (`Flag`),
  KEY `ItemIdx` (`ItemIdx`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# No records for table user_web_cart
#----------------------------

#----------------------------
# Table structure for userinfo
#----------------------------
drop table if exists userinfo;
CREATE TABLE `userinfo` (
  `UserSN` int(10) unsigned NOT NULL default '0',
  `UserNick` varchar(20) NOT NULL default '0',
  `UserID` varchar(22) NOT NULL default '0',
  `Exp` int(10) unsigned NOT NULL default '0',
  `Point` int(10) unsigned NOT NULL default '0',
  `Money` int(10) unsigned NOT NULL default '0',
  `Cash` int(10) unsigned NOT NULL default '0',
  `Level` int(7) unsigned NOT NULL default '0',
  `Ranking` int(10) unsigned NOT NULL default '1',
  `IsAllowMsg` enum('Y','N') NOT NULL default 'Y',
  `IsAllowInvite` enum('Y','N') NOT NULL default 'Y',
  `LastLoginTime` datetime NOT NULL default '0000-00-00 00:00:00',
  `Password` varchar(23) NOT NULL default '0',
  PRIMARY KEY  (`UserSN`),
  KEY `Idx_IDNick` (`UserID`,`UserNick`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table userinfo
#----------------------------
insert  into userinfo values (1, 'CiMed', 'CiMed', 0, 0, 1000000, 1000000, 1, 0, 'Y', 'Y', '2006-10-10 23:30:00', 'testau') ;

#----------------------------
# Table structure for users
#----------------------------
drop table if exists users;
CREATE TABLE `users` (
  `UserSN` int(11) NOT NULL default '0',
  `UserNick` varchar(20) default '0',
  `UserID` varchar(22) NOT NULL default '0',
  `UserName` varchar(16) NOT NULL default '0',
  `UserGender` enum('M','F') NOT NULL default 'M',
  `UserPower` tinyint(4) NOT NULL default '0',
  `UserRegion` varchar(30) NOT NULL default '',
  `UserEMail` varchar(30) NOT NULL default '',
  `RegistedTime` datetime NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`UserSN`),
  KEY `IDX_IDNick` (`UserID`,`UserNick`),
  KEY `IDX_REGISTTIME` (`RegistedTime`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#----------------------------
# Records for table users
#----------------------------
insert  into users values (1, 'CiMed', 'CiMed', 'CiMed', 'M', 0, 'Region', 'cimed@yahoo.com', '2006-10-10 23:00:00') ;
