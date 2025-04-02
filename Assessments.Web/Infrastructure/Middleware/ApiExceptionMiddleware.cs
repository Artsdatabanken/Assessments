using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Assessments.Web.Infrastructure.Middleware;

public static class ApiExceptionMiddleware
{
    public static void HandleApiException(this WebApplication app)
    {
        var apiPaths = new List<string> { "/api", "/odata" };

        app.UseWhen(context => apiPaths.Any(s => context.Request.Path.StartsWithSegments(s)), builder =>
        {
            builder.UseExceptionHandler(apiError =>
            {
                apiError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                    
                    ProblemDetailsContext detailsContext = new()
                    {
                        HttpContext = context,
                        Exception = exceptionHandler.Error,
                        ProblemDetails =
                        {
                            Title = "An error occurred",
                            Type = exceptionHandler.Error.GetType().Name
                        }
                    };

                    if (app.Environment.IsDevelopment())
                        detailsContext.ProblemDetails.Detail = exceptionHandler.Error.Message;

                    var problemDetails = context.RequestServices.GetService<IProblemDetailsService>();

                    await problemDetails.TryWriteAsync(detailsContext);
                });
            });
        });
    }
}