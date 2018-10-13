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

	/// <summary>
	/// Used to send base modifiers to client (to calculate actual modifiers)
	/// </summary>
	[Serializable]
	public class BaseModifiers {
		private Dictionary<WeaponType, double> weaponTypeRangeMultMap;
		private Dictionary<Tuple<DefenceSystemType, WeaponType>, double> defTypeToWepTypeMap;

		public BaseModifiers(Dictionary<WeaponType, double> weaponTypeRangeMultMap, Dictionary<Tuple<DefenceSystemType, WeaponType>, double> defTypeToWepTypeMap) {
			this.WeaponTypeRangeMultMap = weaponTypeRangeMultMap;
			this.DefTypeToWepTypeMap = defTypeToWepTypeMap;
		}

		public Dictionary<WeaponType, double> WeaponTypeRangeMultMap { get => weaponTypeRangeMultMap; set => weaponTypeRangeMultMap = value; }
		public Dictionary<Tuple<DefenceSystemType, WeaponType>, double> DefTypeToWepTypeMap { get => defTypeToWepTypeMap; set => defTypeToWepTypeMap = value; }
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
