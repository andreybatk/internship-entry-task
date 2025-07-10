using TicTacToe.Contracts.Game;
using TicTacToe.Contracts.Helpers;

namespace TicTacToe.BL.Services
{
    public interface IGameService
    {
        Task<ServiceResult<GameResponse>> CreateNewGameAsync(Guid playerXId, Guid playerOId);
        Task<ServiceResult<GameResponse>> MakeMoveAsync(Guid gameId, Guid playerId, int x, int y);
        Task<ServiceResult<GameResponse>> GetGameAsync(Guid gameId);
    }
}