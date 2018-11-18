using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	internal static class GameValidator {

		public static readonly string OK = "validation_ok";

		/// <summary>
		/// returns <see langword="false"/> if validation failed, and <see langword="true"/> if validation was successful. 
		/// Checks: fleet name unique, all ships owned by player, fleet cost limit, ship number in fleet limit, no duplicates, ships must belong to one faction
		/// </summary>
		/// <param name="player"></param>
		/// <param name="fleet"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		internal static string ValidateFleet(Player player, Fleet fleet, IGameDataBase database) {
			if (!database.FleetNameIsUnique(player, fleet.Name)) return FailureReasons.FLEET_NAME_NOT_UNIQUE;
			int fleetSize = 0;
			List<int> shipsIds = new List<int>();
			if (fleet.Ships.Count > (Server.BaseModifiers.MaxShipsInLine * 3)) return FailureReasons.FLEET_SHIP_COUNT_LIMIT;    //too many ships in fleet
			if (fleet.Ships.Count == 0) return FailureReasons.ZERO_SHIPS_IN_FLEET;
			Faction fleetFaction = fleet.Ships.First().Faction;		//first ship defines faction
			foreach (Ship fleetShip in fleet.Ships) {
				if (shipsIds.Contains(fleetShip.Id)) return FailureReasons.DUPLICATES_NOT_ALLOWED;  //no duplicates
				shipsIds.Add(fleetShip.Id);
				DbShip dbShip = database.GetShipWithId(fleetShip.Id);
				if (dbShip == null) throw new NullReferenceException("Ship does not exist!");		//invalid ship id!
				fleetSize += dbShip.ShipBaseStats.Cost;
				if (dbShip.ShipBaseStats.Faction.Equals(fleetFaction)) return FailureReasons.FACTION_MUST_BE_SAME;		//all ships must belong to the same faction
				if (dbShip.Owner.Id != player.Id) return FailureReasons.INVALID_ID;		//ship must belong to player
			}
			if (fleetSize > Server.BaseModifiers.GetPlayersMaxFleetSize(player)) return FailureReasons.FLEET_SIZE_LIMIT;	//fleet size must be in allowed range
			return OK;
		}

	}

}
