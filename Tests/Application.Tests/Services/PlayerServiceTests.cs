using Application.Interfaces;
using Application.Services;
using Application.Tests.Helpers;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ILogger<PlayerService>> _loggerMock;
        private readonly PlayerService _sut; // System Under Test

        public PlayerServiceTests()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _auditServiceMock = new Mock<IAuditService>();
            _loggerMock = new Mock<ILogger<PlayerService>>();

            _sut = new PlayerService(
                _playerRepositoryMock.Object,
                _auditServiceMock.Object,
                _loggerMock.Object
            );
        }

        #region CreatePlayerAsync Tests

        [Fact]
        public async Task CreatePlayerAsync_WithValidData_ShouldCreatePlayer()
        {
            // Arrange
            var telegramId = "123456789";
            var username = "TestUser";
            var languageCode = "en";

            _playerRepositoryMock
                .Setup(x => x.ExistsByTelegramIdAsync(telegramId))
                .ReturnsAsync(false);

            _playerRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Player>()))
                .ReturnsAsync((Player p) => p);

            // Act
            var result = await _sut.CreatePlayerAsync(telegramId, username, languageCode);

            // Assert
            result.Should().NotBeNull();
            result.TelegramId.Should().Be(telegramId);
            result.Username.Should().Be(username);
            result.LanguageCode.Should().Be(languageCode);
            result.IsActive.Should().BeTrue();

            _playerRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Player>(p =>
                    p.TelegramId == telegramId &&
                    p.Username == username &&
                    p.LanguageCode == languageCode
                )),
                Times.Once
            );

            _auditServiceMock.Verify(
                x => x.LogAsync(
                    AuditAction.Create,
                    nameof(Player),
                    telegramId,
                    telegramId,
                    username,
                    null,
                    It.IsAny<object>(),
                    null
                ),
                Times.Once
            );
        }

        [Theory]
        [InlineData(null, "username")]
        [InlineData("", "username")]
        [InlineData("   ", "username")]
        public async Task CreatePlayerAsync_WithInvalidTelegramId_ShouldThrowArgumentException(
            string invalidTelegramId,
            string username)
        {
            // Act
            Func<Task> act = async () => await _sut.CreatePlayerAsync(invalidTelegramId, username);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*TelegramId*");
        }

        [Theory]
        [InlineData("123456", null)]
        [InlineData("123456", "")]
        [InlineData("123456", "   ")]
        public async Task CreatePlayerAsync_WithInvalidUsername_ShouldThrowArgumentException(
            string telegramId,
            string invalidUsername)
        {
            // Act
            Func<Task> act = async () => await _sut.CreatePlayerAsync(telegramId, invalidUsername);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Username*");
        }

        [Fact]
        public async Task CreatePlayerAsync_WithExistingTelegramId_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var telegramId = "123456789";
            var username = "TestUser";

            _playerRepositoryMock
                .Setup(x => x.ExistsByTelegramIdAsync(telegramId))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _sut.CreatePlayerAsync(telegramId, username);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already exists*");
        }

        #endregion

        #region GetPlayerByTelegramIdAsync Tests

        [Fact]
        public async Task GetPlayerByTelegramIdAsync_WithExistingPlayer_ShouldReturnPlayer()
        {
            // Arrange
            var player = TestDataBuilder.CreatePlayer();

            _playerRepositoryMock
                .Setup(x => x.GetByTelegramIdAsync(player.TelegramId))
                .ReturnsAsync(player);

            // Act
            var result = await _sut.GetPlayerByTelegramIdAsync(player.TelegramId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(player);
        }

        [Fact]
        public async Task GetPlayerByTelegramIdAsync_WithNonExistingPlayer_ShouldReturnNull()
        {
            // Arrange
            var telegramId = "nonexistent";

            _playerRepositoryMock
                .Setup(x => x.GetByTelegramIdAsync(telegramId))
                .ReturnsAsync((Player?)null);

            // Act
            var result = await _sut.GetPlayerByTelegramIdAsync(telegramId);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region UpdatePlayerAsync Tests

        [Fact]
        public async Task UpdatePlayerAsync_WithExistingPlayer_ShouldUpdatePlayer()
        {
            // Arrange
            var existingPlayer = TestDataBuilder.CreatePlayer();
            var updatedPlayer = TestDataBuilder.CreatePlayer(existingPlayer.TelegramId, "NewUsername");

            _playerRepositoryMock
                .Setup(x => x.GetByTelegramIdAsync(existingPlayer.TelegramId))
                .ReturnsAsync(existingPlayer);

            _playerRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdatePlayerAsync(updatedPlayer);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("NewUsername");

            _playerRepositoryMock.Verify(
                x => x.UpdateAsync(It.Is<Player>(p => p.Username == "NewUsername")),
                Times.Once
            );

            _auditServiceMock.Verify(
                x => x.LogAsync(
                    AuditAction.Update,
                    nameof(Player),
                    existingPlayer.TelegramId,
                    updatedPlayer.TelegramId,
                    updatedPlayer.Username,
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    null
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdatePlayerAsync_WithNullPlayer_ShouldThrowArgumentNullException()
        {
            // Act
            Func<Task> act = async () => await _sut.UpdatePlayerAsync(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePlayerAsync_WithNonExistingPlayer_ShouldThrowPlayerNotFoundException()
        {
            // Arrange
            var player = TestDataBuilder.CreatePlayer();

            _playerRepositoryMock
                .Setup(x => x.GetByTelegramIdAsync(player.TelegramId))
                .ReturnsAsync((Player?)null);

            // Act
            Func<Task> act = async () => await _sut.UpdatePlayerAsync(player);

            // Assert
            await act.Should().ThrowAsync<PlayerNotFoundException>()
                .Where(ex => ex.TelegramId == player.TelegramId);
        }

        #endregion

        #region UpdatePlayerLanguageAsync Tests

        [Theory]
        [InlineData("en")]
        [InlineData("it")]
        [InlineData("de")]
        public async Task UpdatePlayerLanguageAsync_WithValidLanguage_ShouldUpdateLanguage(string newLanguage)
        {
            // Arrange
            var player = TestDataBuilder.CreatePlayer(languageCode: "en");

            _playerRepositoryMock
                .Setup(x => x.GetByTelegramIdAsync(player.TelegramId))
                .ReturnsAsync(player);

            _playerRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Player>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdatePlayerLanguageAsync(player.TelegramId, newLanguage);

            // Assert
            result.Should().NotBeNull();
            result.LanguageCode.Should().Be(newLanguage);

            _auditServiceMock.Verify(
                x => x.LogAsync(
                    AuditAction.LanguageChange,
                    nameof(Player),
                    player.TelegramId,
                    player.TelegramId,
                    player.Username,
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    null
                ),
                Times.Once
            );
        }

        #endregion

        #region GetAllPlayersAsync Tests

        [Fact]
        public async Task GetAllPlayersAsync_ShouldReturnAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                TestDataBuilder.CreatePlayer(),
                TestDataBuilder.CreatePlayer(),
                TestDataBuilder.CreatePlayer()
            };

            _playerRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _sut.GetAllPlayersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(players);
        }

        [Fact]
        public async Task GetAllPlayersAsync_WithNoPlayers_ShouldReturnEmptyList()
        {
            // Arrange
            _playerRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Player>());

            // Act
            var result = await _sut.GetAllPlayersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion
    }
}
