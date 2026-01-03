using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Jobs.Common;
using JobApplicationTracker.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Features.Jobs.Queries.GetJobById;

/// <summary>
/// Handler for GetJobByIdQuery.
/// Returns job details including application count.
/// </summary>
public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, Result<JobDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetJobByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<JobDetailDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == request.Id, cancellationToken);

        if (job == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Job), request.Id);
        }

        var dto = _mapper.Map<JobDetailDto>(job);
        dto.ApplicationCount = job.Applications.Count;

        return Result<JobDetailDto>.Success(dto);
    }
}
