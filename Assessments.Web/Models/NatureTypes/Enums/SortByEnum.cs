using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Models.NatureTypes.Enums;

public enum SortByEnum
{
    [Display(Name = "Navn")]
    Name,
    
    [Display(Name = "Kategori")]
    Category,
    
    [Display(Name = "Tema")]
    Committee
}