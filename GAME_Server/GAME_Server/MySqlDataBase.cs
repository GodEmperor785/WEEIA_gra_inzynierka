﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using GAME_connection;

namespace GAME_Server {
	public class MySqlDataBase : IGameDataBase, IDisposable {
		#region fields, constructors and properties
		private GameDBContext dbContext;
		private Random rng;

		#region basic queries
		private IQueryable<DbShipTemplate> BasicShipTemplateQuery {
			get {
				//return from shipTemplates in DbContext.ShipTemplates select shipTemplates;
				return DbContext.ShipTemplates.Include(x => x.Faction).Include(x => x.Weapons).Include(x => x.Weapons.Select(d => d.Faction))
					.Include(x => x.Defences).Include(x => x.Defences.Select(d => d.Faction));
			}
		}

		private IQueryable<DbShip> BasicShipQuery {
			get {
				//return from ships in DbContext.Ships select ships;
				return DbContext.Ships.Include(x => x.Owner).Include(x => x.ShipBaseStats.Faction).Include(x => x.ShipBaseStats).Include(x => x.ShipBaseStats.Weapons)
					.Include(x => x.ShipBaseStats.Weapons.Select(w => w.Faction)).Include(x => x.ShipBaseStats.Defences).Include(x => x.ShipBaseStats.Defences.Select(d => d.Faction));
			}
		}

		private IQueryable<DbPlayer> BasicPlayerQuery {
			get {
				//return from players in DbContext.Players select players;
				return DbContext.Players.Include(x => x.OwnedShips);
			}
		}

		private IQueryable<DbFleet> BasicFleetQueryPt1 {
			get {
				var q = DbContext.Fleets.Include(o => o.Owner).Include(fs => fs.Ships).Include(so => so.Ships.Select(s => s.Owner)).Include(sb => sb.Ships.Select(ssb => ssb.ShipBaseStats))
				.Include(x => x.Ships.Select(s => s.ShipBaseStats.Faction));
				return q;
			}
		}

