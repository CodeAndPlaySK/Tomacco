using Spectre.Console;
using Application.Interfaces;
using Console.Models;
using Color = Spectre.Console.Color;
using Table = Spectre.Console.Table;

namespace Console.Services
{
    public class ConsolePlayerUI
    {
        private readonly IPlayerService _playerService;

        public ConsolePlayerUI(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public async Task<ConsoleUser> LoginOrRegisterAsync()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Tomacco").Color(Color.Green));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Benvenuto![/]")
                    .AddChoices("Login", "Registrati", "Esci"));

            return choice switch
            {
                "Login" => await LoginAsync(),
                "Registrati" => await RegisterAsync(),
                _ => new ConsoleUser()
            };
        }

        private async Task<ConsoleUser> LoginAsync()
        {
            var username = AnsiConsole.Ask<string>("Inserisci il tuo [green]username[/]:");

            try
            {
                var player = await _playerService.GetPlayerByUsernameAsync(username);

                if (player == null)
                {
                    AnsiConsole.MarkupLine("[red]Utente non trovato![/]");
                    AnsiConsole.WriteLine("Premi un tasto per continuare...");
                    System.Console.ReadKey();
                    return await LoginOrRegisterAsync();
                }

                player.LastLoginAt = DateTime.UtcNow;
                await _playerService.UpdatePlayerAsync(player);

                AnsiConsole.MarkupLine($"[green]Benvenuto {player.Username}![/]");
                await Task.Delay(1000);

                return new ConsoleUser
                {
                    TelegramId = player.TelegramId,
                    Username = player.Username,
                    LanguageCode = player.LanguageCode
                };
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Errore: {ex.Message}[/]");
                AnsiConsole.WriteLine("Premi un tasto per continuare...");
                System.Console.ReadKey();
                return new ConsoleUser();
            }
        }

        private async Task<ConsoleUser> RegisterAsync()
        {
            var username = AnsiConsole.Ask<string>("Scegli un [green]username[/]:");
            var telegramId = Guid.NewGuid().ToString("N")[..10]; // ID simulato

            try
            {
                var player = await _playerService.CreatePlayerAsync(telegramId, username, "it");

                AnsiConsole.MarkupLine($"[green]Registrazione completata! Benvenuto {player.Username}![/]");
                await Task.Delay(1000);

                return new ConsoleUser
                {
                    TelegramId = player.TelegramId,
                    Username = player.Username,
                    LanguageCode = player.LanguageCode
                };
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Errore: {ex.Message}[/]");
                AnsiConsole.WriteLine("Premi un tasto per continuare...");
                System.Console.ReadKey();
                return new ConsoleUser();
            }
        }

        public async Task ShowProfileAsync(string telegramId)
        {
            try
            {
                var player = await _playerService.GetPlayerByTelegramIdAsync(telegramId);

                if (player == null)
                {
                    AnsiConsole.MarkupLine("[red]Player non trovato![/]");
                    return;
                }

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Green)
                    .AddColumn("[yellow]Campo[/]")
                    .AddColumn("[cyan]Valore[/]");

                table.AddRow("Username", player.Username);
                table.AddRow("ID", player.TelegramId);
                table.AddRow("Lingua", player.LanguageCode);
                table.AddRow("Registrato", player.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
                table.AddRow("Ultimo accesso", player.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");

                AnsiConsole.Write(
                    new Panel(table)
                        .Header("[green]👤 Il tuo Profilo[/]")
                        .BorderColor(Color.Green));
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        public async Task ShowAllPlayersAsync()
        {
            try
            {
                var players = await _playerService.GetAllPlayersAsync();

                if (!players.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]Nessun giocatore registrato.[/]");
                    AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
                    System.Console.ReadKey();
                    return;
                }

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Blue)
                    .AddColumn("[yellow]#[/]")
                    .AddColumn("[cyan]Username[/]")
                    .AddColumn("[green]Ultimo Accesso[/]");

                int i = 1;
                foreach (var player in players.OrderByDescending(p => p.LastLoginAt))
                {
                    var lastSeen = player.LastLoginAt.HasValue
                        ? _GetRelativeTime(player.LastLoginAt.Value)
                        : "mai";

                    table.AddRow(i.ToString(), player.Username, lastSeen);
                    i++;
                }

                AnsiConsole.Write(
                    new Panel(table)
                        .Header($"[blue]👥 Giocatori Registrati ({players.Count()})[/]")
                        .BorderColor(Color.Blue));
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        private static string _GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "ora";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m fa";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h fa";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}g fa";

            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
