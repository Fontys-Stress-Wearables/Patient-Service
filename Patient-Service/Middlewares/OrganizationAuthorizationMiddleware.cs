using Microsoft.Identity.Web;
using Patient_Service.Exceptions;
using Patient_Service.Interfaces;

namespace Patient_Service.Middlewares;

public class OrganizationAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOrganizationService _organizationService;

    public OrganizationAuthorizationMiddleware(RequestDelegate next, IOrganizationService organizationService)
    {
        _next = next;
        _organizationService = organizationService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenant = context.User.GetTenantId();

        if (tenant == null)
        {
            throw new NotFoundException("tenant not found");
        }

        if (!_organizationService.Exists(tenant))
        {
            throw new NotFoundException("tenant not found");
        }

        await _next(context);
    }
}

public static class OrganizationAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseOrganizationAuthorization(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<OrganizationAuthorizationMiddleware>();
    }
}