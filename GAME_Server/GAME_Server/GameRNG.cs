using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;

namespace GAME_Server {
	public class GameRNG {
		private Random rng;

		public GameRNG() {
			this.rng = new Random();
		}

		internal Random RNG { get => rng; }

		/// <summary>
		/// returns if roll is succesful. Generates random double from 0.0 to 1.0 and cheks if it is lower or equal to chance. Chance must be lower or equal 1.0
		/// </summary>
		/// <param name="chance"></param>
		/// <returns></returns>
		public bool RollWithChance(double chance) {
			double roll = RNG.NextDouble();
			if (roll <= chance) return true;
			else return false;
		}

		public Rarity GetRandomRarityWithChances(Dictionary<Rarity, double> chances) {
			foreach (Rarity rarity in GameEnumUtils.GetValues<Rarity>()) {  //for each rarity
				double chanceForRarity = chances[rarity];
				if (RollWithChance(chanceForRarity)) {   //if roll succesful
					return rarity;
				}
			}
			//if not returned yet than return COMMON
			return Rarity.COMMON;
		}

		public double GEtRandomDouble() {
			return RNG.NextDouble();
		}

	}
}
