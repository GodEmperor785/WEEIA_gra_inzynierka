using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using GAME_connection;
using GAME_Validator;

namespace GAME_AdminApp {
	public enum Operation {
		ADD,
		UPDATE,
		NONE
	}

	public partial class AdminForm : Form {
		private Dictionary<Button, int> shipButtonToIdMap = new Dictionary<Button, int>();
		private Dictionary<Button, int> weaponButtonToIdMap = new Dictionary<Button, int>();
		private Dictionary<Button, int> defenceButtonToIdMap = new Dictionary<Button, int>();
		private Dictionary<Button, int> userButtonToIdMap = new Dictionary<Button, int>();
		private string baseShipModifyDesc;
		private string baseWeaponModifyDesc;
		private string baseWeaponCalcChanceToHit;
		private string baseDefenceModifyDesc;
		private string baseUserDescription;

		private Operation operationType = Operation.NONE;
		private Operation weaponOperationType = Operation.NONE;
		private Operation defenceOperationType = Operation.NONE;
		private Operation userOperationType = Operation.NONE;
		private int modifiedShipId;
		private int modifiedWeaponID;
		private int modifiedDefenceID;
		private int modifiedUserID;

		public List<Weapon> ShipWeapons { get; set; }
		public List<DefenceSystem> ShipDefences { get; set; }

		public AdminForm() {
			InitializeComponent();

			shipIdSearchBox.Maximum = decimal.MaxValue;
			weaponIdSearch.Maximum = decimal.MaxValue;
			defenceIdSearch.Maximum = decimal.MaxValue;

			baseShipModifyDesc = shipDescriptionLabel.Text;
			baseWeaponModifyDesc = weaponDescription.Text;
			baseWeaponCalcChanceToHit = weaponCalculatedChanceToHit.Text;
			baseDefenceModifyDesc = defenceDescription.Text;
			baseUserDescription = userDescription.Text;

			SetModfiyControlsEnabled(false, weaponSubmitTable);
			SetModfiyControlsEnabled(false, defenceSubmitTable);
			SetModfiyControlsEnabled(false, userSubmitTable);
		}

		private void AdminForm_FormClosed(object sender, FormClosedEventArgs e) {
			AdminApp.ExitApp();
		}

		private bool CheckSuccessOrFailure() {
			GamePacket packet = AdminApp.Connection.GetReceivedPacket();
			if (packet.OperationType == OperationType.SUCCESS) {
				MessageBox.Show("Operation success", "Operation success");
				return true;
			}
			else {
				MessageBox.Show((string)packet.Packet, "Operation failed!");
				return false;
			}
		}

		/// <summary>
		/// exit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, EventArgs e) {
			DialogResult exitResult = MessageBox.Show("Do you want to exit application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			if (exitResult == DialogResult.Yes) {
				AdminApp.ExitApp();
			}
		}

		private void InitializeFactionComboBox(ComboBox box) {
			box.DataSource = AdminApp.GameData.Factions;
			box.DisplayMember = "Name";
			box.SelectedIndex = 0;
		}

		private void InitializeOrderByPropertyComboBox(ComboBox box, Type type) {
			var propertyList = type.GetProperties();
			box.DataSource = propertyList;
			box.DisplayMember = "Name";
			box.SelectedItem = type.GetProperty("Id");   //set default as Id
		}

		private IEnumerable<T> OrderListByProperty<T>(ComboBox box, IEnumerable<T> list) {
			string orderByPropertyName = ((PropertyInfo)box.SelectedItem).Name;
			list = list.OrderBy(x => x.GetType().GetProperty(orderByPropertyName).GetValue(x, null));
			return list;
		}

		private void SetModfiyControlsEnabled(bool enabled, TableLayoutPanel table) {
			foreach (Control control in table.Controls) {
				control.Enabled = enabled;
			}
		}

//=================================================================================================================================================================================

		#region Ship Templates
		public void InitializeShipDropDownLists() {
			InitializeFactionComboBox(shipFaction);
			InitializeOrderByPropertyComboBox(shipOrderByBox, typeof(Ship));
		}

		private void InitializeShipSubmitDropDownLists() {
			InitializeFactionComboBox(shipFactionBox);

			shipRarityBox.DataSource = Enum.GetValues(typeof(Rarity));
			shipRarityBox.SelectedIndex = 0;
		}

		private void shipSearchButton_Click(object sender, EventArgs e) {
			//first clear existing rows
			AdminFormUtils.ClearPreviousSearch(shipTable, shipButtonToIdMap);

			bool idOrNameSearch = false;
			int idSearch = Convert.ToInt32(shipIdSearchBox.Value);
			string nameSearch = shipNameSearchBox.Text;
			string factionSearch = ((Faction)shipFaction.SelectedItem).Name;

			IEnumerable<Ship> searchedShips = AdminApp.GameData.ShipTemplates.AsEnumerable();
			//if id != 0 search by id
			if (idSearch != 0) {
				idOrNameSearch = true;
				searchedShips = searchedShips.Where(templ => templ.Id == idSearch);
			}
			//if name not empty search by name
			if (!string.IsNullOrWhiteSpace(nameSearch)) {
				idOrNameSearch = true;
				searchedShips = searchedShips.Where(templ => templ.Name.Contains(nameSearch));
			}
			//then search by faction if other are not set
			if(!idOrNameSearch) searchedShips = searchedShips.Where(templ => templ.Faction.Name == factionSearch);
			//lastly order by chosen property
			searchedShips = OrderListByProperty(shipOrderByBox, searchedShips);

			//display results
			AddShipTemplateList(searchedShips.ToList());
		}

