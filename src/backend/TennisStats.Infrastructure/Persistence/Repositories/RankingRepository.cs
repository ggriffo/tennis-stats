using Microsoft.EntityFrameworkCore;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.Persistence.Repositories;

/// <summary>
/// Ranking repository implementation
/// </summary>
public class RankingRepository : Repository<Ranking>, IRankingRepository
{
    public RankingRepository(TennisStatsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ranking>> GetCurrentRankingsAsync(Association association, CancellationToken cancellationToken = default)
    {
        var latestDate = await _dbSet
            .Where(r => r.Association == association)
            .MaxAsync(r => (DateTime?)r.RankingDate, cancellationToken);

        if (latestDate == null)
            return Enumerable.Empty<Ranking>();

        return await _dbSet
            .Where(r => r.Association == association && r.RankingDate == latestDate)
            .Include(r => r.Player)
            .OrderBy(r => r.Rank)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ranking>> GetPlayerRankingHistoryAsync(int playerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.PlayerId == playerId)
            .OrderByDescending(r => r.RankingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ranking?> GetPlayerCurrentRankingAsync(int playerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.PlayerId == playerId)
            .OrderByDescending(r => r.RankingDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Ranking?> GetPlayerRankingForDateAsync(int playerId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.PlayerId == playerId && r.RankingDate.Date == date.Date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ranking>> GetByDateAsync(DateTime date, Association association, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.Association == association && r.RankingDate.Date == date.Date)
            .Include(r => r.Player)
            .OrderBy(r => r.Rank)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ranking>> GetTopRankingsAsync(Association association, int count, CancellationToken cancellationToken = default)
    {
        var latestDate = await _dbSet
            .Where(r => r.Association == association)
            .MaxAsync(r => (DateTime?)r.RankingDate, cancellationToken);

        if (latestDate == null)
            return Enumerable.Empty<Ranking>();

        return await _dbSet
            .Where(r => r.Association == association && r.RankingDate == latestDate)
            .Include(r => r.Player)
            .OrderBy(r => r.Rank)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Ranking?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.ExternalId == externalId, cancellationToken);
    }
}
