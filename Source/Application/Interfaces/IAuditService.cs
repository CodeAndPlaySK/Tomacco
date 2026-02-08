using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(
            AuditAction action,
            string entityType,
            string? entityId = null,
            string? userId = null,
            string? username = null,
            object? oldValues = null,
            object? newValues = null,
            string? additionalInfo = null
        );

        Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId, int limit = 100);
        Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entityType, string entityId);
        Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int limit = 100);
    }
}
