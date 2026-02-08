using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Game
{
    public class EndGameCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<EndGameCommandHandler> _logger;

        public string CommandName => "/endgame";

        public EndGameCommandHandler(
            IGameService gameService,
            ILocalizationService localization,
            ILogger<EndGameCommandHandler> logger)
        {
            _gameService = gameService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                var parts = context.MessageText.Split(' ', 2);
                if (parts.Length < 2)
                {
                    return "❌ <b>Uso:</b> /endgame [codice_partita]\n\n" +
                           "💡 <i>Esempio:</i> /endgame ABC123";
                }

                var gameCode = parts[1].Trim().ToUpper();

                var game = await _gameService.EndGameAsync(gameCode, context.TelegramId);

                _logger.LogInformation("Game {GameCode} ended by {Username}", gameCode, context.Username);

                var duration = game.EndedAt.HasValue && game.StartedAt.HasValue
                    ? game.EndedAt.Value - game.StartedAt.Value
                    : TimeSpan.Zero;

                return $"{_localization.GetString("game_ended", context.LanguageCode, gameCode)}\n\n" +
                       $"⏱️ Durata: {_FormatDuration(duration)}\n" +
                       $"👥 Giocatori: {game.Players.Count}\n" +
                       $"🎯 Turni giocati: {game.Turns.Count}";
            }
            catch (GameNotFoundException ex)
            {
                _logger.LogWarning("Game not found: {GameCode}", ex.GameCode);
                return _localization.GetString("game_not_found", context.LanguageCode);
            }
            catch (UnauthorizedGameActionException)
            {
                return _localization.GetString("not_game_creator", context.LanguageCode);
            }
            catch (InvalidOperationException ex)
            {
                return $"❌ {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending game");
                return _localization.GetString("error_generic", context.LanguageCode, ex.Message);
            }
        }

        private static string _FormatDuration(TimeSpan duration)
        {
            if (duration.TotalMinutes < 1)
                return "< 1 minuto";
            if (duration.TotalHours < 1)
                return $"{(int)duration.TotalMinutes} minuti";
            if (duration.TotalDays < 1)
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";

            return $"{(int)duration.TotalDays}g {duration.Hours}h";
        }
    }
}
