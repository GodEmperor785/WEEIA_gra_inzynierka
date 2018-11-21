using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Entity;
using System.Data.Common;
using System.Data.Entity;
using GAME_connection;

namespace GAME_Server {
	/// <summary>
	/// class defining DB context for server
	/// </summary>
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class GameDBContext : DbContext {
		public DbSet<DbPlayer> Players { get; set; }
		public DbSet<Faction> Factions { get; set; }
		public DbSet<DbWeapon> Weapons { get; set; }
		public DbSet<DbDefenceSystem> DefenceSystems { get; set; }
		public DbSet<DbShip> Ships { get; set; }
		public DbSet<DbFleet> Fleets { get; set; }
		public DbSet<DbBaseModifiers> BaseModifiers { get; set; }
		public DbSet<DbGameHistory> GameHistories { get; set; }
		public DbSet<DbShipTemplate> ShipTemplates { get; set; }
		public DbSet<DbLootBox> LootBoxes { get; set; }
		//public DbSet<DbFleetSizeExpMapping> FleetSizeExpMappings { get; set; }

		public GameDBContext() : base("GameContext") {
			Database.SetInitializer<GameDBContext>(new DropCreateDatabaseAlways<GameDBContext>());				//recreate always
			//Database.SetInitializer<GameDBContext>(null);                                                       //use existing database

			//Database.SetInitializer<GameDBContext>(new DropCreateDatabaseIfModelChanges<GameDBContext>());
			//Database.SetInitializer<GameDBContext>(new CreateDatabaseIfNotExists<GameDBContext>());

			//uncomment this to enable SQL logging to console
			//Database.Log = (string message) => { Console.WriteLine(message); };
		}

		public GameDBContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);
			modelBuilder.Properties<string>().Configure(str => str.HasMaxLength(256));
			modelBuilder.Entity<DbPlayer>().Property(p => p.Username).HasMaxLength(32);
			modelBuilder.Entity<DbPlayer>().HasIndex(user => user.Username).IsUnique(true);       //make player username unique

			//many-to-many join tables custom names
			modelBuilder.Entity<DbShipTemplate>()
				.HasMany(x => x.Weapons)
				.WithMany(x => x.Ships)
				.Map(x => {
					x.MapLeftKey("ShipTemplateID");
					x.MapRightKey("WeaponID");
					x.ToTable("ShipTemplates_Weapons");
				});
			modelBuilder.Entity<DbShipTemplate>()
				.HasMany(x => x.Defences)
				.WithMany(x => x.Ships)
				.Map(x => {
					x.MapLeftKey("ShipTemplateID");
					x.MapRightKey("DefenceSystemID");
					x.ToTable("ShipTemplates_DefenceSystems");
				});
			modelBuilder.Entity<DbFleet>()
				.HasMany(x => x.Ships)
				.WithMany(x => x.Fleets)
				.Map(x => {
					x.MapLeftKey("FleetID");
					x.MapRightKey("ShipID");
					x.ToTable("Fleets_Ships");
				});
		}

	}

}
