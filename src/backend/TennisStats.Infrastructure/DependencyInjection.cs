using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TennisStats.Application.Common.Interfaces;
using TennisStats.Infrastructure.ExternalServices;
using TennisStats.Infrastructure.Persistence;
using TennisStats.Infrastructure.Persistence.Repositories;
using TennisStats.Infrastructure.Services;

namespace TennisStats.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<TennisStatsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(TennisStatsDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IRankingRepository, RankingRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // External API Service
        services.AddHttpClient<IExternalTennisApiService, BallDontLieApiService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Data Import Service
        services.AddScoped<IDataImportService, DataImportService>();

        return services;
    }
}
