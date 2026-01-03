namespace JobApplicationTracker.Application.Common.Interfaces;

/// <summary>
/// Interface for password hashing operations.
/// Abstracts password hashing from the Application layer.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plaintext password.
    /// </summary>
    /// <param name="password">The plaintext password.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plaintext password against a hash.
    /// </summary>
    /// <param name="password">The plaintext password.</param>
    /// <param name="hashedPassword">The stored hashed password.</param>
    /// <returns>True if the password matches the hash.</returns>
    bool VerifyPassword(string password, string hashedPassword);
}
