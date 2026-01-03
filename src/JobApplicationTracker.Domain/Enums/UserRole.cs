namespace JobApplicationTracker.Domain.Enums;

/// <summary>
/// User roles for authorization.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrator with full system access within tenant.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Human Resources personnel who can manage jobs and applications.
    /// </summary>
    HR = 2,

    /// <summary>
    /// Job applicant who can apply for jobs and view own applications.
    /// </summary>
    Candidate = 3
}
