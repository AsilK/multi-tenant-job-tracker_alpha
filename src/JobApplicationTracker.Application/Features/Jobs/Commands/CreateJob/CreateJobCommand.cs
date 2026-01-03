using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Jobs.Common;
using JobApplicationTracker.Domain.Enums;
using MediatR;

namespace JobApplicationTracker.Application.Features.Jobs.Commands.CreateJob;

/// <summary>
/// Command to create a new job posting.
/// </summary>
public class CreateJobCommand : IRequest<Result<JobDto>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public JobType Type { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public DateTime? ClosingDate { get; set; }
}
