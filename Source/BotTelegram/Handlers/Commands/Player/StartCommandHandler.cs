using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Player
{
    public class StartCommandHandler : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        private readonly IAuditService _auditService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<StartCommandHandler> _logger;

        public string CommandName => "/start";

        public StartCommandHandler(
            IPlayerService playerService,
            IAuditService auditService,
            ILocalizationService localization,
            ILogger<StartCommandHandler> logger)
        {
            _playerService = playerService;
            _auditService = auditService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                if (context.Player != null)
                {
                    // Player già esistente - aggiorna last login
                    context.Player.LastLoginAt = DateTime.UtcNow;
                    await _playerService.UpdatePlayerAsync(context.Player);

                    await _auditService.LogAsync(
                        AuditAction.Login,
                        nameof(Domain.Models.Player),
                        context.TelegramId,
                        context.TelegramId,
                        context.Player.Username
                    );

                    _logger.LogInformation("Player {Username} logged in", context.Player.Username);

                    return _localization.GetString("welcome_back", context.LanguageCode, context.Player.Username);
                }

                // Nuovo player - determina la lingua
                var detectedLang = context.UserLanguageCode ?? "en";
                if (!_localization.IsLanguageSupported(detectedLang))
                    detectedLang = "en";

                var player = await _playerService.CreatePlayerAsync(context.TelegramId, context.Username, detectedLang);

                _logger.LogInformation("New player registered: {Username} ({TelegramId})", context.Username, context.TelegramId);

                return _localization.GetString("welcome_new", detectedLang, player.Username, player.TelegramId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling /start for {TelegramId}", context.TelegramId);
                return _localization.GetString("error_registration", context.LanguageCode, ex.Message);
            }
        }
    }
}
