using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>

    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => new { a.EntityType, a.EntityId });
        }
    }
}
