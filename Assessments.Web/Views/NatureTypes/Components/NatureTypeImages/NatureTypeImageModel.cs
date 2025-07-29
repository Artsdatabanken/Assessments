using Assessments.Shared.DTOs.Drupal.Enums;

namespace Assessments.Web.Views.NatureTypes.Components.NatureTypeImages;

public class NatureTypeImageModel
{
    public Uri Url { get; init; }

    public Uri Link { get; init; }

    public string Authors { get; init; }

    public ImageLicenseEnum License { get; init; }

    public string Text { get; init; }
}