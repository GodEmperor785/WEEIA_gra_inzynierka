using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_AdminApp {
	public static class AdminFormUtils {
		public static void ClearPreviousSearch(TableLayoutPanel table, Dictionary<Button, int> buttonToIdMap) {
			//start from last row
			for (int row = table.RowCount - 1; row > 0; row--) {
				//first remove controls in processed row
				for (int col = 0; col < table.ColumnCount; col++) {
					var control = table.GetControlFromPosition(col, row);
					if (control is Button) buttonToIdMap.Remove((Button)control);
					table.Controls.Remove(control);
				}
				//than remove the row
				table.RowStyles.RemoveAt(row);
				table.RowCount--;
			}
		}

		public static void ShowValidationFailedDialog(string msg) {
			MessageBox.Show("Validation failed with message: " + msg, "Validation failed!");
		}

		public static bool ShowConfirmationBox(string opType) {
			DialogResult result = MessageBox.Show("Are you sure you want to submit changes in this " + opType + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
			if (result == DialogResult.Yes) {
				return true;
			}
			else return false;
		}

		public static void AddWeaponListForShip(TableLayoutPanel table, List<Weapon> weapons, Dictionary<CheckBox, int> checkBoxMap) {
			foreach (Weapon wep in weapons) AddWeaponToTable(wep, table, checkBoxToIdMap:checkBoxMap);
		}

		public static void AddWeaponListForWeapons(TableLayoutPanel table, List<Weapon> weapons, Dictionary<Button, int> buttonToIdMap, EventHandler modifyButtonHandler) {
			foreach (Weapon wep in weapons) AddWeaponToTable(wep, table, true, buttonToIdMap, modifyButtonHandler);
		}

		private static void AddWeaponToTable(Weapon weapon, TableLayoutPanel table, bool addModifyButton = false, Dictionary<Button, int> buttonToIdMap = null, EventHandler modifyButtonHandler = null, Dictionary<CheckBox, int> checkBoxToIdMap = null) {
			RowStyle temp = table.RowStyles[table.RowCount - 1];
			table.RowCount++;
			table.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));

			table.Controls.Add(new Label() { Text = weapon.Id.ToString() }, 0, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.Name, AutoSize = true }, 1, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.Faction.Name }, 2, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.Damage.ToString() }, 3, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.NumberOfProjectiles.ToString() }, 4, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.WeaponType.GetWeaponTypyName() }, 5, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.ApEffectivity.ToString() }, 6, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.RangeMultiplier.ToString() }, 7, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = weapon.ChanceToHit.ToString() }, 8, table.RowCount - 1);
			if (addModifyButton) {
				Button modifyButton = new Button() { Text = "Modify this weapon", AutoSize = true };
				modifyButton.Click += new EventHandler(modifyButtonHandler);
				buttonToIdMap.Add(modifyButton, weapon.Id);
				table.Controls.Add(modifyButton, 9, table.RowCount - 1);
			}
			else {
				CheckBox selectThisWeapon = new CheckBox();
				checkBoxToIdMap.Add(selectThisWeapon, weapon.Id);
				table.Controls.Add(selectThisWeapon, 9, table.RowCount - 1);
			}
		}

		public static void AddDefenceSystemListForShip(TableLayoutPanel table, List<DefenceSystem> defences, double shpSize, Dictionary<CheckBox, int> checkBoxMap) {
			foreach (DefenceSystem def in defences) AddDefenceSystemToTable(def, table, shipSize:shpSize, checkBoxToIdMap:checkBoxMap);
		}

		public static void AddDefenceSystemListForDefences(TableLayoutPanel table, List<DefenceSystem> defences, Dictionary<Button, int> buttonToIdMap, EventHandler modifyButtonHandler) {
			foreach (DefenceSystem def in defences) AddDefenceSystemToTable(def, table, true, buttonToIdMap, modifyButtonHandler);
		}

		private static void AddDefenceSystemToTable(DefenceSystem defence, TableLayoutPanel table, bool addModifyButton = false, Dictionary<Button, int> buttonToIdMap = null, EventHandler modifyButtonHandler = null, double shipSize = 1, 
				Dictionary<CheckBox, int> checkBoxToIdMap = null) {
			RowStyle temp = table.RowStyles[table.RowCount - 1];
			table.RowCount++;
			table.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));

			int chg = 4;

			table.Controls.Add(new Label() { Text = defence.Id.ToString() }, 0, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = defence.Name, AutoSize = true }, 1, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = defence.Faction.Name }, 2, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = defence.DefenceValue.ToString() }, 3, table.RowCount - 1);
			if (!addModifyButton) {
				table.Controls.Add(new Label() { Text = (defence.DefenceValue * shipSize).ToString() }, chg, table.RowCount - 1);
				chg++;
			}
			table.Controls.Add(new Label() { Text = defence.SystemType.GetDefenceSystemTypeName() }, chg, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = defence.DefMultAgainstWepTypeMap[WeaponType.KINETIC].ToString() }, chg + 1, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = defence.DefMultAgainstWepTypeMap[WeaponType.LASER].ToString() }, chg + 2, table.RowCount - 1);
			table.Controls.Add(new Label() { Text = defence.DefMultAgainstWepTypeMap[WeaponType.MISSILE].ToString() }, chg + 3, table.RowCount - 1);
			if (addModifyButton) {
				Button modifyButton = new Button() { Text = "Modify this defence", AutoSize = true };
				modifyButton.Click += new EventHandler(modifyButtonHandler);
				buttonToIdMap.Add(modifyButton, defence.Id);
				table.Controls.Add(modifyButton, chg + 4, table.RowCount - 1);
			}
			else {
				CheckBox selectThisDefence = new CheckBox();
				checkBoxToIdMap.Add(selectThisDefence, defence.Id);
				table.Controls.Add(selectThisDefence, chg + 4, table.RowCount - 1);
			}
		}

	}

}
