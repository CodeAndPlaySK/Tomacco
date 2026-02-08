using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class ActionConditionConfiguration : IEntityTypeConfiguration<ActionCondition>
    {
        public void Configure(EntityTypeBuilder<ActionCondition> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ConditionType)
                .HasConversion<string>();

            builder.Property(c => c.RequiredHeroClass)
                .HasConversion<string>();

            builder.Property(c => c.RequiredBuildingKind)
                .HasConversion<string>();
        }
    }
}
