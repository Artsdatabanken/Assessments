using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Models.NatureTypes.Enums;

public enum SortByEnum
{
    [Display(Name = "Naturtype")]
    PopularName,

    [Display(Name = "NiN-kode")]
    NinCode,
    
    [Display(Name = "Kategori")]
    Category
}