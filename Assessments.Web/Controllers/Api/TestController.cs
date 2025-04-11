using Assessments.Shared.Options;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Assessments.Shared.Constants;
using LazyCache;

namespace Assessments.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[EnableCors(nameof(CorsConstants.AllowAny))]
public class TestController(IWebHostEnvironment environment, IOptions<ApplicationOptions> options, IAppCache cache) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        var buildNumber = "Unknown";

        buildNumber = await cache.GetOrAddAsync($"{nameof(TestController)}-{nameof(buildNumber)}", async () =>
        {
            var buildNumberPath = Path.Combine(environment.WebRootPath, "BuildNumber.txt");

            if (!System.IO.File.Exists(buildNumberPath))
                return buildNumber;

            using var file = new StreamReader(buildNumberPath);
            buildNumber = await file.ReadLineAsync();

            return buildNumber;
        });

        return Ok($"Hello from {environment.EnvironmentName}, baseUrl: {options.Value.BaseUrl}, buildNumber: {buildNumber}");
    }
}