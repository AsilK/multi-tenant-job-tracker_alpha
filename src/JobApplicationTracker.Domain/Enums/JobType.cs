namespace JobApplicationTracker.Domain.Enums;

/// <summary>
/// Types of job employment.
/// </summary>
public enum JobType
{
    /// <summary>
    /// Full-time employment (typically 40 hours/week).
    /// </summary>
    FullTime = 1,

    /// <summary>
    /// Part-time employment (less than full-time hours).
    /// </summary>
    PartTime = 2,

    /// <summary>
    /// Contract-based employment with defined duration.
    /// </summary>
    Contract = 3,

    /// <summary>
    /// Fully remote position.
    /// </summary>
    Remote = 4,

    /// <summary>
    /// Hybrid position (mix of remote and on-site).
    /// </summary>
    Hybrid = 5
}
