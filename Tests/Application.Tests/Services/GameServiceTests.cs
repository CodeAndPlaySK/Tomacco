using Application.Interfaces;
using Application.Services;
using Application.Tests.Helpers;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Factories;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<ICityFactory> _cityFactoryMock;
        private readonly Mock<IFamilyFactory> _familyFactoryMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ILogger<GameService>> _loggerMock;
        private readonly GameService _sut;

        public GameServiceTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _cityFactoryMock = new Mock<ICityFactory>();
            _familyFactoryMock = new Mock<IFamilyFactory>();
            _auditServiceMock = new Mock<IAuditService>();
            _loggerMock = new Mock<ILogger<GameService>>();

            _sut = new GameService(
                _gameRepositoryMock.Object,
                _playerRepositoryMock.Object,
                _cityFactoryMock.Object,
                _familyFactoryMock.Object,
                _auditServiceMock.Object,
                _loggerMock.Object
            );
        }

        #region CreateGameAsync Tests

        [Fact]
        public async Task CreateGameAsync_WithValidCreator_ShouldCreateGame()
        {
            // Arrange
            var creator = TestDataBuilder.CreatePlayer();

            _playerRepositoryMock
                .Setup(x => x.GetByTelegramIdAsync(creator.TelegramId))
                .ReturnsAsync(creator);

            _gameRepositoryMock
                .Setup(x => x.ExistsByCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _gameRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Game>()))
                .ReturnsAsync((Game g) => g);

            // Act
            var result = await _sut.CreateGameAsync(creator.TelegramId);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().NotBeNullOrEmpty();
            result.PlayerCreatorId.Should().Be(creator.TelegramId);
            result.State.Should().Be(GameState.NotStarted);
            result.Players.Should().Contain(creator);

            _gameRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Game>(g =>
                    g.PlayerCreatorId == creator.TelegramId &&
                    g.State == GameState.NotStarted
                )),
                Times.Once
            );

            _auditServiceMock.Verify(
                x => x.LogAsync(
                    AuditAction.Create,
                    nameof(Game),
                    It.IsAny<string>(),
                    creator.TelegramId,
                    creator.Username,
                    null,
                    It.IsAny<object>(),
                    null
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateGameAsync_WithNonExistingCreator_ShouldThrowPlayerNotFoundException()
        {
            // Arrange
            var telegramId = "nonexistent";

            _playerRepositoryMock
                .Setup(x => x.GetByTelegramIdAsync(telegramId))
                .ReturnsAsync((Player?)null);

            // Act
            Func<Task> act = async () => await _sut.CreateGameAsync(telegramId);

            // Assert
            await act.Should().ThrowAsync<PlayerNotFoundException>()
                .Where(ex => ex.TelegramId == telegramId);
        }

        #endregion

        #region StartGameAsync Tests

        [Fact]
        public async Task StartGameAsync_WithValidGameAndCreator_ShouldStartGame()
        {
            // Arrange
            var creator = TestDataBuilder.CreatePlayer();
            var player2 = TestDataBuilder.CreatePlayer();
            var game = TestDataBuilder.CreateGame(creator: creator);
            game.Players.Add(player2);

            _gameRepositoryMock
                .Setup(x => x.GetByCodeWithDetailsAsync(game.Code))
                .ReturnsAsync(game);

            _gameRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Game>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.StartGameAsync(game.Code, creator.TelegramId);

            // Assert
            result.Should().NotBeNull();
            result.State.Should().Be(GameState.Running);
            result.StartedAt.Should().NotBeNull();
            result.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            _auditServiceMock.Verify(
                x => x.LogAsync(
                    AuditAction.GameStart,
                    nameof(Game),
                    game.Code,
                    creator.TelegramId,
                    creator.Username,
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    null
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task StartGameAsync_WithNonExistingGame_ShouldThrowGameNotFoundException()
        {
            // Arrange
            var gameCode = "NONEXIST";
            var playerId = "player123";

            _gameRepositoryMock
                .Setup(x => x.GetByCodeWithDetailsAsync(gameCode))
                .ReturnsAsync((Game?)null);

            // Act
            Func<Task> act = async () => await _sut.StartGameAsync(gameCode, playerId);

            // Assert
            await act.Should().ThrowAsync<GameNotFoundException>()
                .Where(ex => ex.GameCode == gameCode);
        }

        [Fact]
        public async Task StartGameAsync_WithNonCreator_ShouldThrowUnauthorizedGameActionException()
        {
            // Arrange
            var creator = TestDataBuilder.CreatePlayer();
            var otherPlayer = TestDataBuilder.CreatePlayer();
            var game = TestDataBuilder.CreateGame(creator: creator);
            game.Players.Add(otherPlayer);

            _gameRepositoryMock
                .Setup(x => x.GetByCodeWithDetailsAsync(game.Code))
                .ReturnsAsync(game);

            // Act
            Func<Task> act = async () => await _sut.StartGameAsync(game.Code, otherPlayer.TelegramId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedGameActionException>()
                .Where(ex =>
                    ex.GameCode == game.Code &&
                    ex.PlayerId == otherPlayer.TelegramId &&
                    ex.Action == "start"
                );
        }

        [Fact]
        public async Task StartGameAsync_WithNotEnoughPlayers_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var creator = TestDataBuilder.CreatePlayer();
            var game = TestDataBuilder.CreateGame(creator: creator);
            game.MinPlayers = 2;
            // Solo 1 player (il creator)

            _gameRepositoryMock
                .Setup(x => x.GetByCodeWithDetailsAsync(game.Code))
                .ReturnsAsync(game);

            // Act
            Func<Task> act = async () => await _sut.StartGameAsync(game.Code, creator.TelegramId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Not enough players*");
        }

        [Fact]
        public async Task StartGameAsync_WithAlreadyStartedGame_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var creator = TestDataBuilder.CreatePlayer();
            var game = TestDataBuilder.CreateGame(creator: creator, state: GameState.Running);
            game.Players.Add(TestDataBuilder.CreatePlayer());

            _gameRepositoryMock
                .Setup(x => x.GetByCodeWithDetailsAsync(game.Code))
                .ReturnsAsync(game);

            // Act
            Func<Task> act = async () => await _sut.StartGameAsync(game.Code, creator.TelegramId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already in state*");
        }

        #endregion

        #region EndGameAsync Tests

        [Fact]
        public async Task EndGameAsync_WithValidGameAndCreator_ShouldEndGame()
        {
            // Arrange
            var creator = TestDataBuilder.CreatePlayer();
            var game = TestDataBuilder.CreateGame(creator: creator, state: GameState.Running);
            game.StartedAt = DateTime.UtcNow.AddHours(-1);

            _gameRepositoryMock
                .Setup(x => x.GetByCodeWithDetailsAsync(game.Code))
                .ReturnsAsync(game);

            _gameRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Game>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.EndGameAsync(game.Code, creator.TelegramId);

            // Assert
            result.Should().NotBeNull();
            result.State.Should().Be(GameState.Ended);
            result.EndedAt.Should().NotBeNull();
            result.EndedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            _auditServiceMock.Verify(
                x => x.LogAsync(
                    AuditAction.GameEnd,
                    nameof(Game),
                    game.Code,
                    creator.TelegramId,
                    creator.Username,
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    null
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task EndGameAsync_WithNotRunningGame_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var creator = TestDataBuilder.CreatePlayer();
            var game = TestDataBuilder.CreateGame(creator: creator, state: GameState.NotStarted);

            _gameRepositoryMock
                .Setup(x => x.GetByCodeWithDetailsAsync(game.Code))
                .ReturnsAsync(game);

            // Act
            Func<Task> act = async () => await _sut.EndGameAsync(game.Code, creator.TelegramId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Cannot end a game in state*");
        }

        #endregion

        #region GetGamesByStateAsync Tests

        [Theory]
        [InlineData(GameState.NotStarted)]
        [InlineData(GameState.Running)]
        [InlineData(GameState.Ended)]
        public async Task GetGamesByStateAsync_ShouldReturnGamesWithSpecifiedState(GameState state)
        {
            // Arrange
            var games = new List<Game>
            {
                TestDataBuilder.CreateGame(state: state),
                TestDataBuilder.CreateGame(state: state),
                TestDataBuilder.CreateGame(state: state)
            };

            _gameRepositoryMock
                .Setup(x => x.GetGamesByStateAsync(state))
                .ReturnsAsync(games);

            // Act
            var result = await _sut.GetGamesByStateAsync(state);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().OnlyContain(g => g.State == state);
        }

        #endregion
    }
}
