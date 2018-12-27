using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;
using GAME_Validator;

namespace GAME_Server {
	/// <summary>
	/// used to validate GameRoom specific object and object that need to be validated with DataBase
	/// </summary>
	public static class GameServerValidator {

		public static readonly string OK = "validation_ok";

		/// <summary>
		/// returns <see langword="false"/> if validation failed, and <see langword="true"/> if validation was successful. 
		/// Checks: fleet name unique, all ships owned by player, fleet cost limit, ship number in fleet limit, no duplicates, ships must belong to one faction
		/// </summary>
		/// <param name="player"></param>
		/// <param name="fleet"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		public static string ValidateFleet(Player player, Fleet fleet, IGameDataBase database, bool isNew) {	//tested - OK
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

		public static string ValidatePlayerBoard(PlayerGameBoard playerGameBoard, Fleet selectedFleet) {		//tested - OK
			List<Ship> allShipsOnBoard = new List<Ship>();
			allShipsOnBoard.AddRange(playerGameBoard.Board[Line.SHORT]);
			allShipsOnBoard.AddRange(playerGameBoard.Board[Line.MEDIUM]);
			allShipsOnBoard.AddRange(playerGameBoard.Board[Line.LONG]);
			foreach(Ship ship in allShipsOnBoard) {
				if(!selectedFleet.Ships.Any(fleetShip => fleetShip.Id == ship.Id)) return FailureReasons.INVALID_ID;        //ship on board must be in selected fleet
			}
			foreach(Ship ship in allShipsOnBoard) {
				int checkedId = ship.Id;
				int count = 0;
				foreach(Ship ship2 in allShipsOnBoard) {
					if (ship2.Id == checkedId) count++;
				}
				if (count > 1) return FailureReasons.DUPLICATES_NOT_ALLOWED;
			}
			int maxShipsInLine = Server.BaseModifiers.MaxShipsInLine;
			if (playerGameBoard.Board[Line.SHORT].Count > maxShipsInLine || playerGameBoard.Board[Line.MEDIUM].Count > maxShipsInLine || playerGameBoard.Board[Line.LONG].Count > maxShipsInLine)
				return FailureReasons.TOO_MANY_SHIPS_IN_LINE;		//too many ships in line

			return OK;
		}

		public static string ValidateMove(Move playerMove, PlayerGameBoard playerBoard, PlayerGameBoard enemyBoard) {       //tested - OK
			Dictionary<Line, int> numberOfShipsInLine = new Dictionary<Line, int> {
				{ Line.SHORT, playerBoard.Board[Line.SHORT].Count },		//start with number of ship that already are on board in given lines
				{ Line.MEDIUM, playerBoard.Board[Line.MEDIUM].Count },
				{ Line.LONG, playerBoard.Board[Line.LONG].Count }
			};
			List<int> movingShips = new List<int>();
			List<int> shipsThatAlreadyMadeMove = new List<int>();
			//first check if ShipPosition index is ok
			foreach(Tuple<ShipPosition, Line> move in playerMove.MoveList) {	//check if index of ship out of range in origin line
				if (move.Item1.ShipIndex >= numberOfShipsInLine[move.Item1.Line]) return FailureReasons.INDEX_IN_LINE_OUT_OF_RANGE + " move from: " + move.Item1.Line + " " + move.Item1.ShipIndex;
			}
			//check if there line capacity is ok
			foreach(Tuple<ShipPosition, Line> move in playerMove.MoveList) {
				movingShips.Add(playerBoard.Board[move.Item1.Line][move.Item1.ShipIndex].Id);
				shipsThatAlreadyMadeMove.Add(playerBoard.Board[move.Item1.Line][move.Item1.ShipIndex].Id);

				numberOfShipsInLine[move.Item2] += 1;		//add one ship to destination
				numberOfShipsInLine[move.Item1.Line] -= 1;	//remove one ship from origin
			}
			foreach(var move in playerMove.MoveList) {
				if (Math.Abs(move.Item2 - move.Item1.Line) > 1) return FailureReasons.MOVE_DISTANCE_TOO_LONG;
			}
			foreach(var line in numberOfShipsInLine) {
				if (line.Value > Server.BaseModifiers.MaxShipsInLine) return FailureReasons.TOO_MANY_SHIPS_IN_LINE  + " " + line.Key;
			}
			foreach(var move in playerMove.MoveList) {
				if (shipsThatAlreadyMadeMove.Where(id => id == playerBoard.Board[move.Item1.Line][move.Item1.ShipIndex].Id).Count() > 1) return FailureReasons.ONE_SHIP_MANY_MOVES;
			}
			foreach(Tuple<ShipPosition, ShipPosition> move in playerMove.AttackList) {  //check if idexes of ships are ok
				if(move.Item1.ShipIndex >= playerBoard.Board[move.Item1.Line].Count) return FailureReasons.INDEX_IN_LINE_OUT_OF_RANGE + " attack from: " + move.Item1.Line + " " + move.Item1.ShipIndex;
				if (move.Item2.ShipIndex >= playerBoard.Board[move.Item2.Line].Count) return FailureReasons.INDEX_IN_LINE_OUT_OF_RANGE + " attack to: " + move.Item2.Line + " " + move.Item2.ShipIndex;
			}
			//than check if attacking ships are not in moving ships
			foreach(Tuple<ShipPosition, ShipPosition> move in playerMove.AttackList) {
				shipsThatAlreadyMadeMove.Add(playerBoard.Board[move.Item1.Line][move.Item1.ShipIndex].Id);
			}
			foreach(Tuple<ShipPosition, ShipPosition> move in playerMove.AttackList) {
				if (shipsThatAlreadyMadeMove.Where(id => id == playerBoard.Board[move.Item1.Line][move.Item1.ShipIndex].Id).Count() > 1) return FailureReasons.ONE_SHIP_MANY_MOVES;
			}

			return OK;
		}

	}

}
