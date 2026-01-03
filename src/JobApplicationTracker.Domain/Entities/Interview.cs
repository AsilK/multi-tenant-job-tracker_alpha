using JobApplicationTracker.Domain.Common;

namespace JobApplicationTracker.Domain.Entities;

/// <summary>
/// Represents an interview scheduled for a job application.
/// </summary>
public class Interview : BaseEntity, IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// The tenant this interview belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The application this interview is for.
    /// </summary>
    public Guid ApplicationId { get; set; }

    /// <summary>
    /// The HR/Admin user conducting the interview.
    /// </summary>
    public Guid InterviewerId { get; set; }

    /// <summary>
    /// Scheduled date and time for the interview.
    /// </summary>
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// Expected duration in minutes.
    /// </summary>
    public int DurationMinutes { get; set; } = 60;

    /// <summary>
    /// Physical location for in-person interviews (optional).
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Video conference meeting link for remote interviews (optional).
    /// </summary>
    public string? MeetingLink { get; set; }

    /// <summary>
    /// Pre-interview notes or agenda.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Post-interview feedback from the interviewer.
    /// </summary>
    public string? Feedback { get; set; }

    /// <summary>
    /// Indicates whether the interview has been completed.
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    // Navigation properties
    /// <summary>
    /// The application this interview is for.
    /// </summary>
    public virtual Application Application { get; set; } = null!;

    /// <summary>
    /// The user conducting the interview.
    /// </summary>
    public virtual User Interviewer { get; set; } = null!;
}
