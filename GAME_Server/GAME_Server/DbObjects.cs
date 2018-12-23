using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using GAME_connection;

namespace GAME_Server {
	[Table("players")]
	public class DbPlayer {
		public DbPlayer() { }

		public DbPlayer( string username, string password, int experience, int maxFleetPoints, int gamesPlayed, int gamesWon, int money) {
			Username = username;
			Password = password;
			Experience = experience;
			MaxFleetPoints = maxFleetPoints;
			GamesPlayed = gamesPlayed;
			GamesWon = gamesWon;
			OwnedShips = new List<DbShip>();
			Money = money;
			IsActive = true;
			IsAdmin = false;
		}

		public DbPlayer(int id, string username, string password, int experience, int maxFleetPoints, int gamesPlayed, int gamesWon, int money) 
			: this(username, password, experience, maxFleetPoints, gamesPlayed, gamesWon, money) {
			Id = id;
		}

		/// <summary>
		/// used to create new player
		/// </summary>
		/// <param name="player"></param>
		public DbPlayer(Player player, int startingMoney) {
			Username = player.Username;
			Password = player.Password;
			Experience = 0;
			MaxFleetPoints = 0;
			Id = player.Id;
			GamesPlayed = 0;
			GamesWon = 0;
			OwnedShips = new List<DbShip>();
			Money = startingMoney;
		}

		public string Username { get; set; }
		public string Password { get; set; }
		public int Experience { get; set; }
		public int MaxFleetPoints { get; set; }
		public int Id { get; set; }
		public int GamesPlayed { get; set; }
		public int GamesWon { get; set; }
		public int Money { get; set; }
		public bool IsActive { get; set; }
		public bool IsAdmin { get; set; }

		public List<DbShip> OwnedShips { get; set; }

		public Player ToPlayer() {
			return new Player(Id, Username, Password, Experience, MaxFleetPoints, GamesPlayed, GamesWon, Money);
		}
	}

	[Table("weapons")]
	public class DbWeapon {

		public DbWeapon() { }

		public DbWeapon(string name, Faction faction, double damage, int numberOfProjectiles, WeaponType weaponType, double rangeMultiplier, double chanceToHit, double apEffectivity) {
			Name = name;
			Faction = faction;
			Damage = damage;
			NumberOfProjectiles = numberOfProjectiles;
			WeaponType = weaponType;
			RangeMultiplier = rangeMultiplier;
			ChanceToHit = chanceToHit;
			ApEffectivity = apEffectivity;
			Ships = new List<DbShipTemplate>();
		}

		public DbWeapon(int id, string name, Faction faction, double damage, int numberOfProjectiles, WeaponType weaponType, double rangeMultiplier, double chanceToHit, double apEffectivity) 
			: this(name, faction, damage, numberOfProjectiles, weaponType, rangeMultiplier, chanceToHit, apEffectivity) {
			Id = id;
		}

		public DbWeapon(Weapon weapon) {
			Name = weapon.Name;
			Faction = weapon.Faction;
			Damage = weapon.Damage;
			NumberOfProjectiles = weapon.NumberOfProjectiles;
			WeaponType = weapon.WeaponType;
			RangeMultiplier = weapon.RangeMultiplier;
			ChanceToHit = weapon.ChanceToHit;
			ApEffectivity = weapon.ApEffectivity;
			Id = weapon.Id;
			Ships = new List<DbShipTemplate>();
		}

		public string Name { get; set; }
		public Faction Faction { get; set; }
		public double Damage { get; set; }
		public int NumberOfProjectiles { get; set; }
		public WeaponType WeaponType { get; set; }
		public double RangeMultiplier { get; set; }
		public double ChanceToHit { get; set; }
		public double ApEffectivity { get; set; }
		public int Id { get; set; }

		public List<DbShipTemplate> Ships { get; set; }

		public Weapon ToWeapon() {
			return new Weapon(Id, Name, Faction, Damage, NumberOfProjectiles, WeaponType, ApEffectivity, RangeMultiplier, ChanceToHit);
		}
	}

	[Table("defence_systems")]
	public class DbDefenceSystem {

		public DbDefenceSystem() { }

