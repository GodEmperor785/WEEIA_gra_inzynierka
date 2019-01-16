using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;
using GAME_connection;
using System.Globalization;
using System.Runtime.CompilerServices;
using GAME_Validator;
using System.Diagnostics;
using System.Security.Authentication;

namespace GAME_Server {
	internal class Server {
		private enum DbInitializeType {
			USE_EXISTING,
			DROP_CREATE_DEBUG,
			DROP_CREATE_BASIC
		}

		public static readonly int EXIT_STATUS_MANUAL_SHUTDOWN = 1;
		public static readonly int EXIT_STATUS_SERVER_ERROR = -1;

		private static int port = TcpConnection.DEFAULT_PORT;

		//database specific fields and properties
		private static IGameDataBase gameDataBase;		//used only for initialization of BaseModifiers etc. Other IGameDataBase are created in user threads
		internal static BaseModifiers baseModifiers;

		//locks
		private static readonly object logLock = new object();
		private static readonly object customRoomsLock = new object();
		private static readonly object rankedRoomsLock = new object();
		private static readonly object loggedInUsersLock = new object();

		internal static IGameDataBase GameDataBase { get => gameDataBase; }
		public static BaseModifiers BaseModifiers { get => baseModifiers; }

		//thread management specific fields and properties
		private static List<Thread> userThreads = new List<Thread>();
		private static List<UserThread> userThreadObjects = new List<UserThread>();
		internal static Dictionary<CustomGameRoom, GameRoomThread> availableCustomGameRooms = new Dictionary<CustomGameRoom, GameRoomThread>();
		internal static List<GameRoomThread> availableRankedGameRooms = new List<GameRoomThread>();

		private static List<string> loggedInUsers = new List<string>(); 

		internal static bool continueAcceptingConnections = true;

		private static string logFilePath;
		private static DbInitializeType dbInitializeOption;

		static void Main(string[] args) {
			try {
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");   //to change exception language to english

				Log("Getting configuration...");
				var useSslVar = ConfigurationManager.AppSettings["useSsl"];
				bool useSsl = Convert.ToBoolean(useSslVar);
				logFilePath = ConfigurationManager.AppSettings["pathToLogFile"];
				dbInitializeOption = (DbInitializeType)Enum.Parse(typeof(DbInitializeType), ConfigurationManager.AppSettings["dbInitializeOption"]);
				Log("Server use SSL: " + useSsl);
				Log("Path to fog file: " + logFilePath);
				Log("DB initialization option: " + dbInitializeOption);

				//some tests
				//Tests.TestRandomness();
				//Tests.DoGameBoardValidationTest();

				InitilizeGameDataFromDB(dbInitializeOption);       //need to change in GameDBContext - the DB initializer

				//TcpListener listener = new TcpListener(ipAddress, port);
				TcpListener listener = new TcpListener(IPAddress.Any, port);
				listener.Start();
				Log("Server listening on port : " + port);
				Thread cliThread = new Thread(new ThreadStart(ServerCLI));
				cliThread.Start();

				while (continueAcceptingConnections) {
					try {
						Log("Server is waiting for client...");
						TcpClient client = listener.AcceptTcpClient();
						TcpConnection gameClient;
						//gameClient = new TcpConnection(client, false, Server.Log, true, true, "hamachi.cer");		//uncomment this to enable ssl (with hamachi certificate)
						if (useSsl) gameClient = new TcpConnection(client, false, Server.Log, true, true, "gameServerCert.cer");        //use this to enable ssl (with public certificate)
						else gameClient = new TcpConnection(client, false, Server.Log);
						Log("Client connected - ip: " + gameClient.RemoteIpAddress + " port: " + gameClient.RemotePortNumber);
						gameClient.PrintSecurityInfo();

						UserThread userThread = new UserThread(gameClient);
						Thread t = new Thread(new ThreadStart(userThread.RunUserThread));
						userThreads.Add(t);
						userThreadObjects.Add(userThread);
						t.Start();
					}
					catch (AuthenticationException aEx) {
						Log("User SSL/TLS authentication failed! " + aEx.Message, true);
					}
					catch (IOException ioEx) {
						Log("Remote disconnected before authentication end! " + ioEx.Message, true);
					}
				}
			} catch(Exception critical) {
				Log("UNHANDLED EXCEPTION HAPPENED - STOPPING SERVER", true);
				Log(critical.Message);
				Log(critical.Source);
				Log(critical.StackTrace);
				Console.ReadKey();
				Environment.Exit(EXIT_STATUS_SERVER_ERROR);
			}
			
		}

		private static void ServerCLI() {
			while (true) {
				string cmd = Console.ReadLine();
				switch (cmd) {
					case "exit":
						Log("exiting...");
						GameDataBase.Dispose();
						continueAcceptingConnections = false;
						foreach (UserThread t in userThreadObjects) {
							t.ClientConnected = false;
							t.EndThread();
						}
						Environment.Exit(EXIT_STATUS_MANUAL_SHUTDOWN);
						break;
					case "info":
						Log("Process information:");
						using (Process serverProcess = Process.GetCurrentProcess()) {
							Log("Used memory: " + serverProcess.PrivateMemorySize64 + " bytes");
							Log("Started at: " + serverProcess.StartTime);
							Log("Thread count: " + serverProcess.Threads.Count);
						}
						break;
					default:
						Log("ERROR: unknown command - " + cmd);
						break;
				}
			}
		}

		//========================= LOGGED IN USERS UTILS ==================================================================================================================

		/// <summary>
		/// used to login a user
		/// </summary>
		/// <param name="username"></param>
		internal static void AddLoggedInUser(string username) {
			Log("Logging IN user: " + username);
			lock (loggedInUsersLock) {
				loggedInUsers.Add(username);
			}
		}

		/// <summary>
		/// used to logout a user
		/// </summary>
		/// <param name="username"></param>
		internal static void RemoveLoggedInUser(string username) {
			string msg = "";
			lock (loggedInUsersLock) {
				if (loggedInUsers.Remove(username)) msg = "success";
				else msg = "already logged out";
			}
			Log("Logging OUT user: " + username + " " + msg);
		}

		internal static bool UserAlreadyLoggedIn(string username) {
			lock (loggedInUsersLock) {
				if (loggedInUsers.Any(name => name == username)) return true;
				else return false;
			}
		}

		//========================= GAME ROOM LIST/DICT UTILS ==================================================================================================================
		internal static List<CustomGameRoom> GetAvailableCustomRooms() {
			List<CustomGameRoom> availableRooms = new List<CustomGameRoom>();
			lock(customRoomsLock) {
				foreach(var roomPair in availableCustomGameRooms) {
					availableRooms.Add(new CustomGameRoom(roomPair.Key, true));
				}
			}
			return availableRooms;
		}

		private static void RemoveCustomRoom(CustomGameRoom roomToRemove) {
			CustomGameRoom removeIt = new CustomGameRoom();
			foreach (var roomPair in availableCustomGameRooms) {
				if (roomToRemove.RoomName == roomPair.Key.RoomName) {
					removeIt = roomPair.Key;
					break;
				}
			}
			availableCustomGameRooms.Remove(removeIt);
		}

		/// <summary>
		/// null check necessary, result string indicates failure type if GameRoomThread is null
		/// </summary>
		/// <param name="roomToJoin"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		internal static GameRoomThread JoinCustomRoom(CustomGameRoom roomToJoin, out string result) {
			string success = "SUCCESS";
			lock (customRoomsLock) {
				foreach (var roomPair in availableCustomGameRooms) {
					var customRoom = roomPair.Key;
					if (roomToJoin.RoomName == customRoom.RoomName && roomToJoin.CreatorsUsername == customRoom.CreatorsUsername) {	//room name found and creator matches
						if (customRoom.OpenForAll) {	//don't check password if there is none - openForAll
							result = success;
							RemoveCustomRoom(customRoom);   //remove custom room from available rooms
							return roomPair.Value;
						}
						else if (roomToJoin.Password == customRoom.Password) {	//else check password
							result = success;
							RemoveCustomRoom(customRoom);	//remove custom room from available rooms
							return roomPair.Value;
						}
						else {
							result = FailureReasons.WRONG_ROOM_PASSWORD;
							return null;		//return null on error
						}
					}
				}
			}
			result = FailureReasons.NO_SUCH_ROOM;
			return null;        //return null on error
		}

		/// <summary>
		/// used by GameRoomThread to remove a custom room
		/// </summary>
		/// <param name="roomToRemove"></param>
		internal static void AbandonCustomGameRoom(CustomGameRoom roomToRemove) {
			lock (customRoomsLock) {
				RemoveCustomRoom(roomToRemove);
			}
		}

		/// <summary>
		/// used to create a new available custom room
		/// </summary>
		/// <param name="roomToAdd"></param>
		internal static void CreateCustomRoom(CustomGameRoom roomToAdd, GameRoomThread roomObj) {
			lock (customRoomsLock) {
				availableCustomGameRooms.Add(roomToAdd, roomObj);
			}
		}

		//----------------- RANKED -------------------------------------------------------------------------------------
		private static void RemoveRankedRoom(GameRoomThread game) {
			availableRankedGameRooms.Remove(game);
		}

		/// <summary>
		/// used to create a new ranked game if there are no free ranked rooms available
		/// </summary>
		/// <param name="newRoom"></param>
		internal static void CreateRankedGame(GameRoomThread newRoom) {
			lock (rankedRoomsLock) {
				availableRankedGameRooms.Add(newRoom);
			}
		}

