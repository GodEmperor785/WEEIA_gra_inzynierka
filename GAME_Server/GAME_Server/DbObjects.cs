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

		public DbPlayer(string username, string password, int experience, int maxFleetPoints, int id, int gamesPlayed, int gamesWon, List<DbShip> ownedShips) {
			Username = username;
			Password = password;
			Experience = experience;
			MaxFleetPoints = maxFleetPoints;
			Id = id;
			GamesPlayed = gamesPlayed;
			GamesWon = gamesWon;
			OwnedShips = ownedShips;
		}

		public string Username { get; set; }
		public string Password { get; set; }
		public int Experience { get; set; }
		public int MaxFleetPoints { get; set; }
		public int Id { get; set; }
		public int GamesPlayed { get; set; }
		public int GamesWon { get; set; }

		public List<DbShip> OwnedShips { get; set; }

		public Player ToPlayer() {
			return new Player(Id, Username, Password, Experience, MaxFleetPoints, GamesPlayed, GamesWon);
		}
	}

	[Table("weapons")]
	public class DbWeapon {

		public DbWeapon() { }

		public DbWeapon(string name, Faction faction, double damage, int numberOfProjectiles, WeaponType weaponType, double rangeMultiplier, double chanceToHit, double apEffectivity, int id) {
			Name = name;
			Faction = faction;
			Damage = damage;
			NumberOfProjectiles = numberOfProjectiles;
			WeaponType = weaponType;
			RangeMultiplier = rangeMultiplier;
			ChanceToHit = chanceToHit;
			ApEffectivity = apEffectivity;
			Id = id;
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

		public DbDefenceSystem(string name, Faction faction, double defenceValue, DefenceSystemType systemType, double defAgainstKinetic, double defAgainstLaser, double defAgainstMissile, int id) {
			Name = name;
			Faction = faction;
			DefenceValue = defenceValue;
			SystemType = systemType;
			DefAgainstKinetic = defAgainstKinetic;
			DefAgainstLaser = defAgainstLaser;
			DefAgainstMissile = defAgainstMissile;
			Id = id;
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

		public DbShip(int id, string name, Faction faction, int cost, double evasion, double hp, List<DbWeapon> weapons, List<DbDefenceSystem> defences, double size, double armor, int expUnlock) {
			Id = id;
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

		public DbFleet(DbPlayer owner, List<DbShip> ships, string name, int id) {
			Owner = owner;
			Ships = ships;
			Name = name;
			Id = id;
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

		public DbGameHistory(int id, DbPlayer winner, DbPlayer loser, DbFleet winnerFleet, DbFleet loserFleet, bool wasDraw, DateTime gameDate) {
			Id = id;
			Winner = winner;
			Loser = loser;
			WinnerFleet = winnerFleet;
			LoserFleet = loserFleet;
			WasDraw = wasDraw;
			GameDate = gameDate;
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
