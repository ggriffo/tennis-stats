using Microsoft.EntityFrameworkCore;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.Persistence.Repositories;

/// <summary>
/// Season repository implementation
/// </summary>
public class SeasonRepository : Repository<Season>, ISeasonRepository
{
    public SeasonRepository(TennisStatsDbContext context) : base(context)
    {
    }

    public async Task<Season?> GetCurrentSeasonAsync(Association association, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Association == association && s.IsCurrent)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Season>> GetByAssociationAsync(Association association, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Association == association)
            .OrderByDescending(s => s.Year)
            .ToListAsync(cancellationToken);
    }

    public async Task<Season?> GetByYearAsync(Association association, int year, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Year == year && s.Association == association, cancellationToken);
    }

    public async Task<Season?> GetWithTournamentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Tournaments.OrderBy(t => t.StartDate))
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public override async Task<Season?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.ExternalId == externalId, cancellationToken);
    }
}
