using Assessments.Shared.Options;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using Assessments.Shared.Constants;

namespace Assessments.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[EnableCors(nameof(CorsConstants.AllowAnyPolicy))]
public class TestController(IWebHostEnvironment environment, IOptions<ApplicationOptions> options) : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        var buildTime = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

        return Ok($"Hello from {environment.EnvironmentName}, baseUrl: {options.Value.BaseUrl}, buildTime: {buildTime:F}");
    }
}