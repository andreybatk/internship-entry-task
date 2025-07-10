using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicTacToe.Contracts.Enums;
using TicTacToe.Contracts.Game;

namespace TicTacToe.Tests
{
    public class GameIntegrationTests : IClassFixture<TestApplicationFactory>
    {
        private readonly HttpClient _client;

        public GameIntegrationTests(TestApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateGame_ShouldReturnSuccess()
        {
            // arrange
            var playerX = Guid.NewGuid();
            var playerO = Guid.NewGuid();

            var url = $"/api/game?playerXId={playerX}&playerOId={playerO}";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            // act
            var response = await _client.PostAsync(url, null);

            // assert
            response.EnsureSuccessStatusCode();
            var game = await response.Content.ReadFromJsonAsync<GameResponse>(options);
            Assert.NotNull(game);
            Assert.Equal(playerX, game.PlayerXId);
            Assert.Equal(GameStatus.InProgress, game.Status);
        }
    }
}
