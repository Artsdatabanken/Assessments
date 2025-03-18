using System.ComponentModel.DataAnnotations;
using Assessments.Web.Models.NatureTypes.Enums;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModelParameters
{
    public string Name { get; init; }

    public string[] Area { get; set; } = [];

    public List<string> Category { get; init; } = [];
    
    public string[] Committee { get; set; } = [];

    public string[] Region { get; set; } = [];

    [Display(Name = "Sorter på")]
    public SortByEnum SortBy { get; set; } = SortByEnum.Name;
}