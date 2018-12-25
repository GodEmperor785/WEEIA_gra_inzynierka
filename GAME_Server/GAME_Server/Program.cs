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

namespace GAME_Server {
	internal class Server {
		public static readonly int EXIT_STATUS_MANUAL_SHUTDOWN = 1;

		private static int port = TcpConnection.DEFAULT_PORT;
		private static string ip = "25.34.213.187";

		//database specific fields and properties
		private static IGameDataBase gameDataBase;		//used only for initialization of BaseModifiers etc. Other IGameDataBase are created in user threads
		private static BaseModifiers baseModifiers;

		//locks
		private static readonly object logLock = new object();
		private static readonly object customRoomsLock = new object();
		private static readonly object rankedRoomsLock = new object();

		//http://www.entityframeworktutorial.net/code-first/database-initialization-strategy-in-code-first.aspx
		//https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework60.html
		//https://stackoverflow.com/questions/50631210/mysql-with-entity-framework-6
		//https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application
		//https://stackoverflow.com/questions/21115776/setting-maxlength-for-all-strings-in-entity-framework-code-first
		//http://www.entityframeworktutorial.net/Querying-with-EDM.aspx
		//http://www.entityframeworktutorial.net/eager-loading-in-entity-framework.aspx
		//http://www.entityframeworktutorial.net/crud-operation-in-connected-scenario-entity-framework.aspx
		//https://mehdi.me/ambient-dbcontext-in-ef6/
		//https://old.windowsvalley.com/uninstall-mysql-from-windows/
		//sciagnij mysql connector i zainstaluj, potem dodaj referencje
		//nuget package manager -> browse -> MySQL i dodaj MySQL.Data.Entity (zwykly MySQL.Data powinien byc dodany wczesniej przy instalacji connectora i dodaniu referencji)
		//6.9.12 mysql dziala

		//https://docs.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/checkout-and-payment-with-paypal
		//wymagane jest ssl i/lub tls 1.2 bo inaczej paypal moze odrzucic, wymagane sa certyfikaty, informacje sa tylko o aplikacjach webowych w przegladarce, jakies dane przekazywane przez sesje - trzebaby robic to inaczej
		//w sumie kilkaset linii kodu - w tym ponad 300 na sama klase z tutoriala (a sama klasa nie wystarczy)

		// - turnieje po okolo 8 graczy o duze nagrody
		// - apka windows forms dla admina

		//DLA WEAPONS I DEFENCE_SYSTEMS:
		// zeby byly duplikaty a raczej ich obejscie to beda nowe rekordy typu "Kinetic Cannon 120mm x4" co oznacza poczworne dzialo typu "Kinetic Cannon 120mm"

		//aplikacja admina nie powinna dzialac podczas dzialania serwera gry - serwer powinien byc wylaczany na czas potrzebny adminowi do zmian!

		internal static IGameDataBase GameDataBase { get => gameDataBase; }
		public static BaseModifiers BaseModifiers { get => baseModifiers; }

		//thread management specific fields and properties
		private static List<Thread> userThreads = new List<Thread>();
		private static List<UserThread> userThreadObjects = new List<UserThread>();
		internal static Dictionary<CustomGameRoom, GameRoomThread> availableCustomGameRooms = new Dictionary<CustomGameRoom, GameRoomThread>();
		internal static List<GameRoomThread> availableRankedGameRooms = new List<GameRoomThread>();

		internal static bool continueAcceptingConnections = true;

		static void Main(string[] args) {
			try {
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");   //to change exception language to english

				Log("Getting configuration...");
				var useSslVar = ConfigurationManager.AppSettings["useSsl"];
				bool useSsl = Convert.ToBoolean(useSslVar);
				Log("Server use SSL: " + useSsl);

				//DoGameBoardValidationTest();
				
				InitilizeGameDataFromDB(false, true);       //change both to true to run ONLY DB test inserts, false and true to continue on debug DB, both false to dont change DB and use existing one

				//IPAddress ipAddress = IPAddress.Parse(ip);
				//TcpListener listener = new TcpListener(ipAddress, port);
				TcpListener listener = new TcpListener(IPAddress.Any, port);
				listener.Start();
				Log("Server listening on port : " + port);
				Thread cliThread = new Thread(new ThreadStart(ServerCLI));
				cliThread.Start();

				while (continueAcceptingConnections) {
					Log("Server is waiting for client...");
					TcpClient client = listener.AcceptTcpClient();
					TcpConnection gameClient;
					//gameClient = new TcpConnection(client, false, Server.Log, true, true, "hamachi.cer");		//uncomment this to enable ssl (with hamachi certificate)
					if (useSsl) gameClient = new TcpConnection(client, false, Server.Log, true, true, "gameServerCert.cer");        //use this to enable ssl (with public certificate)
					else gameClient = new TcpConnection(client, false, Server.Log);
					Log("Client connected - ip: " + gameClient.RemoteIpAddress + " port: " + gameClient.RemotePortNumber);

					//Thread t = new Thread(new ParameterizedThreadStart(Test));
					UserThread userThread = new UserThread(gameClient);
					Thread t = new Thread(new ThreadStart(userThread.RunUserThread));
					userThreads.Add(t);
					userThreadObjects.Add(userThread);
					//t.Start(gameClient);
					t.Start();
				}
			} catch(Exception critical) {
				Log("UNHANDLED EXCEPTION HAPPENED - STOPPING SERVER", true);
				Log(critical.Message);
				Log(critical.Source);
				Log(critical.StackTrace);
				Console.ReadKey();
			}
			
		}

		private static void ServerCLI() {
			string cmd = Console.ReadLine();
			switch(cmd) {
				case "exit":
					Log("exiting...");
					continueAcceptingConnections = false;
					foreach(UserThread t in userThreadObjects) {
						t.ClientConnected = false;
						t.EndThread();
					}
					Environment.Exit(EXIT_STATUS_MANUAL_SHUTDOWN);
					break;
				default:
					Log("ERROR: unknown command - " + cmd);
					break;
			}
		}

		//========================= GAME ROOM LIST/DICT UTILS ==================================================================================================================
		internal static List<CustomGameRoom> GetAvailableCustomRooms() {
			List<CustomGameRoom> availableRooms = new List<CustomGameRoom>();
			lock(customRoomsLock) {
				foreach(var roomPair in availableCustomGameRooms) {
					availableRooms.Add(new CustomGameRoom(roomPair.Key));
				}
			}
			return availableRooms;
		}

