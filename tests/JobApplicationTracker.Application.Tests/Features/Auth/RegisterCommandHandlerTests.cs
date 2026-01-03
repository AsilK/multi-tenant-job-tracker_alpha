using AutoMapper;
using FluentAssertions;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Features.Auth.Commands.Register;
using JobApplicationTracker.Application.Features.Auth.Common;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace JobApplicationTracker.Application.Tests.Features.Auth;

/// <summary>
/// Unit tests for RegisterCommandHandler.
/// Tests user registration logic including success and failure scenarios.
/// </summary>
public class RegisterCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ICurrentTenantService> _tenantServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _jwtServiceMock = new Mock<IJwtService>();
        _tenantServiceMock = new Mock<ICurrentTenantService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _mapperMock = new Mock<IMapper>();

        _handler = new RegisterCommandHandler(
            _contextMock.Object,
            _jwtServiceMock.Object,
            _tenantServiceMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object);
    }

    /// <summary>
    /// SUCCESS SCENARIO: When valid registration data is provided,
    /// the handler should create a user and return auth tokens.
    /// 
    /// Why test this: Core happy path - ensures users can register.
    /// </summary>
    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessWithTokens()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new RegisterCommand
        {
            Email = "newuser@test.com",
            Password = "SecureP@ss123",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Candidate
        };

        // Setup mocks
        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);
        
        // Mock empty users DbSet (no existing user with this email)
        var emptyUsers = new List<User>().AsQueryable();
        var mockDbSet = CreateMockDbSet(emptyUsers);
        _contextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        _passwordHasherMock
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");

        _jwtServiceMock
            .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
            .Returns("test_access_token");
        
        _jwtServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("test_refresh_token");

        _mapperMock
            .Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(new UserDto 
            { 
                Email = command.Email, 
                FirstName = command.FirstName,
                LastName = command.LastName,
                Role = command.Role
            });

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("test_access_token");
        result.Data.RefreshToken.Should().Be("test_refresh_token");
        result.Data.User.Email.Should().Be(command.Email);

        // Verify user was added to context
        _contextMock.Verify(x => x.Users.Add(It.Is<User>(u => 
            u.Email == command.Email && 
            u.TenantId == tenantId)), Times.Once);
        
        // Verify save was called
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// FAILURE SCENARIO: When tenant context is not set,
    /// the handler should return a failure result.
    /// 
    /// Why test this: Multi-tenancy requirement - registration must happen within a tenant.
    /// </summary>
    [Fact]
    public async Task Handle_NoTenantContext_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "user@test.com",
            Password = "SecureP@ss123",
            FirstName = "John",
            LastName = "Doe"
        };

        // TenantId is null
        _tenantServiceMock.Setup(x => x.TenantId).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Tenant context is required");

        // Verify no database operations were performed
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// FAILURE SCENARIO: When email already exists within tenant,
    /// the handler should throw DuplicateEntityException.
    /// 
    /// Why test this: Prevents duplicate accounts, data integrity.
    /// </summary>
    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsDuplicateEntityException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var existingEmail = "existing@test.com";
        
        var command = new RegisterCommand
        {
            Email = existingEmail,
            Password = "SecureP@ss123",
            FirstName = "John",
            LastName = "Doe"
        };

        _tenantServiceMock.Setup(x => x.TenantId).Returns(tenantId);

        // Mock existing user in database
        var existingUsers = new List<User>
        {
            new User 
            { 
                Id = Guid.NewGuid(), 
                Email = existingEmail, 
                TenantId = tenantId,
                PasswordHash = "hash",
                FirstName = "Existing",
                LastName = "User"
            }
        }.AsQueryable();
        
        var mockDbSet = CreateMockDbSet(existingUsers);
        _contextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => 
            _handler.Handle(command, CancellationToken.None));
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

#region Async Query Helpers

/// <summary>
/// Helper class for async query provider in tests.
/// </summary>
internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        => new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(System.Linq.Expressions.Expression expression)
        => _inner.Execute(expression);

    public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default)
    {
        var resultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(name: nameof(IQueryProvider.Execute), genericParameterCount: 1, types: new[] { typeof(System.Linq.Expressions.Expression) })!
            .MakeGenericMethod(resultType)
            .Invoke(_inner, new[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(resultType)
            .Invoke(null, new[] { executionResult })!;
    }
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(System.Linq.Expressions.Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
        => ValueTask.FromResult(_inner.MoveNext());
}

#endregion
