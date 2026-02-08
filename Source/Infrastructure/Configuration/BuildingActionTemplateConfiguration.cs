using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class BuildingActionTemplateConfiguration : IEntityTypeConfiguration<BuildingActionTemplate>
    {
        public void Configure(EntityTypeBuilder<BuildingActionTemplate> builder)
        {
            builder.HasKey(at => at.Id);

            builder.Property(at => at.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(at => at.ActionType)
                .HasConversion<string>();

            // Effetti
            builder.HasMany(at => at.Effects)
                .WithOne(e => e.ActionTemplate)
                .HasForeignKey(e => e.ActionTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Condizioni
            builder.HasMany(at => at.Conditions)
                .WithOne(c => c.ActionTemplate)
                .HasForeignKey(c => c.ActionTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
