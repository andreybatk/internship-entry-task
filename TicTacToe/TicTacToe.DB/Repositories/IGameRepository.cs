using TicTacToe.Contracts.Enums;
using TicTacToe.DB.Entities;

namespace TicTacToe.DB.Repositories
{
    public interface IGameRepository
    {
        Task<Game?> GetGameAsync(Guid id, bool includeMoves = false);
        Task AddGameAsync(Game game);
        Task AddMoveAsync(Move move);
        Task FinishGameAsync(Game game, DateTime? finishedAt, CellSymbol? winner);
    }
}