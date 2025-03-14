using System.ComponentModel.DataAnnotations;
using Assessments.Web.Models.NatureTypes.Enums;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModelParameters
{
    public string Name { get; init; }

    public List<Category> Category { get; init; } = [];
    
    public List<string> Committee { get; set; } = [];

    public List<int> Region { get; set; } = [];

    [Display(Name = "Sorter på")]
    public SortByEnum SortBy { get; set; } = SortByEnum.Name;

    [Display(Name = "Område")]
    public AssessmentRegion? Area { get; set; }
}