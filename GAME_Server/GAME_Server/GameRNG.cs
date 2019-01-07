using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;
using System.Security.Cryptography;

namespace GAME_Server {
	public class GameRNG {
		private Random rng;
		private RNGCryptoServiceProvider cryptoRng;

		public GameRNG() {
			this.rng = new Random();
			this.cryptoRng = new RNGCryptoServiceProvider();
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

		public Rarity GetRandomRarityWithChances(Dictionary<Rarity, double> chancesFromLootbox) {
			//create a sorted dictionary from values from lootbox to make sure order is preserved
			SortedDictionary<Rarity, double> chances = new SortedDictionary<Rarity, double>(chancesFromLootbox);
			SortedDictionary<Rarity, Tuple<double, double>> roulette = new SortedDictionary<Rarity, Tuple<double, double>>();
			foreach (var pair in chances) roulette.Add(pair.Key, null);	//add keys to roulette
			double lastValue = 0.0;
			foreach (var pair in chances) {	//add ranges (min max) for each rarity
				double tempLastValue = lastValue;
				lastValue += pair.Value;
				roulette[pair.Key] = new Tuple<double, double>(tempLastValue, lastValue);
			}
			double roll = GetRandomDouble();
			Rarity rolledRarity = Rarity.COMMON;
			foreach (var pair in roulette) {	//check in which range the roll is
				if (roll >= pair.Value.Item1 && roll < pair.Value.Item2) rolledRarity = pair.Key;
			}
			return rolledRarity;
		}

		public double GetRandomDouble() {
			return RNG.NextDouble();
		}

		/// <summary>
		/// provides random double in rnage from 0.0 to 1.0
		/// </summary>
		/// <returns></returns>
		public double GetRandomDoubleCrypto() {
			bool littleEndian = BitConverter.IsLittleEndian;
			double retVal;

			byte[] randomBytes = new byte[8];
			cryptoRng.GetBytes(randomBytes);
			//if is littleendian reverse bytes order to normal bigendian
			//if (littleEndian) Array.Reverse(randomBytes);

			BitArray bitArray = new BitArray(randomBytes);
			//according to https://docs.microsoft.com/en-us/dotnet/api/system.double?view=netframework-4.7.2 C# stores double in reversed way
			bitArray.Set(63, false);     //set sign as +
			//next set exponent to 1023 because 2^(1023-1023) = 1 and we want that for the return value to be within 0 and 1
			//we need 01111111111
			bitArray.Set(62, false);    
			for (int i = 61; i > 51; i--) bitArray.Set(i, true);
			bitArray.CopyTo(randomBytes, 0);
			//if is littleendian reverse bytes order to normal bigendian
			//if (littleEndian) Array.Reverse(randomBytes);

			retVal = BitConverter.ToDouble(randomBytes, 0);
			return retVal - 1.0;
		}

	}

}
