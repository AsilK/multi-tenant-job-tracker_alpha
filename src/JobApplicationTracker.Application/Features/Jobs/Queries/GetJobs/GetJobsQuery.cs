using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Jobs.Common;
using MediatR;

namespace JobApplicationTracker.Application.Features.Jobs.Queries.GetJobs;

/// <summary>
/// Query to get a paginated list of jobs.
/// </summary>
public class GetJobsQuery : IRequest<Result<PaginatedList<JobDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Department { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Represents a paginated list of items.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
