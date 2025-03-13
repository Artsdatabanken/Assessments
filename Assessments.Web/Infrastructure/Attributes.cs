using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

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