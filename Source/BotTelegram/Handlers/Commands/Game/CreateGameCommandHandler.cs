using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Game
{
    public class CreateGameCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILogger<CreateGameCommandHandler> _logger;

        public string CommandName => "/creategame";

        public CreateGameCommandHandler(
            IGameService gameService,
            ILogger<CreateGameCommandHandler> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                if (context.Player == null)
                {
                    return "❌ Devi prima registrarti con /start";
                }

                // Verifica partite attive
                var activeGameCount = await _gameService.GetPlayerGameCountAsync(
                    context.TelegramId, 
                    GameState.Running
                );

                if (activeGameCount >= 5)
                {
                    return "❌ Hai già troppe partite attive! Completa o abbandona alcune partite prima di crearne altre.";
                }

                // Parsing opzionale: /creategame 8 (numero di slot)
                int citySlots = 12;
                var parts = context.MessageText.Split(' ', 2);
                if (parts.Length > 1 && int.TryParse(parts[1], out var slots))
                {
                    citySlots = Math.Clamp(slots, 6, 20);
                }

                var game = await _gameService.CreateGameAsync(
                    context.TelegramId,
                    citySlots: citySlots
                );

                _logger.LogInformation("Game {GameCode} created by {Username}", game.Code, context.Username);

                return $"""
                    ✅ <b>Partita creata con successo!</b>
                    
                    📋 <b>Codice:</b> <code>{game.Code}</code>
                    👤 <b>Creatore:</b> {context.Username}
                    👥 <b>Giocatori:</b> {game.Players.Count}/{game.MaxPlayers}
                    🏙️ <b>Slot città:</b> {game.CitySlots}
                    📊 <b>Stato:</b> In attesa di giocatori
                    
                    💡 <i>Condividi il codice con altri giocatori!</i>
                    💡 <i>Usa /joingame {game.Code} per unirti</i>
                    💡 <i>Usa /startgame {game.Code} per avviare quando pronti</i>
                    """;
            }
            catch (PlayerNotFoundException)
            {
                return "❌ Devi prima registrarti con /start";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game for {TelegramId}", context.TelegramId);
                return $"❌ Errore nella creazione della partita: {ex.Message}";
            }
        }
    }
}
