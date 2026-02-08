using Application.Interfaces;
using BotTelegram.Handlers;

namespace TelegramBot.Handlers.Commands.Systrem
{
    public class HelpCommandHandler : ICommandHandler
    {
        private readonly ILocalizationService _localization;

        public string CommandName => "/help";

        public HelpCommandHandler(ILocalizationService localization)
        {
            _localization = localization;
        }

        public Task<string> HandleAsync(CommandContext context)
        {
            var response = """
                           📖 <b>Guida ai Comandi</b>

                           <b>👤 Profilo</b>
                           /start - Registrati al gioco
                           /profile - Visualizza il tuo profilo
                           /updatename [nome] - Cambia il tuo nome
                           /players - Lista giocatori registrati

                           <b>🎮 Gestione Partite</b>
                           /creategame [slot] - Crea una nuova partita
                           /games - Lista partite disponibili
                           /joingame [codice] - Unisciti a una partita
                           /mygames - Le tue partite
                           /startgame [codice] - Avvia partita (solo creator)
                           /endgame [codice] - Termina partita (solo creator)

                           <b>📊 Info Partita</b>
                           /gameinfo [codice] - Dettagli partita
                           /cityinfo [codice] - Info città e slot
                           /familyinfo [codice] - La tua famiglia

                           <b>⚙️ Sistema</b>
                           /language - Cambia lingua
                           /audit - Cronologia attività
                           /help - Questa guida

                           <b>💡 Esempi:</b>
                           <code>/creategame 12</code> - Crea partita con 12 slot
                           <code>/joingame ABC123</code> - Unisciti alla partita ABC123
                           <code>/gameinfo ABC123</code> - Vedi info partita
                           """;

            return Task.FromResult(response);
        }
    }
}