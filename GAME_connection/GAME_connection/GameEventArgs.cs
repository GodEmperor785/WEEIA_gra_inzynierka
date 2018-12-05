using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	public class GameEventArgs : EventArgs {
		public GameEventArgs() { }

		public GameEventArgs(int playerNumber) {
			this.PlayerNumber = playerNumber;
		}

		public int PlayerNumber { get; set; }
	}
}
