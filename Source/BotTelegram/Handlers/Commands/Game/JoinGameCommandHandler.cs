using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Game
{
    public class JoinGameCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<JoinGameCommandHandler> _logger;

        public string CommandName => "/joingame";

        public JoinGameCommandHandler(
            IGameService gameService,
            ILocalizationService localization,
            ILogger<JoinGameCommandHandler> logger)
        {
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

                var parts = context.MessageText.Split(' ', 2);
                if (parts.Length < 2)
                {
                    return "❌ <b>Uso:</b> /joingame [codice_partita]\n\n" +
                           "💡 <i>Esempio:</i> /joingame ABC123\n\n" +
                           "Usa /games per vedere le partite disponibili";
                }

                var gameCode = parts[1].Trim().ToUpper();

                // Verifica se può unirsi
                if (!await _gameService.CanPlayerJoinGameAsync(gameCode, context.TelegramId))
                {
                    return "❌ Non puoi unirti a questa partita (già piena, già iniziata o sei già dentro)";
                }

                var game = await _gameService.AddPlayerToGameAsync(gameCode, context.TelegramId);

                _logger.LogInformation("Player {Username} joined game {GameCode}", context.Username, gameCode);

                return _localization.GetString(
                    "game_joined",
                    context.LanguageCode,
                    gameCode,
                    game.Players.Count,
                    game.MaxPlayers
                );
            }
            catch (GameNotFoundException)
            {
                return _localization.GetString("game_not_found", context.LanguageCode);
            }
            catch (PlayerNotFoundException)
            {
                return _localization.GetString("not_registered", context.LanguageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining game");
                return _localization.GetString("error_generic", context.LanguageCode, ex.Message);
            }
        }
    }
}
