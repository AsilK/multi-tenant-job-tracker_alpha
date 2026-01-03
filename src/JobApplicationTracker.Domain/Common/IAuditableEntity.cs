namespace JobApplicationTracker.Domain.Common;

/// <summary>
/// Interface for entities that track audit information.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Timestamp when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the entity was last updated.
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}
