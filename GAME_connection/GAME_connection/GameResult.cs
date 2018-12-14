using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class GameResult {
		private bool youWon;
		private string message;

		public GameResult(bool youWon, string message) {
			this.YouWon = youWon;
			this.Message = message;
		}

		public bool YouWon { get => youWon; set => youWon = value; }
		public string Message { get => message; set => message = value; }
	}
}
