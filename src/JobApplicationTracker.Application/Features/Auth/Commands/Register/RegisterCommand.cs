using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Auth.Common;
using JobApplicationTracker.Domain.Enums;
using MediatR;

namespace JobApplicationTracker.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command to register a new user.
/// </summary>
public class RegisterCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Candidate;
}
