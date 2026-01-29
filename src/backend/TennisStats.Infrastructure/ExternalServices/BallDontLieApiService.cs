using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.ExternalServices;

/// <summary>
/// Implementation of external tennis API service (balldontlie.io)
/// </summary>
public class BallDontLieApiService : IExternalTennisApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BallDontLieApiService> _logger;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string WtaBaseUrl = "https://api.balldontlie.io/wta/v1";
    private const string AtpBaseUrl = "https://api.balldontlie.io/atp/v1";

    public BallDontLieApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<BallDontLieApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["ExternalApi:BallDontLie:ApiKey"] ?? throw new ArgumentNullException("API Key not configured");
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", _apiKey);
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private string GetBaseUrl(Association association) => association == Association.WTA ? WtaBaseUrl : AtpBaseUrl;

    #region Players

    public async Task<ExternalPlayerDto?> GetPlayerAsync(int externalId, Association association, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/players/{externalId}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get player {ExternalId}: {StatusCode}", externalId, response.StatusCode);
                return null;
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiPlayerResponse>(_jsonOptions, cancellationToken);
            return MapToExternalPlayerDto(apiResponse?.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching player {ExternalId}", externalId);
            return null;
        }
    }

    public async Task<IEnumerable<ExternalPlayerDto>> GetPlayersAsync(Association association, int page, int perPage, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/players?page={page}&per_page={perPage}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get players: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ExternalPlayerDto>();
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiPlayersResponse>(_jsonOptions, cancellationToken);
            return apiResponse?.Data?.Select(MapToExternalPlayerDto).Where(p => p != null).Cast<ExternalPlayerDto>() 
                   ?? Enumerable.Empty<ExternalPlayerDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching players");
            return Enumerable.Empty<ExternalPlayerDto>();
        }
    }

    public async Task<IEnumerable<ExternalPlayerDto>> SearchPlayersAsync(string search, Association association, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/players?search={Uri.EscapeDataString(search)}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to search players: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ExternalPlayerDto>();
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiPlayersResponse>(_jsonOptions, cancellationToken);
            return apiResponse?.Data?.Select(MapToExternalPlayerDto).Where(p => p != null).Cast<ExternalPlayerDto>() 
                   ?? Enumerable.Empty<ExternalPlayerDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching players");
            return Enumerable.Empty<ExternalPlayerDto>();
        }
    }

    #endregion

    #region Tournaments

    public async Task<ExternalTournamentDto?> GetTournamentAsync(int externalId, Association association, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/tournaments/{externalId}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get tournament {ExternalId}: {StatusCode}", externalId, response.StatusCode);
                return null;
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiTournamentResponse>(_jsonOptions, cancellationToken);
            return MapToExternalTournamentDto(apiResponse?.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tournament {ExternalId}", externalId);
            return null;
        }
    }

    public async Task<IEnumerable<ExternalTournamentDto>> GetTournamentsAsync(Association association, int seasonYear, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/tournaments?season={seasonYear}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get tournaments: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ExternalTournamentDto>();
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiTournamentsResponse>(_jsonOptions, cancellationToken);
            return apiResponse?.Data?.Select(MapToExternalTournamentDto).Where(t => t != null).Cast<ExternalTournamentDto>() 
                   ?? Enumerable.Empty<ExternalTournamentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tournaments");
            return Enumerable.Empty<ExternalTournamentDto>();
        }
    }

    #endregion

    #region Matches

    public async Task<ExternalMatchDto?> GetMatchAsync(int externalId, Association association, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/matches/{externalId}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get match {ExternalId}: {StatusCode}", externalId, response.StatusCode);
                return null;
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiMatchResponse>(_jsonOptions, cancellationToken);
            return MapToExternalMatchDto(apiResponse?.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching match {ExternalId}", externalId);
            return null;
        }
    }

    public async Task<IEnumerable<ExternalMatchDto>> GetMatchesAsync(Association association, int? tournamentId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>();
            if (tournamentId.HasValue) queryParams.Add($"tournament_ids[]={tournamentId.Value}");
            if (startDate.HasValue) queryParams.Add($"start_date={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"end_date={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/matches{queryString}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get matches: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ExternalMatchDto>();
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiMatchesResponse>(_jsonOptions, cancellationToken);
            return apiResponse?.Data?.Select(MapToExternalMatchDto).Where(m => m != null).Cast<ExternalMatchDto>() 
                   ?? Enumerable.Empty<ExternalMatchDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching matches");
            return Enumerable.Empty<ExternalMatchDto>();
        }
    }

    #endregion

    #region Rankings

    public async Task<IEnumerable<ExternalRankingDto>> GetRankingsAsync(Association association, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/rankings", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get rankings: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ExternalRankingDto>();
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiRankingsResponse>(_jsonOptions, cancellationToken);
            return apiResponse?.Data?.Select(MapToExternalRankingDto).Where(r => r != null).Cast<ExternalRankingDto>() 
                   ?? Enumerable.Empty<ExternalRankingDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rankings");
            return Enumerable.Empty<ExternalRankingDto>();
        }
    }

    #endregion

    #region Seasons

    public async Task<IEnumerable<ExternalSeasonDto>> GetSeasonsAsync(Association association, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{GetBaseUrl(association)}/seasons", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get seasons: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ExternalSeasonDto>();
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiSeasonsResponse>(_jsonOptions, cancellationToken);
            return apiResponse?.Data?.Select(MapToExternalSeasonDto).Where(s => s != null).Cast<ExternalSeasonDto>() 
                   ?? Enumerable.Empty<ExternalSeasonDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching seasons");
            return Enumerable.Empty<ExternalSeasonDto>();
        }
    }

    #endregion

    #region Mapping Methods

    private static ExternalPlayerDto? MapToExternalPlayerDto(ApiPlayer? player)
    {
        if (player == null) return null;

        return new ExternalPlayerDto(
            player.Id,
            player.FirstName ?? "",
            player.LastName ?? "",
            player.FullName ?? $"{player.FirstName} {player.LastName}",
            player.Country,
            player.DateOfBirth,
            player.HeightCm,
            player.WeightKg,
            player.Hand,
            player.Backhand,
            player.TurnedProYear,
            player.ImageUrl
        );
    }

    private static ExternalTournamentDto? MapToExternalTournamentDto(ApiTournament? tournament)
    {
        if (tournament == null) return null;

        return new ExternalTournamentDto(
            tournament.Id,
            tournament.Name ?? "",
            tournament.City,
            tournament.Country,
            tournament.Surface,
            tournament.StartDate,
            tournament.EndDate,
            tournament.PrizeMoney,
            tournament.Currency,
            tournament.Category,
            tournament.Season?.Year ?? DateTime.Now.Year
        );
    }

    private static ExternalMatchDto? MapToExternalMatchDto(ApiMatch? match)
    {
        if (match == null) return null;

        var sets = match.Sets?.Select(s => new ExternalSetDto(
            s.SetNumber,
            s.HomeGames,
            s.AwayGames,
            s.TiebreakHomePoints,
            s.TiebreakAwayPoints
        ));

        ExternalMatchStatisticsDto? stats = null;
        if (match.HomePlayerStats != null || match.AwayPlayerStats != null)
        {
            stats = new ExternalMatchStatisticsDto(
                match.HomePlayerStats?.Aces,
                match.HomePlayerStats?.DoubleFaults,
                match.HomePlayerStats?.FirstServePercentage,
                match.AwayPlayerStats?.Aces,
                match.AwayPlayerStats?.DoubleFaults,
                match.AwayPlayerStats?.FirstServePercentage
            );
        }

        return new ExternalMatchDto(
            match.Id,
            match.Tournament?.Id ?? 0,
            match.HomePlayer?.Id ?? 0,
            match.AwayPlayer?.Id ?? 0,
            match.Winner?.Id,
            match.Round,
            match.Status,
            match.Date,
            match.Date,
            match.Date,
            match.DurationMinutes,
            sets,
            stats
        );
    }

    private static ExternalRankingDto? MapToExternalRankingDto(ApiRanking? ranking)
    {
        if (ranking == null) return null;

        return new ExternalRankingDto(
            ranking.Player?.Id ?? 0,
            ranking.Rank,
            ranking.Points,
            ranking.PreviousRank,
            ranking.RankingDate ?? DateTime.Now
        );
    }

    private static ExternalSeasonDto? MapToExternalSeasonDto(ApiSeason? season)
    {
        if (season == null) return null;

        return new ExternalSeasonDto(
            season.Year,
            season.Year,
            season.StartDate,
            season.EndDate,
            season.Year == DateTime.Now.Year
        );
    }

    #endregion

    #region API Response Classes

    private class ApiPlayerResponse { public ApiPlayer? Data { get; set; } }
    private class ApiPlayersResponse { public List<ApiPlayer>? Data { get; set; } }
    private class ApiTournamentResponse { public ApiTournament? Data { get; set; } }
    private class ApiTournamentsResponse { public List<ApiTournament>? Data { get; set; } }
    private class ApiMatchResponse { public ApiMatch? Data { get; set; } }
    private class ApiMatchesResponse { public List<ApiMatch>? Data { get; set; } }
    private class ApiRankingsResponse { public List<ApiRanking>? Data { get; set; } }
    private class ApiSeasonsResponse { public List<ApiSeason>? Data { get; set; } }

    private class ApiPlayer
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? HeightCm { get; set; }
        public int? WeightKg { get; set; }
        public string? Hand { get; set; }
        public string? Backhand { get; set; }
        public int? TurnedProYear { get; set; }
        public string? ImageUrl { get; set; }
    }

    private class ApiTournament
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Surface { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PrizeMoney { get; set; }
        public string? Currency { get; set; }
        public string? Category { get; set; }
        public ApiSeason? Season { get; set; }
    }

    private class ApiMatch
    {
        public int Id { get; set; }
        public ApiTournament? Tournament { get; set; }
        public ApiPlayer? HomePlayer { get; set; }
        public ApiPlayer? AwayPlayer { get; set; }
        public ApiPlayer? Winner { get; set; }
        public string? Round { get; set; }
        public string? Status { get; set; }
        public DateTime? Date { get; set; }
        public int? DurationMinutes { get; set; }
        public List<ApiSet>? Sets { get; set; }
        public ApiPlayerStats? HomePlayerStats { get; set; }
        public ApiPlayerStats? AwayPlayerStats { get; set; }
    }

    private class ApiSet
    {
        public int SetNumber { get; set; }
        public int HomeGames { get; set; }
        public int AwayGames { get; set; }
        public int? TiebreakHomePoints { get; set; }
        public int? TiebreakAwayPoints { get; set; }
    }

    private class ApiPlayerStats
    {
        public int? Aces { get; set; }
        public int? DoubleFaults { get; set; }
        public int? FirstServePercentage { get; set; }
    }

    private class ApiRanking
    {
        public ApiPlayer? Player { get; set; }
        public int Rank { get; set; }
        public int Points { get; set; }
        public int? PreviousRank { get; set; }
        public DateTime? RankingDate { get; set; }
    }

    private class ApiSeason
    {
        public int Year { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    #endregion
}