		private void AddShipTemplateList(List<Ship> list) {
			foreach(Ship ship in list) {
				AddShipTemplateToTable(ship);
			}
		}

		private void AddShipTemplateToTable(Ship ship) {
			RowStyle temp = shipTable.RowStyles[shipTable.RowCount - 1];
			shipTable.RowCount++;
			shipTable.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));

			Button weaponsButton = new Button() { Text = "Weapons" };
			weaponsButton.Click += new EventHandler(WeaponsButton_Click);
			shipButtonToIdMap.Add(weaponsButton, ship.Id);

			Button defencesButton = new Button() { Text = "Defences" };
			defencesButton.Click += new EventHandler(DefencesButton_Click);
			shipButtonToIdMap.Add(defencesButton, ship.Id);

			Button modifyButton = new Button() { Text = "Modify this template", AutoSize = true };
			modifyButton.Click += new EventHandler(ModifyButton_Click);
			shipButtonToIdMap.Add(modifyButton, ship.Id);

			shipTable.Controls.Add(new Label() { Text = ship.Id.ToString()}, 0, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Name, AutoSize = true }, 1, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Faction.Name }, 2, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Cost.ToString() }, 3, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Evasion.ToString() }, 4, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Hp.ToString() }, 5, shipTable.RowCount - 1);
			shipTable.Controls.Add(weaponsButton, 6, shipTable.RowCount - 1);
			shipTable.Controls.Add(defencesButton, 7, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Size.ToString() }, 8, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Armor.ToString() }, 9, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.ExpUnlock.ToString() }, 10, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Rarity.GetRarityName() }, 11, shipTable.RowCount - 1);
			shipTable.Controls.Add(modifyButton, 12, shipTable.RowCount - 1);
		}

		/// <summary>
		/// show weapons in READ-ONLY mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WeaponsButton_Click(object sender, EventArgs e) {
			int shipID = shipButtonToIdMap[sender as Button];
			Ship ship = GetShipWithID(shipID);
			ShipWeaponsForm form = new ShipWeaponsForm();
			List<Weapon> shipWeapons = AdminApp.GameData.ShipTemplates.Where(s => s.Id == shipID).Select(s => s.Weapons).FirstOrDefault(); 
			form.SetForViewOnly(ship, shipWeapons);
			form.Show();
		}

		/// <summary>
		/// show defences in READ-ONLY mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefencesButton_Click(object sender, EventArgs e) {
			int shipID = shipButtonToIdMap[sender as Button];
			Ship ship = GetShipWithID(shipID);
			ShipDefencesForm form = new ShipDefencesForm();
			List<DefenceSystem> shipDefences = AdminApp.GameData.ShipTemplates.Where(s => s.Id == shipID).Select(s => s.Defences).FirstOrDefault();
			form.SetForViewOnly(ship, shipDefences, ship.Size);
			form.Show();
		}

		private void ModifyButton_Click(object sender, EventArgs e) {
			InitializeShipSubmitDropDownLists();

			int shipID = shipButtonToIdMap[sender as Button];
			shipDescriptionLabel.Text = baseShipModifyDesc + shipID;
			Ship ship = GetShipWithID(shipID);

			SetModfiyControlsEnabled(true, submitTableLayout);
			operationType = Operation.UPDATE;
			modifiedShipId = shipID;

			shipNameBox.Text = ship.Name;
			shipFactionBox.SelectedItem = ship.Faction;
			shipCostBox.Value = (decimal)ship.Cost;
			shipEvasionBox.Value = (decimal)ship.Evasion;
			shipHpBox.Value = (decimal)ship.Hp;
			shipSizeBox.Value = (decimal)ship.Size;
			shipArmorBox.Value = (decimal)ship.Armor;
			shipExpUnlockBox.Value = (decimal)ship.ExpUnlock;
			shipRarityBox.SelectedItem = ship.Rarity;
			ShipWeapons = ship.Weapons;
			ShipDefences = ship.Defences;
		}

		private void addShipButton_Click(object sender, EventArgs e) {
			InitializeShipSubmitDropDownLists();

			shipDescriptionLabel.Text = "Adding new ship template";

			SetModfiyControlsEnabled(true, submitTableLayout);
			operationType = Operation.ADD;

			SetShipFormToDefaultMin();
		}

		private Ship GetShipWithID(int id) {
			return AdminApp.GameData.ShipTemplates.Where(ship => ship.Id == id).FirstOrDefault();
		}

		/// <summary>
		/// send to server
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void submitButton_Click(object sender, EventArgs e) {
			if(AdminFormUtils.ShowConfirmationBox("ship template")) {
				Ship ship = new Ship(shipNameBox.Text, (Faction)shipFactionBox.SelectedItem, Convert.ToInt32(shipCostBox.Value), (double)shipEvasionBox.Value, (double)shipHpBox.Value, (double)shipSizeBox.Value, (double)shipArmorBox.Value,
						ShipWeapons, ShipDefences, Convert.ToInt32(shipExpUnlockBox.Value), (Rarity)shipRarityBox.SelectedItem);

				string validationResult = GameValidator.ValidateShip(ship);
				GamePacket packet;
				if (validationResult == GameValidator.OK) {
					if (operationType == Operation.UPDATE) {
						ship.Id = modifiedShipId;
						packet = new GamePacket(OperationType.UPDATE_SHIP_TEMPLATE, ship);
					}
					else {
						packet = new GamePacket(OperationType.ADD_SHIP_TEMPLATE, ship);
					}

					AdminApp.Connection.Send(packet);
					if (CheckSuccessOrFailure()) {
						shipSyncButton_Click(sender, e);

						//after success return to default min values and disable submit again
						shipDescriptionLabel.Text = baseShipModifyDesc;
						SetShipFormToDefaultMin();
						SetModfiyControlsEnabled(false, submitTableLayout);
						submitButton.Enabled = false;
					}
				}
				else AdminFormUtils.ShowValidationFailedDialog(validationResult);
			}
		}

		private void SetShipFormToDefaultMin() {
			shipNameBox.Text = "";
			shipFactionBox.SelectedIndex = 0;
			shipCostBox.Value = shipCostBox.Minimum;
			shipEvasionBox.Value = shipEvasionBox.Minimum;
			shipHpBox.Value = shipHpBox.Minimum;
			shipSizeBox.Value = shipSizeBox.Minimum;
			shipArmorBox.Value = shipArmorBox.Minimum;
			shipExpUnlockBox.Value = shipExpUnlockBox.Minimum;
			shipRarityBox.SelectedIndex = 0;
			ShipWeapons = new List<Weapon>();
			ShipDefences = new List<DefenceSystem>();
		}

		private void shipSyncButton_Click(object sender, EventArgs e) {
			GamePacket packet = new GamePacket(OperationType.GET_SHIP_TEMPLATES, new object());
			AdminApp.Connection.Send(packet);
			packet = AdminApp.Connection.GetReceivedPacket();
			AdminApp.GameData.ShipTemplates = (List<Ship>) packet.Packet;
		}

		/// <summary>
		/// displays new window with defence system selection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void shipDefencesModifyButton_Click(object sender, EventArgs e) {
			ShipDefencesForm form = new ShipDefencesForm();
			if (operationType == Operation.ADD) {
				form.SetForNew((Faction)shipFactionBox.SelectedItem, ShipDefences, (double)shipSizeBox.Value);
			}
			else {
				int shipID = modifiedShipId;
				Ship ship = GetShipWithID(shipID);
				form.SetForModify(ship, ShipDefences, (double)shipSizeBox.Value);
			}
			form.Show();
		}

		/// <summary>
		/// displays new window with weapon selection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void shipWeaponsModifyButton_Click(object sender, EventArgs e) {
			ShipWeaponsForm form = new ShipWeaponsForm();
			if (operationType == Operation.ADD) {
				form.SetForNew((Faction)shipFactionBox.SelectedItem, ShipWeapons);
			}
			else {
				int shipID = modifiedShipId;
				Ship ship = GetShipWithID(shipID);
				form.SetForModify(ship, ShipWeapons);
			}
			form.Show();
		}
		#endregion

