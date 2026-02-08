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
    public class MoveConfiguration : IEntityTypeConfiguration<Move>
    {
        public void Configure(EntityTypeBuilder<Move> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(m => m.MoveType)
                .HasConversion<string>();

            builder.Property(m => m.SimpleTargetMove)
                .HasConversion<string>();

            builder.Property(m => m.AreaTargetMove)
                .HasConversion<string>();

            builder.Property(m => m.StatToHit)
                .HasConversion<string>();

            // Relazione con Strategy
            builder.HasMany(m => m.Strategies)
                .WithOne(ms => ms.Move)
                .HasForeignKey(ms => ms.MoveId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
