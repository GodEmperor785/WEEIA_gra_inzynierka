using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Ship {
		private int id;
		private string name;
		private Faction faction;
		private int cost;
		private double evasion;
		private double hp;
		private double size;
		private double armor;
		private List<Weapon> weapons;
		private List<DefenceSystem> defences;
		private int expUnlock;      //exp necessary to unlock this ship
		private int shipExp;    //this ships exp
		private Rarity rarity;
		private double shipExpModifier;     //applies to: evasion, weapon-chanceToHit, defenceSystem-defenceValue, weapon-dmg (and maybe to armor and hp). Used like: realStats = baseStats + baseStats*(BaseModifiers.baseShipStatsExpModifier * shipExp) where shis in () is this modifier

		public Ship() { }

		public Ship(int id, string name, Faction faction, int cost, double evasion, double hp, double size, double armor, List<Weapon> weapons, List<DefenceSystem> defences, int expUnlock, int shipExp, Rarity rarity, double shipExpModifier) {
			this.id = id;
			this.name = name;
			this.faction = faction;
			this.cost = cost;
			this.evasion = evasion;
			this.hp = hp;
			this.size = size;
			this.armor = armor;
			this.weapons = weapons;
			this.defences = defences;
			this.expUnlock = expUnlock;
			this.shipExp = shipExp;
			this.rarity = rarity;
			this.shipExpModifier = shipExpModifier;
		}

		public string Name { get => name; set => name = value; }
		public Faction Faction { get => faction; set => faction = value; }
		public int Cost { get => cost; set => cost = value; }
		public double Evasion { get => evasion; set => evasion = value; }
		public double Hp { get => hp; set => hp = value; }
		public List<Weapon> Weapons { get => weapons; set => weapons = value; }
		public List<DefenceSystem> Defences { get => defences; set => defences = value; }
		public double Size { get => size; set => size = value; }
		public double Armor { get => armor; set => armor = value; }
		public int Id { get => id; set => id = value; }
		public int ExpUnlock { get => expUnlock; set => expUnlock = value; }
		public int ShipExp { get => shipExp; set => shipExp = value; }
		public Rarity Rarity { get => rarity; set => rarity = value; }
		public double ShipExpModifier { get => shipExpModifier; set => shipExpModifier = value; }
	}
}
