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

namespace GAME_AdminApp {
	public partial class ShipDefencesForm : Form {
		private Dictionary<CheckBox, int> checkBoxToIdMap = new Dictionary<CheckBox, int>();

		public ShipDefencesForm() {
			InitializeComponent();
		}

		public void SetForViewOnly(Ship ship, List<DefenceSystem> selectedDefences, double shipSize) {
			shipDescriptionLabel.Text = "Viewing possible (faction: " + ship.Faction.Name + ") defence systems for ship with ID: " + ship.Id + ", equipped defences are checked. View is read-only.";
			ShowDefencesOfShip(ship, selectedDefences, shipSize);
			SetEnabled(false);
			saveButton.Enabled = false;
		}

		public void SetForModify(Ship ship, List<DefenceSystem> selectedDefences, double shipSize) {
			shipDescriptionLabel.Text = "Modifying defence systems of ship with ID: " + ship.Id + ", equipped defences are checked. Defence systems of faction " + ship.Faction.Name + " are available.";
			ShowDefencesOfShip(ship, selectedDefences, shipSize);
		}

		public void SetForNew(Faction chosenFaction, List<DefenceSystem> selectedDefences, double shipSize) {
			shipDescriptionLabel.Text = "Modifying defence systems for new ship of faction " + chosenFaction.Name + ". Already selected defences are checked";
			ShowAllDefencesOfFactionPlusSelected(chosenFaction, selectedDefences, shipSize);
		}

		private void ShowAllDefencesOfFactionPlusSelected(Faction shipFaction, List<DefenceSystem> selectedWeapons, double shipSize) {
			List<DefenceSystem> defences = AdminApp.GameData.Defences.Where(def => def.Faction.Id == shipFaction.Id).ToList();
			AdminFormUtils.AddDefenceSystemListForShip(defenceTable, defences, shipSize, checkBoxToIdMap);
			foreach (var pair in checkBoxToIdMap) {
				if (selectedWeapons.Any(w => w.Id == pair.Value)) pair.Key.Checked = true; //check present weapons
			}
		}

		private void ShowDefencesOfShip(Ship ship, List<DefenceSystem> selectedWeapons, double shipSize) {
			List<DefenceSystem> defences = AdminApp.GameData.Defences.Where(def => def.Faction.Id == ship.Faction.Id).ToList();
			AdminFormUtils.AddDefenceSystemListForShip(defenceTable, defences, shipSize, checkBoxToIdMap);
			foreach (var pair in checkBoxToIdMap) {
				if (selectedWeapons.Any(wep => wep.Id == pair.Value)) pair.Key.Checked = true;  //check present weapons
			}
		}

		private void SetEnabled(bool state) {
			foreach (Control control in defenceTable.Controls) {
				control.Enabled = state;
			}
		}

		private void saveButton_Click(object sender, EventArgs e) {
			List<DefenceSystem> chosenDefences = new List<DefenceSystem>();
			foreach (var pair in checkBoxToIdMap) {
				if (pair.Key.Checked) chosenDefences.Add(AdminApp.GameData.Defences.Where(def => def.Id == pair.Value).FirstOrDefault());
			}
			AdminApp.AppForm.ShipDefences = chosenDefences;
			this.Close();
		}

		private void backButton_Click(object sender, EventArgs e) {
			this.Close();
		}
	}
}
