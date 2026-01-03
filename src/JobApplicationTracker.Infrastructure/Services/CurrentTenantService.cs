using JobApplicationTracker.Application.Common.Interfaces;

namespace JobApplicationTracker.Infrastructure.Services;

/// <summary>
/// Service for managing the current tenant context.
/// Used for multi-tenancy data isolation.
/// </summary>
public class CurrentTenantService : ICurrentTenantService
{
    private Guid? _tenantId;

    /// <summary>
    /// Gets the current tenant's unique identifier.
    /// </summary>
    public Guid? TenantId => _tenantId;

    /// <summary>
    /// Sets the current tenant context.
    /// Called by tenant resolution middleware.
    /// </summary>
    public void SetTenant(Guid tenantId)
    {
        _tenantId = tenantId;
    }
}
