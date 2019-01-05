using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	public interface IGameDataBase : IDisposable {
		//SELECT
		BaseModifiers GetBaseModifiers();
		List<Faction> GetAllFactions();
		Faction GetFactionWithId(int id);
		DbFleet GetFleetWithId(int id);
		List<DbFleet> GetAllFleetsOfPlayer(Player player);
		DbShip GetShipWithId(int id);
		List<DbShip> GetAllShips();
		List<DbShip> GetPlayersShips(Player player);
		List<DbShipTemplate> GetAllShipTemplates();
		DbShipTemplate GetShipTemplateWithId(int id);
		List<DbShipTemplate> GetShipsAvailableForExp(int exp);
		DbShipTemplate GetRandomShipTemplateOfRarity(Rarity rarity, int expReq);
		List<DbShipTemplate> GetShipTemplatesWithRarityAndReqExp(Rarity rarity, int reqExp);
		List<DbWeapon> GetAllWeapons();
		List<DbDefenceSystem> GetAllDefences();
		DbLootBox GetLootBoxWithId(int id);
		List<DbLootBox> GetAllLootBoxes();
		DbPlayer GetPlayerWithUsername(string username);
		List<DbPlayer> GetAllPlayers();
		DbGameHistory GetGameHistoryEntry(int id);
		List<DbGameHistory> GetPlayersGameHistory(int playerId);
		int GetPlayerShipCount(Player player);
		int GetPlayerFleetCount(Player player);

		//INSERT
		void AddPlayer(DbPlayer player);
		void AddFaction(Faction faction);
		void AddWeapon(DbWeapon weapon);
		void AddDefenceSystem(DbDefenceSystem defence);
		void AddFleet(Fleet fleet, Player owner);
		void AddShipTemplate(DbShipTemplate shipTemplate);
		void AddShip(DbShip ship);
		void AddBaseModifiers(DbBaseModifiers mods);
		void AddLootBox(DbLootBox lootbox);
		void AddGameHistory(DbGameHistory entry);

		//UPDATE
		void UpdateShipTemplate(DbShipTemplate newData);
		void UpdatePlayer(DbPlayer newData);
		void UpdateFleet(DbFleet newData);
		void UpdateShipExp(Ship ship, int expToAdd);
		void UpdateWeapon(Weapon weapon);
		void UpdateDefenceSystem(DefenceSystem defence);
		void UpdateBaseModifiers(BaseModifiers mods);
		void UpdateLootbox(LootBox newData);

		//DELETE
		bool RemoveShipWithId(int id, bool isAdmin, int playerId = 0);
		bool RemoveFleetWithId(int id, bool isAdmin, int playerId = 0);
		bool RemovePlayerWithUsername(string username);

		//Checks
		bool FleetNameIsUnique(Player player, string fleetName);
		bool PlayerExists(Player player);
		bool PlayerNameIsUnique(Player player);
		bool ValidateUser(Player player);
		bool UserIsAdmin(Player player);

		//Converts
		DbFleet ConvertFleetToDbFleet(Fleet fleet, Player player, bool isNew);
		DbShipTemplate ConvertShipToShipTemplate(Ship ship);

	}

}
