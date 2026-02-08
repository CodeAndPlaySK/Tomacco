using System.Security.Cryptography;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Factories;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICityFactory _cityFactory;
        private readonly IFamilyFactory _familyFactory;
        private readonly IAuditService _auditService;
        private readonly ILogger<GameService> _logger;

        public GameService(
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            ICityFactory cityFactory,
            IFamilyFactory familyFactory,
            IAuditService auditService,
            ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _cityFactory = cityFactory;
            _familyFactory = familyFactory;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Game> CreateGameAsync(
            string creatorTelegramId,
            List<string>? playerTelegramIds = null,
            int? citySlots = null)
        {
            _logger.LogInformation("Creating new game for creator {CreatorId}", creatorTelegramId);

            var creator = await _playerRepository.GetByTelegramIdAsync(creatorTelegramId);
            if (creator == null)
                throw new PlayerNotFoundException(creatorTelegramId);

            var gameCode = await GenerateUniqueGameCodeAsync();

            var game = new Game
            {
                Code = gameCode,
                PlayerCreatorId = creatorTelegramId,
                PlayerCreator = creator,
                State = GameState.NotStarted,
                CreatedAt = DateTime.UtcNow,
                CitySlots = citySlots ?? 12,
                Players = new List<Player> { creator },
                Families = new List<Family>()
            };

            // Aggiungi altri giocatori se specificati
            if (playerTelegramIds != null && playerTelegramIds.Any())
            {
                foreach (var playerId in playerTelegramIds.Where(id => id != creatorTelegramId))
                {
                    var player = await _playerRepository.GetByTelegramIdAsync(playerId);
                    if (player != null)
                    {
                        game.Players.Add(player);
                    }
                }
            }

            await _gameRepository.AddAsync(game);

            await _auditService.LogAsync(
                AuditAction.Create,
                nameof(Game),
                gameCode,
                creatorTelegramId,
                creator.Username,
                newValues: new { game.Code, game.State, PlayerCount = game.Players.Count }
            );

            _logger.LogInformation("Game {GameCode} created with {PlayerCount} players",
                gameCode, game.Players.Count);

            return game;
        }

        public async Task<Game> StartGameAsync(string gameCode, string requestingPlayerId)
        {
            _logger.LogInformation("Starting game {GameCode} by player {PlayerId}", gameCode, requestingPlayerId);

            var game = await _gameRepository.GetByCodeWithDetailsAsync(gameCode);
            if (game == null)
                throw new GameNotFoundException(gameCode);

            if (game.PlayerCreatorId != requestingPlayerId)
                throw new UnauthorizedGameActionException(gameCode, requestingPlayerId, "start");

            if (game.State != GameState.NotStarted)
                throw new InvalidOperationException($"Game is already in state '{game.State}'");

            if (game.Players.Count < game.MinPlayers)
                throw new InvalidOperationException($"Not enough players (minimum {game.MinPlayers})");

            // 1. Crea la città con gli slot
            _logger.LogInformation("Creating city for game {GameCode}", gameCode);
            var city = _cityFactory.CreateCity(gameCode, game.CitySlots);
            game.City = city;

            // 2. Crea una famiglia per ogni giocatore
            _logger.LogInformation("Creating families for {PlayerCount} players", game.Players.Count);
            foreach (var player in game.Players)
            {
                var family = _familyFactory.CreateFamily(
                    gameCode,
                    player,
                    game.InitialGold,
                    game.InitialInfluence
                );
                game.Families.Add(family);
                _logger.LogInformation("Created family '{FamilyName}' for player {PlayerName}",
                    family.Name, player.Username);
            }

            // 3. Avvia il gioco
            var oldState = game.State;
            game.State = GameState.Running;
            game.StartedAt = DateTime.UtcNow;
            game.CurrentTurn = 1;

            await _gameRepository.UpdateAsync(game);

            await _auditService.LogAsync(
                AuditAction.GameStart,
                nameof(Game),
                gameCode,
                requestingPlayerId,
                game.PlayerCreator.Username,
                oldValues: new { State = oldState },
                newValues: new
                {
                    game.State,
                    game.StartedAt,
                    CityName = city.Name,
                    CitySlots = city.Slots.Count,
                    FamilyCount = game.Families.Count
                }
            );

            _logger.LogInformation("Game {GameCode} started with city '{CityName}' and {FamilyCount} families",
                gameCode, city.Name, game.Families.Count);

            return game;
        }

        // ... altri metodi esistenti ...

        public async Task<Game?> GetGameByCodeAsync(string gameCode)
        {
            return await _gameRepository.GetByCodeWithDetailsAsync(gameCode);
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await _gameRepository.GetRecentGamesAsync(100);
        }

        public async Task<IEnumerable<Game>> GetGamesByStateAsync(GameState state)
        {
            return await _gameRepository.GetGamesByStateAsync(state);
        }

        public async Task<IEnumerable<Game>> GetPlayerGamesAsync(string playerTelegramId)
        {
            return await _gameRepository.GetGamesByPlayerAsync(playerTelegramId);
        }

        public async Task<int> GetPlayerGameCountAsync(string playerTelegramId, GameState? state = null)
        {
            return await _gameRepository.GetPlayerGameCountAsync(playerTelegramId, state);
        }

        public async Task<bool> CanPlayerJoinGameAsync(string gameCode, string playerTelegramId)
        {
            var game = await _gameRepository.GetByCodeWithDetailsAsync(gameCode);

            if (game == null) return false;
            if (game.State != GameState.NotStarted) return false;
            if (game.Players.Count >= game.MaxPlayers) return false;
            if (game.Players.Any(p => p.TelegramId == playerTelegramId)) return false;

            return true;
        }

        public async Task<Game> AddPlayerToGameAsync(string gameCode, string playerTelegramId)
        {
            var game = await _gameRepository.GetByCodeWithDetailsAsync(gameCode);
            if (game == null)
                throw new GameNotFoundException(gameCode);

            if (game.State != GameState.NotStarted)
                throw new InvalidOperationException($"Cannot add players to a game in state '{game.State}'");

            if (game.Players.Count >= game.MaxPlayers)
                throw new InvalidOperationException($"Game is full (max {game.MaxPlayers} players)");

            var player = await _playerRepository.GetByTelegramIdAsync(playerTelegramId);
            if (player == null)
                throw new PlayerNotFoundException(playerTelegramId);

            if (game.Players.Any(p => p.TelegramId == playerTelegramId))
                throw new InvalidOperationException("Player is already in the game");

            game.Players.Add(player);
            await _gameRepository.UpdateAsync(game);

            return game;
        }

        public async Task<Game> EndGameAsync(string gameCode, string requestingPlayerId)
        {
            var game = await _gameRepository.GetByCodeWithDetailsAsync(gameCode);
            if (game == null)
                throw new GameNotFoundException(gameCode);

            if (game.PlayerCreatorId != requestingPlayerId)
                throw new UnauthorizedGameActionException(gameCode, requestingPlayerId, "end");

            if (game.State != GameState.Running)
                throw new InvalidOperationException($"Cannot end a game in state '{game.State}'");

            var oldState = game.State;
            game.State = GameState.Ended;
            game.EndedAt = DateTime.UtcNow;

            await _gameRepository.UpdateAsync(game);

            await _auditService.LogAsync(
                AuditAction.GameEnd,
                nameof(Game),
                gameCode,
                requestingPlayerId,
                game.PlayerCreator.Username,
                oldValues: new { State = oldState },
                newValues: new { game.State, game.EndedAt }
            );

            return game;
        }

        private async Task<string> GenerateUniqueGameCodeAsync()
        {
            string code;
            bool exists;

            do
            {
                code = GenerateRandomCode(6);
                exists = await _gameRepository.ExistsByCodeAsync(code);
            } while (exists);

            return code;
        }

        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }

            return new string(result);
        }
    }
}
