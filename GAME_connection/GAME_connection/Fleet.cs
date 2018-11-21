using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Fleet {
		private int id;
		private string name;
		private Player owner;
		private List<Ship> ships;

		public Fleet() {}

		public Fleet(string name, Player owner, List<Ship> ships) {
			this.Owner = owner;
			this.Ships = ships;
			this.Name = name;
		}

		public Fleet(int id, string name, Player owner, List<Ship> ships) : this(name, owner, ships) {
			this.Id = id;
		}

		public int GetFleetPointsSum() {
			int pointSum = 0;
			foreach(Ship ship in Ships) {
				pointSum += ship.Cost;
			}
			return pointSum;
		}

		public bool FleetPointsInAllowedRange() {
			if (GetFleetPointsSum() > owner.MaxFleetPoints) return false;
			else return true;
		}

		public Player Owner { get => owner; set => owner = value; }
		public List<Ship> Ships { get => ships; set => ships = value; }
		public string Name { get => name; set => name = value; }
		public int Id { get => id; set => id = value; }
	}

}
