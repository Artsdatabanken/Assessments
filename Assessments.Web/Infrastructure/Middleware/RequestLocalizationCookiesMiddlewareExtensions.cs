namespace Assessments.Web.Infrastructure.Middleware
{
    public static class RequestLocalizationCookiesMiddlewareExtensions
    {
        public static void UseRequestLocalizationCookies(this IApplicationBuilder app) => app.UseMiddleware<RequestLocalizationCookiesMiddleware>();
    }
}