using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Ranking entity
/// </summary>
public interface IRankingRepository : IRepository<Ranking>
{
    Task<IEnumerable<Ranking>> GetCurrentRankingsAsync(Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ranking>> GetPlayerRankingHistoryAsync(int playerId, CancellationToken cancellationToken = default);
    Task<Ranking?> GetPlayerCurrentRankingAsync(int playerId, CancellationToken cancellationToken = default);
    Task<Ranking?> GetPlayerRankingForDateAsync(int playerId, DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ranking>> GetByDateAsync(DateTime date, Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ranking>> GetTopRankingsAsync(Association association, int count, CancellationToken cancellationToken = default);
}
