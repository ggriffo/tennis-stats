using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Interfaces;

/// <summary>
/// Service interface for importing historical data from external API
/// </summary>
public interface IDataImportService
{
    /// <summary>
    /// Import all players from the external API
    /// </summary>
    Task<DataImportResult> ImportPlayersAsync(
        Association association,
        int maxPages = 100,
        int delayBetweenRequestsMs = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Import tournaments for a specific year range
    /// </summary>
    Task<DataImportResult> ImportTournamentsAsync(
        Association association,
        int startYear,
        int endYear,
        int delayBetweenRequestsMs = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Import current rankings and historical ranking snapshots
    /// </summary>
    Task<DataImportResult> ImportRankingsAsync(
        Association association,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Perform a full historical data import
    /// </summary>
    Task<FullImportResult> ImportAllHistoricalDataAsync(
        Association association,
        int startYear,
        int endYear,
        int delayBetweenRequestsMs = 1000,
        IProgress<ImportProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current import status
    /// </summary>
    ImportStatus GetImportStatus();
}

/// <summary>
/// Result of a data import operation
/// </summary>
public record DataImportResult(
    bool Success,
    int Added,
    int Updated,
    int Skipped,
    int Failed,
    string EntityType,
    TimeSpan Duration,
    string? ErrorMessage = null
);

/// <summary>
/// Result of a full import operation
/// </summary>
public record FullImportResult(
    bool Success,
    DataImportResult PlayersResult,
    DataImportResult TournamentsResult,
    DataImportResult RankingsResult,
    TimeSpan TotalDuration,
    string? ErrorMessage = null
);

/// <summary>
/// Progress information during import
/// </summary>
public record ImportProgress(
    string CurrentOperation,
    string EntityType,
    int Current,
    int Total,
    double PercentComplete
);

/// <summary>
/// Current import status
/// </summary>
public record ImportStatus(
    bool IsRunning,
    string? CurrentOperation,
    DateTime? StartedAt,
    double PercentComplete
);
