using TicTacToe.Contracts.Enums;

namespace TicTacToe.Contracts.Game
{
    public class GameResponse
    {
        public Guid Id { get; set; }

        public int BoardSize { get; set; }

        public GameStatus Status { get; set; }

        public Guid PlayerXId { get; set; }
        public Guid PlayerOId { get; set; }

        public CellSymbol? Winner { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public List<MoveResponse> Moves { get; set; } = [];
    }
}