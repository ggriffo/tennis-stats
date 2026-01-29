using TennisStats.Domain.Common;
using TennisStats.Domain.Enums;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents a tennis tournament
/// </summary>
public class Tournament : BaseEntityWithExternalId
{
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public Surface Surface { get; set; } = Surface.Unknown;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PrizeMoney { get; set; }
    public string? Currency { get; set; }
    public Association Association { get; set; }
    public string? Category { get; set; } // e.g., "Grand Slam", "WTA 1000", "WTA 500", etc.
    public bool IsCompleted { get; set; }

    // Foreign keys
    public int SeasonId { get; set; }

    // Navigation properties
    public virtual Season Season { get; set; } = null!;
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