		/// <summary>
		/// joins ranked game with player with matchmaking score as close as possible to searching player, returns null if there are no rooms
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		internal static GameRoomThread JoinBestGameRoomForPlayer(Player player) {
			double searchingPlayerScore = CalculateMatchmakingScore(player);
			GameRoomThread bestRoom = null;
			double minScoreDelta = double.MaxValue;
			lock(rankedRoomsLock) {
				if (availableRankedGameRooms.Count == 0) return null;		//if no rooms present - return null, user thread will handle that
				foreach (GameRoomThread game in availableRankedGameRooms) {
					double currentDelta = Math.Abs(game.MatchMakingScore - searchingPlayerScore);
					if(currentDelta < minScoreDelta) {
						bestRoom = game;
						minScoreDelta = currentDelta;
					}
				}
				RemoveRankedRoom(bestRoom);
			}
			Server.Log(player.Username + " with score: " + searchingPlayerScore + " joins room with score delta of " + minScoreDelta);
			return bestRoom;
		}

		internal static void AbandonRankedGame(GameRoomThread game) {
			lock (rankedRoomsLock) {
				RemoveRankedRoom(game);
			}
		}

		/// <summary>
		/// Calculates players matchmaking score to find best opponent using Exp and win-lose ratio
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		internal static double CalculateMatchmakingScore(Player player) {
			return (player.Experience * Math.Pow(player.WinLoseRatio, 3));
		}

		//========================= OTHER UTILS ==================================================================================================================

		/// <summary>
		/// Reads basic game data (BaseModifiers) from DB. Use other options for test Databases
		/// </summary>
		private static void InitilizeGameDataFromDB(DbInitializeType dbOption) {
			if (dbOption == DbInitializeType.DROP_CREATE_BASIC) {
				AbstractDataBase.DROP_CREATE_ALWAYS = true;
				gameDataBase = GetGameDBContext();
				Tests.CreateBasicDb();
			}
			else if (dbOption == DbInitializeType.DROP_CREATE_DEBUG) {
				AbstractDataBase.DROP_CREATE_ALWAYS = true;
				gameDataBase = GetGameDBContext();
				Tests.TestGameDB(false);
			}
			else {
				AbstractDataBase.DROP_CREATE_ALWAYS = false;
				gameDataBase = GetGameDBContext();
			}
			Server.baseModifiers = GameDataBase.GetBaseModifiers();
		}

		/// <summary>
		/// performs deep cloning of serializable object. If object is not serializable throws <see cref="ArgumentException"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objectToClone"></param>
		/// <returns></returns>
		internal static T CloneObject<T>(T objectToClone) {
			if (!typeof(T).IsSerializable) throw new ArgumentException("Type of object must be serializable");
			else if (objectToClone == null) return default(T);
			using (var tempStream = new MemoryStream()) {
				var serializer = new BinaryFormatter();
				serializer.Serialize(tempStream, objectToClone);
				tempStream.Position = 0;
				return (T)serializer.Deserialize(tempStream);
			}
		}

		/// <summary>
		/// casts Packet from inside GamePacket to proper type. Should be called like: ProperType p = CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType])
		/// throws <see cref="InvalidCastException"/> if type is not correct so it should be called inside try-catch block
		/// </summary>
		/// <param name="packetToCast"></param>
		/// <param name="properType"></param>
		/// /// <exception cref="InvalidCastException"></exception>
		/// <returns></returns>
		internal static dynamic CastPacketToProperType(dynamic packetToCast, Type properType) {
			return Convert.ChangeType(packetToCast, properType);
		}

		private static bool LogToFile(string msg) {
			try {
				if (logFilePath != null) {
					File.AppendAllText(logFilePath, msg);
					return true;
				}
				else return false;
			}
			catch (Exception e) {
				return false;
			}
		}

		/// <summary>
		/// To be used instead of <see cref="Console.WriteLine"/>, prints message to chosen log - console, text area etc. Appends date at the beginning of message
		/// </summary>
		/// <param name="message"></param>
		internal static void Log(string message) {
			string msg = DateTime.Now + ": " + message;
			lock (logLock) {
				Console.WriteLine(msg);
				if(!LogToFile(msg + Environment.NewLine)) Console.WriteLine("Write to file " + logFilePath + " failed!");
			}
		}

		/// <summary>
		/// To be used instead of <see cref="Console.Write"/>, prints message without new line to chosen log - console, text area etc.
		/// </summary>
		/// <param name="message"></param>
		internal static void LogNoNewLine(string message) {
			lock (logLock) {
				Console.Write(message);
				LogToFile(message);
			}
		}

		/// <summary>
		/// To be used instead of <see cref="Console.WriteLine"/>, prints message to chosen log - console, text area etc. This overload also logs caller line
		/// </summary>
		internal static void Log(string message, bool isCritical, [CallerLineNumber] int callerLine = 0) {
			string msg = "";
			if (isCritical) msg += " >>> CRITICAL: ";
			msg += message + " at line: " + callerLine;
			Log(msg);
		}

		/// <summary>
		/// Creates new object implementing <see cref="IGameDataBase"/> 
		/// </summary>
		/// <returns></returns>
		internal static IGameDataBase GetGameDBContext() {
			return new MySqlDataBase();
		}

