using Application.Interfaces;
using BotTelegram.Handlers;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Game
{
    public class GameInfoCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameInfoCommandHandler> _logger;

        public string CommandName => "/gameinfo";

        public GameInfoCommandHandler(
            IGameService gameService,
            ILogger<GameInfoCommandHandler> logger)
        {
            _gameService = gameService;
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
                        ❌ <b>Uso:</b> /gameinfo [codice_partita]
                        
                        💡 <i>Esempio:</i> /gameinfo ABC123
                        """;
                }

                var gameCode = parts[1].Trim().ToUpper();
                var game = await _gameService.GetGameByCodeAsync(gameCode);

                if (game == null)
                {
                    return "❌ Partita non trovata!";
                }

                var stateEmoji = GetStateEmoji(game.State);
                var stateName = GetStateName(game.State);

                // Info base
                var response = $"""
                    📋 <b>Partita {game.Code}</b>
                    
                    {stateEmoji} <b>Stato:</b> {stateName}
                    👑 <b>Creatore:</b> {game.PlayerCreator.Username}
                    👥 <b>Giocatori:</b> {game.Players.Count}/{game.MaxPlayers}
                    🎯 <b>Turno:</b> {game.CurrentTurn}
                    📅 <b>Creata:</b> {game.CreatedAt:dd/MM/yyyy HH:mm}
                    """;

                if (game.StartedAt.HasValue)
                {
                    response += $"\n🚀 <b>Iniziata:</b> {game.StartedAt.Value:dd/MM/yyyy HH:mm}";
                }

                // Giocatori
                if (game.Players.Any())
                {
                    var playerList = string.Join("\n", game.Players.Select(p =>
                    {
                        var isCreator = p.TelegramId == game.PlayerCreatorId ? " 👑" : "";
                        return $"   • {p.Username}{isCreator}";
                    }));

                    response += $"\n\n<b>👥 Giocatori:</b>\n{playerList}";
                }

                // Città (se avviata)
                if (game.City != null)
                {
                    var emptySlots = game.City.Slots.Count(s => s.IsEmpty);
                    var builtSlots = game.City.Slots.Count - emptySlots;

                    response += $"""
                        
                        
                        🏙️ <b>Città:</b> {game.City.Name}
                        📍 <b>Slot:</b> {emptySlots} vuoti / {builtSlots} costruiti
                        """;
                }

                // Famiglie (se avviata)
                if (game.Families.Any())
                {
                    var familyList = string.Join("\n", game.Families.Select(f =>
                    {
                        var owner = f.Members.FirstOrDefault(m => m.IsOwner);
                        return $"   {f.CoatOfArms} <b>{f.Name}</b>\n      👤 {owner?.Player.Username ?? "N/A"} | 💰{f.Resources.Gold} | ⭐{f.Resources.Influence}";
                    }));

                    response += $"\n\n<b>👨‍👩‍👧‍👦 Famiglie:</b>\n{familyList}";
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game info");
                return $"❌ Errore: {ex.Message}";
            }
        }

        private static string GetStateEmoji(GameState state) => state switch
        {
            GameState.NotStarted => "🔵",
            GameState.Running => "🟢",
            GameState.Ended => "⚫",
            GameState.Cancelled => "🔴",
            _ => "❓"
        };

        private static string GetStateName(GameState state) => state switch
        {
            GameState.NotStarted => "In attesa",
            GameState.Running => "In corso",
            GameState.Ended => "Terminata",
            GameState.Cancelled => "Cancellata",
            _ => "Sconosciuto"
        };
    }
}
