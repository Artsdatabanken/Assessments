using System.ComponentModel.DataAnnotations;

namespace Assessments.Shared.DTOs.NatureTypes.Enums;

public enum CategoryCriteriaType
{
    [Display(Name = "reduksjon i totalarealet")]
    A,

    [Display(Name = "begrenset geografisk utbredelse")]
    B,

    [Display(Name = "abiotisk forringelse")]
    C,

    [Display(Name = "biotisk forringelse")]
    D,

    [Display(Name = "kvantitativ analyse")]
    E
}