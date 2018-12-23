using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class DefenceSystem {
		private int id;
		private string name;
		private Faction faction;
		private double defenceValue;    //how much damage can be blocked (base value modified by modifers)
		private DefenceSystemType systemType;
		private Dictionary<WeaponType, double> defMultAgainstWepTypeMap;    //added to base modifers (stored in DB)
		private double defenceValueLeft;		//used by server for calculation how much defence value is left in this turn, *= ship.Size

		public DefenceSystem() { }

		public DefenceSystem(int id, string name, Faction faction, double defenceValue, DefenceSystemType systemType, double kineticDefMult, double laserDefMult, double missileDefMult) {
			this.Id = id;
			this.Name = name;
			this.Faction = faction;
			this.DefenceValue = defenceValue;
			this.SystemType = systemType;
			DefMultAgainstWepTypeMap = new Dictionary<WeaponType, double> {
				{ WeaponType.KINETIC, kineticDefMult },
				{ WeaponType.LASER, laserDefMult },
				{ WeaponType.MISSILE, missileDefMult }
			};
		}

		public string Name { get => name; set => name = value; }
		public Faction Faction { get => faction; set => faction = value; }
		public double DefenceValue { get => defenceValue; set => defenceValue = value; }
		public DefenceSystemType SystemType { get => systemType; set => systemType = value; }
		public Dictionary<WeaponType, double> DefMultAgainstWepTypeMap { get => defMultAgainstWepTypeMap; set => defMultAgainstWepTypeMap = value; }
		public int Id { get => id; set => id = value; }
		public double DefenceValueLeft { get => defenceValueLeft; set => defenceValueLeft = value; }
	}
}
