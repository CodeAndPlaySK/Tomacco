using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Game
{
    public class ListGamesCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<ListGamesCommandHandler> _logger;

        public string CommandName => "/games";

        public ListGamesCommandHandler(
            IGameService gameService,
            ILocalizationService localization,
            ILogger<ListGamesCommandHandler> logger)
        {
            _gameService = gameService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                var games = await _gameService.GetGamesByStateAsync(Domain.Enums.GameState.NotStarted);

                if (!games.Any())
                {
                    return """
                        📋 <b>Partite Disponibili</b>
                        
                        <i>Nessuna partita disponibile al momento.</i>
                        
                        💡 Usa /creategame per creare una nuova partita!
                        """;
                }

                var gameList = string.Join("\n\n", games.Take(10).Select(g =>
                {
                    var isFull = g.Players.Count >= g.MaxPlayers;
                    var fullIndicator = isFull ? "🔒" : "🟢";
                    var relativeTime = GetRelativeTime(g.CreatedAt);

                    return $"""
                        {fullIndicator} <b>{g.Code}</b>
                        👤 {g.PlayerCreator.Username}
                        👥 {g.Players.Count}/{g.MaxPlayers} giocatori
                        🏙️ {g.CitySlots} slot città
                        📅 {relativeTime}
                        """;
                }));

                return $"""
                    📋 <b>Partite Disponibili ({games.Count()})</b>
                    
                    {gameList}
                    
                    💡 Usa <code>/joingame [codice]</code> per unirti
                    💡 Usa <code>/gameinfo [codice]</code> per dettagli
                    """;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing games");
                return $"❌ Errore: {ex.Message}";
            }
        }

        private static string GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1) return "Ora";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minuti fa";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} ore fa";

            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
