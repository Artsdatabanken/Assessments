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

    public static FilterAndMetaData Criteria => new()
    {
        FilterButtonName = "kriteriefiltre",
        FilterButtonText = "Utslagsgivende kriterier",
        Filters = [.. Enum.GetValues<CategoryCriteriaType>()
            .Select(x => new FilterItem
            {
                NameShort = x.ToString(),
                Name = $"{x} - {x.DisplayName()}",
                Description = x.DisplayName()
            })]
    };

    public static FilterAndMetaData CodeItems(List<CodeItem> codeItems)
    {
        return new FilterAndMetaData
        {
            FilterButtonName = "faktorfiltre",
            FilterButtonText = "Påvirkningsfaktorer",
            Filters =
            [
                .. codeItems
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
        count += model.Criteria.Length;
        count += model.CodeItem.Length;

        return count;
    }
}