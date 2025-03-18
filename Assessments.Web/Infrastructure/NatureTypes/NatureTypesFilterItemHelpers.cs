using Assessments.Web.Models.NatureTypes;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using static Assessments.Web.Infrastructure.FilterHelpers;

namespace Assessments.Web.Infrastructure.NatureTypes;

public static class NatureTypesFilterHelpers
{
    public static FilterAndMetaData Areas => new()
    {
        FilterButtonName = "områdefiltre",
        FilterButtonText = "Vurderingsområde",
        Filters = [.. Enum.GetValues<AssessmentRegion>()
            .Select(x => new FilterItem
            {
                Name = x.ToString(),
                NameShort = x.ToString()
            })],
    };

    public static FilterAndMetaData Regions(List<Region> modelRegions) =>
        new()
        {
            FilterButtonName = "områdefiltre",
            FilterButtonText = "Vurderingsområde",
            Filters =
            [
                .. modelRegions
                    .Select(x => new FilterItem
                    {
                        Name = x.Name,
                        NameShort = x.Name
                    })
            ],
        };

    public static int GetActiveSelectionCount(NatureTypesListViewModel model)
    {
        var count = 0;

        count += model.Area.Length;
        count += model.Region.Length;

        return count;
    }
}