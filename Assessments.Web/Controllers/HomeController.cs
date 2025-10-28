using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Controllers;

public class HomeController : BaseController<HomeController>
{
    public IActionResult Index()
    {
        if (!Environment.IsDevelopment())
            return new RedirectResult("https://artsdatabanken.no/");

        return View();
    }

    // videresend gamle landingssider

    [Route("rodlisteforarter")]
    public IActionResult Species() => new RedirectResult("https://artsdatabanken.no/arter/rodlista-arter/om-norsk-rodliste-arter", true);

    [Route("fremmedartslista")]
    public IActionResult AlienSpecies() => new RedirectResult("https://artsdatabanken.no/arter/om-fremmedartslista", true);
}