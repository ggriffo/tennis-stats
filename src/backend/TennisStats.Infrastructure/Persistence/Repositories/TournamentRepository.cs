using Microsoft.EntityFrameworkCore;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.Persistence.Repositories;

/// <summary>
/// Tournament repository implementation
/// </summary>
public class TournamentRepository : Repository<Tournament>, ITournamentRepository
{
    public TournamentRepository(TennisStatsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tournament>> GetBySeasonAsync(int seasonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.SeasonId == seasonId)
            .OrderBy(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tournament>> GetByAssociationAsync(Association association, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Association == association)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tournament>> GetBySurfaceAsync(Surface surface, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Surface == surface)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tournament?> GetWithMatchesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Matches)
                .ThenInclude(m => m.Player1)
            .Include(t => t.Matches)
                .ThenInclude(m => m.Player2)
            .Include(t => t.Matches)
                .ThenInclude(m => m.Sets)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Tournament>> GetUpcomingAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.StartDate > DateTime.UtcNow && !t.IsCompleted)
            .OrderBy(t => t.StartDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tournament>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.StartDate >= startDate && t.StartDate <= endDate)
            .OrderBy(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Tournament?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.ExternalId == externalId, cancellationToken);
    }
}
