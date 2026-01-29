using Microsoft.EntityFrameworkCore;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.Persistence.Repositories;

/// <summary>
/// Match repository implementation
/// </summary>
public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(TennisStatsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Match>> GetByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.TournamentId == tournamentId)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Sets)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetByPlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.Player1Id == playerId || m.Player2Id == playerId)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Tournament)
            .Include(m => m.Sets)
            .OrderByDescending(m => m.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetHeadToHeadAsync(int player1Id, int player2Id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => (m.Player1Id == player1Id && m.Player2Id == player2Id) ||
                       (m.Player1Id == player2Id && m.Player2Id == player1Id))
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Tournament)
            .Include(m => m.Sets)
            .OrderByDescending(m => m.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Match?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .Include(m => m.Tournament)
            .Include(m => m.Sets)
            .Include(m => m.Statistics)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetByStatusAsync(MatchStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.Status == status)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Tournament)
            .OrderBy(m => m.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetRecentByPlayerAsync(int playerId, int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => (m.Player1Id == playerId || m.Player2Id == playerId) && m.Status == MatchStatus.Completed)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Tournament)
            .Include(m => m.Sets)
            .OrderByDescending(m => m.EndedAt ?? m.ScheduledAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Match>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.ScheduledAt >= startDate && m.ScheduledAt <= endDate)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Tournament)
            .OrderBy(m => m.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Match?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(m => m.ExternalId == externalId, cancellationToken);
    }
}
