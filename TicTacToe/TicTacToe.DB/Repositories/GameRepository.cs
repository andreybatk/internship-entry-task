using Microsoft.EntityFrameworkCore;
using TicTacToe.Contracts.Enums;
using TicTacToe.DB.Entities;

namespace TicTacToe.DB.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;

        public GameRepository(ApplicationDbContext db)
        {
            _context = db;
        }

        public async Task<Game?> GetGameAsync(Guid id, bool includeMoves = false)
        {
            if (includeMoves)
            {
                return await _context.Games
                    .Include(g => g.Moves)
                    .FirstOrDefaultAsync(g => g.Id == id);
            }
            else
            {
                return await _context.Games.FindAsync(id);
            }
        }

        public async Task AddGameAsync(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
        }

        public async Task AddMoveAsync(Move move)
        {
            await _context.Moves.AddAsync(move);
            await _context.SaveChangesAsync();
        }
        public async Task FinishGameAsync(Game game, DateTime? finishedAt, CellSymbol? winner)
        {
            game.FinishedAt = finishedAt;
            game.Winner = winner;

            await _context.SaveChangesAsync();
        }
    }
}
