using JobApplicationTracker.Domain.Common;

namespace JobApplicationTracker.Domain.Entities;

/// <summary>
/// Represents a job application submitted by a candidate.
/// </summary>
public class Application : BaseEntity, IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// The tenant this application belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The job this application is for.
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// The candidate (user) who submitted this application.
    /// </summary>
    public Guid CandidateId { get; set; }

    /// <summary>
    /// URL to the candidate's resume/CV file.
    /// </summary>
    public string ResumeUrl { get; set; } = string.Empty;

    /// <summary>
    /// Optional cover letter text.
    /// </summary>
    public string? CoverLetter { get; set; }

    /// <summary>
    /// Date and time when the application was submitted.
    /// </summary>
    public DateTime AppliedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// The job this application is for.
    /// </summary>
    public virtual Job Job { get; set; } = null!;

    /// <summary>
    /// The candidate who submitted this application.
    /// </summary>
    public virtual User Candidate { get; set; } = null!;

    /// <summary>
    /// Status history of this application.
    /// </summary>
    public virtual ICollection<ApplicationStatus> StatusHistory { get; set; } = new List<ApplicationStatus>();

    /// <summary>
    /// Interviews scheduled for this application.
    /// </summary>
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    /// <summary>
    /// Gets the current (latest) status of the application.
    /// </summary>
    public ApplicationStatus? CurrentStatus => StatusHistory
        .OrderByDescending(s => s.ChangedAt)
        .FirstOrDefault();
}
