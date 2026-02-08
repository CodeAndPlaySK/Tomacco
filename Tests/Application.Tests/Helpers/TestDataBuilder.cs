using Domain.Enums;
using Domain.Models;

namespace Application.Tests.Helpers
{
    public static class TestDataBuilder
    {
        public static Player CreatePlayer(
            string? telegramId = null,
            string? username = null,
            string languageCode = "en")
        {
            return new Player
            {
                TelegramId = telegramId ?? Guid.NewGuid().ToString("N")[..10],
                Username = username ?? $"TestUser_{Guid.NewGuid().ToString("N")[..8]}",
                LanguageCode = languageCode,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static Game CreateGame(
            string? code = null,
            Player? creator = null,
            GameState state = GameState.NotStarted)
        {
            var actualCreator = creator ?? CreatePlayer();

            return new Game
            {
                Code = code ?? GenerateGameCode(),
                PlayerCreatorId = actualCreator.TelegramId,
                PlayerCreator = actualCreator,
                State = state,
                CreatedAt = DateTime.UtcNow,
                MaxPlayers = 10,
                MinPlayers = 2,
                Players = new List<Player> { actualCreator }
            };
        }

        public static AuditLog CreateAuditLog(
            AuditAction action = AuditAction.Create,
            string? userId = null,
            string? username = null)
        {
            return new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                Action = action.ToString(),
                EntityType = "TestEntity",
                EntityId = Guid.NewGuid().ToString(),
                UserId = userId ?? "test-user-id",
                Username = username ?? "TestUser"
            };
        }

        private static string GenerateGameCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
