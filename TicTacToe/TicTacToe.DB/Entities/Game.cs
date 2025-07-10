using TicTacToe.Contracts.Enums;

namespace TicTacToe.DB.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        public int BoardSize { get; set; }

        public GameStatus Status { get; set; }

        public Guid PlayerXId { get; set; }
        public Guid PlayerOId { get; set; }

        public CellSymbol? Winner { get; set; } 

        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public List<Move> Moves { get; set; } = [];
    }
}