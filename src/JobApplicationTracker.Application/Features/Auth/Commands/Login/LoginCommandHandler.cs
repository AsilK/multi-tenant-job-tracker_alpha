using AutoMapper;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Features.Auth.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler for the LoginCommand.
/// Authenticates user credentials and returns tokens.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ICurrentTenantService _tenantService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
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

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Check if tenant is set
        if (!_tenantService.TenantId.HasValue)
        {
            return Result<AuthResponse>.Failure("Tenant context is required.");
        }

        // Find user by email within tenant
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.TenantId == _tenantService.TenantId.Value, 
                cancellationToken);

        if (user == null)
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result<AuthResponse>.Failure("User account is deactivated.");
        }

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;

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
