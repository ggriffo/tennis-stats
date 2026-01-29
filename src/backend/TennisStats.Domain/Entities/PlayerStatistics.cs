using TennisStats.Domain.Common;
using TennisStats.Domain.Enums;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents aggregated player statistics for a season
/// </summary>
public class PlayerStatistics : BaseEntity
{
    public int Year { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public int TitlesWon { get; set; }
    public int? TotalAces { get; set; }
    public int? TotalDoubleFaults { get; set; }
    public decimal? FirstServePercentage { get; set; }
    public decimal? FirstServePointsWonPercentage { get; set; }
    public decimal? SecondServePointsWonPercentage { get; set; }
    public decimal? BreakPointsSavedPercentage { get; set; }
    public decimal? ServiceGamesWonPercentage { get; set; }
    public decimal? ReturnGamesWonPercentage { get; set; }
    public int? TotalPrizeMoney { get; set; }
    public string? Currency { get; set; }

    // Surface-specific win/loss
    public int HardMatchesWon { get; set; }
    public int HardMatchesLost { get; set; }
    public int ClayMatchesWon { get; set; }
    public int ClayMatchesLost { get; set; }
    public int GrassMatchesWon { get; set; }
    public int GrassMatchesLost { get; set; }

    // Foreign keys
    public int PlayerId { get; set; }

    // Navigation properties
    public virtual Player Player { get; set; } = null!;
}
