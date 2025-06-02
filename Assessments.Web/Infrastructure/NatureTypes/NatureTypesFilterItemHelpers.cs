using Assessments.Shared.DTOs.NatureTypes.Enums;
using Assessments.Shared.Extensions;
using Assessments.Shared.Helpers;
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
                Name = x.GetDescription(),
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

    public static FilterAndMetaData Topics(List<NinCodeTopic> topics) => new()
    {
        FilterButtonName = "temafiltre",
        FilterButtonText = "Tema",
        Filters =
        [
            .. topics.GroupBy(x => x.Description).OrderBy(x => x.Key)
                .Select(x => new FilterItem
                {
                    Name = x.Key,
                    NameShort = x.Key
                })
        ]
    };

    public static FilterAndMetaData Regions(List<Region> regions) =>
        new()
        {
            FilterButtonName = "regionsfiltre",
            FilterButtonText = "Regioner og havområder",
            Filters =
            [
                .. regions
                    .Select(x => new FilterItem
                    {
                        Name = x.Name,
                        NameShort = x.Name
                    })
            ]
        };

    public static FilterAndMetaData CodeItems(List<CodeItem> codeItems)
    {
        // hardkodet liste med id'er som bestemmer hva som skal vises og rekkefølge
        int[] codeItemFilterIds = [13, 14, 15, 16, 3, 4, 5, 6, 7, 8, 9, 10, 11];

        return new FilterAndMetaData
        {
            FilterButtonName = "faktorfiltre",
            FilterButtonText = "Påvirkningsfaktorer",
            Filters =
            [
                .. codeItems.Where(x => codeItemFilterIds.Contains(x.Id))
                    .OrderBy(x => Array.IndexOf(codeItemFilterIds, x.Id))
                    .Select(x => new FilterItem
                    {
                        Name = x.Description,
                        NameShort = x.Id.ToString()
                    })
            ]
        };
    }

    public static int GetActiveSelectionCount(NatureTypesListViewModel model)
    {
        var count = 0;

        count += model.Area.Length;
        count += model.Category.Length;
        count += model.Region.Length;
        count += model.Topic.Length;
        count += model.CodeItem.Length;

        return count;
    }
}