-- MySQL dump 10.13  Distrib 5.7.20, for Win64 (x86_64)
--
-- Host: localhost    Database: gamedatabase
-- ------------------------------------------------------
-- Server version	5.7.20-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `base_modifiers`
--

DROP TABLE IF EXISTS `base_modifiers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `base_modifiers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `KineticRange` double NOT NULL,
  `LaserRange` double NOT NULL,
  `MissileRange` double NOT NULL,
  `KineticPD` double NOT NULL,
  `KineticShield` double NOT NULL,
  `KineticIF` double NOT NULL,
  `LaserPD` double NOT NULL,
  `LaserShield` double NOT NULL,
  `LaserIF` double NOT NULL,
  `MissilePD` double NOT NULL,
  `MissileShield` double NOT NULL,
  `MissileIF` double NOT NULL,
  `BaseShipStatsExpModifier` double NOT NULL,
  `FleetSizeExpModifier` double NOT NULL,
  `BaseFleetMaxSize` int(11) NOT NULL,
  `MaxAbsoluteFleetSize` int(11) NOT NULL,
  `MaxShipExp` int(11) NOT NULL,
  `MaxShipsInLine` int(11) NOT NULL,
  `MaxFleetsPerPlayer` int(11) NOT NULL,
  `MaxShipsPerPlayer` int(11) NOT NULL,
  `StartingMoney` int(11) NOT NULL,
  `ExpForVictory` int(11) NOT NULL,
  `ExpForLoss` int(11) NOT NULL,
  `MoneyForVictory` int(11) NOT NULL,
  `MoneyForLoss` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `base_modifiers`
--

