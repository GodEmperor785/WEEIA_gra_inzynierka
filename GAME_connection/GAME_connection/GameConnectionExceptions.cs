using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	public class ConnectionEndedException : Exception {
		private string orign;
		private int playerNumber;

		public ConnectionEndedException() {}

		public ConnectionEndedException(string message) : base(message) {}

		public ConnectionEndedException(string message, int playerNumber) : base(message) {
			PlayerNumber = playerNumber;
		}

		public ConnectionEndedException(string message, string origin) : this(message) {
			this.Orign = origin;
		}

		/// <summary>
		/// should specify whether exception occured on send or receive
		/// </summary>
		public string Orign { get => orign; set => orign = value; }
		public int PlayerNumber { get => playerNumber; set => playerNumber = value; }
	}

	public class SendTimeoutException : Exception {
		private int playerNumber;

		public SendTimeoutException() { }

		public SendTimeoutException(string message) : base(message) {}

		public SendTimeoutException(int playerNumber) {
			this.PlayerNumber = playerNumber;
		}

		public int PlayerNumber { get => playerNumber; set => playerNumber = value; }
	}

	public class ReceiveTimeoutException : Exception {
		private int playerNumber;

		public ReceiveTimeoutException() { }

		public ReceiveTimeoutException(string message) : base(message) { }

		public ReceiveTimeoutException(int playerNumber) {
			this.PlayerNumber = playerNumber;
		}

		public int PlayerNumber { get => playerNumber; set => playerNumber = value; }
	}

	public class NotAdministratorException : Exception {
		public NotAdministratorException() { }

		public NotAdministratorException(string message) : base(message) { }
	}

}
