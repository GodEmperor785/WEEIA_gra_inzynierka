using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_Validator {
	/// <summary>
	/// Provides reasons for failures used by Server and AdminApp
	/// </summary>
	public static class FailureReasons {
		public static readonly string INVALID_PACKET = "Invalid type of received packet";
		public static readonly string INVALID_PACKET_TYPE = "Invalid packet OperationType received";
		public static readonly string INCORRECT_LOGIN = "Incorrect login data for username: ";
		public static readonly string USERNAME_ALREADY_EXISTS = "Username already exists: ";
		public static readonly string CLIENT_DISCONNECTED = "Connection ended without proper disconnect";
		public static readonly string NOT_ENOUGH_MONEY = "Not enough money";
		public static readonly string INVALID_ID = "Invalid object data received - invalid id";
		public static readonly string TOO_MANY_SHIPS = "You have too many ships - can't buy new ones until you sell some";
		public static readonly string TOO_MANY_FLEETS = "You have too many fleets";
		public static readonly string FLEET_NAME_NOT_UNIQUE = "Fleet name not unique";
		public static readonly string FLEET_SHIP_COUNT_LIMIT = "Too many ships in fleet";
		public static readonly string FLEET_SIZE_LIMIT = "Cost of ships in fleet too big";
		public static readonly string NO_SHIPS_IN_FLEET = "Fleet needs to have at least 1 ship in it";
		public static readonly string DUPLICATES_NOT_ALLOWED = "no duplicates in lists are allowed";
		public static readonly string ZERO_SHIPS_IN_FLEET = "fleet must contain at least one ship";
		public static readonly string FACTION_MUST_BE_SAME = "All elements must belong to the same faction";
		public static readonly string NO_SUCH_ROOM = "Room already full or deleted";
		public static readonly string WRONG_ROOM_PASSWORD = "Wrong room password";
		public static readonly string ROOM_FULL = "Room is already full";
		public static readonly string CANT_ABANDON = "Cannot abandon room that is already full";
		public static readonly string NO_FLEET_SELECTED = "No fleet selected for game";
		public static readonly string TOO_MANY_SHIPS_IN_LINE = "Too many ships in line";
		public static readonly string RECEIVE_TIMEOUT = "Timeout";
		public static readonly string INVALID_FLEET_SETUP = "Invalid fleet setup";
		public static readonly string ELEMENT_NOT_ACTIVE = "Invalid ID - element not active";
		public static readonly string INDEX_IN_LINE_OUT_OF_RANGE = "Index out of range in ShipIndex";
		public static readonly string ONE_SHIP_MANY_MOVES = "Ship cannot make many moves in the same time";
		public static readonly string MOVE_DISTANCE_TOO_LONG = "Ship cannot move more than 1 line a turn";
		public static readonly string VALUE_NOT_IN_RANGE = "Value not in allowed range: ";
		public static readonly string NO_WEAPONS_OR_DEFENCES = "Ship has to have at least 1 weapon and 1 defence system";
	}
}
