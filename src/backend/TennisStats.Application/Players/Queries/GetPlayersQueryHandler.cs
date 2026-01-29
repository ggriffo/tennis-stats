using AutoMapper;
using MediatR;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Application.Common.Models;
using TennisStats.Application.Players.DTOs;

namespace TennisStats.Application.Players.Queries;

/// <summary>
/// Handler for GetPlayersQuery
/// </summary>
public class GetPlayersQueryHandler : IRequestHandler<GetPlayersQuery, Result<PaginatedList<PlayerDto>>>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRankingRepository _rankingRepository;
    private readonly IMapper _mapper;

    public GetPlayersQueryHandler(
        IPlayerRepository playerRepository,
        IRankingRepository rankingRepository,
        IMapper mapper)
    {
        _playerRepository = playerRepository;
        _rankingRepository = rankingRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<PlayerDto>>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var players = await _playerRepository.GetByAssociationAsync(request.Association, cancellationToken);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLowerInvariant();
                players = players.Where(p => 
                    p.FullName.ToLowerInvariant().Contains(searchLower) ||
                    p.FirstName.ToLowerInvariant().Contains(searchLower) ||
                    p.LastName.ToLowerInvariant().Contains(searchLower));
            }

            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                players = players.Where(p => p.Country != null && 
                    p.Country.Equals(request.Country, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = players.Count();
            var pagedPlayers = players
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var playerDtos = _mapper.Map<List<PlayerDto>>(pagedPlayers);

            // Get current rankings for players
            foreach (var dto in playerDtos)
            {
                var ranking = await _rankingRepository.GetPlayerCurrentRankingAsync(dto.Id, cancellationToken);
                if (ranking != null)
                {
                    dto.CurrentRank = ranking.Rank;
                    dto.CurrentPoints = ranking.Points;
                }
            }

            var paginatedList = new PaginatedList<PlayerDto>(playerDtos, totalCount, request.Page, request.PageSize);
            return Result<PaginatedList<PlayerDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<PlayerDto>>.Failure($"Error retrieving players: {ex.Message}");
        }
    }
}
