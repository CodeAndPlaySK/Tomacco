using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class ActiveActionSlotConfiguration : IEntityTypeConfiguration<ActiveActionSlot>
    {
        public void Configure(EntityTypeBuilder<ActiveActionSlot> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.BuildingId, s.ActionTemplateId });

            // Relazione con ActionTemplate
            builder.HasOne(s => s.ActionTemplate)
                .WithMany()
                .HasForeignKey(s => s.ActionTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bids
            builder.HasMany(s => s.Bids)
                .WithOne(b => b.ActiveActionSlot)
                .HasForeignKey(b => b.ActiveActionSlotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
