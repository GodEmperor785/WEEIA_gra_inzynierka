using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_Server {
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
		public static readonly string DUPLICATES_NOT_ALLOWED = "no duplicates in lists are allowed";
		public static readonly string ZERO_SHIPS_IN_FLEET = "fleet must contain at least one ship";
		public static readonly string FACTION_MUST_BE_SAME = "All elements must belong to the same faction";
		public static readonly string NO_SUCH_ROOM = "Room already full or deleted";
		public static readonly string WRONG_ROOM_PASSWORD = "Wrong room password";
		public static readonly string ROOM_FULL = "Room is already full";
		public static readonly string CANT_ABANDON = "Cannot abandon room that is already full";
	}
}
