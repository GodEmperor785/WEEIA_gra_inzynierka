using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	internal class InMemoryGameDataBase : IGameDataBase {
		//all ships, fleets, weapons, defence systems, players, factions, etc. go here!
		internal InMemoryGameDataBase() {
			//and possibly here
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
