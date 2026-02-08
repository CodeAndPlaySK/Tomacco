using Domain.Enums;
using Domain.Models;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GameDbContext : DbContext
    {
        // Players & Families
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Family> Families => Set<Family>();
        public DbSet<FamilyOfPlayer> FamilyOfPlayers => Set<FamilyOfPlayer>();

        // Games
        public DbSet<Game> Games => Set<Game>();
        public DbSet<GameTurn> GameTurns => Set<GameTurn>();

        // Cities & Slots
        public DbSet<City> Cities => Set<City>();
        public DbSet<SlotBuilding> SlotBuildings => Set<SlotBuilding>();

        // Buildings
        public DbSet<BuildingTemplate> BuildingTemplates => Set<BuildingTemplate>();
        public DbSet<BuildingActionTemplate> BuildingActionTemplates => Set<BuildingActionTemplate>();
        public DbSet<Building> Buildings => Set<Building>();
        public DbSet<ActiveActionSlot> ActiveActionSlots => Set<ActiveActionSlot>();
        public DbSet<ActionBid> ActionBids => Set<ActionBid>();

        // Effects & Conditions
        public DbSet<ActionEffect> ActionEffects => Set<ActionEffect>();
        public DbSet<ActionCondition> ActionConditions => Set<ActionCondition>();

        // Heroes & Moves
        public DbSet<Hero> Heroes => Set<Hero>();
        public DbSet<Move> Moves => Set<Move>();
        public DbSet<MoveTypeStrategy> MoveStrategies => Set<MoveTypeStrategy>();

        // Audit
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            // Applica tutte le configurazioni
            model.ApplyConfiguration(new PlayerConfiguration());
            model.ApplyConfiguration(new FamilyOfPlayerConfiguration());
            model.ApplyConfiguration(new GameConfiguration());
            model.ApplyConfiguration(new CityConfiguration());
            model.ApplyConfiguration(new SlotBuildingConfiguration());
            model.ApplyConfiguration(new BuildingTemplateConfiguration());
            model.ApplyConfiguration(new BuildingActionTemplateConfiguration());
            model.ApplyConfiguration(new BuildingConfiguration());
            model.ApplyConfiguration(new ActiveActionSlotConfiguration());
            model.ApplyConfiguration(new ActionBidConfiguration());
            model.ApplyConfiguration(new ActionEffectConfiguration());
            model.ApplyConfiguration(new ActionConditionConfiguration());
            model.ApplyConfiguration(new HeroConfiguration());
            model.ApplyConfiguration(new MoveConfiguration());
            model.ApplyConfiguration(new MoveStrategyConfiguration());

            // Configurazioni semplici inline
            model.Entity<Family>().HasKey(f => f.Id);
            model.Entity<Family>().HasIndex(f => f.Name);

            model.Entity<GameTurn>().HasKey(gt => gt.Id);
            model.Entity<GameTurn>().HasIndex(gt => new { gt.GameCode, gt.TurnNumber });

            model.Entity<GameTurn>()
                .HasOne(gt => gt.Game)
                .WithMany(g => g.Turns)
                .HasForeignKey(gt => gt.GameCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
