using TennisStats.Domain.Common;
using TennisStats.Domain.Enums;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents a player ranking at a specific point in time
/// </summary>
public class Ranking : BaseEntityWithExternalId
{
    public int Rank { get; set; }
    public int Points { get; set; }
    public int? PreviousRank { get; set; }
    public int? RankChange { get; set; }
    public DateTime RankingDate { get; set; }
    public Association Association { get; set; }

    // Foreign keys
    public int PlayerId { get; set; }
    public int SeasonId { get; set; }

    // Navigation properties
    public virtual Player Player { get; set; } = null!;
    public virtual Season Season { get; set; } = null!;
}
