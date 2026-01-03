using JobApplicationTracker.Domain.Common;

namespace JobApplicationTracker.Domain.Entities;

/// <summary>
/// Represents a tenant (company/organization) in the multi-tenant system.
/// Each tenant has isolated data and users.
/// </summary>
public class Tenant : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// The name of the company or organization.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique subdomain identifier for the tenant (e.g., "acme" for acme.jobtracker.com).
    /// </summary>
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the tenant account is active.
    /// Inactive tenants cannot access the system.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    /// <summary>
    /// Collection of users belonging to this tenant.
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Collection of job postings created by this tenant.
    /// </summary>
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
