using TicTacToe.BL.Mappers;
using TicTacToe.Contracts.Enums;
using TicTacToe.Contracts.Game;
using TicTacToe.Contracts.Helpers;
using TicTacToe.DB.Entities;
using TicTacToe.DB.Repositories;

namespace TicTacToe.BL.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly GameSettings _settings;
        private readonly Random _random = new();

        public GameService(IGameRepository repository, GameSettings settings)
        {
            _gameRepository = repository;
            _settings = settings;
        }

        public async Task<ServiceResult<GameResponse>> CreateNewGameAsync(Guid playerXId, Guid playerOId)
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                BoardSize = _settings.BoardSize,
                PlayerXId = playerXId,
                PlayerOId = playerOId,
                Status = GameStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            };

            await _gameRepository.AddGameAsync(game);
            return ServiceResult<GameResponse>.Ok(game.ToResponse());
        }

        public async Task<ServiceResult<GameResponse>> MakeMoveAsync(Guid gameId, Guid playerId, int x, int y)
        {
            var game = await _gameRepository.GetGameAsync(gameId, includeMoves: true);

            if (game == null)
                return ServiceResult<GameResponse>.Fail("Game not found");

            if (game.Status != GameStatus.InProgress)
                return ServiceResult<GameResponse>.Fail("Game already finished");

            if (x < 0 || y < 0 || x >= game.BoardSize || y >= game.BoardSize)
                return ServiceResult<GameResponse>.Fail("Move out of bounds");

            var existingMove = game.Moves.FirstOrDefault(m => m.X == x && m.Y == y);

            if (existingMove != null)
            {
                if (existingMove.PlayerId != playerId)
                    return ServiceResult<GameResponse>.Fail("Cell already taken by another player");

                return ServiceResult<GameResponse>.Ok(game.ToResponse());
            }

            CellSymbol playerSymbol = game.PlayerXId == playerId ? CellSymbol.X : CellSymbol.O;

            int nextMoveNumber = game.Moves.Count + 1;

            bool wasInverted = false;
            CellSymbol symbolToPlace = playerSymbol;

            if (nextMoveNumber % 3 == 0 && _random.NextDouble() < 0.1)
            {
                symbolToPlace = playerSymbol == CellSymbol.X ? CellSymbol.O : CellSymbol.X;
                wasInverted = true;
            }

            var move = new Move
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                PlayerId = playerId,
                MoveNumber = nextMoveNumber,
                X = x,
                Y = y,
                SymbolPlaced = symbolToPlace,
                WasInverted = wasInverted
            };

            await _gameRepository.AddMoveAsync(move);

            game.Status = CalculateGameStatus(game, symbolToPlace, x, y);
            if (game.Status != GameStatus.InProgress)
            {
                CellSymbol? winner = game.Status == GameStatus.Draw ? null : symbolToPlace;
                await _gameRepository.FinishGameAsync(game, DateTime.UtcNow, winner);
            }
            
            return ServiceResult<GameResponse>.Ok(game.ToResponse());
        }

        public async Task<ServiceResult<GameResponse>> GetGameAsync(Guid gameId)
        {
            var game = await _gameRepository.GetGameAsync(gameId, includeMoves: true);

            if(game == null)
                return ServiceResult<GameResponse>.Fail("Game not found");

            return ServiceResult<GameResponse>.Ok(game.ToResponse());
        }

        private GameStatus CalculateGameStatus(Game game, CellSymbol symbol, int newX, int newY)
        {
            var moves = game.Moves.Where(m => m.GameId == game.Id).ToList();
            int winLength = _settings.WinLength;

            int CountInDirection(int startX, int startY, int dx, int dy) // сколько наших символов в одной стороне
            {
                int count = 0;
                int x = startX + dx;
                int y = startY + dy;

                while (x >= 0 && y >= 0 && x < game.BoardSize && y < game.BoardSize &&
                       moves.Any(m => m.X == x && m.Y == y && m.SymbolPlaced == symbol))
                {
                    count++;
                    x += dx;
                    y += dy;
                }

                return count;
            }

            int total;

            total = 1 + CountInDirection(newX, newY, -1, 0) + CountInDirection(newX, newY, 1, 0); // считаем сколько наших символов слева и справа
            if (total >= winLength) return symbol == CellSymbol.X ? GameStatus.XWon : GameStatus.OWon;

            total = 1 + CountInDirection(newX, newY, 0, -1) + CountInDirection(newX, newY, 0, 1); // считаем сколько наших символов сверху и снизу
            if (total >= winLength) return symbol == CellSymbol.X ? GameStatus.XWon : GameStatus.OWon;

            total = 1 + CountInDirection(newX, newY, -1, -1) + CountInDirection(newX, newY, 1, 1); // считаем сколько наших символов по левой диагонали 
            if (total >= winLength) return symbol == CellSymbol.X ? GameStatus.XWon : GameStatus.OWon;

            total = 1 + CountInDirection(newX, newY, -1, 1) + CountInDirection(newX, newY, 1, -1); // считаем сколько наших символов по правой диагонали 
            if (total >= winLength) return symbol == CellSymbol.X ? GameStatus.XWon : GameStatus.OWon;

            if (game.Moves.Count >= game.BoardSize * game.BoardSize)
                return GameStatus.Draw;

            return GameStatus.InProgress;
        }
    }
}
