using JobApplicationTracker.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.API.Middleware;

/// <summary>
/// Middleware for resolving the current tenant from the request.
/// Extracts tenant information from header, subdomain, or JWT claims.
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService, IServiceProvider serviceProvider)
    {
        Guid? tenantId = null;

        // Try to get tenant ID from header
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader))
        {
            if (Guid.TryParse(tenantHeader.FirstOrDefault(), out var id))
            {
                tenantId = id;
            }
        }

        // If not found, try to get from JWT claims
        if (!tenantId.HasValue && context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantClaim, out var id))
            {
                tenantId = id;
            }
        }

        // Validate tenant exists
        if (tenantId.HasValue)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApplicationDbContext>();
            
            var tenantExists = await dbContext.Tenants
                .IgnoreQueryFilters()
                .AnyAsync(t => t.Id == tenantId.Value && t.IsActive);

            if (tenantExists)
            {
                tenantService.SetTenant(tenantId.Value);
                _logger.LogDebug("Tenant context set to {TenantId}", tenantId.Value);
            }
            else
            {
                _logger.LogWarning("Invalid tenant ID: {TenantId}", tenantId.Value);
                tenantId = null;
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension methods for registering TenantResolutionMiddleware.
/// </summary>
public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}
