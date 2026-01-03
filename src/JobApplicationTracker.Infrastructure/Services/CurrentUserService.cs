using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace JobApplicationTracker.Infrastructure.Services;

/// <summary>
/// Service for accessing the current authenticated user's information.
/// Extracts claims from the HTTP context.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user's unique identifier from claims.
    /// </summary>
    public Guid? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Gets the current user's email address from claims.
    /// </summary>
    public string? Email => _httpContextAccessor.HttpContext?.User
        .FindFirstValue(ClaimTypes.Email);

    /// <summary>
    /// Gets the current user's role from claims.
    /// </summary>
    public UserRole? Role
    {
        get
        {
            var role = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(role, out var r) ? r : null;
        }
    }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User
        .Identity?.IsAuthenticated ?? false;
}
