using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class GameState {
		private PlayerGameBoard yourGameBoard;
		private PlayerGameBoard enemyGameBoard;

		public GameState() {
			YourGameBoard = new PlayerGameBoard();
			EnemyGameBoard = new PlayerGameBoard();
		}

		public GameState(PlayerGameBoard yourGameBoard, PlayerGameBoard enemyGameBoard) {
			this.YourGameBoard = yourGameBoard;
			this.EnemyGameBoard = enemyGameBoard;
		}

		public PlayerGameBoard YourGameBoard { get => yourGameBoard; set => yourGameBoard = value; }
		public PlayerGameBoard EnemyGameBoard { get => enemyGameBoard; set => enemyGameBoard = value; }
	}
}
