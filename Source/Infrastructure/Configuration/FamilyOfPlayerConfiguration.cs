using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class FamilyOfPlayerConfiguration : IEntityTypeConfiguration<FamilyOfPlayer>
    {
        public void Configure(EntityTypeBuilder<FamilyOfPlayer> builder)
        {
            // Chiave composita
            builder.HasKey(fp => new { fp.FamilyId, fp.PlayerTelegramId });

            // Relazione con Player
            builder.HasOne(fp => fp.Player)
                .WithMany()
                .HasForeignKey(fp => fp.PlayerTelegramId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(fp => fp.PlayerTelegramId);
        }
    }
}
