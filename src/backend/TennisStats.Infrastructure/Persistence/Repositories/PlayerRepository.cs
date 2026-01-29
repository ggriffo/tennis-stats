using Microsoft.EntityFrameworkCore;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.Persistence.Repositories;

/// <summary>
/// Player repository implementation
/// </summary>
public class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public PlayerRepository(TennisStatsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Player>> GetByAssociationAsync(Association association, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Association == association)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Country != null && p.Country.ToLower() == country.ToLower())
            .OrderBy(p => p.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Player>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var searchLower = name.ToLower();
        return await _dbSet
            .Where(p => p.FullName.ToLower().Contains(searchLower) ||
                       p.FirstName.ToLower().Contains(searchLower) ||
                       p.LastName.ToLower().Contains(searchLower))
            .OrderBy(p => p.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Player?> GetWithRankingsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Rankings.OrderByDescending(r => r.RankingDate).Take(52))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Player?> GetWithStatisticsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Statistics.OrderByDescending(s => s.Year))
            .Include(p => p.Rankings.OrderByDescending(r => r.RankingDate).Take(1))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Player>> GetTopRankedAsync(Association association, int count, CancellationToken cancellationToken = default)
    {
        var latestRankingDate = await _context.Rankings
            .Where(r => r.Association == association)
            .MaxAsync(r => (DateTime?)r.RankingDate, cancellationToken);

        if (latestRankingDate == null)
            return Enumerable.Empty<Player>();

        var topPlayerIds = await _context.Rankings
            .Where(r => r.Association == association && r.RankingDate == latestRankingDate)
            .OrderBy(r => r.Rank)
            .Take(count)
            .Select(r => r.PlayerId)
            .ToListAsync(cancellationToken);

        return await _dbSet
            .Where(p => topPlayerIds.Contains(p.Id))
            .Include(p => p.Rankings.Where(r => r.RankingDate == latestRankingDate))
            .ToListAsync(cancellationToken);
    }

    public override async Task<Player?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.ExternalId == externalId, cancellationToken);
    }
}
