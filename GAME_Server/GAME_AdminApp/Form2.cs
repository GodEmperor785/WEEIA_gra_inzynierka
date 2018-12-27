using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GAME_connection;
using GAME_Validator;

namespace GAME_AdminApp {
	public enum Operation {
		ADD,
		UPDATE,
		NONE
	}

	public partial class AdminForm : Form {
		private Dictionary<Button, int> buttonToIdMap = new Dictionary<Button, int>();
		private string baseShipModifyDesc;

		private Operation operationType = Operation.NONE;
		private int modifiedShipId;

		public ShipWeaponsForm WeaponsForm { get; set; }

		public List<Weapon> ShipWeapons { get; set; }
		public List<DefenceSystem> ShipDefences { get; set; }

		public AdminForm() {
			InitializeComponent();

			shipIdSearchBox.Maximum = decimal.MaxValue;
			baseShipModifyDesc = shipDescriptionLabel.Text;
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

		#region Ship Templates
		public void InitializeShipDropDownLists() {
			shipFaction.DataSource = AdminApp.GameData.Factions;
			shipFaction.DisplayMember = "Name";
			shipFaction.SelectedIndex = 0;
		}

		private void InitializeShipSubmitDropDownLists() {
			shipFactionBox.DataSource = AdminApp.GameData.Factions;
			shipFactionBox.DisplayMember = "Name";
			shipFactionBox.SelectedIndex = 0;

			shipRarityBox.DataSource = Enum.GetValues(typeof(Rarity));
			shipRarityBox.SelectedIndex = 0;
		}

		/// <summary>
		/// exit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, EventArgs e) {
			DialogResult exitResult = MessageBox.Show("Do you want to exit application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			if(exitResult == DialogResult.Yes) {
				AdminApp.ExitApp();
			}
		}

		private void shipSearchButton_Click(object sender, EventArgs e) {
			//first clear existing rows
			AdminFormUtils.ClearPreviousSearch(shipTable, buttonToIdMap);

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

			//lastly display results
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
			buttonToIdMap.Add(weaponsButton, ship.Id);

			Button defencesButton = new Button() { Text = "Defences" };
			defencesButton.Click += new EventHandler(DefencesButton_Click);
			buttonToIdMap.Add(defencesButton, ship.Id);

			Button modifyButton = new Button() { Text = "Modify this template", AutoSize = true };
			modifyButton.Click += new EventHandler(ModifyButton_Click);
			buttonToIdMap.Add(modifyButton, ship.Id);

			shipTable.Controls.Add(new Label() { Text = ship.Id.ToString()}, 0, shipTable.RowCount - 1);
			shipTable.Controls.Add(new Label() { Text = ship.Name }, 1, shipTable.RowCount - 1);
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

		private void WeaponsButton_Click(object sender, EventArgs e) {
			int shipID = buttonToIdMap[sender as Button];
			Ship ship = GetShipWithID(shipID);
			ShipWeaponsForm form = new ShipWeaponsForm();
			List<Weapon> shipWeapons = AdminApp.GameData.ShipTemplates.Where(s => s.Id == shipID).Select(s => s.Weapons).FirstOrDefault(); 
			form.SetForViewOnly(ship, shipWeapons);
			form.Show();
		}

		private void DefencesButton_Click(object sender, EventArgs e) {
			int shipID = buttonToIdMap[sender as Button];
			
		}

		private void ModifyButton_Click(object sender, EventArgs e) {
			int shipID = buttonToIdMap[sender as Button];
			shipDescriptionLabel.Text = baseShipModifyDesc + shipID;
			Ship ship = GetShipWithID(shipID);

			SetModfiyControlsEnabled(true);
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
			shipDescriptionLabel.Text = "Adding new ship template";

			SetModfiyControlsEnabled(true);
			operationType = Operation.ADD;

			shipNameBox.Text = "";
			shipFactionBox.SelectedIndex = 0;
			shipCostBox.Value = 1;
			shipEvasionBox.Value = 0.001M;
			shipHpBox.Value = 1;
			shipSizeBox.Value = 1;
			shipArmorBox.Value = 0;
			shipExpUnlockBox.Value = 0;
			shipRarityBox.SelectedIndex = 0;
			ShipWeapons = new List<Weapon>();
			ShipDefences = new List<DefenceSystem>();
		}

		private void SetModfiyControlsEnabled(bool enabled) {
			foreach(Control control in submitTableLayout.Controls) {
				control.Enabled = enabled;
			}
			if(enabled) InitializeShipSubmitDropDownLists();
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
					if(CheckSuccessOrFailure()) shipSyncButton_Click(sender, e);
				}
				else AdminFormUtils.ShowValidationFailedDialog(validationResult);
			}
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
			//TODO
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
				form.Show();
			}
			else {
				int shipID = modifiedShipId;
				Ship ship = GetShipWithID(shipID);
				string description = "Weapons for ship with ID: " + shipID;

				form.SetForModify(ship, ShipWeapons);
				form.Show();
			}
		}
		#endregion

		#region Weapons

		#endregion

		#region Defences

		#endregion

		#region BaseModifiers

		#endregion

		#region Players

		#endregion
	}

}
