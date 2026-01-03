namespace JobApplicationTracker.Domain.Enums;

/// <summary>
/// Status types for job applications.
/// Represents the lifecycle of an application from submission to final outcome.
/// </summary>
public enum ApplicationStatusType
{
    /// <summary>
    /// Application has been submitted and is awaiting review.
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Application is being reviewed by HR or hiring manager.
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Candidate has been shortlisted for further consideration.
    /// </summary>
    Shortlisted = 3,

    /// <summary>
    /// Interview has been scheduled with the candidate.
    /// </summary>
    InterviewScheduled = 4,

    /// <summary>
    /// Job offer has been extended to the candidate.
    /// </summary>
    Offered = 5,

    /// <summary>
    /// Candidate has been hired.
    /// </summary>
    Hired = 6,

    /// <summary>
    /// Application has been rejected.
    /// </summary>
    Rejected = 7,

    /// <summary>
    /// Candidate has withdrawn their application.
    /// </summary>
    Withdrawn = 8
}
