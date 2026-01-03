using JobApplicationTracker.Domain.Common;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Domain.Entities;

/// <summary>
/// Represents a user in the system (Admin, HR, or Candidate).
/// </summary>
public class User : BaseEntity, IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// The tenant this user belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// User's email address. Must be unique within a tenant.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt hashed password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number (optional).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's role determining their permissions.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Indicates whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Refresh token for JWT authentication.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Expiration date for the refresh token.
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation properties
    /// <summary>
    /// The tenant this user belongs to.
    /// </summary>
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Applications submitted by this user (if Candidate).
    /// </summary>
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    /// <summary>
    /// Interviews conducted by this user (if HR/Admin).
    /// </summary>
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}
