

-- 
-- Table structure for table `files`
-- 

CREATE TABLE `files` (
  `file_id` mediumint(8) NOT NULL auto_increment,
  `file_path` mediumtext NOT NULL,
  `tune_id` mediumint(8) NOT NULL,
  PRIMARY KEY  (`file_id`),
  KEY `tune_id` (`tune_id`),
  KEY `file_path` (`file_path`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

-- 
-- Table structure for table `tunes`
-- 

CREATE TABLE `tunes` (
  `tune_id` mediumint(8) NOT NULL auto_increment,
  `tune_by` varchar(255) NOT NULL,
  `tune_title` varchar(255) NOT NULL,
  `tune_comment` varchar(255) NOT NULL,
  PRIMARY KEY  (`tune_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
