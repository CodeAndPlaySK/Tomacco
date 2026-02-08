using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class BuildingTemplateConfiguration : IEntityTypeConfiguration<BuildingTemplate>
    {
        public void Configure(EntityTypeBuilder<BuildingTemplate> builder)
        {
            builder.HasKey(bt => bt.Id);

            builder.Property(bt => bt.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(bt => bt.KindType)
                .HasConversion<string>();

            builder.HasIndex(bt => bt.KindType);

            // Relazione con ActionTemplates
            builder.HasMany(bt => bt.ActionTemplates)
                .WithOne(at => at.BuildingTemplate)
                .HasForeignKey(at => at.BuildingTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
