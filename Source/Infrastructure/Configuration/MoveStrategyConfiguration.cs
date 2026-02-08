using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class MoveStrategyConfiguration : IEntityTypeConfiguration<MoveTypeStrategy>
    {
        public void Configure(EntityTypeBuilder<MoveTypeStrategy> builder)
        {
            builder.HasKey(ms => ms.Id);

            builder.Property(ms => ms.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ms => ms.StrategyType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(ms => ms.StatToBuff)
                .HasConversion<string>();

            builder.Property(ms => ms.StatToHeal)
                .HasConversion<string>();

            builder.HasIndex(ms => new { ms.MoveId, ms.Order });

            // Relazione con Move
            builder.HasOne(ms => ms.Move)
                .WithMany(m => m.Strategies)
                .HasForeignKey(ms => ms.MoveId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
