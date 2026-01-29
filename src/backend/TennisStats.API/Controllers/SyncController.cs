using Microsoft.AspNetCore.Mvc;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Enums;

namespace TennisStats.API.Controllers;

/// <summary>
/// Sync data from external tennis API (quick sync for recent updates)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Sync")]
[Produces("application/json")]
public class SyncController : ControllerBase
{
    private readonly IExternalTennisApiService _externalApiService;
    private readonly IPlayerRepository _playerRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRankingRepository _rankingRepository;
    private readonly ILogger<SyncController> _logger;

    public SyncController(
        IExternalTennisApiService externalApiService,
        IPlayerRepository playerRepository,
        ISeasonRepository seasonRepository,
        ITournamentRepository tournamentRepository,
        IRankingRepository rankingRepository,
        ILogger<SyncController> logger)
    {
        _externalApiService = externalApiService;
        _playerRepository = playerRepository;
        _seasonRepository = seasonRepository;
        _tournamentRepository = tournamentRepository;
        _rankingRepository = rankingRepository;
        _logger = logger;
    }

    /// <summary>
    /// Sync players from external API
    /// </summary>
    /// <param name="association">Association (WTA or ATP)</param>
    /// <param name="pages">Number of pages to sync</param>
    /// <returns>Sync result</returns>
    [HttpPost("players")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncPlayers(
        [FromQuery] Association association = Association.WTA,
        [FromQuery] int pages = 5)
    {
        try
        {
            var synced = 0;
            var updated = 0;

            for (int page = 1; page <= pages; page++)
            {
                var externalPlayers = await _externalApiService.GetPlayersAsync(association, page, 25);

                foreach (var externalPlayer in externalPlayers)
                {
                    var existingPlayer = await _playerRepository.GetByExternalIdAsync(externalPlayer.Id);

                    if (existingPlayer == null)
                    {
                        var newPlayer = new Domain.Entities.Player
                        {
                            ExternalId = externalPlayer.Id,
                            FirstName = externalPlayer.FirstName,
                            LastName = externalPlayer.LastName,
                            FullName = externalPlayer.FullName,
                            Country = externalPlayer.Country,
                            DateOfBirth = externalPlayer.DateOfBirth,
                            HeightCm = externalPlayer.HeightCm,
                            WeightKg = externalPlayer.WeightKg,
                            Hand = ParseHand(externalPlayer.Hand),
                            Backhand = ParseBackhand(externalPlayer.Backhand),
                            TurnedProYear = externalPlayer.TurnedProYear,
                            ImageUrl = externalPlayer.ImageUrl,
                            Association = association,
                            LastSyncedAt = DateTime.UtcNow
                        };

                        await _playerRepository.AddAsync(newPlayer);
                        synced++;
                    }
                    else
                    {
                        existingPlayer.FirstName = externalPlayer.FirstName;
                        existingPlayer.LastName = externalPlayer.LastName;
                        existingPlayer.FullName = externalPlayer.FullName;
                        existingPlayer.Country = externalPlayer.Country;
                        existingPlayer.DateOfBirth = externalPlayer.DateOfBirth;
                        existingPlayer.HeightCm = externalPlayer.HeightCm;
                        existingPlayer.WeightKg = externalPlayer.WeightKg;
                        existingPlayer.Hand = ParseHand(externalPlayer.Hand);
                        existingPlayer.Backhand = ParseBackhand(externalPlayer.Backhand);
                        existingPlayer.TurnedProYear = externalPlayer.TurnedProYear;
                        existingPlayer.ImageUrl = externalPlayer.ImageUrl;
                        existingPlayer.LastSyncedAt = DateTime.UtcNow;

                        await _playerRepository.UpdateAsync(existingPlayer);
                        updated++;
                    }
                }
            }

            _logger.LogInformation("Synced {Synced} new players, updated {Updated} existing players for {Association}",
                synced, updated, association);

            return Ok(new { synced, updated, association = association.ToString() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing players for {Association}", association);
            return StatusCode(500, new { error = "Failed to sync players", details = ex.Message });
        }
    }

    /// <summary>
    /// Sync rankings from external API
    /// </summary>
    /// <param name="association">Association (WTA or ATP)</param>
    /// <returns>Sync result</returns>
    [HttpPost("rankings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncRankings([FromQuery] Association association = Association.WTA)
    {
        try
        {
            var externalRankings = await _externalApiService.GetRankingsAsync(association);
            var synced = 0;

            // Get or create current season
            var currentSeason = await _seasonRepository.GetCurrentSeasonAsync(association);
            if (currentSeason == null)
            {
                currentSeason = new Domain.Entities.Season
                {
                    Year = DateTime.Now.Year,
                    Association = association,
                    IsCurrent = true,
                    ExternalId = DateTime.Now.Year
                };
                await _seasonRepository.AddAsync(currentSeason);
            }

            foreach (var externalRanking in externalRankings)
            {
                var player = await _playerRepository.GetByExternalIdAsync(externalRanking.PlayerId);
                if (player == null) continue;

                var existingRanking = await _rankingRepository.GetPlayerCurrentRankingAsync(player.Id);
                
                // Only add new ranking if it's different or doesn't exist
                if (existingRanking == null || existingRanking.Rank != externalRanking.Rank || 
                    existingRanking.RankingDate.Date != externalRanking.RankingDate.Date)
                {
                    var newRanking = new Domain.Entities.Ranking
                    {
                        ExternalId = player.ExternalId,
                        PlayerId = player.Id,
                        SeasonId = currentSeason.Id,
                        Rank = externalRanking.Rank,
                        Points = externalRanking.Points,
                        PreviousRank = externalRanking.PreviousRank,
                        RankChange = externalRanking.PreviousRank.HasValue 
                            ? externalRanking.PreviousRank.Value - externalRanking.Rank 
                            : null,
                        RankingDate = externalRanking.RankingDate,
                        Association = association,
                        LastSyncedAt = DateTime.UtcNow
                    };

                    await _rankingRepository.AddAsync(newRanking);
                    synced++;
                }
            }

            _logger.LogInformation("Synced {Synced} rankings for {Association}", synced, association);
            return Ok(new { synced, association = association.ToString() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing rankings for {Association}", association);
            return StatusCode(500, new { error = "Failed to sync rankings", details = ex.Message });
        }
    }

    private static Hand ParseHand(string? hand)
    {
        return hand?.ToLower() switch
        {
            "right" => Hand.Right,
            "left" => Hand.Left,
            _ => Hand.Unknown
        };
    }

    private static BackhandType ParseBackhand(string? backhand)
    {
        return backhand?.ToLower() switch
        {
            "one-handed" or "one handed" or "1" => BackhandType.OneHanded,
            "two-handed" or "two handed" or "2" => BackhandType.TwoHanded,
            _ => BackhandType.Unknown
        };
    }
}