		private IQueryable<DbFleet> BasicFleetQueryPt2 {
			get {
				var q2 = DbContext.Fleets.Include(xx => xx.Ships.Select(ss => ss.ShipBaseStats.Weapons)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)));
				return q2;
			}
		}

		private IQueryable<DbFleet> BasicFleetQueryPt3 {
			get {
				var q3 = DbContext.Fleets.Include(xxx => xxx.Ships.Select(sss => sss.ShipBaseStats.Defences)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Defences.Select(d => d.Faction)));
				return q3;
			}
		}

		/*private IQueryable<DbFleet> BasicFleetQuery {
				get {*/
				//return from fleets in DbContext.Fleets select fleets;
				/*return DbContext.Fleets.Include(x => x.Owner).Include(x => x.Ships.Select(s => s.Owner)).Include(x => x.Ships.Select(s => s.ShipBaseStats)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Faction))
					.Include(x => x.Ships.Select(s => s.ShipBaseStats.Weapons)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)))
					.Include(x => x.Ships.Select(s => s.ShipBaseStats.Defences)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Defences.Select(d => d.Faction)));*/
				/*var q = DbContext.Fleets.Include(o => o.Owner).Include(fs => fs.Ships).Include(so => so.Ships.Select(s => s.Owner)).Include(sb => sb.Ships.Select(ssb => ssb.ShipBaseStats))
				.Include(x => x.Ships.Select(s => s.ShipBaseStats.Faction));
				var q2 = DbContext.Fleets.Include(xx => xx.Ships.Select(ss => ss.ShipBaseStats.Weapons)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)));
				var q3 = DbContext.Fleets.Include(xxx => xxx.Ships.Select(sss => sss.ShipBaseStats.Defences)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Defences.Select(d => d.Faction)));
				var basic = q.ToList();
				var weps = q2.ToList();
				var defs = q3.ToList();
				return q;
			}
		}*/

		private IQueryable<DbWeapon> BasicWeaponQuery {
			get {
				//return from weapons in DbContext.Weapons select weapons;
				return DbContext.Weapons.Include(x => x.Faction);
			}
		}

		private IQueryable<DbDefenceSystem> BasicDefenceSystemQuery {
			get {
				//return from defences in DbContext.DefenceSystems select defences;
				return DbContext.DefenceSystems.Include(x => x.Faction);
			}
		}

		private IQueryable<DbLootBox> BasicLootBoxQuery {
			get {
				return from lootboxes in DbContext.LootBoxes select lootboxes;
			}
		}

		private IQueryable<DbGameHistory> BasicPlayersHistoryQuery {
			get {
				var q = DbContext.GameHistories.Include(x => x.Winner).Include(x => x.Loser).Include(x => x.WinnerFleet).Include(x => x.LoserFleet);
				return q;
			}
		}

		private DbGameHistory HistoryQuery(int entryId) {
			var q = DbContext.GameHistories.Include(x => x.Winner).Include(x => x.WinnerFleet).Include(x => x.WinnerFleet.Owner).Include(x => x.WinnerFleet.Ships)
				.Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats)).Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Faction))
				.Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats)).Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Faction));
			var q2 = DbContext.GameHistories.Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Weapons)).Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)));
			var q22 = DbContext.GameHistories.Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Defences)).Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Defences.Select(w => w.Faction)));
			var q3 = DbContext.GameHistories.Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Weapons)).Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)));
			var q32 = DbContext.GameHistories.Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Defences)).Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Defences.Select(w => w.Faction)));
			q = q.Where(history => history.Id == entryId);
			q2 = q2.Where(history => history.Id == entryId);
			q3 = q3.Where(history => history.Id == entryId);
			q22 = q22.Where(history => history.Id == entryId);
			q32 = q32.Where(history => history.Id == entryId);
			var basic = q.FirstOrDefault();
			var winner = q2.FirstOrDefault();
			var winner2 = q22.FirstOrDefault();
			var loser = q3.FirstOrDefault();
			var loser2 = q32.FirstOrDefault();
			return basic;
			/*return DbContext.GameHistories.Include(x => x.Winner).Include(x => x.WinnerFleet).Include(x => x.WinnerFleet.Owner).Include(x => x.WinnerFleet.Ships)
				.Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats)).Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Faction))
				.Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Weapons)).Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)))
				.Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Defences)).Include(x => x.WinnerFleet.Ships.Select(s => s.ShipBaseStats.Defences.Select(w => w.Faction)))
				.Include(x => x.Loser).Include(x => x.LoserFleet).Include(x => x.LoserFleet.Owner).Include(x => x.LoserFleet.Ships)
				.Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats)).Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Faction))
				.Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Weapons)).Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)))
				.Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Defences)).Include(x => x.LoserFleet.Ships.Select(s => s.ShipBaseStats.Defences.Select(w => w.Faction)));*/
		}
		#endregion

		internal MySqlDataBase() {
			dbContext = new GameDBContext();
			rng = new Random();
			//dbContext.Configuration.LazyLoadingEnabled = false;
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
		/// converts <see cref="Fleet"/> from user to server's internal <see cref="DbFleet"/>
		/// </summary>
		/// <param name="fleet"></param>
		/// <returns></returns>
		public DbFleet ConvertFleetToDbFleet(Fleet fleet, Player player, bool isNew) {
			List<int> shipIds = fleet.Ships.Select(s => s.Id).ToList();
			List<DbShip> shipsForFleet = this.GetShips(shipIds);
			DbPlayer owner = this.GetPlayerWithUsername(player.Username);
			if (!isNew) return new DbFleet(fleet.Id, owner, shipsForFleet, fleet.Name);     //if fleet is NOT new remember ID
			else return new DbFleet(owner, shipsForFleet, fleet.Name);
		}

		/// <summary>
		/// adds fleet to Db, does NOT validate the fleet, this should be done earlier (including <see cref="FleetNameIsUnique"/>)
		/// </summary>
		/// <param name="fleet"></param>
		public void AddFleet(Fleet fleet, Player owner) {                                                 //NOT TESTED
			DbContext.Fleets.Add(ConvertFleetToDbFleet(fleet, owner, true));
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
			/*var q1 = DbContext.Fleets.Include(o => o.Owner).Include(fs => fs.Ships).Include(so => so.Ships.Select(s => s.Owner)).Include(sb => sb.Ships.Select(ssb => ssb.ShipBaseStats))
				.Include(x => x.Ships.Select(s => s.ShipBaseStats.Faction)).Where(x => x.Owner.Id == player.Id);
			var q2 = DbContext.Fleets.Include(xx => xx.Ships.Select(ss => ss.ShipBaseStats.Weapons)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Weapons.Select(w => w.Faction)))
				.Where(x => x.Owner.Id == player.Id);
			var q3 = DbContext.Fleets.Include(xxx => xxx.Ships.Select(sss => sss.ShipBaseStats.Defences)).Include(x => x.Ships.Select(s => s.ShipBaseStats.Defences.Select(d => d.Faction)))
				.Where(x => x.Owner.Id == player.Id);
			var basic = q1.ToList();
			var weps = q2.ToList();
			var defs = q3.ToList();
			return basic;*/
			var q1 = BasicFleetQueryPt1.Where(x => x.Owner.Id == player.Id).ToList();
			var q2 = BasicFleetQueryPt2.Where(x => x.Owner.Id == player.Id).ToList();
			var q3 = BasicFleetQueryPt3.Where(x => x.Owner.Id == player.Id).ToList();
			return q1;
		}

		public List<DbShip> GetAllShips() {
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
			var q1 = BasicFleetQueryPt1.Where(x => x.Id == id).FirstOrDefault();
			var q2 = BasicFleetQueryPt2.Where(x => x.Id == id).FirstOrDefault();
			var q3 = BasicFleetQueryPt3.Where(x => x.Id == id).FirstOrDefault();
			return q1;
			//var query = BasicFleetQuery.Where(fleet => fleet.Id == id);
			//return query.FirstOrDefault();
		}

		/// <summary>
		/// gets player with specified username. Checks only first selected player because player username is unique
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public DbPlayer GetPlayerWithUsername(string username) {
			var query = BasicPlayerQuery.Where(player => player.Username == username);
			return query.FirstOrDefault();  //player username is unique
		}

		public DbShip GetShipWithId(int id) {
			var query = BasicShipQuery.Where(ship => ship.Id == id);
			return query.FirstOrDefault();
		}

		public DbShipTemplate GetShipTemplateWithId(int id) {
			var query = BasicShipTemplateQuery.Where(shipTempl => shipTempl.Id == id);
			return query.FirstOrDefault();
		}

		public List<DbWeapon> GetAllWeapons() {
			return BasicWeaponQuery.ToList();
		}

		public List<DbDefenceSystem> GetAllDefences() {
			return BasicDefenceSystemQuery.ToList();
		}

		/// <summary>
		/// gets DbWeapons with ids
		/// </summary>
		/// <param name="weps"></param>
		/// <returns></returns>
		public List<DbWeapon> GetDbWeapons(List<int> weps) {
			var query = BasicWeaponQuery.Where(weapons => weps.Contains(weapons.Id));
			return query.ToList();
		}

		/// <summary>
		/// gets DbDefenceSystems with ids
		/// </summary>
		/// <param name="defs"></param>
		/// <returns></returns>
		public List<DbDefenceSystem> GetDbDefenceSystems(List<int> defs) {
			var query = BasicDefenceSystemQuery.Where(defences => defs.Contains(defences.Id));
			return query.ToList();
		}

		/// <summary>
		/// gets ships which ids are in shipIds
		/// </summary>
		/// <param name="shipIds"></param>
		/// <returns></returns>
		public List<DbShip> GetShips(List<int> shipIds) {
			var query = BasicShipQuery.Where(ships => shipIds.Contains(ships.Id));
			return query.ToList();
		}

		/// <summary>
		/// returns ships templates that have exp requirement equal or lower than specified in parameter
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public List<DbShipTemplate> GetShipsAvailableForExp(int exp) {
			var query = BasicShipTemplateQuery.Where(shipTempl => shipTempl.ExpUnlock <= exp);
			return query.ToList();
		}

		public List<DbShipTemplate> GetShipTemplatesWithRarityAndReqExp(Rarity rarity, int reqExp) {
			var query = BasicShipTemplateQuery.Where(shipTempl => ((shipTempl.ExpUnlock <= reqExp) && (shipTempl.ShipRarity == rarity)));
			return query.ToList();
		}

		public int GetPlayerShipCount(Player player) {
			return GetPlayersShips(player).Count;
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

		/// <summary>
		/// gets basic player game history - fleets are not set, you need to call <see cref="GetGameHistoryEntry"/>
		/// </summary>
		/// <param name="playerId"></param>
		/// <returns></returns>
		public List<DbGameHistory> GetPlayersGameHistory(int playerId) {
			/*var query = BasicHistoryQuery(playerId);
			return query.ToList();*/
			var query = BasicPlayersHistoryQuery.Where(history => history.Winner.Id == playerId || history.Loser.Id == playerId);
			return query.ToList();
		}

		public DbGameHistory GetGameHistoryEntry(int id) {
			return HistoryQuery(id);
		}

		public List<DbPlayer> GetAllPlayers() {
			return BasicPlayerQuery.ToList();
		}

		public List<DbShipTemplate> GetAllShipTemplates() {
			return BasicShipTemplateQuery.ToList();
		}

		public List<DbShip> GetPlayersShips(Player player) {
			var query = BasicShipQuery.Where(ship => ship.Owner.Id == player.Id);
			return query.ToList();
		}

		public int GetPlayerFleetCount(Player player) {
			return GetAllFleetsOfPlayer(player).Count;
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

		public void UpdateFleet(DbFleet newData) {
			var fleetToUpdate = GetFleetWithId(newData.Id);
			fleetToUpdate.Name = newData.Name;
			fleetToUpdate.Ships = newData.Ships;
			//fleetToUpdate.Owner = newData.Owner;
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
			if (!isAdmin && shipToDelete.Owner.Id != playerId) return false; //user cant delete ship he does not own!

			DbContext.Ships.Remove(shipToDelete);
			SaveChanges();
			return true;
		}

		/// <summary>
		/// for admin call RemoveFleetWithId(fleetId, true), player cant remove fleets he does not own
		/// </summary>
		/// <param name="id"></param>
		/// <param name="isAdmin"></param>
		/// <param name="playerId"></param>
		/// <returns></returns>
		public bool RemoveFleetWithId(int id, bool isAdmin, int playerId = 0) {
			var fleetToDelete = GetFleetWithId(id);
			if (!isAdmin && fleetToDelete.Owner.Id != playerId) return false; //user cant delete fleet he does not own!

			DbContext.Fleets.Remove(fleetToDelete);
			SaveChanges();
			return true;
		}
		#endregion

		#region Checks
		public bool PlayerExists(Player player) {
			return DbContext.Players.Any(dbPlayer => dbPlayer.Username == player.Username);
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

		public bool FleetPointsInRange(DbFleet fleet, Player player) {
			// TODO
			return true;
		}

		public bool FleetShipsExpRequirement(DbFleet fleet, Player player) {
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
