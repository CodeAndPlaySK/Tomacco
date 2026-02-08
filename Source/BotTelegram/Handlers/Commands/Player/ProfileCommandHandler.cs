using Application.Interfaces;
using Application.Services;
using BotTelegram.Services;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BotTelegram.Handlers.SubHandlers
{
    public class ProfileCommandHandler : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        private readonly IGameService _gameService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<ProfileCommandHandler> _logger;

        public string CommandName => "/profile";

        public ProfileCommandHandler(
            IPlayerService playerService,
            IGameService gameService,
            ILocalizationService localization,
            ILogger<ProfileCommandHandler> logger)
        {
            _playerService = playerService;
            _gameService = gameService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                if (context.Player == null)
                {
                    return _localization.GetString("not_registered", context.LanguageCode);
                }

                // Statistiche giochi
                var totalGames = await _gameService.GetPlayerGameCountAsync(context.TelegramId);
                var activeGames = await _gameService.GetPlayerGameCountAsync(context.TelegramId, GameState.Running);

                var langName = _localization.GetLanguageName(context.Player.LanguageCode);

                return $"{_localization.GetString("profile_title", context.LanguageCode)}\n" +
                       $"{_localization.GetString("profile_username", context.LanguageCode, context.Player.Username)}\n" +
                       $"{_localization.GetString("profile_id", context.LanguageCode, context.Player.TelegramId)}\n" +
                       $"{_localization.GetString("profile_language", context.LanguageCode, langName)}\n" +
                       $"📅 Registrato: {context.Player.CreatedAt:dd/MM/yyyy HH:mm}\n" +
                       $"🕐 Ultimo accesso: {context.Player.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}\n" +
                       $"🎮 Partite totali: {totalGames}\n" +
                       $"▶️ Partite attive: {activeGames}\n\n" +
                       $"{_localization.GetString("profile_footer", context.LanguageCode)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling /profile for {TelegramId}", context.TelegramId);
                return _localization.GetString("error_generic", context.LanguageCode, ex.Message);
            }
        }
    }
}
