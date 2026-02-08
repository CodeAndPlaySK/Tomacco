using Application.Interfaces;
using Console.Models;
using Domain.Enums;
using Domain.Exceptions;
using Spectre.Console;

namespace Console.Services
{
    public class ConsoleGameUI
    {
        private readonly IGameService _gameService;

        public ConsoleGameUI(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task CreateGameAsync(ConsoleUser user)
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[green]🎮 Crea Nuova Partita[/]").RuleStyle("green"));
                AnsiConsole.WriteLine();

                // Chiedi configurazione
                var citySlots = AnsiConsole.Prompt(
                    new TextPrompt<int>("Quanti slot per la città? (default 12)")
                        .DefaultValue(12)
                        .ValidationErrorMessage("[red]Inserisci un numero tra 6 e 20[/]")
                        .Validate(n => n >= 6 && n <= 20));

                var game = await AnsiConsole.Status()
                    .StartAsync("Creazione partita in corso...", async ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Dots);
                        ctx.SpinnerStyle(Style.Parse("green"));
                        return await _gameService.CreateGameAsync(user.TelegramId, citySlots: citySlots);
                    });

                var panel = new Panel($"""
                    ✅ Partita creata con successo!
                    
                    📋 Codice: [yellow]{game.Code}[/]
                    👤 Creatore: [cyan]{user.Username}[/]
                    👥 Giocatori: {game.Players.Count}/{game.MaxPlayers}
                    🏙️ Slot città: {game.CitySlots}
                    📊 Stato: [blue]In attesa di giocatori[/]
                    
                    💡 Condividi il codice con altri giocatori!
                    """)
                    .Header("[green]Partita Creata[/]")
                    .BorderColor(Color.Green)
                    .Padding(1, 1);

                AnsiConsole.Write(panel);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        public async Task ViewGameDetailsAsync(ConsoleUser user)
        {
            try
            {
                var gameCode = AnsiConsole.Ask<string>("Inserisci il [yellow]codice della partita[/]:").ToUpper();

                var game = await _gameService.GetGameByCodeAsync(gameCode);

                if (game == null)
                {
                    AnsiConsole.MarkupLine("[red]❌ Partita non trovata![/]");
                    AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
                    System.Console.ReadKey();
                    return;
                }

                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule($"[green]🎮 Partita {game.Code}[/]").RuleStyle("green"));
                AnsiConsole.WriteLine();

                // Info generali
                var infoTable = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Green)
                    .AddColumn("[yellow]Campo[/]")
                    .AddColumn("[cyan]Valore[/]");

                infoTable.AddRow("Codice", game.Code);
                infoTable.AddRow("Stato", GetStateDisplay(game.State));
                infoTable.AddRow("Creatore", game.PlayerCreator.Username);
                infoTable.AddRow("Giocatori", $"{game.Players.Count}/{game.MaxPlayers}");
                infoTable.AddRow("Turno Corrente", game.CurrentTurn.ToString());
                infoTable.AddRow("Creata", game.CreatedAt.ToString("dd/MM/yyyy HH:mm"));

                if (game.StartedAt.HasValue)
                    infoTable.AddRow("Iniziata", game.StartedAt.Value.ToString("dd/MM/yyyy HH:mm"));

                AnsiConsole.Write(new Panel(infoTable)
                    .Header("[green]Info Partita[/]")
                    .BorderColor(Color.Green));

                // Giocatori
                if (game.Players.Any())
                {
                    var playersTable = new Table()
                        .Border(TableBorder.Rounded)
                        .BorderColor(Color.Blue)
                        .AddColumn("[yellow]#[/]")
                        .AddColumn("[cyan]Giocatore[/]")
                        .AddColumn("[green]Famiglia[/]");

                    int i = 1;
                    foreach (var player in game.Players)
                    {
                        var family = game.Families.FirstOrDefault(f =>
                            f.Members.Any(m => m.PlayerTelegramId == player.TelegramId));

                        var isCreator = player.TelegramId == game.PlayerCreatorId ? " 👑" : "";

                        playersTable.AddRow(
                            i.ToString(),
                            player.Username + isCreator,
                            family != null ? $"{family.CoatOfArms} {family.Name}" : "[grey]N/A[/]"
                        );
                        i++;
                    }

                    AnsiConsole.Write(new Panel(playersTable)
                        .Header("[blue]Giocatori[/]")
                        .BorderColor(Color.Blue));
                }

