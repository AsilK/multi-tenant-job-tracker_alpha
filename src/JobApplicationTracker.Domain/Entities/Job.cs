using JobApplicationTracker.Domain.Common;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Domain.Entities;

/// <summary>
/// Represents a job posting created by a tenant.
/// </summary>
public class Job : BaseEntity, IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// The tenant this job belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The job title or position name.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Full job description including responsibilities and qualifications.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The department or team for this position.
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Physical location or "Remote" if fully remote.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Type of employment (FullTime, PartTime, Contract, etc.).
    /// </summary>
    public JobType Type { get; set; }

    /// <summary>
    /// Minimum salary for this position (optional).
    /// </summary>
    public decimal? MinSalary { get; set; }

    /// <summary>
    /// Maximum salary for this position (optional).
    /// </summary>
    public decimal? MaxSalary { get; set; }

    /// <summary>
    /// Indicates whether the job is currently accepting applications.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time when the job was posted.
    /// </summary>
    public DateTime PostedAt { get; set; }

    /// <summary>
    /// Application deadline (optional).
    /// </summary>
    public DateTime? ClosingDate { get; set; }

    // Navigation properties
    /// <summary>
    /// The tenant that created this job posting.
    /// </summary>
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Applications received for this job.
    /// </summary>
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}
