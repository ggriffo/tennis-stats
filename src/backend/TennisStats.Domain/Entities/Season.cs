using TennisStats.Domain.Common;
using TennisStats.Domain.Enums;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents a tennis season
/// </summary>
public class Season : BaseEntityWithExternalId
{
    public int Year { get; set; }
    public Association Association { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }

    // Navigation properties
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    public virtual ICollection<Ranking> Rankings { get; set; } = new List<Ranking>();
}
