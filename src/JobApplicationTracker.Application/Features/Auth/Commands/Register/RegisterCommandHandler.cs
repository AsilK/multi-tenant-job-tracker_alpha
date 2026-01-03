using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Auth.Common;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handler for the RegisterCommand.
/// Creates a new user account and returns authentication tokens.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ICurrentTenantService _tenantService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IJwtService jwtService,
        ICurrentTenantService tenantService,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _context = context;
        _jwtService = jwtService;
        _tenantService = tenantService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if tenant is set
        if (!_tenantService.TenantId.HasValue)
        {
            return Result<AuthResponse>.Failure("Tenant context is required.");
        }

        // Check if email already exists within the tenant
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.TenantId == _tenantService.TenantId.Value, 
                cancellationToken);

        if (existingUser != null)
        {
            throw new DuplicateEntityException("User", "email", request.Email);
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantService.TenantId.Value,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse
        {
            User = _mapper.Map<UserDto>(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        return Result<AuthResponse>.Success(response);
    }
}
