using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TennisStats.Domain.Entities;
using TennisStats.Domain.Enums;

namespace TennisStats.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context
/// </summary>
public class TennisStatsDbContext : DbContext
{
    public TennisStatsDbContext(DbContextOptions<TennisStatsDbContext> options) : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Set> Sets => Set<Set>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Ranking> Rankings => Set<Ranking>();
    public DbSet<MatchStatistics> MatchStatistics => Set<MatchStatistics>();
    public DbSet<PlayerStatistics> PlayerStatistics => Set<PlayerStatistics>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure all DateTime properties to use UTC
        ConfigureDateTimeUtcConversion(modelBuilder);

        // Player configuration
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId);
            entity.HasIndex(e => e.Association);
            entity.HasIndex(e => new { e.LastName, e.FirstName });
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Hand).HasConversion<int>();
            entity.Property(e => e.Backhand).HasConversion<int>();
            entity.Property(e => e.Association).HasConversion<int>();
        });

        // Season configuration
        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId);
            entity.HasIndex(e => new { e.Year, e.Association }).IsUnique();
            entity.Property(e => e.Association).HasConversion<int>();
        });

        // Tournament configuration
        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId);
            entity.HasIndex(e => e.Association);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Surface).HasConversion<int>();
            entity.Property(e => e.Association).HasConversion<int>();

            entity.HasOne(e => e.Season)
                .WithMany(s => s.Tournaments)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Match configuration
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.CourtName).HasMaxLength(100);
            entity.Property(e => e.Round).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.Tournament)
                .WithMany(t => t.Matches)
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Player1)
                .WithMany(p => p.MatchesAsPlayer1)
                .HasForeignKey(e => e.Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Player2)
                .WithMany(p => p.MatchesAsPlayer2)
                .HasForeignKey(e => e.Player2Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Winner)
                .WithMany(p => p.MatchesWon)
                .HasForeignKey(e => e.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Set configuration
        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Match)
                .WithMany(m => m.Sets)
                .HasForeignKey(e => e.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Game configuration
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Score).HasMaxLength(50);
            entity.HasOne(e => e.Set)
                .WithMany(s => s.Games)
                .HasForeignKey(e => e.SetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Ranking configuration
        modelBuilder.Entity<Ranking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId);
            entity.HasIndex(e => new { e.PlayerId, e.RankingDate });
            entity.HasIndex(e => new { e.Association, e.RankingDate, e.Rank });
            entity.Property(e => e.Association).HasConversion<int>();

            entity.HasOne(e => e.Player)
                .WithMany(p => p.Rankings)
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Season)
                .WithMany(s => s.Rankings)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Match statistics configuration
        modelBuilder.Entity<MatchStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Match)
                .WithOne(m => m.Statistics)
                .HasForeignKey<MatchStatistics>(e => e.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Player statistics configuration
        modelBuilder.Entity<PlayerStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PlayerId, e.Year }).IsUnique();
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.FirstServePercentage).HasPrecision(5, 2);
            entity.Property(e => e.FirstServePointsWonPercentage).HasPrecision(5, 2);
            entity.Property(e => e.SecondServePointsWonPercentage).HasPrecision(5, 2);
            entity.Property(e => e.BreakPointsSavedPercentage).HasPrecision(5, 2);
            entity.Property(e => e.ServiceGamesWonPercentage).HasPrecision(5, 2);
            entity.Property(e => e.ReturnGamesWonPercentage).HasPrecision(5, 2);

            entity.HasOne(e => e.Player)
                .WithMany(p => p.Statistics)
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Configures all DateTime and DateTime? properties to ensure UTC kind for PostgreSQL compatibility
    /// </summary>
    private static void ConfigureDateTimeUtcConversion(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Domain.Common.BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
