using Microsoft.AspNetCore.Mvc;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Enums;

namespace TennisStats.API.Controllers;

/// <summary>
/// Import historical data from external API (bulk import for database population)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Import")]
[Produces("application/json")]
public class ImportController : ControllerBase
{
    private readonly IDataImportService _importService;
    private readonly ILogger<ImportController> _logger;

    public ImportController(IDataImportService importService, ILogger<ImportController> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    /// <summary>
    /// Import all players from the external API
    /// </summary>
    /// <param name="association">WTA or ATP (default: WTA)</param>
    /// <param name="maxPages">Maximum number of pages to fetch (default: 100)</param>
    /// <param name="delayMs">Delay between API requests in milliseconds (default: 1000)</param>
    [HttpPost("players")]
    [ProducesResponseType(typeof(DataImportResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<DataImportResult>> ImportPlayers(
        [FromQuery] Association association = Association.WTA,
        [FromQuery] int maxPages = 100,
        [FromQuery] int delayMs = 1000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting player import for {Association}", association);

        var result = await _importService.ImportPlayersAsync(association, maxPages, delayMs, cancellationToken);
        
        _logger.LogInformation("Player import completed: {Added} added, {Updated} updated, {Failed} failed",
            result.Added, result.Updated, result.Failed);

        return Ok(result);
    }

    /// <summary>
    /// Import tournaments for a range of years
    /// </summary>
    /// <param name="association">WTA or ATP (default: WTA)</param>
    /// <param name="startYear">Start year (default: 2020)</param>
    /// <param name="endYear">End year (default: current year)</param>
    /// <param name="delayMs">Delay between API requests in milliseconds (default: 1000)</param>
    [HttpPost("tournaments")]
    [ProducesResponseType(typeof(DataImportResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<DataImportResult>> ImportTournaments(
        [FromQuery] Association association = Association.WTA,
        [FromQuery] int? startYear = null,
        [FromQuery] int? endYear = null,
        [FromQuery] int delayMs = 1000,
        CancellationToken cancellationToken = default)
    {
        var fromYear = startYear ?? 2020;
        var toYear = endYear ?? DateTime.UtcNow.Year;

        _logger.LogInformation("Starting tournament import for {Association} from {StartYear} to {EndYear}",
            association, fromYear, toYear);

        var result = await _importService.ImportTournamentsAsync(association, fromYear, toYear, delayMs, cancellationToken);

        _logger.LogInformation("Tournament import completed: {Added} added, {Updated} updated, {Failed} failed",
            result.Added, result.Updated, result.Failed);

        return Ok(result);
    }

    /// <summary>
    /// Import current rankings
    /// </summary>
    /// <param name="association">WTA or ATP (default: WTA)</param>
    [HttpPost("rankings")]
    [ProducesResponseType(typeof(DataImportResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<DataImportResult>> ImportRankings(
        [FromQuery] Association association = Association.WTA,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting rankings import for {Association}", association);

        var result = await _importService.ImportRankingsAsync(association, cancellationToken);

        _logger.LogInformation("Rankings import completed: {Added} added, {Updated} updated, {Failed} failed",
            result.Added, result.Updated, result.Failed);

        return Ok(result);
    }

    /// <summary>
    /// Import all historical data (players, tournaments, rankings) for years 2020 to current
    /// This is a long-running operation that may take several minutes
    /// </summary>
    /// <param name="association">WTA or ATP (default: WTA)</param>
    /// <param name="startYear">Start year (default: 2020)</param>
    /// <param name="endYear">End year (default: current year)</param>
    /// <param name="delayMs">Delay between API requests in milliseconds (default: 1000)</param>
    [HttpPost("full")]
    [ProducesResponseType(typeof(FullImportResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<FullImportResult>> ImportFullHistoricalData(
        [FromQuery] Association association = Association.WTA,
        [FromQuery] int? startYear = null,
        [FromQuery] int? endYear = null,
        [FromQuery] int delayMs = 1000,
        CancellationToken cancellationToken = default)
    {
        var fromYear = startYear ?? 2020;
        var toYear = endYear ?? DateTime.UtcNow.Year;

        _logger.LogInformation("Starting FULL historical data import for {Association} from {StartYear} to {EndYear}",
            association, fromYear, toYear);

        var progress = new Progress<ImportProgress>(p =>
        {
            _logger.LogInformation("Import progress: {Message} ({Current}/{Total}) - {Percent:F1}%",
                p.CurrentOperation, p.Current, p.Total, p.PercentComplete);
        });

        var result = await _importService.ImportAllHistoricalDataAsync(association, fromYear, toYear, delayMs, progress, cancellationToken);

        _logger.LogInformation("Full import completed in {Duration}. Players: {PlayersAdded}, Tournaments: {TournamentsAdded}, Rankings: {RankingsAdded}",
            result.TotalDuration, result.PlayersResult.Added, result.TournamentsResult.Added, result.RankingsResult.Added);

        return Ok(result);
    }

    /// <summary>
    /// Get current import status
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(ImportStatus), StatusCodes.Status200OK)]
    public ActionResult<ImportStatus> GetImportStatus()
    {
        var status = _importService.GetImportStatus();
        return Ok(status);
    }

    /// <summary>
    /// Health check endpoint to verify import service is available
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> HealthCheck()
    {
        return Ok(new 
        { 
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "DataImportService"
        });
    }
}
