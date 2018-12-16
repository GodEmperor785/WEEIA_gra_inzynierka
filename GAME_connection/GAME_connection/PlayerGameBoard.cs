using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class PlayerGameBoard {
		private List<Ship> shortRange;
		private List<Ship> mediumRange;
		private List<Ship> longRange;

		public PlayerGameBoard() {
			ShortRange = new List<Ship>();
			MediumRange = new List<Ship>();
			LongRange = new List<Ship>();
		}

		public PlayerGameBoard(List<Ship> shortRange, List<Ship> mediumRange, List<Ship> longRange) {
			ShortRange = shortRange;
			MediumRange = mediumRange;
			LongRange = longRange;
		}

		public List<Ship> ShortRange { get => shortRange; set => shortRange = value; }
		public List<Ship> MediumRange { get => mediumRange; set => mediumRange = value; }
		public List<Ship> LongRange { get => longRange; set => longRange = value; }

		public bool PlayerHasNoShips() {
			bool shortRangeEmpty = !ShortRange.Any();
			bool mediumRangeEmpty = !MediumRange.Any();
			bool longRangeEmpty = !LongRange.Any();
			return (shortRangeEmpty && mediumRangeEmpty && longRangeEmpty);
		}
	}
}
