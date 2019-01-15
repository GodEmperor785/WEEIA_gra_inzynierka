using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	public static class Tests {

		private static IGameDataBase gameDataBase;

		public static IGameDataBase GameDataBase { get => gameDataBase; set => gameDataBase = value; }

		private static DbBaseModifiers GetDbBaseModifiers() {
			DbBaseModifiers mods = new DbBaseModifiers() {
				Id = 1,
				KineticRange = 1,           //each of range mults has to be from 0.0 to 1.0
				LaserRange = 0.5,           //they indicate how important is range for given WeaponType to hit its target, look in Game.cs at chanceToHit calculations for more
				MissileRange = 0.0,         //missile are invulnerable to range

				KineticPD = 2,
				KineticShield = 5,
				KineticIF = 1.2,
				LaserPD = 0,
				LaserShield = 1.5,
				LaserIF = 2,
				MissilePD = 8,
				MissileShield = 1,
				MissileIF = 1.2,

				BaseShipStatsExpModifier = 0.002,
				MaxShipsPerPlayer = 150,
				StartingMoney = 1000,
				ExpForVictory = 20,
				ExpForLoss = 10,
				FleetSizeExpModifier = 0.5,
				BaseFleetMaxSize = 1000,
				MaxAbsoluteFleetSize = 5000,
				MaxShipExp = 1000,
				MaxShipsInLine = 5,
				MaxFleetsPerPlayer = 8,
				MoneyForVictory = 80,
				MoneyForLoss = 40
			};
			return mods;
		}

		/// <summary>
		/// Drop create db with only admin user, BaseModifiers and factions
		/// </summary>
		public static void CreateBasicDb() {
			GameDataBase = Server.GetGameDBContext();
			DbBaseModifiers mods = GetDbBaseModifiers();
			GameDataBase.AddBaseModifiers(mods);
			Server.baseModifiers = GameDataBase.GetBaseModifiers();

			Faction empire = new Faction(1, "Empire");
			Faction alliance = new Faction(2, "Alliance");
			Faction union = new Faction(3, "Union");
			GameDataBase.AddFaction(empire);
			GameDataBase.AddFaction(alliance);
			GameDataBase.AddFaction(union);

			string admin = "admin";
			DbPlayer adminDbUser = new DbPlayer(admin, PasswordManager.GeneratePasswordHash(admin), 0, Server.BaseModifiers.BaseFleetMaxSize, 0, 0, Server.BaseModifiers.StartingMoney) {
				IsAdmin = true
			};
			GameDataBase.AddPlayer(adminDbUser);
		}

		/// <summary>
		/// Adds a few entries to DB
		/// </summary>
		public static void TestGameDB(bool exitOnFinish) {
			GameDataBase = Server.GetGameDBContext();
			Server.Log("WARNING: DEBUG - DB TEST PROGRAM WILL EXIT AFTER THE TEST", true);
			Server.Log(">>> BaseModifiers first");
			DbBaseModifiers mods = GetDbBaseModifiers();
			GameDataBase.AddBaseModifiers(mods);
			Server.baseModifiers = GameDataBase.GetBaseModifiers();

			Server.Log(">>> Some factions");
			Faction empire = new Faction(1, "Empire");
			Faction alliance = new Faction(2, "Alliance");
			Faction union = new Faction(3, "Union");
			GameDataBase.AddFaction(empire);
			GameDataBase.AddFaction(alliance);
			GameDataBase.AddFaction(union);
			var factionList = GameDataBase.GetAllFactions();
			empire = factionList.ElementAt(0);
			alliance = factionList.ElementAt(1);
			union = factionList.ElementAt(2);
			Server.Log("Factions in DB are:");
			foreach (Faction f in factionList) Server.Log(f.Name);

			Server.Log(">>> Some users");
			string p1Name = "player1";
			string p2Name = "player2";
			string a1Name = "admin";
			DbPlayer p1 = new DbPlayer(p1Name, PasswordManager.GeneratePasswordHash(p1Name), Server.BaseModifiers.ExpForVictory * 2 + Server.BaseModifiers.ExpForLoss, Server.BaseModifiers.BaseFleetMaxSize, 3, 2, Server.BaseModifiers.StartingMoney);
			DbPlayer p2 = new DbPlayer(p2Name, PasswordManager.GeneratePasswordHash(p2Name), Server.BaseModifiers.ExpForLoss * 2 + Server.BaseModifiers.ExpForVictory, Server.BaseModifiers.BaseFleetMaxSize, 3, 1, Server.BaseModifiers.StartingMoney);
			DbPlayer a1 = new DbPlayer(a1Name, PasswordManager.GeneratePasswordHash(a1Name), 0, Server.BaseModifiers.BaseFleetMaxSize, 0, 0, Server.BaseModifiers.StartingMoney) {
				IsAdmin = true
			};
			GameDataBase.AddPlayer(p1);
			GameDataBase.AddPlayer(p2);
			GameDataBase.AddPlayer(a1);
			var playersList = GameDataBase.GetAllPlayers();
			Server.Log("Players in DB are:");
			foreach (DbPlayer p in playersList) Server.Log(p.Username);

			Server.Log(">>> Some lootboxes");
			DbLootBox l1 = new DbLootBox(100, "basic lootbox", 0.5, 0.3, 0.15, 0.05, 2);        //lootbox names hardcoded in client - better not change it...
			DbLootBox l2 = new DbLootBox(300, "better lootbox", 0.4, 0.4, 0.15, 0.05, 4);
			DbLootBox l3 = new DbLootBox(1000, "supreme lootbox", 0.1, 0.3, 0.4, 0.2, 4);
			GameDataBase.AddLootBox(l1);
			GameDataBase.AddLootBox(l2);
			GameDataBase.AddLootBox(l3);
			var lootboxList = GameDataBase.GetAllLootBoxes();
			Server.Log("Lootboxes in DB are:");
			foreach (DbLootBox l in lootboxList) Server.Log(l.Name);

			Server.Log(">>> Some weapons and defences");
			DbWeapon w1 = new DbWeapon("kinetic 100mm", empire, 40, 6, WeaponType.KINETIC, 0.5, 0.5, 10);
			DbWeapon w2 = new DbWeapon("kinetic 5mm", alliance, 2.5, 80, WeaponType.KINETIC, 1, 0.2, 2);
			DbWeapon w3 = new DbWeapon("axial UV laser", alliance, 250, 1, WeaponType.LASER, 0.2, 0.85, 55);
			DbWeapon w4 = new DbWeapon("IR laser turret", empire, 20, 4, WeaponType.LASER, 0.5, 0.7, 15);
			DbWeapon w5 = new DbWeapon("imperial cruise missile", empire, 200, 1, WeaponType.MISSILE, 0, 0.999, 45);
			DbWeapon w6 = new DbWeapon("alliance swarm missile", alliance, 20, 10, WeaponType.MISSILE, 0, 0.99, 5);
			GameDataBase.AddWeapon(w1);
			GameDataBase.AddWeapon(w2);
			GameDataBase.AddWeapon(w3);
			GameDataBase.AddWeapon(w4);
			GameDataBase.AddWeapon(w5);
			GameDataBase.AddWeapon(w6);
			DbDefenceSystem d1 = new DbDefenceSystem("imperial PD", empire, 100, DefenceSystemType.POINT_DEFENCE, 2, 0, 4);
			DbDefenceSystem d2 = new DbDefenceSystem("alliance PD", alliance, 80, DefenceSystemType.POINT_DEFENCE, 2.5, 0, 5);
			DbDefenceSystem d3 = new DbDefenceSystem("imperial shield", empire, 60, DefenceSystemType.SHIELD, 4, 1, 1.2);
			DbDefenceSystem d4 = new DbDefenceSystem("alliance shield", alliance, 80, DefenceSystemType.SHIELD, 3.5, 0.95, 1.2);
			DbDefenceSystem d5 = new DbDefenceSystem("imperial IF", empire, 45, DefenceSystemType.INTEGRITY_FIELD, 3, 3, 3);
			DbDefenceSystem d6 = new DbDefenceSystem("alliance IF", alliance, 40, DefenceSystemType.INTEGRITY_FIELD, 3, 4, 3);
			GameDataBase.AddDefenceSystem(d1);
			GameDataBase.AddDefenceSystem(d2);
			GameDataBase.AddDefenceSystem(d3);
			GameDataBase.AddDefenceSystem(d4);
			GameDataBase.AddDefenceSystem(d5);
			GameDataBase.AddDefenceSystem(d6);
			var weps = GameDataBase.GetAllWeapons();
			Server.Log("Weapons in DB are:");
			foreach (var p in weps) Server.Log(p.Name);
			var defs = GameDataBase.GetAllDefences();
			Server.Log("Defences in DB are:");
			foreach (var p in defs) Server.Log(p.Name);

			Server.Log(">>> Some ship templates");
			List<DbWeapon> al1wep = new List<DbWeapon> {
				weps[1],
				weps[2],
				weps[5]
			};
			List<DbWeapon> al2wep = new List<DbWeapon> {
				weps[1],
				weps[5]
			};
			List<DbWeapon> imp1wep = new List<DbWeapon> {
				weps[0],
				weps[3],
				weps[4]
			};
			List<DbWeapon> imp2wep = new List<DbWeapon> {
				weps[0],
				weps[3]
			};
			List<DbDefenceSystem> al1def = new List<DbDefenceSystem> {
				defs[1],
				defs[3],
				defs[5]
			};
			List<DbDefenceSystem> al2def = new List<DbDefenceSystem> {
				defs[1],
				defs[5]
			};
			List<DbDefenceSystem> imp1def = new List<DbDefenceSystem> {
				defs[0],
				defs[2],
				defs[4]
			};
			List<DbDefenceSystem> imp2def = new List<DbDefenceSystem> {
				defs[0],
				defs[4]
			};
			DbShipTemplate imp1 = new DbShipTemplate("Class Warrior Imperial Cruiser", empire, 90, 0.5, 500, 3, 75, imp1wep, imp1def, 0, Rarity.RARE);
			DbShipTemplate imp2 = new DbShipTemplate("Class Dagger Imperial Destroyer", empire, 30, 0.9, 150, 1, 25, imp2wep, imp2def, 0, Rarity.COMMON);
			DbShipTemplate al1 = new DbShipTemplate("Class Hammer Alliance Cruiser", alliance, 95, 0.5, 550, 3, 80, al1wep, al1def, 0, Rarity.RARE);
			DbShipTemplate al2 = new DbShipTemplate("Class Ferret Alliance Destroyer", alliance, 30, 0.88, 175, 1, 28, al2wep, al2def, 0, Rarity.COMMON);
			GameDataBase.AddShipTemplate(imp1);
			GameDataBase.AddShipTemplate(imp2);
			GameDataBase.AddShipTemplate(al1);
			GameDataBase.AddShipTemplate(al2);
			var shipTemplates = GameDataBase.GetAllShipTemplates();
			Server.Log("Ship Templates in DB are:");
			foreach (var p in shipTemplates) Server.Log(p.Name + " weapon count=" + p.Weapons.Count);

			Server.Log(">>> Some ships for players");
			DbShip s1p1 = shipTemplates[2].GenerateNewShipOfThisTemplate(p1);
			DbShip s2p1 = shipTemplates[2].GenerateNewShipOfThisTemplate(p1);
			DbShip s3p1 = shipTemplates[3].GenerateNewShipOfThisTemplate(p1);
			DbShip s1p2 = shipTemplates[0].GenerateNewShipOfThisTemplate(p2);
			DbShip s2p2 = shipTemplates[1].GenerateNewShipOfThisTemplate(p2);
			DbShip s3p2 = shipTemplates[1].GenerateNewShipOfThisTemplate(p2);
			GameDataBase.AddShip(s1p1);
			GameDataBase.AddShip(s2p1);
			GameDataBase.AddShip(s3p1);
			GameDataBase.AddShip(s1p2);
			GameDataBase.AddShip(s2p2);
			GameDataBase.AddShip(s3p2);
			var ships = GameDataBase.GetAllShips();
			Server.Log("Ships in DB are:");
			foreach (var p in ships) Server.Log("Ship of template: " + p.ShipBaseStats.Name + " owned by: " + p.Owner.Username);

			Server.Log(">>> Some fleets for players");
			List<DbShip> p1fleet = new List<DbShip> {
				ships[0],
				ships[1],
				ships[2]
			};
			List<DbShip> p2fleet = new List<DbShip> {
				ships[3],
				ships[4],
				ships[5]
			};
			DbFleet f1 = new DbFleet(p1, p1fleet, p1Name + "_Fleet");
			DbFleet f2 = new DbFleet(p2, p2fleet, p2Name + "_Fleet");
			GameDataBase.AddFleet(f1.ToFleetOnlyActiveShips(), p1.ToPlayer());
			GameDataBase.AddFleet(f2.ToFleetOnlyActiveShips(), p2.ToPlayer());
			var fleetsP1 = GameDataBase.GetAllFleetsOfPlayer(p1.ToPlayer());
			var fleetsP2 = GameDataBase.GetAllFleetsOfPlayer(p2.ToPlayer());
			Server.Log("Fleets in DB are:");
			foreach (var p in fleetsP1) Server.Log("fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);
			foreach (var p in fleetsP2) Server.Log("fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);

			Server.Log(">>> Some game history");
			DbGameHistory h1 = new DbGameHistory(p1, p2, fleetsP1.First(), fleetsP2.First(), false, new DateTime(2018, 10, 28, 11, 15, 58));
			DbGameHistory h2 = new DbGameHistory(p1, p2, fleetsP1.First(), fleetsP2.First(), false, new DateTime(2018, 10, 29, 12, 25, 1));
			DbGameHistory h3 = new DbGameHistory(p2, p1, fleetsP2.First(), fleetsP1.First(), false, new DateTime(2018, 11, 6, 18, 2, 30));
			GameDataBase.AddGameHistory(h1);
			GameDataBase.AddGameHistory(h2);
			GameDataBase.AddGameHistory(h3);
			var historiesP1 = GameDataBase.GetPlayersGameHistory(p1.Id);
			var historiesP2 = GameDataBase.GetPlayersGameHistory(p2.Id);
			Server.Log("Game Histories in DB are:");
			foreach (var p in historiesP1) Server.Log("winner: " + p.Winner.Username + " loser: " + p.Loser.Username + " winner fleet: " + p.WinnerFleet.Name);
			foreach (var p in historiesP2) Server.Log("winner: " + p.Winner.Username + " loser: " + p.Loser.Username + " winner fleet: " + p.WinnerFleet.Name);
			var x3 = GameDataBase.GetGameHistoryEntry(1);
			Server.Log("winner: " + x3.Winner.Username + " loser: " + x3.Loser.Username + " winner fleet: " + x3.WinnerFleet.Name + " fleet first ship weapon faction name: "
				+ x3.WinnerFleet.Ships[0].ShipBaseStats.Weapons[0].Faction.Name);

			Server.Log(">>> Additional ship templates and weapons for application testing - so the DB is more or less complete");
			Server.Log(">>> first more weapons and defences");
			DbWeapon w7 = new DbWeapon("kinetic 100mm x4", empire, 40, 6 * 4, WeaponType.KINETIC, 0.5, 0.5, 10);
			DbWeapon w8 = new DbWeapon("axial HE UV femtosecond laser", empire, 2000, 1, WeaponType.LASER, 0.25, 0.9, 100);
			DbWeapon w9 = new DbWeapon("neutron warhead missiles x8", alliance, 50, 8, WeaponType.MISSILE, 0, 0.995, 900);
			DbWeapon w10 = new DbWeapon("Marauder cruise missile launchers x10", alliance, 210, 10, WeaponType.MISSILE, 0, 0.999, 70);
			GameDataBase.AddWeapon(w7);
			GameDataBase.AddWeapon(w8);
			GameDataBase.AddWeapon(w9);
			GameDataBase.AddWeapon(w10);
			DbDefenceSystem d7 = new DbDefenceSystem("Imperial Smart IF", empire, 60, DefenceSystemType.INTEGRITY_FIELD, 6, 6, 6);
			DbDefenceSystem d8 = new DbDefenceSystem("alliance High Power IF", alliance, 110, DefenceSystemType.INTEGRITY_FIELD, 3, 3, 3);
			DbDefenceSystem d9 = new DbDefenceSystem("imperial heavy shield", empire, 250, DefenceSystemType.SHIELD, 4, 1, 1.2);
			DbDefenceSystem d10 = new DbDefenceSystem("alliance multicore shield", alliance, 100, DefenceSystemType.SHIELD, 8, 1.2, 6);
			DbDefenceSystem d11 = new DbDefenceSystem("imperial mass PD", empire, 180, DefenceSystemType.POINT_DEFENCE, 4, 0, 10);
			DbDefenceSystem d12 = new DbDefenceSystem("alliance high precision laser PD", alliance, 250, DefenceSystemType.POINT_DEFENCE, 2.5, 0, 6);
			GameDataBase.AddDefenceSystem(d7);
			GameDataBase.AddDefenceSystem(d8);
			GameDataBase.AddDefenceSystem(d9);
			GameDataBase.AddDefenceSystem(d10);
			GameDataBase.AddDefenceSystem(d11);
			GameDataBase.AddDefenceSystem(d12);
			weps = GameDataBase.GetAllWeapons();
			Server.Log("Weapons in DB are:");
			foreach (var p in weps) Server.Log(p.Name);
			defs = GameDataBase.GetAllDefences();
			Server.Log("Defences in DB are:");
			foreach (var p in defs) Server.Log(p.Name);

			Server.Log(">>> Now some rare templates");
			List<DbWeapon> impRareWep = new List<DbWeapon> {
				weps[3],
				weps[4],
				weps[6],
				weps[7]
			};
			List<DbWeapon> AlRareWep = new List<DbWeapon> {
				weps[1],
				weps[5],
				weps[8],
				weps[9]
			};
			List<DbDefenceSystem> ImpRareDef = new List<DbDefenceSystem> {
				defs[6],
				defs[8],
				defs[10]
			};
			List<DbDefenceSystem> AlRareDef = new List<DbDefenceSystem> {
				defs[7],
				defs[9],
				defs[11]
			};
			DbShipTemplate impRare = new DbShipTemplate("Class Master Imperial Battleship", empire, 300, 0.1, 2500, 8, 220, impRareWep, ImpRareDef, 0, Rarity.LEGENDARY);
			DbShipTemplate alRare = new DbShipTemplate("Class Apocalypse Alliance Missile Ship", alliance, 250, 0.15, 1200, 6.5, 140, AlRareWep, AlRareDef, 0, Rarity.VERY_RARE);
			GameDataBase.AddShipTemplate(impRare);
			GameDataBase.AddShipTemplate(alRare);
			shipTemplates = GameDataBase.GetAllShipTemplates();
			Server.Log("Ship Templates in DB are:");
			foreach (var p in shipTemplates) Server.Log(p.Name + " weapon count=" + p.Weapons.Count);

			Server.Log("########################################################################################################");
			fleetsP1 = GameDataBase.GetAllFleetsOfPlayer(p1.ToPlayer());
			fleetsP2 = GameDataBase.GetAllFleetsOfPlayer(p2.ToPlayer());
			Server.Log("Fleets in DB are:");
			foreach (var p in fleetsP1) Server.Log(p.Id + " fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);
			foreach (var p in fleetsP2) Server.Log(p.Id + " fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);
			ships = GameDataBase.GetAllShips();
			Server.Log("Ships in DB are:");
			foreach (var p in ships) Server.Log(p.Id + " Ship of template: " + p.ShipBaseStats.Name + " owned by: " + p.Owner.Username + " active: " + p.IsActive);
			ships = GameDataBase.GetPlayersShips(p1.ToPlayer());
			Server.Log("P1 Ships in DB are:");
			foreach (var p in ships) Server.Log(p.Id + " Ship of template: " + p.ShipBaseStats.Name + " owned by: " + p.Owner.Username + " active: " + p.IsActive);
			Server.Log("Game Histories in DB are:");
			historiesP1 = GameDataBase.GetPlayersGameHistory(p1.Id);
			foreach (var p in historiesP1) {
				Server.Log("winner: " + p.Winner.Username + " loser: " + p.Loser.Username + " winner fleet count: " + p.WinnerFleet.Name);
				Server.Log("winner fleet:");
				foreach (var s in p.WinnerFleet.Ships) Server.Log(s.Id + " Ship of template: " + s.ShipBaseStats.Name + " owned by: " + s.Owner.Username + " active: " + s.IsActive);
				Server.Log("loser fleet:");
				foreach (var s in p.LoserFleet.Ships) Server.Log(s.Id + " Ship of template: " + s.ShipBaseStats.Name + " owned by: " + s.Owner.Username + " active: " + s.IsActive);
			}

			//GameDataBase.RemoveShipWithId(fleetsP1.First().Ships.First().Id, true);
			//GameDataBase.RemoveShipWithId(fleetsP1.First().Ships[1].Id, true);
			Server.Log("----------");
			fleetsP1 = GameDataBase.GetAllFleetsOfPlayer(p1.ToPlayer());
			fleetsP2 = GameDataBase.GetAllFleetsOfPlayer(p2.ToPlayer());
			Server.Log("Fleets in DB are:");
			foreach (var p in fleetsP1) Server.Log(p.Id + " fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);
			foreach (var p in fleetsP2) Server.Log(p.Id + " fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);
			ships = GameDataBase.GetAllShips();
			Server.Log("Ships in DB are:");
			foreach (var p in ships) Server.Log(p.Id + " Ship of template: " + p.ShipBaseStats.Name + " owned by: " + p.Owner.Username + " active: " + p.IsActive);
			ships = GameDataBase.GetPlayersShips(p1.ToPlayer());
			Server.Log("P1 Ships in DB are:");
			foreach (var p in ships) Server.Log(p.Id + " Ship of template: " + p.ShipBaseStats.Name + " owned by: " + p.Owner.Username + " active: " + p.IsActive);
			using (IGameDataBase GameDataBase = new MySqlDataBase()) {
				Server.Log("Game Histories in DB are:");
				historiesP1 = GameDataBase.GetPlayersGameHistory(p1.Id);
				foreach (var p in historiesP1) {
					var pp = GameDataBase.GetGameHistoryEntry(p.Id);
					Server.Log("winner: " + pp.Winner.Username + " loser: " + pp.Loser.Username + " winner fleet count: " + pp.WinnerFleet.Name);
					Server.Log("winner fleet:");
					foreach (var s in pp.WinnerFleet.Ships) Server.Log(s.Id + " Ship of template: " + s.ShipBaseStats.Name + " owned by: " + s.Owner.Username + " active: " + s.IsActive);
					Server.Log("loser fleet:");
					foreach (var s in pp.LoserFleet.Ships) Server.Log(s.Id + " Ship of template: " + s.ShipBaseStats.Name + " owned by: " + s.Owner.Username + " active: " + s.IsActive);
				}
			}

			Server.Log("DEBUG: TEST END", true);
			if (exitOnFinish) {
				Server.Log("DEBUG: exiting application and closing connections", true);
				Console.ReadKey();
				GameDataBase.Dispose();
				Environment.Exit(0);
			}
		}

		public static void DoGameBoardValidationTest() {
			//game board test
			Server.baseModifiers = GetDbBaseModifiers().ToBaseModifiers();

			Weapon chaff = new Weapon(1, "chaff", new Faction(), 5, 100, WeaponType.KINETIC, 3, 0.9, 0.3);
			Weapon oneHitKiller = new Weapon(2, "one hit killer", new Faction(), 50000, 1, WeaponType.LASER, 10000, 0, 1);
			DefenceSystem d1 = new DefenceSystem(1, "d1", new Faction(), 10, DefenceSystemType.POINT_DEFENCE, 2, 0, 2);
			DefenceSystem superDef = new DefenceSystem(2, "superDef", new Faction(), 4000, DefenceSystemType.SHIELD, 2, 2, 2);
			List<Weapon> weakWeapons = new List<Weapon> { chaff };
			List<Weapon> allWeapons = new List<Weapon> { chaff, oneHitKiller };
			List<DefenceSystem> weakDef = new List<DefenceSystem> { d1 };
			List<DefenceSystem> allDefs = new List<DefenceSystem> { d1, superDef };

			Ship p1s1 = new Ship() { Id = 1, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s2 = new Ship() { Id = 2, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s3 = new Ship() { Id = 3, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s4 = new Ship() { Id = 4, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s5 = new Ship() { Id = 5, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s6 = new Ship() { Id = 6, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s7 = new Ship() { Id = 7, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s8 = new Ship() { Id = 8, Evasion = 0.1, Armor = 200, Hp = 1500, Size = 9, Defences = CloneList(allDefs), Weapons = CloneList(allWeapons) };

			Ship p2s1 = new Ship() { Id = 11, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s2 = new Ship() { Id = 12, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s3 = new Ship() { Id = 13, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s4 = new Ship() { Id = 14, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s5 = new Ship() { Id = 15, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s6 = new Ship() { Id = 16, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s7 = new Ship() { Id = 17, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s8 = new Ship() { Id = 18, Evasion = 0.1, Armor = 800, Hp = 500, Size = 8, Defences = CloneList(allDefs), Weapons = CloneList(allWeapons) };

			List<Ship> p1s = new List<Ship> { p1s1, p1s2, p1s3, p1s4, };
			List<Ship> p1m = new List<Ship> { p1s6, p1s7, p1s5 };
			List<Ship> p1l = new List<Ship> { p1s8 };
			List<Ship> p1all = new List<Ship> { p1s1, p1s2, p1s3, p1s4, p1s5, p1s6, p1s7, p1s8 };
			Fleet p1fleet = new Fleet("p1fleet", new Player(), p1all);
			PlayerGameBoard p1gameBoard = new PlayerGameBoard(p1s, p1m, p1l);

			List<Ship> p2s = new List<Ship> { p2s1, p2s2 };
			List<Ship> p2m = new List<Ship> { p2s3, p2s4, p2s5, p2s8 };
			List<Ship> p2l = new List<Ship> { p2s6, p2s7 };
			List<Ship> p2all = new List<Ship> { p2s1, p2s2, p2s3, p2s4, p2s5, p2s6, p2s7, p2s8 };
			Fleet p2fleet = new Fleet("p2fleet", new Player(), p2all);
			PlayerGameBoard p2gameBoard = new PlayerGameBoard(p2s, p2m, p2l);

			string p1validate = GameServerValidator.ValidatePlayerBoard(p1gameBoard, p1fleet);
			string p2validate = GameServerValidator.ValidatePlayerBoard(p2gameBoard, p2fleet);
			Console.WriteLine("p1 board validate: " + p1validate);
			Console.WriteLine("p2 board validate: " + p2validate);

			PrintGameBoards(p1gameBoard, p2gameBoard);

			//Tuple<ShipPosition, Line> p1mm1 = new Tuple<ShipPosition, Line>(new ShipPosition(Line.SHORT, 0), Line.MEDIUM);
			//Tuple<ShipPosition, Line> p1mm2 = new Tuple<ShipPosition, Line>(new ShipPosition(Line.MEDIUM, 0), Line.SHORT);
			Tuple<ShipPosition, ShipPosition> p1ma1 = new Tuple<ShipPosition, ShipPosition>(new ShipPosition(Line.SHORT, 0), new ShipPosition(Line.MEDIUM, 1));
			Tuple<ShipPosition, ShipPosition> p1ma2 = new Tuple<ShipPosition, ShipPosition>(new ShipPosition(Line.SHORT, 1), new ShipPosition(Line.MEDIUM, 1));

			Tuple<ShipPosition, ShipPosition> p1killmove = new Tuple<ShipPosition, ShipPosition>(new ShipPosition(Line.LONG, 0), new ShipPosition(Line.MEDIUM, 1));

			Tuple<ShipPosition, ShipPosition> p2ma1 = new Tuple<ShipPosition, ShipPosition>(new ShipPosition(Line.MEDIUM, 1), new ShipPosition(Line.SHORT, 1));
			Move p1move = new Move();
			Move p2move = new Move();
			//p1move.MoveList.Add(p1mm1);
			//p1move.MoveList.Add(p1mm2);
			p1move.AttackList.Add(p1ma1);
			p1move.AttackList.Add(p1ma2);
			p2move.AttackList.Add(p2ma1);

			p1validate = GameServerValidator.ValidateMove(p1move, p1gameBoard, p2gameBoard);
			p2validate = GameServerValidator.ValidateMove(p2move, p2gameBoard, p1gameBoard);
			Console.WriteLine("p1 move validate: " + p1validate);
			Console.WriteLine("p2 move validate: " + p2validate);
			Console.WriteLine("press any key to contine with game test");
			Console.ReadKey();

			Console.WriteLine("======== Game test =========");
			Game game = new Game(p1gameBoard, p2gameBoard) {
				EnableDebug = true
			};
			try {
				game.MakeTurn(p1move, true, p2move, true);
				PrintGameBoards(game.Player1GameBoard, game.Player2GameBoard);
				Console.WriteLine("press any key to contine with next move");
				Console.ReadKey();

				p1validate = GameServerValidator.ValidateMove(p1move, p1gameBoard, p2gameBoard);
				p2validate = GameServerValidator.ValidateMove(p2move, p2gameBoard, p1gameBoard);
				Console.WriteLine("p1 move validate: " + p1validate);
				Console.WriteLine("p2 move validate: " + p2validate);
				game.MakeTurn(p1move, true, p2move, true);
				PrintGameBoards(game.Player1GameBoard, game.Player2GameBoard);

				Console.ReadKey();
			}
			catch (Exception e) {
				Console.WriteLine(e.Source);
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
			}
			Console.ReadKey();
			Environment.Exit(0);
		}

		private static List<T> CloneList<T>(List<T> src) {
			List<T> ret = new List<T>();
			foreach (var def in src) ret.Add(Server.CloneObject(def));
			return ret;
		}

		private static void PrintGameBoards(PlayerGameBoard p1gameBoard, PlayerGameBoard p2gameBoard) {
			Console.WriteLine("p1 game board");
			foreach (var line in p1gameBoard.Board) {
				Console.Write(line.Key + " :   \t");
				foreach (Ship s in line.Value) Console.Write(s.Id + "\t");
				Console.Write(Environment.NewLine);
			}
			Console.WriteLine("p2 game board");
			foreach (var line in p2gameBoard.Board) {
				Console.Write(line.Key + " :   \t");
				foreach (Ship s in line.Value) Console.Write(s.Id + "\t");
				Console.Write(Environment.NewLine);
			}
		}

		public static void TestRandomness() {
			Dictionary<Rarity, double> chances = new Dictionary<Rarity, double>() {
					{ Rarity.COMMON, 0.5 },
					{ Rarity.RARE, 0.2 },
					{ Rarity.VERY_RARE, 0.2 },
					{ Rarity.LEGENDARY, 0.1 }
				};
			Dictionary<Rarity, int> counts = new Dictionary<Rarity, int>() {
					{ Rarity.COMMON, 0 },
					{ Rarity.RARE, 0 },
					{ Rarity.VERY_RARE, 0 },
					{ Rarity.LEGENDARY, 0 }
			};

			GameRNG rng = new GameRNG();
			int totalRolls = 10000000;

			for (int i = 0; i < totalRolls; i++) {
				Rarity rolled = rng.GetRandomRarityWithChances(chances);
				counts[rolled]++;
			}
			foreach (var pair in counts) {
				Server.Log(pair.Key.GetRarityName() + " count = " + pair.Value + " " + ((double)pair.Value / (double)totalRolls));
			}

			counts = new Dictionary<Rarity, int>() {
				{ Rarity.COMMON, 0 },
				{ Rarity.RARE, 0 },
				{ Rarity.VERY_RARE, 0 },
				{ Rarity.LEGENDARY, 0 }
			};
			GameDataBase = Server.GetGameDBContext();
			int numberOfShips = 10000;
			List<DbShip> randomShips = new List<DbShip>();
			DbPlayer p = new DbPlayer() {
				Id = 2,
				Username = "random",
				Experience = 0
			};
			var GameRNG = new GameRNG();
			for (int i = 0; i < numberOfShips; i++) {    //repeat as many times as there are ships in lootbox
				Rarity drawnRarity = GameRNG.GetRandomRarityWithChances(chances);
				randomShips.Add(GameDataBase.GetRandomShipTemplateOfRarity(drawnRarity, p.Experience).GenerateNewShipOfThisTemplate(p));
			}
			foreach (var ship in randomShips) {
				//Server.Log(ship.ShipBaseStats.Name + ", rarity = " + ship.ShipBaseStats.ShipRarity);
				counts[ship.ShipBaseStats.ShipRarity]++;
			}
			foreach (var pair in counts) {
				Server.Log(pair.Key.GetRarityName() + " count = " + pair.Value + " " + ((double)pair.Value / (double)numberOfShips));
			}

			Console.ReadKey();
			Environment.Exit(0);
		}

	}

}
