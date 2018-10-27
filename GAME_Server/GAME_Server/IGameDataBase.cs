using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	interface IGameDataBase {
		DbFleet GetFleetWithId(int id);

		DbShip GetShipWithId(int id);

		DbPlayer GetPlayerWithUsername(string username);

		List<DbFleet> GetAllFleetsOfPlayer(Player player);

		List<DbShip> GetAllShips();

		List<Faction> GetAllFactions();

		BaseModifiers GetBaseModifiers();

		bool FleetNameIsUnique(Player player, string fleetName);

		bool PlayerExists(Player player);

		bool PlayerNameIsUnique(Player player);

		void AddPlayer(DbPlayer player);

		void AddShip(DbShip ship);

		void AddFaction(Faction faction);

		void AddWeapon(DbWeapon weapon);

		void AddDefenceSystem(DbDefenceSystem defence);

		List<DbWeapon> GetAllWeapons();

		List<DbDefenceSystem> GetAllDefences();

		void UpdateShip(DbShip newData);

		bool RemoveShipWithId(int id);

		List<DbShip> GetShipsAvailableForExp(int exp);

		bool ValidateUser(Player player);

		void AddFleet(Fleet fleet);

		//TODO
		//deletes, updates and inserts
	}
}