LOCK TABLES `base_modifiers` WRITE;
/*!40000 ALTER TABLE `base_modifiers` DISABLE KEYS */;
INSERT INTO `base_modifiers` VALUES (1,1,0.5,0,2,5,1.2,0,1.5,2,8,1,1.2,0.002,0.5,1000,5000,1000,5,8,150,1000,20,10,80,40);
/*!40000 ALTER TABLE `base_modifiers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `defence_systems`
--

DROP TABLE IF EXISTS `defence_systems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `defence_systems` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) DEFAULT NULL,
  `DefenceValue` double NOT NULL,
  `SystemType` int(11) NOT NULL,
  `DefAgainstKinetic` double NOT NULL,
  `DefAgainstLaser` double NOT NULL,
  `DefAgainstMissile` double NOT NULL,
  `Faction_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Faction_Id` (`Faction_Id`) USING HASH,
  CONSTRAINT `FK_defence_systems_Factions_Faction_Id` FOREIGN KEY (`Faction_Id`) REFERENCES `factions` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `defence_systems`
--

LOCK TABLES `defence_systems` WRITE;
/*!40000 ALTER TABLE `defence_systems` DISABLE KEYS */;
INSERT INTO `defence_systems` VALUES (1,'Standard PD',80,1,1.5,0,3,1),(2,'Heavy PD',120,1,1.5,0,3,1),(3,'Light PD',40,1,1.5,0,3,1),(4,'Standard Shield',60,2,3.2,1,1.2,1),(5,'Multilayer Shield',150,2,3.4,1.4,1.3,1),(6,'Light Shield',30,2,3.2,1.05,1.25,1),(7,'Standard IF',45,3,1.2,2.1,1.2,1),(8,'High Power IF',120,3,1.2,2.1,1.2,1),(9,'Light IF',22.5,3,1.2,2.1,1.2,1),(10,'Standard IF',50,3,1.2,2,1.2,2),(11,'Smart IF',100,3,1.6,2.4,1.6,2),(12,'Light IF',25,3,1.2,2,1.2,2),(13,'Standard Shield',65,2,3,1.2,1.1,2),(14,'Heavy Shield',165,2,3.1,1.2,1.2,2),(15,'Light Shield',32.5,2,3.1,1.2,1.1,2),(16,'Standard PD',90,1,1.4,0,2.8,2),(17,'Massed PD',145,1,1.4,0,2.8,2),(18,'Light PD',45,1,1.4,0,2.8,2);
/*!40000 ALTER TABLE `defence_systems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `factions`
--

DROP TABLE IF EXISTS `factions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `factions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `factions`
--

LOCK TABLES `factions` WRITE;
/*!40000 ALTER TABLE `factions` DISABLE KEYS */;
INSERT INTO `factions` VALUES (1,'Empire'),(2,'Alliance'),(3,'Union');
/*!40000 ALTER TABLE `factions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fleets`
--

DROP TABLE IF EXISTS `fleets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fleets` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Owner_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Owner_Id` (`Owner_Id`) USING HASH,
  CONSTRAINT `FK_fleets_players_Owner_Id` FOREIGN KEY (`Owner_Id`) REFERENCES `players` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fleets`
--

LOCK TABLES `fleets` WRITE;
/*!40000 ALTER TABLE `fleets` DISABLE KEYS */;
/*!40000 ALTER TABLE `fleets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fleets_ships`
--

DROP TABLE IF EXISTS `fleets_ships`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fleets_ships` (
  `FleetID` int(11) NOT NULL,
  `ShipID` int(11) NOT NULL,
  PRIMARY KEY (`FleetID`,`ShipID`),
  KEY `IX_FleetID` (`FleetID`) USING HASH,
  KEY `IX_ShipID` (`ShipID`) USING HASH,
  CONSTRAINT `FK_Fleets_Ships_fleets_FleetID` FOREIGN KEY (`FleetID`) REFERENCES `fleets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_Fleets_Ships_ships_ShipID` FOREIGN KEY (`ShipID`) REFERENCES `ships` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fleets_ships`
--

LOCK TABLES `fleets_ships` WRITE;
/*!40000 ALTER TABLE `fleets_ships` DISABLE KEYS */;
/*!40000 ALTER TABLE `fleets_ships` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `game_history`
--

DROP TABLE IF EXISTS `game_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `game_history` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `WasDraw` tinyint(1) NOT NULL,
  `GameDate` datetime NOT NULL,
  `Loser_Id` int(11) DEFAULT NULL,
  `LoserFleet_Id` int(11) DEFAULT NULL,
  `Winner_Id` int(11) DEFAULT NULL,
  `WinnerFleet_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Loser_Id` (`Loser_Id`) USING HASH,
  KEY `IX_LoserFleet_Id` (`LoserFleet_Id`) USING HASH,
  KEY `IX_Winner_Id` (`Winner_Id`) USING HASH,
  KEY `IX_WinnerFleet_Id` (`WinnerFleet_Id`) USING HASH,
  CONSTRAINT `FK_Game_History_fleets_LoserFleet_Id` FOREIGN KEY (`LoserFleet_Id`) REFERENCES `fleets` (`Id`),
  CONSTRAINT `FK_Game_History_fleets_WinnerFleet_Id` FOREIGN KEY (`WinnerFleet_Id`) REFERENCES `fleets` (`Id`),
  CONSTRAINT `FK_Game_History_players_Loser_Id` FOREIGN KEY (`Loser_Id`) REFERENCES `players` (`Id`),
  CONSTRAINT `FK_Game_History_players_Winner_Id` FOREIGN KEY (`Winner_Id`) REFERENCES `players` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `game_history`
--

LOCK TABLES `game_history` WRITE;
/*!40000 ALTER TABLE `game_history` DISABLE KEYS */;
/*!40000 ALTER TABLE `game_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lootboxes`
--

DROP TABLE IF EXISTS `lootboxes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lootboxes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Cost` int(11) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `CommonChance` double NOT NULL,
  `RareChance` double NOT NULL,
  `VeryRareChance` double NOT NULL,
  `LegendaryChance` double NOT NULL,
  `NumberOfShips` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lootboxes`
--

LOCK TABLES `lootboxes` WRITE;
/*!40000 ALTER TABLE `lootboxes` DISABLE KEYS */;
INSERT INTO `lootboxes` VALUES (1,200,'basic lootbox',0.5,0.3,0.15,0.05,2),(2,450,'better lootbox',0.4,0.4,0.15,0.05,4),(3,1000,'supreme lootbox',0.1,0.3,0.4,0.2,4);
/*!40000 ALTER TABLE `lootboxes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `players`
--

DROP TABLE IF EXISTS `players`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `players` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(32) DEFAULT NULL,
  `Password` varchar(256) DEFAULT NULL,
  `Experience` int(11) NOT NULL,
  `GamesPlayed` int(11) NOT NULL,
  `GamesWon` int(11) NOT NULL,
  `Money` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `IsAdmin` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Username` (`Username`) USING HASH
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `players`
--

LOCK TABLES `players` WRITE;
/*!40000 ALTER TABLE `players` DISABLE KEYS */;
INSERT INTO `players` VALUES (1,'admin','TmdyY8CzeU1PrwWDUNAZFvdkm/ykxoeq/UOqpfyKkTn6WsQdiBbQP6UaN5S9ssiQ',0,0,0,1000,1,1);
/*!40000 ALTER TABLE `players` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ship_templates`
--

DROP TABLE IF EXISTS `ship_templates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ship_templates` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) DEFAULT NULL,
  `Cost` int(11) NOT NULL,
  `Evasion` double NOT NULL,
  `Hp` double NOT NULL,
  `Size` double NOT NULL,
  `Armor` double NOT NULL,
  `ExpUnlock` int(11) NOT NULL,
  `ShipRarity` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Faction_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Faction_Id` (`Faction_Id`) USING HASH,
  CONSTRAINT `FK_ship_templates_Factions_Faction_Id` FOREIGN KEY (`Faction_Id`) REFERENCES `factions` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ship_templates`
--

LOCK TABLES `ship_templates` WRITE;
/*!40000 ALTER TABLE `ship_templates` DISABLE KEYS */;
INSERT INTO `ship_templates` VALUES (1,'York class Cruiser',90,0.6,500,3,70,0,1,1,1),(2,'Suffolk class Cruiser',100,0.6,480,3.2,65,0,3,1,1),(3,'London class Heavy Cruiser',150,0.5,700,4.5,90,0,2,1,1),(4,'Las Vegas class Destroyer',40,1,340,1.5,45,0,1,1,1),(5,'Washington class Battleship',500,0.1,1200,9.5,150,20,4,1,1),(6,'San Diego class Light Cruiser',70,0.8,425,2.3,55,20,1,1,1),(7,'Enterprise class Missile Ship',600,0.2,1000,9,100,30,4,1,1),(8,'San Francisco class Battlecruiser',300,0.22,900,7,100,0,2,1,1),(9,'Paris class Destroyer',45,1,350,1.6,50,0,1,1,1),(10,'Indianapolis class Light Cruiser',75,0.8,450,2.5,65,0,3,1,1),(11,'Tyr class Battleship',600,0.1,1250,10,175,0,4,1,2),(12,'Freyja class Heavy Cruiser',150,0.48,720,4.8,85,0,2,1,2),(13,'Loki class Light Cruiser',70,0.8,420,2.3,58,20,1,1,2),(14,'Thor class Missile Ship',600,0.2,1050,9,110,20,3,1,2),(15,'Odin class Cruiser',100,0.6,520,3.1,65,0,2,1,2),(16,'Aegir class Destroyer',45,1,340,1.6,40,0,1,1,2),(17,'Heimdall class Monitor',350,0.3,700,7,65,0,3,1,2),(18,'Hel class Cruier',100,0.6,460,1,55,0,2,1,2),(19,'Vidar class Light Cruiser',70,0.8,440,2.35,60,20,1,1,2),(20,'Freyr class Destroyer',50,1,350,1.6,40,0,2,1,2);
/*!40000 ALTER TABLE `ship_templates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ships`
--

DROP TABLE IF EXISTS `ships`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ships` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ShipExp` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Owner_Id` int(11) DEFAULT NULL,
  `ShipBaseStats_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Owner_Id` (`Owner_Id`) USING HASH,
  KEY `IX_ShipBaseStats_Id` (`ShipBaseStats_Id`) USING HASH,
  CONSTRAINT `FK_ships_players_Owner_Id` FOREIGN KEY (`Owner_Id`) REFERENCES `players` (`Id`),
  CONSTRAINT `FK_ships_ship_templates_ShipBaseStats_Id` FOREIGN KEY (`ShipBaseStats_Id`) REFERENCES `ship_templates` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ships`
--

LOCK TABLES `ships` WRITE;
/*!40000 ALTER TABLE `ships` DISABLE KEYS */;
/*!40000 ALTER TABLE `ships` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shiptemplates_defencesystems`
--

DROP TABLE IF EXISTS `shiptemplates_defencesystems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `shiptemplates_defencesystems` (
  `ShipTemplateID` int(11) NOT NULL,
  `DefenceSystemID` int(11) NOT NULL,
  PRIMARY KEY (`ShipTemplateID`,`DefenceSystemID`),
  KEY `IX_ShipTemplateID` (`ShipTemplateID`) USING HASH,
  KEY `IX_DefenceSystemID` (`DefenceSystemID`) USING HASH,
  CONSTRAINT `FK_83346591fdec43609882839e2bdcceae` FOREIGN KEY (`DefenceSystemID`) REFERENCES `defence_systems` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_cc6d37dfcafa4310bfc252d27324f5ee` FOREIGN KEY (`ShipTemplateID`) REFERENCES `ship_templates` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shiptemplates_defencesystems`
--

LOCK TABLES `shiptemplates_defencesystems` WRITE;
/*!40000 ALTER TABLE `shiptemplates_defencesystems` DISABLE KEYS */;
INSERT INTO `shiptemplates_defencesystems` VALUES (1,1),(1,4),(1,7),(2,1),(2,4),(2,7),(3,1),(3,4),(3,6),(3,7),(3,9),(4,1),(4,4),(4,9),(5,2),(5,3),(5,5),(5,6),(5,8),(6,1),(6,4),(6,7),(7,2),(7,4),(7,6),(7,8),(8,2),(8,5),(8,8),(9,1),(9,4),(9,9),(10,1),(10,4),(10,7),(10,9),(11,11),(11,12),(11,14),(11,15),(11,17),(11,18),(12,10),(12,13),(12,15),(12,16),(12,18),(13,10),(13,13),(13,16),(14,10),(14,14),(14,17),(15,10),(15,13),(15,16),(16,12),(16,13),(16,16),(17,10),(17,13),(17,16),(18,10),(18,13),(18,16),(19,10),(19,13),(19,16),(20,12),(20,13),(20,16);
/*!40000 ALTER TABLE `shiptemplates_defencesystems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shiptemplates_weapons`
--

DROP TABLE IF EXISTS `shiptemplates_weapons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `shiptemplates_weapons` (
  `ShipTemplateID` int(11) NOT NULL,
  `WeaponID` int(11) NOT NULL,
  PRIMARY KEY (`ShipTemplateID`,`WeaponID`),
  KEY `IX_ShipTemplateID` (`ShipTemplateID`) USING HASH,
  KEY `IX_WeaponID` (`WeaponID`) USING HASH,
  CONSTRAINT `FK_ShipTemplates_Weapons_ship_templates_ShipTemplateID` FOREIGN KEY (`ShipTemplateID`) REFERENCES `ship_templates` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_ShipTemplates_Weapons_weapons_WeaponID` FOREIGN KEY (`WeaponID`) REFERENCES `weapons` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shiptemplates_weapons`
--

LOCK TABLES `shiptemplates_weapons` WRITE;
/*!40000 ALTER TABLE `shiptemplates_weapons` DISABLE KEYS */;
INSERT INTO `shiptemplates_weapons` VALUES (1,1),(1,2),(1,6),(1,15),(2,1),(2,2),(2,5),(2,6),(2,11),(3,2),(3,4),(3,5),(3,6),(3,11),(4,1),(4,7),(4,10),(4,15),(5,1),(5,6),(5,7),(5,8),(5,9),(5,10),(5,16),(6,1),(6,2),(6,6),(6,10),(6,15),(7,3),(7,4),(7,5),(7,6),(7,7),(7,12),(7,14),(8,1),(8,6),(8,8),(8,9),(8,10),(8,16),(9,1),(9,2),(9,7),(9,10),(9,15),(10,1),(10,2),(10,4),(10,6),(10,10),(10,15),(11,17),(11,21),(11,22),(11,24),(11,25),(11,26),(11,27),(11,28),(12,18),(12,21),(12,24),(12,25),(12,26),(12,28),(13,19),(13,21),(13,23),(13,26),(13,27),(14,18),(14,24),(14,27),(14,28),(14,29),(14,30),(14,31),(15,18),(15,21),(15,23),(15,25),(15,26),(15,28),(16,19),(16,21),(16,23),(16,26),(16,27),(17,20),(17,22),(17,23),(17,24),(17,26),(17,27),(17,31),(18,18),(18,21),(18,24),(18,25),(18,27),(18,28),(19,19),(19,21),(19,23),(19,26),(19,27),(20,19),(20,21),(20,23),(20,26),(20,27);
/*!40000 ALTER TABLE `shiptemplates_weapons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weapons`
--

DROP TABLE IF EXISTS `weapons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `weapons` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) DEFAULT NULL,
  `Damage` double NOT NULL,
  `NumberOfProjectiles` int(11) NOT NULL,
  `WeaponType` int(11) NOT NULL,
  `RangeMultiplier` double NOT NULL,
  `ChanceToHit` double NOT NULL,
  `ApEffectivity` double NOT NULL,
  `Faction_Id` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Faction_Id` (`Faction_Id`) USING HASH,
  CONSTRAINT `FK_weapons_Factions_Faction_Id` FOREIGN KEY (`Faction_Id`) REFERENCES `factions` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weapons`
--

LOCK TABLES `weapons` WRITE;
/*!40000 ALTER TABLE `weapons` DISABLE KEYS */;
INSERT INTO `weapons` VALUES (1,'100mm Cannon',40,6,2,0.5,0.5,10,1),(2,'Small IR laser turret',30,4,1,0.5,0.7,5,1),(3,'Heavy Cruise Missile',400,1,3,0,0.999,45,1),(4,'40mm EM Cannon',60,2,2,0.3,0.75,60,1),(5,'Medium Axial UV laser',150,1,1,0.2,0.85,55,1),(6,'Standard Missile',45,4,3,0,0.99,15,1),(7,'Swarm Missile',15,27,3,0,0.98,5,1),(8,'605mm Main Cannon',350,2,2,0.2,0.7,12,1),(9,'Heavy Axial UV laser',300,1,1,0.2,0.85,80,1),(10,'120mm fast firing Cannon',50,8,2,0.4,0.6,15,1),(11,'240mm Main Cannon',120,3,2,0.3,0.7,10,1),(12,'Cruise Missile Array x2',200,8,3,0,0.999,45,1),(14,'Standard Missile Array x2',45,32,3,0,0.98,15,1),(15,'Small Axial UV laser',75,1,1,0.2,0.85,45,1),(16,'Small IR laser turret x6',30,24,1,0.5,0.7,5,1),(17,'Experimental Super Heavy X-Ray Axial laser',750,1,1,0.2,0.85,125,2),(18,'Medium Axial UV laser',175,1,1,0.2,0.85,40,2),(19,'Small Axial UV laser',87.5,1,1,0.2,0.85,32,2),(20,'Heavy Axial UV laser',350,1,1,0.2,0.85,64,2),(21,'UV laser turret x2',50,4,1,0.5,0.7,10,2),(22,'420mm Main Cannon',250,3,2,0.3,0.7,12,2),(23,'5mm chaff gun',5,120,2,0.5,0.3,4,2),(24,'30mm EM Cannon x4',45,8,2,0.3,0.75,45,2),(25,'200mm Main Cannon',100,4,2,0.3,0.68,10,2),(26,'120mm Cannon x2',42,8,2,0.5,0.5,6,2),(27,'Swarm Missile',14,30,3,0,0.98,6,2),(28,'Standard Missile',50,4,3,0,0.99,12,2),(29,'Cruise Missile Array x2',210,8,3,0,0.999,40,2),(30,'Antimatter Cruise Missile',900,1,3,0,0.97,100,2),(31,'Standard Missile Array x2',50,32,3,0,0.99,12,2);
/*!40000 ALTER TABLE `weapons` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-01-12 18:21:00