		public DbDefenceSystem(string name, Faction faction, double defenceValue, DefenceSystemType systemType, double defAgainstKinetic, double defAgainstLaser, double defAgainstMissile) {
			Name = name;
			Faction = faction;
			DefenceValue = defenceValue;
			SystemType = systemType;
			DefAgainstKinetic = defAgainstKinetic;
			DefAgainstLaser = defAgainstLaser;
			DefAgainstMissile = defAgainstMissile;
			Ships = new List<DbShipTemplate>();
		}

		public DbDefenceSystem(int id, string name, Faction faction, double defenceValue, DefenceSystemType systemType, double defAgainstKinetic, double defAgainstLaser, double defAgainstMissile)
			: this(name, faction, defenceValue, systemType, defAgainstKinetic, defAgainstLaser, defAgainstMissile) {
			Id = id;
		}

		public DbDefenceSystem(DefenceSystem defenceSystem) {
			Name = defenceSystem.Name;
			Faction = defenceSystem.Faction;
			DefenceValue = defenceSystem.DefenceValue;
			SystemType = defenceSystem.SystemType;
			DefAgainstKinetic = defenceSystem.DefMultAgainstWepTypeMap[WeaponType.KINETIC];
			DefAgainstLaser = defenceSystem.DefMultAgainstWepTypeMap[WeaponType.LASER];
			DefAgainstMissile = defenceSystem.DefMultAgainstWepTypeMap[WeaponType.MISSILE];
			Id = defenceSystem.Id;
			Ships = new List<DbShipTemplate>();
		}

		public string Name { get; set; }
		public Faction Faction { get; set; }
		public double DefenceValue { get; set; }
		public DefenceSystemType SystemType { get; set; }
		public double DefAgainstKinetic { get; set; }
		public double DefAgainstLaser { get; set; }
		public double DefAgainstMissile { get; set; }
		public int Id { get; set; }

		public List<DbShipTemplate> Ships { get; set; }

		public DefenceSystem ToDefenceSystem() {
			return new DefenceSystem(Id, Name, Faction, DefenceValue, SystemType, DefAgainstKinetic, DefAgainstLaser, DefAgainstMissile);
		}
	}

	[Table("ship_templates")]
	public class DbShipTemplate {
		public DbShipTemplate() { }

		public DbShipTemplate(string name, Faction faction, int cost, double evasion, double hp, double size, double armor, List<DbWeapon> weapons, List<DbDefenceSystem> defences, int expUnlock, Rarity rarity) {
			Name = name;
			Faction = faction;
			Cost = cost;
			Evasion = evasion;
			Hp = hp;
			Weapons = weapons;
			Defences = defences;
			Size = size;
			Armor = armor;
			ExpUnlock = expUnlock;
			ShipRarity = rarity;
			ShipsOfThisTemplate = new List<DbShip>();
		}

		public DbShipTemplate(int id, string name, Faction faction, int cost, double evasion, double hp, double size, double armor, List<DbWeapon> weapons, List<DbDefenceSystem> defences, int expUnlock, Rarity rarity)
			: this(name, faction, cost, evasion, hp, size, armor, weapons, defences, expUnlock, rarity) {
			Id = id;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public Faction Faction { get; set; }
		public int Cost { get; set; }
		public double Evasion { get; set; }
		public double Hp { get; set; }
		public List<DbWeapon> Weapons { get; set; }
		public List<DbDefenceSystem> Defences { get; set; }
		public double Size { get; set; }
		public double Armor { get; set; }
		public int ExpUnlock { get; set; }
		public Rarity ShipRarity { get; set; }

		public List<DbShip> ShipsOfThisTemplate { get; set; }

		/// <summary>
		/// creates new <see cref="DbShip"/> of this <see cref="DbShipTemplate"/>. Remember to get this <see cref="DbShipTemplate"/> and owning <see cref="DbPlayer"/> for DB!!
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public DbShip GenerateNewShipOfThisTemplate(DbPlayer owner) {
			return new DbShip(owner, 0, this);
		}
	}

	[Table("ships")]
	public class DbShip {

		public DbShip() { }

		public DbShip(DbPlayer owner, int shipExp, DbShipTemplate shipBaseStats) {
			Owner = owner;
			ShipExp = shipExp;
			ShipBaseStats = shipBaseStats;
			Fleets = new List<DbFleet>();
			IsActive = true;
		}

