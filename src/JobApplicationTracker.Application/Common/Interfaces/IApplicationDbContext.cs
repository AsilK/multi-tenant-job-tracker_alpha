using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Common.Interfaces;

/// <summary>
/// Interface for the application database context.
/// Defines all entity sets and save operations.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Tenants in the system.
    /// </summary>
    DbSet<Tenant> Tenants { get; }

    /// <summary>
    /// Users (Admin, HR, Candidate) in the system.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Job postings.
    /// </summary>
    DbSet<Job> Jobs { get; }

    /// <summary>
    /// Job applications from candidates.
    /// </summary>
    DbSet<Domain.Entities.Application> Applications { get; }

    /// <summary>
    /// Application status history.
    /// </summary>
    DbSet<ApplicationStatus> ApplicationStatuses { get; }

    /// <summary>
    /// Scheduled interviews.
    /// </summary>
    DbSet<Interview> Interviews { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
