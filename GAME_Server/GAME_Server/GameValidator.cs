using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	public static class GameValidator {

		public static readonly string OK = "validation_ok";

		/// <summary>
		/// returns <see langword="false"/> if validation failed, and <see langword="true"/> if validation was successful. 
		/// Checks: fleet name unique, all ships owned by player, fleet cost limit, ship number in fleet limit, no duplicates, ships must belong to one faction
		/// </summary>
		/// <param name="player"></param>
		/// <param name="fleet"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		public static string ValidateFleet(Player player, Fleet fleet, IGameDataBase database, bool isNew) {
			if (isNew) {	//if fleet is added, not modified check if player can have one more fleet
				if (database.GetPlayerFleetCount(player) >= Server.BaseModifiers.MaxFleetsPerPlayer) return FailureReasons.TOO_MANY_FLEETS;
				if (!database.FleetNameIsUnique(player, fleet.Name)) return FailureReasons.FLEET_NAME_NOT_UNIQUE;
			}
			else {  //if update
				DbFleet oldFleet = database.GetFleetWithId(fleet.Id);
				if(oldFleet.Owner.Id != player.Id) return FailureReasons.INVALID_ID;        //modified fleet must belong to player
				if(oldFleet.Name != fleet.Name) {		
					if (!database.FleetNameIsUnique(player, fleet.Name)) return FailureReasons.FLEET_NAME_NOT_UNIQUE;
				}
			}
			int fleetSize = 0;
			List<int> shipsIds = new List<int>();
			if (fleet.Ships.Count > (Server.BaseModifiers.MaxShipsInLine * 3)) return FailureReasons.FLEET_SHIP_COUNT_LIMIT;    //too many ships in fleet
			if (fleet.Ships.Count == 0) return FailureReasons.ZERO_SHIPS_IN_FLEET;
			Faction fleetFaction = fleet.Ships.First().Faction;		//first ship defines faction
			foreach (Ship fleetShip in fleet.Ships) {
				if (shipsIds.Contains(fleetShip.Id)) return FailureReasons.DUPLICATES_NOT_ALLOWED;  //no duplicates
				shipsIds.Add(fleetShip.Id);
				DbShip dbShip = database.GetShipWithId(fleetShip.Id);
				if (dbShip == null) throw new NullReferenceException("Ship does not exist!");       //invalid ship id!
				if (!dbShip.IsActive) return FailureReasons.ELEMENT_NOT_ACTIVE;
				fleetSize += dbShip.ShipBaseStats.Cost;
				if (!dbShip.ShipBaseStats.Faction.Equals(fleetFaction)) return FailureReasons.FACTION_MUST_BE_SAME;		//all ships must belong to the same faction
				if (dbShip.Owner.Id != player.Id) return FailureReasons.INVALID_ID;		//ship must belong to player
			}
			if (fleetSize > Server.BaseModifiers.GetPlayersMaxFleetSize(player)) return FailureReasons.FLEET_SIZE_LIMIT;	//fleet size must be in allowed range
			return OK;
		}

		public static string ValidatePlayerBoard(PlayerGameBoard playerGameBoard, Fleet selectedFleet) {
			List<Ship> allShipsOnBoard = new List<Ship>();
			allShipsOnBoard.AddRange(playerGameBoard.ShortRange);
			allShipsOnBoard.AddRange(playerGameBoard.MediumRange);
			allShipsOnBoard.AddRange(playerGameBoard.LongRange);
			foreach(Ship ship in allShipsOnBoard) {
				if(!selectedFleet.Ships.Any(fleetShip => fleetShip.Id == ship.Id)) return FailureReasons.INVALID_ID;        //ship on board must be in selected fleet
			}
			int maxShipsInLine = Server.BaseModifiers.MaxShipsInLine;
			if (playerGameBoard.ShortRange.Count > maxShipsInLine || playerGameBoard.MediumRange.Count > maxShipsInLine || playerGameBoard.LongRange.Count > maxShipsInLine)
				return FailureReasons.TOO_MANY_SHIPS_IN_LINE;		//too many ships in line

			return OK;
		}

	}

}
