using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Tournament entity
/// </summary>
public interface ITournamentRepository : IRepository<Tournament>
{
    Task<IEnumerable<Tournament>> GetBySeasonAsync(int seasonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tournament>> GetByAssociationAsync(Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tournament>> GetBySurfaceAsync(Surface surface, CancellationToken cancellationToken = default);
    Task<Tournament?> GetWithMatchesAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tournament>> GetUpcomingAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tournament>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
