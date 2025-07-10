using TicTacToe.Contracts.Game;
using TicTacToe.DB.Entities;

namespace TicTacToe.BL.Mappers
{
    public static class GameMapper
    {
        public static GameResponse ToResponse(this Game game)
        {
            return new GameResponse
            {
                Id = game.Id,
                BoardSize = game.BoardSize,
                Status = game.Status,
                PlayerXId = game.PlayerXId,
                PlayerOId = game.PlayerOId,
                Winner = game.Winner,
                CreatedAt = game.CreatedAt,
                FinishedAt = game.FinishedAt,
                Moves = game.Moves?
                    .OrderBy(m => m.MoveNumber)
                    .Select(m => new MoveResponse
                    {
                        Id = m.Id,
                        GameId = m.GameId,
                        MoveNumber = m.MoveNumber,
                        X = m.X,
                        Y = m.Y,
                        PlayerId = m.PlayerId,
                        SymbolPlaced = m.SymbolPlaced,
                        WasInverted = m.WasInverted
                    }).ToList() ?? new()
            };
        }
    }
}