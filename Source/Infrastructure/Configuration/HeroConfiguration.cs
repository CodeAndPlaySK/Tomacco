using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class HeroConfiguration : IEntityTypeConfiguration<Hero>
    {
        public void Configure(EntityTypeBuilder<Hero> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                .HasMaxLength(100)
                .IsRequired();

            // Relazione con Family
            builder.HasOne(h => h.Family)
                .WithMany()
                .HasForeignKey("FamilyId")
                .OnDelete(DeleteBehavior.Restrict);

            // HeroStats come Owned Entity (embedded nella stessa tabella o in una separata)
            builder.OwnsOne(h => h.Stats, stats =>
            {
                stats.Property(s => s.Level).HasDefaultValue(1);
                stats.Property(s => s.Physic);
                stats.Property(s => s.Mind);
                stats.Property(s => s.Faith);
                stats.Property(s => s.Speed);
                stats.Property(s => s.Charisma);
                stats.Property(s => s.Defence);

                // BarPoints come Owned Types (saranno colonne nella tabella Heroes)
                stats.OwnsOne(s => s.LifePoints, lifePoints =>
                {
                    lifePoints.Property(bp => bp.Current).HasColumnName("LifePoints_Current");
                    lifePoints.Property(bp => bp.Max).HasColumnName("LifePoints_Max");
                });

                stats.OwnsOne(s => s.MoralityPoints, moralityPoints =>
                {
                    moralityPoints.Property(bp => bp.Current).HasColumnName("MoralityPoints_Current");
                    moralityPoints.Property(bp => bp.Max).HasColumnName("MoralityPoints_Max");
                });
            });

            // Mosse come relazione 1-to-many
            builder.HasMany(h => h.Moves)
                .WithOne()
                .HasForeignKey("HeroId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
