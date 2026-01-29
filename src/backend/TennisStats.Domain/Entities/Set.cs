using TennisStats.Domain.Common;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents a set within a match
/// </summary>
public class Set : BaseEntity
{
    public int SetNumber { get; set; }
    public int Player1Games { get; set; }
    public int Player2Games { get; set; }
    public int? TiebreakPlayer1Points { get; set; }
    public int? TiebreakPlayer2Points { get; set; }
    public bool IsTiebreak { get; set; }
    public int? WinnerPlayerId { get; set; }

    // Foreign keys
    public int MatchId { get; set; }

    // Navigation properties
    public virtual Match Match { get; set; } = null!;
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
