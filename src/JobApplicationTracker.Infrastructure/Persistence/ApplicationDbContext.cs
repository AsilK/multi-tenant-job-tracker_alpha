using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Domain.Common;
using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobApplicationTracker.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for the application.
/// Implements multi-tenancy through global query filters.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentTenantService _currentTenantService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantService currentTenantService) : base(options)
    {
        _currentTenantService = currentTenantService;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();
    public DbSet<ApplicationStatus> ApplicationStatuses => Set<ApplicationStatus>();
    public DbSet<Interview> Interviews => Set<Interview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filters for multi-tenancy
        ConfigureMultiTenancyFilters(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures global query filters for tenant data isolation.
    /// </summary>
    private void ConfigureMultiTenancyFilters(ModelBuilder modelBuilder)
    {
        // Apply tenant filter to all entities implementing ITenantEntity
        modelBuilder.Entity<User>()
            .HasQueryFilter(e => _currentTenantService.TenantId == null || e.TenantId == _currentTenantService.TenantId);
        
        modelBuilder.Entity<Job>()
            .HasQueryFilter(e => _currentTenantService.TenantId == null || e.TenantId == _currentTenantService.TenantId);
        
        modelBuilder.Entity<Domain.Entities.Application>()
            .HasQueryFilter(e => _currentTenantService.TenantId == null || e.TenantId == _currentTenantService.TenantId);
        
        modelBuilder.Entity<Interview>()
            .HasQueryFilter(e => _currentTenantService.TenantId == null || e.TenantId == _currentTenantService.TenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields for new and modified entities
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        // Set TenantId for new tenant entities
        if (_currentTenantService.TenantId.HasValue)
        {
            foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = _currentTenantService.TenantId.Value;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
