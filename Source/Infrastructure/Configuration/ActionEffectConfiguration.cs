using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class ActionEffectConfiguration : IEntityTypeConfiguration<ActionEffect>
    {
        public void Configure(EntityTypeBuilder<ActionEffect> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EffectType)
                .HasConversion<string>();

            builder.Property(e => e.TargetType)
                .HasConversion<string>();

            builder.Property(e => e.StatAffected)
                .HasConversion<string>();

            builder.Property(e => e.RequiredHeroClass)
                .HasConversion<string>();

            builder.HasIndex(e => new { e.ActionTemplateId, e.Order });
        }
    }
}
