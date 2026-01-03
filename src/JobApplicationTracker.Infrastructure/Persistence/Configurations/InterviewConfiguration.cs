using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobApplicationTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Interview entity.
/// </summary>
public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.ToTable("Interviews");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Location)
            .HasMaxLength(200);

        builder.Property(i => i.MeetingLink)
            .HasMaxLength(500);

        builder.Property(i => i.Notes)
            .HasMaxLength(2000);

        builder.Property(i => i.Feedback)
            .HasMaxLength(5000);

        // Indexes for common queries
        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.ScheduledAt);
        builder.HasIndex(i => i.InterviewerId);
    }
}
