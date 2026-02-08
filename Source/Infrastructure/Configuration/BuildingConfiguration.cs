using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        public void Configure(EntityTypeBuilder<Building> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.CustomName)
                .HasMaxLength(100);

            // Relazione con Template
            builder.HasOne(b => b.Template)
                .WithMany()
                .HasForeignKey(b => b.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relazione con Family (proprietario)
            builder.HasOne(b => b.FamilyOwner)
                .WithMany()
                .HasForeignKey(b => b.FamilyOwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Slot azioni attive
            builder.HasMany(b => b.ActiveActionSlots)
                .WithOne(s => s.Building)
                .HasForeignKey(s => s.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
