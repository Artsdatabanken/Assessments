using System.ComponentModel.DataAnnotations;

namespace Assessments.Shared.DTOs.NatureTypes.Enums;

public enum AssessmentRegionDto
{
    [Display(Name = "Fastlands-Norge med havområder")] 
    Norge = 0,

    [Display(Name = "Svalbard")]
    Svalbard = 1
}