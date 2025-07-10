using Moq;
using TicTacToe.BL.Services;
using TicTacToe.Contracts.Enums;
using TicTacToe.Contracts.Helpers;
using TicTacToe.DB.Entities;
using TicTacToe.DB.Repositories;

namespace TicTactoe.Tests
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;

        public GameServiceTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(5, 4)]
        [InlineData(10, 5)]
        public async Task CreateNewGameAsync_ShouldReturnSuccessResult(int boardSize, int winLength)
        {
            // arrange
            var settings = new GameSettings { BoardSize = boardSize, WinLength = winLength };
            var gameService = new GameService(_gameRepositoryMock.Object, settings);
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();

            // act
            var result = await gameService.CreateNewGameAsync(playerX, playerO);

            // assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(GameStatus.InProgress, result.Data.Status);

            _gameRepositoryMock.Verify(repo => repo.AddGameAsync(It.IsAny<Game>()), Times.Once);
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(5, 4)]
        [InlineData(10, 5)]
        public async Task MakeMoveAsync_ShouldReturnFail_WhenGameNotFound(int boardSize, int winLength)
        {
            // arrange
            var settings = new GameSettings { BoardSize = boardSize, WinLength = winLength };
            var gameService = new GameService(_gameRepositoryMock.Object, settings);
            _gameRepositoryMock
                .Setup(repo => repo.GetGameAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync((Game?)null);

            // act
            var result = await gameService.MakeMoveAsync(Guid.NewGuid(), Guid.NewGuid(), 0, 0);

            // assert
            Assert.False(result.Success);
            Assert.Equal("Game not found", result.ErrorMessage);
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(5, 4)]
        [InlineData(10, 5)]
        public async Task MakeMoveAsync_ShouldReturnFail_WhenMoveOutOfBounds(int boardSize, int winLength)
        {
            // arrange
            var settings = new GameSettings { BoardSize = boardSize, WinLength = winLength };
            var gameService = new GameService(_gameRepositoryMock.Object, settings);
            var gameId = Guid.NewGuid();
            var playerId = Guid.NewGuid();

            var game = new Game
            {
                Id = gameId,
                BoardSize = boardSize,
                PlayerXId = playerId,
                PlayerOId = Guid.NewGuid(),
                Status = GameStatus.InProgress,
                Moves = new List<Move>()
            };

            _gameRepositoryMock
                .Setup(repo => repo.GetGameAsync(gameId, true))
                .ReturnsAsync(game);

            // act
            var result = await gameService.MakeMoveAsync(gameId, playerId, 10, 10);

            // assert
            Assert.False(result.Success);
            Assert.Equal("Move out of bounds", result.ErrorMessage);
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(5, 4)]
        [InlineData(10, 5)]
        public async Task MakeMoveAsync_ShouldReturnSuccess_WhenValid(int boardSize, int winLength)
        {
            // arrange
            var settings = new GameSettings { BoardSize = boardSize, WinLength = winLength };
            var gameService = new GameService(_gameRepositoryMock.Object, settings);
            var gameId = Guid.NewGuid();
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();

            var game = new Game
            {
                Id = gameId,
                BoardSize = boardSize,
                PlayerXId = playerX,
                PlayerOId = playerO,
                Status = GameStatus.InProgress,
                Moves = new List<Move>()
            };

            _gameRepositoryMock
                .Setup(repo => repo.GetGameAsync(gameId, true))
                .ReturnsAsync(game);

            _gameRepositoryMock
                .Setup(repo => repo.AddMoveAsync(It.IsAny<Move>()))
                .Returns<Move>(move =>
                {
                    game.Moves.Add(move);
                    return Task.CompletedTask;
                });

            _gameRepositoryMock
                .Setup(repo => repo.FinishGameAsync(It.IsAny<Game>(), It.IsAny<DateTime>(), It.IsAny<CellSymbol?>()))
                .Returns(Task.CompletedTask);

            // act
            var result = await gameService.MakeMoveAsync(gameId, playerX, 0, 0);

            // assert
            Assert.True(result.Success);
            Assert.Equal(gameId, result.Data.Id);
            Assert.Single(result.Data.Moves);
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(5, 4)]
        [InlineData(10, 5)]
        public async Task GetGameAsync_ShouldReturnFail_WhenGameNotFound(int boardSize, int winLength)
        {
            var settings = new GameSettings { BoardSize = boardSize, WinLength = winLength };
            var gameService = new GameService(_gameRepositoryMock.Object, settings);
            _gameRepositoryMock
                .Setup(repo => repo.GetGameAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync((Game?)null);

            var result = await gameService.GetGameAsync(Guid.NewGuid());

            Assert.False(result.Success);
            Assert.Equal("Game not found", result.ErrorMessage);
        }

        [Theory]
        [InlineData(3, 3)]
        [InlineData(5, 4)]
        [InlineData(10, 5)]
        public async Task GetGameAsync_ShouldReturnGame_WhenFound(int boardSize, int winLength)
        {
            var settings = new GameSettings { BoardSize = boardSize, WinLength = winLength };
            var gameService = new GameService(_gameRepositoryMock.Object, settings);
            var game = new Game
            {
                Id = Guid.NewGuid(),
                BoardSize = boardSize,
                PlayerXId = Guid.NewGuid(),
                PlayerOId = Guid.NewGuid(),
                Status = GameStatus.InProgress,
                Moves = new List<Move>()
            };

            _gameRepositoryMock
                .Setup(repo => repo.GetGameAsync(game.Id, true))
                .ReturnsAsync(game);

            var result = await gameService.GetGameAsync(game.Id);

            Assert.True(result.Success);
            Assert.Equal(game.Id, result.Data.Id);
        }
    }
}
