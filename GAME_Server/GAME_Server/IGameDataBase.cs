using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	interface IGameDataBase {
		Fleet GetFleetWithId(int id);

		Ship GetShipWithId(int id);

		Player GetPlayerWithUsername(string username);

		List<Fleet> GetAllFleetsOfPlayer(Player player);

		List<Ship> GetAllShips();

		List<Faction> GetAllFactions();

		BaseModifiers GetBaseModifiers();
	}
}
