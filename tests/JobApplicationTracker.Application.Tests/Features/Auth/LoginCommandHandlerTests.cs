using AutoMapper;
using FluentAssertions;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Features.Auth.Commands.Login;
using JobApplicationTracker.Application.Features.Auth.Common;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JobApplicationTracker.Application.Tests.Features.Auth;

/// <summary>
/// Unit tests for LoginCommandHandler.
/// Tests user authentication logic including success and failure scenarios.
/// </summary>
public class LoginCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ICurrentTenantService> _tenantServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _jwtServiceMock = new Mock<IJwtService>();
        _tenantServiceMock = new Mock<ICurrentTenantService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _mapperMock = new Mock<IMapper>();

        _handler = new LoginCommandHandler(
            _contextMock.Object,
            _jwtServiceMock.Object,
            _tenantServiceMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object);
    }

    /// <summary>
    /// SUCCESS SCENARIO: Valid credentials should return auth tokens.
    /// 
    /// Why test this: Core authentication flow - users must be able to login.
    /// </summary>
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithTokens()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "user@test.com";
        var password = "SecureP@ss123";

        var command = new LoginCommand
        {
            Email = email,
            Password = password
        };

        var existingUser = new User
        {
            Id = userId,
            TenantId = tenantId,
            Email = email,
            PasswordHash = "hashed_password",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Candidate,
            IsActive = true
        };

        // Setup mocks
        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);

        var users = new List<User> { existingUser }.AsQueryable();
        var mockDbSet = CreateMockDbSet(users);
        _contextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(password, existingUser.PasswordHash))
            .Returns(true);  // Password matches

        _jwtServiceMock
            .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
            .Returns("new_access_token");
        
        _jwtServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new_refresh_token");

        _mapperMock
            .Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(new UserDto { Email = email, Role = UserRole.Candidate });

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("new_access_token");
        result.Data.RefreshToken.Should().Be("new_refresh_token");

        // Verify refresh token was updated
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// FAILURE SCENARIO: Wrong password should return failure.
    /// 
    /// Why test this: Security - invalid credentials must be rejected.
    /// </summary>
    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new LoginCommand
        {
            Email = "user@test.com",
            Password = "WrongPassword"
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = command.Email,
            PasswordHash = "correct_hash",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);

        var users = new List<User> { existingUser }.AsQueryable();
        var mockDbSet = CreateMockDbSet(users);
        _contextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        // Password verification fails
        _passwordHasherMock
            .Setup(x => x.VerifyPassword(command.Password, existingUser.PasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email or password");

        // Verify no tokens were generated
        _jwtServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<User>()), Times.Never);
    }

    /// <summary>
    /// FAILURE SCENARIO: Non-existent user should return failure.
    /// 
    /// Why test this: Security - prevent user enumeration attacks.
    /// </summary>
    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new LoginCommand
        {
            Email = "nonexistent@test.com",
            Password = "AnyPassword"
        };

        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);

        // Empty users - no user found
        var emptyUsers = new List<User>().AsQueryable();
        var mockDbSet = CreateMockDbSet(emptyUsers);
        _contextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email or password");
        
        // Same message as wrong password (security best practice)
    }

    /// <summary>
    /// FAILURE SCENARIO: Deactivated user should not be able to login.
    /// 
    /// Why test this: Business rule - disabled accounts cannot authenticate.
    /// </summary>
    [Fact]
    public async Task Handle_DeactivatedUser_ReturnsFailure()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new LoginCommand
        {
            Email = "deactivated@test.com",
            Password = "CorrectPassword"
        };

        var deactivatedUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = command.Email,
            PasswordHash = "hash",
            FirstName = "Deactivated",
            LastName = "User",
            IsActive = false  // Account disabled
        };

        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);

        var users = new List<User> { deactivatedUser }.AsQueryable();
        var mockDbSet = CreateMockDbSet(users);
        _contextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(command.Password, deactivatedUser.PasswordHash))
            .Returns(true);  // Password is correct

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deactivated");
    }

    /// <summary>
    /// Helper method to create a mock DbSet for async queries.
    /// </summary>
    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        
        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(data.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        
        return mockSet;
    }
}
