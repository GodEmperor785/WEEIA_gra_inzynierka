using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	interface IGameDataBase : IDisposable {
		DbFleet GetFleetWithId(int id);

		DbShip GetShipWithId(int id);

		DbPlayer GetPlayerWithUsername(string username);

		List<DbFleet> GetAllFleetsOfPlayer(Player player);

		List<DbShip> GetAllShips();

		List<Faction> GetAllFactions();

		BaseModifiers GetBaseModifiers();

		bool FleetNameIsUnique(Player player, string fleetName);

		bool PlayerExists(Player player);

		bool PlayerNameIsUnique(Player player);

		void AddPlayer(DbPlayer player);

		void AddFaction(Faction faction);

		void AddWeapon(DbWeapon weapon);

		void AddDefenceSystem(DbDefenceSystem defence);

		List<DbWeapon> GetAllWeapons();

		List<DbDefenceSystem> GetAllDefences();

		void UpdateShipTemplate(DbShipTemplate newData);

		bool RemoveTemplateShipWithId(int id);

		List<DbShipTemplate> GetShipsAvailableForExp(int exp);

		bool ValidateUser(Player player);

		void AddFleet(Fleet fleet);

		void AddShipTemplate(DbShipTemplate shipTemplate);

		void AddShip(DbShip ship);

		DbShipTemplate GetShipTemplateWithId(int id);

		List<DbShipTemplate> GetShipTemplatesWithRarityAndReqExp(Rarity rarity, int reqExp);

		int GetPlayerShipCount(Player player);

		DbShipTemplate GetRandomShipTemplateOfRarity(Rarity rarity, int expReq);

		DbLootBox GetLootBoxWithId(int id);

		List<DbLootBox> GetAllLootBoxes();

		void UpdatePlayer(DbPlayer newData);

		bool RemoveShipWithId(int id, bool isAdmin, int playerId = 0);

		List<DbGameHistory> GetPlayersGameHistory(int playerId);

		void AddBaseModifiers(DbBaseModifiers mods);

		List<DbPlayer> GetAllPlayers();

		void AddLootBox(DbLootBox lootbox);

		List<DbShipTemplate> GetAllShipTemplates();

		void AddGameHistory(DbGameHistory entry);
	}
}
