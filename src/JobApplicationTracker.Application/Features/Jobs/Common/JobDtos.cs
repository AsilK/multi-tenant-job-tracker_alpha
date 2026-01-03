using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.Features.Jobs.Common;

/// <summary>
/// Data transfer object for job listing.
/// </summary>
public class JobDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public JobType Type { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool IsActive { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime? ClosingDate { get; set; }
}

/// <summary>
/// Data transfer object for job details including description.
/// </summary>
public class JobDetailDto : JobDto
{
    public string Description { get; set; } = string.Empty;
    public int ApplicationCount { get; set; }
}
