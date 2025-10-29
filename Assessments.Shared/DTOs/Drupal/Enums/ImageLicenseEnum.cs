using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Assessments.Shared.DTOs.Drupal.Enums;
public enum ImageLicenseEnum
{
    Unknown,

    [Display(Name = "CC BY 4.0"), Description("https://creativecommons.org/licenses/by/4.0/deed.no")]
    T72,

    [Display(Name = "CC BY-SA 4.0"), Description("https://creativecommons.org/licenses/by-sa/4.0/deed.no")]
    T73
}
