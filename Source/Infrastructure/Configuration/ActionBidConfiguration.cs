using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class ActionBidConfiguration : IEntityTypeConfiguration<ActionBid>
    {
        public void Configure(EntityTypeBuilder<ActionBid> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasIndex(b => new { b.ActiveActionSlotId, b.TurnNumber });
            builder.HasIndex(b => new { b.FamilyId, b.TurnNumber });

            // Relazione con Family
            builder.HasOne(b => b.Family)
                .WithMany()
                .HasForeignKey(b => b.FamilyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relazione con Hero
            builder.HasOne(b => b.Hero)
                .WithMany()
                .HasForeignKey(b => b.HeroId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
