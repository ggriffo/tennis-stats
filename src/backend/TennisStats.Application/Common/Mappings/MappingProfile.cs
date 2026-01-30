using AutoMapper;
using TennisStats.Application.Players.DTOs;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Player mappings
        CreateMap<Player, PlayerDto>()
            .ForMember(dest => dest.Hand, opt => opt.MapFrom(src => src.Hand.ToString()))
            .ForMember(dest => dest.Backhand, opt => opt.MapFrom(src => src.Backhand.ToString()))
            .ForMember(dest => dest.Association, opt => opt.MapFrom(src => src.Association.ToString()));

        CreateMap<Player, PlayerDetailDto>()
            .ForMember(dest => dest.Hand, opt => opt.MapFrom(src => src.Hand.ToString()))
            .ForMember(dest => dest.Backhand, opt => opt.MapFrom(src => src.Backhand.ToString()))
            .ForMember(dest => dest.Association, opt => opt.MapFrom(src => src.Association.ToString()));

        // Ranking mappings
        CreateMap<Ranking, RankingHistoryDto>();

        // Player statistics mappings
        CreateMap<PlayerStatistics, PlayerSeasonStatsDto>();

        // Tournament mappings
        CreateMap<Tournament, TournamentDto>()
            .ForMember(dest => dest.Surface, opt => opt.MapFrom(src => src.Surface.ToString()))
            .ForMember(dest => dest.Association, opt => opt.MapFrom(src => src.Association.ToString()));

        CreateMap<Tournament, TournamentDetailDto>()
            .ForMember(dest => dest.Surface, opt => opt.MapFrom(src => src.Surface.ToString()))
            .ForMember(dest => dest.Association, opt => opt.MapFrom(src => src.Association.ToString()));

        // Match mappings
        CreateMap<Match, MatchDto>()
            .ForMember(dest => dest.Round, opt => opt.MapFrom(src => src.Round.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Match, MatchDetailDto>()
            .ForMember(dest => dest.Round, opt => opt.MapFrom(src => src.Round.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // Set mappings
        CreateMap<Set, SetDto>();

        // Match statistics mappings
        CreateMap<MatchStatistics, MatchStatisticsDto>();

        // Season mappings
        CreateMap<Season, SeasonDto>()
            .ForMember(dest => dest.Association, opt => opt.MapFrom(src => src.Association.ToString()));

        // Ranking mappings for list
        CreateMap<Ranking, RankingDto>()
            .ForMember(dest => dest.Association, opt => opt.MapFrom(src => src.Association.ToString()));
    }
}

// Additional DTOs needed for mappings

public class TournamentDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public string Surface { get; set; } = "Unknown";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PrizeMoney { get; set; }
    public string? Currency { get; set; }
    public string Association { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsCompleted { get; set; }
}

public class TournamentDetailDto : TournamentDto
{
    public ICollection<MatchDto> Matches { get; set; } = new List<MatchDto>();
}

public class MatchDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string Round { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? DurationMinutes { get; set; }
    public int Player1Id { get; set; }
    public string? Player1Name { get; set; }
    public int Player2Id { get; set; }
    public string? Player2Name { get; set; }
    public int? WinnerId { get; set; }
    public string? Score { get; set; }
}

public class MatchDetailDto : MatchDto
{
    public int TournamentId { get; set; }
    public string? TournamentName { get; set; }
    public ICollection<SetDto> Sets { get; set; } = new List<SetDto>();
    public MatchStatisticsDto? Statistics { get; set; }
}

public class SetDto
{
    public int SetNumber { get; set; }
    public int Player1Games { get; set; }
    public int Player2Games { get; set; }
    public int? TiebreakPlayer1Points { get; set; }
    public int? TiebreakPlayer2Points { get; set; }
    public bool IsTiebreak { get; set; }
}

public class MatchStatisticsDto
{
    public int? Player1Aces { get; set; }
    public int? Player1DoubleFaults { get; set; }
    public int? Player1FirstServePercentage { get; set; }
    public int? Player1FirstServesIn { get; set; }
    public int? Player1FirstServesTotal { get; set; }
    public int? Player1FirstServePointsWon { get; set; }
    public int? Player1FirstServePointsTotal { get; set; }
    public int? Player1SecondServePointsWon { get; set; }
    public int? Player1SecondServePointsTotal { get; set; }
    public int? Player1BreakPointsSaved { get; set; }
    public int? Player1BreakPointsFaced { get; set; }
    public int? Player1Winners { get; set; }
    public int? Player1UnforcedErrors { get; set; }
    
    // Calculated percentages for Player 1
    public decimal? Player1FirstServePercentageCalc => Player1FirstServesIn.HasValue && Player1FirstServesTotal.HasValue && Player1FirstServesTotal.Value > 0 ? Math.Round((decimal)Player1FirstServesIn.Value / Player1FirstServesTotal.Value * 100, 1) : null;
    public decimal? Player1FirstServePointsWonPercentage => Player1FirstServePointsWon.HasValue && Player1FirstServePointsTotal.HasValue && Player1FirstServePointsTotal.Value > 0 ? Math.Round((decimal)Player1FirstServePointsWon.Value / Player1FirstServePointsTotal.Value * 100, 1) : null;
    public decimal? Player1SecondServePointsWonPercentage => Player1SecondServePointsWon.HasValue && Player1SecondServePointsTotal.HasValue && Player1SecondServePointsTotal.Value > 0 ? Math.Round((decimal)Player1SecondServePointsWon.Value / Player1SecondServePointsTotal.Value * 100, 1) : null;
    public decimal? Player1BreakPointsSavedPercentage => Player1BreakPointsSaved.HasValue && Player1BreakPointsFaced.HasValue && Player1BreakPointsFaced.Value > 0 ? Math.Round((decimal)Player1BreakPointsSaved.Value / Player1BreakPointsFaced.Value * 100, 1) : null;
    
    public int? Player2Aces { get; set; }
    public int? Player2DoubleFaults { get; set; }
    public int? Player2FirstServePercentage { get; set; }
    public int? Player2FirstServesIn { get; set; }
    public int? Player2FirstServesTotal { get; set; }
    public int? Player2FirstServePointsWon { get; set; }
    public int? Player2FirstServePointsTotal { get; set; }
    public int? Player2SecondServePointsWon { get; set; }
    public int? Player2SecondServePointsTotal { get; set; }
    public int? Player2BreakPointsSaved { get; set; }
    public int? Player2BreakPointsFaced { get; set; }
    public int? Player2Winners { get; set; }
    public int? Player2UnforcedErrors { get; set; }
    
    // Calculated percentages for Player 2
    public decimal? Player2FirstServePercentageCalc => Player2FirstServesIn.HasValue && Player2FirstServesTotal.HasValue && Player2FirstServesTotal.Value > 0 ? Math.Round((decimal)Player2FirstServesIn.Value / Player2FirstServesTotal.Value * 100, 1) : null;
    public decimal? Player2FirstServePointsWonPercentage => Player2FirstServePointsWon.HasValue && Player2FirstServePointsTotal.HasValue && Player2FirstServePointsTotal.Value > 0 ? Math.Round((decimal)Player2FirstServePointsWon.Value / Player2FirstServePointsTotal.Value * 100, 1) : null;
    public decimal? Player2SecondServePointsWonPercentage => Player2SecondServePointsWon.HasValue && Player2SecondServePointsTotal.HasValue && Player2SecondServePointsTotal.Value > 0 ? Math.Round((decimal)Player2SecondServePointsWon.Value / Player2SecondServePointsTotal.Value * 100, 1) : null;
    public decimal? Player2BreakPointsSavedPercentage => Player2BreakPointsSaved.HasValue && Player2BreakPointsFaced.HasValue && Player2BreakPointsFaced.Value > 0 ? Math.Round((decimal)Player2BreakPointsSaved.Value / Player2BreakPointsFaced.Value * 100, 1) : null;
}

public class SeasonDto
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public int Year { get; set; }
    public string Association { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
}

public class RankingDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? Country { get; set; }
    public int Rank { get; set; }
    public int Points { get; set; }
    public int? PreviousRank { get; set; }
    public int? RankChange { get; set; }
    public DateTime RankingDate { get; set; }
    public string Association { get; set; } = string.Empty;
}
