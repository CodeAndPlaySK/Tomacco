using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Player
{
    public class ListPlayersCommandHandler : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<ListPlayersCommandHandler> _logger;

        public string CommandName => "/players";

        public ListPlayersCommandHandler(
            IPlayerService playerService,
            ILocalizationService localization,
            ILogger<ListPlayersCommandHandler> logger)
        {
            _playerService = playerService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                var players = await _playerService.GetAllPlayersAsync();

                if (!players.Any())
                {
                    return _localization.GetString("players_empty", context.LanguageCode);
                }

                // Ordina per ultimo accesso
                var sortedPlayers = players
                    .OrderByDescending(p => p.LastLoginAt ?? p.CreatedAt)
                    .Take(20) // Limita a 20 per non sovraccaricare
                    .ToList();

                var playerList = string.Join("\n",
                    sortedPlayers.Select((p, i) =>
                    {
                        var lastSeen = p.LastLoginAt.HasValue
                            ? $"({_GetRelativeTime(p.LastLoginAt.Value)})"
                            : "(mai)";

                        return $"{i + 1}. <b>{p.Username}</b> {_localization.GetLanguageName(p.LanguageCode)} {lastSeen}";
                    }));

                return $"{_localization.GetString("players_title", context.LanguageCode, sortedPlayers.Count)}\n{playerList}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling /players");
                return _localization.GetString("error_generic", context.LanguageCode, ex.Message);
            }
        }

        private string _GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "ora";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m fa";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h fa";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}g fa";

            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
