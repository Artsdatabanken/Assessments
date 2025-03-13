using Assessments.Shared.DTOs.Drupal;

namespace Assessments.Web.Views.Shared.Components.Footer;

public class FooterModel
{
    public string Body;

    public string Some;

    public IEnumerable<ReferenceResponseDto> Links { get; set; } = [];
}