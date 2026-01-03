using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.Features.Applications.Common;

/// <summary>
/// Data transfer object for job application listing.
/// </summary>
public class ApplicationDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CandidateName { get; set; } = string.Empty;
    public ApplicationStatusType CurrentStatus { get; set; }
    public DateTime AppliedAt { get; set; }
}

/// <summary>
/// Data transfer object for job application details.
/// </summary>
public class ApplicationDetailDto : ApplicationDto
{
    public string CandidateEmail { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public List<ApplicationStatusDto> StatusHistory { get; set; } = new();
    public List<InterviewDto> Interviews { get; set; } = new();
}

/// <summary>
/// Data transfer object for application status history.
/// </summary>
public class ApplicationStatusDto
{
    public Guid Id { get; set; }
    public ApplicationStatusType Status { get; set; }
    public string? Notes { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedByUserName { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for interview.
/// </summary>
public class InterviewDto
{
    public Guid Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string? Notes { get; set; }
    public string? Feedback { get; set; }
    public bool IsCompleted { get; set; }
    public string InterviewerName { get; set; } = string.Empty;
}
