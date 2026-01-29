namespace TennisStats.Domain.Enums;

/// <summary>
/// Match status
/// </summary>
public enum MatchStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Postponed = 5,
    Walkover = 6,
    Retired = 7
}
