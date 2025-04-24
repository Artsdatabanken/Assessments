using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Assessments.Shared.Constants;
using Assessments.Shared.Options;
using Microsoft.Extensions.Options;

namespace Assessments.Web.Infrastructure;

/// <summary>
/// Disables endpoint in production environment
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class NotReadyForProduction : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var environment = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();

        if (environment.IsProduction())
            context.Result = new NotFoundResult();
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}

/// <summary>
/// Midlertidig tilgangskontroll for naturtyper i testmiljøet
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
public class CookieRequiredAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var environment = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();

        if (!environment.IsStaging())
            await next();

        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApplicationOptions>>();

        var temporaryAccessCookie = context.HttpContext.Request.Cookies[NatureTypesConstants.TemporaryAccessCookieName];

        if (string.IsNullOrEmpty(temporaryAccessCookie)
            || !temporaryAccessCookie.Equals(options.Value.NatureTypes.TemporaryAccessKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }
}