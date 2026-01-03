using JobApplicationTracker.Domain.Common;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Domain.Entities;

/// <summary>
/// Represents a status change in an application's lifecycle.
/// Maintains a history of all status changes for audit purposes.
/// </summary>
public class ApplicationStatus : BaseEntity
{
    /// <summary>
    /// The application this status belongs to.
    /// </summary>
    public Guid ApplicationId { get; set; }

    /// <summary>
    /// The status type.
    /// </summary>
    public ApplicationStatusType Status { get; set; }

    /// <summary>
    /// Optional notes about this status change.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// The user who made this status change.
    /// </summary>
    public Guid ChangedByUserId { get; set; }

    /// <summary>
    /// Date and time when the status was changed.
    /// </summary>
    public DateTime ChangedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// The application this status belongs to.
    /// </summary>
    public virtual Application Application { get; set; } = null!;

    /// <summary>
    /// The user who made this status change.
    /// </summary>
    public virtual User ChangedByUser { get; set; } = null!;
}
