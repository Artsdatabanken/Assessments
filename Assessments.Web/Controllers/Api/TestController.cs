using Assessments.Shared.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Assessments.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TestController(IWebHostEnvironment environment, IOptions<ApplicationOptions> options) : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        var file = Path.Combine(environment.ContentRootPath, "appsettings.json");
        var buildTime = System.IO.File.GetLastWriteTime(file);

        return Ok($"Hello from {environment.EnvironmentName}, baseUrl: {options.Value.BaseUrl}, buildTime: {buildTime:F}");
    }
}