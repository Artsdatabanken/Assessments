using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using X.PagedList;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModel(IPagedList<Assessment> Assessments) : NatureTypesListViewModelParameters;

public record NatureTypesListViewModelParameters
{
    public string Name { get; init; }

    public List<Category> Category { get; init; } = [];
}