		public DbShip(int id, DbPlayer owner, int shipExp, DbShipTemplate shipBaseStats)
			: this(owner, shipExp, shipBaseStats) {
			Id = id;
		}

		public int Id { get; set; }
		public DbPlayer Owner { get; set; }
		public int ShipExp { get; set; }
		public DbShipTemplate ShipBaseStats { get; set; }
		public bool IsActive { get; set; }

		public List<DbFleet> Fleets { get; set; }

		public Ship ToShip() {
			List<Weapon> nonDbWeapons = new List<Weapon>();
			List<DefenceSystem> nonDbDefenceSystems = new List<DefenceSystem>();
			foreach(DbWeapon weapon in ShipBaseStats.Weapons) {
				nonDbWeapons.Add(weapon.ToWeapon());
			}
			foreach(DbDefenceSystem defSystem in ShipBaseStats.Defences) {
				nonDbDefenceSystems.Add(defSystem.ToDefenceSystem());
			}
			return new Ship(Id, ShipBaseStats.Name, ShipBaseStats.Faction, ShipBaseStats.Cost, ShipBaseStats.Evasion, ShipBaseStats.Hp, ShipBaseStats.Size,
				ShipBaseStats.Armor, nonDbWeapons, nonDbDefenceSystems, ShipBaseStats.ExpUnlock, ShipExp, ShipBaseStats.ShipRarity, Server.BaseModifiers.BaseShipStatsExpModifier*ShipExp);
		}
	}

	[Table("fleets")]
	public class DbFleet {

		public DbFleet() { }

		public DbFleet(DbPlayer owner, List<DbShip> ships, string name) {
			Owner = owner;
			Ships = ships;
			Name = name;
			IsActive = true;
		}

		public DbFleet(int id, DbPlayer owner, List<DbShip> ships, string name) : this(owner, ships, name) {
			Id = id;
		}

		public DbPlayer Owner { get; set; }
		public List<DbShip> Ships { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }
		public bool IsActive { get; set; }

		public Fleet ToFleet() {
			List<Ship> nonDbShips = new List<Ship>();
			foreach(DbShip ship in Ships) {
				nonDbShips.Add(ship.ToShip());
			}
			return new Fleet(Id, Name, Owner.ToPlayer(), nonDbShips);
		}
	}

	[Table("Game_History")]
	public class DbGameHistory {
		public DbGameHistory() { }

		public DbGameHistory( DbPlayer winner, DbPlayer loser, DbFleet winnerFleet, DbFleet loserFleet, bool wasDraw, DateTime gameDate) {
			Winner = winner;
			Loser = loser;
			WinnerFleet = winnerFleet;
			LoserFleet = loserFleet;
			WasDraw = wasDraw;
			GameDate = gameDate;
		}

		public DbGameHistory(int id, DbPlayer winner, DbPlayer loser, DbFleet winnerFleet, DbFleet loserFleet, bool wasDraw, DateTime gameDate)
			: this(winner, loser, winnerFleet, loserFleet, wasDraw, gameDate) {
			Id = id;
		}

		public int Id { get; set; }
		public DbPlayer Winner { get; set; }
		public DbPlayer Loser { get; set; }
		public DbFleet WinnerFleet { get; set; }
		public DbFleet LoserFleet { get; set; }
		public bool WasDraw { get; set; }
		public DateTime GameDate { get; set; }

		public GameHistory ToGameHistory(bool fullEntry) {
			if(fullEntry) {
				return new GameHistory(Id, Winner.ToPlayer(), Loser.ToPlayer(), WinnerFleet.ToFleet(), LoserFleet.ToFleet(), WasDraw, GameDate);
			}
			else {
				return new GameHistory(Id, Winner.ToPlayer(), Loser.ToPlayer(), new Fleet(WinnerFleet.Name, Winner.ToPlayer(), new List<Ship>()), 
					new Fleet(LoserFleet.Name, Loser.ToPlayer(), new List<Ship>()), WasDraw, GameDate);
			}
		}
	}

