using TennisStats.Domain.Common;
using TennisStats.Domain.Enums;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents a tennis match
/// </summary>
public class Match : BaseEntityWithExternalId
{
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public Round Round { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;
    public int? DurationMinutes { get; set; }
    public string? CourtName { get; set; }

    // Foreign keys
    public int TournamentId { get; set; }
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }
    public int? WinnerId { get; set; }

    // Navigation properties
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Player Player1 { get; set; } = null!;
    public virtual Player Player2 { get; set; } = null!;
    public virtual Player? Winner { get; set; }
    public virtual ICollection<Set> Sets { get; set; } = new List<Set>();
    public virtual MatchStatistics? Statistics { get; set; }
}
