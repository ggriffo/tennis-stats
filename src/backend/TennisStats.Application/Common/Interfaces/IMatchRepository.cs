using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Match entity
/// </summary>
public interface IMatchRepository : IRepository<Match>
{
    Task<IEnumerable<Match>> GetByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetByPlayerAsync(int playerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetHeadToHeadAsync(int player1Id, int player2Id, CancellationToken cancellationToken = default);
    Task<Match?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetByStatusAsync(MatchStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetRecentByPlayerAsync(int playerId, int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
