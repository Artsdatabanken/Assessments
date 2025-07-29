using Assessments.Shared.DTOs.Drupal;
using Assessments.Shared.DTOs.Drupal.Enums;
using Assessments.Shared.Helpers;
using Assessments.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Views.NatureTypes.Components.NatureTypeImages;

public class NatureTypeImagesViewComponent(IDrupalRepository drupalRepository) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string longCode)
    {
        var dtos = await drupalRepository.ImageModelsByLongCode(longCode);
        var models = new List<NatureTypeImageModel>();

        foreach (var model in dtos)
        {
            var imageCarouselModel = new NatureTypeImageModel
            {
                Url = model.Url,
                Link = model.Link,
                Text = model.Text,
                Authors = string.Join(" | ", await GetAuthors(model)),
                License = model.License.Split("/").Last().ToEnum(ImageLicenseEnum.Unknown)
            };

            models.Add(imageCarouselModel);
        }

        return View("NatureTypeImages", models);
    }

    private async Task<List<string>> GetAuthors(ImageModelDto model)
    {
        var authors = new List<string>();

        foreach (var author in model.Authors)
        {
            if (!int.TryParse(author.Split("/").Last(), out var nodeId))
                continue;

            var contentById = await drupalRepository.ContentById(nodeId);
            authors.Add(contentById.Title);
        }

        return authors;
    }
}