	[Table("lootboxes")]
	public class DbLootBox {
		public int Id { get; set; }
		public int Cost { get; set; }
		public string Name { get; set; }
		public double CommonChance { get; set; }
		public double RareChance { get; set; }
		public double VeryRareChance { get; set; }
		public double LegendaryChance { get; set; }
		public int NumberOfShips { get; set; }

		public DbLootBox() { }

		public DbLootBox(int cost, string name, double commonChance, double rareChance, double veryRareChance, double legendaryChance, int numberOfShips) {
			Cost = cost;
			Name = name;
			CommonChance = commonChance;
			RareChance = rareChance;
			VeryRareChance = veryRareChance;
			LegendaryChance = legendaryChance;
			NumberOfShips = numberOfShips;
		}

		public DbLootBox(int id, int cost, string name, double commonChance, double rareChance, double veryRareChance, double legendaryChance, int numberOfShips) :
			this(cost, name, commonChance, rareChance, veryRareChance, legendaryChance, numberOfShips) {
			Id = id;
		}

		public LootBox ToLootBox() {
			Dictionary<Rarity, double> chancesForRarities = new Dictionary<Rarity, double>() {
				{ Rarity.COMMON, CommonChance },
				{ Rarity.RARE, RareChance },
				{ Rarity.VERY_RARE, VeryRareChance },
				{ Rarity.LEGENDARY, LegendaryChance }
			};
			return new LootBox(Id, Cost, Name, chancesForRarities, NumberOfShips);
		}
	}

	[Table("base_modifiers")]
	public class DbBaseModifiers {
		public int Id { get; set; }

		//weaponTypeRangeMultMap
		public double KineticRange { get; set; }

		public double LaserRange { get; set; }

		public double MissileRange { get; set; }

		//defTypeToWepTypeMap
		public double KineticPD { get; set; }
		public double KineticShield { get; set; }
		public double KineticIF { get; set; }

		public double LaserPD { get; set; }
		public double LaserShield { get; set; }
		public double LaserIF { get; set; }

		public double MissilePD { get; set; }
		public double MissileShield { get; set; }
		public double MissileIF { get; set; }

		//exp and fleet size modifiers
		public double BaseShipStatsExpModifier { get; set; }
		public double FleetSizeExpModifier { get; set; }
		public int BaseFleetMaxSize { get; set; }
		public int MaxAbsoluteFleetSize { get; set; }
		public int MaxShipExp { get; set; }
		public int MaxShipsInLine { get; set; }
		public int MaxFleetsPerPlayer { get; set; }

		//maxShipsPerPlayer
		public int MaxShipsPerPlayer { get; set; }

		//starting money
		public int StartingMoney { get; set; }

		//exp gain values
		public int ExpForVictory { get; set; }
		public int ExpForLoss { get; set; }

		//money gain values
		public int MoneyForVictory { get; set; }
		public int MoneyForLoss {get; set; }




		public BaseModifiers ToBaseModifiers() {
			Dictionary<WeaponType, double> weaponTypeRangeMultMap = new Dictionary<WeaponType, double>() {
				{ WeaponType.KINETIC, KineticRange },
				{ WeaponType.LASER, LaserRange },
				{ WeaponType.MISSILE, MissileRange }
			};
			Dictionary<Tuple<DefenceSystemType, WeaponType>, double> defTypeToWepTypeMap = new Dictionary<Tuple<DefenceSystemType, WeaponType>, double> {
				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.KINETIC), KineticPD },
				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.SHIELD, WeaponType.KINETIC), KineticShield },
				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.KINETIC), KineticIF },

				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.LASER), LaserPD },
				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.SHIELD, WeaponType.LASER), LaserShield },
				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.LASER), LaserIF },

				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.MISSILE), MissilePD },
				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.SHIELD, WeaponType.MISSILE), MissileShield },
				{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.MISSILE), MissileIF }
			};
			return new BaseModifiers(weaponTypeRangeMultMap, defTypeToWepTypeMap, BaseShipStatsExpModifier, MaxShipsPerPlayer, StartingMoney, ExpForVictory, ExpForLoss, 
				FleetSizeExpModifier, MaxAbsoluteFleetSize, MaxShipExp, BaseFleetMaxSize, MaxShipsInLine, MaxFleetsPerPlayer, MoneyForVictory, MoneyForLoss);
		}
	}

}
