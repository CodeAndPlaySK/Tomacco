using Console.Models;
using Console.Services;
using Spectre.Console;

namespace Console.Menus
{
    public class MainMenu
    {
        private readonly ConsolePlayerUI _playerUI;
        private readonly ConsoleGameUI _gameUI;

        public MainMenu(ConsolePlayerUI playerUI, ConsoleGameUI gameUI)
        {
            _playerUI = playerUI;
            _gameUI = gameUI;
        }

        public async Task RunAsync()
        {
            ConsoleUser? currentUser = null;

            while (true)
            {
                if (currentUser == null || !currentUser.IsLoggedIn)
                {
                    currentUser = await _playerUI.LoginOrRegisterAsync();

                    if (!currentUser.IsLoggedIn)
                    {
                        AnsiConsole.MarkupLine("[yellow]Arrivederci![/]");
                        return;
                    }
                }

                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Tomacco").Color(Color.Green));
                AnsiConsole.Write(new Rule($"[cyan]Benvenuto, {currentUser.Username}![/]").RuleStyle("cyan"));
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Cosa vuoi fare?[/]")
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .AddChoiceGroup("[yellow]👤 Profilo[/]", new[]
                        {
                            "   Il mio profilo",
                            "   Lista giocatori"
                        })
                        .AddChoiceGroup("[yellow]🎮 Partite[/]", new[]
                        {
                            "   Crea partita",
                            "   Partite disponibili",
                            "   Le mie partite",
                            "   Unisciti a partita",
                            "   Dettagli partita",
                            "   Avvia partita",
                            "   Termina partita"
                        })
                        .AddChoiceGroup("[yellow]⚙️ Sistema[/]", new[]
                        {
                            "   Logout",
                            "   Esci"
                        }));

                var trimmedChoice = choice.Trim();

                switch (trimmedChoice)
                {
                    case "Il mio profilo":
                        await _playerUI.ShowProfileAsync(currentUser.TelegramId);
                        break;
                    case "Lista giocatori":
                        await _playerUI.ShowAllPlayersAsync();
                        break;
                    case "Crea partita":
                        await _gameUI.CreateGameAsync(currentUser);
                        break;
                    case "Partite disponibili":
                        await _gameUI.ListGamesAsync();
                        break;
                    case "Le mie partite":
                        await _gameUI.MyGamesAsync(currentUser);
                        break;
                    case "Unisciti a partita":
                        await _gameUI.JoinGameAsync(currentUser);
                        break;
                    case "Dettagli partita":
                        await _gameUI.ViewGameDetailsAsync(currentUser);
                        break;
                    case "Avvia partita":
                        await _gameUI.StartGameAsync(currentUser);
                        break;
                    case "Termina partita":
                        await _gameUI.EndGameAsync(currentUser);
                        break;
                    case "Logout":
                        currentUser = null;
                        AnsiConsole.MarkupLine("[yellow]Logout effettuato![/]");
                        await Task.Delay(1000);
                        break;
                    case "Esci":
                        AnsiConsole.MarkupLine("[yellow]Arrivederci![/]");
                        return;
                }
            }
        }
    }
}
