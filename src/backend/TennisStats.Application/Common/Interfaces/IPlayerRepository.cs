using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Player entity
/// </summary>
public interface IPlayerRepository : IRepository<Player>
{
    Task<IEnumerable<Player>> GetByAssociationAsync(Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Player?> GetWithRankingsAsync(int id, CancellationToken cancellationToken = default);
    Task<Player?> GetWithStatisticsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Player>> GetTopRankedAsync(Association association, int count, CancellationToken cancellationToken = default);
}
