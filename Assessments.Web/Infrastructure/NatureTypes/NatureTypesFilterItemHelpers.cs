using Assessments.Shared.DTOs.NatureTypes.Enums;
using Assessments.Shared.Helpers;
using Assessments.Web.Models.NatureTypes;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using System.Collections.Generic;
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
            })]
    };

    public static FilterAndMetaData Categories => new()
    {
        FilterButtonName = "kategorifiltre",
        FilterButtonText = "Kategori",
        Filters = [.. Enum.GetValues<NatureTypeCategoryDto>()
            .Select(x => new FilterItem
            {
                NameShort = x.ToString(),
                Name = x.DisplayName(),
                Description = x.DisplayName()
            })]
    };

    public static FilterAndMetaData Committees(List<Committee> committees) => new()
    {
        FilterButtonName = "temafiltre",
        FilterButtonText = "Tema",
        Filters =
        [
            .. committees
                .Select(x => new FilterItem
                {
                    Name = x.Name,
                    NameShort = x.Name
                })
        ]
    };

    public static FilterAndMetaData Regions(List<Region> modelRegions) =>
        new()
        {
            FilterButtonName = "regionsfiltre",
            FilterButtonText = "Regioner og havområder",
            Filters =
            [
                .. modelRegions
                    .Select(x => new FilterItem
                    {
                        Name = x.Name,
                        NameShort = x.Name
                    })
            ]
        };

    public static int GetActiveSelectionCount(NatureTypesListViewModel model)
    {
        var count = 0;

        count += model.Area.Length;
        count += model.Category.Length;
        count += model.Region.Length;
        count += model.Committee.Length;

        return count;
    }
}