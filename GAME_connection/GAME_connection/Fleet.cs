using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Fleet {
		private Player owner;
		private List<Ship> ships;

		public Fleet() {}

		public Fleet(Player owner, List<Ship> ships) {
			this.Owner = owner;
			this.Ships = ships;
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
	}

}
