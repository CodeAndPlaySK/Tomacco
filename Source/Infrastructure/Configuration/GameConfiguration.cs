using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasKey(g => g.Code);

            builder.Property(g => g.Code)
                .HasMaxLength(10);

            builder.Property(g => g.State)
                .HasConversion<string>();

            builder.HasIndex(g => g.State);
            builder.HasIndex(g => g.CreatedAt);

            // Relazione con PlayerCreator
            builder.HasOne(g => g.PlayerCreator)
                .WithMany()
                .HasForeignKey(g => g.PlayerCreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relazione many-to-many con Players
            builder.HasMany(g => g.Players)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "GamePlayer",
                    j => j.HasOne<Player>().WithMany().HasForeignKey("TelegramId"),
                    j => j.HasOne<Game>().WithMany().HasForeignKey("GameCode")
                );

            // Relazione 1-to-1 con City (configurata in CityConfiguration)
        }
    }
}
