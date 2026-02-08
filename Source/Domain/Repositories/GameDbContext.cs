using Domain.Entities;
using Tomacco.Source.Entities;

namespace Domain.Repositories
{
    public class GameDbContext : DbContext
    {
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Family> Families => Set<Family>();
        public DbSet<FamilyOfPlayer> Families => Set<FamilyOfPlayer>();
        public DbSet<Hero> Heroes => Set<Hero>();
        public DbSet<City> Buildings => Set<City>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=game.db");
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Player>()
               .HasIndex(f => f.Username)
               .IsUnique();

            model.Entity<Family>()
                .HasIndex(f => f.Name)
                .IsUnique();
        }
    }
}
