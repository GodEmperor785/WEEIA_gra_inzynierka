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
		PLAYER_DATA,

		MAKE_MOVE
	}

	/// <summary>
	/// Maps  <see cref="OperationType"/> to <see cref="Type"/> (class) of <see cref="GamePacket.Packet"/> in <see cref="GamePacket"/> object
	/// </summary>
	public static class OperationsMap {
		private static readonly Dictionary<OperationType, Type> operationMapping;

		static OperationsMap() {
			operationMapping = new Dictionary<OperationType, Type>();
			operationMapping.Add(OperationType.CONNECTION_TEST, typeof(object));			//nothing - never used
			operationMapping.Add(OperationType.VIEW_FLEETS, typeof(List<Fleet>));			//list of players fleets
			operationMapping.Add(OperationType.UPDATE_FLEET, typeof(Fleet));                //updated fleet object		maybe only list of ids?
			operationMapping.Add(OperationType.ADD_FLEET, typeof(Fleet));					//new fleet object			maybe only list of ids?
			operationMapping.Add(OperationType.PLAY_RANKED, typeof(object));				//nothing - never used
			operationMapping.Add(OperationType.PLAY_CUSTOM, typeof(string));                //TODO <---------------------------------------------
			operationMapping.Add(OperationType.LOGIN, typeof(Player));                      //player object with password and username set
			operationMapping.Add(OperationType.REGISTER, typeof(Player));                   //player object with password and username set
			operationMapping.Add(OperationType.DISCONNECT, typeof(object));                 //nothing - never used
			operationMapping.Add(OperationType.PLAYER_DATA, typeof(Player));                 //player object with exp and maxFleetPoints set
			operationMapping.Add(OperationType.MAKE_MOVE, typeof(string));                  //TODO <---------------------------------------------
		}

		public static Dictionary<OperationType, Type> OperationMapping => operationMapping;
	}

}
