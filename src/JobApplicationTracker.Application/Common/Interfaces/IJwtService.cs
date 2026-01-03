using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Common.Interfaces;

/// <summary>
/// Interface for JWT token generation and validation.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates an access token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate a token for.</param>
    /// <returns>JWT access token string.</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    /// <returns>Random refresh token string.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a token and extracts the user ID.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>User ID if valid, null otherwise.</returns>
    Guid? ValidateToken(string token);
}
