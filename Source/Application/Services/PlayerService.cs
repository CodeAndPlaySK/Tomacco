using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(
            IPlayerRepository playerRepository,
            IAuditService auditService,
            ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Player> CreatePlayerAsync(string telegramId, string username, string languageCode = "en")
        {
            _logger.LogInformation("Creating player: {TelegramId}, {Username}", telegramId, username);

            if (string.IsNullOrWhiteSpace(telegramId))
                throw new ArgumentException("TelegramId cannot be empty", nameof(telegramId));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (await _playerRepository.ExistsByTelegramIdAsync(telegramId))
            {
                _logger.LogWarning("Player {TelegramId} already exists", telegramId);
                throw new InvalidOperationException($"Player with TelegramId '{telegramId}' already exists");
            }

            var player = new Player
            {
                TelegramId = telegramId,
                Username = username,
                LanguageCode = languageCode,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true
            };

            await _playerRepository.AddAsync(player);

            await _auditService.LogAsync(
                AuditAction.Create,
                nameof(Player),
                telegramId,
                telegramId,
                username,
                newValues: new { player.TelegramId, player.Username, player.LanguageCode }
            );

            _logger.LogInformation("Player created: {TelegramId}, {Username}", telegramId, username);

            return player;
        }

        public async Task<Player?> GetPlayerByTelegramIdAsync(string telegramId)
        {
            _logger.LogDebug("Getting player by TelegramId: {TelegramId}", telegramId);
            return await _playerRepository.GetByTelegramIdAsync(telegramId);
        }

        public async Task<Player?> GetPlayerByUsernameAsync(string username)
        {
            _logger.LogDebug("Getting player by Username: {Username}", username);
            return await _playerRepository.GetByUsernameAsync(username);
        }

        public async Task<bool> PlayerExistsAsync(string telegramId)
        {
            return await _playerRepository.ExistsByTelegramIdAsync(telegramId);
        }

        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var existingPlayer = await _playerRepository.GetByTelegramIdAsync(player.TelegramId);
            if (existingPlayer == null)
                throw new PlayerNotFoundException(player.TelegramId);

            var oldValues = new { existingPlayer.Username, existingPlayer.LanguageCode };

            existingPlayer.Username = player.Username;
            existingPlayer.LastLoginAt = player.LastLoginAt;

            await _playerRepository.UpdateAsync(existingPlayer);

            await _auditService.LogAsync(
                AuditAction.Update,
                nameof(Player),
                player.TelegramId,
                player.TelegramId,
                player.Username,
                oldValues: oldValues,
                newValues: new { player.Username, player.LanguageCode }
            );

            _logger.LogInformation("Player updated: {TelegramId}", player.TelegramId);

            return existingPlayer;
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            _logger.LogDebug("Getting all players");
            return await _playerRepository.GetAllAsync();
        }

        public async Task<Player> UpdatePlayerLanguageAsync(string telegramId, string languageCode)
        {
            _logger.LogInformation("Updating language for player {TelegramId}: {LanguageCode}", telegramId, languageCode);

            var player = await _playerRepository.GetByTelegramIdAsync(telegramId);
            if (player == null)
                throw new PlayerNotFoundException(telegramId);

            var oldLanguage = player.LanguageCode;
            player.LanguageCode = languageCode;

            await _playerRepository.UpdateAsync(player);

            await _auditService.LogAsync(
                AuditAction.LanguageChange,
                nameof(Player),
                telegramId,
                telegramId,
                player.Username,
                oldValues: new { LanguageCode = oldLanguage },
                newValues: new { LanguageCode = languageCode }
            );

            _logger.LogInformation("Language changed from {OldLanguage} to {NewLanguage} for {Username}",
                oldLanguage, languageCode, player.Username);

            return player;
        }
    }
}
