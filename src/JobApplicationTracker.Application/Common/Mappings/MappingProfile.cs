using AutoMapper;
using JobApplicationTracker.Application.Features.Auth.Commands.Register;
using JobApplicationTracker.Application.Features.Auth.Common;
using JobApplicationTracker.Application.Features.Jobs.Common;
using JobApplicationTracker.Application.Features.Jobs.Commands.CreateJob;
using JobApplicationTracker.Application.Features.Applications.Common;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterCommand, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Job mappings
        CreateMap<Job, JobDto>();
        CreateMap<Job, JobDetailDto>();
        CreateMap<CreateJobCommand, Job>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PostedAt, opt => opt.Ignore());

        // Application mappings
        CreateMap<Domain.Entities.Application, ApplicationDto>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
            .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate.FullName))
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => 
                src.StatusHistory.OrderByDescending(s => s.ChangedAt).FirstOrDefault()!.Status));

        CreateMap<Domain.Entities.Application, ApplicationDetailDto>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
            .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Candidate.FullName))
            .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Candidate.Email));

        // ApplicationStatus mapping
        CreateMap<ApplicationStatus, ApplicationStatusDto>();

        // Interview mapping
        CreateMap<Interview, InterviewDto>()
            .ForMember(dest => dest.InterviewerName, opt => opt.MapFrom(src => src.Interviewer.FullName));
    }
}
