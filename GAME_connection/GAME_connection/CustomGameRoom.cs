using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class CustomGameRoom {
		private string roomName;
		private bool openForAll;	//uses password or not
		private string password;
		private string creatorsUsername;

		public CustomGameRoom() { }

		/// <summary>
		/// use to join openFoprAll rooms
		/// </summary>
		/// <param name="roomName"></param>
		public CustomGameRoom(string roomName) {
			this.RoomName = roomName;
		}

		/// <summary>
		/// use to join room with a password
		/// </summary>
		/// <param name="roomName"></param>
		/// <param name="password"></param>
		public CustomGameRoom(string roomName, string password) : this(roomName) {
			this.Password = password;
		}

		/// <summary>
		/// use to create new game room
		/// </summary>
		/// <param name="roomName"></param>
		/// <param name="password"></param>
		/// <param name="openForAll"></param>
		/// <param name="creatorsUserName"></param>
		public CustomGameRoom(string roomName, string password, bool openForAll, string creatorsUserName) : this(roomName, password) {
			this.OpenForAll = openForAll;
			this.CreatorsUsername = creatorsUserName;
		}

		/// <summary>
		/// used by server to answer clients GET_CUSTOM_ROOMS request
		/// </summary>
		/// <param name="room"></param>
		/// <param name="unsetPassword"></param>
		public CustomGameRoom(CustomGameRoom room, bool unsetPassword) {
			this.RoomName = room.RoomName;
			this.OpenForAll = room.OpenForAll;
			this.CreatorsUsername = room.CreatorsUsername;
			if (unsetPassword) this.Password = "";
			else this.Password = room.Password;
		}

		public string RoomName { get => roomName; set => roomName = value; }
		public bool OpenForAll { get => openForAll; set => openForAll = value; }
		public string Password { get => password; set => password = value; }
		public string CreatorsUsername { get => creatorsUsername; set => creatorsUsername = value; }
	}
}
