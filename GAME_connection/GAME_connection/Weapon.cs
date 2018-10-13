using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Weapon {
		private string name;
		private Faction faction;
		private double damage;	//of one projectile
		private int numberOfProjectiles;
		private WeaponType weaponType;
		private double apEffectivity;	//effectivity against armor (dmg after armor calculation * this multiplier, dmg cant be bigger than before armor calculation)
		private double rangeMultiplier; //bigger multiplier - less damage, added to base weapontype multiplier(stored in DB)
		private double chanceToHit;	//of one projectile (maybe from 0 to 100?)

		public Weapon() { }

		public Weapon(string name, Faction faction, double damage, int numberOfProjectiles, WeaponType weaponType, double apEffectivity, double rangeMultiplier, double chanceToHit) {
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
	}
}
