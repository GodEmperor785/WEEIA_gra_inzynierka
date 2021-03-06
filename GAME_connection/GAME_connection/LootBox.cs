﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class LootBox {
		private int id;
		private int cost;
		private string name;
		private Dictionary<Rarity, double> chancesForRarities;
		private int numberOfShips;

		public LootBox() { }

		public LootBox(int cost, string name, Dictionary<Rarity, double> chancesForRarities, int numberOfShips) : this() {
			this.Cost = cost;
			this.Name = name;
			this.ChancesForRarities = chancesForRarities;
			this.NumberOfShips = numberOfShips;
		}

		public LootBox(int id, int cost, string name, Dictionary<Rarity,double> chancesForRarities, int numberOfShips)
				:this(cost, name, chancesForRarities, numberOfShips) {
			this.Id = id;
		}

		public int Id { get => id; set => id = value; }
		public int Cost { get => cost; set => cost = value; }
		public string Name { get => name; set => name = value; }
		public Dictionary<Rarity, double> ChancesForRarities { get => chancesForRarities; set => chancesForRarities = value; }
		public int NumberOfShips { get => numberOfShips; set => numberOfShips = value; }
	}
}
