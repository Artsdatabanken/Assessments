using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Assessments.Web.Infrastructure.Middleware;

// husker valgt språk ved navigering til ulike sider med cookies
public class RequestLocalizationCookiesMiddleware(IOptions<RequestLocalizationOptions> requestLocalizationOptions) : IMiddleware
{
    private CookieRequestCultureProvider Provider { get; } = requestLocalizationOptions.Value.RequestCultureProviders.Where(x => x is CookieRequestCultureProvider).Cast<CookieRequestCultureProvider>().FirstOrDefault();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (Provider != null)
        {
            var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();

            if (requestCultureFeature != null)
            {
                    context.Response.Cookies.Append(Provider.CookieName, CookieRequestCultureProvider.MakeCookieValue(requestCultureFeature.RequestCulture));
            }
        }

        await next(context);
    }
}