                // Città (se esiste)
                if (game.City != null)
                {
                    var emptySlots = game.City.Slots.Count(s => s.IsEmpty);
                    var builtSlots = game.City.Slots.Count(s => !s.IsEmpty);

                    AnsiConsole.MarkupLine($"\n[cyan]🏙️ Città:[/] {game.City.Name}");
                    AnsiConsole.MarkupLine($"   📍 Slot: {emptySlots} vuoti / {builtSlots} costruiti");
                }

                // Famiglie (se esistono)
                if (game.Families.Any())
                {
                    AnsiConsole.WriteLine();
                    var familiesTable = new Table()
                        .Border(TableBorder.Rounded)
                        .BorderColor(Color.Yellow)
                        .AddColumn("[yellow]Stemma[/]")
                        .AddColumn("[cyan]Famiglia[/]")
                        .AddColumn("[gold1]Oro[/]")
                        .AddColumn("[purple]Influenza[/]");

                    foreach (var family in game.Families)
                    {
                        familiesTable.AddRow(
                            family.CoatOfArms,
                            family.Name,
                            family.Resources.Gold.ToString(),
                            family.Resources.Influence.ToString()
                        );
                    }

                    AnsiConsole.Write(new Panel(familiesTable)
                        .Header("[yellow]Famiglie[/]")
                        .BorderColor(Color.Yellow));
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        private static string GetStateDisplay(GameState state)
        {
            return state switch
            {
                GameState.NotStarted => "[blue]In attesa[/]",
                GameState.Running => "[green]In corso[/]",
                GameState.Ended => "[grey]Terminata[/]",
                GameState.Cancelled => "[red]Cancellata[/]",
                _ => "[grey]Sconosciuto[/]"
            };
        }

        public async Task ListGamesAsync()
        {
            try
            {
                var games = await _gameService.GetGamesByStateAsync(GameState.NotStarted);

                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[blue]📋 Partite Disponibili[/]").RuleStyle("blue"));
                AnsiConsole.WriteLine();

                if (!games.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]Nessuna partita disponibile al momento.[/]");
                    AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
                    System.Console.ReadKey();
                    return;
                }

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Blue)
                    .AddColumn("[yellow]Codice[/]")
                    .AddColumn("[cyan]Creatore[/]")
                    .AddColumn("[green]Giocatori[/]")
                    .AddColumn("[magenta]Stato[/]")
                    .AddColumn("[grey]Creata[/]");

                foreach (var game in games)
                {
                    var status = game.Players.Count >= game.MaxPlayers ? "🔒 Piena" : "🟢 Aperta";
                    var relativeTime = _GetRelativeTime(game.CreatedAt);

                    table.AddRow(
                        $"[yellow]{game.Code}[/]",
                        game.PlayerCreator.Username,
                        $"{game.Players.Count}/{game.MaxPlayers}",
                        status,
                        relativeTime
                    );
                }

                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        public async Task MyGamesAsync(ConsoleUser user)
        {
            try
            {
                var games = await _gameService.GetPlayerGamesAsync(user.TelegramId);

                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[green]🎮 Le Mie Partite[/]").RuleStyle("green"));
                AnsiConsole.WriteLine();

                if (!games.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]Non sei ancora in nessuna partita.[/]");
                    AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
                    System.Console.ReadKey();
                    return;
                }

                var gamesByState = games.GroupBy(g => g.State);

                foreach (var group in gamesByState.OrderBy(g => g.Key))
                {
                    var stateInfo = _GetStateInfo(group.Key);
                    AnsiConsole.MarkupLine($"\n[bold]{stateInfo.Emoji} {stateInfo.Name}[/] ({group.Count()})");

                    var table = new Table()
                        .Border(TableBorder.Rounded)
                        .BorderColor(stateInfo.Color)
                        .AddColumn("[yellow]Codice[/]")
                        .AddColumn("[cyan]Creatore[/]")
                        .AddColumn("[green]Giocatori[/]")
                        .AddColumn("[grey]Data[/]");

                    foreach (var game in group)
                    {
                        var isCreator = game.PlayerCreatorId == user.TelegramId ? "👑 " : "";
                        table.AddRow(
                            $"[yellow]{game.Code}[/]",
                            $"{isCreator}{game.PlayerCreator.Username}",
                            $"{game.Players.Count}/{game.MaxPlayers}",
                            game.CreatedAt.ToString("dd/MM HH:mm")
                        );
                    }

                    AnsiConsole.Write(table);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        public async Task JoinGameAsync(ConsoleUser user)
        {
            try
            {
                var gameCode = AnsiConsole.Ask<string>("Inserisci il [yellow]codice della partita[/]:").ToUpper();

                if (!await _gameService.CanPlayerJoinGameAsync(gameCode, user.TelegramId))
                {
                    AnsiConsole.MarkupLine("[red]❌ Non puoi unirti a questa partita![/]");
                    AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
                    System.Console.ReadKey();
                    return;
                }

                var game = await AnsiConsole.Status()
                    .StartAsync("Unione alla partita in corso...", async ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Dots);
                        return await _gameService.AddPlayerToGameAsync(gameCode, user.TelegramId);
                    });

                AnsiConsole.MarkupLine($"[green]✅ Ti sei unito alla partita {game.Code}![/]");
                AnsiConsole.MarkupLine($"👥 Giocatori: {game.Players.Count}/{game.MaxPlayers}");
            }
            catch (GameNotFoundException)
            {
                AnsiConsole.MarkupLine("[red]❌ Partita non trovata![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        public async Task StartGameAsync(ConsoleUser user)
        {
            try
            {
                var gameCode = AnsiConsole.Ask<string>("Inserisci il [yellow]codice della partita[/] da avviare:").ToUpper();

                var game = await AnsiConsole.Status()
                    .StartAsync("Avvio partita in corso...", async ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Dots);
                        ctx.Status("Creazione città...");
                        await Task.Delay(500);

                        ctx.Status("Creazione famiglie...");
                        await Task.Delay(500);

                        ctx.Status("Inizializzazione gioco...");
                        return await _gameService.StartGameAsync(gameCode, user.TelegramId);
                    });

                // Mostra info città
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule($"[green]🚀 Partita {game.Code} Avviata![/]").RuleStyle("green"));
                AnsiConsole.WriteLine();

                // Info città
                if (game.City != null)
                {
                    var cityTable = new Table()
                        .Border(TableBorder.Rounded)
                        .BorderColor(Color.Cyan)
                        .AddColumn("[yellow]🏙️ Città[/]")
                        .AddColumn("[cyan]Info[/]");

                    cityTable.AddRow("Nome", game.City.Name);
                    cityTable.AddRow("Descrizione", game.City.Description);
                    cityTable.AddRow("Slot disponibili", game.City.Slots.Count.ToString());

                    AnsiConsole.Write(new Panel(cityTable)
                        .Header("[cyan]Città Generata[/]")
                        .BorderColor(Color.Cyan));

                    // Lista slot
                    var slotsTable = new Table()
                        .Border(TableBorder.Rounded)
                        .BorderColor(Color.Blue)
                        .AddColumn("[yellow]#[/]")
                        .AddColumn("[cyan]Nome Slot[/]")
                        .AddColumn("[green]Stato[/]");

                    foreach (var slot in game.City.Slots.OrderBy(s => s.Position))
                    {
                        slotsTable.AddRow(
                            slot.Position.ToString(),
                            slot.Name,
                            slot.IsEmpty ? "[grey]Vuoto[/]" : $"[green]{slot.Building?.Name}[/]"
                        );
                    }

                    AnsiConsole.Write(new Panel(slotsTable)
                        .Header("[blue]Slot Edifici[/]")
                        .BorderColor(Color.Blue));
                }

                AnsiConsole.WriteLine();

                // Info famiglie
                if (game.Families.Any())
                {
                    var familiesTable = new Table()
                        .Border(TableBorder.Rounded)
                        .BorderColor(Color.Yellow)
                        .AddColumn("[yellow]Stemma[/]")
                        .AddColumn("[cyan]Famiglia[/]")
                        .AddColumn("[green]Giocatore[/]")
                        .AddColumn("[gold1]Oro[/]")
                        .AddColumn("[purple]Influenza[/]");

                    foreach (var family in game.Families)
                    {
                        var owner = family.Members.FirstOrDefault(m => m.IsOwner);
                        familiesTable.AddRow(
                            family.CoatOfArms,
                            family.Name,
                            owner?.Player.Username ?? "N/A",
                            family.Resources.Gold.ToString(),
                            family.Resources.Influence.ToString()
                        );
                    }

                    AnsiConsole.Write(new Panel(familiesTable)
                        .Header("[yellow]👨‍👩‍👧‍👦 Famiglie in Gioco[/]")
                        .BorderColor(Color.Yellow));
                }

                AnsiConsole.MarkupLine($"\n[green]🎮 Buona fortuna a tutti i giocatori![/]");
            }
            catch (GameNotFoundException)
            {
                AnsiConsole.MarkupLine("[red]❌ Partita non trovata![/]");
            }
            catch (UnauthorizedGameActionException)
            {
                AnsiConsole.MarkupLine("[red]❌ Solo il creatore può avviare la partita![/]");
            }
            catch (InvalidOperationException ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ {ex.Message}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        public async Task EndGameAsync(ConsoleUser user)
        {
            try
            {
                var gameCode = AnsiConsole.Ask<string>("Inserisci il [yellow]codice della partita[/] da terminare:").ToUpper();

                var game = await AnsiConsole.Status()
                    .StartAsync("Chiusura partita in corso...", async ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Dots);
                        return await _gameService.EndGameAsync(gameCode, user.TelegramId);
                    });

                var duration = game.EndedAt.HasValue && game.StartedAt.HasValue
                    ? game.EndedAt.Value - game.StartedAt.Value
                    : TimeSpan.Zero;

                AnsiConsole.MarkupLine($"[green]🏁 Partita {game.Code} terminata![/]");
                AnsiConsole.MarkupLine($"⏱️ Durata: {_FormatDuration(duration)}");
                AnsiConsole.MarkupLine($"🎯 Turni giocati: {game.Turns.Count}");
            }
            catch (GameNotFoundException)
            {
                AnsiConsole.MarkupLine("[red]❌ Partita non trovata![/]");
            }
            catch (UnauthorizedGameActionException)
            {
                AnsiConsole.MarkupLine("[red]❌ Solo il creatore può terminare la partita![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Errore: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nPremi un tasto per continuare...");
            System.Console.ReadKey();
        }

        private static string _GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Ora";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m fa";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h fa";

            return dateTime.ToString("dd/MM HH:mm");
        }

        private static string _FormatDuration(TimeSpan duration)
        {
            if (duration.TotalMinutes < 1)
                return "< 1 minuto";
            if (duration.TotalHours < 1)
                return $"{(int)duration.TotalMinutes} minuti";
            if (duration.TotalDays < 1)
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";

            return $"{(int)duration.TotalDays}g {duration.Hours}h";
        }

        private static (string Emoji, string Name, Color Color) _GetStateInfo(GameState state)
        {
            return state switch
            {
                GameState.NotStarted => ("🔵", "In attesa", Color.Blue),
                GameState.Running => ("🟢", "In corso", Color.Green),
                GameState.Ended => ("⚫", "Terminate", Color.Grey),
                GameState.Cancelled => ("🔴", "Cancellate", Color.Red),
                _ => ("❓", "Sconosciuto", Color.White)
            };
        }
    }
}
