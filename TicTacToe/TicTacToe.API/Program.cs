using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TicTacToe.BL.Services;
using TicTacToe.Contracts.Helpers;
using TicTacToe.DB;
using TicTacToe.DB.Repositories;

namespace TicTacToe.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString(
                "DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var gameSettings = new GameSettings();
            builder.Configuration.Bind("GameSettings", gameSettings);
            builder.Services.AddSingleton(gameSettings);

            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IGameService, GameService>();

            if(builder.Environment.EnvironmentName != "Testing")
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            }

            builder.Services.AddSwaggerGen();

            builder.Services.AddHealthChecks();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var env = services.GetRequiredService<IWebHostEnvironment>();
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                if (!env.IsEnvironment("Testing"))
                {
                    dbContext.Database.Migrate();
                }
            }

            app.MapHealthChecks("/health");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }
    }
}
