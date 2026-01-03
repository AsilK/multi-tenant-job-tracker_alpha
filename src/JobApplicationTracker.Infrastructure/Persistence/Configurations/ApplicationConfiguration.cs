using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Application entity.
/// </summary>
public class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.Application>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Application> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ResumeUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.CoverLetter)
            .HasMaxLength(5000);

        // Configure relationships
        builder.HasMany(a => a.StatusHistory)
            .WithOne(s => s.Application)
            .HasForeignKey(s => s.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Interviews)
            .WithOne(i => i.Application)
            .HasForeignKey(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate applications (same candidate for same job)
        builder.HasIndex(a => new { a.JobId, a.CandidateId })
            .IsUnique();

        // Indexes for common queries
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.AppliedAt);

        // Ignore computed property
        builder.Ignore(a => a.CurrentStatus);
    }
}
