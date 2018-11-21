using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class Faction {
		private int id;
		private string name;

		public Faction() { }

		public Faction(int id, string name) {
			this.Id = id;
			this.Name = name;
		}

		public string Name { get => name; set => name = value; }
		public int Id { get => id; set => id = value; }



		public override bool Equals(object obj) {
			Faction castObj = obj as Faction;
			if (castObj == null) return false;
			return (Id == castObj.Id && Name == castObj.Name);
		}

		public override string ToString() {
			return Name;
		}
	}
}
