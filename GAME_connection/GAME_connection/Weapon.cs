using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Weapon {
		private int id;
		private string name;
		private Faction faction;
		private double damage;	//of one projectile
		private int numberOfProjectiles;
		private WeaponType weaponType;
		private double apEffectivity;	//effectivity against armor (dmg after armor calculation * this multiplier, dmg cant be bigger than before armor calculation)
		private double rangeMultiplier; //indicates how important is range for this specific weapon - from 0 to 1
		private double chanceToHit; //of one projectile (from 0.0 to 1.0) - this is the base chance to hit of this weapon, it is later used against distance, weapon type to range mult and this weapons rangeMultiplier

		public Weapon() { }

		public Weapon(int id, string name, Faction faction, double damage, int numberOfProjectiles, WeaponType weaponType, double apEffectivity, double rangeMultiplier, double chanceToHit) {
			this.Id = id;
			this.Name = name;
			this.Faction = faction;
			this.Damage = damage;
			this.NumberOfProjectiles = numberOfProjectiles;
			this.WeaponType = weaponType;
			this.ApEffectivity = apEffectivity;
			this.RangeMultiplier = rangeMultiplier;
			this.ChanceToHit = chanceToHit;
		}

		public string Name { get => name; set => name = value; }
		public Faction Faction { get => faction; set => faction = value; }
		public double Damage { get => damage; set => damage = value; }
		public int NumberOfProjectiles { get => numberOfProjectiles; set => numberOfProjectiles = value; }
		public WeaponType WeaponType { get => weaponType; set => weaponType = value; }
		public double RangeMultiplier { get => rangeMultiplier; set => rangeMultiplier = value; }
		public double ChanceToHit { get => chanceToHit; set => chanceToHit = value; }
		public double ApEffectivity { get => apEffectivity; set => apEffectivity = value; }
		public int Id { get => id; set => id = value; }
	}
}
