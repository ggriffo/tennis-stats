using MediatR;
using Microsoft.AspNetCore.Mvc;
using TennisStats.Application.Common.Models;
using TennisStats.Application.Players.DTOs;
using TennisStats.Application.Players.Queries;
using TennisStats.Domain.Enums;

namespace TennisStats.API.Controllers;

/// <summary>
/// Manage tennis players data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Players")]
[Produces("application/json")]
public class PlayersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlayersController> _logger;

    public PlayersController(IMediator mediator, ILogger<PlayersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of players
    /// </summary>
    /// <param name="association">Association (WTA or ATP)</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="search">Search term for player name</param>
    /// <param name="country">Filter by country</param>
    /// <returns>Paginated list of players</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<PlayerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPlayers(
        [FromQuery] Association association = Association.WTA,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? country = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = new GetPlayersQuery(association, page, pageSize, search, country);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get players: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get player by ID with detailed information
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <returns>Player details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PlayerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayer(int id)
    {
        var query = new GetPlayerByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "NOT_FOUND")
            {
                return NotFound(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get top ranked players
    /// </summary>
    /// <param name="association">Association (WTA or ATP)</param>
    /// <param name="count">Number of players to return</param>
    /// <returns>List of top ranked players</returns>
    [HttpGet("top")]
    [ProducesResponseType(typeof(IEnumerable<PlayerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopPlayers(
        [FromQuery] Association association = Association.WTA,
        [FromQuery] int count = 10)
    {
        if (count < 1 || count > 100) count = 10;

        var query = new GetPlayersQuery(association, 1, count);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data?.Items);
    }
}
