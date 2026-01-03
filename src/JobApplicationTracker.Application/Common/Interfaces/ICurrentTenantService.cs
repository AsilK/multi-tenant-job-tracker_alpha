namespace JobApplicationTracker.Application.Common.Interfaces;

/// <summary>
/// Interface for accessing the current tenant context.
/// Used for multi-tenancy data isolation.
/// </summary>
public interface ICurrentTenantService
{
    /// <summary>
    /// Gets the current tenant's unique identifier.
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Sets the current tenant context.
    /// Called by tenant resolution middleware.
    /// </summary>
    void SetTenant(Guid tenantId);
}
