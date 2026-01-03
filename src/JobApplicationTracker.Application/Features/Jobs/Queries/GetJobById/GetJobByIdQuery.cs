using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Jobs.Common;
using MediatR;

namespace JobApplicationTracker.Application.Features.Jobs.Queries.GetJobById;

/// <summary>
/// Query to get a job by its ID.
/// </summary>
public class GetJobByIdQuery : IRequest<Result<JobDetailDto>>
{
    public Guid Id { get; set; }
}
