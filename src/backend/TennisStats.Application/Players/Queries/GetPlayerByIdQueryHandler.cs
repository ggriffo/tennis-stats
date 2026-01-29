using AutoMapper;
using MediatR;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Application.Common.Models;
using TennisStats.Application.Players.DTOs;

namespace TennisStats.Application.Players.Queries;

/// <summary>
/// Handler for GetPlayerByIdQuery
/// </summary>
public class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, Result<PlayerDetailDto>>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRankingRepository _rankingRepository;
    private readonly IMapper _mapper;

    public GetPlayerByIdQueryHandler(
        IPlayerRepository playerRepository,
        IRankingRepository rankingRepository,
        IMapper mapper)
    {
        _playerRepository = playerRepository;
        _rankingRepository = rankingRepository;
        _mapper = mapper;
    }

    public async Task<Result<PlayerDetailDto>> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var player = await _playerRepository.GetWithStatisticsAsync(request.Id, cancellationToken);
            
            if (player == null)
            {
                return Result<PlayerDetailDto>.Failure("Player not found", "NOT_FOUND");
            }

            var playerDto = _mapper.Map<PlayerDetailDto>(player);

            // Get ranking history
            var rankings = await _rankingRepository.GetPlayerRankingHistoryAsync(request.Id, cancellationToken);
            playerDto.RankingHistory = _mapper.Map<List<RankingHistoryDto>>(rankings.OrderByDescending(r => r.RankingDate).Take(52));

            // Get current ranking
            var currentRanking = rankings.OrderByDescending(r => r.RankingDate).FirstOrDefault();
            if (currentRanking != null)
            {
                playerDto.CurrentRank = currentRanking.Rank;
                playerDto.CurrentPoints = currentRanking.Points;
            }

            // Get current season stats
            var currentStats = player.Statistics.OrderByDescending(s => s.Year).FirstOrDefault();
            if (currentStats != null)
            {
                playerDto.CurrentSeasonStats = _mapper.Map<PlayerSeasonStatsDto>(currentStats);
            }

            return Result<PlayerDetailDto>.Success(playerDto);
        }
        catch (Exception ex)
        {
            return Result<PlayerDetailDto>.Failure($"Error retrieving player: {ex.Message}");
        }
    }
}
