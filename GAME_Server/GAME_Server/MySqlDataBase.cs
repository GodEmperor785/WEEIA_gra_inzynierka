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

		internal MySqlDataBase() {
			dbContext = new GameDBContext();
		}

		internal GameDBContext DbContext { get => dbContext; }
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
		public void AddFleet(Fleet fleet) {													//NOT TESTED
			List<int> shipIds = fleet.Ships.Select(s => s.Id).ToList();
			List<DbShip> shipsForFleet = this.GetShips(shipIds);
			DbPlayer owner = this.GetPlayerWithUsername(fleet.Owner.Username);
			DbFleet newFleet = new DbFleet(owner, shipsForFleet, fleet.Name);
			DbContext.Fleets.Add(newFleet);
		}

		#endregion

		#region SELECT
		public List<Faction> GetAllFactions() {
			var query = from factions in DbContext.Factions
						select factions;
			return query.ToList();
		}

		public List<DbFleet> GetAllFleetsOfPlayer(Player player) {
			var query = from fleets in DbContext.Fleets
						where fleets.Owner.Id == player.Id
						select fleets;
			return query.ToList();
		}

		public List<DbShip> GetAllShips() {
			var query = from dbShips in DbContext.Ships
						select dbShips;
			return query.ToList();
		}

		public BaseModifiers GetBaseModifiers() {
			var query = from baseMods in DbContext.BaseModifiers
						where baseMods.Id == 1
						select baseMods;
			var baseModifiers = query.FirstOrDefault();
			return baseModifiers.ToBaseModifiers();
		}

		public DbFleet GetFleetWithId(int id) {
			var query = from fleets in DbContext.Fleets
						where fleets.Id == id
						select fleets;
			return query.FirstOrDefault();
		}

		/// <summary>
		/// gets player with specified username. Checks only first selected player because player username is unique
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public DbPlayer GetPlayerWithUsername(string username) {
			var query = from players in DbContext.Players
						where players.Username == username
						select players;
			return query.FirstOrDefault();	//player username is unique
		}

		public DbShip GetShipWithId(int id) {
			var query = from ships in DbContext.Ships
						where ships.Id == id
						select ships;
			return query.FirstOrDefault();
		}

		public DbShipTemplate GetShipTemplateWithId(int id) {
			var query = from shipTemplates in DbContext.ShipTemplates
						where shipTemplates.Id == id
						select shipTemplates;
			return query.FirstOrDefault();
		}

		public List<DbWeapon> GetAllWeapons() {
			var query = from weapons in DbContext.Weapons
						select weapons;
			return query.ToList();
		}

		public List<DbDefenceSystem> GetAllDefences() {
			var query = from defences in DbContext.DefenceSystems
						select defences;
			return query.ToList();
		}

		/// <summary>
		/// gets DbWeapons with ids
		/// </summary>
		/// <param name="weps"></param>
		/// <returns></returns>
		public List<DbWeapon> GetDbWeapons(List<int> weps) {
			//
			var query = from weapons in DbContext.Weapons
						where weps.Contains(weapons.Id)
						select weapons;
			return query.ToList();
		}

		/// <summary>
		/// gets DbDefenceSystems with ids
		/// </summary>
		/// <param name="defs"></param>
		/// <returns></returns>
		public List<DbDefenceSystem> GetDbDefenceSystems(List<int> defs) {
			var query = from defences in DbContext.DefenceSystems
						where defs.Contains(defences.Id)
						select defences;
			return query.ToList();
		}

		/// <summary>
		/// gets ships which ids are in shipIds
		/// </summary>
		/// <param name="shipIds"></param>
		/// <returns></returns>
		public List<DbShip> GetShips(List<int> shipIds) {
			var query = from ships in DbContext.Ships
						where shipIds.Contains(ships.Id)
						select ships;
			return query.ToList();
		}

		/// <summary>
		/// returns ships templates that have exp requirement equal or lower than specified in parameter
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public List<DbShipTemplate> GetShipsAvailableForExp(int exp) {
			var query = from ships in DbContext.ShipTemplates
						where ships.ExpUnlock <= exp
						select ships;
			return query.ToList();
		}

		#endregion

		#region UPDATE
		/// <summary>
		/// updates ship template with new data, weapon and defence systems lists in newData must be got from DB! Make sure you have a previously unused list from DB before you update with it
		/// </summary>
		/// <param name="newData"></param>
		public void UpdateShipTemplate(DbShipTemplate newData) {
			var shipToUpdate = (from ship in DbContext.ShipTemplates where ship.Id == newData.Id select ship).First();
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
		#endregion

		#region DELETE
		/// <summary>
		/// deletes ship with specified id, can't delete ship that belongs to any player or to any fleet. returns true if delete completed, false if at least one ship of this template exists
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool RemoveTemplateShipWithId(int id) {
			var shipToDelete = (from ship in DbContext.ShipTemplates where ship.Id == id select ship).First();
			if (shipToDelete.ShipsOfThisTemplate.Count > 0) return false;

			//shipToDelete.Weapons.Clear();
			//shipToDelete.Defences.Clear();
			//SaveChanges();
			DbContext.ShipTemplates.Remove(shipToDelete);
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
