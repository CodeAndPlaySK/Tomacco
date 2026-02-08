using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using BotTelegram.Handlers;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.City
{
    public class CityInfoCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILogger<CityInfoCommandHandler> _logger;

        public string CommandName => "/cityinfo";

        public CityInfoCommandHandler(
            IGameService gameService,
            ILogger<CityInfoCommandHandler> logger)
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
                        ❌ <b>Uso:</b> /cityinfo [codice_partita]
                        
                        💡 <i>Esempio:</i> /cityinfo ABC123
                        """;
                }

                var gameCode = parts[1].Trim().ToUpper();
                var game = await _gameService.GetGameByCodeAsync(gameCode);

                if (game == null)
                {
                    return "❌ Partita non trovata!";
                }

                if (game.City == null)
                {
                    return "❌ La partita non è ancora stata avviata. La città verrà creata all'avvio.";
                }

                var city = game.City;

                // Header
                var response = $"""
                    🏙️ <b>{city.Name}</b>
                    <i>{city.Description}</i>
                    
                    📍 <b>Slot Edifici ({city.Slots.Count}):</b>
                    
                    """;

                // Lista slot
                foreach (var slot in city.Slots.OrderBy(s => s.Position))
                {
                    var status = slot.IsEmpty
                        ? "⬜ <i>Vuoto</i>"
                        : $"🏗️ <b>{slot.Building?.Name ?? "Costruzione"}</b>";

                    response += $"{slot.Position}. {slot.Name}: {status}\n";
                }

                // Statistiche
                var emptyCount = city.Slots.Count(s => s.IsEmpty);
                var builtCount = city.Slots.Count - emptyCount;

                response += $"""
                    
                    📊 <b>Statistiche:</b>
                    ⬜ Slot vuoti: {emptyCount}
                    🏗️ Slot costruiti: {builtCount}
                    """;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting city info");
                return $"❌ Errore: {ex.Message}";
            }
        }
    }
}
