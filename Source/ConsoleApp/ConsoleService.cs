using System.Linq.Expressions;
using Application.Services;
using Domain.Services;

namespace ConsoleApp
{
    public interface IConsoleService
    {
        void Run(IMenuPageResolver menuPageResolver);
        IMenuPageResolver CreateMenuPageResolver();
    }

    public class ConsoleService : IConsoleService
    {
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;

        public ConsoleService(IGameService gameService, IPlayerService playerService)
        {
            _gameService = gameService;
            _playerService = playerService;
        }

        public void Run(IMenuPageResolver menuPageResolver)
        {
            menuPageResolver.DisplayMenu();
        }

        public IMenuPageResolver CreateMenuPageResolver()
        {
            var menuPageReg = new MenuPageRegistry();

            var invalidInputPage = new MenuPage("Invalid input", _ShowInvalidMenu);
            var initialPage = new MenuPage("Initial page", _ShowInitialMenu, isRootPage: true);
            var entitiesManagementPage = new MenuPage("Entities management", _ShowEntitiesManagementMenu);
            var gameManagementPage = new MenuPage("Game management", _ShowGameManagementMenu);
            var createNewGamePage = new MenuPage("Create new game", _ShowCreateNewGameMenu);
            var showAllGames = new MenuPage("Show all games", _ShowAllGamesMenu);
            var playerManagementPage = new MenuPage("Player management", _ShowPlayerManagementMenu);
            var configManagementPage = new MenuPage("Configuration management", _ShowConfigManagementMenu);
            var createNewPlayerPage = new MenuPage("Create new player", _ShowCreateNewPlayerMenu);
            var showAllPlayers = new MenuPage("Show all players", _ShowAllPlayersMenu);

            menuPageReg.Register(invalidInputPage);
            menuPageReg.Register(initialPage);
            menuPageReg.Register(entitiesManagementPage);
            menuPageReg.Register(gameManagementPage);
            menuPageReg.Register(createNewGamePage);
            menuPageReg.Register(showAllGames);
            menuPageReg.Register(playerManagementPage);
            menuPageReg.Register(configManagementPage);
            menuPageReg.Register(createNewPlayerPage);
            menuPageReg.Register(showAllPlayers);

            initialPage.OtherPages.Add(["E", "ENTITIES"], entitiesManagementPage);
            initialPage.OtherPages.Add(["C", "CONFIG", "CONFIGURATION"], configManagementPage);

            entitiesManagementPage.OtherPages.Add(["G", "GAME"], gameManagementPage);
            entitiesManagementPage.OtherPages.Add(["P", "PLAYER"], playerManagementPage);

            gameManagementPage.OtherPages.Add(["N", "NEW"], createNewGamePage);
            gameManagementPage.OtherPages.Add(["L", "LIST"], showAllGames);

            playerManagementPage.OtherPages.Add(["N", "NEW"], createNewPlayerPage);
            playerManagementPage.OtherPages.Add(["L", "LIST"], showAllPlayers);

            //var cityManagementPage = new MenuPage("City management", _ShowCityManagementMenu);

            //var familyManagementPage = new MenuPage("Family management", _ShowFamilyManagementMenu);

            return new MenuPageResolver(menuPageReg);
        }

        private void _ShowInvalidMenu()
        {
            Console.WriteLine("*************************");
            Console.WriteLine("Invalid input!");
            Console.WriteLine("*************************");
        }

