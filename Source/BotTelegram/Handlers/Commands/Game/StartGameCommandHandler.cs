using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Game
{
    public class StartGameCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<StartGameCommandHandler> _logger;

        public string CommandName => "/startgame";

        public StartGameCommandHandler(
            IGameService gameService,
            ILocalizationService localization,
            ILogger<StartGameCommandHandler> logger)
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
                    return """
                        ❌ <b>Uso:</b> /startgame [codice_partita]
                        
                        💡 <i>Esempio:</i> /startgame ABC123
                        """;
                }

                var gameCode = parts[1].Trim().ToUpper();

                var game = await _gameService.StartGameAsync(gameCode, context.TelegramId);

                _logger.LogInformation("Game {GameCode} started by {Username}", gameCode, context.Username);

                // Costruisci messaggio dettagliato
                var playerList = string.Join("\n", game.Players.Select(p => $"   • {p.Username}"));

                // Info famiglie
                var familyList = string.Join("\n", game.Families.Select(f =>
                {
                    var owner = f.Members.FirstOrDefault(m => m.IsOwner);
                    return $"   {f.CoatOfArms} <b>{f.Name}</b> ({owner?.Player.Username ?? "N/A"}) - 💰{f.Gold} ⭐{f.Influence}";
                }));

                // Info slot città
                var cityInfo = game.City != null
                    ? $"""
                        
                        🏙️ <b>Città:</b> {game.City.Name}
                        📍 <b>Slot disponibili:</b> {game.City.Slots.Count}
                        """
                    : "";

                return $"""
                    🚀 <b>Partita {gameCode} avviata!</b>
                    {cityInfo}
                    
                    👥 <b>Giocatori ({game.Players.Count}):</b>
                    {playerList}
                    
                    👨‍👩‍👧‍👦 <b>Famiglie:</b>
                    {familyList}
                    
                    🎯 <b>Turno:</b> {game.CurrentTurn}
                    
                    🎮 <i>Buona fortuna a tutti!</i>
                    
                    💡 Usa /gameinfo {gameCode} per vedere i dettagli
                    """;
            }
            catch (GameNotFoundException ex)
            {
                _logger.LogWarning("Game not found: {GameCode}", ex.GameCode);
                return "❌ Partita non trovata!";
            }
            catch (UnauthorizedGameActionException)
            {
                return "❌ Solo il creatore può avviare la partita!";
            }
            catch (InvalidOperationException ex)
            {
                return $"❌ {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game");
                return $"❌ Errore nell'avvio della partita: {ex.Message}";
            }
        }
    }
}
