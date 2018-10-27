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
		}

		public DbPlayer(int id, string username, string password, int experience, int maxFleetPoints, int gamesPlayed, int gamesWon, int money) 
			: this(username, password, experience, maxFleetPoints, gamesPlayed, gamesWon, money) {
			Id = id;
		}

		public DbPlayer(Player player) {
			Username = player.Username;
			Password = player.Password;
			Experience = player.Experience;
			MaxFleetPoints = player.MaxFleetPoints;
			Id = player.Id;
			GamesPlayed = player.GamesPlayed;
			GamesWon = player.GamesWon;
			OwnedShips = new List<DbShip>();
		}

		public string Username { get; set; }
		public string Password { get; set; }
		public int Experience { get; set; }
		public int MaxFleetPoints { get; set; }
		public int Id { get; set; }
		public int GamesPlayed { get; set; }
		public int GamesWon { get; set; }
		public int Money { get; set; }

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
			Ships = new List<DbShip>();
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
			Ships = new List<DbShip>();
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

		public List<DbShip> Ships { get; set; }

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
			Ships = new List<DbShip>();
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
			Ships = new List<DbShip>();
		}

		public string Name { get; set; }
		public Faction Faction { get; set; }
		public double DefenceValue { get; set; }
		public DefenceSystemType SystemType { get; set; }
		public double DefAgainstKinetic { get; set; }
		public double DefAgainstLaser { get; set; }
		public double DefAgainstMissile { get; set; }
		public int Id { get; set; }

		public List<DbShip> Ships { get; set; }

		public DefenceSystem ToDefenceSystem() {
			return new DefenceSystem(Id, Name, Faction, DefenceValue, SystemType, DefAgainstKinetic, DefAgainstLaser, DefAgainstMissile);
		}
	}

	[Table("ships")]
	public class DbShip {

		public DbShip() { }

		public DbShip(string name, Faction faction, int cost, double evasion, double hp, double size, double armor, List<DbWeapon> weapons, List<DbDefenceSystem> defences, int expUnlock) {
			Name = name;
			Faction = faction;
			Cost = cost;
			Evasion = evasion;
			Hp = hp;
			Weapons = weapons;
			Defences = defences;
			Size = size;
			Armor = armor;
			Fleets = new List<DbFleet>();
			PlayersOwningShip = new List<DbPlayer>();
			ExpUnlock = expUnlock;
		}

		public DbShip(int id, string name, Faction faction, int cost, double evasion, double hp, double size, double armor, List<DbWeapon> weapons, List<DbDefenceSystem> defences, int expUnlock)
			: this(name, faction, cost, evasion, hp, size, armor, weapons, defences, expUnlock) {
			Id = id;
		}

		public DbShip(Ship ship) {
			Id = ship.Id;
			Name = ship.Name;
			Faction = ship.Faction;
			Cost = ship.Cost;
			Evasion = ship.Evasion;
			Hp = ship.Hp;
			List<DbWeapon> weapons = new List<DbWeapon>();
			foreach(Weapon wep in ship.Weapons) weapons.Add(new DbWeapon(wep));
			Weapons = weapons;
			List<DbDefenceSystem> defences = new List<DbDefenceSystem>();
			foreach (DefenceSystem def in ship.Defences) defences.Add(new DbDefenceSystem(def));
			Defences = defences;
			Size = ship.Size;
			Armor = ship.Armor;
			Fleets = new List<DbFleet>();
			PlayersOwningShip = new List<DbPlayer>();
			ExpUnlock = ship.ExpUnlock;
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

		public List<DbFleet> Fleets { get; set; }
		public List<DbPlayer> PlayersOwningShip { get; set; }

		public Ship ToShip() {
			List<Weapon> nonDbWeapons = new List<Weapon>();
			List<DefenceSystem> nonDbDefenceSystems = new List<DefenceSystem>();
			foreach(DbWeapon weapon in Weapons) {
				nonDbWeapons.Add(weapon.ToWeapon());
			}
			foreach(DbDefenceSystem defSystem in Defences) {
				nonDbDefenceSystems.Add(defSystem.ToDefenceSystem());
			}
			return new Ship(Id, Name, Faction, Cost, Evasion, Hp, Size, Armor, nonDbWeapons, nonDbDefenceSystems, ExpUnlock);
		}
	}

	[Table("fleets")]
	public class DbFleet {

		public DbFleet() { }

		public DbFleet(DbPlayer owner, List<DbShip> ships, string name) {
			Owner = owner;
			Ships = ships;
			Name = name;
		}

		public DbFleet(int id, DbPlayer owner, List<DbShip> ships, string name) : this(owner, ships, name) {
			Id = id;
		}

		public DbFleet(Fleet fleet) {
			Owner = new DbPlayer(fleet.Owner);
			List<DbShip> ships = new List<DbShip>();
			foreach (Ship ship in fleet.Ships) ships.Add(new DbShip(ship));
			Ships = ships;
			Name = fleet.Name;
			Id = fleet.Id;
		}

		public DbPlayer Owner { get; set; }
		public List<DbShip> Ships { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }

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

		public GameHistory ToGameHistory() {
			return new GameHistory(Id, Winner.ToPlayer(), Loser.ToPlayer(), WinnerFleet.ToFleet(), LoserFleet.ToFleet(), WasDraw, GameDate);
		}
	}

	/*[Table("fleet_size_exp")]
	public class DbFleetSizeExpMapping {
		public int Id { get; set; }
		public int Experience { get; set; }
		public int MaxFleetSize { get; set; }
	}*/

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
			return new BaseModifiers(weaponTypeRangeMultMap, defTypeToWepTypeMap);
		}
	}

}
