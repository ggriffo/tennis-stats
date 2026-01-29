using MediatR;
using TennisStats.Application.Common.Models;
using TennisStats.Application.Players.DTOs;

namespace TennisStats.Application.Players.Queries;

/// <summary>
/// Query to get a player by ID
/// </summary>
public record GetPlayerByIdQuery(int Id) : IRequest<Result<PlayerDetailDto>>;
