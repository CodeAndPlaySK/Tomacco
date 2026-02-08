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
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(p => p.TelegramId);

            builder.Property(p => p.TelegramId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Username)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.LanguageCode)
                .HasMaxLength(5)
                .HasDefaultValue("en");

            builder.HasIndex(p => p.Username);
            builder.HasIndex(p => p.CreatedAt);
        }
    }
}
