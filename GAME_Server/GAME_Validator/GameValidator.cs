using System;
using GAME_connection;

namespace GAME_Validator {
	public static class GameValidator {
		public static readonly string OK = "OK";

		public static bool IsInRange(this int number, int min, int max) {
			return (number >= min && number <= max);
		}

		public static bool IsInRange(this double number, double min, double max) {
			return (number >= min && number <= max);
		}

		public static bool IsInRangeExcludeMin(this double number, double min, double max) {
			return (number > min && number <= max);
		}

		public static string ValidateShip(Ship ship) {
			if (!ship.Evasion.IsInRangeExcludeMin(0.0, 1.0)) return FailureReasons.VALUE_NOT_IN_RANGE + "Evasion";
			if (ship.ExpUnlock < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "Exp Unlock";
			if (ship.Size < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "Size";
			if (string.IsNullOrWhiteSpace(ship.Name)) return FailureReasons.VALUE_NOT_IN_RANGE + "Name";
			if (ship.Hp < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "HP";
			if (ship.Cost < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "Cost";
			if (ship.Armor < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "Armor";
			if (ship.Faction == null) return FailureReasons.VALUE_NOT_IN_RANGE + "Faction";
			if (ship.Weapons == null) return FailureReasons.VALUE_NOT_IN_RANGE + "Faction";
			if (ship.Defences == null) return FailureReasons.VALUE_NOT_IN_RANGE + "Faction";
			if (ship.Defences.Count == 0) return FailureReasons.NO_WEAPONS_OR_DEFENCES;
			if (ship.Weapons.Count == 0) return FailureReasons.NO_WEAPONS_OR_DEFENCES;
			return OK;
		}

	}

	
}
