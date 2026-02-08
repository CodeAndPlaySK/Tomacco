using Application.Interfaces;
using BotTelegram.Handlers;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Family
{
    public class FamilyInfoCommandHandler : ICommandHandler
    {
        private readonly IGameService _gameService;
        private readonly ILogger<FamilyInfoCommandHandler> _logger;

        public string CommandName => "/familyinfo";

        public FamilyInfoCommandHandler(
            IGameService gameService,
            ILogger<FamilyInfoCommandHandler> logger)
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
                        ❌ <b>Uso:</b> /familyinfo [codice_partita]
                        
                        💡 <i>Esempio:</i> /familyinfo ABC123
                        
                        Mostra la tua famiglia nella partita specificata.
                        """;
                }

                var gameCode = parts[1].Trim().ToUpper();
                var game = await _gameService.GetGameByCodeAsync(gameCode);

                if (game == null)
                {
                    return "❌ Partita non trovata!";
                }

                if (!game.Families.Any())
                {
                    return "❌ La partita non è ancora stata avviata. Le famiglie verranno create all'avvio.";
                }

                // Trova la famiglia del giocatore
                var myFamily = game.Families.FirstOrDefault(f =>
                    f.Members.Any(m => m.PlayerTelegramId == context.TelegramId));

                if (myFamily == null)
                {
                    return "❌ Non fai parte di nessuna famiglia in questa partita!";
                }

                // Info famiglia
                var members = string.Join("\n", myFamily.Members.Select(m =>
                {
                    var role = m.IsOwner ? "👑 Capofamiglia" : "👤 Membro";
                    return $"   • {m.Player.Username} - {role}";
                }));

                var heroes = myFamily.Heroes.Any()
                    ? string.Join("\n", myFamily.Heroes.Select(h =>
                        $"   • {h.Name} ({h.HeroClassType}) - Lv.{h.Stats.Level}"))
                    : "   <i>Nessun eroe</i>";

                var buildings = myFamily.Buildings.Any()
                    ? string.Join("\n", myFamily.Buildings.Select(b =>
                    {
                        var status = b.IsCompleted ? "✅" : $"🔨 ({b.TurnsRemaining} turni)";
                        return $"   • {b.Name} {status}";
                    }))
                    : "   <i>Nessun edificio</i>";

                return $"""
                    {myFamily.CoatOfArms} <b>Casa {myFamily.Name}</b>
                    <i>{myFamily.Description}</i>
                    
                    💰 <b>Risorse:</b>
                       Oro: {myFamily.Resources.Gold}
                       Influenza: {myFamily.Resources.Influence}
                    
                    👥 <b>Membri ({myFamily.Members.Count}):</b>
                    {members}
                    
                    ⚔️ <b>Eroi ({myFamily.Heroes.Count}):</b>
                    {heroes}
                    
                    🏗️ <b>Edifici ({myFamily.Buildings.Count}):</b>
                    {buildings}
                    """;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting family info");
                return $"❌ Errore: {ex.Message}";
            }
        }
    }
}
