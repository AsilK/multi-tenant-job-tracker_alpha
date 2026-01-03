using JobApplicationTracker.Application.Features.Jobs.Commands.CreateJob;
using JobApplicationTracker.Application.Features.Jobs.Common;
using JobApplicationTracker.Application.Features.Jobs.Queries.GetJobById;
using JobApplicationTracker.Application.Features.Jobs.Queries.GetJobs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers;

/// <summary>
/// Controller for job posting operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a paginated list of jobs.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="department">Optional department filter.</param>
    /// <param name="isActive">Optional active status filter.</param>
    /// <returns>Paginated list of jobs.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedList<JobDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? department = null,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetJobsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Department = department,
            IsActive = isActive
        };

        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    /// <summary>
    /// Get a job by ID.
    /// </summary>
    /// <param name="id">Job ID.</param>
    /// <returns>Job details.</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJob(Guid id)
    {
        var query = new GetJobByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { Message = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new job posting.
    /// Requires Admin or HR role.
    /// </summary>
    /// <param name="command">Job creation details.</param>
    /// <returns>Created job.</returns>
    [HttpPost]
    [Authorize(Policy = "HROrAdmin")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { Message = result.Error });
        }

        return CreatedAtAction(nameof(GetJob), new { id = result.Data!.Id }, result.Data);
    }
}
