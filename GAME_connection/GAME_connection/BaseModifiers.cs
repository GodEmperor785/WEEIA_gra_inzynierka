using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public enum WeaponType {
		KINETIC = 1,
		LASER = 2,
		MISSILE = 3
	}

	[Serializable]
	public enum DefenceSystemType {
		POINT_DEFENCE = 1,
		SHIELD = 2,
		INTEGRITY_FIELD = 3,
	}

	[Serializable]
	public enum Rarity {
		COMMON = 1,			//white or light grey
		RARE = 2,			//blue
		VERY_RARE = 3,		//purple
		LEGENDARY = 4		//gold
	}

	#region enum utils
	public static class GameEnumUtils {
		/// <summary>
		/// should be used to get string name of given <see cref="Rarity"/> enum. Use like: string x = Rarity.COMMON.GetRarityName();
		/// </summary>
		/// <param name="rarity"></param>
		/// <returns></returns>
		public static string GetRarityName(this Rarity rarity) {
			switch (rarity) {
				case Rarity.COMMON:
					return "Common";
				case Rarity.RARE:
					return "Rare";
				case Rarity.VERY_RARE:
					return "Very rare";
				case Rarity.LEGENDARY:
					return "Legendary";
				default:
					return "";
			}
		}

		/// <summary>
		/// should be used to get string name of given <see cref="WeaponType"/> enum. Use like: string x = WeaponType.KINETIC.GetWeaponTypeyName();
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetWeaponTypeyName(this WeaponType type) {
			switch (type) {
				case WeaponType.KINETIC:
					return "Kinetic";
				case WeaponType.LASER:
					return "Laser";
				case WeaponType.MISSILE:
					return "Missile";
				default:
					return "";
			}
		}

		/// <summary>
		/// should be used to get string name of given <see cref="DefenceSystemType"/> enum. Use like: string x = DefenceSystemType.SHIELD.GetDefenceSystemType();
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetDefenceSystemType(this DefenceSystemType type) {
			switch (type) {
				case DefenceSystemType.POINT_DEFENCE:
					return "Point defence";
				case DefenceSystemType.SHIELD:
					return "Shield";
				case DefenceSystemType.INTEGRITY_FIELD:
					return "Integrity field";
				default:
					return "";
			}
		}

		/// <summary>
		/// returns list af all values of given enum. Use like: GameEnumUtils.GetValues<Rarity>()
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetValues<T>() {
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
	}
	#endregion

	/// <summary>
	/// Used to send base modifiers to client (to calculate actual modifiers)
	/// </summary>
	[Serializable]
	public class BaseModifiers {
		private Dictionary<WeaponType, double> weaponTypeRangeMultMap;
		private Dictionary<Tuple<DefenceSystemType, WeaponType>, double> defTypeToWepTypeMap;

		private double baseShipStatsExpModifier;
		private int maxShipsPerPlayer;
		private int startingMoney;
		private int expForVictory;
		private int expForLoss;
		private int expForDraw;
		private double fleetSizeExpModifier;    //maxFleetSize = baseFleetMaxSize + (fleetSizeExpModifier * players_exp)
		private int baseFleetMaxSize;
		private int maxAbsoluteFleetSize;
		private int maxShipExp;
		private int maxShipsInLine;
		private int maxFleetsPerPlayer;

		public BaseModifiers() { }

		public BaseModifiers(Dictionary<WeaponType, double> weaponTypeRangeMultMap, Dictionary<Tuple<DefenceSystemType, WeaponType>, double> defTypeToWepTypeMap,
				double baseShipStatsExpModifier, int maxShipsPerPlayer, int startingMoney, int expForVictory, int expForLoss, double fleetSizeExpModifier, int maxAbsoluteFleetSize,
				int maxShipExp, int baseFleetMaxSize, int maxShipsInLine, int maxFleetsPerPlayer) {
			this.WeaponTypeRangeMultMap = weaponTypeRangeMultMap;
			this.DefTypeToWepTypeMap = defTypeToWepTypeMap;
			this.BaseShipStatsExpModifier = baseShipStatsExpModifier;
			this.MaxShipsPerPlayer = maxShipsPerPlayer;
			this.StartingMoney = startingMoney;
			this.ExpForVictory = expForVictory;
			this.ExpForLoss = expForLoss;
			this.ExpForDraw = (expForVictory + expForLoss) / 2;
			this.FleetSizeExpModifier = fleetSizeExpModifier;
			this.MaxAbsoluteFleetSize = maxAbsoluteFleetSize;
			this.MaxShipExp = maxShipExp;
			this.BaseFleetMaxSize = baseFleetMaxSize;
			this.MaxShipsInLine = maxShipsInLine;
			this.MaxFleetsPerPlayer = maxFleetsPerPlayer;
		}

		public Dictionary<WeaponType, double> WeaponTypeRangeMultMap { get => weaponTypeRangeMultMap; set => weaponTypeRangeMultMap = value; }
		public Dictionary<Tuple<DefenceSystemType, WeaponType>, double> DefTypeToWepTypeMap { get => defTypeToWepTypeMap; set => defTypeToWepTypeMap = value; }
		public double BaseShipStatsExpModifier { get => baseShipStatsExpModifier; set => baseShipStatsExpModifier = value; }
		public int MaxShipsPerPlayer { get => maxShipsPerPlayer; set => maxShipsPerPlayer = value; }
		public int StartingMoney { get => startingMoney; set => startingMoney = value; }
		public int ExpForVictory { get => expForVictory; set => expForVictory = value; }
		public int ExpForLoss { get => expForLoss; set => expForLoss = value; }
		public int ExpForDraw { get => expForDraw; set => expForDraw = value; }
		public double FleetSizeExpModifier { get => fleetSizeExpModifier; set => fleetSizeExpModifier = value; }
		public int MaxAbsoluteFleetSize { get => maxAbsoluteFleetSize; set => maxAbsoluteFleetSize = value; }
		public int MaxShipExp { get => maxShipExp; set => maxShipExp = value; }
		public int BaseFleetMaxSize { get => baseFleetMaxSize; set => baseFleetMaxSize = value; }
		public int MaxShipsInLine { get => maxShipsInLine; set => maxShipsInLine = value; }
		public int MaxFleetsPerPlayer { get => maxFleetsPerPlayer; set => maxFleetsPerPlayer = value; }

		public int GetPlayersMaxFleetSize(Player player) {
			int calculatedMaxFleetSize = (int)(BaseFleetMaxSize + (FleetSizeExpModifier * player.Experience));
			if (calculatedMaxFleetSize < MaxAbsoluteFleetSize) return calculatedMaxFleetSize;
			else return MaxAbsoluteFleetSize;
		}
	}

	/*
	 * kolejnosc:
	 *	- bron vs odleglosc i chanceToHit
	 *	- bron vs obrona
	 *	- bron vs pancerz
	 *	- bron vs hp
	 * pseudokod wstepny na obliczenia(pewnie wiecej bedzie zmiennych lokalnych):
	
	foreach(weapon in shipWeapons) {		//dla kazdej broni
		for(int i = 0; i < weapon.numberOfProjectiles; i++) {	//dla kazdego pocisku
			if(weaponHits(weapon, range)) {
				[weaponHits(...):	weapon.chanceToHit -= ( weapon.chanceToHit * ( distance * weaponTypeRangeMultMap(weapon.WeaponType) * weapon.rangeMultiplier );
									... chanceToHit vs. (losowosc + evasion + 1/size) ...
									//chanceToHit prawdopodobnie jako procent i potem losowanie liczby i sprawdzenie czy mniejsza od chanceToHit
				]
				foreach(defenceSystem in enemyDefenceSystems) {		//dla kazdego systemu obronnego
					weapon.dmg -= ( defenceSystem.defenceValue * defTypeToWepTypeMap(new Tuple<weapon.WepaonType, defenceSystem.DefenceSystemType>)
									* defMultAgainstWepTypeMap(weapon.WepaonType));
				}
				tempApDmg = weapon.dmg;
				weapon.dmg -= sqrt(enemyArmor);	//lub inna funkcja ktora ma ujemna druga pochodna - rosnie coraz wolniej
				weapon.dmg *= weapon.apEffectivity;
				if(weapon.dmg > tempApDmg) weapon.dmg = tempApDmg;	//dmg po pancerzu nie moze byc wieksze od dmg przed pancerzem
				if(weapon.dmg < 0) weapon.dmg = 0;	//bron nie moze leczyc...
				enemyHp -= weapon.dmg;
			}
			else do nothing;
		}
	}
	 */
}
