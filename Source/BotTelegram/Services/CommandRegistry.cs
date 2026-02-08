using BotTelegram.Handlers.SubHandlers;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Handlers.Commands.City;
using TelegramBot.Handlers.Commands.Family;
using TelegramBot.Handlers.Commands.Game;
using TelegramBot.Handlers.Commands.Player;
using TelegramBot.Handlers.Commands.Systrem;

namespace BotTelegram.Handlers
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, Func<IServiceProvider, ICommandHandler>> _commands = new();

        public CommandRegistry()
        {
            // Player Commands
            Register("/start", sp => sp.GetRequiredService<StartCommandHandler>());
            Register("/profile", sp => sp.GetRequiredService<ProfileCommandHandler>());
            Register("/players", sp => sp.GetRequiredService<ListPlayersCommandHandler>());
            Register("/updatename", sp => sp.GetRequiredService<UpdateNameCommandHandler>());

            // Game Commands
            Register("/creategame", sp => sp.GetRequiredService<CreateGameCommandHandler>());
            Register("/games", sp => sp.GetRequiredService<ListGamesCommandHandler>());
            Register("/joingame", sp => sp.GetRequiredService<JoinGameCommandHandler>());
            Register("/mygames", sp => sp.GetRequiredService<MyGamesCommandHandler>());
            Register("/startgame", sp => sp.GetRequiredService<StartGameCommandHandler>());
            Register("/endgame", sp => sp.GetRequiredService<EndGameCommandHandler>());

            // Game Info Commands (nuovi)
            Register("/gameinfo", sp => sp.GetRequiredService<GameInfoCommandHandler>());
            Register("/cityinfo", sp => sp.GetRequiredService<CityInfoCommandHandler>());
            Register("/familyinfo", sp => sp.GetRequiredService<FamilyInfoCommandHandler>());

            // System Commands
            Register("/help", sp => sp.GetRequiredService<HelpCommandHandler>());
            Register("/language", sp => sp.GetRequiredService<LanguageCommandHandler>());
            Register("/audit", sp => sp.GetRequiredService<AuditCommandHandler>());
        }

        private void Register(string command, Func<IServiceProvider, ICommandHandler> factory)
        {
            _commands[command.ToLower()] = factory;
        }

        public ICommandHandler? GetHandler(string command, IServiceProvider serviceProvider)
        {
            var commandKey = command.Split(' ')[0].ToLower();
            return _commands.TryGetValue(commandKey, out var factory)
                ? factory(serviceProvider)
                : null;
        }

        public bool IsSpecialCommand(string command)
        {
            return command.StartsWith("/setlang_");
        }
    }
}
