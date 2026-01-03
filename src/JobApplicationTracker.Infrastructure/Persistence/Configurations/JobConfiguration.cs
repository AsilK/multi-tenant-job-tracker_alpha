using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Job entity.
/// </summary>
public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.Description)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(j => j.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.MinSalary)
            .HasPrecision(18, 2);

        builder.Property(j => j.MaxSalary)
            .HasPrecision(18, 2);

        // Configure relationships
        builder.HasMany(j => j.Applications)
            .WithOne(a => a.Job)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for common queries
        builder.HasIndex(j => j.TenantId);
        builder.HasIndex(j => j.IsActive);
        builder.HasIndex(j => j.PostedAt);
    }
}
