using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class GameHistory {
		private int id;
		private Player winner;
		private Player loser;
		private Fleet winnerFleet;
		private Fleet loserFleet;
		private bool wasDraw;
		private DateTime gameDate;

		public GameHistory() {}

		public GameHistory(int id, Player winner, Player loser, Fleet winnerFleet, Fleet loserFleet, bool wasDraw, DateTime gameDate) {
			Id = id;
			Winner = winner;
			Loser = loser;
			WinnerFleet = winnerFleet;
			LoserFleet = loserFleet;
			WasDraw = wasDraw;
			GameDate = gameDate;
		}

		public int Id { get => id; set => id = value; }
		public Player Winner { get => winner; set => winner = value; }
		public Player Loser { get => loser; set => loser = value; }
		public Fleet WinnerFleet { get => winnerFleet; set => winnerFleet = value; }
		public Fleet LoserFleet { get => loserFleet; set => loserFleet = value; }
		public bool WasDraw { get => wasDraw; set => wasDraw = value; }
		public DateTime GameDate { get => gameDate; set => gameDate = value; }
	}

}
