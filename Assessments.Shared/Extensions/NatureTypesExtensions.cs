using Assessments.Shared.Constants;
using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.DTOs.NatureTypes.Enums;
using Assessments.Shared.Helpers;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Shared.Extensions;

public static class NatureTypesExtensions
{
    public static string GetDescription(this Category category) => category.ConvertTo<NatureTypeCategoryDto>().DisplayName();

    public static string GetColor(this Category category)
    {
        return category switch
        {
            Category.CO => "#262F31",
            Category.CR => "#D61900",
            Category.EN => "#F34F39",
            Category.VU => "#EB8107",
            Category.NT => "#E6C000",
            Category.DD => "#6C6C6C",
            Category.LC => "#61A360",
            Category.NA => "",
            Category.NE => "",
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }

    public static string GetDescription(this AssessmentRegion region) => region.ConvertTo<AssessmentRegionDto>().DisplayName();

    public static string GetCitation(this List<CommitteeUser> committeeUsers, Committee committee, bool includeDetails)
    {
        var users = committeeUsers
            .Where(x => x.CommitteeId == committee.Id)
            .OrderByDescending(x => x.Level).ThenBy(x => x.User.LastName)
            .Select(x => !string.IsNullOrEmpty(x.User.CitationName) ? x.User.CitationName : $"{x.User.LastName}, {x.User.FirstName[0]}.")
            .ToList();

        if (!includeDetails)
            return users.JoinAnd(", ", " og ");

        var citation = $"{users.JoinAnd(", ", " og ")} ({NatureTypesConstants.PublishedDate:d.M.yyy}). {committee.Name}. {NatureTypesConstants.CitationSummary}";

        return citation;
    }

    public static string GetDisplayName(this CriteriaCategory category)
    {
        return category switch
        {
            CriteriaCategory.CR => "kritisk truet",
            CriteriaCategory.EN => "sterkt truet",
            CriteriaCategory.VU => "sårbar",
            CriteriaCategory.NT => "nær truet",
            CriteriaCategory.LC => "uten risiko",
            CriteriaCategory.DD => "datamangel",
            CriteriaCategory.NE => "ikke vurdert",
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }

    public static string GetDescription(this CriteriaCategory category, CriteriaCategoryType type)
    {
        return type switch
        {
            CriteriaCategoryType.A => category switch
            {
                CriteriaCategory.CR => "≥ 80%",
                CriteriaCategory.EN => "≥ 50%",
                CriteriaCategory.VU => "≥ 30%",
                CriteriaCategory.NT => "≥ 20%",
                CriteriaCategory.LC => string.Empty,
                CriteriaCategory.DD => string.Empty,
                CriteriaCategory.NE => string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.B1 => category switch
            {
                CriteriaCategory.CR => "≤ 2000 km2",
                CriteriaCategory.EN => "≤ 20 000 km2",
                CriteriaCategory.VU => "≤ 50 000 km2",
                CriteriaCategory.NT => "≤ 55 000 km2",
                CriteriaCategory.LC => string.Empty,
                CriteriaCategory.DD => string.Empty,
                CriteriaCategory.NE => string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.B2 => category switch
            {
                CriteriaCategory.CR => "≤ 2",
                CriteriaCategory.EN => "≤ 20",
                CriteriaCategory.VU => "≤ 50",
                CriteriaCategory.NT => "≤ 55",
                CriteriaCategory.LC => string.Empty,
                CriteriaCategory.DD => string.Empty,
                CriteriaCategory.NE => string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.BSites => category switch
            {
                CriteriaCategory.CR => "ant lok 1",
                CriteriaCategory.EN => "ant lok ≤ 5",
                CriteriaCategory.VU => "ant lok ≤ 10",
                CriteriaCategory.NT => string.Empty,
                CriteriaCategory.LC => string.Empty,
                CriteriaCategory.DD => string.Empty,
                CriteriaCategory.NE => string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.C or CriteriaCategoryType.D => category switch
            {
                CriteriaCategory.CR => "≥ 80%",
                CriteriaCategory.EN => "≥ 50%",
                CriteriaCategory.VU => "≥ 30%",
                CriteriaCategory.NT => "≥ 20%",
                CriteriaCategory.LC => "< 20%",
                CriteriaCategory.DD => nameof(CriteriaCategory.DD),
                CriteriaCategory.NE => nameof(CriteriaCategory.NE),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static string GetDescription(this CriteriaCategoryThreatDefinedlocation criteriaCategoryChange)
    {
        return criteriaCategoryChange switch
        {
            CriteriaCategoryThreatDefinedlocation.None => string.Empty,
            CriteriaCategoryThreatDefinedlocation.VU => "< 5 lokaliteter",
            CriteriaCategoryThreatDefinedlocation.NT => "< 10 lokaliteter",
            _ => throw new ArgumentOutOfRangeException(nameof(criteriaCategoryChange), criteriaCategoryChange, null)
        };
    }

    public static List<CategoryCriteriaType> GetCategoryCriteriaTypes(string categoryCriteria)
    {
        if (string.IsNullOrEmpty(categoryCriteria))
            return [];

        // utslagsgivende kriterier fra "Endelig kategori og kriterium"

        var categoryCriterion = categoryCriteria[2..]; // ta bort kategori

        if (string.IsNullOrEmpty(categoryCriterion))
            return [];

        return [.. Array.ConvertAll(categoryCriterion.Split('+'), x => x.Trim()[..1]).Distinct().ToEnumerable<CategoryCriteriaType>()];
    }

    public static List<CodeItemDto> GetCodeItemModels(this List<AssessmentCodeItem> assessmentCodeItems)
    {
        var codeItemModels = new List<CodeItemDto>();

        codeItemModels.AddRange(assessmentCodeItems.OrderBy(x => x.CodeItemId).GroupBy(x => new { x.CodeItemId }).Select(group =>
            new CodeItemDto
            {
                CodeItemId = group.First().CodeItemId,
                CodeItemDescription = group.First().CodeItemDescription,
                TimeOfIncident = group.First(x => x.CodeItemParamLevel.CodeItemParamTypeId == 1).CodeItemParamLevel
                    .Description,
                InfluenceFactor = group.First(x => x.CodeItemParamLevel.CodeItemParamTypeId == 2).CodeItemParamLevel
                    .Description,
                Magnitude = group.First(x => x.CodeItemParamLevel.CodeItemParamTypeId == 3).CodeItemParamLevel
                    .Description,
            }));

        return codeItemModels;
    }
}