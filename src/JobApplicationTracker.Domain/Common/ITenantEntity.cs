namespace JobApplicationTracker.Domain.Common;

/// <summary>
/// Interface for entities that belong to a specific tenant.
/// Used for multi-tenancy data isolation.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// The tenant identifier this entity belongs to.
    /// </summary>
    Guid TenantId { get; set; }
}
