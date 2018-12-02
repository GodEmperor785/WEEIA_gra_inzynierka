using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Player {
		private int id;
		private string username;
		private string password;
		private int experience;
		private int maxFleetPoints;
		private int gamesPlayed;
		private int gamesWon;
		private int money;

		public Player() { }

		public Player(string username, string password) {
			this.Username = username;
			this.Password = password;
		}

		public Player(int id, string username, string password, int experience, int maxFleetPoints, int gamesPlayed, int gamesWon, int money) {
			this.Id = id;
			this.Username = username;
			this.Password = password;
			this.Experience = experience;
			this.MaxFleetPoints = maxFleetPoints;
			this.GamesPlayed = gamesPlayed;
			this.GamesWon = gamesWon;
			this.Money = money;
		}

		public string Username { get => username; set => username = value; }
		public string Password { get => password; set => password = value; }
		public int Experience { get => experience; set => experience = value; }
		public int MaxFleetPoints { get => maxFleetPoints; set => maxFleetPoints = value; }
		public int Id { get => id; set => id = value; }
		public int GamesPlayed { get => gamesPlayed; set => gamesPlayed = value; }
		public int GamesWon { get => gamesWon; set => gamesWon = value; }
		public int Money { get => money; set => money = value; }
		public int WinLoseRatio {
			get {
				return GamesWon / GamesPlayed;
			}
		}
	}
}
