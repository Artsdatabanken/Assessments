using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Models.NatureTypes.Enums;

public enum SortByEnum
{
    [Display(Name = "NiN")]
    NinCode,

    [Display(Name = "Naturtype")]
    Name,
    
    [Display(Name = "Kategori")]
    Category
}