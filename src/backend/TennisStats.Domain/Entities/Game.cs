using TennisStats.Domain.Common;

namespace TennisStats.Domain.Entities;

/// <summary>
/// Represents a game within a set
/// </summary>
public class Game : BaseEntity
{
    public int GameNumber { get; set; }
    public int ServerPlayerId { get; set; }
    public int? WinnerPlayerId { get; set; }
    public bool IsBreak { get; set; }
    public string? Score { get; set; } // e.g., "40-30", "Deuce", "Advantage"

    // Foreign keys
    public int SetId { get; set; }

    // Navigation properties
    public virtual Set Set { get; set; } = null!;
}
