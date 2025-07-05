-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.4.32-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             12.8.0.6908
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for db_test
CREATE DATABASE IF NOT EXISTS `db_test` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */;
USE `db_test`;

-- Dumping structure for table db_test.emu_abuse_reports
CREATE TABLE IF NOT EXISTS `emu_abuse_reports` (
  `initiator` varchar(16) DEFAULT NULL,
  `target` varchar(16) NOT NULL,
  `type` varchar(12) NOT NULL,
  `comment` varchar(300) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_abuse_reports: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_achievements
CREATE TABLE IF NOT EXISTS `emu_achievements` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `achievement_id` int(10) unsigned NOT NULL,
  `progress` int(10) NOT NULL DEFAULT 0,
  `completion_time` bigint(20) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_achievements: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_anticheat_punish_mode
CREATE TABLE IF NOT EXISTS `emu_anticheat_punish_mode` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `punish_mode` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_anticheat_punish_mode: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_anticheat_report
CREATE TABLE IF NOT EXISTS `emu_anticheat_report` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `type` varchar(50) NOT NULL,
  `score` int(11) NOT NULL,
  `calls` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_anticheat_report: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_banforleave
CREATE TABLE IF NOT EXISTS `emu_banforleave` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `type` int(11) NOT NULL DEFAULT 0,
  `unban_time` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_banforleave: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_bans
CREATE TABLE IF NOT EXISTS `emu_bans` (
  `user_id` bigint(20) unsigned NOT NULL DEFAULT 0,
  `rule` varchar(100) NOT NULL,
  `unban_time` bigint(20) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_bans: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_clans
CREATE TABLE IF NOT EXISTS `emu_clans` (
  `clan_id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(16) NOT NULL,
  `description` varchar(2000) NOT NULL,
  `creation_date` int(10) unsigned NOT NULL,
  PRIMARY KEY (`clan_id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- Dumping data for table db_test.emu_clans: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_clan_members
CREATE TABLE IF NOT EXISTS `emu_clan_members` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `clan_id` bigint(20) unsigned NOT NULL,
  `clan_role` int(10) NOT NULL DEFAULT 3,
  `clan_points` int(10) NOT NULL DEFAULT 0,
  `invite_date` bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_clan_members: ~443 rows (approximately)
INSERT IGNORE INTO `emu_clan_members` (`profile_id`, `clan_id`, `clan_role`, `clan_points`, `invite_date`) VALUES
	(105, 44, 1, 14117, 1694753589),
	(108, 106, 1, 614, 1695646845),
	(109, 47, 1, 48, 1694758849),
	(111, 44, 3, 8330, 1694762625),
	(112, 48, 1, 6122, 1694762819),
	(113, 332, 1, 969, 1697810879),
	(114, 49, 1, 314, 1694763516),
	(116, 70, 3, 8338, 1697896146),
	(117, 316, 1, 134, 1697700448),
	(118, 51, 1, 24, 1694768181),
	(119, 44, 3, 944, 1695309545),
	(124, 70, 3, 1777, 1699779837),
	(126, 52, 1, 8, 1694772826),
	(127, 53, 1, 68, 1694773105),
	(128, 54, 1, 0, 1694773605),
	(131, 70, 3, 300, 1699879905),
	(132, 483, 2, 312, 1700240547),
	(134, 78, 1, 813, 1695038439),
	(135, 266, 1, 1089, 1697470391),
	(136, 332, 3, 16, 1699639770),
	(139, 70, 3, 3457, 1698164567),
	(141, 75, 1, 1664, 1694976837),
	(143, 70, 2, 17474, 1695022800),
	(146, 59, 1, 25, 1694794803),
	(147, 130, 1, 923, 1696147118),
	(148, 69, 1, 88, 1695050403),
	(149, 89, 1, 445, 1695548540),
	(150, 156, 1, 108, 1696773204),
	(153, 44, 3, 651, 1695037894),
	(154, 214, 1, 128, 1697033105),
	(155, 64, 1, 9019, 1694861694),
	(157, 62, 1, 9591, 1694842602),
	(158, 70, 3, 2168, 1695478675),
	(162, 417, 1, 140, 1699293252),
	(164, 48, 3, 0, 1694858261),
	(165, 483, 2, 453, 1699606150),
	(166, 128, 1, 696, 1696092757),
	(171, 65, 1, 213, 1694873407),
	(177, 66, 1, 170, 1694890336),
	(178, 46, 1, 24, 1694936303),
	(179, 44, 3, 4967, 1694893828),
	(182, 64, 3, 485, 1694934512),
	(183, 69, 3, 53, 1694953423),
	(184, 68, 1, 613, 1694936415),
	(186, 504, 3, 29, 1700508427),
	(187, 85, 1, 186, 1695127513),
	(189, 70, 2, 1579, 1699202091),
	(190, 332, 3, 1014, 1698482032),
	(191, 64, 2, 4658, 1695042607),
	(192, 70, 1, 23515, 1694948731),
	(195, 77, 1, 196, 1695020548),
	(198, 72, 1, 203, 1694960447),
	(200, 68, 3, 218, 1694970160),
	(203, 102, 1, 8390, 1695565786),
	(204, 70, 2, 9402, 1694980999),
	(205, 76, 1, 1736, 1695002576),
	(209, 82, 1, 3224, 1695114547),
	(210, 102, 3, 69, 1698589668),
	(211, 79, 1, 387, 1695051917),
	(212, 70, 2, 18501, 1695124762),
	(216, 417, 2, 775, 1699112343),
	(217, 79, 2, 184, 1695057230),
	(220, 70, 3, 11481, 1696771860),
	(221, 48, 3, 0, 1695122890),
	(222, 182, 1, 414, 1696779051),
	(223, 292, 2, 115, 1697640234),
	(224, 44, 3, 627, 1695127942),
	(225, 44, 3, 2457, 1695128266),
	(226, 88, 1, 383, 1695216523),
	(228, 70, 2, 5124, 1695139253),
	(229, 44, 3, 3003, 1695139185),
	(230, 70, 3, 3830, 1695140003),
	(233, 89, 2, 3413, 1695554599),
	(234, 86, 1, 581, 1695148080),
	(239, 103, 1, 125, 1695568331),
	(241, 70, 2, 18804, 1695219371),
	(242, 94, 1, 138, 1695394840),
	(244, 332, 2, 1201, 1697810887),
	(245, 70, 3, 3006, 1696677532),
	(249, 89, 2, 3494, 1695399759),
	(250, 48, 2, 1147, 1696263373),
	(251, 161, 1, 1449, 1696527510),
	(252, 92, 1, 4276, 1695311869),
	(253, 93, 1, 159, 1695316147),
	(257, 89, 2, 1357, 1695413171),
	(259, 89, 3, 671, 1695461383),
	(261, 89, 2, 2742, 1695504917),
	(265, 69, 3, 16, 1695712240),
	(267, 97, 1, 281, 1695405540),
	(269, 99, 1, 2117, 1695495555),
	(271, 265, 3, 554, 1697743979),
	(272, 89, 2, 2301, 1695574986),
	(274, 98, 1, 2, 1695486330),
	(276, 89, 2, 2075, 1695571660),
	(278, 89, 3, 2677, 1695543462),
	(279, 69, 3, 3830, 1695651755),
	(281, 72, 3, 74, 1695556258),
	(283, 89, 2, 1354, 1695575859),
	(284, 104, 2, 133, 1696350263),
	(286, 104, 1, 163, 1695573858),
	(288, 432, 1, 1653, 1698825028),
	(291, 109, 1, 60, 1695652405),
	(292, 89, 3, 791, 1695643331),
	(294, 169, 1, 32, 1696671219),
	(296, 110, 3, 136, 1695659555),
	(297, 110, 1, 278, 1695659508),
	(300, 110, 3, 102, 1695661782),
	(305, 102, 3, 260, 1697047726),
	(306, 332, 2, 846, 1697810903),
	(310, 386, 2, 4537, 1698488944),
	(313, 187, 3, 1124, 1697566394),
	(314, 102, 2, 1711, 1695742078),
	(316, 89, 2, 627, 1695744568),
	(317, 187, 2, 1400, 1697394078),
	(319, 69, 3, 32, 1695798450),
	(320, 116, 1, 22, 1695808407),
	(321, 99, 3, 39, 1696254738),
	(322, 417, 3, 472, 1699109943),
	(326, 177, 3, 59, 1699715118),
	(327, 332, 2, 315, 1697890284),
	(329, 130, 2, 146, 1696257643),
	(332, 70, 3, 1108, 1696602084),
	(334, 123, 1, 1442, 1696055070),
	(337, 504, 2, 3096, 1699886047),
	(338, 102, 2, 522, 1696342473),
	(339, 126, 1, 154, 1696092745),
	(340, 64, 2, 3480, 1696188949),
	(342, 64, 2, 1577, 1696386590),
	(344, 131, 1, 39, 1696151469),
	(346, 161, 2, 2036, 1696769993),
	(347, 142, 1, 70, 1696242255),
	(348, 132, 1, 119, 1696163677),
	(349, 135, 1, 180, 1696169898),
	(350, 149, 1, 60, 1696337360),
	(351, 141, 1, 20894, 1696213137),
	(352, 89, 3, 534, 1696251569),
	(353, 143, 1, 606, 1696251862),
	(354, 44, 3, 3098, 1696251509),
	(357, 48, 3, 500, 1696340569),
	(358, 159, 1, 46, 1696517287),
	(359, 148, 1, 126, 1696306774),
	(361, 327, 3, 158, 1697799079),
	(364, 152, 1, 209, 1696394740),
	(368, 217, 1, 222, 1697122722),
	(369, 273, 1, 276, 1697551414),
	(371, 155, 1, 8, 1696435467),
	(372, 48, 3, 143, 1697130289),
	(373, 158, 1, 127, 1696449646),
	(374, 157, 1, 244, 1696439015),
	(375, 44, 3, 355, 1697819156),
	(376, 102, 3, 88, 1696592287),
	(379, 156, 3, 3070, 1696773191),
	(380, 163, 1, 98, 1696594816),
	(383, 166, 1, 1456, 1696693277),
	(386, 427, 1, 0, 1698768278),
	(388, 172, 1, 162, 1696694140),
	(389, 173, 1, 146, 1696711805),
	(390, 102, 3, 579, 1698923079),
	(391, 174, 1, 153, 1696719012),
	(393, 229, 1, 472, 1697188268),
	(394, 274, 2, 828, 1698497583),
	(395, 156, 3, 49, 1696871063),
	(396, 177, 1, 4914, 1696758315),
	(397, 176, 1, 1235, 1696757243),
	(399, 470, 1, 66, 1699274080),
	(401, 229, 3, 333, 1697217375),
	(405, 394, 1, 29, 1698412004),
	(407, 475, 1, 27, 1699358841),
	(408, 185, 1, 160, 1696793856),
	(409, 187, 1, 3247, 1696799101),
	(411, 421, 3, 690, 1699207966),
	(414, 70, 3, 5274, 1699294371),
	(416, 376, 1, 123, 1698230540),
	(420, 156, 3, 79, 1697125233),
	(421, 190, 1, 838, 1696864399),
	(422, 325, 3, 332, 1698072639),
	(427, 226, 1, 1023, 1697119561),
	(429, 45, 1, 48, 1697112685),
	(430, 461, 2, 949, 1699288693),
	(431, 438, 1, 80, 1698856085),
	(434, 48, 3, 104, 1696929452),
	(436, 256, 3, 8, 1697957220),
	(437, 211, 1, 361, 1697031462),
	(438, 196, 1, 16, 1696934326),
	(439, 197, 1, 78, 1696938722),
	(440, 187, 2, 1133, 1697465378),
	(443, 292, 1, 260, 1697636196),
	(449, 208, 1, 88, 1696975910),
	(451, 378, 1, 597, 1698241135),
	(452, 48, 3, 982, 1697377545),
	(454, 187, 2, 3031, 1697046058),
	(455, 297, 1, 0, 1697640014),
	(458, 278, 1, 83, 1697618699),
	(461, 274, 1, 2234, 1697558692),
	(462, 221, 1, 90, 1697114864),
	(466, 227, 1, 0, 1697120752),
	(471, 232, 1, 423, 1697212299),
	(472, 265, 3, 298, 1698007289),
	(474, 308, 1, 220, 1697644519),
	(475, 231, 1, 356, 1697201497),
	(478, 229, 3, 490, 1697191114),
	(479, 505, 1, 513, 1699888715),
	(485, 70, 2, 1366, 1699724420),
	(487, 252, 1, 3420, 1697313601),
	(488, 237, 1, 846, 1697229490),
	(489, 237, 2, 830, 1697230768),
	(491, 238, 1, 96, 1697247114),
	(493, 239, 1, 290, 1697270024),
	(495, 64, 2, 3254, 1697569904),
	(496, 187, 3, 223, 1700508080),
	(497, 102, 3, 607, 1697275891),
	(503, 321, 1, 974, 1697732993),
	(504, 245, 1, 138, 1697296453),
	(505, 48, 3, 577, 1697312813),
	(506, 273, 3, 264, 1697553429),
	(508, 262, 1, 32, 1697407407),
	(510, 509, 1, 0, 1700048767),
	(511, 310, 1, 130, 1697648459),
	(513, 265, 1, 10322, 1697450854),
	(515, 102, 2, 10, 1698939664),
	(517, 256, 1, 206, 1697376361),
	(521, 392, 3, 27, 1698409433),
	(522, 253, 1, 0, 1697323646),
	(527, 265, 3, 367, 1697470267),
	(530, 254, 1, 0, 1697364995),
	(532, 265, 3, 1398, 1697731924),
	(534, 265, 3, 1033, 1697453469),
	(535, 187, 2, 2063, 1697377176),
	(536, 257, 1, 204, 1697386185),
	(538, 259, 1, 384, 1697388822),
	(539, 265, 3, 325, 1697460438),
	(540, 267, 1, 0, 1697470987),
	(542, 504, 2, 1133, 1699885986),
	(544, 311, 1, 303, 1697649272),
	(547, 261, 1, 65, 1697396233),
	(548, 249, 1, 733, 1697396930),
	(549, 48, 3, 632, 1697644017),
	(552, 265, 2, 5685, 1697461137),
	(553, 386, 3, 418, 1698653564),
	(556, 268, 1, 587, 1697471674),
	(560, 187, 2, 433, 1697471809),
	(563, 504, 1, 5116, 1699885975),
	(567, 271, 1, 77, 1697485131),
	(568, 483, 1, 1544, 1699564636),
	(569, 265, 3, 2220, 1697488483),
	(571, 48, 2, 0, 1697529033),
	(574, 48, 3, 0, 1697532354),
	(577, 511, 1, 225, 1700056463),
	(579, 362, 1, 349, 1698064918),
	(580, 334, 1, 1036, 1697821201),
	(583, 514, 1, 10, 1700245973),
	(584, 461, 3, 16, 1699712506),
	(585, 277, 1, 386, 1697618068),
	(592, 265, 3, 1273, 1697639051),
	(594, 291, 1, 613, 1697636009),
	(595, 290, 1, 165, 1697635984),
	(598, 310, 3, 130, 1697648465),
	(599, 332, 2, 1530, 1697810909),
	(600, 332, 3, 1129, 1698676828),
	(604, 417, 3, 416, 1699280914),
	(605, 314, 1, 0, 1697689601),
	(607, 390, 1, 34, 1698340948),
	(608, 386, 1, 0, 1698918675),
	(609, 313, 1, 0, 1697727806),
	(610, 265, 3, 883, 1697719113),
	(614, 325, 2, 3547, 1697736585),
	(616, 325, 1, 2405, 1697736345),
	(619, 326, 1, 0, 1697737372),
	(627, 404, 1, 386, 1698440086),
	(630, 327, 1, 110, 1697799072),
	(631, 404, 3, 326, 1698440118),
	(634, 332, 3, 149, 1697974776),
	(635, 417, 2, 322, 1698950274),
	(636, 404, 3, 277, 1698440174),
	(638, 335, 1, 211, 1697875447),
	(640, 336, 1, 77, 1697886879),
	(653, 420, 1, 458, 1698686414),
	(655, 345, 1, 34, 1697914534),
	(659, 364, 1, 1463, 1698091941),
	(665, 265, 3, 238, 1697995939),
	(666, 435, 1, 0, 1698838105),
	(667, 352, 1, 166, 1697981550),
	(668, 102, 2, 1667, 1698417217),
	(671, 462, 1, 94, 1699196175),
	(672, 417, 2, 511, 1698674866),
	(673, 374, 1, 993, 1698227454),
	(674, 359, 1, 71, 1698000833),
	(676, 70, 3, 4261, 1699193957),
	(683, 64, 3, 305, 1698436038),
	(685, 363, 1, 118, 1698074491),
	(689, 504, 2, 2197, 1699967212),
	(690, 265, 3, 130, 1698180653),
	(694, 365, 1, 86, 1698093449),
	(695, 332, 3, 16, 1698485575),
	(697, 321, 2, 35, 1698164486),
	(698, 70, 3, 3359, 1699274440),
	(699, 332, 3, 1236, 1698666073),
	(700, 102, 2, 1445, 1698934427),
	(702, 421, 1, 1031, 1698694878),
	(703, 381, 1, 50, 1698309692),
	(709, 421, 2, 1123, 1698694889),
	(714, 413, 3, 0, 1698739655),
	(715, 385, 1, 137, 1698328737),
	(717, 392, 1, 234, 1698343743),
	(718, 265, 3, 0, 1698344876),
	(719, 391, 1, 114, 1698343412),
	(722, 265, 3, 161, 1698345599),
	(725, 398, 1, 1176, 1698415907),
	(726, 398, 2, 1246, 1698415919),
	(728, 398, 2, 466, 1698426629),
	(729, 398, 2, 950, 1698426765),
	(737, 398, 2, 753, 1698503110),
	(739, 405, 1, 495, 1698446305),
	(741, 507, 1, 489, 1699986665),
	(742, 332, 3, 32, 1698482016),
	(744, 474, 1, 0, 1699357461),
	(746, 398, 2, 1032, 1698503180),
	(748, 408, 1, 0, 1698517196),
	(754, 102, 3, 1198, 1698948828),
	(755, 413, 1, 397, 1698601616),
	(756, 274, 2, 122, 1698573620),
	(761, 447, 3, 655, 1699049089),
	(763, 447, 3, 422, 1699049099),
	(764, 102, 3, 861, 1699374918),
	(766, 412, 1, 225, 1698597472),
	(767, 412, 3, 14, 1698597485),
	(768, 416, 1, 892, 1698670011),
	(769, 325, 3, 1831, 1698606703),
	(770, 480, 1, 804, 1699529087),
	(772, 102, 3, 113, 1698670690),
	(773, 102, 3, 24, 1698948883),
	(774, 415, 1, 56, 1698623388),
	(777, 421, 3, 316, 1698739942),
	(778, 102, 3, 66, 1698670693),
	(780, 416, 2, 434, 1698758544),
	(782, 102, 3, 0, 1698926923),
	(784, 423, 1, 4380, 1698726766),
	(786, 102, 2, 772, 1698998487),
	(789, 433, 1, 1420, 1698837335),
	(791, 102, 3, 200, 1698759956),
	(792, 483, 3, 264, 1699893649),
	(793, 426, 1, 906, 1698766663),
	(794, 441, 1, 154, 1698917013),
	(795, 447, 1, 696, 1699003286),
	(796, 416, 2, 580, 1698780233),
	(798, 426, 2, 17, 1698849751),
	(799, 439, 3, 625, 1698868251),
	(800, 439, 1, 1100, 1698868177),
	(801, 451, 1, 750, 1699025833),
	(802, 430, 1, 16, 1698830152),
	(803, 386, 3, 272, 1698834048),
	(804, 437, 1, 8, 1698840939),
	(805, 461, 2, 1702, 1699193459),
	(807, 461, 1, 1848, 1699193426),
	(808, 416, 3, 657, 1699126321),
	(809, 483, 3, 890, 1699566454),
	(810, 439, 3, 512, 1698876769),
	(811, 439, 2, 889, 1698868843),
	(814, 440, 1, 0, 1698876826),
	(816, 102, 3, 238, 1698952832),
	(818, 102, 3, 287, 1699442315),
	(822, 432, 3, 66, 1699208726),
	(823, 102, 2, 362, 1698934322),
	(825, 446, 1, 16, 1698967061),
	(826, 439, 3, 302, 1698950644),
	(828, 414, 1, 314, 1698956595),
	(834, 417, 2, 329, 1699189093),
	(835, 450, 1, 0, 1699019345),
	(837, 417, 2, 160, 1699552597),
	(843, 141, 3, 1250, 1699968847),
	(844, 480, 3, 57, 1699804826),
	(845, 274, 3, 235, 1699078357),
	(846, 141, 2, 2965, 1699365418),
	(847, 374, 2, 1214, 1699369054),
	(853, 102, 3, 16, 1699114911),
	(855, 272, 1, 750, 1699132318),
	(864, 461, 2, 1313, 1699196134),
	(865, 141, 3, 125, 1699291343),
	(866, 141, 2, 600, 1699283909),
	(869, 461, 2, 203, 1699218799),
	(870, 265, 3, 70, 1699256623),
	(875, 468, 1, 382, 1699273017),
	(876, 467, 1, 137, 1699269012),
	(877, 468, 2, 195, 1699273471),
	(883, 461, 3, 230, 1699288061),
	(884, 461, 2, 1038, 1699288714),
	(886, 265, 3, 116, 1699551324),
	(888, 102, 3, 35, 1699356256),
	(890, 508, 1, 668, 1700036105),
	(896, 478, 1, 56, 1699436066),
	(898, 417, 3, 514, 1699446734),
	(899, 177, 2, 515, 1699455603),
	(900, 177, 2, 661, 1699458067),
	(902, 504, 3, 68, 1700214796),
	(903, 265, 3, 66, 1699533334),
	(905, 265, 3, 47, 1699529113),
	(907, 493, 1, 98, 1699696359),
	(908, 493, 2, 83, 1699696368),
	(909, 265, 3, 347, 1699542424),
	(912, 498, 3, 9, 1699799304),
	(916, 44, 3, 16, 1699630140),
	(917, 426, 2, 587, 1699790914),
	(918, 70, 3, 128, 1699802269),
	(919, 480, 3, 224, 1699799405),
	(920, 504, 2, 2179, 1699970291),
	(922, 426, 2, 368, 1699790916),
	(926, 332, 3, 13, 1699713116),
	(930, 480, 2, 16, 1699713132),
	(933, 498, 1, 0, 1699796651),
	(934, 426, 2, 290, 1699802151),
	(935, 515, 3, 94, 1700383096),
	(936, 506, 1, 40, 1699978038),
	(938, 500, 1, 453, 1699822551),
	(943, 480, 3, 923, 1699886372),
	(944, 504, 3, 506, 1700226456),
	(945, 519, 1, 642, 1700408000),
	(948, 504, 3, 510, 1700213950),
	(949, 504, 3, 0, 1699980205),
	(950, 524, 2, 566, 1700504900),
	(951, 504, 3, 0, 1700048687),
	(954, 510, 1, 0, 1700049944),
	(956, 504, 3, 1610, 1700053871),
	(957, 504, 3, 0, 1700058529),
	(959, 507, 3, 438, 1700079932),
	(960, 507, 2, 127, 1700077362),
	(962, 504, 3, 70, 1700419850),
	(967, 504, 3, 54, 1700223640),
	(970, 504, 3, 129, 1700421174),
	(976, 141, 3, 60, 1700225091),
	(978, 504, 3, 421, 1700218818),
	(988, 515, 1, 385, 1700314491),
	(989, 516, 1, 392, 1700321888),
	(990, 516, 2, 344, 1700321899),
	(993, 504, 3, 542, 1700319343),
	(994, 70, 3, 145, 1700320314),
	(997, 274, 3, 135, 1700335741),
	(998, 504, 3, 11, 1700377598),
	(999, 500, 2, 253, 1700331775),
	(1003, 504, 3, 32, 1700475323),
	(1012, 525, 1, 354, 1700516152),
	(1014, 521, 1, 137, 1700493077),
	(1015, 519, 3, 0, 1700422349),
	(1022, 524, 1, 621, 1700504886);

-- Dumping structure for table db_test.emu_connects
CREATE TABLE IF NOT EXISTS `emu_connects` (
  `user_id` bigint(20) unsigned NOT NULL,
  `ipaddress` varchar(16) NOT NULL,
  UNIQUE KEY `user_id` (`user_id`,`ipaddress`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_connects: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_donationalerts
CREATE TABLE IF NOT EXISTS `emu_donationalerts` (
  `ID` int(11) NOT NULL,
  `username` text CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `message` text CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `amount` int(11) DEFAULT NULL,
  `created_at_ts` int(255) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Dumping data for table db_test.emu_donationalerts: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_dynamic_multipliers
CREATE TABLE IF NOT EXISTS `emu_dynamic_multipliers` (
  `name` varchar(50) NOT NULL,
  `multiplier` int(11) DEFAULT NULL,
  `cry_money_finalzile_pvp` int(11) DEFAULT 0,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_dynamic_multipliers: ~0 rows (approximately)
INSERT IGNORE INTO `emu_dynamic_multipliers` (`name`, `multiplier`, `cry_money_finalzile_pvp`) VALUES
	('TEST X5', 5, 54535);

-- Dumping structure for table db_test.emu_friends
CREATE TABLE IF NOT EXISTS `emu_friends` (
  `first_id` bigint(20) unsigned NOT NULL,
  `second_id` bigint(20) unsigned NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- Dumping data for table db_test.emu_friends: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_items
CREATE TABLE IF NOT EXISTS `emu_items` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `profile_id` bigint(20) unsigned NOT NULL,
  `type` tinyint(4) unsigned NOT NULL DEFAULT 0,
  `name` varchar(50) NOT NULL,
  `config` varchar(50) NOT NULL DEFAULT 'dm=0;material=default',
  `attached_to` tinyint(1) unsigned NOT NULL DEFAULT 0,
  `slot` int(11) NOT NULL DEFAULT 0,
  `equipped` int(10) NOT NULL DEFAULT 0,
  `expired_confirmed` tinyint(1) NOT NULL DEFAULT 0,
  `buy_time_utc` bigint(20) NOT NULL DEFAULT 0,
  `expiration_time_utc` bigint(20) NOT NULL DEFAULT 0,
  `quantity` int(11) NOT NULL DEFAULT -1,
  `total_durability_points` mediumint(9) NOT NULL DEFAULT -1,
  `durability_points` mediumint(9) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- Dumping data for table db_test.emu_items: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_login_bonus
CREATE TABLE IF NOT EXISTS `emu_login_bonus` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `current_streak` tinyint(3) NOT NULL DEFAULT 0,
  `current_reward` tinyint(3) NOT NULL DEFAULT -1,
  `last_seen_reward` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_login_bonus: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_mutes
CREATE TABLE IF NOT EXISTS `emu_mutes` (
  `user_id` bigint(20) NOT NULL DEFAULT 0,
  `rule` varchar(5) NOT NULL,
  `unmute_time` bigint(20) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_mutes: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_news
CREATE TABLE IF NOT EXISTS `emu_news` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Title` text CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `Descriptions` text NOT NULL,
  `Icon` text NOT NULL,
  `Status` text NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Dumping data for table db_test.emu_news: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_notifications
CREATE TABLE IF NOT EXISTS `emu_notifications` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `profile_id` bigint(20) unsigned NOT NULL DEFAULT 0,
  `type` mediumint(7) NOT NULL,
  `expiration_time_utc` bigint(20) NOT NULL DEFAULT 0,
  `confirmation` tinyint(1) NOT NULL DEFAULT 0,
  `data` varchar(1000) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_notifications: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_pc_info
CREATE TABLE IF NOT EXISTS `emu_pc_info` (
  `profile_id` bigint(20) unsigned NOT NULL DEFAULT 0,
  `hwid` int(11) NOT NULL,
  `os_64` tinyint(4) NOT NULL,
  `os_ver` tinyint(4) NOT NULL,
  `gpu_device_id` smallint(6) NOT NULL DEFAULT 0,
  `gpu_vendor_id` smallint(6) NOT NULL DEFAULT 0,
  `cpu_model` tinyint(4) NOT NULL DEFAULT 0,
  `cpu_family` tinyint(4) NOT NULL DEFAULT 0,
  `cpu_vendor` tinyint(4) NOT NULL DEFAULT 0,
  `cpu_num_cores` tinyint(4) NOT NULL DEFAULT 0,
  `cpu_stepping` tinyint(4) NOT NULL DEFAULT 0,
  `physical_memory` mediumint(9) NOT NULL DEFAULT 0,
  PRIMARY KEY (`profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_pc_info: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_persistent_settings
CREATE TABLE IF NOT EXISTS `emu_persistent_settings` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `type` varchar(10) NOT NULL,
  `name` varchar(50) NOT NULL,
  `value` varchar(50) NOT NULL,
  UNIQUE KEY `s_key` (`profile_id`,`type`,`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_persistent_settings: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_pin_codes
CREATE TABLE IF NOT EXISTS `emu_pin_codes` (
  `pin` varchar(30) NOT NULL,
  `ammount` int(11) NOT NULL DEFAULT 0,
  `reward` text NOT NULL,
  UNIQUE KEY `pin` (`pin`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_pin_codes: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_pin_used
CREATE TABLE IF NOT EXISTS `emu_pin_used` (
  `profile_id` int(11) NOT NULL,
  `pin` varchar(30) NOT NULL,
  UNIQUE KEY `unique` (`profile_id`,`pin`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_pin_used: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_profiles
CREATE TABLE IF NOT EXISTS `emu_profiles` (
  `profile_id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `user_id` bigint(20) unsigned NOT NULL,
  `nickname` varchar(16) NOT NULL,
  `head` varchar(16) NOT NULL DEFAULT 'default_head_01',
  `experience` int(11) NOT NULL DEFAULT 0,
  `exp_freezed` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `current_class` int(10) NOT NULL DEFAULT 0,
  `height` int(10) NOT NULL DEFAULT 1,
  `fatness` int(10) NOT NULL DEFAULT 0,
  `banner_badge` int(10) unsigned NOT NULL DEFAULT 4294967295,
  `banner_mark` int(10) unsigned NOT NULL DEFAULT 4294967295,
  `banner_stripe` int(10) unsigned NOT NULL DEFAULT 4294967295,
  `game_money` int(11) NOT NULL DEFAULT 250000,
  `cry_money` int(11) NOT NULL DEFAULT 50000,
  `crown_money` int(11) NOT NULL DEFAULT 100000,
  `last_seen_date` bigint(20) NOT NULL,
  `locked_experience` bit(1) NOT NULL DEFAULT b'0',
  `register_data` bigint(20) NOT NULL,
  `locked_stats` int(11) DEFAULT 0,
  PRIMARY KEY (`profile_id`),
  UNIQUE KEY `nickname` (`nickname`),
  UNIQUE KEY `user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- Dumping data for table db_test.emu_profiles: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_profile_bans
CREATE TABLE IF NOT EXISTS `emu_profile_bans` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `room_type` tinyint(4) unsigned NOT NULL,
  `ban_type` tinyint(4) unsigned NOT NULL,
  `unban_utc` bigint(20) NOT NULL,
  `untrial_utc` bigint(20) NOT NULL,
  `last_ban_index` tinyint(4) unsigned NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_profile_bans: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_profile_progression_state
CREATE TABLE IF NOT EXISTS `emu_profile_progression_state` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `mission_unlocked` varchar(600) NOT NULL DEFAULT 'trainingmission,easymission,normalmission,hardmission,zombieeasy,zombienormal,zombiehard,survivalmission,campaignsections,campaignsection1,campaignsection2,campaignsection3,volcanoeasy,volcanonormal,volcanohard,volcanosurvival,anubiseasy,anubisnormal,anubishard,anubiseasy2,anubisnormal2,anubishard2,zombietowereasy,zombietowernormal,zombietowerhard,icebreakereasy,icebreakernormal,icebreakerhard,chernobyleasy,chernobylnormal,chernobylhard,japaneasy,japannormal,japanhard,marseasy,marsnormal,marshard,blackwood,pve_arena',
  `tutorial_unlocked` tinyint(3) unsigned NOT NULL DEFAULT 1,
  `tutorial_passed` tinyint(2) unsigned NOT NULL DEFAULT 1,
  `class_unlocked` tinyint(2) unsigned NOT NULL DEFAULT 31,
  PRIMARY KEY (`profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_profile_progression_state: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_pvp_rating
CREATE TABLE IF NOT EXISTS `emu_pvp_rating` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `rank` int(11) NOT NULL DEFAULT 0,
  `max_rank` int(11) NOT NULL DEFAULT 0,
  `games_history` varchar(32) NOT NULL DEFAULT '',
  PRIMARY KEY (`profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_pvp_rating: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_qiwi
CREATE TABLE IF NOT EXISTS `emu_qiwi` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `messageId` text DEFAULT NULL,
  `INVOICE_UID` text DEFAULT NULL,
  `amount` int(11) DEFAULT NULL,
  `account` text DEFAULT NULL,
  `date` text DEFAULT NULL,
  `status` text DEFAULT NULL,
  `oplata` int(11) DEFAULT 0,
  `user_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=171 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Dumping data for table db_test.emu_qiwi: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_random_boxes
CREATE TABLE IF NOT EXISTS `emu_random_boxes` (
  `profile_id` int(11) DEFAULT NULL,
  `name` varchar(50) DEFAULT NULL,
  `value` bigint(20) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Dumping data for table db_test.emu_random_boxes: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_sponsors
CREATE TABLE IF NOT EXISTS `emu_sponsors` (
  `profile_id` bigint(20) unsigned NOT NULL,
  `sponsor_id` tinyint(3) unsigned NOT NULL,
  `sponsor_points` tinyint(3) unsigned NOT NULL,
  `next_unlock_item` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_sponsors: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_stats
CREATE TABLE IF NOT EXISTS `emu_stats` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `profile_id` bigint(20) unsigned NOT NULL,
  `stat` varchar(50) NOT NULL,
  `class` tinyint(4) unsigned DEFAULT NULL,
  `mode` tinyint(4) unsigned DEFAULT NULL,
  `difficulty` varchar(25) DEFAULT NULL,
  `item_type` varchar(50) DEFAULT NULL,
  `value` bigint(20) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE KEY `s_key` (`profile_id`,`stat`,`class`,`mode`,`difficulty`,`item_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_stats: ~0 rows (approximately)

-- Dumping structure for table db_test.emu_users
CREATE TABLE IF NOT EXISTS `emu_users` (
  `user_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `vk_id` int(11) NOT NULL,
  `login` varchar(64) NOT NULL,
  `password` varchar(64) NOT NULL,
  `token` varchar(500) NOT NULL,
  `cry_token` varchar(64) NOT NULL,
  `ipaddress` varchar(16) NOT NULL,
  `balance` int(11) NOT NULL,
  `first_name` varchar(50) NOT NULL DEFAULT 'No',
  `last_name` varchar(50) NOT NULL DEFAULT 'Name',
  `photo_200` varchar(500) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT 'https://vk.com/images/camera_200.png',
  `activated` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `vk_id` (`vk_id`,`login`)
) ENGINE=InnoDB AUTO_INCREMENT=100819 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table db_test.emu_users: ~1 rows (approximately)
INSERT IGNORE INTO `emu_users` (`user_id`, `vk_id`, `login`, `password`, `token`, `cry_token`, `ipaddress`, `balance`, `first_name`, `last_name`, `photo_200`, `activated`) VALUES
	(11, 335030918, '', '', 'vk1.a.MLJWyriohHLFUdSVhwQgN0WBomeTV6SGRSrb9lPPEtBluwbjLjhBcN4ZO6bhQ2uzgcK_3VFfFk1T4AlpjxlUxVNC2lncQyYrbLo3t89UmKhA3k1ioUj5-t8RJzVdoqOIUHRYCofmpc7iguoFxgBiI-4yWnBxltTuqcSWhKWaynPsomxFaR_BcPm215HtSBS4', 'e6c1e77d-9815-46c8-99e1-c5a2a2a9e275', '87.225.116.216', 50272, 'Данила', 'Балакин\'', 'https://sun1-21.userapi.com/s/v1/ig2/dPKYYpQRL87LycYA9VJ1U-sBpFWA7FyXWqUTDiL-rLFO89dDHto2CAPYxZfQDAZ6fTstjX2876axThTKiRGdpz4Q.jpg?size=200x200&quality=95&crop=5,0,1074,1074&ava=1', 1);

-- Dumping structure for table db_test.launcher_market
CREATE TABLE IF NOT EXISTS `launcher_market` (
  `ID` bigint(20) NOT NULL DEFAULT 0,
  `Type` int(11) DEFAULT 0,
  `name` varchar(50) DEFAULT NULL,
  `count` bigint(20) DEFAULT 0,
  `Rub` bigint(20) DEFAULT 1,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Dumping data for table db_test.launcher_market: ~0 rows (approximately)

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
