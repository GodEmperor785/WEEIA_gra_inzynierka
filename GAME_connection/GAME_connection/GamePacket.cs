using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class GamePacket {
		private OperationType operationType;
		private object packet;

		public GamePacket() {}

		public GamePacket(OperationType operation, object data) {
			this.OperationType = operation;
			this.Packet = data;
		}

		public static GamePacket CreateConnectionTestPacket() {
			return new GamePacket(OperationType.CONNECTION_TEST, null);
		}

		public OperationType OperationType { get => operationType; set => operationType = value; }
		public object Packet { get => packet; set => packet = value; }
	}

}
