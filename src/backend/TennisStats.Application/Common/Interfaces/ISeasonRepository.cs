using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Season entity
/// </summary>
public interface ISeasonRepository : IRepository<Season>
{
    Task<Season?> GetCurrentSeasonAsync(Association association, CancellationToken cancellationToken = default);
    Task<IEnumerable<Season>> GetByAssociationAsync(Association association, CancellationToken cancellationToken = default);
    Task<Season?> GetByYearAsync(Association association, int year, CancellationToken cancellationToken = default);
    Task<Season?> GetWithTournamentsAsync(int id, CancellationToken cancellationToken = default);
}