		internal static void SetBaseModifiers(BaseModifiers newMods) {
			Server.baseModifiers = newMods;
		}

	}

	#region User Thread
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	//---------USER THREAD--------------------------------------------------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	internal class UserThread {
		#region properties attributes and constructors
		private TcpConnection client;
		private Player user;
		private IGameDataBase gameDataBase;
		private GameRNG gameRNG;
		private bool clientConnected;
		private bool threadAlreadyEnded;
		private object loopLock = new object();
		private object userLock = new object();
		private object threadEndedLock = new object();
		private Fleet selectedFleetForGame = null;

		internal UserThread(TcpConnection client) {
			client.ConnectionEnded += UserDisconnectedHandler;
			//client.ConnectionTestReceived += ConnectionTestReceived;
			this.Client = client;
			this.GameDataBase = Server.GetGameDBContext();
			this.gameRNG = new GameRNG();
		}

		private void ConnectionTestReceived(object sender, EventArgs e) {
			Server.Log("connection test received, queue count = " + Client.QueueCount);
		}

		public TcpConnection Client { get => client; set => client = value; }
		public Player User {
			get => user;
			set => user = value;
		}
		public IGameDataBase GameDataBase { get => gameDataBase; set => gameDataBase = value; }     //use only this as DB in this thread
		public GameRNG GameRNG { get => gameRNG; set => gameRNG = value; }
		internal bool ClientConnected {
			get {
				bool clientConnectedLocal;
				lock (loopLock) {
					clientConnectedLocal = clientConnected;
				}
				return clientConnectedLocal;
			}
			set {
				lock(loopLock) {
					clientConnected = value;
				}
			}
		}
		public bool ThreadAlreadyEnded {
			get {
				bool localThreadAlreadyEnded;
				lock (threadEndedLock) {
					localThreadAlreadyEnded = threadAlreadyEnded;
				}
				return localThreadAlreadyEnded;
			}
			set {
				lock (threadEndedLock) {
					threadAlreadyEnded = value;
				}
			}
		}
		public bool IsInGame { get; set; }

		public Fleet SelectedFleetForGame { get => selectedFleetForGame; set => selectedFleetForGame = value; }
		#endregion

		/// <summary>
		/// main function of <see cref="UserThread"/>
		/// </summary>
		internal void RunUserThread() {
			Server.Log("User connected at: " + Client.RemoteIpAddress + ":" + Client.RemotePortNumber);
			try {
				//first do login or register
				bool loginSuccess = false;
				bool adminLogin = false;
				while (!loginSuccess) {		//while login is not succesful
					LoginResult loginResult = LoginOrRegister();
					if (loginResult == LoginResult.LOGIN_SUCCESS) loginSuccess = true;
					else if(loginResult == LoginResult.ADMIN_LOGIN_SUCCESS) {
						loginSuccess = true;
						adminLogin = true;
					}
				}
				if (loginSuccess) {
					if (adminLogin) {
						this.User = GameDataBase.GetPlayerWithUsername(this.User.Username).ToPlayer();
						this.User.Password = "";
						Server.AddLoggedInUser(User.Username);

						AdminDataPacket adminPacket = new AdminDataPacket(
							GameDataBase.GetAllShipTemplates().Select(dbTempl => dbTempl.ToShip()).ToList(),
							GameDataBase.GetAllWeapons().Select(wep => wep.ToWeapon()).ToList(),
							GameDataBase.GetAllDefences().Select(def => def.ToDefenceSystem()).ToList(),
							Server.BaseModifiers,
							GameDataBase.GetAllFactions(),
							GameDataBase.GetAllLootBoxes().Select(l => l.ToLootBox()).ToList()
						);
						Client.Send(new GamePacket(OperationType.ADMIN_PACKET, adminPacket));

						AdminProcessing();	//main logic
					}
					else {
						//set and send user data
						this.User = GameDataBase.GetPlayerWithUsername(this.User.Username).ToPlayer();
						this.User.Password = "";
						Server.AddLoggedInUser(User.Username);

						Client.Send(new GamePacket(OperationType.PLAYER_DATA, this.User));
						Client.Send(new GamePacket(OperationType.BASE_MODIFIERS, Server.BaseModifiers));

						UserProcessing();   //main logic
					}
				}
				else EndThread();
			}
			catch (ConnectionEndedException) {
				Server.Log(FailureReasons.CLIENT_DISCONNECTED, true);
				EndThread();
			}
			catch (Exception criticalEx) {
				Server.Log("UNHANDLED EXCEPTIONt: " + criticalEx.GetType().ToString() + " " + criticalEx.Message + " in: " + criticalEx.Source, true);
				Server.Log(": stack trace:" + Environment.NewLine + criticalEx.StackTrace, true);
			}
		}

		private enum LoginResult {
			LOGIN_SUCCESS,
			ADMIN_LOGIN_SUCCESS,
			LOGIN_FAILED,
			REGISTRATION_SUCCESS,
			REGISTRATION_FAILED,
			ERROR
		}

		#region login/register
		/// <summary>
		/// Checks if login or register is allowed for given player data and returns true if it is so. Receive 1 msg (typeof(packet) = Player), send 1 msg (opType = FAILURE/SUCCESS)
		/// </summary>
		/// <returns></returns>
		private LoginResult LoginOrRegister() {
			GamePacket packet = Client.GetReceivedPacket();
			Player playerObject;
			//cast to proper type
			if (packet.OperationType == OperationType.LOGIN || packet.OperationType == OperationType.REGISTER) {
				try {
					playerObject = Server.CastPacketToProperType(packet.Packet, OperationsMap.OperationMapping[packet.OperationType]);
				} catch (InvalidCastException) {
					SendFailure(FailureReasons.INVALID_PACKET + " got " + packet.Packet.GetType());
					return LoginResult.ERROR;
				}
				//if type ok do login or register
				if (packet.OperationType == OperationType.LOGIN) {
					if (GameDataBase.PlayerExists(playerObject) && GameDataBase.ValidateUser(playerObject) && (!Server.UserAlreadyLoggedIn(playerObject.Username))) {
						this.User = playerObject;
						if (GameDataBase.UserIsAdmin(playerObject)) {
							Server.Log("Succesfully logged in ADMIN: " + playerObject.Username);
							SendSuccess();
							return LoginResult.ADMIN_LOGIN_SUCCESS;
						}
						else {
							Server.Log("Succesfully logged in player: " + playerObject.Username);
							SendSuccess();
							return LoginResult.LOGIN_SUCCESS;
						}
					}
					else {
						SendFailure(FailureReasons.INCORRECT_LOGIN + playerObject.Username);
						return LoginResult.LOGIN_FAILED;
					}
				}
				else if (packet.OperationType == OperationType.REGISTER) {
					if ((!GameDataBase.PlayerExists(playerObject)) && GameDataBase.PlayerNameIsUnique(playerObject)) {
						RegisterNewPlayer(playerObject);
						Server.Log("Succesfully registered player: " + playerObject.Username);
						this.User = playerObject;
						SendSuccess();
						return LoginResult.REGISTRATION_SUCCESS;
					}
					else {
						SendFailure(FailureReasons.USERNAME_ALREADY_EXISTS + playerObject.Username);
						return LoginResult.REGISTRATION_FAILED;
					}
				}
				return LoginResult.ERROR;
			}
			else if(packet.OperationType == OperationType.DISCONNECT) {
				string msg = "User disconnected before succesful login!";
				Server.Log(msg);
				throw new ConnectionEndedException(msg);
			}
			else {
				SendFailure(FailureReasons.INVALID_PACKET_TYPE);
				return LoginResult.ERROR;
			}
		}

		private void RegisterNewPlayer(Player newPlayerData) {
			DbPlayer newPlayer = new DbPlayer(newPlayerData, Server.BaseModifiers.StartingMoney);	//new player with basic data and 0s
			newPlayer.OwnedShips = ShipTemplatesToShips( GameDataBase.GetShipTemplatesWithRarityAndReqExp(Rarity.COMMON, 0), newPlayer );   //basic ships
			newPlayer.OwnedShips.Add(GameDataBase.GetRandomShipTemplateOfRarity(Rarity.VERY_RARE, 0).GenerateNewShipOfThisTemplate(newPlayer));	//and add one VERY_RARE ship
			GameDataBase.AddPlayer(newPlayer);  //finally add player to db
		}
		#endregion

		#region main logic
		private void AdminProcessing() {
			ClientConnected = true;
			string validationResult;
			while (ClientConnected) {
				GamePacket gamePacket = Client.GetReceivedPacket();
				try {
					switch (gamePacket.OperationType) {
						case OperationType.ADD_LOOTBOX:
							LootBox newLootbox;
							newLootbox = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to add new lootbox");
							validationResult = GameValidator.ValidateLootbox(newLootbox);
							if (validationResult == GameValidator.OK) {
								GameDataBase.AddLootBox(new DbLootBox(
									newLootbox.Cost,
									newLootbox.Name,
									newLootbox.ChancesForRarities[Rarity.COMMON],
									newLootbox.ChancesForRarities[Rarity.RARE],
									newLootbox.ChancesForRarities[Rarity.VERY_RARE],
									newLootbox.ChancesForRarities[Rarity.LEGENDARY],
									newLootbox.NumberOfShips
								));
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.UPDATE_LOOTBOX:
							LootBox lootboxToUpdate;
							lootboxToUpdate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to update lootbox with id " + lootboxToUpdate.Id);
							validationResult = GameValidator.ValidateLootbox(lootboxToUpdate);
							if (validationResult == GameValidator.OK) {
								if (lootboxToUpdate.Id != 0) {
									GameDataBase.UpdateLootbox(lootboxToUpdate);
									SendSuccess();
								}
								else SendFailure(FailureReasons.INVALID_ID);
							}
							else SendFailure(validationResult);
							break;
						case OperationType.GET_LOOTBOXES:
							Server.Log("ADMIN: " + User.Username + ": wants to view lootboxes");
							List<DbLootBox> dbLootBoxes = GameDataBase.GetAllLootBoxes();
							List<LootBox> lootBoxes = dbLootBoxes.Select(x => x.ToLootBox()).ToList();
							Client.Send(new GamePacket(OperationType.GET_LOOTBOXES, lootBoxes));
							break;
						case OperationType.ADD_USER:
							AdminAppPlayer newUser;
							newUser = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to add new user - IsAdmin: " + newUser.IsAdmin);
							validationResult = GameValidator.ValidateUser(newUser, true);
							if (validationResult == GameValidator.OK) {
								if (GameDataBase.PlayerNameIsUnique(newUser)) { //if name is unique - add new user
									DbPlayer newDbUser = new DbPlayer(newUser, Server.BaseModifiers.StartingMoney, newUser.IsAdmin) {
										Experience = newUser.Experience,
										Money = newUser.Money
									};
									GameDataBase.AddPlayer(newDbUser);
									SendSuccess();
								}
								else SendFailure(FailureReasons.USERNAME_ALREADY_EXISTS + newUser.Username);
							}
							else SendFailure(validationResult);
							break;
						case OperationType.UPDATE_USER:
							AdminAppPlayer userToUpdate;
							userToUpdate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to update user with id " + userToUpdate.Id);
							validationResult = GameValidator.ValidateUser(userToUpdate, false);
							if (validationResult == GameValidator.OK) {
								DbPlayer newData = new DbPlayer() { Id = userToUpdate.Id, Username = userToUpdate.Username, Experience = userToUpdate.Experience, GamesPlayed = userToUpdate.GamesPlayed,
									GamesWon = userToUpdate.GamesWon, Money = userToUpdate.Money, IsActive = userToUpdate.IsActive, IsAdmin = userToUpdate.IsAdmin
								};
								GameDataBase.UpdatePlayer(newData);
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.DEACTIVATE_USER:
							AdminAppPlayer userToDeactivate;
							userToDeactivate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to deactivate user with id " + userToDeactivate.Id);
							if (GameDataBase.RemovePlayerWithUsername(userToDeactivate.Username)) SendSuccess();
							else SendFailure(FailureReasons.INVALID_ID);
							break;
						case OperationType.GET_USERS:
							Server.Log("ADMIN: " + User.Username + ": wants to view users");
							List<AdminAppPlayer> allUsers = GameDataBase.GetAllPlayers().Select(x => x.ToAdminAppPlayer()).ToList();
							Client.Send(new GamePacket(OperationType.GET_USERS, allUsers));
							break;
						case OperationType.UPDATE_BASE_MODIFIERS:
							BaseModifiers updatedMods;
							updatedMods = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to modify base modifiers");
							validationResult = GameValidator.ValidateBaseModifiers(updatedMods);
							if (validationResult == GameValidator.OK) {
								GameDataBase.UpdateBaseModifiers(updatedMods);
								Server.SetBaseModifiers(updatedMods);
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.BASE_MODIFIERS:
							Server.Log("ADMIN: " + User.Username + ": wants to view base modifiers");
							BaseModifiers mods = GameDataBase.GetBaseModifiers();
							Client.Send(new GamePacket(OperationType.BASE_MODIFIERS, mods));
							break;
						case OperationType.UPDATE_DEFENCE:
							DefenceSystem defenceSystemToUpdate;
							defenceSystemToUpdate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to modify defence system with id " + defenceSystemToUpdate.Id);
							validationResult = GameValidator.ValidateDefenceSystem(defenceSystemToUpdate);
							if (validationResult == GameValidator.OK) {
								GameDataBase.UpdateDefenceSystem(defenceSystemToUpdate);
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.ADD_DEFENCE:
							DefenceSystem defenceSystem;
							defenceSystem = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to add new defence system");
							validationResult = GameValidator.ValidateDefenceSystem(defenceSystem);
							if (validationResult == GameValidator.OK) {
								defenceSystem.Id = 0;
								DbDefenceSystem dbWeapon = new DbDefenceSystem(defenceSystem) {
									Faction = GameDataBase.GetFactionWithId(defenceSystem.Faction.Id)
								};
								GameDataBase.AddDefenceSystem(dbWeapon);
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.GET_DEFENCES:
							Server.Log("ADMIN: " + User.Username + ": wants to view defence systems");
							List<DefenceSystem> defences = GameDataBase.GetAllDefences().Select(x => x.ToDefenceSystem()).ToList();
							Client.Send(new GamePacket(OperationType.GET_DEFENCES, defences));
							break;
						case OperationType.UPDATE_WEAPON:
							Weapon weaponToUpdate;
							weaponToUpdate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to modify weapon with id " + weaponToUpdate.Id);
							validationResult = GameValidator.ValidateWeapon(weaponToUpdate);
							if (validationResult == GameValidator.OK) {
								GameDataBase.UpdateWeapon(weaponToUpdate);
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.ADD_WEAPON:
							Weapon weapon;
							weapon = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to add new weapon");
							validationResult = GameValidator.ValidateWeapon(weapon);
							if (validationResult == GameValidator.OK) {
								weapon.Id = 0;
								DbWeapon dbWeapon = new DbWeapon(weapon) {
									Faction = GameDataBase.GetFactionWithId(weapon.Faction.Id)
								};
								GameDataBase.AddWeapon(dbWeapon);
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.GET_WEAPONS:
							Server.Log("ADMIN: " + User.Username + ": wants to view weapons");
							List<Weapon> weapons = GameDataBase.GetAllWeapons().Select(x => x.ToWeapon()).ToList();
							Client.Send(new GamePacket(OperationType.GET_WEAPONS, weapons));
							break;
						case OperationType.UPDATE_SHIP_TEMPLATE:
							Ship shipToUpdate;
							shipToUpdate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to modify ship template with id " + shipToUpdate.Id);
							validationResult = GameValidator.ValidateShip(shipToUpdate);
							if (validationResult == GameValidator.OK) {
								DbShipTemplate newData = GameDataBase.ConvertShipToShipTemplate(shipToUpdate);
								newData.Id = shipToUpdate.Id;
								GameDataBase.UpdateShipTemplate(newData);
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.ADD_SHIP_TEMPLATE:
							Ship targetShip;
							targetShip = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log("ADMIN: " + User.Username + ": wants to add ship template");
							validationResult = GameValidator.ValidateShip(targetShip);
							if(validationResult == GameValidator.OK) {
								GameDataBase.AddShipTemplate(GameDataBase.ConvertShipToShipTemplate(targetShip));
								SendSuccess();
							}
							else SendFailure(validationResult);
							break;
						case OperationType.GET_SHIP_TEMPLATES:
							Server.Log("ADMIN: " + User.Username + ": wants to view ship templates");
							List<Ship> ships = GameDataBase.GetAllShipTemplates().Select(x => x.ToShip()).ToList();
							Client.Send(new GamePacket(OperationType.GET_SHIP_TEMPLATES, ships));
							break;
						case OperationType.DISCONNECT:
							Server.Log("ADMIN: " + User.Username + ": wants to disconnect");
							ClientConnected = false;
							break;
						default:
							SendFailure(FailureReasons.INVALID_PACKET);
							Server.Log("ADMIN: " + User.Username + ": unsupported operation: " + gamePacket.OperationType);
							break;
					}
				}
				catch (InvalidCastException) {
					SendFailure(FailureReasons.INVALID_PACKET);
				}
				catch (NullReferenceException ex) {
					Server.Log("ADMIN: " + "NULL caught - " + User.Username + "Exception message: " + ex.Message + " From:" + ex.Source, true);
					Server.Log(ex.StackTrace);
					SendFailure(FailureReasons.INVALID_ID);
				}
				catch (Exception criticalEx) {
					Server.Log("ADMIN: " + User.Username + ": UNHANDLED EXCEPTIONt: " + criticalEx.GetType().ToString() + " " + criticalEx.Message + " in: " + criticalEx.Source, true);
					Server.Log("ADMIN: " + User.Username + ": stack trace:" + Environment.NewLine + criticalEx.StackTrace, true);
				}
			}
			//finally, after the loop end thread
			Server.Log("ADMIN: " + User.Username + ": Thread Ending");
			this.EndThread();
		}

		//=======================================================================================================================

		private void UserProcessing() {
			//bool clientConnected = true;
			ClientConnected = true;
			DbPlayer thisUser;
			string validationResult;
			mainLoop: while (ClientConnected) {
				Server.Log(User.Username + ": waiting for request ");
				GamePacket gamePacket = Client.GetReceivedPacket();
				Server.Log(User.Username + ": received packet");
				try {
					gameSwitch: switch (gamePacket.OperationType) {
						case OperationType.PLAYER_DATA:
							Server.Log(User.Username + ": wants to view his data");
							GetPlayerFromDb();
							this.User.Password = "";
							Client.Send(new GamePacket(OperationType.PLAYER_DATA, this.User));
							break;
						//====================================================== SHOP =====================================================================================================
						case OperationType.GET_LOOTBOXES:
							Server.Log(User.Username + ": wants to view lootboxes");
							List<DbLootBox> dbLootBoxes = GameDataBase.GetAllLootBoxes();
							List<LootBox> lootBoxes = dbLootBoxes.Select(x => x.ToLootBox()).ToList();
							Client.Send(new GamePacket(OperationType.GET_LOOTBOXES, lootBoxes));
							break;
						case OperationType.BUY:
							LootBox targetLootBox;
							targetLootBox = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + ": wants to buy lootbox " + targetLootBox.Name);
							DbLootBox lootbox = GameDataBase.GetLootBoxWithId(targetLootBox.Id);
							thisUser = GetPlayerFromDb();
							if (lootbox.Cost > User.Money) SendFailure(FailureReasons.NOT_ENOUGH_MONEY);
							else if (Server.BaseModifiers.MaxShipsPerPlayer < (GameDataBase.GetPlayerShipCount(User) + lootbox.NumberOfShips)) SendFailure(FailureReasons.TOO_MANY_SHIPS);
							else {		// all checks successful
								thisUser.Money -= lootbox.Cost;
								User.Money -= lootbox.Cost;

								List<DbShip> randomShips = new List<DbShip>();
								LootBox normalLootBox = lootbox.ToLootBox();
								for (int i = 0; i < lootbox.NumberOfShips; i++) {    //repeat as many times as there are ships in lootbox
									Rarity drawnRarity =  GameRNG.GetRandomRarityWithChances(normalLootBox.ChancesForRarities);
									randomShips.Add(GameDataBase.GetRandomShipTemplateOfRarity(drawnRarity, thisUser.Experience).GenerateNewShipOfThisTemplate(thisUser));
								}
								List<Ship> resultForPlayer = new List<Ship>();
								foreach (DbShip ship in randomShips) {
									GameDataBase.AddShip(ship); //add all new ships to DB
									thisUser.OwnedShips.Add(ship);  //add ship to users ships
									resultForPlayer.Add(ship.ToShip());
								}

								GameDataBase.UpdatePlayer(thisUser);
								//finally send result to player - first success than list of bought ships
								SendSuccess();
								Client.Send(new GamePacket(OperationType.BOUGHT_SHIPS, resultForPlayer));
							}
							break;
						case OperationType.SELL_SHIP:
							Ship shipToSell = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + ": wants to sell ship with id " + shipToSell.Id + " for " + shipToSell.Cost);
							DbShip dbShipToSell = GameDataBase.GetShipWithId(shipToSell.Id);
							int shipValue = dbShipToSell.ShipBaseStats.Cost;
							thisUser = GetPlayerFromDb();
							if(GameDataBase.RemoveShipWithId(dbShipToSell.Id, false, thisUser.Id)) {
								thisUser.Money += shipValue;
								User.Money += shipValue;
								GameDataBase.UpdatePlayer(thisUser);
								SendSuccess();
							} else {
								SendFailure(FailureReasons.INVALID_ID);
							}
							break;
						//====================================================== FLEETS MENU =====================================================================================================
						case OperationType.VIEW_FLEETS:
							Server.Log(User.Username + ": wants to view his fleets");
							List<DbFleet> userDbFleets = GameDataBase.GetAllFleetsOfPlayer(User);
							List<Fleet> userFleets = userDbFleets.Select(x => x.ToFleetOnlyActiveShips()).ToList();
							Client.Send(new GamePacket(OperationType.VIEW_FLEETS, userFleets));
							break;
						case OperationType.VIEW_ALL_PLAYER_SHIPS:
							Server.Log(User.Username + ": wants to view his ships");
							List<DbShip> userDbShips = GameDataBase.GetPlayersShips(User);
							//List<DbShip> userDbShips = GetPlayersShips(User, ((MySqlDataBase)Server.GameDataBase).DbContext);
							List<Ship> userShips = userDbShips.Select(x => x.ToShip()).ToList();
							Client.Send(new GamePacket(OperationType.VIEW_ALL_PLAYER_SHIPS, userShips));
							break;
						case OperationType.ADD_FLEET:
							Fleet fleetToAdd = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(fleetToAdd.Name);
							Server.Log(User.Username + ": wants to add a new fleet");
							validationResult = GameServerValidator.ValidateFleet(User, fleetToAdd, GameDataBase, true);
							if (validationResult == GameServerValidator.OK) {  //fleet is ok
								GameDataBase.AddFleet(fleetToAdd, User);
								SendSuccess();
							}
							else {
								SendFailure(validationResult);
							}
							break;
						case OperationType.UPDATE_FLEET:
							Fleet fleetToUpdate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + ": wants to modify fleet with id " + fleetToUpdate.Id);
							validationResult = GameServerValidator.ValidateFleet(User, fleetToUpdate, GameDataBase, false);
							if (validationResult == GameServerValidator.OK) {  //fleet is ok
								GameDataBase.RemoveFleetWithId(fleetToUpdate.Id, false, User.Id);	//remove old fleet - set it as not active
								//DbFleet dbFleetToUpdate = GameDataBase.ConvertFleetToDbFleet(fleetToUpdate, User, true);
								//dbFleetToUpdate.Ships.AddRange(GameDataBase.GetNotActiveShipsForFleetWithId(fleetToUpdate.Id));
								GameDataBase.AddFleet(fleetToUpdate, User);		//add modified fleet as new - preserve old as not active for game history
								//GameDataBase.UpdateFleet(dbFleetToUpdate);
								SendSuccess();
							}
							else {
								SendFailure(validationResult);
							}
							break;
						case OperationType.DELETE_FLEET:
							Fleet fleetToDelete = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + ": wants to delete fleet with id " + fleetToDelete.Id);
							if (GameDataBase.RemoveFleetWithId(fleetToDelete.Id, false, User.Id)) SendSuccess();
							else SendFailure(FailureReasons.INVALID_ID);
							break;
						//====================================================== PLAYER STATS =====================================================================================================
						case OperationType.GET_PLAYER_STATS:
							Server.Log(User.Username + " wants to view game history");
							using (IGameDataBase tempDB = Server.GetGameDBContext()) {
								List<DbGameHistory> dbGameHistory = tempDB.GetPlayersGameHistory(User.Id);
								List<GameHistory> gameHistory = dbGameHistory.Select(x => x.ToGameHistory(false)).ToList();
								Client.Send(new GamePacket(OperationType.GET_PLAYER_STATS, gameHistory));
							}
							break;
						case OperationType.GET_PLAYER_STATS_ENTRY:
							GameHistory entry = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + " wants to view details of game history entry with ID: " + entry.Id);
							using (IGameDataBase tempDB = Server.GetGameDBContext()) {
								DbGameHistory dbEntry = tempDB.GetGameHistoryEntry(entry.Id);
								entry = dbEntry.ToGameHistory(true);
								Client.Send(new GamePacket(OperationType.GET_PLAYER_STATS_ENTRY, entry));
							}
							break;
						//====================================================== DISCONNECT =====================================================================================================
						case OperationType.DISCONNECT:
							Server.Log(User.Username + ": wants to disconnect");
							ClientConnected = false;
							break;
						//====================================================== SELECT FLEET FOR GAME =====================================================================================================
						case OperationType.SELECT_FLEET:
							Server.Log(User.Username + ": wants to select fleet used for next game");
							Fleet selectedFleet = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							DbFleet fleetForGame = GameDataBase.GetFleetWithId(selectedFleet.Id);
							if (fleetForGame.Owner.Id != User.Id || fleetForGame.IsActive == false) SendFailure(FailureReasons.INVALID_ID);
							else {
								SelectedFleetForGame = fleetForGame.ToFleetOnlyActiveShips();
								SendSuccess();
							}
							break;									
						//====================================================== CUSTOM GAME =====================================================================================================
						case OperationType.GET_CUSTOM_ROOMS:
							Server.Log(User.Username + ": wants to get list od custom rooms");
							List<CustomGameRoom> rooms = Server.GetAvailableCustomRooms();
							Client.Send(new GamePacket(OperationType.GET_CUSTOM_ROOMS, rooms));
							break;
						case OperationType.PLAY_CUSTOM_CREATE:
							if (SelectedFleetForGame == null) SendFailure(FailureReasons.NO_FLEET_SELECTED);
							else {
								CustomGameRoom roomToCreate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
								Server.Log(User.Username + ": wants to create a new custom game with name: " + roomToCreate.RoomName);
								GameRoomThread customGameRoomToCreate = new GameRoomThread(Client, User, GameDataBase, true, this, roomToCreate);
								Server.CreateCustomRoom(roomToCreate, customGameRoomToCreate);
								SendSuccess();
								Thread newCustomGameThread = new Thread(new ThreadStart(customGameRoomToCreate.RunGameThread));
								newCustomGameThread.Start();
								//newCustomGameThread.Join();
								Server.Log(User.Username + ": create game room: " + roomToCreate.RoomName + "successful, user thread blocks");
								IsInGame = true;
								customGameRoomToCreate.gameEnded.WaitOne();     //block until end of the game
								IsInGame = false;
								Server.Log(User.Username + ": has ended his game, user thread continues");
								UnsetSelectedFleetAfterGame();
							}
							break;
						case OperationType.PLAY_CUSTOM_JOIN:
							if (SelectedFleetForGame == null) SendFailure(FailureReasons.NO_FLEET_SELECTED);
							else {
								CustomGameRoom roomToJoin = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
								Server.Log(User.Username + ": wants to join custom game with name: " + roomToJoin.RoomName);
								GameRoomThread customGameRoomToJoin = Server.JoinCustomRoom(roomToJoin, out string joinResult);
								if (customGameRoomToJoin == null) {     //join failed
									UnsetSelectedFleetAfterGame();
									SendFailure(joinResult);
								}
								else {      //join succeeded - thread removed from available and JoinThisRoom can be called
									bool joinSuccess = customGameRoomToJoin.JoinThisRoom(Client, User, GameDataBase, this); //JoinThisRoom sends success
									if (joinSuccess) {
										//SendSuccess();
										Server.Log(User.Username + ": join game room: " + roomToJoin.RoomName + "successful, user thread blocks");
										IsInGame = true;
										customGameRoomToJoin.gameEnded.WaitOne();   //block until end of the game
										IsInGame = false;
										Server.Log(User.Username + ": has ended his game, user thread continues");
										UnsetSelectedFleetAfterGame();
									}
									else {
										UnsetSelectedFleetAfterGame();
										SendFailure(FailureReasons.ROOM_FULL);
									}
								}
							}
							break;
						//====================================================== RANKED GAME =====================================================================================================
						case OperationType.PLAY_RANKED:
							if (SelectedFleetForGame == null) SendFailure(FailureReasons.NO_FLEET_SELECTED);
							else {
								Server.Log(User.Username + ": wants to play ranked game");
								GameRoomThread rankedGame = Server.JoinBestGameRoomForPlayer(User);
								if (rankedGame == null) {           //there are no rooms - need to create new one
									Server.Log(User.Username + ": no ranked rooms available - creating new one");
									GameRoomThread newRankedRoom = new GameRoomThread(Client, User, GameDataBase, false, this);
									Server.CreateRankedGame(newRankedRoom);
									Thread newRankedGameThread = new Thread(new ThreadStart(newRankedRoom.RunGameThread));
									SendSuccess();
									newRankedGameThread.Start();
									//newCustomGameThread.Join();
									Server.Log(User.Username + ": create ranked game room successful, user thread blocks");
									IsInGame = true;
									newRankedRoom.gameEnded.WaitOne();     //block until end of the game
									IsInGame = false;
									Server.Log(User.Username + ": has ended his game, user thread continues");
									UnsetSelectedFleetAfterGame();
								}
								else {      //join successful - GameRoomThread removed from available and JoinThisThread can be called
									Server.Log(User.Username + ": join ranked room possible - joining");
									bool joinSuccess = rankedGame.JoinThisRoom(Client, User, GameDataBase, this);   //JoinThisRoom sends success
									if (joinSuccess) {
										//SendSuccess();
										Server.Log(User.Username + ": join game room successful, user thread blocks");
										IsInGame = true;
										rankedGame.gameEnded.WaitOne();   //block until end of the game
										IsInGame = false;
										Server.Log(User.Username + ": has ended his game, user thread continues");
										UnsetSelectedFleetAfterGame();
									}
									else {
										Server.Log(User.Username + ": join failed - join result false");
										UnsetSelectedFleetAfterGame();
										SendFailure(FailureReasons.ROOM_FULL);
									}
								}
							}
							break;
						default:
							SendFailure(FailureReasons.INVALID_PACKET);
							Server.Log(User.Username + ": unsupported operation: " + gamePacket.OperationType);
							break;
					}
				}
				catch (InvalidCastException) {
					SendFailure(FailureReasons.INVALID_PACKET);
				}
				catch (NullReferenceException ex) {			//null indicates that some object does not exist in DB
					Server.Log("NULL caught - " + User.Username + "is sending wrong id values - Exception message: " + ex.Message + " From:" + ex.Source, true);
					Server.Log(ex.StackTrace);
					SendFailure(FailureReasons.INVALID_ID);
				}
				catch (Exception criticalEx) {
					Server.Log(User.Username + ": UNHANDLED EXCEPTIONt: " + criticalEx.GetType().ToString() + " " + criticalEx.Message + " in: " + criticalEx.Source, true);
					Server.Log(User.Username + ": stack trace:" + Environment.NewLine + criticalEx.StackTrace, true);
				}
			}
			//finally, after the loop end thread
			Server.Log(User.Username + ": Thread Ending");
			this.EndThread();
		}
		#endregion

		#region user thread utils
		private void UserDisconnectedHandler(object sender, GameEventArgs e) {
			if(User != null) Server.Log(User.Username + ": sudden disconnection (disconnect event received) - ending user thread");
			else Server.Log("sudden disconnection (disconnect event received) - ending user thread");
			ClientConnected = false;
			if(!IsInGame) EndThread();
		}

		/// <summary>
		/// sends <see cref="OperationType.FAILURE"/> <see cref="GamePacket"/> to client and logs reason and caller line
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="callerLine">DO NOT SET</param>
		private void SendFailure(string reason, [CallerLineNumber] int callerLine = 0) {
			string failMsg = "! FAIL: Operation failed at: " + callerLine + ". Reason: " + reason;
			Server.Log(failMsg);
			this.Client.Send(new GamePacket(OperationType.FAILURE, reason));
		}

		private void SendSuccess() {
			this.Client.Send(new GamePacket(OperationType.SUCCESS, new object()));
		}

		private List<DbShip> ShipTemplatesToShips(List<DbShipTemplate> shipTemplates, DbPlayer player) {
			return shipTemplates.Select(x => x.GenerateNewShipOfThisTemplate(player)).ToList();
		}

		private DbPlayer GetPlayerFromDb() {
			DbPlayer player = GameDataBase.GetPlayerWithUsername(this.User.Username);
			this.User = player.ToPlayer();
			return player;
		}

		private void UnsetSelectedFleetAfterGame() {
			SelectedFleetForGame = null;
		}

		/// <summary>
		/// does necessary resource cleanup: disconnects from DB and disconnects TcpConnection
		/// </summary>
		internal void EndThread() {
			if (!ThreadAlreadyEnded) {
				ThreadAlreadyEnded = true;
				if (User != null) Server.Log(User.Username + " Thread ending - ending connections");
				else Server.Log("Thread ending - ending connections");
				if (User != null) Server.RemoveLoggedInUser(User.Username);
				this.GameDataBase.Dispose();
				this.Client.Disconnect();
			}
		}
		#endregion

	}
	#endregion

	#region Game Room Thread
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	//---------GAME ROOM THREAD---------------------------------------------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	internal class GameRoomThread {
		#region properties attributes and constructors
		public static readonly string SUDDEN_DISCONNED_GAME_RESULT = "Second player suddenly disconnected, you won";
		public static readonly string SUDDEN_DISCONNECT_LOG_STRING = "disconnected before game end";
		public static readonly string SURRENDER_LOG_STRING = "surrendered before game end";
		public readonly string YOU_WON_MESSAGE;
		public readonly string YOU_LOST_MESSAGE;
		public readonly string DRAW_MESSAGE;
		public static readonly int SETUP_FLEET_TIMEOUT = 120000;    //in miliseconds
		public static readonly int MAKE_MOVE_TIMEOUT = 180000;	//in miliseconds

		private Player player1;     //host
		private Player player2;

		private TcpConnection player1Conn;	//host
		private TcpConnection player2Conn;

		private IGameDataBase player1DB;
		private IGameDataBase player2DB;

		private UserThread player1ThreadObj;
		private UserThread player2ThreadObj;

		private PlayerGameBoard player1GameBoard;
		private PlayerGameBoard player2GameBoard;

		private Game thisGame;

		private Fleet player1Fleet;
		private Fleet player2Fleet;

		private GameState gameBoard;

		private CustomGameRoom customRoomDescriptor;

		private string usernamesOfPlayers;

		private bool isCustom;
		private bool isFull;
		private bool isAbandoned;
		private bool continueGameLoop;
		private bool fleetSetupOk;
		private bool gameAlreadyEnded;
		private bool player1MoveOK, player2MoveOK = true;

		private object joinLock = new object();
		private object isFullLock = new object();
		private object isAbandonedLock = new object();
		private object matchmakinglock = new object();
		private object continueGameLoopLock = new object();
		private object gameEndLock = new object();
		private object gameInProgressLock = new object();   //used to lock game progress and sudden game end
		private object gameAlredyEndedLock = new object();
		
		private double matchmakingScore;

		private AutoResetEvent roomFull;	//indicates that waiting for second player is over
		internal ManualResetEvent gameEnded;

		internal GameRoomThread(TcpConnection hostConnection, Player host, IGameDataBase player1DB, bool isCustom, UserThread player1ThreadObj, CustomGameRoom customGameObj = null) {
			hostConnection.GameAbandoned += GameAbandonedHandler;   //add event that indicates that player abandoned game
			hostConnection.ConnectionEnded += PlayerDisconnectedHandler;    //add event that indicates player disconnection
			hostConnection.Surrender += SurrenderHandler;	//add event that indicates player surrender
			Player1 = host;
			Player1Conn = hostConnection;
			Player1Conn.PlayerNumber = 1;
			Player1DB = player1DB;
			IsCustom = isCustom;
			IsFull = false;
			MatchMakingScore = Server.CalculateMatchmakingScore(Player1);
			Player1ThreadObj = player1ThreadObj;
			Player1Fleet = player1ThreadObj.SelectedFleetForGame;
			roomFull = new AutoResetEvent(false);
			gameEnded = new ManualResetEvent(false);
			if(isCustom) {
				customRoomDescriptor = customGameObj;
			}

			YOU_WON_MESSAGE = "You won! You gained " + Server.BaseModifiers.MoneyForVictory + " money and " + Server.BaseModifiers.ExpForVictory + " experience points";
			YOU_LOST_MESSAGE = "You lost! You gained " + Server.BaseModifiers.MoneyForLoss + " money and " + Server.BaseModifiers.ExpForLoss + " experience points";
			DRAW_MESSAGE = "Draw! You gained " + (int)((Server.BaseModifiers.MoneyForVictory + Server.BaseModifiers.MoneyForLoss) / 2) + " money and " + Server.BaseModifiers.ExpForDraw + " experience points";
		}

		public Player Player1 { get => player1;set => player1 = value; }
		public Player Player2 { get => player2; set => player2 = value; }
		public TcpConnection Player1Conn { get => player1Conn; set => player1Conn = value; }
		public TcpConnection Player2Conn { get => player2Conn; set => player2Conn = value; }
		public bool IsCustom { get => isCustom; set => isCustom = value; }
		public double MatchMakingScore { get => matchmakingScore; set => matchmakingScore = value; }
		internal IGameDataBase Player1DB { get => player1DB; set => player1DB = value; }
		internal IGameDataBase Player2DB { get => player2DB; set => player2DB = value; }
		public bool IsFull {
			get {
				bool localIsFull;
				lock (isFullLock) {
					localIsFull = isFull;
				}
				return localIsFull;
			}
			set {
				lock (isFullLock) {
					isFull = value;
				}
			}
		}
		public bool IsAbandoned {
			get {
				bool localIsAbandoned;
				lock (isAbandonedLock) {
					localIsAbandoned = isAbandoned;
				}
				return localIsAbandoned;
			}
			set {
				lock (isAbandonedLock) {
					isAbandoned = value;
				}
			}
		}
		public bool ContinueGameLoop {
			get {
				bool localContinueGameLoop;
				lock (continueGameLoopLock) {
					localContinueGameLoop = continueGameLoop;
				}
				return localContinueGameLoop;
			}
			set {
				lock (continueGameLoopLock) {
					continueGameLoop = value;
				}
			}
		}
		public bool GameAlreadyEnded {
			get {
				bool localGameAlreadyEnded;
				lock (gameAlredyEndedLock) {
					localGameAlreadyEnded = gameAlreadyEnded;
				}
				return localGameAlreadyEnded;
			}
			set {
				lock (gameAlredyEndedLock) {
					gameAlreadyEnded = value;
				}
			}
		}
		public string UsernamesOfPlayers { get => usernamesOfPlayers; set => usernamesOfPlayers = value; }
		internal UserThread Player1ThreadObj { get => player1ThreadObj; set => player1ThreadObj = value; }
		internal UserThread Player2ThreadObj { get => player2ThreadObj; set => player2ThreadObj = value; }
		public Fleet Player1Fleet { get => player1Fleet; set => player1Fleet = value; }
		public Fleet Player2Fleet { get => player2Fleet; set => player2Fleet = value; }
		public GameState GameBoard { get => gameBoard; set => gameBoard = value; }
		public PlayerGameBoard Player1GameBoard { get => player1GameBoard; set => player1GameBoard = value; }
		public PlayerGameBoard Player2GameBoard { get => player2GameBoard; set => player2GameBoard = value; }
		public Game ThisGame { get => thisGame; set => thisGame = value; }
		#endregion

		#region main logic
		/// <summary>
		/// main function of <see cref="GameRoomThread"/>
		/// </summary>
		internal async void RunGameThread() {
			Server.Log("Game room started by: " + Player1.Username + " is custom: " + IsCustom + ", waiting for second player...");
			try {
				roomFull.WaitOne();
				if (IsAbandoned) {      //creator of room abandoned this room - end it, logging and setting variables is done in event handler
					Server.Log("Game room started by: " + Player1.Username + " abandoned");
					EndGameThread();
				}
				else {  //room is full and game can start
					UsernamesOfPlayers = Player1.Username + "__vs__" + Player2.Username;
					Server.Log(UsernamesOfPlayers + ": room is full, starting game");
					SendSuccess(Player1Conn);
					SendSuccess(Player2Conn);

					string validateResult;
					fleetSetupOk = true;
					GamePacket player1Packet, player2Packet;

					//first get, process and validate players fleet setups
					Task<GamePacket> player1Task = GetPlayersPacket(Player1Conn, SETUP_FLEET_TIMEOUT);
					Task<GamePacket> player2Task = GetPlayersPacket(Player2Conn, SETUP_FLEET_TIMEOUT);
					player1Packet = await player1Task;
					player2Packet = await player2Task;
					if (player1Packet != null) {
						try {
							if (CheckDisconnectContinueGame(player1Packet, 1)) {
								if (CheckIfCorrectPacketAndIfNotEnqueueBack(OperationType.SETUP_FLEET, player1Packet, Player1Conn)) {
									Player1GameBoard = Server.CastPacketToProperType(player1Packet.Packet, OperationsMap.OperationMapping[player1Packet.OperationType]);
									validateResult = GameServerValidator.ValidatePlayerBoard(Player1GameBoard, Player1Fleet);
									if (validateResult != GameServerValidator.OK) {
										EndGameOnError(1, validateResult);
									}
									else SendSuccess(Player1Conn);  //if setup OK send success
								}
							}
						}
						catch (InvalidCastException) {
							EndGameOnError(1, FailureReasons.INVALID_PACKET);
						}
					}
					else EndGameOnError(1, FailureReasons.RECEIVE_TIMEOUT);
					if (player2Packet != null) {
						try {
							if (CheckDisconnectContinueGame(player2Packet, 2)) {
								if (CheckIfCorrectPacketAndIfNotEnqueueBack(OperationType.SETUP_FLEET, player2Packet, Player2Conn)) {
									Player2GameBoard = Server.CastPacketToProperType(player2Packet.Packet, OperationsMap.OperationMapping[player2Packet.OperationType]);
									validateResult = GameServerValidator.ValidatePlayerBoard(Player2GameBoard, Player2Fleet);
									if (validateResult != GameServerValidator.OK) {
										EndGameOnError(2, validateResult);
									}
									else SendSuccess(Player2Conn);  //if setup OK send success
								}
							}
						}
						catch (InvalidCastException) {
							EndGameOnError(2, FailureReasons.INVALID_PACKET);
						}
					}
					else EndGameOnError(2, FailureReasons.RECEIVE_TIMEOUT);

					Move player1Move = new Move();
					Move player2Move = new Move();
					if (fleetSetupOk) {
						ContinueGameLoop = true;
						GameAlreadyEnded = false;
						ThisGame = new Game(Player1GameBoard, Player2GameBoard);
						ThisGame.EnableDebug = true;
						while (ContinueGameLoop) {
							player1MoveOK = true;
							player2MoveOK = true;
							//first send gamestate to players (yourBoard, enemyBoard)
							Player1Conn.Send(new GamePacket(OperationType.GAME_STATE, ThisGame.Player1GameState));
							Player2Conn.Send(new GamePacket(OperationType.GAME_STATE, ThisGame.Player2GameState));
							//than wait for moves and p[rocess them - if invalid move - skip it
							Task<GamePacket> player1MoveTask = GetPlayersPacket(Player1Conn, MAKE_MOVE_TIMEOUT);
							Task<GamePacket> player2MoveTask = GetPlayersPacket(Player2Conn, MAKE_MOVE_TIMEOUT);
							player1Packet = await player1MoveTask;
							player2Packet = await player2MoveTask;
							if (player1Packet != null) {
								try {
									if (CheckDisconnectContinueGame(player1Packet, 1)) {
										if (CheckIfCorrectPacketAndIfNotEnqueueBack(OperationType.MAKE_MOVE, player1Packet, Player1Conn)) {
											player1Move = Server.CastPacketToProperType(player1Packet.Packet, OperationsMap.OperationMapping[player1Packet.OperationType]);
											validateResult = GameServerValidator.ValidateMove(player1Move, ThisGame.Player1GameBoard, ThisGame.Player2GameBoard);
											if (validateResult != GameServerValidator.OK) {
												SkipMove(Player1Conn, "invalid move: " + validateResult);
											}
											else SendSuccess(Player1Conn);  //if move OK send success
										}
									}
								}
								catch (InvalidCastException) {
									SkipMove(Player1Conn, FailureReasons.INVALID_PACKET);
								}
							}
							else {
								SkipMove(Player1Conn, "make move timeout");
							}
							if (player2Packet != null) {
								try {
									if (CheckDisconnectContinueGame(player2Packet, 2)) {
										if (CheckIfCorrectPacketAndIfNotEnqueueBack(OperationType.MAKE_MOVE, player2Packet, Player2Conn)) {
											player2Move = Server.CastPacketToProperType(player2Packet.Packet, OperationsMap.OperationMapping[player2Packet.OperationType]);
											validateResult = GameServerValidator.ValidateMove(player2Move, ThisGame.Player2GameBoard, ThisGame.Player1GameBoard);
											if (validateResult != GameServerValidator.OK) {
												SkipMove(Player2Conn, "invalid move: " + validateResult);
											}
											else SendSuccess(Player2Conn);  //if move OK send success
										}
									}
								}
								catch (InvalidCastException) {
									SkipMove(Player2Conn, FailureReasons.INVALID_PACKET);
								}
							}
							else {
								SkipMove(Player2Conn, "make move timeout");
							}

							ThisGame.MakeTurn(player1Move, player1MoveOK, player2Move, player2MoveOK);

							//check game result and possibly end the game
							Victory gameResultAfterThisTurn = ThisGame.CheckGameEndResult();
							if (gameResultAfterThisTurn == Victory.PLAYER_1) {
								UpdateLoserAndWiner(Player2, Player1);
								Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(true, YOU_WON_MESSAGE)));
								Player2Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, YOU_LOST_MESSAGE)));
								SetGameEnded();
							}
							else if (gameResultAfterThisTurn == Victory.PLAYER_2) {
								UpdateLoserAndWiner(Player1, Player2);
								Player2Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(true, YOU_WON_MESSAGE)));
								Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, YOU_LOST_MESSAGE)));
								SetGameEnded();
							}
							else if (gameResultAfterThisTurn == Victory.DRAW) {
								UpdateDraw();
								Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, DRAW_MESSAGE)));
								Player2Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, DRAW_MESSAGE)));
								SetGameEnded();
							}
							//else if Victory.NOT_YET and game continues
						}
					}

					if (!GameAlreadyEnded) {
						Server.Log(UsernamesOfPlayers + ": game finished, ending game room...");
						EndGameThread();
					}
				}
			}
			catch (ConnectionEndedException ex) {
				Server.Log(UsernamesOfPlayers + ": player with number " + ex.PlayerNumber + " suddenly disconnected");
				EndGameBeforeProperEnd(ex.PlayerNumber, SUDDEN_DISCONNECT_LOG_STRING);
			}
			catch (NullReferenceException ex2) {
				Server.Log(UsernamesOfPlayers + ": null caught: " + ex2.Message + " in: " + ex2.Source, true);
				Server.Log(UsernamesOfPlayers + ": stack trace:" + Environment.NewLine + ex2.StackTrace, true);
			}
			catch (Exception criticalEx) {
				Server.Log(UsernamesOfPlayers + ": UNHANDLED EXCEPTIONt: " + criticalEx.GetType().ToString() + " " + criticalEx.Message + " in: " + criticalEx.Source, true);
				Server.Log(UsernamesOfPlayers + ": stack trace:" + Environment.NewLine + criticalEx.StackTrace, true);
			}
		}
		#endregion

		/// <summary>
		/// async method to get players packet in parallel with second call of this method
		/// </summary>
		/// <param name="playerConection"></param>
		/// <returns></returns>
		private async Task<GamePacket> GetPlayersPacket(TcpConnection playerConection, int timeout) {
			try {
				Task<GamePacket> receivedPacket = Task.Run(() => playerConection.GetReceivedPacket(timeout));
				return await receivedPacket;
			} catch(ConnectionEndedException) {
				return new GamePacket(OperationType.DISCONNECT, new object());
			}
		}

		private void SkipMove(TcpConnection playerConnThatTiemouted, string reason) {
			SendFailure(playerConnThatTiemouted, reason);
			if (playerConnThatTiemouted.PlayerNumber == 1) player1MoveOK = false;
			else if (playerConnThatTiemouted.PlayerNumber == 2) player2MoveOK = false;
			else Server.Log(UsernamesOfPlayers + ": Invalid player number in skip move handler! Actual: " + playerConnThatTiemouted.PlayerNumber + ", should be 1 or 2", true);
		}

		/// <summary>
		/// returns true if no disconnect, false if disconnect and game should be ended
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="numberOfDisconnectedPlayer"></param>
		/// <returns></returns>
		private bool CheckDisconnectContinueGame(GamePacket packet, int numberOfDisconnectedPlayer) {
			if (packet.OperationType == OperationType.DISCONNECT) {
				EndGameBeforeProperEnd(numberOfDisconnectedPlayer, "Player with number " + numberOfDisconnectedPlayer + " disconnected");
				fleetSetupOk = false;
				return false;
			}
			else return true;
		}

		private bool CheckIfCorrectPacketAndIfNotEnqueueBack(OperationType expectedType, GamePacket packet, TcpConnection playerConnection) {
			if (packet.OperationType == expectedType) return true;
			else {
				playerConnection.EnqueuePacket(packet);	//put back not matching packet into the queue
				return false;
			}
		}

		#region game room utils
		/// <summary>
		/// sends Success to joiner if join ok and set event that starts the game
		/// </summary>
		/// <param name="joinerConnection"></param>
		/// <param name="joiner"></param>
		/// <param name="player2DB"></param>
		/// <param name="player2ThreadObj"></param>
		/// <returns></returns>
		internal bool JoinThisRoom(TcpConnection joinerConnection, Player joiner, IGameDataBase player2DB, UserThread player2ThreadObj) {
			lock (joinLock) {
				if (!IsFull) {
					joinerConnection.GameAbandoned += GameAbandonedHandler;   //add event that indicates that player abandoned game
					joinerConnection.ConnectionEnded += PlayerDisconnectedHandler;    //add event that indicates player disconnection
					joinerConnection.Surrender += SurrenderHandler;   //add event that indicates player surrender
					Player2 = joiner;
					Player2Conn = joinerConnection;
					Player2Conn.PlayerNumber = 2;
					Player2DB = player2DB;
					Player2ThreadObj = player2ThreadObj;
					Player2Fleet = player2ThreadObj.SelectedFleetForGame;
					IsFull = true;
					SendSuccess(joinerConnection);
					roomFull.Set();
					return true;
				}
				else return false;
			}
		}

		private void UpdateFleetShipsExp(IGameDataBase playerDB, int amountOfExp, Fleet fleetToUpdate) {
			foreach(Ship ship in fleetToUpdate.Ships) {
				playerDB.UpdateShipExp(ship, amountOfExp);
			}
		}

		private void UpdateLoserAndWiner(Player loser, Player winner) {
			Server.Log(UsernamesOfPlayers + ": Game ended - result - winner: " + winner.Username + " loser: " + loser.Username);

			IGameDataBase winnerDB;
			IGameDataBase loserDB;
			Fleet winnerFleet;
			Fleet loserFleet;
			if(winner.Username == Player1.Username) {
				winnerDB = Player1DB;
				winnerFleet = Player1Fleet;
				loserDB = Player2DB;
				loserFleet = Player2Fleet;
			}
			else {
				winnerDB = Player2DB;
				winnerFleet = Player2Fleet;
				loserDB = Player1DB;
				loserFleet = Player1Fleet;
			}
			DbPlayer dbWinner = winnerDB.GetPlayerWithUsername(winner.Username);
			dbWinner.Experience += Server.BaseModifiers.ExpForVictory;
			dbWinner.GamesWon += 1;
			dbWinner.GamesPlayed += 1;
			dbWinner.Money += Server.BaseModifiers.MoneyForVictory;
			winnerDB.UpdatePlayer(dbWinner);
			UpdateFleetShipsExp(winnerDB, Server.BaseModifiers.ExpForVictory, winnerFleet);

			DbPlayer dbLoser = loserDB.GetPlayerWithUsername(loser.Username);
			dbLoser.Experience += Server.BaseModifiers.ExpForLoss;
			dbLoser.GamesPlayed += 1;
			dbLoser.Money += Server.BaseModifiers.MoneyForLoss;
			loserDB.UpdatePlayer(dbLoser);
			UpdateFleetShipsExp(loserDB, Server.BaseModifiers.ExpForLoss, loserFleet);

			using (IGameDataBase tempDB = Server.GetGameDBContext()) {
				DbGameHistory entryToAdd = new DbGameHistory(tempDB.GetPlayerWithUsername(dbWinner.Username), tempDB.GetPlayerWithUsername(dbLoser.Username), 
						tempDB.GetFleetWithId(winnerFleet.Id), tempDB.GetFleetWithId(loserFleet.Id),
						false, DateTime.Now);
				tempDB.AddGameHistory(entryToAdd);
			}
		}

		private void UpdateDraw() {
			Server.Log(UsernamesOfPlayers + ": Game ended - result: DRAW!");

			DbPlayer dbPlayer1 = Player1DB.GetPlayerWithUsername(Player1.Username);
			dbPlayer1.Experience += Server.BaseModifiers.ExpForDraw;
			dbPlayer1.GamesPlayed += 1;
			dbPlayer1.Money += (int)((Server.BaseModifiers.MoneyForVictory + Server.BaseModifiers.MoneyForLoss) / 2);
			Player1DB.UpdatePlayer(dbPlayer1);
			UpdateFleetShipsExp(Player1DB, Server.BaseModifiers.ExpForDraw, Player1Fleet);

			DbPlayer dbPlayer2 = Player2DB.GetPlayerWithUsername(Player2.Username);
			dbPlayer2.Experience += Server.BaseModifiers.ExpForDraw;
			dbPlayer2.GamesPlayed += 1;
			dbPlayer2.Money += (int)((Server.BaseModifiers.MoneyForVictory + Server.BaseModifiers.MoneyForLoss) / 2);
			Player2DB.UpdatePlayer(dbPlayer2);
			UpdateFleetShipsExp(Player2DB, Server.BaseModifiers.ExpForDraw, Player2Fleet);

			using (IGameDataBase tempDB = Server.GetGameDBContext()) {
				DbGameHistory entryToAdd = new DbGameHistory(tempDB.GetPlayerWithUsername(dbPlayer1.Username), tempDB.GetPlayerWithUsername(dbPlayer2.Username),
						tempDB.GetFleetWithId(Player1Fleet.Id), tempDB.GetFleetWithId(Player1Fleet.Id),
						true, DateTime.Now);
				tempDB.AddGameHistory(entryToAdd);
			}
		}

		private void HandleSuddenGameEnd(Player disconnectedPlayer, string reasonToLog) {
			if(IsFull) {    //all players were connected - need to set that disconnected player lost
				Server.Log(disconnectedPlayer.Username + ": " + reasonToLog);
				if (disconnectedPlayer.Username == Player1.Username) UpdateLoserAndWiner(Player1, Player2);
				else UpdateLoserAndWiner(Player2, Player1);
				EndGameThread();
			}
		}

		private void SetGameEnded() {
			if (!GameAlreadyEnded) {
				ContinueGameLoop = false;
			}
		}

		#region TcpConnection event handlers
		private void GameAbandonedHandler(object sender, EventArgs e) {
			if (!IsFull) {
				Server.Log(Player1.Username + ": abandoned his game before someone joined, ending game thread...");
				if (IsCustom) Server.AbandonCustomGameRoom(customRoomDescriptor);
				else Server.AbandonRankedGame(this);
				IsAbandoned = true;
				roomFull.Set();
				SendSuccess(Player1Conn);
			}
			else {
				Server.Log(Player1.Username + ": cannot abandon game if second player already joined");
				SendFailure(Player2Conn, FailureReasons.CANT_ABANDON);
			}
		}

		private void PlayerDisconnectedHandler(object sender, GameEventArgs e) {
			Server.Log("Player " + e.PlayerNumber + " disconnected");
			EndGameBeforeProperEnd(e.PlayerNumber, SUDDEN_DISCONNECT_LOG_STRING);
		}

		private void SurrenderHandler(object sender, GameEventArgs e) {
			Server.Log("Player " + e.PlayerNumber + " surrendered");
			EndGameBeforeProperEnd(e.PlayerNumber, SURRENDER_LOG_STRING);
		}

		private void EndGameBeforeProperEnd(int numberOfPlayerThatEndedGame, string reasonToLog) {
			lock (gameInProgressLock) {
				if (!GameAlreadyEnded) {
					GameAlreadyEnded = true;
					if (IsFull) {
						if (numberOfPlayerThatEndedGame == 1) {
							Player2Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(true, reasonToLog)));
							Player1ThreadObj.ClientConnected = false;   //tell user thread that player is no longer connected
							HandleSuddenGameEnd(Player1, reasonToLog);
						}
						else if (numberOfPlayerThatEndedGame == 2) {
							Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(true, reasonToLog)));
							Player2ThreadObj.ClientConnected = false;
							HandleSuddenGameEnd(Player2, reasonToLog);
						}
						else Server.Log(UsernamesOfPlayers + ": Invalid player number in sudden game end handler! Actual: " + numberOfPlayerThatEndedGame + ", should be 1 or 2", true);
					}
					else {
						Server.Log(UsernamesOfPlayers + ": Sudden game end before room was full - game abandoned");
						roomFull.Set();
						IsAbandoned = true;
					}
				}
			}
		}

		private void EndGameOnError(int numberOfPlayerThatEndedGame, string reason) {
			if (numberOfPlayerThatEndedGame == 1) SendFailure(Player1Conn, reason);
			else if (numberOfPlayerThatEndedGame == 1) SendFailure(Player2Conn, reason);
			fleetSetupOk = false;
			EndGameBeforeProperEnd(numberOfPlayerThatEndedGame, reason);
		}
		#endregion

		/// <summary>
		/// this should be the last operation called in this thread
		/// </summary>
		internal void EndGameThread() {
			ContinueGameLoop = false;
			lock (gameEndLock) {
				ClearPlayerNumberInConnection(Player1Conn);
				if(IsFull) ClearPlayerNumberInConnection(Player2Conn);
				gameEnded.Set();
			}
		}

		internal double GetMatchmakingScore() {
			lock(matchmakinglock) {
				return MatchMakingScore;
			}
		}

		private void ClearPlayerNumberInConnection(TcpConnection connection) {
			connection.PlayerNumber = 0;
		}

		/// <summary>
		/// sends <see cref="OperationType.FAILURE"/> <see cref="GamePacket"/> to client and logs reason and caller line
		/// </summary>
		private void SendFailure(TcpConnection playerConn, string reason, [CallerLineNumber] int callerLine = 0) {
			string failMsg = "! FAIL: Operation failed at: " + callerLine + ". Reason: " + reason;
			Server.Log(failMsg);
			playerConn.Send(new GamePacket(OperationType.FAILURE, reason));
		}

		private void SendSuccess(TcpConnection playerConn) {
			playerConn.Send(new GamePacket(OperationType.SUCCESS, new object()));
		}
		#endregion

	}
	#endregion

}
