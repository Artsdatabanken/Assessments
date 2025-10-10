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
}