﻿using System;
using System.Linq;
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
			if (ship.Weapons == null) return FailureReasons.VALUE_NOT_IN_RANGE + "Weapons";
			if (ship.Defences == null) return FailureReasons.VALUE_NOT_IN_RANGE + "Defences";
			if (ship.Defences.Count == 0) return FailureReasons.NO_WEAPONS_OR_DEFENCES;
			if (ship.Weapons.Count == 0) return FailureReasons.NO_WEAPONS_OR_DEFENCES;
			if (!ship.Weapons.All(wep => wep.Faction.Id == ship.Faction.Id)) return FailureReasons.FACTION_MUST_BE_SAME;
			if (!ship.Defences.All(def => def.Faction.Id == ship.Faction.Id)) return FailureReasons.FACTION_MUST_BE_SAME;
			return OK;
		}

		public static string ValidateWeapon(Weapon weapon) {
			if (weapon.ApEffectivity <= 0) return FailureReasons.VALUE_NOT_IN_RANGE + "AP Effectivity";
			if (!weapon.ChanceToHit.IsInRange(0.0, 1.0)) return FailureReasons.VALUE_NOT_IN_RANGE + "Chance to hit";
			if (weapon.Damage < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "Damage";
			if (weapon.Faction == null) return FailureReasons.VALUE_NOT_IN_RANGE + "Faction";
			if (string.IsNullOrWhiteSpace(weapon.Name)) return FailureReasons.VALUE_NOT_IN_RANGE + "Name";
			if (weapon.NumberOfProjectiles <= 0) return FailureReasons.VALUE_NOT_IN_RANGE + "Number of projectiles";
			if (!weapon.RangeMultiplier.IsInRange(0.0, 1.0)) return FailureReasons.VALUE_NOT_IN_RANGE + "Range multiplier";
			return OK;
		}

		public static string ValidateDefenceSystem(DefenceSystem defence) {
			if (defence.DefenceValue < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "Defence value";
			if (defence.DefMultAgainstWepTypeMap[WeaponType.KINETIC] < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "kinetic multilpier";
			if (defence.DefMultAgainstWepTypeMap[WeaponType.LASER] != 0 && defence.SystemType == DefenceSystemType.POINT_DEFENCE) return FailureReasons.VALUE_NOT_IN_RANGE + "laser multilpier for PD";
			if (defence.DefMultAgainstWepTypeMap[WeaponType.LASER] < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "laser multiplier";
			if (defence.DefMultAgainstWepTypeMap[WeaponType.MISSILE] < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "missile multiplier";
			if (defence.Faction == null) return FailureReasons.VALUE_NOT_IN_RANGE + "Faction";
			if (string.IsNullOrWhiteSpace(defence.Name)) return FailureReasons.VALUE_NOT_IN_RANGE + "Name";
			return OK;
		}

	}

	
}