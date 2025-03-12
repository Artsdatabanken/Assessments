using Assessments.Shared.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Assessments.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TestController(IWebHostEnvironment environment, IOptions<ApplicationOptions> options) : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        var lastWriteTime = System.IO.File.GetLastWriteTimeUtc(Assembly.GetExecutingAssembly().Location);

        var now = DateTime.Now;
        var localOffset = now - now.ToUniversalTime();

        var buildTime = lastWriteTime + localOffset;

        return Ok($"Hello from {environment.EnvironmentName}, baseUrl: {options.Value.BaseUrl}, buildTime: {buildTime:F}");
    }
}