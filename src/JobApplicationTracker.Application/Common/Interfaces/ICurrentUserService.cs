using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.Common.Interfaces;

/// <summary>
/// Interface for accessing the current authenticated user's information.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's unique identifier.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user's email address.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's role.
    /// </summary>
    UserRole? Role { get; }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
