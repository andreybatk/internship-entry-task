using TicTacToe.Contracts.Enums;

namespace TicTacToe.Contracts.Game
{
    public class MoveResponse
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }

        public int MoveNumber { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Guid PlayerId { get; set; }

        public CellSymbol SymbolPlaced { get; set; }

        public bool WasInverted { get; set; }
    }
}
