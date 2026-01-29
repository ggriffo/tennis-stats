using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Interfaces;

/// <summary>
/// Interface for external Tennis API service (balldontlie.io)
/// </summary>
public interface IExternalTennisApiService
{
    // Players
    Task<ExternalPlayerDto?> GetPlayerAsync(int externalId, Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalPlayerDto>> GetPlayersAsync(Association association, int page, int perPage, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalPlayerDto>> SearchPlayersAsync(string search, Association association, CancellationToken cancellationToken = default);

    // Tournaments
    Task<ExternalTournamentDto?> GetTournamentAsync(int externalId, Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalTournamentDto>> GetTournamentsAsync(Association association, int seasonYear, CancellationToken cancellationToken = default);

    // Matches
    Task<ExternalMatchDto?> GetMatchAsync(int externalId, Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalMatchDto>> GetMatchesAsync(Association association, int? tournamentId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);

    // Rankings
    Task<IEnumerable<ExternalRankingDto>> GetRankingsAsync(Association association, CancellationToken cancellationToken = default);

    // Seasons
    Task<IEnumerable<ExternalSeasonDto>> GetSeasonsAsync(Association association, CancellationToken cancellationToken = default);
}

// External API DTOs
public record ExternalPlayerDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string? Country,
    DateTime? DateOfBirth,
    int? HeightCm,
    int? WeightKg,
    string? Hand,
    string? Backhand,
    int? TurnedProYear,
    string? ImageUrl
);

public record ExternalTournamentDto(
    int Id,
    string Name,
    string? City,
    string? Country,
    string? Surface,
    DateTime? StartDate,
    DateTime? EndDate,
    int? PrizeMoney,
    string? Currency,
    string? Category,
    int SeasonYear
);

public record ExternalMatchDto(
    int Id,
    int TournamentId,
    int Player1Id,
    int Player2Id,
    int? WinnerId,
    string? Round,
    string? Status,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? EndedAt,
    int? DurationMinutes,
    IEnumerable<ExternalSetDto>? Sets,
    ExternalMatchStatisticsDto? Statistics
);

public record ExternalSetDto(
    int SetNumber,
    int Player1Games,
    int Player2Games,
    int? TiebreakPlayer1Points,
    int? TiebreakPlayer2Points
);

public record ExternalMatchStatisticsDto(
    int? Player1Aces,
    int? Player1DoubleFaults,
    int? Player1FirstServePercentage,
    int? Player2Aces,
    int? Player2DoubleFaults,
    int? Player2FirstServePercentage
);

public record ExternalRankingDto(
    int PlayerId,
    int Rank,
    int Points,
    int? PreviousRank,
    DateTime RankingDate
);

public record ExternalSeasonDto(
    int Id,
    int Year,
    DateTime? StartDate,
    DateTime? EndDate,
    bool IsCurrent
);
