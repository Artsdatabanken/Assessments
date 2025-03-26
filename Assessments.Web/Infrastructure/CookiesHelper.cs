using System.Text.Json.Nodes;

namespace Assessments.Web.Infrastructure;

public static class CookiesHelper
{
    /// <summary>
    /// sjekk om bruker har godtatt cookies
    /// </summary>
    public static bool UserAcceptedCookies(HttpContext context)
    {
        var cookieInformationConsent = context.Request.Cookies["CookieInformationConsent"];

        if (cookieInformationConsent == null)
            return false;

        var node = JsonNode.Parse(cookieInformationConsent)!;

        var consentsApprovedNode = node["consents_approved"];

        if (consentsApprovedNode == null)
            return false;

        var consentsApproved = consentsApprovedNode.AsArray().GetValues<string>();

        return consentsApproved.Any(x => x == "cookie_cat_necessary");
    }
}