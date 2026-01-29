using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Application.Common.Mappings;
using TennisStats.Domain.Enums;

namespace TennisStats.API.Controllers;

/// <summary>
/// Manage player rankings data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Rankings")]
[Produces("application/json")]
public class RankingsController : ControllerBase
{
    private readonly IRankingRepository _rankingRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RankingsController> _logger;

    public RankingsController(
        IRankingRepository rankingRepository,
        IMapper mapper,
        ILogger<RankingsController> logger)
    {
        _rankingRepository = rankingRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get current rankings
    /// </summary>
    /// <param name="association">Association (WTA or ATP)</param>
    /// <param name="count">Number of rankings to return</param>
    /// <returns>List of current rankings</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RankingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentRankings(
        [FromQuery] Association association = Association.WTA,
        [FromQuery] int count = 100)
    {
        try
        {
            if (count < 1 || count > 500) count = 100;

            var rankings = await _rankingRepository.GetTopRankingsAsync(association, count);
            var rankingDtos = rankings.Select(r => new RankingDto
            {
                Id = r.Id,
                PlayerId = r.PlayerId,
                PlayerName = r.Player?.FullName,
                Country = r.Player?.Country,
                Rank = r.Rank,
                Points = r.Points,
                PreviousRank = r.PreviousRank,
                RankChange = r.RankChange,
                RankingDate = r.RankingDate,
                Association = r.Association.ToString()
            });

            return Ok(rankingDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rankings for {Association}", association);
            return StatusCode(500, new { error = "Failed to get rankings" });
        }
    }

    /// <summary>
    /// Get ranking history for a player
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <returns>Player's ranking history</returns>
    [HttpGet("player/{playerId}")]
    [ProducesResponseType(typeof(IEnumerable<RankingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerRankingHistory(int playerId)
    {
        try
        {
            var rankings = await _rankingRepository.GetPlayerRankingHistoryAsync(playerId);
            var rankingDtos = _mapper.Map<List<RankingDto>>(rankings);

            return Ok(rankingDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ranking history for player {PlayerId}", playerId);
            return StatusCode(500, new { error = "Failed to get ranking history" });
        }
    }
}
