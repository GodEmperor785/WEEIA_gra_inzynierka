using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	public enum OperationType {
		//internal
		CONNECTION_TEST,

		//fleets
		VIEW_FLEETS,
		UPDATE_FLEET,
		ADD_FLEET,
		DELETE_FLEET,
		VIEW_ALL_PLAYER_SHIPS,

		//game
		PLAY_RANKED,
		PLAY_CUSTOM,
		MAKE_MOVE,

		//login/register/logout
		LOGIN,
		REGISTER,
		DISCONNECT,
		PLAYER_DATA,
		GET_PLAYER_STATS,

		//shop
		GET_LOOTBOXES,
		BOUGHT_SHIPS,
		BUY,
		SELL_SHIP,

		//action results
		SUCCESS,
		FAILURE
	}

	/// <summary>
	/// Maps  <see cref="OperationType"/> to <see cref="Type"/> (class) of <see cref="GamePacket.Packet"/> in <see cref="GamePacket"/> object
	/// </summary>
	public static class OperationsMap {
		private static readonly Dictionary<OperationType, Type> operationMapping;

		static OperationsMap() {
			operationMapping = new Dictionary<OperationType, Type>();
			operationMapping.Add(OperationType.CONNECTION_TEST, typeof(object));			//nothing - never used									- null
			operationMapping.Add(OperationType.VIEW_FLEETS, typeof(List<Fleet>));           //list of players fleets								- important for client
			operationMapping.Add(OperationType.DELETE_FLEET, typeof(Fleet));                //list of player owned ships							- important for clientF
			operationMapping.Add(OperationType.UPDATE_FLEET, typeof(Fleet));                //updated fleet object		maybe only list of ids?		- important for server, server respons with S/F
			operationMapping.Add(OperationType.ADD_FLEET, typeof(Fleet));                   //new fleet object			maybe only list of ids?		- important for server, server respons with S/F
			operationMapping.Add(OperationType.DELETE_FLEET, typeof(Fleet));                //fleet to delete		maybe only fleet name or id?	- important for server, server respons with S/F
			operationMapping.Add(OperationType.PLAY_RANKED, typeof(object));				//nothing - never used									- null
			operationMapping.Add(OperationType.PLAY_CUSTOM, typeof(string));                //TODO <---------------------------------------------	- 
			operationMapping.Add(OperationType.LOGIN, typeof(Player));                      //player object with password and username set			- important for server, server respons with S/F
			operationMapping.Add(OperationType.REGISTER, typeof(Player));                   //player object with password and username set			- important for server, server respons with S/F
			operationMapping.Add(OperationType.DISCONNECT, typeof(object));                 //nothing - never used									- importnant for server, no server response
			operationMapping.Add(OperationType.PLAYER_DATA, typeof(Player));                 //player object with exp and maxFleetPoints set		- important for client
			operationMapping.Add(OperationType.GET_PLAYER_STATS, typeof(List<GameHistory>)); //list of game history entries for given player		- important for client
			operationMapping.Add(OperationType.MAKE_MOVE, typeof(string));                  //TODO <---------------------------------------------	- 
			operationMapping.Add(OperationType.SUCCESS, typeof(object));                    //nothing - never used									- null
			operationMapping.Add(OperationType.FAILURE, typeof(string));                    //reason for failure									- important for client
			operationMapping.Add(OperationType.GET_LOOTBOXES, typeof(List<LootBox>));       //list of available lootboxes							- important for client
			operationMapping.Add(OperationType.BUY, typeof(LootBox));                       //lootbox to buy										- important for server, server respons with S/F
			operationMapping.Add(OperationType.BOUGHT_SHIPS, typeof(List<Ship>));           //ships from lootbox									- important for client
			operationMapping.Add(OperationType.SELL_SHIP, typeof(Ship));                    //ship to sell for money								- important for server, server respons with S/F

			// S/F = SUCCESS/FAILURE
			//if important for server: client must set correct internal packet, server checks validity of packet and responds with S/F
			//if important for client: client send this OperationType and server does not care for internal packet, server responds with the right internal packet
		}

		public static Dictionary<OperationType, Type> OperationMapping => operationMapping;
	}

}
