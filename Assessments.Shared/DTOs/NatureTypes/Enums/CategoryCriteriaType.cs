using System.ComponentModel.DataAnnotations;

namespace Assessments.Shared.DTOs.NatureTypes.Enums;

public enum CategoryCriteriaType
{
    [Display(Name = "Reduksjon i totalarealet")]
    A,

    [Display(Name = "Begrenset geografisk utbredelse")]
    B,

    [Display(Name = "Abiotisk forringelse")]
    C,

    [Display(Name = "Biotisk forringelse")]
    D,

    [Display(Name = "Kvantitativ analyse")]
    E
}