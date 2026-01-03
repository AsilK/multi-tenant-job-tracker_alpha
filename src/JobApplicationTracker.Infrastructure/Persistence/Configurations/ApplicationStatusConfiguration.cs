using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for ApplicationStatus entity.
/// </summary>
public class ApplicationStatusConfiguration : IEntityTypeConfiguration<ApplicationStatus>
{
    public void Configure(EntityTypeBuilder<ApplicationStatus> builder)
    {
        builder.ToTable("ApplicationStatuses");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        // Indexes for common queries
        builder.HasIndex(s => s.ApplicationId);
        builder.HasIndex(s => s.ChangedAt);
    }
}
