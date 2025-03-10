using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Assessments.Web.Views.Shared.Components.CulturePicker;

public class CulturePickerViewComponent : ViewComponent
{
    private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

    public CulturePickerViewComponent(IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _localizationOptions = localizationOptions;
    }

    public IViewComponentResult Invoke()
    {
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            
        var model = new CulturePickerModel
        {
            SupportedCultures = _localizationOptions.Value.SupportedUICultures?.ToList(),
            CurrentUICulture = cultureFeature.RequestCulture.UICulture
        };

        return View(model);
    }
}