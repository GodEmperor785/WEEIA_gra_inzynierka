using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	public class MySqlDataBase : IGameDataBase {

		internal MySqlDataBase() {}

		public bool CheckIfFleetNameUnique(Player player, string fleetName) {
			throw new NotImplementedException();
		}

		public List<Faction> GetAllFactions() {
			throw new NotImplementedException();
		}

		public List<Fleet> GetAllFleetsOfPlayer(Player player) {
			throw new NotImplementedException();
		}

		public List<Ship> GetAllShips() {
			throw new NotImplementedException();
		}

		public BaseModifiers GetBaseModifiers() {
			throw new NotImplementedException();
		}

		public Fleet GetFleetWithId(int id) {
			throw new NotImplementedException();
		}

		public Player GetPlayerWithUsername(string username) {
			throw new NotImplementedException();
		}

		public Ship GetShipWithId(int id) {
			throw new NotImplementedException();
		}
	}
}
