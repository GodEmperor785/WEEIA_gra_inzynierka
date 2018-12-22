using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	/// <summary>
	/// you can use Line x = (Line)someINTvalue;
	/// </summary>
	[Serializable]
	public enum Line {
		/// <summary>
		/// equal to 0
		/// </summary>
		SHORT,
		/// <summary>
		/// equal to 1
		/// </summary>
		MEDIUM,
		/// <summary>
		/// equal to 2
		/// </summary>
		LONG
	}

	[Serializable]
	public class ShipPosition {
		private Line lineIndex;
		private int shipIndex;	//index on list

		public ShipPosition(Line lineIndex, int shipIndex) {
			this.Line = lineIndex;
			this.ShipIndex = shipIndex;
		}

		public Line Line { get => lineIndex; set => lineIndex = value; }
		public int ShipIndex { get => shipIndex; set => shipIndex = value; }
	}

	[Serializable]
	public class Move {
		List< Tuple<ShipPosition, ShipPosition> > attackList;	//your ship position first, enemy second
		List< Tuple<ShipPosition, Line> > moveList;             //your ship position and target line
																//if ship is not present in any list it is defending by default

		public Move() {
			AttackList = new List<Tuple<ShipPosition, ShipPosition>>();
			MoveList = new List<Tuple<ShipPosition, Line>>();
		}

		public Move(List<Tuple<ShipPosition, ShipPosition>> attackList, List<Tuple<ShipPosition, Line>> moveList) {
			this.AttackList = attackList;
			this.MoveList = moveList;
		}

		public List<Tuple<ShipPosition, ShipPosition>> AttackList { get => attackList; set => attackList = value; }
		public List<Tuple<ShipPosition, Line>> MoveList { get => moveList; set => moveList = value; }
		
	}
}
