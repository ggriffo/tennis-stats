using TennisStats.Domain.Common;
using TennisStats.Domain.Enums;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents a tennis player (WTA or ATP)
/// </summary>
public class Player : BaseEntityWithExternalId
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? HeightCm { get; set; }
    public int? WeightKg { get; set; }
    public Hand Hand { get; set; } = Hand.Unknown;
    public BackhandType Backhand { get; set; } = BackhandType.Unknown;
    public int? TurnedProYear { get; set; }
    public string? ImageUrl { get; set; }
    public Association Association { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Ranking> Rankings { get; set; } = new List<Ranking>();
    public virtual ICollection<Match> MatchesAsPlayer1 { get; set; } = new List<Match>();
    public virtual ICollection<Match> MatchesAsPlayer2 { get; set; } = new List<Match>();
    public virtual ICollection<Match> MatchesWon { get; set; } = new List<Match>();
    public virtual ICollection<PlayerStatistics> Statistics { get; set; } = new List<PlayerStatistics>();
}
