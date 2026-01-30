using TennisStats.Domain.Enums;

namespace TennisStats.Application.Players.DTOs;

/// <summary>
/// Player data transfer object
/// </summary>
public class PlayerDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Age => DateOfBirth.HasValue ? (int)((DateTime.Today - DateOfBirth.Value).TotalDays / 365.25) : null;
    public int? HeightCm { get; set; }
    public int? WeightKg { get; set; }
    public string Hand { get; set; } = "Unknown";
    public string Backhand { get; set; } = "Unknown";
    public int? TurnedProYear { get; set; }
    public string? ImageUrl { get; set; }
    public string Association { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? CurrentRank { get; set; }
    public int? CurrentPoints { get; set; }
}

/// <summary>
/// Player detail DTO with statistics
/// </summary>
public class PlayerDetailDto : PlayerDto
{
    public ICollection<RankingHistoryDto> RankingHistory { get; set; } = new List<RankingHistoryDto>();
    public PlayerSeasonStatsDto? CurrentSeasonStats { get; set; }
}

/// <summary>
/// Ranking history entry
/// </summary>
public class RankingHistoryDto
{
    public int Rank { get; set; }
    public int Points { get; set; }
    public int? RankChange { get; set; }
    public DateTime RankingDate { get; set; }
}

/// <summary>
/// Player season statistics
/// </summary>
public class PlayerSeasonStatsDto
{
    public int Year { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public decimal WinPercentage => MatchesPlayed > 0 ? Math.Round((decimal)MatchesWon / MatchesPlayed * 100, 1) : 0;
    public int TitlesWon { get; set; }
    
    // Surface-specific statistics
    public int HardMatchesWon { get; set; }
    public int HardMatchesLost { get; set; }
    public int ClayMatchesWon { get; set; }
    public int ClayMatchesLost { get; set; }
    public int GrassMatchesWon { get; set; }
    public int GrassMatchesLost { get; set; }
    
    // Surface win percentages
    public decimal HardWinPercentage => (HardMatchesWon + HardMatchesLost) > 0 ? Math.Round((decimal)HardMatchesWon / (HardMatchesWon + HardMatchesLost) * 100, 1) : 0;
    public decimal ClayWinPercentage => (ClayMatchesWon + ClayMatchesLost) > 0 ? Math.Round((decimal)ClayMatchesWon / (ClayMatchesWon + ClayMatchesLost) * 100, 1) : 0;
    public decimal GrassWinPercentage => (GrassMatchesWon + GrassMatchesLost) > 0 ? Math.Round((decimal)GrassMatchesWon / (GrassMatchesWon + GrassMatchesLost) * 100, 1) : 0;
}
