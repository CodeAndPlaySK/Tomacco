using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Tomacco.Source.Entities;

namespace Domain.Repositories
{
    public class GameDbContext : DbContext
    {
        public DbSet<Family> Families => Set<Family>();
        public DbSet<Hero> Heroes => Set<Hero>();
        public DbSet<Building> Buildings => Set<Building>();
        public DbSet<BuildingHeroSlot> BuildingHeroSlots => Set<BuildingHeroSlot>();
        public DbSet<BuildingAction> BuildingActions => Set<BuildingAction>();
        public DbSet<ActiveBuildingAction> ActiveBuildingActions => Set<ActiveBuildingAction>();
        public DbSet<ResourceEvent> ResourceEvents => Set<ResourceEvent>();
        public DbSet<GameEvent> GameEvents => Set<GameEvent>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=game.db");
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Family>()
                .HasIndex(f => f.Name)
                .IsUnique();

            model.Entity<Building>()
                .HasIndex(b => b.Code)
                .IsUnique();

            model.Entity<GameEvent>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
