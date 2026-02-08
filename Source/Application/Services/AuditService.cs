using System.Text.Json;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    

    public class AuditService : IAuditService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(GameDbContext context, ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(
            AuditAction action,
            string entityType,
            string? entityId = null,
            string? userId = null,
            string? username = null,
            object? oldValues = null,
            object? newValues = null,
            string? additionalInfo = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Timestamp = DateTime.UtcNow,
                    Action = action.ToString(),
                    EntityType = entityType,
                    EntityId = entityId,
                    UserId = userId,
                    Username = username,
                    OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                    AdditionalInfo = additionalInfo
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Audit: {Action} on {EntityType} {EntityId} by {Username} ({UserId})",
                    action, entityType, entityId ?? "N/A", username ?? "System", userId ?? "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio dell'audit log");
            }
        }

        public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId, int limit = 100)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entityType, string entityId)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int limit = 100)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }
    }
}
