using Assessments.Shared.DTOs.Drupal.Enums;
using Assessments.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Views.Shared.Components.TopBar;

public class TopBarViewComponent(IDrupalRepository drupalRepository) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var content = await drupalRepository.ContentByType(ContentModelType.HeaderMenu);
        
        var model = new TopBarModel();
        
        if (content != null)
            model.Records = content.Records?.ToList() ?? [];

        return View("TopBar", model);
    }
}