//=================================================================================================================================================================================

		#region Weapons
		public void InitializeWeaponsDropDownLists() {
			InitializeFactionComboBox(weaponFactionSearch);
			InitializeOrderByPropertyComboBox(weaponOrderBy, typeof(Weapon));

			weaponTypeSearch.DataSource = Enum.GetValues(typeof(WeaponType));
			weaponTypeSearch.SelectedIndex = 0;
		}

		private void InitializeWeaponsSubmitDropDownLists() {
			InitializeFactionComboBox(weaponFactionBox);

			weaponTypeBox.DataSource = Enum.GetValues(typeof(WeaponType));
			weaponTypeBox.SelectedIndex = 0;
		}

		private void button1_Click_1(object sender, EventArgs e) {
			//first clear existing rows
			AdminFormUtils.ClearPreviousSearch(weaponTable, weaponButtonToIdMap);

			bool idOrNameSearch = false;
			int idSearch = Convert.ToInt32(weaponIdSearch.Value);
			string nameSearch = weaponNameSearch.Text;
			string factionSearch = ((Faction)weaponFactionSearch.SelectedItem).Name;
			WeaponType typeSearch = ((WeaponType)weaponTypeSearch.SelectedItem);

			IEnumerable<Weapon> searchedWeapons = AdminApp.GameData.Weapons.AsEnumerable();
			//if id != 0 search by id
			if (idSearch != 0) {
				idOrNameSearch = true;
				searchedWeapons = searchedWeapons.Where(wep => wep.Id == idSearch);
			}
			//if name not empty search by name
			if (!string.IsNullOrWhiteSpace(nameSearch)) {
				idOrNameSearch = true;
				searchedWeapons = searchedWeapons.Where(wep => wep.Name.Contains(nameSearch));
			}
			//then search by faction and type if other are not set
			if (!idOrNameSearch) searchedWeapons = searchedWeapons.Where(wep => wep.Faction.Name == factionSearch);
			if (!idOrNameSearch) searchedWeapons = searchedWeapons.Where(wep => wep.WeaponType == typeSearch);

			//lastly order by chosen property
			searchedWeapons = OrderListByProperty(weaponOrderBy, searchedWeapons);

			AdminFormUtils.AddWeaponListForWeapons(weaponTable, searchedWeapons.ToList(), weaponButtonToIdMap, WeaponModifyButton_Click);
		}

		private Weapon GetWeaponWithId(int id) {
			return AdminApp.GameData.Weapons.Where(wep => wep.Id == id).FirstOrDefault();
		}

		private void WeaponModifyButton_Click(object sender, EventArgs e) {
			InitializeWeaponsSubmitDropDownLists();

			int weaponId = weaponButtonToIdMap[sender as Button];
			weaponDescription.Text = baseWeaponModifyDesc + weaponId;
			Weapon weapon = GetWeaponWithId(weaponId);

			SetModfiyControlsEnabled(true, weaponSubmitTable);
			weaponOperationType = Operation.UPDATE;
			modifiedWeaponID = weaponId;

			weaponNameBox.Text = weapon.Name;
			weaponFactionBox.SelectedItem = weapon.Faction;
			weaponDamageBox.Value = (decimal)weapon.Damage;
			weaponNumberOfProjectilesBox.Value = weapon.NumberOfProjectiles;
			weaponTypeBox.SelectedItem = weapon.WeaponType;
			weaponApBox.Value = (decimal)weapon.Damage;
			weaponRangeMultBox.Value = (decimal)weapon.RangeMultiplier;
			weaponChanceToHitBox.Value = (decimal)weapon.ChanceToHit;
			UpdateCalculatedChanceToHit();
		}

		private void addWeaponButton_Click(object sender, EventArgs e) {
			InitializeWeaponsSubmitDropDownLists();

			weaponDescription.Text = "Adding new weapon";

			SetModfiyControlsEnabled(true, weaponSubmitTable);
			weaponOperationType = Operation.ADD;

			SetWeaponSubmitToDefaultMin();
		}

		private void SetWeaponSubmitToDefaultMin() {
			weaponNameBox.Text = "";
			weaponFactionBox.SelectedIndex = 0;
			weaponDamageBox.Value = weaponDamageBox.Minimum;
			weaponNumberOfProjectilesBox.Value = weaponNumberOfProjectilesBox.Minimum;
			weaponTypeBox.SelectedIndex = 0;
			weaponApBox.Value = weaponApBox.Minimum;
			weaponRangeMultBox.Value = weaponRangeMultBox.Minimum;
			weaponChanceToHitBox.Value = weaponChanceToHitBox.Minimum;
			weaponCalculatedChanceToHit.Text = baseWeaponCalcChanceToHit;
		}

		private void syncWeaponsButton_Click(object sender, EventArgs e) {
			GamePacket packet = new GamePacket(OperationType.GET_WEAPONS, new object());
			AdminApp.Connection.Send(packet);
			packet = AdminApp.Connection.GetReceivedPacket();
			AdminApp.GameData.Weapons = (List<Weapon>)packet.Packet;
		}

		private void weaponSubmitButton_Click(object sender, EventArgs e) {
			if (AdminFormUtils.ShowConfirmationBox("weapon")) {
				Weapon weapon = new Weapon(weaponNameBox.Text, (Faction)weaponFactionBox.SelectedItem, (double)weaponDamageBox.Value, Convert.ToInt32(weaponNumberOfProjectilesBox.Value), (WeaponType)weaponTypeBox.SelectedItem, (double)weaponApBox.Value,
						(double)weaponRangeMultBox.Value, (double)weaponChanceToHitBox.Value);

				string validationResult = GameValidator.ValidateWeapon(weapon);
				GamePacket packet;
				if (validationResult == GameValidator.OK) {
					if (weaponOperationType == Operation.UPDATE) {
						weapon.Id = modifiedWeaponID;
						packet = new GamePacket(OperationType.UPDATE_WEAPON, weapon);
					}
					else {
						packet = new GamePacket(OperationType.ADD_WEAPON, weapon);
					}

					AdminApp.Connection.Send(packet);
					if (CheckSuccessOrFailure()) {
						syncWeaponsButton_Click(sender, e);

						//after success return to default min values and disable submit again
						weaponDescription.Text = baseWeaponModifyDesc;
						SetWeaponSubmitToDefaultMin();
						SetModfiyControlsEnabled(false, weaponSubmitTable);
						weaponSubmitButton.Enabled = false;
					}
				}
				else AdminFormUtils.ShowValidationFailedDialog(validationResult);
			}
		}

		/// <summary>
		/// calculates weapon chance to hit with equation from Game.cs
		/// </summary>
		private void UpdateCalculatedChanceToHit() {
			double maxDistChanceToHit = ((double)weaponChanceToHitBox.Value - ((Math.Sqrt(5.0) / 5.0) * AdminApp.GameData.BaseModifiers.WeaponTypeRangeMultMap[(WeaponType)weaponTypeBox.SelectedItem] * ((double)weaponRangeMultBox.Value) * 1.0));
			double minDistChanceToHit = ((double)weaponChanceToHitBox.Value - ((Math.Sqrt(1.0) / 5.0) * AdminApp.GameData.BaseModifiers.WeaponTypeRangeMultMap[(WeaponType)weaponTypeBox.SelectedItem] * ((double)weaponRangeMultBox.Value) * 1.0));
			maxDistChanceToHit = Math.Min(maxDistChanceToHit, 0.01);
			minDistChanceToHit = Math.Min(minDistChanceToHit, 0.01);
			string calculatedChances = "max distance = " + string.Format("{0:N2}%", maxDistChanceToHit * 100.0) + "    " + "min distance = " + string.Format("{0:N2}%", minDistChanceToHit * 100.0);
			weaponCalculatedChanceToHit.Text = calculatedChances;
		}

		private void weaponChanceToHitBox_ValueChanged(object sender, EventArgs e) {
			UpdateCalculatedChanceToHit();
		}

		private void weaponRangeMultBox_ValueChanged(object sender, EventArgs e) {
			UpdateCalculatedChanceToHit();
		}

		private void weaponTypeBox_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateCalculatedChanceToHit();
		}
		#endregion