        private void _ShowInitialMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" WELCOME TO TOMACCO CONSOLE");
            Console.WriteLine("***************************");
            Console.WriteLine();
            Console.WriteLine("E/Entities] Manage entities");
            Console.WriteLine("C/Config] Manage configuration");
            Console.WriteLine("exit] Terminate app");
        }

        private void _ShowEntitiesManagementMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" ENTITY MANAGEMENT         ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            Console.WriteLine("G/Game] Manage games");
            Console.WriteLine("P/Player] Manage players");
            Console.WriteLine();
            Console.WriteLine("B/Back] Return back");
        }

        private void _ShowGameManagementMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" GAME MANAGEMENT           ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            Console.WriteLine("N/New] Create new game");
            Console.WriteLine("S/Start] Start a game");
            Console.WriteLine("D/Delete] Delete a game");
            Console.WriteLine("L/List] List of all games");
            Console.WriteLine();
            Console.WriteLine("B/Back] Return back");
        }

        private void _ShowCreateNewGameMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" CREATE NEW GAME           ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            while (true)
            {
                var allPlayers = _playerService.GetAllPlayers();
                if (allPlayers.Count == 0)
                {
                    Console.WriteLine("There is no player available to be the game's creator. Game not created");
                    return;
                }
                Console.WriteLine("Choose the creator of the game from the list:");
                for (int idxPlayer = 0; idxPlayer < allPlayers.Count; idxPlayer++)
                {
                    Console.WriteLine($"{idxPlayer}: {allPlayers[idxPlayer].Username}");
                }

                var inputIdxPlayerCreator = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(inputIdxPlayerCreator) || !int.TryParse(inputIdxPlayerCreator, out var idxPlayerCreator)
                    || idxPlayerCreator < 0 || idxPlayerCreator >= allPlayers.Count)
                {
                    Console.WriteLine("Wrong input. Game not created.");
                    return;
                }

                var playerCreator = allPlayers[idxPlayerCreator];

                var newGame = _gameService.CreateGame(playerCreator, []);
                Console.WriteLine($"Game created successfully. Game code: '{newGame.Code}'");
                return;
            }
        }

        private void _ShowAllGamesMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" LIST GAMES                ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            var allGames = _gameService.GetAllGames();
            for (int i = 0; i < allGames.Count; i++)
            {
                var game = allGames[i];
                Console.WriteLine($"{i + 1}) {game.Code} ({game.State}) {game.Players.Count} players");
            }
        }


        private void _ShowConfigManagementMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" CONFIGURATION MANAGEMENT  ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            Console.WriteLine("R/Read] Read current configuration");
            Console.WriteLine("A/Assign] Reset configuration");
            Console.WriteLine("L/Load] Load configuration");
            Console.WriteLine();
            Console.WriteLine("B/Back] Return back");
        }

        private void _ShowPlayerManagementMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" PLAYER MANAGEMENT         ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            Console.WriteLine("N/New] Create new player");
            Console.WriteLine("A/Assign] Assign a player to a game");
            Console.WriteLine("L/List] List of all players");
            Console.WriteLine();
            Console.WriteLine("B/Back] Return back");
        }

        private void _ShowCreateNewPlayerMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" CREATE NEW PLAYER         ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            while (true)
            {
                Console.WriteLine("Insert username");
                var username = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }
                Console.WriteLine("Insert telegramId");
                var telegramId = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(telegramId))
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }
                Console.WriteLine($"Username:   {username}");
                Console.WriteLine($"TelegramId: {telegramId}");
                while (true)
                { 
                    Console.WriteLine("Do you confirm ? (Y/YES/N/NO)");
                    var decision = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        Console.WriteLine("Invalid input");
                        continue;
                    }

                    decision = decision!.Trim().ToUpper();
                    if (decision is "N" or "NO")
                    {
                        Console.WriteLine("Player creation interrupted!");
                        Console.WriteLine("X X X X X X X X X X ");
                        return;
                    }

                    if (_playerService.GetPlayer(telegramId) != null)
                    {
                        Console.WriteLine($"Player with telegram id '{telegramId}' already present! Creation failed");
                        Console.WriteLine("X X X X X X X X X X ");
                        return;

                    }

                    if (!_playerService.CreateNewPlayer(username, telegramId))
                    {
                        Console.WriteLine("Player creation failed!");
                        Console.WriteLine("X X X X X X X X X X ");
                        return;

                    }
                    Console.WriteLine("Player created successfully");
                    Console.WriteLine("☺ ☺ ☺ ☺ ☺ ☺ ☺ ☺ ☺ ☺ ☺");
                    return;
                }
            }
        }

        private void _ShowAllPlayersMenu()
        {
            Console.WriteLine("***************************");
            Console.WriteLine(" LIST PLAYERS              ");
            Console.WriteLine("***************************");
            Console.WriteLine();
            var allPlayers = _playerService.GetAllPlayers();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                var player = allPlayers[i];
                Console.WriteLine($"{i + 1}) {player.Username}");
            }
        }
    }
}
