using System.ComponentModel.DataAnnotations;
using Assessments.Web.Models.NatureTypes.Enums;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using X.PagedList;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModel(IPagedList<Assessment> Assessments) : NatureTypesListViewModelParameters
{
    public List<Committee> Committees { get; set; }

    public List<Region> Regions { get; set; }
}

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