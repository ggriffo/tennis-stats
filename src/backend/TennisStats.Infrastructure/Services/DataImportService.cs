using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.Services;

/// <summary>
/// Service for importing historical data from external API into local database
/// </summary>
public class DataImportService : IDataImportService
{
    private readonly IExternalTennisApiService _externalApi;
    private readonly IPlayerRepository _playerRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRankingRepository _rankingRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DataImportService> _logger;

    private ImportStatus _currentStatus = new(false, null, null, 0);
    private readonly object _statusLock = new();

    public DataImportService(
        IExternalTennisApiService externalApi,
        IPlayerRepository playerRepository,
        ITournamentRepository tournamentRepository,
        IRankingRepository rankingRepository,
        ISeasonRepository seasonRepository,
        IUnitOfWork unitOfWork,
        ILogger<DataImportService> logger)
    {
        _externalApi = externalApi;
        _playerRepository = playerRepository;
        _tournamentRepository = tournamentRepository;
        _rankingRepository = rankingRepository;
        _seasonRepository = seasonRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public ImportStatus GetImportStatus()
    {
        lock (_statusLock)
        {
            return _currentStatus;
        }
    }

    private void UpdateStatus(bool isRunning, string? operation, double percentComplete)
    {
        lock (_statusLock)
        {
            _currentStatus = new ImportStatus(
                isRunning,
                operation,
                isRunning && _currentStatus.StartedAt == null ? DateTime.UtcNow : _currentStatus.StartedAt,
                percentComplete
            );
        }
    }

    public async Task<DataImportResult> ImportPlayersAsync(
        Association association,
        int maxPages = 100,
        int delayBetweenRequestsMs = 1000,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        int added = 0, updated = 0, skipped = 0, failed = 0;

        try
        {
            UpdateStatus(true, $"Importing {association} players", 0);
            _logger.LogInformation("Starting player import for {Association}", association);

            int page = 1;
            int perPage = 25;
            bool hasMoreData = true;

            while (hasMoreData && page <= maxPages)
            {
                cancellationToken.ThrowIfCancellationRequested();

                UpdateStatus(true, $"Importing {association} players (page {page})", (double)page / maxPages * 100);

                var players = await _externalApi.GetPlayersAsync(association, page, perPage, cancellationToken);
                var playerList = players.ToList();

                if (playerList.Count == 0)
                {
                    hasMoreData = false;
                    continue;
                }

                foreach (var externalPlayer in playerList)
                {
                    try
                    {
                        var existingPlayer = await _playerRepository.GetByExternalIdAsync(externalPlayer.Id, cancellationToken);

                        if (existingPlayer == null)
                        {
                            var newPlayer = MapToPlayer(externalPlayer, association);
                            await _playerRepository.AddAsync(newPlayer, cancellationToken);
                            added++;
                        }
                        else
                        {
                            UpdatePlayer(existingPlayer, externalPlayer);
                            await _playerRepository.UpdateAsync(existingPlayer, cancellationToken);
                            updated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to import player {PlayerId}", externalPlayer.Id);
                        failed++;
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("Imported page {Page} of {Association} players: {Added} added, {Updated} updated",
                    page, association, added, updated);

                page++;

                // Respect rate limits
                if (hasMoreData && delayBetweenRequestsMs > 0)
                {
                    await Task.Delay(delayBetweenRequestsMs, cancellationToken);
                }
            }

            stopwatch.Stop();
            _logger.LogInformation(
                "Completed player import for {Association}: {Added} added, {Updated} updated, {Failed} failed in {Duration}",
                association, added, updated, failed, stopwatch.Elapsed);

            return new DataImportResult(true, added, updated, skipped, failed, "Players", stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Player import cancelled for {Association}", association);
            return new DataImportResult(false, added, updated, skipped, failed, "Players", stopwatch.Elapsed, "Operation cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during player import for {Association}", association);
            return new DataImportResult(false, added, updated, skipped, failed, "Players", stopwatch.Elapsed, ex.Message);
        }
        finally
        {
            UpdateStatus(false, null, 100);
        }
    }

    public async Task<DataImportResult> ImportTournamentsAsync(
        Association association,
        int startYear,
        int endYear,
        int delayBetweenRequestsMs = 1000,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        int added = 0, updated = 0, skipped = 0, failed = 0;

        try
        {
            UpdateStatus(true, $"Importing {association} tournaments", 0);
            _logger.LogInformation("Starting tournament import for {Association} from {StartYear} to {EndYear}",
                association, startYear, endYear);

            int totalYears = endYear - startYear + 1;
            int currentYear = 0;

            for (int year = startYear; year <= endYear; year++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                currentYear++;
                UpdateStatus(true, $"Importing {association} tournaments for {year}", (double)currentYear / totalYears * 100);

                // Ensure season exists
                var season = await EnsureSeasonExistsAsync(association, year, cancellationToken);

                var tournaments = await _externalApi.GetTournamentsAsync(association, year, cancellationToken);

                foreach (var externalTournament in tournaments)
                {
                    try
                    {
                        var existingTournament = await _tournamentRepository.GetByExternalIdAsync(externalTournament.Id, cancellationToken);

                        if (existingTournament == null)
                        {
                            var newTournament = MapToTournament(externalTournament, association, season.Id);
                            await _tournamentRepository.AddAsync(newTournament, cancellationToken);
                            added++;
                        }
                        else
                        {
                            UpdateTournament(existingTournament, externalTournament, season.Id);
                            await _tournamentRepository.UpdateAsync(existingTournament, cancellationToken);
                            updated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to import tournament {TournamentId}", externalTournament.Id);
                        failed++;
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("Imported {Year} {Association} tournaments: {Added} added, {Updated} updated",
                    year, association, added, updated);

                // Respect rate limits
                if (year < endYear && delayBetweenRequestsMs > 0)
                {
                    await Task.Delay(delayBetweenRequestsMs, cancellationToken);
                }
            }

            stopwatch.Stop();
            _logger.LogInformation(
                "Completed tournament import for {Association}: {Added} added, {Updated} updated, {Failed} failed in {Duration}",
                association, added, updated, failed, stopwatch.Elapsed);

            return new DataImportResult(true, added, updated, skipped, failed, "Tournaments", stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Tournament import cancelled for {Association}", association);
            return new DataImportResult(false, added, updated, skipped, failed, "Tournaments", stopwatch.Elapsed, "Operation cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during tournament import for {Association}", association);
            return new DataImportResult(false, added, updated, skipped, failed, "Tournaments", stopwatch.Elapsed, ex.Message);
        }
        finally
        {
            UpdateStatus(false, null, 100);
        }
    }

    public async Task<DataImportResult> ImportRankingsAsync(
        Association association,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        int added = 0, updated = 0, skipped = 0, failed = 0;

        try
        {
            UpdateStatus(true, $"Importing {association} rankings", 0);
            _logger.LogInformation("Starting rankings import for {Association}", association);

            // Get current season
            var currentYear = DateTime.UtcNow.Year;
            var season = await EnsureSeasonExistsAsync(association, currentYear, cancellationToken);

            var rankings = await _externalApi.GetRankingsAsync(association, cancellationToken);
            var rankingList = rankings.ToList();
            int total = rankingList.Count;
            int processed = 0;

            foreach (var externalRanking in rankingList)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    processed++;
                    UpdateStatus(true, $"Importing {association} rankings", (double)processed / total * 100);

                    // Find the player
                    var player = await _playerRepository.GetByExternalIdAsync(externalRanking.PlayerId, cancellationToken);
                    
                    if (player == null)
                    {
                        _logger.LogDebug("Player {PlayerId} not found for ranking, skipping", externalRanking.PlayerId);
                        skipped++;
                        continue;
                    }

                    // Check if we already have this ranking
                    var existingRanking = await _rankingRepository.GetPlayerRankingForDateAsync(
                        player.Id, externalRanking.RankingDate, cancellationToken);

                    if (existingRanking == null)
                    {
                        var newRanking = new Ranking
                        {
                            ExternalId = player.ExternalId,
                            PlayerId = player.Id,
                            SeasonId = season.Id,
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

                        await _rankingRepository.AddAsync(newRanking, cancellationToken);
                        added++;
                    }
                    else
                    {
                        // Update existing ranking if values changed
                        if (existingRanking.Rank != externalRanking.Rank ||
                            existingRanking.Points != externalRanking.Points)
                        {
                            existingRanking.Rank = externalRanking.Rank;
                            existingRanking.Points = externalRanking.Points;
                            existingRanking.PreviousRank = externalRanking.PreviousRank;
                            existingRanking.RankChange = externalRanking.PreviousRank.HasValue
                                ? externalRanking.PreviousRank.Value - externalRanking.Rank
                                : null;
                            existingRanking.LastSyncedAt = DateTime.UtcNow;

                            await _rankingRepository.UpdateAsync(existingRanking, cancellationToken);
                            updated++;
                        }
                        else
                        {
                            skipped++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to import ranking for player {PlayerId}", externalRanking.PlayerId);
                    failed++;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation(
                "Completed rankings import for {Association}: {Added} added, {Updated} updated, {Skipped} skipped, {Failed} failed in {Duration}",
                association, added, updated, skipped, failed, stopwatch.Elapsed);

            return new DataImportResult(true, added, updated, skipped, failed, "Rankings", stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Rankings import cancelled for {Association}", association);
            return new DataImportResult(false, added, updated, skipped, failed, "Rankings", stopwatch.Elapsed, "Operation cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during rankings import for {Association}", association);
            return new DataImportResult(false, added, updated, skipped, failed, "Rankings", stopwatch.Elapsed, ex.Message);
        }
        finally
        {
            UpdateStatus(false, null, 100);
        }
    }

    public async Task<FullImportResult> ImportAllHistoricalDataAsync(
        Association association,
        int startYear,
        int endYear,
        int delayBetweenRequestsMs = 1000,
        IProgress<ImportProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var totalStopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "Starting full historical import for {Association} from {StartYear} to {EndYear}",
                association, startYear, endYear);

            // Step 1: Import players first (required for rankings)
            progress?.Report(new ImportProgress("Importing players", "Players", 0, 3, 0));
            var playersResult = await ImportPlayersAsync(association, 100, delayBetweenRequestsMs, cancellationToken);

            if (!playersResult.Success)
            {
                _logger.LogError("Player import failed, aborting full import");
                return new FullImportResult(
                    false,
                    playersResult,
                    new DataImportResult(false, 0, 0, 0, 0, "Tournaments", TimeSpan.Zero, "Skipped due to player import failure"),
                    new DataImportResult(false, 0, 0, 0, 0, "Rankings", TimeSpan.Zero, "Skipped due to player import failure"),
                    totalStopwatch.Elapsed,
                    "Player import failed"
                );
            }

            // Step 2: Import tournaments
            progress?.Report(new ImportProgress("Importing tournaments", "Tournaments", 1, 3, 33));
            var tournamentsResult = await ImportTournamentsAsync(association, startYear, endYear, delayBetweenRequestsMs, cancellationToken);

            // Step 3: Import rankings
            progress?.Report(new ImportProgress("Importing rankings", "Rankings", 2, 3, 66));
            var rankingsResult = await ImportRankingsAsync(association, cancellationToken);

            totalStopwatch.Stop();

            progress?.Report(new ImportProgress("Import complete", "All", 3, 3, 100));

            var success = playersResult.Success && tournamentsResult.Success && rankingsResult.Success;

            _logger.LogInformation(
                "Full historical import completed for {Association}. Success: {Success}. Duration: {Duration}",
                association, success, totalStopwatch.Elapsed);

            return new FullImportResult(
                success,
                playersResult,
                tournamentsResult,
                rankingsResult,
                totalStopwatch.Elapsed
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during full historical import for {Association}", association);
            return new FullImportResult(
                false,
                new DataImportResult(false, 0, 0, 0, 0, "Players", TimeSpan.Zero, ex.Message),
                new DataImportResult(false, 0, 0, 0, 0, "Tournaments", TimeSpan.Zero, ex.Message),
                new DataImportResult(false, 0, 0, 0, 0, "Rankings", TimeSpan.Zero, ex.Message),
                totalStopwatch.Elapsed,
                ex.Message
            );
        }
    }

    #region Helper Methods

    private async Task<Season> EnsureSeasonExistsAsync(Association association, int year, CancellationToken cancellationToken)
    {
        var season = await _seasonRepository.GetByYearAsync(association, year, cancellationToken);
        
        if (season == null)
        {
            season = new Season
            {
                ExternalId = year,
                Year = year,
                Association = association,
                StartDate = new DateTime(year, 1, 1),
                EndDate = new DateTime(year, 12, 31),
                IsCurrent = year == DateTime.UtcNow.Year
            };

            await _seasonRepository.AddAsync(season, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return season;
    }

    private static Player MapToPlayer(ExternalPlayerDto dto, Association association)
    {
        return new Player
        {
            ExternalId = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FullName = dto.FullName,
            Country = dto.Country,
            DateOfBirth = dto.DateOfBirth,
            HeightCm = dto.HeightCm,
            WeightKg = dto.WeightKg,
            Hand = ParseHand(dto.Hand),
            Backhand = ParseBackhand(dto.Backhand),
            TurnedProYear = dto.TurnedProYear,
            ImageUrl = dto.ImageUrl,
            Association = association,
            IsActive = true,
            LastSyncedAt = DateTime.UtcNow
        };
    }

    private static void UpdatePlayer(Player player, ExternalPlayerDto dto)
    {
        player.FirstName = dto.FirstName;
        player.LastName = dto.LastName;
        player.FullName = dto.FullName;
        player.Country = dto.Country;
        player.DateOfBirth = dto.DateOfBirth;
        player.HeightCm = dto.HeightCm;
        player.WeightKg = dto.WeightKg;
        player.Hand = ParseHand(dto.Hand);
        player.Backhand = ParseBackhand(dto.Backhand);
        player.TurnedProYear = dto.TurnedProYear;
        player.ImageUrl = dto.ImageUrl;
        player.LastSyncedAt = DateTime.UtcNow;
    }

    private static Tournament MapToTournament(ExternalTournamentDto dto, Association association, int seasonId)
    {
        return new Tournament
        {
            ExternalId = dto.Id,
            Name = dto.Name,
            City = dto.City,
            Country = dto.Country,
            Surface = ParseSurface(dto.Surface),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            PrizeMoney = dto.PrizeMoney,
            Currency = dto.Currency,
            Category = dto.Category,
            Association = association,
            SeasonId = seasonId,
            IsCompleted = dto.EndDate.HasValue && dto.EndDate.Value < DateTime.UtcNow,
            LastSyncedAt = DateTime.UtcNow
        };
    }

    private static void UpdateTournament(Tournament tournament, ExternalTournamentDto dto, int seasonId)
    {
        tournament.Name = dto.Name;
        tournament.City = dto.City;
        tournament.Country = dto.Country;
        tournament.Surface = ParseSurface(dto.Surface);
        tournament.StartDate = dto.StartDate;
        tournament.EndDate = dto.EndDate;
        tournament.PrizeMoney = dto.PrizeMoney;
        tournament.Currency = dto.Currency;
        tournament.Category = dto.Category;
        tournament.SeasonId = seasonId;
        tournament.IsCompleted = dto.EndDate.HasValue && dto.EndDate.Value < DateTime.UtcNow;
        tournament.LastSyncedAt = DateTime.UtcNow;
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

    private static Surface ParseSurface(string? surface)
    {
        return surface?.ToLower() switch
        {
            "hard" => Surface.Hard,
            "clay" => Surface.Clay,
            "grass" => Surface.Grass,
            "carpet" => Surface.Carpet,
            _ => Surface.Unknown
        };
    }

    #endregion
}
