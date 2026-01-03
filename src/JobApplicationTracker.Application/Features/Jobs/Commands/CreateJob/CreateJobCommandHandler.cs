using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Jobs.Common;
using JobApplicationTracker.Domain.Entities;
using MediatR;

namespace JobApplicationTracker.Application.Features.Jobs.Commands.CreateJob;

/// <summary>
/// Handler for CreateJobCommand.
/// Creates a new job posting within the current tenant.
/// </summary>
public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<JobDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _tenantService;
    private readonly IMapper _mapper;

    public CreateJobCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService tenantService,
        IMapper mapper)
    {
        _context = context;
        _tenantService = tenantService;
        _mapper = mapper;
    }

    public async Task<Result<JobDto>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantService.TenantId.HasValue)
        {
            return Result<JobDto>.Failure("Tenant context is required.");
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantService.TenantId.Value,
            Title = request.Title,
            Description = request.Description,
            Department = request.Department,
            Location = request.Location,
            Type = request.Type,
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary,
            ClosingDate = request.ClosingDate,
            IsActive = true,
            PostedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<JobDto>.Success(_mapper.Map<JobDto>(job));
    }
}
