using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(c => c.GameCode).IsUnique();

            // Relazione 1-to-1 con Game
            builder.HasOne(c => c.Game)
                .WithOne(g => g.City)
                .HasForeignKey<City>(c => c.GameCode)
                .OnDelete(DeleteBehavior.Cascade);

            // Relazione con Slots
            builder.HasMany(c => c.Slots)
                .WithOne(s => s.City)
                .HasForeignKey(s => s.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
