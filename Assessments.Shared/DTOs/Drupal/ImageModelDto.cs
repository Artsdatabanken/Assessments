using Assessments.Shared.DTOs.Drupal.Enums;

namespace Assessments.Shared.DTOs.Drupal;

public record ImageModelDto
{
    public Uri Url { get; set; }

    public Uri Link { get; set; }

    public string Text { get; set; }

    public List<string> Authors { get; set; } = [];

    public ImageLicenseEnum License { get; set; }

    public string LongCode { get; set; }
}