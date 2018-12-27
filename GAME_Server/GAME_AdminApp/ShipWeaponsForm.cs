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
	public partial class ShipWeaponsForm : Form {
		private Dictionary<CheckBox, int> checkBoxToIdMap = new Dictionary<CheckBox, int>();

		public bool IsModify { get; set; }
		public int ModifyId { get; set; }

		public ShipWeaponsForm() {
			InitializeComponent();
		}

		public void SetForViewOnly(Ship ship, List<Weapon> selectedWeapons) {
			ShowWeaponsOfShip(ship, selectedWeapons);
			SetEnabled(false);
			saveButton.Enabled = false;
		} 

		public void SetForModify(Ship ship, List<Weapon> selectedWeapons) {
			ShowWeaponsOfShip(ship, selectedWeapons);
		}

		public void SetForNew(Faction chosenFaction, List<Weapon> selectedWeapons) {
			ShowAllWeaponsOfFactionPlusSelected(chosenFaction, selectedWeapons);
		}

		private void ShowAllWeaponsOfFactionPlusSelected(Faction shipFaction, List<Weapon> selectedWeapons) {
			List<Weapon> weapons = AdminApp.GameData.Weapons.Where(wep => wep.Faction.Id == shipFaction.Id).ToList();
			AdminFormUtils.AddWeaponListForShip(weaponTable, weapons, checkBoxToIdMap);
			foreach (var pair in checkBoxToIdMap) {
				if (selectedWeapons.Any(w => w.Id == pair.Value)) pair.Key.Checked = true; //check present weapons
			}
		}

		private void ShowWeaponsOfShip(Ship ship, List<Weapon> selectedWeapons) {
			List<Weapon> weapons = AdminApp.GameData.Weapons.Where(wep => wep.Faction.Id == ship.Faction.Id).ToList();
			AdminFormUtils.AddWeaponListForShip(weaponTable, weapons, checkBoxToIdMap);
			foreach(var pair in checkBoxToIdMap) {
				if (selectedWeapons.Any(wep => wep.Id == pair.Value)) pair.Key.Checked = true;	//check present weapons
			}
		}

		private void SetEnabled(bool state) {
			foreach(Control control in weaponTable.Controls) {
				control.Enabled = state;
			}
		}

		private void backButton_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void saveButton_Click(object sender, EventArgs e) {
			List<Weapon> chosenWeapons = new List<Weapon>();
			foreach (var pair in checkBoxToIdMap) {
				if (pair.Key.Checked) chosenWeapons.Add( AdminApp.GameData.Weapons.Where(wep => wep.Id == pair.Value).FirstOrDefault() );
			}
			AdminApp.AppForm.ShipWeapons = chosenWeapons;
			/*if (IsModify) {
				Ship modifiedShip = AdminApp.GameData.ShipTemplates.Where(s => s.Id == ModifyId).FirstOrDefault();
				modifiedShip.Weapons = chosenWeapons;
			}*/
			this.Close();
		}

	}

}
