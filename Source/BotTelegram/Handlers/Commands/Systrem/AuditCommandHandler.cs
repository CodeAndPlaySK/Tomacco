using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Systrem
{
    public class AuditCommandHandler : ICommandHandler
    {
        private readonly IAuditService _auditService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<AuditCommandHandler> _logger;

        public string CommandName => "/audit";

        public AuditCommandHandler(
            IAuditService auditService,
            ILocalizationService localization,
            ILogger<AuditCommandHandler> logger)
        {
            _auditService = auditService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                var auditLogs = await _auditService.GetUserAuditLogsAsync(context.TelegramId, 10);

                if (!auditLogs.Any())
                {
                    return "📊 <b>Cronologia Attività</b>\n\nNessuna attività registrata.";
                }

                var logList = string.Join("\n", auditLogs.Select(log =>
                    $"• {GetActionEmoji(log.Action)} <b>{log.Action}</b> - {log.Timestamp:dd/MM/yyyy HH:mm:ss}\n" +
                    $"  <i>{log.EntityType}{(log.EntityId != null ? $" ({log.EntityId})" : "")}</i>"
                ));

                return $"📊 <b>Cronologia Attività (ultimi 10)</b>\n\n{logList}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit for {TelegramId}", context.TelegramId);
                return _localization.GetString("error_generic", context.LanguageCode, ex.Message);
            }
        }

        private static string GetActionEmoji(string action)
        {
            return action switch
            {
                nameof(AuditAction.Create) => "➕",
                nameof(AuditAction.Update) => "✏️",
                nameof(AuditAction.Delete) => "🗑️",
                nameof(AuditAction.Read) => "👁️",
                nameof(AuditAction.Login) => "🔐",
                nameof(AuditAction.Logout) => "🚪",
                nameof(AuditAction.LanguageChange) => "🌍",
                nameof(AuditAction.GameStart) => "🎮",
                nameof(AuditAction.GameEnd) => "🏁",
                nameof(AuditAction.CommandExecuted) => "⚡",
                _ => "📝"
            };
        }
    }
}
