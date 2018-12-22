using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class PlayerGameBoard {
		private Dictionary<Line, List<Ship>> board;
		/*private List<Ship> shortRange;
		private List<Ship> mediumRange;
		private List<Ship> longRange;*/

		public PlayerGameBoard() {
			/*ShortRange = new List<Ship>();
			MediumRange = new List<Ship>();
			LongRange = new List<Ship>();*/
			Board = new Dictionary<Line, List<Ship>>() {
				{ Line.SHORT, new List<Ship>() },
				{ Line.MEDIUM, new List<Ship>() },
				{ Line.LONG, new List<Ship>() },
			};
		}

		public PlayerGameBoard(List<Ship> shortRange, List<Ship> mediumRange, List<Ship> longRange) {
			/*ShortRange = shortRange;
			MediumRange = mediumRange;
			LongRange = longRange;*/
			Board = new Dictionary<Line, List<Ship>>() {
				{ Line.SHORT, shortRange },
				{ Line.MEDIUM, mediumRange },
				{ Line.LONG, longRange },
			};
		}

		/*public List<Ship> ShortRange { get => shortRange; set => shortRange = value; }
		public List<Ship> MediumRange { get => mediumRange; set => mediumRange = value; }
		public List<Ship> LongRange { get => longRange; set => longRange = value; }*/
		public Dictionary<Line, List<Ship>> Board { get => board; set => board = value; }

		public bool PlayerHasNoShips() {
			int numberOfShips = 0;
			foreach(var line in Board) {
				numberOfShips += line.Value.Count;
			}
			if (numberOfShips == 0) return true;
			else return false;
			/*bool shortRangeEmpty = !ShortRange.Any();
			bool mediumRangeEmpty = !MediumRange.Any();
			bool longRangeEmpty = !LongRange.Any();
			return (shortRangeEmpty && mediumRangeEmpty && longRangeEmpty);*/
		}
	}
}
