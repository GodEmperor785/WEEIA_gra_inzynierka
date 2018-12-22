using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class PlayerGameBoard {
		private Dictionary<Line, List<Ship>> board;

		public PlayerGameBoard() {
			Board = new Dictionary<Line, List<Ship>>() {
				{ Line.SHORT, new List<Ship>() },
				{ Line.MEDIUM, new List<Ship>() },
				{ Line.LONG, new List<Ship>() },
			};
		}

		public PlayerGameBoard(List<Ship> shortRange, List<Ship> mediumRange, List<Ship> longRange) {
			Board = new Dictionary<Line, List<Ship>>() {
				{ Line.SHORT, shortRange },
				{ Line.MEDIUM, mediumRange },
				{ Line.LONG, longRange },
			};
		}

		public PlayerGameBoard(Dictionary<Line, List<Ship>> board) {
			Board = board;
		}

		public Dictionary<Line, List<Ship>> Board { get => board; set => board = value; }

		public bool PlayerHasNoShips() {
			int numberOfShips = 0;
			foreach(var line in Board) {
				numberOfShips += line.Value.Count;
			}
			if (numberOfShips == 0) return true;
			else return false;
		}
	}
}
