namespace TennisStats.Domain.Common;

/// <summary>
/// Base entity class with external API ID support
/// </summary>
public abstract class BaseEntityWithExternalId : BaseEntity
{
    /// <summary>
    /// The ID from the external API (balldontlie.io)
    /// </summary>
    public int ExternalId { get; set; }
    
    /// <summary>
    /// Timestamp when data was last synced from external API
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }
}
