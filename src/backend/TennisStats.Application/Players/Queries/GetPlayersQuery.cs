using MediatR;
using TennisStats.Application.Common.Models;
using TennisStats.Application.Players.DTOs;
using TennisStats.Domain.Enums;

namespace TennisStats.Application.Players.Queries;

/// <summary>
/// Query to get paginated list of players
/// </summary>
public record GetPlayersQuery(
    Association Association,
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    string? Country = null
) : IRequest<Result<PaginatedList<PlayerDto>>>;
