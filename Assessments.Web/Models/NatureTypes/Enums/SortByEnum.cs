using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Models.NatureTypes.Enums;

public enum SortByEnum
{
    [Display(Name = "Naturtype")]
    Name,
    
    [Display(Name = "Kategori")]
    Category
}