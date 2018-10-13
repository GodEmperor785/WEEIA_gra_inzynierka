using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Player {
		private string username;
		private string password;
		private int experience;
		private int maxFleetPoints;

		public Player() { }

		public Player(string username, string password) {
			this.Username = username;
			this.Password = password;
		}

		public Player(string username, string password, int experience, int maxFleetPoints) {
			this.Username = username;
			this.Password = password;
			this.Experience = experience;
			this.MaxFleetPoints = maxFleetPoints;
		}

		public string Username { get => username; set => username = value; }
		public string Password { get => password; set => password = value; }
		public int Experience { get => experience; set => experience = value; }
		public int MaxFleetPoints { get => maxFleetPoints; set => maxFleetPoints = value; }
	}
}
