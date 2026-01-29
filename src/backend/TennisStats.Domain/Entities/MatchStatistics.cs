using TennisStats.Domain.Common;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents match statistics for both players
/// </summary>
public class MatchStatistics : BaseEntity
{
    // Player 1 Statistics
    public int? Player1Aces { get; set; }
    public int? Player1DoubleFaults { get; set; }
    public int? Player1FirstServePercentage { get; set; }
    public int? Player1FirstServePointsWon { get; set; }
    public int? Player1SecondServePointsWon { get; set; }
    public int? Player1BreakPointsSaved { get; set; }
    public int? Player1BreakPointsFaced { get; set; }
    public int? Player1ServiceGamesPlayed { get; set; }
    public int? Player1ServiceGamesWon { get; set; }
    public int? Player1TotalPointsWon { get; set; }
    public int? Player1Winners { get; set; }
    public int? Player1UnforcedErrors { get; set; }
    public int? Player1NetPointsWon { get; set; }
    public int? Player1NetPointsPlayed { get; set; }

    // Player 2 Statistics
    public int? Player2Aces { get; set; }
    public int? Player2DoubleFaults { get; set; }
    public int? Player2FirstServePercentage { get; set; }
    public int? Player2FirstServePointsWon { get; set; }
    public int? Player2SecondServePointsWon { get; set; }
    public int? Player2BreakPointsSaved { get; set; }
    public int? Player2BreakPointsFaced { get; set; }
    public int? Player2ServiceGamesPlayed { get; set; }
    public int? Player2ServiceGamesWon { get; set; }
    public int? Player2TotalPointsWon { get; set; }
    public int? Player2Winners { get; set; }
    public int? Player2UnforcedErrors { get; set; }
    public int? Player2NetPointsWon { get; set; }
    public int? Player2NetPointsPlayed { get; set; }

    // Foreign keys
    public int MatchId { get; set; }

    // Navigation properties
    public virtual Match Match { get; set; } = null!;
}
