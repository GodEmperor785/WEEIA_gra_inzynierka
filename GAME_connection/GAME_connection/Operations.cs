using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	public enum OperationType {
		CONNECTION_TEST,

		VIEW_FLEETS,
		UPDATE_FLEET,
		ADD_FLEET,

		PLAY_RANKED,
		PLAY_CUSTOM,

		LOGIN,
		REGISTER,
		DISCONNECT,

		MAKE_MOVE
	}

	public static class OperationsMap {
		private static readonly Dictionary<OperationType, Type> operationMapping;

		static OperationsMap() {
			operationMapping = new Dictionary<OperationType, Type>();
			operationMapping.Add(OperationType.CONNECTION_TEST, null);
		}

		public static Dictionary<OperationType, Type> OperationMapping => operationMapping;
	}

}
