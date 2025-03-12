using Assessments.Shared.DTOs.Drupal.Enums;
using Assessments.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Views.Shared.Components.Footer;

public class FooterViewComponent(IDrupalRepository drupalRepository) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var mainContent = await drupalRepository.ContentByType(ContentModelType.FooterMain);
        var someContent = await drupalRepository.ContentByType(ContentModelType.FooterSome);
        var footerLinks = await drupalRepository.ContentByType(ContentModelType.FooterLinks);
        
        var footerModel = new FooterModel
        {
            Body = mainContent?.Body,
            Some = someContent?.Body,
            Links = footerLinks?.Records?.SelectMany(x => x.References).ToList() ?? []
        };

        return View("Footer", footerModel);
    }
}