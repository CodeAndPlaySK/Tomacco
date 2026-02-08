using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class FamilyConfiguration : IEntityTypeConfiguration<Family>
    {
        public void Configure(EntityTypeBuilder<Family> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(f => f.CoatOfArms)
                .HasMaxLength(10);

            builder.HasIndex(f => new { f.GameCode, f.Name }).IsUnique();

            // Relazione con Game
            builder.HasOne(f => f.Game)
                .WithMany(g => g.Families)
                .HasForeignKey(f => f.GameCode)
                .OnDelete(DeleteBehavior.Cascade);

            // Relazione con Members
            builder.HasMany(f => f.Members)
                .WithOne(m => m.Family)
                .HasForeignKey(m => m.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relazione con Heroes
            builder.HasMany(f => f.Heroes)
                .WithOne(h => h.Family)
                .HasForeignKey(h => h.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relazione con Buildings
            builder.HasMany(f => f.Buildings)
                .WithOne(b => b.FamilyOwner)
                .HasForeignKey(b => b.FamilyOwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
