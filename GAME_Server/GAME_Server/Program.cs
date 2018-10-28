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
using GAME_connection;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace GAME_Server {
	internal class Server {
		private static int port = TcpConnection.DEFAULT_PORT;
		private static string ip = "127.0.0.1";

		//database specific fields and properties
		private static IGameDataBase gameDataBase;
		private static List<Ship> allShips;
		private static List<Faction> allFactions;
		private static BaseModifiers baseModifiers;

		private static readonly object logLock = new object();

		//http://www.entityframeworktutorial.net/code-first/database-initialization-strategy-in-code-first.aspx
		//https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework60.html
		//https://stackoverflow.com/questions/50631210/mysql-with-entity-framework-6
		//https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application
		//https://stackoverflow.com/questions/21115776/setting-maxlength-for-all-strings-in-entity-framework-code-first
		//http://www.entityframeworktutorial.net/Querying-with-EDM.aspx
		//http://www.entityframeworktutorial.net/eager-loading-in-entity-framework.aspx
		//http://www.entityframeworktutorial.net/crud-operation-in-connected-scenario-entity-framework.aspx
		//sciagnij mysql connector i zainstaluj, potem dodaj referencje
		//nuget package manager -> browse -> MySQL i dodaj MySQL.Data.Entity (zwykly MySQL.Data powinien byc dodany wczesniej przy instalacji connectora i dodaniu referencji)
		//6.9.12 mysql dziala

		//https://docs.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/checkout-and-payment-with-paypal
		//wymagane jest ssl i/lub tls 1.2 bo inaczej paypal moze odrzucic, wymagane sa certyfikaty, informacje sa tylko o aplikacjach webowych w przegladarce, jakies dane przekazywane przez sesje - trzebaby robic to inaczej
		//w sumie kilkaset linii kodu - w tym ponad 300 na sama klase z tutoriala (a sama klasa nie wystarczy)

		// - historia rozgrywek, kto z kim i jakie floty
		// - wirtualna waluta i kupowanie kart, przypisywanie kart do gracza, player w wersji DB, many-to-many
		// - turnieje po okolo 8 graczy o duze nagrody
		// - apka windows forms dla admina

		internal static IGameDataBase GameDataBase { get => gameDataBase; }
		public static List<Ship> AllShips { get => allShips; }
		public static List<Faction> AllFactions { get => allFactions; }
		public static BaseModifiers BaseModifiers { get => baseModifiers; }

		//thread management specific fields and properties
		private static List<Thread> userThreads = new List<Thread>();

		internal static bool continueAcceptingConnections = true;

		static void Main(string[] args) {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");   //to change exception language to english
			InitilizeGameDataFromDB();

			IPAddress ipAddress = IPAddress.Parse(ip);
			TcpListener listener = new TcpListener(ipAddress, port);
			listener.Start();
			Log("Server listening on: " + ip + ":" + port);

			while (continueAcceptingConnections) {
				Log("Server is waiting for client...");
				TcpClient client = listener.AcceptTcpClient();
				TcpConnection gameClient = new TcpConnection(client, false);
				Log("Client connected - ip: " + gameClient.RemoteIpAddress + " port: " + gameClient.RemotePortNumber);

				Thread t = new Thread(new ParameterizedThreadStart(Test));
				//UserThread userThread = new UserThread(gameClient);
				//Thread t = new Thread(new ThreadStart(userThread.RunUserThread));
				userThreads.Add(t);
				t.Start(gameClient);
			}
		}

		/// <summary>
		/// Reads basic game data from DB into memory. Does not read player and fleet data, these should be read by user threads
		/// </summary>
		private static void InitilizeGameDataFromDB() {
			gameDataBase = new MySqlDataBase();

			DbPlayer p1 = new DbPlayer(1, "player1", "haslo1", 0, 100, 0, 0, 0);
			DbPlayer p2 = new DbPlayer(2, "player2", "haslo2", 0, 100, 0, 0, 0);

			gameDataBase.AddPlayer(p1);
			gameDataBase.AddPlayer(p2);

			Thread.Sleep(500);
			if (Server.GameDataBase.PlayerExists(p1.ToPlayer())) Console.WriteLine("ano jest");
			else Console.WriteLine("nima");

			Thread.Sleep(500);

			var f1 = new Faction(1, "test");
			var f2 = new Faction(2, "test2");
			gameDataBase.AddFaction(f1);
			gameDataBase.AddFaction(f2);
			var faction = gameDataBase.GetAllFactions().First();

			DbWeapon w1 = new DbWeapon(1, "w1", faction, 10.0, 15, WeaponType.KINETIC, 1.5, 1.4, 12.0);
			DbWeapon w2 = new DbWeapon(2, "w2", faction, 12.0, 15, WeaponType.LASER, 2.6, 5.4, 88.0);
			DbWeapon w3 = new DbWeapon(3, "w3", faction, 10.0, 17, WeaponType.KINETIC, 1.0, 1.4, 55.0);
			gameDataBase.AddWeapon(w1);
			gameDataBase.AddWeapon(w2);
			gameDataBase.AddWeapon(w3);
			DbDefenceSystem d1 = new DbDefenceSystem(1, "d1", faction, 5.0, DefenceSystemType.SHIELD, 2.0, 2.0, 1.3);
			DbDefenceSystem d2 = new DbDefenceSystem(2, "s2", faction, 3.0, DefenceSystemType.INTEGRITY_FIELD, 1.2, 1.3, 1.5);
			gameDataBase.AddDefenceSystem(d1);
			gameDataBase.AddDefenceSystem(d2);

			var weps = gameDataBase.GetAllWeapons();
			var defs = gameDataBase.GetAllDefences(); 
			List<DbWeapon> weapons1 = new List<DbWeapon> {
				weps[0],
				weps[1],
				weps[2]
			};
			List<DbWeapon> weapons2 = new List<DbWeapon> {
				weps[0],
				weps[2]
			};
			List<DbWeapon> weapons3 = new List<DbWeapon> {
				weps[0],
				weps[2]
			};
			List<DbDefenceSystem> denences = new List<DbDefenceSystem> {
				defs[0],
				defs[1]
			};
			List<DbDefenceSystem> denences2 = new List<DbDefenceSystem> {
				defs[0],
				defs[1]
			};
			DbShip s1 = new DbShip(1, "s1", faction, 10, 10.0, 1000.0, 4.0, 54.0, weapons1, denences, 2000);
			DbShip s2 = new DbShip(2, "s2", faction, 20, 5.0, 500.0, 1.0, 23.0, weapons2, denences2, 1000);
			gameDataBase.AddShip(s1);
			gameDataBase.AddShip(s2);

			Thread.Sleep(500);
			var ships = gameDataBase.GetAllShips();
			foreach(DbShip ship in ships) Console.WriteLine(ship.Name + " " + ship.Id + " " + ship.Weapons[0].Name);

			Console.WriteLine("teraz apdejt");
			DbShip sUp = new DbShip(1, "s1", faction, 99, 10.0, 9999.0, 4.0, 54.0, weapons3, denences, 2000);
			gameDataBase.UpdateShip(sUp);

			Thread.Sleep(500);
			Console.WriteLine("i delete");
			gameDataBase.RemoveShipWithId(1);
			//UWAGA NA UZYWANIE LIST!!! DLA KAZDEGO NOWEGO OBIEKTU ROB NOWA LISTE BO INACZEJ DOSTAJE DALNA I WYWALA CALA JOIN TABLE!!! 
			//REFERENCJE W TYCH LISTACH MOGA BYC TE SAME ALE INSTANCJE LIST MUSZA BYC ROZNE!!!

			Environment.Exit(0);

			/*baseModifiers = GameDataBase.GetBaseModifiers();
			allFactions = GameDataBase.GetAllFactions();
			allShips = GameDataBase.GetAllShips();*/
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

		/// <summary>
		/// To ne used instead of <see cref="Console.WriteLine"/>, prints message to chosen log - console, text area etc.
		/// </summary>
		/// <param name="message"></param>
		internal static void Log(string message) {
			lock (logLock) {
				Console.WriteLine(message);
			}
		}

		/// <summary>
		/// To ne used instead of <see cref="Console.WriteLine"/>, prints message to chosen log - console, text area etc. This overload also logs caller line
		/// </summary>
		internal static void Log(string message, bool isCritical, [CallerLineNumber] int callerLine = 0) {
			string msg = "";
			if (isCritical) msg += "CRITICAL: ";
			msg = message + " at line: " + callerLine;
			lock (logLock) {
				Console.WriteLine(msg);
			}
		}

		/// <summary>
		/// Temporary connection tester method. Should be removed later
		/// </summary>
		/// <param name="clientObj"></param>
		private static void Test(object clientObj) {
			try {
				TcpConnection client = (TcpConnection)clientObj;

				Console.WriteLine("Trying to receive...");
				GamePacket packet = client.GetReceivedPacket();
				Console.WriteLine("Received packet: " + packet.OperationType);

				Console.WriteLine("Sleeping some time...");
				Thread.Sleep(3000);

				Console.WriteLine("Trying to send...");
				string msg = "server test msg";
				client.Send(new GamePacket(OperationType.LOGIN, msg));

				Console.WriteLine("Trying to receive with timeout...");
				try {
					client.GetReceivedPacket(2000, 1);
				} catch (ReceiveTimeoutException e) {
					Console.WriteLine("Failed to receive with timeout. Disconnected player number: " + e.PlayerNumber);
				}

				Console.WriteLine("Trying to receive complex packet...");
				packet = client.GetReceivedPacket();
				Fleet fleet = (Fleet)packet.Packet;
				Console.WriteLine("there are " + fleet.Ships.Count + " ships in the received fleet");

				Fleet clonedFleet = CloneObject(fleet);
				Console.WriteLine("there are " + fleet.Ships.Count + " ships in the CLONED fleet");

				Console.WriteLine("Waiting for client to send DISCONNECT...");
				packet = client.GetReceivedPacket();
				if (packet.OperationType == OperationType.DISCONNECT) {
					Console.WriteLine("received disconnect");
					client.Disconnect();
				}
				else Console.WriteLine("not ok");

				Console.WriteLine("All OK! Closing...");
				//Thread.Sleep(1000);
				//client.Dispose();
			} catch(ConnectionEndedException ex) {
				Console.WriteLine("Exception: " + ex.Message + "Exception type: " + ex.ToString());
			}
			Console.WriteLine("Test end");
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

		internal UserThread(TcpConnection client) {
			this.Client = client;
		}

		public TcpConnection Client { get => client; set => client = value; }
		public Player User { get => user; set => user = value; }
		#endregion

		/// <summary>
		/// main function of <see cref="UserThread"/>
		/// </summary>
		internal void RunUserThread() {
			try {
				//first do login or register
				bool loginSuccess = false;
				while (!loginSuccess) {		//while login is not succesful
					loginSuccess = LoginOrRegister();
				}
				if(loginSuccess) {
					Client.Send(new GamePacket(OperationType.SUCCESS, new object()));
					//set and send user data
					this.User = Server.GameDataBase.GetPlayerWithUsername(this.User.Username).ToPlayer();
					Client.Send(new GamePacket(OperationType.PLAYER_DATA, this.User));
				}
			} catch(ConnectionEndedException) {
				Server.Log(FailureReasons.CLIENT_DISCONNECTED, true);
			}
		}

		/// <summary>
		/// Checks if login or register is allowed for given player data and returns true if it is so. Receive 1 msg (typeof(packet) = Player), send 1 msg (opType = FAILURE/SUCCESS)
		/// </summary>
		/// <returns></returns>
		private bool LoginOrRegister() {
			GamePacket packet = Client.GetReceivedPacket();
			Player playerObject;
			//cast to proper type
			try {
				playerObject = Server.CastPacketToProperType(packet.Packet, OperationsMap.OperationMapping[packet.OperationType]);
			} catch(InvalidCastException) {
				SendFailure(FailureReasons.INVALID_INTERNAL_PACKET);
				return false;
			}
			//if type ok do login or register
			if(packet.OperationType == OperationType.LOGIN) {
				if (Server.GameDataBase.PlayerExists(playerObject) && Server.GameDataBase.ValidateUser(playerObject)) {
					this.User = playerObject;
					return true;
				}
				else {
					SendFailure(FailureReasons.INCORRECT_LOGIN + playerObject.Username);
					return false;
				}
			}
			else if(packet.OperationType == OperationType.REGISTER) {
				if ((!Server.GameDataBase.PlayerExists(playerObject)) && Server.GameDataBase.PlayerNameIsUnique(playerObject)) {
					Server.GameDataBase.AddPlayer(new DbPlayer(playerObject));
					this.User = playerObject;
					return true;
				}
				else {
					SendFailure(FailureReasons.USERNAME_ALREADY_EXISTS + playerObject.Username);
					return false;
				}
			}
			else if(packet.OperationType == OperationType.DISCONNECT) {
				Server.Log("User disconnected before succesful login!");
				return false;
			}
			else {
				SendFailure(FailureReasons.INVALID_PACKET_TYPE);
				return false;
			}
		}

		/// <summary>
		/// sends <see cref="OperationType.FAILURE"/> <see cref="GamePacket"/> to client and logs reason and caller line
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="callerLine">DO NOT SET</param>
		private void SendFailure(string reason, [CallerLineNumber] int callerLine = 0) {
			string failMsg = "Operation Failed! at: " + callerLine + ". Reason: " + reason;
			Server.Log(failMsg);
			this.Client.Send(new GamePacket(OperationType.FAILURE, reason));
		}

	}
	#endregion

	#region Game Room Thread
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	//---------GAME ROOM THREAD---------------------------------------------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	internal class GameRoomThread {
		#region properties attributes and constructors
		private Player player1;     //host
		private Player player2;

		private TcpConnection player1Conn;	//host
		private TcpConnection player2Conn;

		internal GameRoomThread(TcpConnection hostConnection, Player host) {
			Player1 = host;
		}

		public Player Player1 { get => player1; set => player1 = value; }
		public Player Player2 { get => player2; set => player2 = value; }
		public TcpConnection Player1Conn { get => player1Conn; set => player1Conn = value; }
		public TcpConnection Player2Conn { get => player2Conn; set => player2Conn = value; }
		#endregion

		/// <summary>
		/// main function of <see cref="GameRoomThread"/>
		/// </summary>
		internal void RunGameThread() {

		}

	}
	#endregion

}
