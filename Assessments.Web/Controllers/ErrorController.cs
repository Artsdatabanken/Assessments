using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Controllers;

[Route("Error")]
[ApiExplorerSettings(IgnoreApi = true)]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorController : Controller
{
    [Route("{code:int}")]
    public IActionResult Error(int code) => code == 404 ? View("ErrorNotFound") : View();

    public IActionResult Error() => View();
}