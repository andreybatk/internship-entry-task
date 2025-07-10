using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using TicTacToe.Contracts.Game;

namespace TicTacToe.API.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ETagFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            if (executedContext.Result is ObjectResult objectResult &&
                objectResult.Value is GameResponse game)
            {
                var etag = GenerateEtagFromGame(game);
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var incomingEtag))
                {
                    if (etag == incomingEtag)
                    {
                        executedContext.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
                        return;
                    }
                }

                response.Headers[HeaderNames.ETag] = etag;
            }
        }

        private static string GenerateEtagFromGame(GameResponse game)
        {
            var raw = $"{game.Id}-{game.Moves.Count}-{game.FinishedAt?.Ticks}";
            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
            return $"\"{hash}\"";
        }
    }
}
