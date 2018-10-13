using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Faction {
		private string name;

		public Faction() { }

		public Faction(string name) {
			this.Name = name;
		}

		public string Name { get => name; set => name = value; }
	}
}
