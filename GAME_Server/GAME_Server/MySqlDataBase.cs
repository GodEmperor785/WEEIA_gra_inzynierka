using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	public class MySqlDataBase : IGameDataBase, IDisposable {
		#region fields, constructors and properties
		private GameDBContext dbContext;
		private Random rng;

		#region basic queries
		private IEnumerable<DbShipTemplate> BasicShipTemplateQuery { 
			get {
				return from shipTemplates in DbContext.ShipTemplates select shipTemplates;
			}
		}

		private IEnumerable<DbShip> BasicShipQuery {
			get {
				return from ships in DbContext.Ships select ships;
			}
		}

		private IEnumerable<DbPlayer> BasicPlayerQuery {
			get {
				return from players in DbContext.Players select players;
			}
		}

		private IEnumerable<DbFleet> BasicFleetQuery {
			get {
				return from fleets in DbContext.Fleets select fleets;
			}
		}

		private IEnumerable<DbWeapon> BasicWeaponQuery {
			get {
				return from weapons in DbContext.Weapons select weapons;
			}
		}

		private IEnumerable<DbDefenceSystem> BasicDefenceSystemQuery {
			get {
				return from defences in DbContext.DefenceSystems select defences;
			}
		}

		private IEnumerable<DbLootBox> BasicLootBoxQuery {
			get {
				return from lootboxes in DbContext.LootBoxes select lootboxes;
			}
		}
		#endregion

		internal MySqlDataBase() {
			dbContext = new GameDBContext();
			rng = new Random();
		}

		internal GameDBContext DbContext { get => dbContext; }
		internal Random DbRNG { get => rng; }
		#endregion

		private int SaveChanges() {
			return DbContext.SaveChanges();
		}

		#region INSERT
		/// <summary>
		/// adds a new player to database. Players list of ships is initially empty!
		/// </summary>
		/// <param name="player"></param>
		public void AddPlayer(DbPlayer player) {
			DbContext.Players.Add(player);
			SaveChanges();
		}

		public void AddShip(DbShip ship) {
			//DbShip newShip = new DbShip(ship);

			//foreach (DbWeapon weapon in newShip.Weapons) weapon.Ships.Add(newShip);
			//foreach (DbDefenceSystem defence in newShip.Defences) defence.Ships.Add(newShip);

			DbContext.Ships.Add(ship);
			SaveChanges();
		}

		public void AddShipTemplate(DbShipTemplate shipTemplate) {
			DbContext.ShipTemplates.Add(shipTemplate);
			SaveChanges();
		}

		public void AddFaction(Faction faction) {
			DbContext.Factions.Add(faction);
			SaveChanges();
		}

		public void AddWeapon(DbWeapon weapon) {
			DbContext.Weapons.Add(weapon);
			SaveChanges();
		}

		public void AddDefenceSystem(DbDefenceSystem defence) {
			DbContext.DefenceSystems.Add(defence);
			SaveChanges();
		}

		/// <summary>
		/// adds fleet to Db, does NOT validate the fleet, this should be done earlier (including <see cref="FleetNameIsUnique"/>, <see cref="FleetPointsInRange"/> and <see cref="FleetShipsExpRequirement"/>)
		/// </summary>
		/// <param name="fleet"></param>
		public void AddFleet(Fleet fleet) {                                                 //NOT TESTED
			List<int> shipIds = fleet.Ships.Select(s => s.Id).ToList();
			List<DbShip> shipsForFleet = this.GetShips(shipIds);
			DbPlayer owner = this.GetPlayerWithUsername(fleet.Owner.Username);
			DbFleet newFleet = new DbFleet(owner, shipsForFleet, fleet.Name);
			DbContext.Fleets.Add(newFleet);
			SaveChanges();
		}

		public void AddBaseModifiers(DbBaseModifiers mods) {
			DbContext.BaseModifiers.Add(mods);
			SaveChanges();
		}

		public void AddLootBox(DbLootBox lootbox) {
			DbContext.LootBoxes.Add(lootbox);
			SaveChanges();
		}

		public void AddGameHistory(DbGameHistory entry) {
			DbContext.GameHistories.Add(entry);
			SaveChanges();
		}

		#endregion

		#region SELECT
		public List<Faction> GetAllFactions() {
			var query = from factions in DbContext.Factions
						select factions;
			return query.ToList();
		}

		public List<DbFleet> GetAllFleetsOfPlayer(Player player) {
			/*var query = from fleets in DbContext.Fleets
						where fleets.Owner.Id == player.Id
						select fleets;*/
			var query = BasicFleetQuery.Where(fleet => fleet.Owner.Id == player.Id);
			return query.ToList();
		}

		public List<DbShip> GetAllShips() {
			/*var query = from dbShips in DbContext.Ships
						select dbShips;*/
			return BasicShipQuery.ToList();
		}

		public BaseModifiers GetBaseModifiers() {
			var query = from baseMods in DbContext.BaseModifiers
						where baseMods.Id == 1
						select baseMods;
			var baseModifiers = query.FirstOrDefault();
			return baseModifiers.ToBaseModifiers();
		}

		public DbFleet GetFleetWithId(int id) {
			/*var query = from fleets in DbContext.Fleets
						where fleets.Id == id
						select fleets;*/
			var query = BasicFleetQuery.Where(fleet => fleet.Id == id);
			return query.FirstOrDefault();
		}

		/// <summary>
		/// gets player with specified username. Checks only first selected player because player username is unique
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public DbPlayer GetPlayerWithUsername(string username) {
			/*var query = from players in DbContext.Players
						where players.Username == username
						select players;*/
			var query = BasicPlayerQuery.Where(player => player.Username == username);
			return query.FirstOrDefault();	//player username is unique
		}

		public DbShip GetShipWithId(int id) {
			/*var query = from ships in DbContext.Ships
						where ships.Id == id
						select ships;*/
			var query = BasicShipQuery.Where(ship => ship.Id == id);
			return query.FirstOrDefault();
		}

		public DbShipTemplate GetShipTemplateWithId(int id) {
			/*var query = from shipTemplates in DbContext.ShipTemplates
						where shipTemplates.Id == id
						select shipTemplates;*/
			var query = BasicShipTemplateQuery.Where(shipTempl => shipTempl.Id == id);
			return query.FirstOrDefault();
		}

		public List<DbWeapon> GetAllWeapons() {
			/*var query = from weapons in DbContext.Weapons
						select weapons;*/
			return BasicWeaponQuery.ToList();
		}

		public List<DbDefenceSystem> GetAllDefences() {
			/*var query = from defences in DbContext.DefenceSystems
						select defences;*/
			return BasicDefenceSystemQuery.ToList();
		}

		/// <summary>
		/// gets DbWeapons with ids
		/// </summary>
		/// <param name="weps"></param>
		/// <returns></returns>
		public List<DbWeapon> GetDbWeapons(List<int> weps) {
			/*var query = from weapons in DbContext.Weapons
						where weps.Contains(weapons.Id)
						select weapons;*/
			var query = BasicWeaponQuery.Where(weapons => weps.Contains(weapons.Id));
			return query.ToList();
		}

		/// <summary>
		/// gets DbDefenceSystems with ids
		/// </summary>
		/// <param name="defs"></param>
		/// <returns></returns>
		public List<DbDefenceSystem> GetDbDefenceSystems(List<int> defs) {
			/*var query = from defences in DbContext.DefenceSystems
						where defs.Contains(defences.Id)
						select defences;*/
			var query = BasicDefenceSystemQuery.Where(defences => defs.Contains(defences.Id));
			return query.ToList();
		}

		/// <summary>
		/// gets ships which ids are in shipIds
		/// </summary>
		/// <param name="shipIds"></param>
		/// <returns></returns>
		public List<DbShip> GetShips(List<int> shipIds) {
			/*var query = from ships in DbContext.Ships
						where shipIds.Contains(ships.Id)
						select ships;*/
			var query = BasicShipQuery.Where(ships => shipIds.Contains(ships.Id));
			return query.ToList();
		}

		/// <summary>
		/// returns ships templates that have exp requirement equal or lower than specified in parameter
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public List<DbShipTemplate> GetShipsAvailableForExp(int exp) {
			/*var query = from ships in DbContext.ShipTemplates
						where ships.ExpUnlock <= exp
						select ships;*/
			var query = BasicShipTemplateQuery.Where(shipTempl => shipTempl.ExpUnlock <= exp);
			return query.ToList();
		}

		public List<DbShipTemplate> GetShipTemplatesWithRarityAndReqExp(Rarity rarity, int reqExp) {
			var query = BasicShipTemplateQuery.Where(shipTempl => ((shipTempl.ExpUnlock <= reqExp) && (shipTempl.ShipRarity == rarity)) );
			return query.ToList();
		}

		public int GetPlayerShipCount(Player player) {
			return GetPlayerWithUsername(player.Username).OwnedShips.Count();
		}

		/// <summary>
		/// gets random ship of selected rarity and proper exp requirement
		/// </summary>
		/// <param name="rarity"></param>
		/// <param name="expReq"></param>
		/// <returns></returns>
		public DbShipTemplate GetRandomShipTemplateOfRarity(Rarity rarity, int expReq) {
			/*var query = BasicShipTemplateQuery.Where(shipTempl => ( (shipTempl.ShipRarity == rarity) && (shipTempl.ExpUnlock <= expReq) ));
			int shipsOfSelectedRarityCount = query.Count();
			return query.ElementAt(DbRNG.Next(0, shipsOfSelectedRarityCount));*/
			List<DbShipTemplate> availableShips = GetShipTemplatesWithRarityAndReqExp(rarity, expReq);
			return availableShips.ElementAt(DbRNG.Next(0, availableShips.Count));
		}

		public List<DbLootBox> GetAllLootBoxes() {
			return BasicLootBoxQuery.ToList();
		}

		public DbLootBox GetLootBoxWithId(int id) {
			var query = BasicLootBoxQuery.Where(lootbox => lootbox.Id == id);
			return query.FirstOrDefault();
		}

		public List<DbGameHistory> GetPlayersGameHistory(int playerId) {
			var query = from history in DbContext.GameHistories
						where (history.Winner.Id == playerId || history.Loser.Id == playerId)
						select history;
			return query.ToList();
		}

		public List<DbPlayer> GetAllPlayers() {
			return BasicPlayerQuery.ToList();
		}

		public List<DbShipTemplate> GetAllShipTemplates() {
			return BasicShipTemplateQuery.ToList();
		}

		#endregion

		#region UPDATE
		/// <summary>
		/// updates ship template with new data, weapon and defence systems lists in newData must be got from DB! Make sure you have a previously unused list from DB before you update with it
		/// </summary>
		/// <param name="newData"></param>
		public void UpdateShipTemplate(DbShipTemplate newData) {
			var shipToUpdate = GetShipTemplateWithId(newData.Id);
			//update should be done like this
			shipToUpdate.Name = newData.Name;
			shipToUpdate.Faction = newData.Faction;
			shipToUpdate.Cost = newData.Cost;
			shipToUpdate.Evasion = newData.Evasion;
			shipToUpdate.Hp = newData.Hp;
			shipToUpdate.Size = newData.Size;
			shipToUpdate.Armor = newData.Armor;
			shipToUpdate.ExpUnlock = newData.ExpUnlock;
			shipToUpdate.Weapons = newData.Weapons;
			shipToUpdate.Defences = newData.Defences;
			shipToUpdate.ShipRarity = newData.ShipRarity;
			SaveChanges();
		}

		public void UpdatePlayer(DbPlayer newData) {
			var userToUpdate = GetPlayerWithUsername(newData.Username);
			userToUpdate.Password = newData.Password;
			userToUpdate.MaxFleetPoints = newData.MaxFleetPoints;
			userToUpdate.Experience = newData.Experience;
			userToUpdate.GamesPlayed = newData.GamesPlayed;
			userToUpdate.GamesWon = newData.GamesWon;
			userToUpdate.Money = newData.Money;
			//userToUpdate.OwnedShips = newData.OwnedShips;
			SaveChanges();
		}
		#endregion

		#region DELETE
		/// <summary>
		/// deletes ship with specified id, can't delete ship that belongs to any player or to any fleet. returns true if delete completed, false if at least one ship of this template exists
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool RemoveTemplateShipWithId(int id) {
			var shipToDelete = GetShipTemplateWithId(id);
			if (shipToDelete.ShipsOfThisTemplate.Count > 0) return false;

			//shipToDelete.Weapons.Clear();
			//shipToDelete.Defences.Clear();
			//SaveChanges();
			DbContext.ShipTemplates.Remove(shipToDelete);
			SaveChanges();
			return true;
		}

		/// <summary>
		/// for admin call like: RemoveShipWithId(shipId, true), player cant remove ship he does not own
		/// </summary>
		/// <param name="id"></param>
		/// <param name="playerId"></param>
		/// <param name="isAdmin"></param>
		/// <returns></returns>
		public bool RemoveShipWithId(int id, bool isAdmin, int playerId = 0) {
			var shipToDelete = GetShipWithId(id);
			if(!isAdmin && shipToDelete.Owner.Id != playerId) return false; //user cant delete ship he does not own!

			DbContext.Ships.Remove(shipToDelete);
			SaveChanges();
			return true;
		}
		#endregion

		#region Checks
		public bool PlayerExists(Player player) {
			return DbContext.Players.Any(dbPlayer => dbPlayer.Id == player.Id);
		}

		public bool PlayerNameIsUnique(Player player) {
			if (DbContext.Players.Any(dbPlayer => dbPlayer.Username == player.Username)) return false;
			else return true;
		}

		public bool ValidateUser(Player player) {
			var playerFromDb = (from players in DbContext.Players
							   where players.Username == player.Username
							   select players).First();
			if (playerFromDb.Password == player.Password) return true;
			else return false;
		}

		public bool FleetNameIsUnique(Player player, string fleetName) {
			if (DbContext.Fleets.Any(fleet => fleet.Name == fleetName && fleet.Owner.Id == player.Id)) return false;
			else return true;
		}

		public bool FleetPointsInRange(Fleet fleet, Player player) {
			// TODO
			return true;
		}

		public bool FleetShipsExpRequirement(Fleet fleet, Player player) {
			// TODO
			return true;
		}

		#endregion

		#region IDisposable
		public void Dispose() {
			this.DbContext.Dispose();
		}
		#endregion
	}
}
