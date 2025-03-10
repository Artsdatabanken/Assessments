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
        var buildTime = System.IO.File.GetLastWriteTimeUtc(Assembly.GetExecutingAssembly().Location).ToLocalTime();
        
        return Ok($"Hello from {environment.EnvironmentName}, baseUrl: {options.Value.BaseUrl}, buildTime: {buildTime:F}");
    }
}