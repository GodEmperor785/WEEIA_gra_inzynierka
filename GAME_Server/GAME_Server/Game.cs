﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	public enum Victory {
		NOT_YET,
		PLAYER_1,
		PLAYER_2,
		DRAW
	}

	public class Game {
		public static readonly double MIN_DISTANCE_OFFSET = 1.0;    //offset for minimum distance - SHORT to SHORT
		public static readonly double MAX_DISTANCE = (double)Line.LONG + (double)Line.LONG + MIN_DISTANCE_OFFSET;

		private GameRNG RNG;

		public PlayerGameBoard Player1GameBoard { get; set; }
		public PlayerGameBoard Player2GameBoard { get; set; }
		public GameState Player1GameState {
			get {
				return new GameState(Player1GameBoard, Player2GameBoard);
			}
		}
		public GameState Player2GameState {
			get {
				return new GameState(Player2GameBoard, Player1GameBoard);
			}
		}
		public Move EmptyMove {
			get {
				return new Move();
			}
		}
		public PlayerGameBoard Player1GameBoardReference { get; set; }  //references are used to process move - they hold the state before modification
		public PlayerGameBoard Player2GameBoardReference { get; set; }

		public Game() {
			Player1GameBoard = new PlayerGameBoard();
			Player2GameBoard = new PlayerGameBoard();
			RNG = new GameRNG();
		}

		public Game(PlayerGameBoard player1GameBoard, PlayerGameBoard player2GameBoard) {
			Player1GameBoard = player1GameBoard;
			Player2GameBoard = player2GameBoard;
			RNG = new GameRNG();
		}

		#region player move processing
		public void ProcessPlayer1AttackMove(Move playersMove) {
			this.ProcessAttackMove(playersMove, Player1GameBoardReference, Player2GameBoardReference);
		}

		public void ProcessPlayer2AttackMove(Move playersMove) {
			this.ProcessAttackMove(playersMove, Player2GameBoardReference, Player1GameBoardReference);
		}

		/// <summary>
		/// attack processing should be done before move processing, because game boards need to be updated by attack values before moving
		/// yourBoard and enemyBoard should be REFERENCE PlayerGameBoards!
		/// </summary>
		/// <param name="move"></param>
		/// <param name="yourBoard"></param>
		/// <param name="enemyBoard"></param>
		private void ProcessAttackMove(Move move, PlayerGameBoard yourBoard, PlayerGameBoard enemyBoard) {
			foreach(var attackOrder in move.AttackList) {
				Server.Log("Attack from: " + attackOrder.Item1.Line + " " + attackOrder.Item1.ShipIndex + "  to: " + attackOrder.Item2.Line + " " + attackOrder.Item2.ShipIndex);
				//get affected ships
				Ship attackingShip = yourBoard.Board[attackOrder.Item1.Line][attackOrder.Item1.ShipIndex];
				Ship defendingShip = enemyBoard.Board[attackOrder.Item2.Line][attackOrder.Item2.ShipIndex];
				//if target ship is already destroyed (set to null) dont co anything
				if (defendingShip != null) {
					//calculate distance  
					double distance = (double)attackOrder.Item1.Line + (double)attackOrder.Item2.Line + MIN_DISTANCE_OFFSET;
					bool shipDestroyed = false;
					//start attack loop for each weapon on attacking ship
					foreach (Weapon weapon in attackingShip.Weapons) {
						//for each projectile of this weapon
						for (int i = 0; i < weapon.NumberOfProjectiles; i++) {
							//if current projectile hits its target
							bool dmgBlockedByDefences = false;
							if (WeaponProjectileHit(weapon, distance, defendingShip)) {
								double dmg = weapon.Damage;
								//decrease weapon dmg by defence systems of defending ship
								foreach (DefenceSystem defenceSystem in defendingShip.Defences) {
									if (weapon.WeaponType == WeaponType.LASER && defenceSystem.SystemType == DefenceSystemType.POINT_DEFENCE) { //point defence wont fire on lasers!
										double baseDefValueMult = Server.BaseModifiers.DefTypeToWepTypeMap[new Tuple<DefenceSystemType, WeaponType>(defenceSystem.SystemType, weapon.WeaponType)];
										double systemDefValueMult = defenceSystem.DefMultAgainstWepTypeMap[weapon.WeaponType];
										double dmgThatCanBeMigitated = defenceSystem.DefenceValueLeft * baseDefValueMult * systemDefValueMult;
										if (dmg > dmgThatCanBeMigitated) {  //weapon is too strong for this defence system to block it - use all def value left to block
											dmg -= dmgThatCanBeMigitated;
											defenceSystem.DefenceValueLeft = 0.0;
										}
										else {  //weapon weaker - this defence system blocks all dmg from this projectile - exit defence loop
											dmg = 0.0;
											dmgBlockedByDefences = true;
											defenceSystem.DefenceValueLeft -= (dmg / (baseDefValueMult * systemDefValueMult));
											break;
										}
									}
								}
								if (!dmgBlockedByDefences) {
									//decrease dmg with ship armor - weapons AP vs armor
									double minDamagePossibleForProjectile = 0.1;
									double APvsArmor = weapon.ApEffectivity / defendingShip.Armor;
									//damage after armor cannot be greater than before armor
									if (APvsArmor <= 1) dmg *= APvsArmor;
									//dmg cannot be less than min value if it passed defences
									if (dmg < minDamagePossibleForProjectile) dmg = minDamagePossibleForProjectile;
									defendingShip.Hp -= dmg;
									//if ship destroyed stop calculations for this move and mark it as destroyed
									if (defendingShip.Hp <= 0) {
										shipDestroyed = true;
										break;
									}
								}
							}
						}
						//if ship destroyed exit the weapon loop
						if (shipDestroyed) break;
					}
					//if ship was destroyed set it to null
					if (shipDestroyed) enemyBoard.Board[attackOrder.Item2.Line][attackOrder.Item2.ShipIndex] = null;
				}
			}
		}

		private bool WeaponProjectileHit(Weapon weapon, double distance, Ship targetShip) {
			//equation 1: ((baseChance + weapon.chance)/(div=2)) - (dist * weaponTypeMult * weaponMult * (evasion/40))
			//max(chanceToHit) = 1 - min = 0.5, max(dist) = 5 - min = 1, max(WeaponTypeRangeMult) = 1.0 - min = 0.0, max(weapon.RangeMultiplier) = 2 - min = 1.0, max(evasion) = 2 - min = 1
			//double baseChanceToHit = 1.0;
			//double div = 2.0;
			//double chanceToHit = ((weapon.ChanceToHit + baseChanceToHit) / div) - (distance * Server.BaseModifiers.WeaponTypeRangeMultMap[weapon.WeaponType] * weapon.RangeMultiplier * (targetShip.Evasion / 40.0));

			//equation 2: ( weapon.chance - ( (sqrt(dist)/max(dist)) * weaponTypeMult * weaponMult * evasion) )
			//weapon.chance <0.0 - 1.0>, dist <1 - 5>, weaponTypeMult <0.0 - 1.0>, weaponMult <0.0 - 1.0>, evasion <0.0 - 1.0>
			//all mults and evasion indicate how important they are - 0 not at all, 1 - maximally
			//ex. missiles have mults at 0 because they track targets, kinetics have high mults because they are slow and unguided, lasers have medium mults because they are fastest but unguided
			//evasion indicates how good a ship is at evading attacks
			double chanceToHit = (weapon.ChanceToHit - ( (Math.Sqrt(distance)/MAX_DISTANCE) * Server.BaseModifiers.WeaponTypeRangeMultMap[weapon.WeaponType] * weapon.RangeMultiplier * targetShip.Evasion));

			//chanceToHit has to be at least this
			double minChanceToHit = 0.01;
			chanceToHit = Math.Max(chanceToHit, minChanceToHit);
			//finally roll to determine if projectile hit its target
			return RNG.RollWithChance(chanceToHit);
		}

		public void ProcessPlayer1MoveOrders(Move playersMove) {
			this.ProcessMoveOrders(playersMove, Player1GameBoardReference);
		}

		public void ProcessPlayer2MoveOrders(Move playersMove) {
			this.ProcessMoveOrders(playersMove, Player2GameBoardReference);
		}

		/// <summary>
		/// move processing should be done AFTER processing attack, because it can only move surviving ships
		/// yourBoard should be a REFERENCE PlayerGameBoard!
		/// </summary>
		/// <param name="move"></param>
		/// <param name="yourBoard"></param>
		private void ProcessMoveOrders(Move move, PlayerGameBoard yourBoard) {
			foreach (var moveOrder in move.MoveList) {
				if (yourBoard.Board[moveOrder.Item1.Line][moveOrder.Item1.ShipIndex] != null) {
					Server.Log("Move from: " + moveOrder.Item1.Line + " " + moveOrder.Item1.ShipIndex + "  to: " + moveOrder.Item2 + "  ship_id: " + yourBoard.Board[moveOrder.Item1.Line][moveOrder.Item1.ShipIndex].Id);
					//first add ship to destination line
					yourBoard.Board[moveOrder.Item2].Add(yourBoard.Board[moveOrder.Item1.Line][moveOrder.Item1.ShipIndex]);
					//than remove ship from origin line
					yourBoard.Board[moveOrder.Item1.Line].RemoveAt(moveOrder.Item1.ShipIndex);
				}
				else Server.Log("Ship at: " + moveOrder.Item1.Line + " " + moveOrder.Item1.ShipIndex + "  was destroyed");
			}
		}
		#endregion

		#region setting ship states
		public void SetShipStatesForPlayer1(Move move) {
			SetShipStates(move, Player1GameBoard);
		}

		public void SetShipStatesForPlayer2(Move move) {
			SetShipStates(move, Player2GameBoard);
		}

		private void SetShipStates(Move move, PlayerGameBoard processedBoard) {
			//first set all to defence
			foreach (var line in processedBoard.Board) {
				foreach (var ship in line.Value) {
					ship.State = ShipState.DEFENDING;
				}
			}
			//than set states accordingly to players move - start with MoveList
			foreach (var moveOrder in move.MoveList) {
				processedBoard.Board[moveOrder.Item1.Line][moveOrder.Item1.ShipIndex].State = ShipState.MOVING;
			}
			//than process AttackList
			foreach (var attackOrder in move.AttackList) {
				processedBoard.Board[attackOrder.Item1.Line][attackOrder.Item1.ShipIndex].State = ShipState.ATTACKING;
			}

			//add defenceValueLeft (in this turn) to defences of ships
			double stateDefMult = 2.0;
			foreach (var line in processedBoard.Board) {
				foreach (var ship in line.Value) {
					foreach (var def in ship.Defences) {
						//if ship is in DEFENDING state it has better defence value
						if (ship.State == ShipState.DEFENDING) def.DefenceValueLeft = stateDefMult * def.DefenceValue;
						else def.DefenceValueLeft = def.DefenceValue;
						//bigger ships have better defences
						def.DefenceValueLeft *= ship.Size;
					}
				}
			}
			//sort weapon and defence system lists
			foreach (var line in processedBoard.Board) {
				foreach (var ship in line.Value) {
					//before attack calculation order list by WeaponType value (laser faster than kinetics faster than missiles) and fire 'swarm' weapons first and fire weaker first
					ship.Weapons = ship.Weapons.OrderBy(wep => (int)wep.WeaponType).ThenByDescending(wep => wep.NumberOfProjectiles).ThenBy(wep => wep.Damage).ToList();
					//before attack calculation oreder list by DefenceSystemType value (PD longer range than shield longer range than IF) and fire weaker defences first
					ship.Defences = ship.Defences.OrderBy(def => (int)def.SystemType).ThenBy(def => def.DefenceValue).ToList();
				}
			}
		}
		#endregion

		#region open and finalize move
		public void OpenTurn() {
			//clone current player game boards to references
			this.Player1GameBoardReference = Server.CloneObject(Player1GameBoard);
			this.Player2GameBoardReference = Server.CloneObject(Player2GameBoard);
		}

		public void FinalizeMove() {
			//first remove ships that are nulls - they were destroyed by other players attacks
			foreach(var line in Player1GameBoardReference.Board) {
				line.Value.RemoveAll(ship => ship == null);
			}
			foreach (var line in Player2GameBoardReference.Board) {
				line.Value.RemoveAll(ship => ship == null);
			}
			this.Player1GameBoard = Player1GameBoardReference;
			this.Player2GameBoard = Player2GameBoardReference;
		}
		#endregion

		public Victory CheckGameEndResult() {
			bool player1HasNoShips = Player1GameBoard.PlayerHasNoShips();
			bool player2HasNoShips = Player2GameBoard.PlayerHasNoShips();

			if (player1HasNoShips && player2HasNoShips) return Victory.DRAW;
			else if (player1HasNoShips) return Victory.PLAYER_2;
			else if (player2HasNoShips) return Victory.PLAYER_1;
			else return Victory.NOT_YET;
		}

	}
}