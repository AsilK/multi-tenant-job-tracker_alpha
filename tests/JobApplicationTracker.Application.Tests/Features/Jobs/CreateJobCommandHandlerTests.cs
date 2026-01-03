using AutoMapper;
using FluentAssertions;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Jobs.Commands.CreateJob;
using JobApplicationTracker.Application.Features.Jobs.Common;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JobApplicationTracker.Application.Tests.Features.Jobs;

/// <summary>
/// Unit tests for CreateJobCommandHandler.
/// Tests job creation logic including authorization and validation scenarios.
/// </summary>
public class CreateJobCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentTenantService> _tenantServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateJobCommandHandler _handler;

    public CreateJobCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _tenantServiceMock = new Mock<ICurrentTenantService>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateJobCommandHandler(
            _contextMock.Object,
            _tenantServiceMock.Object,
            _mapperMock.Object);
    }

    /// <summary>
    /// SUCCESS SCENARIO: Valid job data with tenant context creates a job.
    /// 
    /// Why test this: Core functionality - HR must be able to post jobs.
    /// </summary>
    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessWithJobDto()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new CreateJobCommand
        {
            Title = "Senior Developer",
            Description = "Looking for experienced .NET developer",
            Department = "Engineering",
            Location = "Remote",
            Type = JobType.FullTime,
            MinSalary = 80000,
            MaxSalary = 120000
        };

        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);

        var mockDbSet = new Mock<DbSet<Job>>();
        _contextMock.Setup(x => x.Jobs).Returns(mockDbSet.Object);
        
        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<JobDto>(It.IsAny<Job>()))
            .Returns(new JobDto 
            { 
                Title = command.Title,
                Department = command.Department,
                Type = command.Type
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);

        // Verify job was created with correct tenant
        mockDbSet.Verify(x => x.Add(It.Is<Job>(j => 
            j.Title == command.Title && 
            j.TenantId == tenantId &&
            j.IsActive == true)), Times.Once);
    }

    /// <summary>
    /// FAILURE SCENARIO: No tenant context should return failure.
    /// 
    /// Why test this: Jobs must belong to a tenant for data isolation.
    /// </summary>
    [Fact]
    public async Task Handle_NoTenantContext_ReturnsFailure()
    {
        // Arrange
        var command = new CreateJobCommand
        {
            Title = "Developer",
            Description = "Job description",
            Department = "IT",
            Location = "Office"
        };

        // TenantId is null
        _tenantServiceMock.Setup(x => x.TenantId).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Tenant context is required");
    }

    /// <summary>
    /// VALIDATION SCENARIO: Job is created with IsActive = true by default.
    /// 
    /// Why test this: Business rule - new jobs should be immediately visible.
    /// </summary>
    [Fact]
    public async Task Handle_NewJob_SetsIsActiveToTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new CreateJobCommand
        {
            Title = "Tester",
            Description = "QA position",
            Department = "QA",
            Location = "Hybrid",
            Type = JobType.Contract
        };

        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);

        Job? capturedJob = null;
        var mockDbSet = new Mock<DbSet<Job>>();
        mockDbSet.Setup(x => x.Add(It.IsAny<Job>()))
            .Callback<Job>(j => capturedJob = j);
        
        _contextMock.Setup(x => x.Jobs).Returns(mockDbSet.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapperMock.Setup(x => x.Map<JobDto>(It.IsAny<Job>())).Returns(new JobDto());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedJob.Should().NotBeNull();
        capturedJob!.IsActive.Should().BeTrue();
        capturedJob.PostedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