//=================================================================================================================================================================================

		#region Defences
		public void InitializeDefencesDropDownLists() {
			InitializeFactionComboBox(defenceFactionSearch);
			InitializeOrderByPropertyComboBox(defenceOrderBy, typeof(DefenceSystem));

			defenceTypeSearch.DataSource = Enum.GetValues(typeof(DefenceSystemType));
			defenceTypeSearch.SelectedIndex = 0;
		}

		private void InitializeDefencesSubmitDropDownLists() {
			InitializeFactionComboBox(defenceFactionBox);

			defenceTypeBox.DataSource = Enum.GetValues(typeof(DefenceSystemType));
			defenceTypeBox.SelectedIndex = 0;
		}

		private void defenceSearchButton_Click(object sender, EventArgs e) {
			//first clear existing rows
			AdminFormUtils.ClearPreviousSearch(defenceTable, defenceButtonToIdMap);

			bool idOrNameSearch = false;
			int idSearch = Convert.ToInt32(defenceIdSearch.Value);
			string nameSearch = defenceNameSearch.Text;
			string factionSearch = ((Faction)defenceFactionSearch.SelectedItem).Name;
			DefenceSystemType typeSearch = ((DefenceSystemType)defenceTypeSearch.SelectedItem);

			IEnumerable<DefenceSystem> searchedDefences = AdminApp.GameData.Defences.AsEnumerable();
			//if id != 0 search by id
			if (idSearch != 0) {
				idOrNameSearch = true;
				searchedDefences = searchedDefences.Where(def => def.Id == idSearch);
			}
			//if name not empty search by name
			if (!string.IsNullOrWhiteSpace(nameSearch)) {
				idOrNameSearch = true;
				searchedDefences = searchedDefences.Where(def => def.Name.Contains(nameSearch));
			}
			//then search by faction and type if other are not set
			if (!idOrNameSearch) searchedDefences = searchedDefences.Where(def => def.Faction.Name == factionSearch);
			if (!idOrNameSearch) searchedDefences = searchedDefences.Where(def => def.SystemType == typeSearch);

			//lastly order by chosen property
			searchedDefences = OrderListByProperty(defenceOrderBy, searchedDefences);

			AdminFormUtils.AddDefenceSystemListForDefences(defenceTable, searchedDefences.ToList(), defenceButtonToIdMap, DefenceModifyButton_Click);
		}

		private DefenceSystem GetDefenceSystemWithId(int id) {
			return AdminApp.GameData.Defences.Where(def => def.Id == id).FirstOrDefault();
		}

		private void DefenceModifyButton_Click(object sender, EventArgs e) {
			InitializeDefencesSubmitDropDownLists();

			int defenceId = defenceButtonToIdMap[sender as Button];
			defenceDescription.Text = baseDefenceModifyDesc + defenceId;
			DefenceSystem defence = GetDefenceSystemWithId(defenceId);

			SetModfiyControlsEnabled(true, defenceSubmitTable);
			defenceOperationType = Operation.UPDATE;
			modifiedDefenceID = defenceId;

			defenceNameBox.Text = defence.Name;
			defenceFactionBox.SelectedItem = defence.Faction;
			defenceValueBox.Value = (decimal)defence.DefenceValue;
			defenceTypeBox.SelectedItem = defence.SystemType;
			defenceKineticMultBox.Value = (decimal)defence.DefMultAgainstWepTypeMap[WeaponType.KINETIC];
			defenceLaserMultBox.Value = (decimal)defence.DefMultAgainstWepTypeMap[WeaponType.LASER];
			defenceMissileMultBox.Value = (decimal)defence.DefMultAgainstWepTypeMap[WeaponType.MISSILE];
		}

		private void SetDefenceSubmitToDefaultMin() {
			defenceNameBox.Text = "";
			defenceFactionBox.SelectedIndex = 0;
			defenceValueBox.Value = defenceValueBox.Minimum;
			defenceTypeBox.SelectedIndex = 0;
			defenceKineticMultBox.Value = defenceKineticMultBox.Minimum;
			defenceLaserMultBox.Value = defenceLaserMultBox.Minimum;
			defenceMissileMultBox.Value = defenceMissileMultBox.Minimum;
		}

		private void defenceAddButton_Click(object sender, EventArgs e) {
			InitializeDefencesSubmitDropDownLists();

			defenceDescription.Text = "Adding new defence system";

			SetModfiyControlsEnabled(true, defenceSubmitTable);
			defenceOperationType = Operation.ADD;

			SetDefenceSubmitToDefaultMin();
		}

		private void defenceSyncButton_Click(object sender, EventArgs e) {
			GamePacket packet = new GamePacket(OperationType.GET_DEFENCES, new object());
			AdminApp.Connection.Send(packet);
			packet = AdminApp.Connection.GetReceivedPacket();
			AdminApp.GameData.Defences = (List<DefenceSystem>)packet.Packet;
		}

		private void defenceSubmitButton_Click(object sender, EventArgs e) {
			if (AdminFormUtils.ShowConfirmationBox("defence system")) {
				DefenceSystem defenceSystem = new DefenceSystem(defenceNameBox.Text, (Faction)defenceFactionBox.SelectedItem, (double)defenceValueBox.Value, (DefenceSystemType)defenceTypeBox.SelectedItem, (double)defenceKineticMultBox.Value,
						(double)defenceLaserMultBox.Value, (double)defenceMissileMultBox.Value);

				string validationResult = GameValidator.ValidateDefenceSystem(defenceSystem);
				GamePacket packet;
				if (validationResult == GameValidator.OK) {
					if (defenceOperationType == Operation.UPDATE) {
						defenceSystem.Id = modifiedDefenceID;
						packet = new GamePacket(OperationType.UPDATE_DEFENCE, defenceSystem);
					}
					else {
						packet = new GamePacket(OperationType.ADD_DEFENCE, defenceSystem);
					}

					AdminApp.Connection.Send(packet);
					if (CheckSuccessOrFailure()) {
						defenceSyncButton_Click(sender, e);

						//after success return to default min values and disable submit again
						defenceDescription.Text = baseDefenceModifyDesc;
						SetDefenceSubmitToDefaultMin();
						SetModfiyControlsEnabled(false, defenceSubmitTable);
						defenceSubmitButton.Enabled = false;
					}
				}
				else AdminFormUtils.ShowValidationFailedDialog(validationResult);
			}
		}

		public void FixLaserMultForPD() {
			if (defenceTypeBox.SelectedItem != null) {
				if (((DefenceSystemType)defenceTypeBox.SelectedItem) == DefenceSystemType.POINT_DEFENCE) {
					if (defenceLaserMultBox.Value != 0) defenceLaserMultBox.Value = 0;   //point defence won't work on lasers
				}
			}
		}

		private void defenceTypeBox_SelectedIndexChanged(object sender, EventArgs e) {
			FixLaserMultForPD();
		}

		private void defenceLaserMultBox_ValueChanged(object sender, EventArgs e) {
			FixLaserMultForPD();
		}
		#endregion

