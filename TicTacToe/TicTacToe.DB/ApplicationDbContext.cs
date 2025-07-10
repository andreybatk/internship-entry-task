using Microsoft.EntityFrameworkCore;
using TicTacToe.DB.Entities;

namespace TicTacToe.DB
{
    public class ApplicationDbContext : DbContext
    {
        // dotnet ef migrations add InitialCreate -p TicTacToe.DB\TicTacToe.DB.csproj -s TicTacToe.API\TicTacToe.API.csproj

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Moves)
                .WithOne(m => m.Game)
                .HasForeignKey(m => m.GameId);
        }
    }
}
