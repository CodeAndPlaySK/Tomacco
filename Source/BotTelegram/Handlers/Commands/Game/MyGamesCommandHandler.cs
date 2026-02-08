using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Game
{
    public class MyGamesCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<MyGamesCommandHandler> _logger;

        public string CommandName => "/mygames";

        public MyGamesCommandHandler(
            IGameService gameService,
            ILocalizationService localization,
            ILogger<MyGamesCommandHandler> logger)
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

                var games = await _gameService.GetPlayerGamesAsync(context.TelegramId);

                if (!games.Any())
                {
                    return "📋 <b>Le tue partite</b>\n\n" +
                           "Non sei ancora in nessuna partita!\n\n" +
                           "💡 Usa /creategame per creare una partita\n" +
                           "💡 Usa /games per vedere partite disponibili";
                }

                var gamesByState = games.GroupBy(g => g.State);

                var result = "📋 <b>Le tue partite</b>\n\n";

                foreach (var group in gamesByState.OrderBy(g => g.Key))
                {
                    var stateEmoji = _GetStateEmoji(group.Key);
                    var stateName = _GetStateName(group.Key);

                    result += $"{stateEmoji} <b>{stateName}</b> ({group.Count()})\n";

                    foreach (var game in group.Take(5))
                    {
                        var isCreator = game.PlayerCreatorId == context.TelegramId ? "👑 " : "";
                        result += $"  • {isCreator}<code>{game.Code}</code> - {game.Players.Count} giocatori\n";
                    }

                    result += "\n";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing player games");
                return _localization.GetString("error_generic", context.LanguageCode, ex.Message);
            }
        }

        private static string _GetStateEmoji(GameState state)
        {
            return state switch
            {
                GameState.NotStarted => "🔵",
                GameState.Running => "🟢",
                GameState.Ended => "⚫",
                GameState.Cancelled => "🔴",
                _ => "❓"
            };
        }

        private static string _GetStateName(GameState state)
        {
            return state switch
            {
                GameState.NotStarted => "In attesa",
                GameState.Running => "In corso",
                GameState.Ended => "Terminate",
                GameState.Cancelled => "Cancellate",
                _ => "Sconosciuto"
            };
        }
    }
}
