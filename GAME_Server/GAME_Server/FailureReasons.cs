using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_Server {
	public static class FailureReasons {
		public static readonly string INVALID_INTERNAL_PACKET = "Invalid type of internal packet";
		public static readonly string INVALID_PACKET_TYPE = "Invalid packet OperationType received";
		public static readonly string INCORRECT_LOGIN = "Incorrect login data for username: ";
		public static readonly string USERNAME_ALREADY_EXISTS = "Username already exists: ";
		public static readonly string CLIENT_DISCONNECTED = "Connection ended without proper disconnect";
		public static readonly string NOT_ENOUGH_MONEY = "Not enough money";
		public static readonly string INVALID_ID = "Invalid object data received";
	}
}
