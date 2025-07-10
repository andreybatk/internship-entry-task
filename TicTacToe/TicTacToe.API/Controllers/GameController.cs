using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.Filters;
using TicTacToe.BL.Services;
using TicTacToe.Contracts.Game;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Создать новую игру
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateGame([FromQuery] Guid playerXId, [FromQuery] Guid playerOId)
    {
        if (playerXId == Guid.Empty || playerOId == Guid.Empty)
            return BadRequest("PlayerXId or PlayerOId is empty.");

        var result = await _gameService.CreateNewGameAsync(playerXId, playerOId);

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Сделать ход
    /// </summary>
    [HttpPost("move")]
    [ETagFilter]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MakeMove(Guid gameId, [FromQuery] Guid playerId, [FromQuery] int x, [FromQuery] int y)
    {
        var result = await _gameService.MakeMoveAsync(gameId, playerId, x, y);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Получить игру по Guid
    /// </summary>
    [HttpGet("{gameId}")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGame(Guid gameId)
    {
        if (gameId == Guid.Empty)
            return BadRequest("GameId is empty.");

        var result = await _gameService.GetGameAsync(gameId);

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}