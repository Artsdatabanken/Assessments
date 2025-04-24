using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Assessments.Web.Controllers;

[Route("Error")]
[ApiExplorerSettings(IgnoreApi = true)]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorController : Controller
{
    [Route("{code:int}")]
    public IActionResult Error(int code)
    {
        var codeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var defaultResult = code == 404 ? View("ErrorNotFound") : View(code);

        if (codeReExecuteFeature is null)
            return defaultResult;

        var apiPaths = new List<string> { "/api", "/odata" };

        if (apiPaths.Any(x => codeReExecuteFeature.OriginalPath.StartsWith(x)) && code is 404)
            return Problem(statusCode: (int)HttpStatusCode.NotFound);

        return defaultResult;
    }

    public IActionResult Error() => View();
}