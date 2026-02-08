using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class SlotBuildingConfiguration : IEntityTypeConfiguration<SlotBuilding>
    {
        public void Configure(EntityTypeBuilder<SlotBuilding> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .HasMaxLength(100);

            builder.HasIndex(s => new { s.CityId, s.Position }).IsUnique();

            // Relazione opzionale con Building
            builder.HasOne(s => s.Building)
                .WithOne()
                .HasForeignKey<SlotBuilding>(s => s.BuildingId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