//=================================================================================================================================================================================

		#region BaseModifiers
		public void InitializeBaseModifiers() {
			SetBaseModifiersValues(AdminApp.GameData.BaseModifiers);
		}

		private void SetBaseModifiersValues(BaseModifiers m) {
			modsAbsoluteFleetMaxSizeBox.Value = m.MaxAbsoluteFleetSize;
			modsBaseFleetSizeBox.Value = m.BaseFleetMaxSize;
			modsExpLossBox.Value = m.ExpForLoss;
			modsExpVictoryBox.Value = m.ExpForVictory;
			modsFleetSizeExpModifierBox.Value = (decimal)m.FleetSizeExpModifier;
			modsKineticRangeBox.Value = (decimal)m.WeaponTypeRangeMultMap[WeaponType.KINETIC];
			modsLaserRangeBox.Value = (decimal)m.WeaponTypeRangeMultMap[WeaponType.LASER];
			modsMissileRangeBox.Value = (decimal)m.WeaponTypeRangeMultMap[WeaponType.MISSILE];
			modsMaxFleetNumberPerPlayerBox.Value = m.MaxFleetsPerPlayer;
			modsMaxShipExpBox.Value = m.MaxShipExp;
			modsMaxShipsPerPlayerBox.Value = m.MaxShipsPerPlayer;
			modsMoneyLossBox.Value = m.MoneyForLoss;
			modsMoneyVictoryBox.Value = m.MoneyForVictory;
			modsNumOfShipInLineBox.Value = m.MaxShipsInLine;
			modsIfKineticBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.KINETIC)];
			modsIfLaserBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.LASER)];
			modsIfMissilesBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.MISSILE)];
			modsPdKineticBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.KINETIC)];
			modsPdMissilesBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.MISSILE)];
			modsShieldKineticBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.SHIELD, WeaponType.KINETIC)];
			modsShieldLaserBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.SHIELD, WeaponType.LASER)];
			modsShieldMissilesBox.Value = (decimal)m.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(DefenceSystemType.SHIELD, WeaponType.MISSILE)];
			modsShipExpModifierBox.Value = (decimal)m.BaseShipStatsExpModifier;
			modsStartingMoneyBox.Value = m.StartingMoney;
		}

		private void modsSubmitButton_Click(object sender, EventArgs e) {
			if (AdminFormUtils.ShowConfirmationBox("Base Modifiers")) {
				Dictionary<WeaponType, double> weaponTypeRangeMultMap = new Dictionary<WeaponType, double>() {
					{ WeaponType.KINETIC, (double)modsKineticRangeBox.Value },
					{ WeaponType.LASER, (double)modsLaserRangeBox.Value },
					{ WeaponType.MISSILE, (double)modsMissileRangeBox.Value }
				};
				Dictionary<Tuple<DefenceSystemType, WeaponType>, double> defTypeToWepTypeMap = new Dictionary<Tuple<DefenceSystemType, WeaponType>, double> {
					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.KINETIC), (double)modsPdKineticBox.Value },
					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.SHIELD, WeaponType.KINETIC), (double)modsShieldKineticBox.Value },
					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.KINETIC),(double)modsIfKineticBox.Value },

					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.LASER), 0.0 },
					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.SHIELD, WeaponType.LASER), (double)modsShieldLaserBox.Value },
					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.LASER), (double)modsIfLaserBox.Value },

					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.POINT_DEFENCE, WeaponType.MISSILE), (double)modsPdMissilesBox.Value },
					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.SHIELD, WeaponType.MISSILE), (double)modsShieldMissilesBox.Value },
					{ new Tuple<DefenceSystemType,WeaponType>(DefenceSystemType.INTEGRITY_FIELD, WeaponType.MISSILE), (double)modsIfMissilesBox.Value }
				};
				BaseModifiers newMods = new BaseModifiers(
					weaponTypeRangeMultMap,
					defTypeToWepTypeMap,
					(double)modsShipExpModifierBox.Value,
					Convert.ToInt32(modsMaxShipsPerPlayerBox.Value),
					Convert.ToInt32(modsStartingMoneyBox.Value),
					Convert.ToInt32(modsExpVictoryBox.Value),
					Convert.ToInt32(modsExpLossBox.Value),
					(double)modsFleetSizeExpModifierBox.Value,
					Convert.ToInt32(modsAbsoluteFleetMaxSizeBox.Value),
					Convert.ToInt32(modsMaxShipExpBox.Value),
					Convert.ToInt32(modsBaseFleetSizeBox.Value),
					Convert.ToInt32(modsNumOfShipInLineBox.Value),
					Convert.ToInt32(modsMaxFleetNumberPerPlayerBox.Value),
					Convert.ToInt32(modsMoneyVictoryBox.Value),
					Convert.ToInt32(modsMoneyLossBox.Value)
				);

				bool warningRes = true;
				if (newMods.WeaponTypeRangeMultMap[WeaponType.MISSILE] > 0) {
					DialogResult warningResult = MessageBox.Show("Missile range multiplier is greater than 0, are you sure you want to submit such value?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
					warningRes = warningResult == DialogResult.Yes;
				}
				if (warningRes) {
					string validationResult = GameValidator.ValidateBaseModifiers(newMods);
					if (validationResult == GameValidator.OK) {
						GamePacket packet = new GamePacket(OperationType.UPDATE_BASE_MODIFIERS, newMods);
						AdminApp.Connection.Send(packet);
						if (CheckSuccessOrFailure()) {
							SyncBaseModifiers();
							InitializeBaseModifiers();
						}
					}
					else AdminFormUtils.ShowValidationFailedDialog(validationResult);
				}
			}
		}

		private void SyncBaseModifiers() {
			GamePacket packet = new GamePacket(OperationType.BASE_MODIFIERS, new object());
			AdminApp.Connection.Send(packet);
			packet = AdminApp.Connection.GetReceivedPacket();
			AdminApp.GameData.BaseModifiers = (BaseModifiers)packet.Packet;
		}
		#endregion

		//=================================================================================================================================================================================

		#region Players
		public void InitializeUsers() {
			InitializeOrderByPropertyComboBox(userOrderBy, typeof(AdminAppPlayer));
			syncUsersButton_Click(null, null);
		}

		private void syncUsersButton_Click(object sender, EventArgs e) {
			GamePacket packet = new GamePacket(OperationType.GET_USERS, new object());
			AdminApp.Connection.Send(packet);
			packet = AdminApp.Connection.GetReceivedPacket();
			AdminApp.AllUsers = (List<AdminAppPlayer>)packet.Packet;
		}

		private void userSearchButton_Click(object sender, EventArgs e) {
			//first clear existing rows
			AdminFormUtils.ClearPreviousSearch(userTable, userButtonToIdMap);

			bool idOrNameSearch = false;
			int idSearch = Convert.ToInt32(userIdSearch.Value);
			string nameSearch = userNameSearch.Text;
			bool isAdminSearch = userIsAdminSearch.Checked;

			IEnumerable<AdminAppPlayer> searchedUsers = AdminApp.AllUsers.AsEnumerable();
			//if id != 0 search by id
			if (idSearch != 0) {
				idOrNameSearch = true;
				searchedUsers = searchedUsers.Where(usr => usr.Id == idSearch);
			}
			//if name not empty search by name
			if (!string.IsNullOrWhiteSpace(nameSearch)) {
				idOrNameSearch = true;
				searchedUsers = searchedUsers.Where(usr => usr.Username.Contains(nameSearch));
			}
			//then search by faction and type if other are not set
			if (!idOrNameSearch) searchedUsers = searchedUsers.Where(usr => usr.IsAdmin == isAdminSearch);

			//lastly order by chosen property
			searchedUsers = OrderListByProperty(userOrderBy, searchedUsers);

			AddUserList(searchedUsers.ToList());
		}

		private void AddUserList(List<AdminAppPlayer> list) {
			foreach (AdminAppPlayer user in list) {
				AddUserToTable(user);
			}
		}

		private void AddUserToTable(AdminAppPlayer user) {
			RowStyle temp = userTable.RowStyles[userTable.RowCount - 1];
			userTable.RowCount++;
			userTable.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));

			Button deactivateButton = new Button() { Text = "Deactivate this user" };
			deactivateButton.Click += new EventHandler(UserDeactivateButton_Click);
			userButtonToIdMap.Add(deactivateButton, user.Id);

			Button modifyButton = new Button() { Text = "Modify this user", AutoSize = true };
			modifyButton.Click += new EventHandler(UserModifyButton_Click);
			userButtonToIdMap.Add(modifyButton, user.Id);

			double winLoseRatio = 0.0;
			if (user.GamesPlayed != 0) winLoseRatio = user.WinLoseRatio;
			userTable.Controls.Add(new Label() { Text = user.Id.ToString() }, 0, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.Username, AutoSize = true }, 1, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.Experience.ToString() }, 2, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.MaxFleetPoints.ToString() }, 3, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.GamesPlayed.ToString() }, 4, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.GamesWon.ToString() }, 5, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = string.Format("{0:N2}%", winLoseRatio * 100.0) }, 6, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.Money.ToString() }, 7, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.IsActive.ToString() }, 8, userTable.RowCount - 1);
			userTable.Controls.Add(new Label() { Text = user.IsAdmin.ToString() }, 9, userTable.RowCount - 1);
			userTable.Controls.Add(modifyButton, 10, userTable.RowCount - 1);
			userTable.Controls.Add(deactivateButton, 11, userTable.RowCount - 1);
		}

		private AdminAppPlayer GetUserWithId(int id) {
			return AdminApp.AllUsers.Where(usr => usr.Id == id).FirstOrDefault();
		}

		private void UserModifyButton_Click(object sender, EventArgs e) {
			int userID = userButtonToIdMap[sender as Button];
			userDescription.Text = baseUserDescription + userID;
			AdminAppPlayer user = GetUserWithId(userID);

			SetModfiyControlsEnabled(true, userSubmitTable);
			userOperationType = Operation.UPDATE;
			modifiedUserID = userID;

			userNameBox.Text = user.Username;
			userNameBox.Enabled = false;
			userPasswordBox.Text = "";
			userPasswordBox.Enabled = false;
			userRepeatPasswordBox.Text = "";
			userRepeatPasswordBox.Enabled = false;
			userExpBox.Value = user.Experience;
			userMoneyBox.Value = user.Money;
			userIsActiveBox.Checked = user.IsActive;
			userIsAdminBox.Checked = user.IsAdmin;
		}

		private void UserDeactivateButton_Click(object sender, EventArgs e) {
			if (AdminFormUtils.ShowConfirmationBox("user - deactivate him")) {
				int userId = userButtonToIdMap[sender as Button];
				AdminAppPlayer player = GetUserWithId(userId);
				GamePacket packet = new GamePacket(OperationType.DEACTIVATE_USER, player);
				AdminApp.Connection.Send(packet);
				if (CheckSuccessOrFailure()) {
					syncUsersButton_Click(sender, e);
				}
			}
		}

		private void userSubmitButton_Click(object sender, EventArgs e) {
			if (AdminFormUtils.ShowConfirmationBox("user")) {
				AdminAppPlayer user = new AdminAppPlayer(userNameBox.Text, userPasswordBox.Text, Convert.ToInt32(userExpBox.Value), 0, 0, 
						0, Convert.ToInt32(userMoneyBox.Value), userIsActiveBox.Checked, userIsAdminBox.Checked);

				bool isNew = userOperationType == Operation.ADD;
				string validationResult = GameValidator.ValidateUser(user, isNew);
				GamePacket packet;
				bool passwordOk = true;
				if (validationResult == GameValidator.OK) {
					if (userOperationType == Operation.UPDATE) {
						AdminAppPlayer temp = GetUserWithId(modifiedUserID);
						user.Id = modifiedUserID;
						user.GamesPlayed = temp.GamesPlayed;
						user.GamesWon = temp.GamesWon;
						packet = new GamePacket(OperationType.UPDATE_USER, user);
					}
					else {
						if (userPasswordBox.Text != userRepeatPasswordBox.Text) passwordOk = false;
						packet = new GamePacket(OperationType.ADD_USER, user);
					}

					if (passwordOk) {
						AdminApp.Connection.Send(packet);
						if (CheckSuccessOrFailure()) {
							syncUsersButton_Click(sender, e);

							//after success return to default min values and disable submit again
							userDescription.Text = baseUserDescription;
							SetUserSubmitToDefaultMin();
							SetModfiyControlsEnabled(false, userSubmitTable);
						}
					}
					else AdminFormUtils.ShowValidationFailedDialog("Repeated password does not match!");
				}
				else AdminFormUtils.ShowValidationFailedDialog(validationResult);
			}
		}

		private void userAddNewButton_Click(object sender, EventArgs e) {
			SetModfiyControlsEnabled(true, userSubmitTable);

			userDescription.Text = "Adding new user";
			userOperationType = Operation.ADD;

			SetUserSubmitToDefaultMin();
			userIsActiveBox.Enabled = false;
		}

		private void SetUserSubmitToDefaultMin() {
			userMoneyBox.Value = AdminApp.GameData.BaseModifiers.StartingMoney;
			userExpBox.Value = userExpBox.Minimum;
			userIsActiveBox.Checked = true;
			userIsAdminBox.Checked = false;
			userNameBox.Text = "";
			userPasswordBox.Text = "";
			userRepeatPasswordBox.Text = "";
		}
		#endregion

	}

}