		private static void RemoveCustomRoom(CustomGameRoom roomToRemove) {
			CustomGameRoom removeIt = new CustomGameRoom();
			foreach (var roomPair in availableCustomGameRooms) {
				if (roomToRemove.RoomName == roomPair.Key.RoomName) {
					removeIt = roomPair.Key;
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

		//====================================================================================================================================================================================
		//===DB_TEST==========================================================================================================================================================================
		//====================================================================================================================================================================================
		/// <summary>
		/// Reads basic game data from DB into memory. Does not read player and fleet data, these should be read by user threads
		/// </summary>
		private static void InitilizeGameDataFromDB(bool exitOnFinish, bool dropCreate) {
			gameDataBase = GetGameDBContext();
			if (dropCreate) {
				Server.Log("WARNING: DEBUG - DB TEST PROGRAM WILL EXIT AFTER THE TEST", true);
				Server.Log(">>> BaseModifiers first");
				DbBaseModifiers mods = new DbBaseModifiers() {
					Id = 1,
					KineticRange = 1,			//each of range mults has to be from 0.0 to 1.0
					LaserRange = 0.5,			//they indicate how important is range for given WeaponType to hit its target, look in Game.cs at chanceToHit calculations for more
					MissileRange = 0.0,			//missile are invulnerable to range

					KineticPD = 2,
					KineticShield = 5,
					KineticIF = 1.2,
					LaserPD = 0,
					LaserShield = 0.5,
					LaserIF = 2,
					MissilePD = 8,
					MissileShield = 1,
					MissileIF = 1.2,

					BaseShipStatsExpModifier = 0.01,
					MaxShipsPerPlayer = 150,
					StartingMoney = 1000,
					ExpForVictory = 20,
					ExpForLoss = 10,
					FleetSizeExpModifier = 0.5,
					BaseFleetMaxSize = 1000,
					MaxAbsoluteFleetSize = 5000,
					MaxShipExp = 1000,
					MaxShipsInLine = 5,
					MaxFleetsPerPlayer = 8,
					MoneyForVictory = 80,
					MoneyForLoss = 40
				};
				GameDataBase.AddBaseModifiers(mods);
				Server.baseModifiers = GameDataBase.GetBaseModifiers();

				Server.Log(">>> Some factions");
				Faction empire = new Faction(1, "Empire");
				Faction alliance = new Faction(2, "Alliance");
				Faction union = new Faction(3, "Union");
				GameDataBase.AddFaction(empire);
				GameDataBase.AddFaction(alliance);
				GameDataBase.AddFaction(union);
				var factionList = GameDataBase.GetAllFactions();
				empire = factionList.ElementAt(0);
				alliance = factionList.ElementAt(1);
				union = factionList.ElementAt(2);
				Server.Log("Factions in DB are:");
				foreach (Faction f in factionList) Server.Log(f.Name);

				Server.Log(">>> Some users");
				string p1Name = "player1";
				string p2Name = "player2";
				DbPlayer p1 = new DbPlayer(p1Name, p1Name, BaseModifiers.ExpForVictory * 2 + BaseModifiers.ExpForLoss, 300, 3, 2, BaseModifiers.StartingMoney);
				DbPlayer p2 = new DbPlayer(p2Name, p2Name, BaseModifiers.ExpForLoss * 2 + BaseModifiers.ExpForVictory, 300, 3, 1, BaseModifiers.StartingMoney);
				GameDataBase.AddPlayer(p1);
				GameDataBase.AddPlayer(p2);
				var playersList = GameDataBase.GetAllPlayers();
				Server.Log("Players in DB are:");
				foreach (DbPlayer p in playersList) Server.Log(p.Username);

				Server.Log(">>> Some lootboxes");
				DbLootBox l1 = new DbLootBox(100, "basic lootbox", 0.5, 0.3, 0.15, 0.05, 2);		//lootbox names hardcoded in client - better not change it...
				DbLootBox l2 = new DbLootBox(300, "better lootbox", 0.4, 0.4, 0.15, 0.05, 4);
				DbLootBox l3 = new DbLootBox(1000, "supreme lootbox", 0.1, 0.3, 0.4, 0.2, 4);
				GameDataBase.AddLootBox(l1);
				GameDataBase.AddLootBox(l2);
				GameDataBase.AddLootBox(l3);
				var lootboxList = GameDataBase.GetAllLootBoxes();
				Server.Log("Lootboxes in DB are:");
				foreach (DbLootBox l in lootboxList) Server.Log(l.Name);

				Server.Log(">>> Some weapons and defences");
				DbWeapon w1 = new DbWeapon("kinetic 100mm", empire, 40, 6, WeaponType.KINETIC, 0.5, 0.5, 10);
				DbWeapon w2 = new DbWeapon("kinetic 5mm", alliance, 2.5, 80, WeaponType.KINETIC, 1, 0.2, 2);
				DbWeapon w3 = new DbWeapon("axial UV laser", alliance, 250, 1, WeaponType.LASER, 0.2, 0.85, 55);
				DbWeapon w4 = new DbWeapon("IR laser turret", empire, 20, 4, WeaponType.LASER, 0.5, 0.7, 15);
				DbWeapon w5 = new DbWeapon("imperial cruise missile", empire, 200, 1, WeaponType.MISSILE, 0, 0.999, 45);
				DbWeapon w6 = new DbWeapon("alliance swarm missile", alliance, 20, 10, WeaponType.MISSILE, 0, 0.99, 5);
				gameDataBase.AddWeapon(w1);
				gameDataBase.AddWeapon(w2);
				gameDataBase.AddWeapon(w3);
				gameDataBase.AddWeapon(w4);
				gameDataBase.AddWeapon(w5);
				gameDataBase.AddWeapon(w6);
				DbDefenceSystem d1 = new DbDefenceSystem("imperial PD", empire, 100, DefenceSystemType.POINT_DEFENCE, 2, 0, 4);
				DbDefenceSystem d2 = new DbDefenceSystem("alliance PD", alliance, 80, DefenceSystemType.POINT_DEFENCE, 2.5, 0, 5);
				DbDefenceSystem d3 = new DbDefenceSystem("imperial shield", empire, 60, DefenceSystemType.SHIELD, 4, 1, 1.2);
				DbDefenceSystem d4 = new DbDefenceSystem("alliance shield", alliance, 80, DefenceSystemType.SHIELD, 3.5, 0.95, 1.2);
				DbDefenceSystem d5 = new DbDefenceSystem("imperial IF", empire, 45, DefenceSystemType.INTEGRITY_FIELD, 3, 3, 3);
				DbDefenceSystem d6 = new DbDefenceSystem("alliance IF", alliance, 40, DefenceSystemType.INTEGRITY_FIELD, 3, 4, 3);
				gameDataBase.AddDefenceSystem(d1);
				gameDataBase.AddDefenceSystem(d2);
				gameDataBase.AddDefenceSystem(d3);
				gameDataBase.AddDefenceSystem(d4);
				gameDataBase.AddDefenceSystem(d5);
				gameDataBase.AddDefenceSystem(d6);
				var weps = gameDataBase.GetAllWeapons();
				Server.Log("Weapons in DB are:");
				foreach (var p in weps) Server.Log(p.Name);
				var defs = gameDataBase.GetAllDefences();
				Server.Log("Defences in DB are:");
				foreach (var p in defs) Server.Log(p.Name);

				Server.Log(">>> Some ship templates");
				List<DbWeapon> al1wep = new List<DbWeapon> {
					weps[1],
					weps[2],
					weps[5]
				};
				List<DbWeapon> al2wep = new List<DbWeapon> {
					weps[1],
					weps[5]
				};
				List<DbWeapon> imp1wep = new List<DbWeapon> {
					weps[0],
					weps[3],
					weps[4]
				};
				List<DbWeapon> imp2wep = new List<DbWeapon> {
					weps[0],
					weps[3]
				};
				List<DbDefenceSystem> al1def = new List<DbDefenceSystem> {
					defs[1],
					defs[3],
					defs[5]
				};
				List<DbDefenceSystem> al2def = new List<DbDefenceSystem> {
					defs[1],
					defs[5]
				};
				List<DbDefenceSystem> imp1def = new List<DbDefenceSystem> {
					defs[0],
					defs[2],
					defs[4]
				};
				List<DbDefenceSystem> imp2def = new List<DbDefenceSystem> {
					defs[0],
					defs[4]
				};
				DbShipTemplate imp1 = new DbShipTemplate("Class Warrior Imperial Cruiser", empire, 90, 0.5, 500, 3, 75, imp1wep, imp1def, 0, Rarity.RARE);
				DbShipTemplate imp2 = new DbShipTemplate("Class Dagger Imperial Destroyer", empire, 30, 0.9, 150, 1, 25, imp2wep, imp2def, 0, Rarity.COMMON);
				DbShipTemplate al1 = new DbShipTemplate("Class Hammer Alliance Cruiser", alliance, 95, 0.5, 550, 3, 80, al1wep, al1def, 0, Rarity.RARE);
				DbShipTemplate al2 = new DbShipTemplate("Class Ferret Alliance Destroyer", alliance, 30, 0.88, 175, 1, 28, al2wep, al2def, 0, Rarity.COMMON);
				gameDataBase.AddShipTemplate(imp1);
				gameDataBase.AddShipTemplate(imp2);
				gameDataBase.AddShipTemplate(al1);
				gameDataBase.AddShipTemplate(al2);
				var shipTemplates = GameDataBase.GetAllShipTemplates();
				Server.Log("Ship Templates in DB are:");
				foreach (var p in shipTemplates) Server.Log(p.Name + " weapon count=" + p.Weapons.Count);

				Server.Log(">>> Some ships for players");
				DbShip s1p1 = shipTemplates[2].GenerateNewShipOfThisTemplate(p1);
				DbShip s2p1 = shipTemplates[2].GenerateNewShipOfThisTemplate(p1);
				DbShip s3p1 = shipTemplates[3].GenerateNewShipOfThisTemplate(p1);
				DbShip s1p2 = shipTemplates[0].GenerateNewShipOfThisTemplate(p2);
				DbShip s2p2 = shipTemplates[1].GenerateNewShipOfThisTemplate(p2);
				DbShip s3p2 = shipTemplates[1].GenerateNewShipOfThisTemplate(p2);
				GameDataBase.AddShip(s1p1);
				GameDataBase.AddShip(s2p1);
				GameDataBase.AddShip(s3p1);
				GameDataBase.AddShip(s1p2);
				GameDataBase.AddShip(s2p2);
				GameDataBase.AddShip(s3p2);
				var ships = GameDataBase.GetAllShips();
				Server.Log("Ships in DB are:");
				foreach (var p in ships) Server.Log("Ship of template: " + p.ShipBaseStats.Name + " owned by: " + p.Owner.Username);

				Server.Log(">>> Some fleets for players");
				List<DbShip> p1fleet = new List<DbShip> {
					ships[0],
					ships[1],
					ships[2]
				};
				List<DbShip> p2fleet = new List<DbShip> {
					ships[3],
					ships[4],
					ships[5]
				};
				DbFleet f1 = new DbFleet(p1, p1fleet, p1Name + "_Fleet");
				DbFleet f2 = new DbFleet(p2, p2fleet, p2Name + "_Fleet");
				GameDataBase.AddFleet(f1.ToFleet(), p1.ToPlayer());
				GameDataBase.AddFleet(f2.ToFleet(), p2.ToPlayer());
				var fleetsP1 = GameDataBase.GetAllFleetsOfPlayer(p1.ToPlayer());
				var fleetsP2 = GameDataBase.GetAllFleetsOfPlayer(p2.ToPlayer());
				Server.Log("Fleets in DB are:");
				foreach (var p in fleetsP1) Server.Log("fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);
				foreach (var p in fleetsP2) Server.Log("fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);

				Server.Log(">>> Some game history");
				DbGameHistory h1 = new DbGameHistory(p1, p2, fleetsP1.First(), fleetsP2.First(), false, new DateTime(2018, 10, 28, 11, 15, 58));
				DbGameHistory h2 = new DbGameHistory(p1, p2, fleetsP1.First(), fleetsP2.First(), false, new DateTime(2018, 10, 29, 12, 25, 1));
				DbGameHistory h3 = new DbGameHistory(p2, p1, fleetsP2.First(), fleetsP1.First(), false, new DateTime(2018, 11, 6, 18, 2, 30));
				GameDataBase.AddGameHistory(h1);
				GameDataBase.AddGameHistory(h2);
				GameDataBase.AddGameHistory(h3);
				var historiesP1 = GameDataBase.GetPlayersGameHistory(p1.Id);
				var historiesP2 = GameDataBase.GetPlayersGameHistory(p2.Id);
				Server.Log("Game Histories in DB are:");
				foreach (var p in historiesP1) Server.Log("winner: " + p.Winner.Username + " loser: " + p.Loser.Username + " winner fleet: " + p.WinnerFleet.Name);
				foreach (var p in historiesP2) Server.Log("winner: " + p.Winner.Username + " loser: " + p.Loser.Username + " winner fleet: " + p.WinnerFleet.Name);
				var x3 = gameDataBase.GetGameHistoryEntry(1);
				Server.Log("winner: " + x3.Winner.Username + " loser: " + x3.Loser.Username + " winner fleet: " + x3.WinnerFleet.Name + " fleet first ship weapon faction name: "
					+ x3.WinnerFleet.Ships[0].ShipBaseStats.Weapons[0].Faction.Name);

				Server.Log(">>> Additional ship templates and weapons for application testing - so the DB is more or less complete");
				Server.Log(">>> first more weapons and defences");
				DbWeapon w7 = new DbWeapon("kinetic 100mm x4", empire, 40, 6 * 4, WeaponType.KINETIC, 0.5, 0.5, 10);
				DbWeapon w8 = new DbWeapon("axial HE UV femtosecond laser", empire, 2000, 1, WeaponType.LASER, 0.25, 0.9, 100);
				DbWeapon w9 = new DbWeapon("neutron warhead missiles x8", alliance, 50, 8, WeaponType.MISSILE, 0, 0.995, 900);
				DbWeapon w10 = new DbWeapon("Marauder cruise missile launchers x10", alliance, 210, 10, WeaponType.MISSILE, 0, 0.999, 70);
				gameDataBase.AddWeapon(w7);
				gameDataBase.AddWeapon(w8);
				gameDataBase.AddWeapon(w9);
				gameDataBase.AddWeapon(w10);
				DbDefenceSystem d7 = new DbDefenceSystem("Imperial Smart IF", empire, 60, DefenceSystemType.INTEGRITY_FIELD, 6, 6, 6);
				DbDefenceSystem d8 = new DbDefenceSystem("alliance High Power IF", alliance, 110, DefenceSystemType.INTEGRITY_FIELD, 3, 3, 3);
				DbDefenceSystem d9 = new DbDefenceSystem("imperial heavy shield", empire, 250, DefenceSystemType.SHIELD, 4, 1, 1.2);
				DbDefenceSystem d10 = new DbDefenceSystem("alliance multicore shield", alliance, 100, DefenceSystemType.SHIELD, 8, 1.2, 6);
				DbDefenceSystem d11 = new DbDefenceSystem("imperial mass PD", empire, 180, DefenceSystemType.POINT_DEFENCE, 4, 0, 10);
				DbDefenceSystem d12 = new DbDefenceSystem("alliance high precision laser PD", alliance, 250, DefenceSystemType.POINT_DEFENCE, 2.5, 0, 6);
				gameDataBase.AddDefenceSystem(d7);
				gameDataBase.AddDefenceSystem(d8);
				gameDataBase.AddDefenceSystem(d9);
				gameDataBase.AddDefenceSystem(d10);
				gameDataBase.AddDefenceSystem(d11);
				gameDataBase.AddDefenceSystem(d12);
				weps = gameDataBase.GetAllWeapons();
				Server.Log("Weapons in DB are:");
				foreach (var p in weps) Server.Log(p.Name);
				defs = gameDataBase.GetAllDefences();
				Server.Log("Defences in DB are:");
				foreach (var p in defs) Server.Log(p.Name);

				Server.Log(">>> Now some rare templates");
				List<DbWeapon> impRareWep = new List<DbWeapon> {
					weps[3],
					weps[4],
					weps[6],
					weps[7]
				};
				List<DbWeapon> AlRareWep = new List<DbWeapon> {
					weps[1],
					weps[5],
					weps[8],
					weps[9]
				};
				List<DbDefenceSystem> ImpRareDef = new List<DbDefenceSystem> {
					defs[6],
					defs[8],
					defs[10]
				};
				List<DbDefenceSystem> AlRareDef = new List<DbDefenceSystem> {
					defs[7],
					defs[9],
					defs[11]
				};
				DbShipTemplate impRare = new DbShipTemplate("Class Master Imperial Battleship", empire, 300, 0.1, 2500, 8, 220, impRareWep, ImpRareDef, 0, Rarity.LEGENDARY);
				DbShipTemplate alRare = new DbShipTemplate("Class Apocalypse Alliance Missile Ship", alliance, 250, 0.15, 1200, 6.5, 140, AlRareWep, AlRareDef, 0, Rarity.VERY_RARE);
				gameDataBase.AddShipTemplate(impRare);
				gameDataBase.AddShipTemplate(alRare);
				shipTemplates = GameDataBase.GetAllShipTemplates();
				Server.Log("Ship Templates in DB are:");
				foreach (var p in shipTemplates) Server.Log(p.Name + " weapon count=" + p.Weapons.Count);

				//place for future testing

				Server.Log("DEBUG: TEST END", true);
				if (exitOnFinish) {
					Server.Log("DEBUG: exiting application and closing connections", true);
					Console.ReadKey();
					GameDataBase.Dispose();
					Environment.Exit(0);
				}
			}
			else {  //get only base modifiers
				Server.baseModifiers = GameDataBase.GetBaseModifiers();
				Player pl = new Player(1, "", "", 0, 0, 0, 0, 0);
				var x = gameDataBase.GetAllFleetsOfPlayer(pl);
				foreach (var p in x) Server.Log("fleet name: " + p.Name + " owned by: " + p.Owner.Username + " with ship count: " + p.Ships.Count);
				var x2 = gameDataBase.GetPlayersGameHistory(1);
				foreach (var p in x2) Server.Log("winner: " + p.Winner.Username + " loser: " + p.Loser.Username + " winner fleet: " + p.WinnerFleet.Name);
				var x3 = gameDataBase.GetGameHistoryEntry(1);
				Server.Log("winner: " + x3.Winner.Username + " loser: " + x3.Loser.Username + " winner fleet: " + x3.WinnerFleet.Name + " fleet first ship weapon faction name: " 
					+ x3.WinnerFleet.Ships[0].ShipBaseStats.Weapons[0].Faction.Name );
			}


			/*string p1Name = "player1";
			DbPlayer p1 = new DbPlayer(1, p1Name, "haslo1", 0, 100, 0, 0, 0);
			DbPlayer p2 = new DbPlayer(2, "player2", "haslo2", 0, 100, 0, 0, 0);

			gameDataBase.AddPlayer(p1);
			gameDataBase.AddPlayer(p2);

			Thread.Sleep(500);
			if (Server.GameDataBase.PlayerExists(p1.ToPlayer())) Console.WriteLine("ano jest");
			else Console.WriteLine("nima");

			Thread.Sleep(500);

			//create factions
			var f1 = new Faction(1, "test");
			var f2 = new Faction(2, "test2");
			gameDataBase.AddFaction(f1);
			gameDataBase.AddFaction(f2);
			var faction = gameDataBase.GetAllFactions().First();

			//create weapons and defences (independent of one another)
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

			//make lists of weapons and defences - NEW LIST FOR EACH SHIP
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
			//create ship template objects
			DbShipTemplate st1 = new DbShipTemplate(1, "s1", faction, 10, 10.0, 1000.0, 4.0, 54.0, weapons1, denences, 2000, Rarity.COMMON);
			DbShipTemplate st2 = new DbShipTemplate(2, "s2", faction, 20, 5.0, 500.0, 1.0, 23.0, weapons2, denences2, 1000, Rarity.VERY_RARE);
			gameDataBase.AddShipTemplate(st1);
			gameDataBase.AddShipTemplate(st2);

			Thread.Sleep(500);
			Console.WriteLine("teraz apdejt");
			DbShipTemplate sUp = new DbShipTemplate(1, "s1", faction, 99, 10.0, 9999.0, 4.0, 54.0, weapons3, denences, 2000, Rarity.RARE);
			gameDataBase.UpdateShipTemplate(sUp);

			Thread.Sleep(500);
			Console.WriteLine("i delete");
			gameDataBase.RemoveTemplateShipWithId(2);

			Thread.Sleep(500);
			Console.WriteLine("no i insert DbShip");
			var player = gameDataBase.GetPlayerWithUsername(p1Name);
			var shpT = gameDataBase.GetShipTemplateWithId(1);
			DbShip sh1 = new DbShip(1, player, 1000, shpT);
			gameDataBase.AddShip(sh1);

			Thread.Sleep(500);
			Console.WriteLine("select ships");
			var ships = gameDataBase.GetAllShips();
			foreach (DbShip ship in ships) Console.WriteLine(ship.ShipBaseStats.Name + " " + ship.Id + " " + ship.ShipBaseStats.Weapons[0].Name + " " + ship.ShipBaseStats.ShipRarity);

			Thread.Sleep(500);
			Console.WriteLine("no i 2 insert DbShip - przez generate");
			var player_ = gameDataBase.GetPlayerWithUsername(p1Name);
			var shpT_ = gameDataBase.GetShipTemplateWithId(1);
			DbShip sh2 = shpT_.GenerateNewShipOfThisTemplate(player_);
			gameDataBase.AddShip(sh2);

			Thread.Sleep(500);
			Console.WriteLine("select ships po raz 2");
			var ships2 = gameDataBase.GetAllShips();
			foreach (DbShip ship in ships2) Console.WriteLine(ship.ShipBaseStats.Name + " " + ship.Id + " " + ship.ShipBaseStats.Weapons[0].Name + " " + ship.ShipBaseStats.ShipRarity);
			//UWAGA NA UZYWANIE LIST!!! DLA KAZDEGO NOWEGO OBIEKTU ROB NOWA LISTE BO INACZEJ DOSTAJE DALNA I WYWALA CALA JOIN TABLE!!! 
			//REFERENCJE W TYCH LISTACH MOGA BYC TE SAME ALE INSTANCJE LIST MUSZA BYC ROZNE!!!
			
			Console.ReadKey();

			Environment.Exit(0);*/

			/*baseModifiers = GameDataBase.GetBaseModifiers();
			allFactions = GameDataBase.GetAllFactions();*/
		}
		//====================================================================================================================================================================================
		//====================================================================================================================================================================================
		//====================================================================================================================================================================================

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
		/// To be used instead of <see cref="Console.WriteLine"/>, prints message to chosen log - console, text area etc. Appends date at the beginning of message
		/// </summary>
		/// <param name="message"></param>
		internal static void Log(string message) {
			string msg = DateTime.Now + ": " + message;
			lock (logLock) {
				Console.WriteLine(msg);
			}
		}

		/// <summary>
		/// To be used instead of <see cref="Console.Write"/>, prints message without new line to chosen log - console, text area etc.
		/// </summary>
		/// <param name="message"></param>
		internal static void LogNoNewLine(string message) {
			lock (logLock) {
				Console.Write(message);
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

				/*Console.WriteLine("Trying to receive with timeout...");
				try {
					client.GetReceivedPacket(2000);
				} catch (ReceiveTimeoutException e) {
					Console.WriteLine("Failed to receive with timeout. Disconnected player number: " + e.PlayerNumber);
				}*/

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

		private static void DoGameBoardValidationTest() {
			//game board test
			baseModifiers = (new DbBaseModifiers() {
				Id = 1,
				KineticRange = 1,           //each of range mults has to be from 0.0 to 1.0
				LaserRange = 0.5,           //they indicate how important is range for given WeaponType to hit its target, look in Game.cs at chanceToHit calculations for more
				MissileRange = 0.0,         //missile are invulnerable to range

				KineticPD = 2,
				KineticShield = 5,
				KineticIF = 1.2,
				LaserPD = 0,
				LaserShield = 0.5,
				LaserIF = 2,
				MissilePD = 8,
				MissileShield = 1,
				MissileIF = 1.2,

				BaseShipStatsExpModifier = 0.01,
				MaxShipsPerPlayer = 150,
				StartingMoney = 1000,
				ExpForVictory = 20,
				ExpForLoss = 10,
				FleetSizeExpModifier = 0.5,
				BaseFleetMaxSize = 1000,
				MaxAbsoluteFleetSize = 5000,
				MaxShipExp = 1000,
				MaxShipsInLine = 5,
				MaxFleetsPerPlayer = 8,
				MoneyForVictory = 80,
				MoneyForLoss = 40
			}).ToBaseModifiers();

			Weapon chaff = new Weapon(1, "chaff", new Faction(), 5, 100, WeaponType.KINETIC, 3, 0.9, 0.3);
			Weapon oneHitKiller = new Weapon(2, "one hit killer", new Faction(), 50000, 1, WeaponType.LASER, 10000, 0, 1);
			DefenceSystem d1 = new DefenceSystem(1, "d1", new Faction(), 10, DefenceSystemType.POINT_DEFENCE, 2, 0, 2);
			DefenceSystem superDef = new DefenceSystem(2, "superDef", new Faction(), 4000, DefenceSystemType.SHIELD, 2, 2, 2);
			List<Weapon> weakWeapons = new List<Weapon> { chaff };
			List<Weapon> allWeapons = new List<Weapon> { chaff, oneHitKiller };
			List<DefenceSystem> weakDef = new List<DefenceSystem> { d1 };
			List<DefenceSystem> allDefs = new List<DefenceSystem> { d1, superDef };

			Ship p1s1 = new Ship() { Id = 1, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s2 = new Ship() { Id = 2, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s3 = new Ship() { Id = 3, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s4 = new Ship() { Id = 4, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s5 = new Ship() { Id = 5, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s6 = new Ship() { Id = 6, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s7 = new Ship() { Id = 7, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p1s8 = new Ship() { Id = 8, Evasion = 0.1, Armor = 200, Hp = 1500, Size = 9, Defences = CloneList(allDefs), Weapons = CloneList(allWeapons) };

			Ship p2s1 = new Ship() { Id = 11, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s2 = new Ship() { Id = 12, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s3 = new Ship() { Id = 13, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s4 = new Ship() { Id = 14, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s5 = new Ship() { Id = 15, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s6 = new Ship() { Id = 16, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s7 = new Ship() { Id = 17, Evasion = 0.5, Armor = 20, Hp = 100, Size = 2, Defences = CloneList(weakDef), Weapons = CloneList(weakWeapons) };
			Ship p2s8 = new Ship() { Id = 18, Evasion = 0.1, Armor = 800, Hp = 500, Size = 8, Defences = CloneList(allDefs), Weapons = CloneList(allWeapons) };

			List<Ship> p1s = new List<Ship> { p1s1, p1s2, p1s3, p1s4, };
			List<Ship> p1m = new List<Ship> { p1s6, p1s7, p1s5 };
			List<Ship> p1l = new List<Ship> { p1s8 };
			List<Ship> p1all = new List<Ship> { p1s1, p1s2, p1s3, p1s4, p1s5, p1s6, p1s7, p1s8 };
			Fleet p1fleet = new Fleet("p1fleet", new Player(), p1all);
			PlayerGameBoard p1gameBoard = new PlayerGameBoard(p1s, p1m, p1l);

			List<Ship> p2s = new List<Ship> { p2s1, p2s2 };
			List<Ship> p2m = new List<Ship> { p2s3, p2s4, p2s5, p2s8 };
			List<Ship> p2l = new List<Ship> { p2s6, p2s7 };
			List<Ship> p2all = new List<Ship> { p2s1, p2s2, p2s3, p2s4, p2s5, p2s6, p2s7, p2s8 };
			Fleet p2fleet = new Fleet("p2fleet", new Player(), p2all);
			PlayerGameBoard p2gameBoard = new PlayerGameBoard(p2s, p2m, p2l);

			string p1validate = GameValidator.ValidatePlayerBoard(p1gameBoard, p1fleet);
			string p2validate = GameValidator.ValidatePlayerBoard(p2gameBoard, p2fleet);
			Console.WriteLine("p1 board validate: " + p1validate);
			Console.WriteLine("p2 board validate: " + p2validate);

			PrintGameBoards(p1gameBoard, p2gameBoard);

			Tuple<ShipPosition, Line> p1mm1 = new Tuple<ShipPosition, Line>(new ShipPosition(Line.SHORT, 0), Line.MEDIUM);
			Tuple<ShipPosition, Line> p1mm2 = new Tuple<ShipPosition, Line>(new ShipPosition(Line.MEDIUM, 0), Line.SHORT);
			Tuple<ShipPosition, ShipPosition> p1ma1 = new Tuple<ShipPosition, ShipPosition>(new ShipPosition(Line.SHORT, 1), new ShipPosition(Line.SHORT, 1));
			Tuple<ShipPosition, ShipPosition> p1ma2 = new Tuple<ShipPosition, ShipPosition>(new ShipPosition(Line.LONG, 0), new ShipPosition(Line.SHORT, 0));	//destroy this ship
			Move p1move = new Move();
			p1move.MoveList.Add(p1mm1);
			p1move.MoveList.Add(p1mm2);
			p1move.AttackList.Add(p1ma1);
			p1move.AttackList.Add(p1ma2);

			p1validate = GameValidator.ValidateMove(p1move, p1gameBoard, p2gameBoard);
			Console.WriteLine("p1 move validate: " + p1validate);
			Console.WriteLine("press any key to contine with game test");
			Console.ReadKey();

			Console.WriteLine("======== Game test =========");
			Game game = new Game(p1gameBoard, p2gameBoard);
			game.SetShipStatesForPlayer1(p1move);
			game.SetShipStatesForPlayer2(game.EmptyMove);
			game.ProcessPlayer1MoveOrders(p1move);
			game.ProcessPlayer1AttackMove(p1move);
			game.FinalizeMove();

			PrintGameBoards(p1gameBoard, p2gameBoard);

			Console.ReadKey();

			Environment.Exit(0);
		}

		private static List<T> CloneList<T>(List<T> src) {
			List<T> ret = new List<T>();
			foreach (var def in src) ret.Add(CloneObject(def));
			return ret;
		}

		private static void PrintGameBoards(PlayerGameBoard p1gameBoard, PlayerGameBoard p2gameBoard) {
			Console.WriteLine("p1 game board");
			foreach (var line in p1gameBoard.Board) {
				Console.Write(line.Key + " :   \t");
				foreach (Ship s in line.Value) Console.Write(s.Id + "\t");
				Console.Write(Environment.NewLine);
			}
			Console.WriteLine("p2 game board");
			foreach (var line in p2gameBoard.Board) {
				Console.Write(line.Key + " :   \t");
				foreach (Ship s in line.Value) Console.Write(s.Id + "\t");
				Console.Write(Environment.NewLine);
			}
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
		private object loopLock = new object();
		private object userLock = new object();
		private Fleet selectedFleetForGame = null;

		internal UserThread(TcpConnection client) {
			client.ConnectionEnded += UserDisconnectedHandler;
			this.Client = client;
			this.GameDataBase = Server.GetGameDBContext();
			this.gameRNG = new GameRNG();
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
				while (!loginSuccess) {		//while login is not succesful
					LoginResult loginResult = LoginOrRegister();
					if (loginResult == LoginResult.LOGIN_SUCCESS) loginSuccess = true;
				}
				if (loginSuccess) {
					//set and send user data
					this.User = GameDataBase.GetPlayerWithUsername(this.User.Username).ToPlayer();
					this.User.Password = "";
					Client.Send(new GamePacket(OperationType.PLAYER_DATA, this.User));
					Client.Send(new GamePacket(OperationType.BASE_MODIFIERS, Server.BaseModifiers));

					UserProcessing();   //main logic
				}
				else EndThread();
			} catch(ConnectionEndedException) {
				Server.Log(FailureReasons.CLIENT_DISCONNECTED, true);
				EndThread();
			}
		}

		private enum LoginResult {
			LOGIN_SUCCESS,
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
					if (GameDataBase.PlayerExists(playerObject) && GameDataBase.ValidateUser(playerObject)) {
						this.User = playerObject;
						Server.Log("Succesfully logged in player: " + playerObject.Username);
						SendSuccess();
						return LoginResult.LOGIN_SUCCESS;
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
		private void UserProcessing() {
			//bool clientConnected = true;
			ClientConnected = true;
			DbPlayer thisUser;
			string validationResult;
			mainLoop: while (ClientConnected) {
				GamePacket gamePacket = Client.GetReceivedPacket();
				try {
					gameSwitch: switch (gamePacket.OperationType) {
						//====================================================== SHOP =====================================================================================================
						case OperationType.GET_LOOTBOXES:       //dont care about internal packet		//OK
							Server.Log(User.Username + ": wants to view lootboxes");
							List<DbLootBox> dbLootBoxes = GameDataBase.GetAllLootBoxes();
							List<LootBox> lootBoxes = dbLootBoxes.Select(x => x.ToLootBox()).ToList();
							Client.Send(new GamePacket(OperationType.GET_LOOTBOXES, lootBoxes));
							break;
						case OperationType.BUY:                     //OK
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
						case OperationType.SELL_SHIP:                   //OK
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
						case OperationType.VIEW_FLEETS:         //dont care about internal packet		//OK
							Server.Log(User.Username + ": wants to view his fleets");
							List<DbFleet> userDbFleets = GameDataBase.GetAllFleetsOfPlayer(User);
							List<Fleet> userFleets = userDbFleets.Select(x => x.ToFleet()).ToList();
							Client.Send(new GamePacket(OperationType.VIEW_FLEETS, userFleets));
							break;
						case OperationType.VIEW_ALL_PLAYER_SHIPS:       //dont care about internal packet
							Server.Log(User.Username + ": wants to view his ships");
							List<DbShip> userDbShips = GameDataBase.GetPlayersShips(User);
							//List<DbShip> userDbShips = GetPlayersShips(User, ((MySqlDataBase)Server.GameDataBase).DbContext);
							List<Ship> userShips = userDbShips.Select(x => x.ToShip()).ToList();
							Client.Send(new GamePacket(OperationType.VIEW_ALL_PLAYER_SHIPS, userShips));
							break;
						case OperationType.ADD_FLEET:           //OK
							Fleet fleetToAdd = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(fleetToAdd.Name);
							Server.Log(User.Username + ": wants to add a new fleet");
							validationResult = GameValidator.ValidateFleet(User, fleetToAdd, GameDataBase, true);
							if (validationResult == GameValidator.OK) {  //fleet is ok
								GameDataBase.AddFleet(fleetToAdd, User);
								SendSuccess();
							}
							else {
								SendFailure(validationResult);
							}
							break;
						case OperationType.UPDATE_FLEET:		//OK
							Fleet fleetToUpdate = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + ": wants to modify fleet with id " + fleetToUpdate.Id);
							validationResult = GameValidator.ValidateFleet(User, fleetToUpdate, GameDataBase, false);
							if (validationResult == GameValidator.OK) {  //fleet is ok
								GameDataBase.UpdateFleet(GameDataBase.ConvertFleetToDbFleet(fleetToUpdate, User, false));
								SendSuccess();
							}
							else {
								SendFailure(validationResult);
							}
							break;
						case OperationType.DELETE_FLEET:		//OK
							Fleet fleetToDelete = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + ": wants to delete fleet with id " + fleetToDelete.Id);
							if (GameDataBase.RemoveFleetWithId(fleetToDelete.Id, false, User.Id)) SendSuccess();
							else SendFailure(FailureReasons.INVALID_ID);
							break;
						//====================================================== PLAYER STATS =====================================================================================================
						case OperationType.GET_PLAYER_STATS:		//OK
							Server.Log(User.Username + " wants to view game history");
							List<DbGameHistory> dbGameHistory = GameDataBase.GetPlayersGameHistory(User.Id);
							List<GameHistory> gameHistory = dbGameHistory.Select(x => x.ToGameHistory(false)).ToList();
							Client.Send(new GamePacket(OperationType.GET_PLAYER_STATS, gameHistory));
							break;
						case OperationType.GET_PLAYER_STATS_ENTRY:		//OK
							GameHistory entry = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							Server.Log(User.Username + " wants to view details of game history entry with ID: " + entry.Id);
							DbGameHistory dbEntry = GameDataBase.GetGameHistoryEntry(entry.Id);
							entry = dbEntry.ToGameHistory(true);
							Client.Send(new GamePacket(OperationType.GET_PLAYER_STATS_ENTRY, entry));
							break;
						//====================================================== PLAYER STATS =====================================================================================================
						case OperationType.DISCONNECT:          //OK
							Server.Log(User.Username + ": wants to disconnect");
							ClientConnected = false;
							break;
						//====================================================== SELECT FLEET FOR GAME =====================================================================================================
						case OperationType.SELECT_FLEET:        //TODO NOT TESTED
							Server.Log(User.Username + ": wants to select fleet used for next game");
							Fleet selectedFleet = Server.CastPacketToProperType(gamePacket.Packet, OperationsMap.OperationMapping[gamePacket.OperationType]);
							DbFleet fleetForGame = GameDataBase.GetFleetWithId(selectedFleet.Id);
							if (fleetForGame.Owner.Id != User.Id) SendFailure(FailureReasons.INVALID_ID);
							else {
								SelectedFleetForGame = fleetForGame.ToFleet();
								SendSuccess();
							}
							break;									
						//====================================================== CUSTOM GAME =====================================================================================================
						case OperationType.GET_CUSTOM_ROOMS:		//TODO NOT TESTED
							Server.Log(User.Username + ": wants to get list od custom rooms");
							List<CustomGameRoom> rooms = Server.GetAvailableCustomRooms();
							Client.Send(new GamePacket(OperationType.GET_CUSTOM_ROOMS, rooms));
							break;
						case OperationType.PLAY_CUSTOM_CREATE:      //TODO NOT TESTED
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
								customGameRoomToCreate.gameEnded.WaitOne();     //block until end of the game
								Server.Log(User.Username + ": has ended his game, user thread continues");
								UnsetSelectedFleetAfterGame();
							}
							break;
						case OperationType.PLAY_CUSTOM_JOIN:        //TODO NOT TESTED
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
									bool joinSuccess = customGameRoomToJoin.JoinThisRoom(Client, User, GameDataBase, this);
									if (joinSuccess) {
										SendSuccess();
										Server.Log(User.Username + ": join game room: " + roomToJoin.RoomName + "successful, user thread blocks");
										customGameRoomToJoin.gameEnded.WaitOne();   //block until end of the game
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
						case OperationType.PLAY_RANKED:     //TODO NOT TESTED
							if (SelectedFleetForGame == null) SendFailure(FailureReasons.NO_FLEET_SELECTED);
							else {
								Server.Log(User.Username + ": wants to play ranked game");
								GameRoomThread rankedGame = Server.JoinBestGameRoomForPlayer(User);
								if (rankedGame == null) {           //there are no rooms - need to create new one
									GameRoomThread newRankedRoom = new GameRoomThread(Client, User, GameDataBase, false, this);
									Thread newRankedGameThread = new Thread(new ThreadStart(newRankedRoom.RunGameThread));
									newRankedGameThread.Start();
									//newCustomGameThread.Join();
									Server.Log(User.Username + ": create ranked game room successful, user thread blocks");
									newRankedRoom.gameEnded.WaitOne();     //block until end of the game
									Server.Log(User.Username + ": has ended his game, user thread continues");
									UnsetSelectedFleetAfterGame();
								}
								else {      //join successful - GameRoomThread removed from available and JoinThisThread can be called
									bool joinSuccess = rankedGame.JoinThisRoom(Client, User, GameDataBase, this);
									if (joinSuccess) {
										SendSuccess();
										Server.Log(User.Username + ": join game room successful, user thread blocks");
										rankedGame.gameEnded.WaitOne();   //block until end of the game
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
						default:		//OK
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
			}
			//finally, after the loop end thread
			Server.Log(User.Username + ": Thread Ending");
			this.EndThread();
		}
		#endregion

		#region user thread utils
		private void UserDisconnectedHandler(object sender, GameEventArgs e) {
			Server.Log(User.Username + ": sudden disconnection (disconnect event received) - ending user thread");
			ClientConnected = false;
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
			Server.Log(User.Username + " ending connections");
			this.GameDataBase.Dispose();
			this.Client.Disconnect();
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
		public readonly string YOU_WON_MESSAGE;// = "You won!";
		public readonly string YOU_LOST_MESSAGE;// = "You lost!";
		public readonly string DRAW_MESSAGE;// = "Draw!";
		public static readonly int SETUP_FLEET_TIMEOUT = 120000;    //in miliseconds
		public static readonly int MAKE_MOVE_TIMEOUT = 60000;	//in miliseconds

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
							Player1GameBoard = Server.CastPacketToProperType(player1Packet.Packet, OperationsMap.OperationMapping[player1Packet.OperationType]);
							validateResult = GameValidator.ValidatePlayerBoard(Player1GameBoard, Player1Fleet);
							if (validateResult != GameValidator.OK) {
								EndGameOnError(1, validateResult);
							}
							else SendSuccess(Player1Conn);	//if setup OK send success
						}
						catch (InvalidCastException) {
							EndGameOnError(1, FailureReasons.INVALID_PACKET);
						}
					}
					else EndGameOnError(1, FailureReasons.RECEIVE_TIMEOUT);
					if (player2Packet != null) {
						try {
							Player2GameBoard = Server.CastPacketToProperType(player2Packet.Packet, OperationsMap.OperationMapping[player2Packet.OperationType]);
							validateResult = GameValidator.ValidatePlayerBoard(Player2GameBoard, Player2Fleet);
							if (validateResult != GameValidator.OK) {
								EndGameOnError(2, validateResult);
							}
							else SendSuccess(Player2Conn);  //if setup OK send success
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
						while (ContinueGameLoop) {
							//lock (gameInProgressLock) {	//cant do this with await
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
									player1Move = Server.CastPacketToProperType(player1Packet.Packet, OperationsMap.OperationMapping[player1Packet.OperationType]);
									validateResult = GameValidator.ValidateMove(player1Move, ThisGame.Player1GameBoard, ThisGame.Player2GameBoard);
									if (validateResult != GameValidator.OK) {
										SkipMove(Player1Conn, "invalid move: " + validateResult);
									}
									else SendSuccess(Player1Conn);  //if move OK send success
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
									player2Move = Server.CastPacketToProperType(player2Packet.Packet, OperationsMap.OperationMapping[player2Packet.OperationType]);
									validateResult = GameValidator.ValidateMove(player2Move, ThisGame.Player2GameBoard, ThisGame.Player1GameBoard);
									if (validateResult != GameValidator.OK) {
										SkipMove(Player2Conn, "invalid move: " + validateResult);
									}
									else SendSuccess(Player2Conn);  //if move OK send success
								}
								catch (InvalidCastException) {
									SkipMove(Player2Conn, FailureReasons.INVALID_PACKET);
								}
							}
							else {
								SkipMove(Player2Conn, "make move timeout");
							}

							ThisGame.MakeTurn(player1Move, player1MoveOK, player2Move, player2MoveOK);
							/*
							//set ship states according to moves
							if (player1MoveOK) ThisGame.SetShipStatesForPlayer1(player1Move);
							else ThisGame.SetShipStatesForPlayer1(ThisGame.EmptyMove);
							if (player2MoveOK) ThisGame.SetShipStatesForPlayer2(player2Move);
							else ThisGame.SetShipStatesForPlayer2(ThisGame.EmptyMove);

							//first process attack orders
							if (player1MoveOK) ThisGame.ProcessPlayer1AttackMove(player1Move);
							if (player2MoveOK) ThisGame.ProcessPlayer2AttackMove(player2Move);

							//first process move orders - move the surviving ships
							if (player1MoveOK) ThisGame.ProcessPlayer1MoveOrders(player1Move);
							if (player2MoveOK) ThisGame.ProcessPlayer2MoveOrders(player2Move);

							//finalize move by updating the PlayerGameBoards in ThisGame
							ThisGame.FinalizeMove();
							*/

							//check game result and possibly end the game
							Victory gameResultAfterThisTurn = ThisGame.CheckGameEndResult();
							if(gameResultAfterThisTurn == Victory.PLAYER_1) {
								UpdateLoserAndWiner(Player2, Player1);
								Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(true, YOU_WON_MESSAGE)));
								Player2Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, YOU_LOST_MESSAGE)));
								SetGameEnded();
							}
							else if(gameResultAfterThisTurn == Victory.PLAYER_2) {
								UpdateLoserAndWiner(Player1, Player2);
								Player2Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(true, YOU_WON_MESSAGE)));
								Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, YOU_LOST_MESSAGE)));
								SetGameEnded();
							}
							else if(gameResultAfterThisTurn == Victory.DRAW) {
								UpdateDraw();
								Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, DRAW_MESSAGE)));
								Player2Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(false, DRAW_MESSAGE)));
								SetGameEnded();
							}
							//else if Victory.NOT_YET and game continues

							//na poczatku dla ruchu statku zrobic tymczasowa liste par <statek,linia_docelowa> zeby nie bylo problemow z indexami i nullami
							//przed przetwarzaniem zrobic refernecyjna liste tych par i na jej podstawie brac statki i wstawiac na nowy PlayerGameBoard
							//docelowo obiekty zwracane do klienta to te property w tym obiekcie, ta lista par tylko dla referencji
							//klient moze wrzuca null na miejsce startowe ruchu zeby sie nie pogubil

							//}
						}
					}

					Server.Log(UsernamesOfPlayers + ": game finished, ending game room...");
					if(!GameAlreadyEnded) EndGameThread();
				}
			}
			catch (ConnectionEndedException ex) {
				Server.Log(UsernamesOfPlayers + ": player with number " + ex.PlayerNumber + " suddenly disconnected");
				EndGameBeforeProperEnd(ex.PlayerNumber, SUDDEN_DISCONNECT_LOG_STRING);
			}
		}
		#endregion

		/// <summary>
		/// async method to get players packet in parallel with second call of this method
		/// </summary>
		/// <param name="playerConection"></param>
		/// <returns></returns>
		private async Task<GamePacket> GetPlayersPacket(TcpConnection playerConection, int timeout) {
			Task<GamePacket> receivedPacket = Task.Run(() => playerConection.GetReceivedPacket(timeout));
			return await receivedPacket;
		}

		private void SkipMove(TcpConnection playerConnThatTiemouted, string reason) {
			SendFailure(playerConnThatTiemouted, reason);
			if (playerConnThatTiemouted.PlayerNumber == 1) player1MoveOK = false;
			else if (playerConnThatTiemouted.PlayerNumber == 2) player2MoveOK = false;
			else Server.Log(UsernamesOfPlayers + ": Invalid player number in skip move handler! Actual: " + playerConnThatTiemouted.PlayerNumber + ", should be 1 or 2", true);
		}

		#region game room utils
		internal bool JoinThisRoom(TcpConnection joinerConnection, Player joiner, IGameDataBase player2DB, UserThread player2ThreadObj) {
			lock (joinLock) {
				if (!IsFull) {
					Player2 = joiner;
					Player2Conn = joinerConnection;
					Player2Conn.PlayerNumber = 2;
					Player2DB = player2DB;
					Player2ThreadObj = player2ThreadObj;
					Player2Fleet = player2ThreadObj.SelectedFleetForGame;
					IsFull = true;
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
			Server.Log(UsernamesOfPlayers + ": Game ended - result: winner " + winner.Username + " loser: " + loser.Username);

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
			EndGameBeforeProperEnd(e.PlayerNumber, SUDDEN_DISCONNECT_LOG_STRING);
		}

		private void SurrenderHandler(object sender, GameEventArgs e) {
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
							HandleSuddenGameEnd(Player1, SUDDEN_DISCONNECT_LOG_STRING);
						}
						else if (numberOfPlayerThatEndedGame == 2) {
							Player1Conn.Send(new GamePacket(OperationType.GAME_END, new GameResult(true, reasonToLog)));
							Player2ThreadObj.ClientConnected = false;
							HandleSuddenGameEnd(Player2, SUDDEN_DISCONNECT_LOG_STRING);
						}
						else Server.Log(UsernamesOfPlayers + ": Invalid player number in sudden game end handler! Actual: " + numberOfPlayerThatEndedGame + ", should be 1 or 2", true);
					}
					else Server.Log(UsernamesOfPlayers + ": Sudden game begore room was full");
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
				ClearPlayerNumberInConnection(Player2Conn);
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
