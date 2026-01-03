using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Auth.Common;
using MediatR;

namespace JobApplicationTracker.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user.
/// </summary>
public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
