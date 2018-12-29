using System;
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

		public static string ValidateBaseModifiers(BaseModifiers mods) {
			if (mods.BaseFleetMaxSize <= 0 || mods.BaseFleetMaxSize > mods.MaxAbsoluteFleetSize) return FailureReasons.VALUE_NOT_IN_RANGE + "base fleet max size";
			if (!mods.BaseShipStatsExpModifier.IsInRangeExcludeMin(0.0, 1.0)) return FailureReasons.VALUE_NOT_IN_RANGE + "ship exp modifier";
			if (mods.DefTypeToWepTypeMap.Count != 9) return FailureReasons.VALUE_NOT_IN_RANGE + "wrong number of values in def to wep type multipliers";
			foreach (var entry in mods.DefTypeToWepTypeMap) {
				if(entry.Value < 0) return FailureReasons.VALUE_NOT_IN_RANGE + entry.Key.Item1.GetDefenceSystemTypeName() + " to " + entry.Key.Item2.GetWeaponTypyName() + " multiplier";
			}
			if (mods.ExpForLoss < 0 || mods.ExpForLoss > mods.ExpForVictory) return FailureReasons.VALUE_NOT_IN_RANGE + "exp for loss";
			if (mods.ExpForVictory < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "exp for victory";
			if (mods.FleetSizeExpModifier < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "fleet size exp modifier";
			if (mods.MaxAbsoluteFleetSize <= 0) return FailureReasons.VALUE_NOT_IN_RANGE + "max absolute fleet size";
			if (mods.MaxFleetsPerPlayer <= 0) return FailureReasons.VALUE_NOT_IN_RANGE + "max fleets per player";
			if (mods.MaxShipExp < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "max ship exp";
			if (mods.MaxShipsInLine <= 0) return FailureReasons.VALUE_NOT_IN_RANGE + "max ships in line";
			if (mods.MaxShipsPerPlayer <= 0) return FailureReasons.VALUE_NOT_IN_RANGE + "max ships per player";
			if (mods.MoneyForLoss < 0 || mods.MoneyForLoss > mods.MoneyForVictory) return FailureReasons.VALUE_NOT_IN_RANGE + "money for loss";
			if (mods.MoneyForVictory < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "money for victory";
			if (mods.StartingMoney < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "starting money";
			if (mods.WeaponTypeRangeMultMap.Count != 3) return FailureReasons.VALUE_NOT_IN_RANGE + "wrong number of values in range multipliers";
			foreach (var entry in mods.WeaponTypeRangeMultMap) {
				if (entry.Value < 0) return FailureReasons.VALUE_NOT_IN_RANGE + entry.Key.GetWeaponTypyName() + " range multiplier";
			}
			return OK;
		}

		public static string ValidateUser(AdminAppPlayer user, bool isNew) {
			if (string.IsNullOrWhiteSpace(user.Username) && isNew) return FailureReasons.VALUE_NOT_IN_RANGE + "username";
			if (string.IsNullOrWhiteSpace(user.Password) && isNew) return FailureReasons.VALUE_NOT_IN_RANGE + "password";
			if (user.IsActive == false && isNew) return FailureReasons.VALUE_NOT_IN_RANGE + "is active";
			if (user.Money < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "money";
			if (user.Experience < 0) return FailureReasons.VALUE_NOT_IN_RANGE + "experience";
			return OK;
		}

	}
	
}
