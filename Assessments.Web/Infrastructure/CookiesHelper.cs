namespace Assessments.Web.Infrastructure;

public static class CookiesHelper
{
    /// <summary>
    /// sjekk om bruker har godtatt cookies
    /// </summary>
    public static bool UserAcceptedCookies(HttpContext context)
    {
        var acceptedCookie = context.Request.Cookies["acceptedcookie"];
        
        var userAcceptedCookies = !string.IsNullOrEmpty(acceptedCookie) 
            && acceptedCookie.Equals("yes", StringComparison.OrdinalIgnoreCase);
        
        return userAcceptedCookies;
    }
}