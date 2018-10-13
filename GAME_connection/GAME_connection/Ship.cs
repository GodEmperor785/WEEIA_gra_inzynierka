using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Ship {
		private string name;
		private Faction faction;
		private int cost;
		private double evasion;
		private double hp;
		private double size;
		private double armor;
		private List<Weapon> weapons;
		private List<DefenceSystem> defences;

		public Ship() { }

		public Ship(string name, Faction faction, int cost, double evasion, double hp, double size, double armor, List<Weapon> weapons, List<DefenceSystem> defences) {
			this.name = name;
			this.faction = faction;
			this.cost = cost;
			this.evasion = evasion;
			this.hp = hp;
			this.size = size;
			this.armor = armor;
			this.weapons = weapons;
			this.defences = defences;
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
	}
}
