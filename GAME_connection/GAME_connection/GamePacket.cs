using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class GamePacket {
		private string operationType;
		private object packet;

		public GamePacket() {}

		public GamePacket(string operation, object data) {
			this.OperationType = operation;
			this.Packet = data;
		}

		public string OperationType { get => operationType; set => operationType = value; }
		public object Packet { get => packet; set => packet = value; }
	}

}
