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
		SELECT_FLEET,
		PLAY_RANKED,
		PLAY_CUSTOM_JOIN,
		PLAY_CUSTOM_CREATE,
		GET_CUSTOM_ROOMS,
		MAKE_MOVE,
		GAME_STATE,
		SETUP_FLEET,
		ABANDON_GAME,
		SURRENDER,
		GAME_END,

		//login/register/logout
		LOGIN,
		REGISTER,
		DISCONNECT,
		PLAYER_DATA,
		GET_PLAYER_STATS,
		GET_PLAYER_STATS_ENTRY,
		BASE_MODIFIERS,

		//shop
		GET_LOOTBOXES,
		BOUGHT_SHIPS,
		BUY,
		SELL_SHIP,

		//action results
		SUCCESS,
		FAILURE,

		//admin
		ADMIN_PACKET,
		GET_SHIP_TEMPLATES,
		ADD_SHIP_TEMPLATE,
		UPDATE_SHIP_TEMPLATE,
		GET_WEAPONS,
		ADD_WEAPON,
		UPDATE_WEAPON,
		GET_DEFENCES,
		ADD_DEFENCE,
		UPDATE_DEFENCE
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
			operationMapping.Add(OperationType.VIEW_ALL_PLAYER_SHIPS, typeof(Fleet));       //list of player owned ships							- important for client
			operationMapping.Add(OperationType.UPDATE_FLEET, typeof(Fleet));                //updated fleet object		maybe only list of ids?		- important for server, server respons with S/F
			operationMapping.Add(OperationType.ADD_FLEET, typeof(Fleet));                   //new fleet object			maybe only list of ids?		- important for server, server respons with S/F
			operationMapping.Add(OperationType.DELETE_FLEET, typeof(Fleet));                //fleet to delete		maybe only fleet name or id?	- important for server, server respons with S/F
			operationMapping.Add(OperationType.SELECT_FLEET, typeof(Fleet));                //fleet used for a game, called before PLAY				- important for server, server respons with S/F
			operationMapping.Add(OperationType.PLAY_RANKED, typeof(object));                //internal packet does not matter, S/F response			- server respons with S/F
			operationMapping.Add(OperationType.ABANDON_GAME, typeof(object));               //internal packet does not matter, S/F response			- server respons with S/F
			operationMapping.Add(OperationType.PLAY_CUSTOM_JOIN, typeof(CustomGameRoom));   //room with room name, creator name and password		- important for server, server respons with S/F
			operationMapping.Add(OperationType.PLAY_CUSTOM_CREATE, typeof(CustomGameRoom)); //room with all variables set							- important for server, server respons with S/F
			operationMapping.Add(OperationType.GET_CUSTOM_ROOMS, typeof(List<CustomGameRoom>)); //list of available vustom rooms					- important for client
			operationMapping.Add(OperationType.GAME_END, typeof(GameResult));				//result of game										- important for client
			operationMapping.Add(OperationType.LOGIN, typeof(Player));                      //player object with password and username set			- important for server, server respons with S/F
			operationMapping.Add(OperationType.REGISTER, typeof(Player));                   //player object with password and username set			- important for server, server respons with S/F
			operationMapping.Add(OperationType.DISCONNECT, typeof(object));                 //nothing - never used									- importnant for server, no server response
			operationMapping.Add(OperationType.PLAYER_DATA, typeof(Player));                //player object with exp and maxFleetPoints set			- important for client
			operationMapping.Add(OperationType.BASE_MODIFIERS, typeof(BaseModifiers));      //base modifiers for client								- important for client
			operationMapping.Add(OperationType.GET_PLAYER_STATS, typeof(List<GameHistory>)); //list of game history entries for given player		- important for client
			operationMapping.Add(OperationType.GET_PLAYER_STATS_ENTRY, typeof(GameHistory)); //full game history entry with given id				- important for client
			operationMapping.Add(OperationType.MAKE_MOVE, typeof(Move));                    //Move the client wants to make							- important for server, server respons with S/F
			operationMapping.Add(OperationType.GAME_STATE, typeof(GameState));              //GameState object										- important for client
			operationMapping.Add(OperationType.SETUP_FLEET, typeof(PlayerGameBoard));       //PlayerGameBoard object with initial setup in 3 lines	- important for server, server respons with S/F
			operationMapping.Add(OperationType.SUCCESS, typeof(object));                    //nothing - never used									- null
			operationMapping.Add(OperationType.FAILURE, typeof(string));                    //reason for failure									- important for client
			operationMapping.Add(OperationType.GET_LOOTBOXES, typeof(List<LootBox>));       //list of available lootboxes							- important for client
			operationMapping.Add(OperationType.BUY, typeof(LootBox));                       //lootbox to buy										- important for server, server respons with S/F
			operationMapping.Add(OperationType.BOUGHT_SHIPS, typeof(List<Ship>));           //ships from lootbox									- important for client, SUCCESS before this
			operationMapping.Add(OperationType.SELL_SHIP, typeof(Ship));                    //ship to sell for money								- important for server, server respons with S/F

			operationMapping.Add(OperationType.GET_SHIP_TEMPLATES, typeof(List<Ship>));     //list of all ship templates							- important for client
			operationMapping.Add(OperationType.ADD_SHIP_TEMPLATE, typeof(Ship));            //ship template to add									- important for server, server respons with S/F
			operationMapping.Add(OperationType.UPDATE_SHIP_TEMPLATE, typeof(Ship));         //ship template to update								- important for server, server respons with S/F
			operationMapping.Add(OperationType.GET_WEAPONS, typeof(List<Weapon>));			//list of all weapons									- important for client
			operationMapping.Add(OperationType.ADD_WEAPON, typeof(Weapon));					//weapon to add											- important for server, server respons with S/F
			operationMapping.Add(OperationType.UPDATE_WEAPON, typeof(Weapon));              //weapon to update										- important for server, server respons with S/F
			operationMapping.Add(OperationType.GET_DEFENCES, typeof(List<DefenceSystem>));  //list of all defences									- important for client
			operationMapping.Add(OperationType.ADD_DEFENCE, typeof(DefenceSystem));         //defence to add										- important for server, server respons with S/F
			operationMapping.Add(OperationType.UPDATE_DEFENCE, typeof(DefenceSystem));      //defence to update										- important for server, server respons with S/F

			// S/F = SUCCESS/FAILURE
			//if important for server: client must set correct internal packet, server checks validity of packet and responds with S/F
			//if important for client: client sends this OperationType and server does not care for internal packet, server responds with the right internal packet and same OperationType as client

			//when getting game history you need to first get all game histories with: GET_PLAYER_STATS and then to view fleet details use GET_PLAYER_STATS_ENTRY with right game history entry

			//initial order of packets:
			//1. client sends LOGIN or REGISTER
			//2. server responds with S/F
			//3. if SUCCESS continue, if FAILURE repeat
			//4. server sends PLAYER_DATA to client
			//5. server sends BASE_MODIFIERS to client
			//6. normal menu operations, client sends request and server responds accordingly, requests to get something usually have no S/F conformation, modification requests have S/F response

			//order of packet in game room
			//1. client sends PLAY_RANKED or PLAY_CUSTOM_JOIN or PLAY_CUSTOM_CREATE
			//2. server sends SUCCESS or FAILURE
			//3. client waits for second player
			//4. a> if ABANDON no server response
			//5. b> else server sends SUCCESS after second player joins
			//6. client sends SETUP_FLEET
			//7. server sends SUCCESS or FAILURE on validating fleet setup
			//8. a> if fail game ends and back to menu for client (server sends FAILURE to client)
			//9. b> else proper game starts (server sends SUCCESS to client)
			//     game loop (while client does not receive GAME_END)
			//1. server sends GAME_STATE to client
			//2. client sends MAKE_MOVE
			//3. server sends SUCCESS or FAILURE on validating move
			//4. a> if validation failed move is skipped (server sends FAILURE to client)
			//5. b> if validation success move is processed (server sends SUCCESS to client)
			//6. loop continues until GAME_END
		}

		public static Dictionary<OperationType, Type> OperationMapping => operationMapping;
	}

}
