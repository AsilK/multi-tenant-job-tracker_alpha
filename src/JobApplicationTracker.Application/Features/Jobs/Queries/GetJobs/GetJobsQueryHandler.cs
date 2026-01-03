using AutoMapper;
using AutoMapper.QueryableExtensions;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Jobs.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Features.Jobs.Queries.GetJobs;

/// <summary>
/// Handler for GetJobsQuery.
/// Returns a paginated list of jobs with optional filtering.
/// </summary>
public class GetJobsQueryHandler : IRequestHandler<GetJobsQuery, Result<PaginatedList<JobDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetJobsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<JobDto>>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Jobs.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(j => 
                j.Title.Contains(request.SearchTerm) || 
                j.Description.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            query = query.Where(j => j.Department == request.Department);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(j => j.IsActive == request.IsActive.Value);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderByDescending(j => j.PostedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<JobDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var result = new PaginatedList<JobDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        return Result<PaginatedList<JobDto>>.Success(result);
    }
}
