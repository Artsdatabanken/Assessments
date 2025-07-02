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

    public static string GetDescription(this AssessmentRegion region) => region.ConvertTo<AssessmentRegionDto>().DisplayName();

    public static string GetCitation(this List<CommitteeUser> committeeUsers, Committee committee)
    {
        var users = committeeUsers
            .Where(x => x.CommitteeId == committee.Id)
            .OrderByDescending(x => x.Level).ThenBy(x => x.User.LastName)
            .Select(x => !string.IsNullOrEmpty(x.User.CitationName) ? x.User.CitationName : $"{x.User.LastName}, {x.User.FirstName[0]}.")
            .ToList();

        var citation = $"{users.JoinAnd(", ", " og ")} (alfabetisk) (2025). {committee.Name}. {NatureTypesConstants.CitationSummary}";

        return citation;
    }

    public static string GetDescription(this CriteriaCategory category, CriteriaCategoryType type)
    {
        return type switch
        {
            CriteriaCategoryType.A => category switch
            {
                CriteriaCategory.CR => "≥ 80% CR",
                CriteriaCategory.EN => "≥ 50% EN",
                CriteriaCategory.VU => "≥ 30% VU",
                CriteriaCategory.NT => "≥ 20% NT",
                CriteriaCategory.LC => nameof(CriteriaCategory.LC),
                CriteriaCategory.DD => nameof(CriteriaCategory.DD),
                CriteriaCategory.NE => nameof(CriteriaCategory.NE),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.B1 => category switch
            {
                CriteriaCategory.CR => "≤ 2000 km2 CR",
                CriteriaCategory.EN => "≤ 20 000 km2 EN",
                CriteriaCategory.VU => "≤ 50 000 km2 VU",
                CriteriaCategory.NT => "≤ 55 000 km2 NT",
                CriteriaCategory.LC => nameof(CriteriaCategory.LC),
                CriteriaCategory.DD => nameof(CriteriaCategory.DD),
                CriteriaCategory.NE => nameof(CriteriaCategory.NE),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.B2 => category switch
            {
                CriteriaCategory.CR => "≤ 2 CR",
                CriteriaCategory.EN => "≤ 20 EN",
                CriteriaCategory.VU => "≤ 50 VU",
                CriteriaCategory.NT => "≤ 55 NT",
                CriteriaCategory.LC => nameof(CriteriaCategory.LC),
                CriteriaCategory.DD => nameof(CriteriaCategory.DD),
                CriteriaCategory.NE => nameof(CriteriaCategory.NE),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.BSites => category switch
            {
                CriteriaCategory.CR => "ant lok 1 CR",
                CriteriaCategory.EN => "ant lok ≤ 5 EN",
                CriteriaCategory.VU => "ant lok ≤ 10 VU",
                CriteriaCategory.NT => string.Empty,
                CriteriaCategory.LC => string.Empty,
                CriteriaCategory.DD => string.Empty,
                CriteriaCategory.NE => string.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            CriteriaCategoryType.C or CriteriaCategoryType.D => category switch
            {
                CriteriaCategory.CR => "≥ 80 %",
                CriteriaCategory.EN => "≥ 50 %",
                CriteriaCategory.VU => "≥ 30 %",
                CriteriaCategory.NT => "≥ 20 %",
                CriteriaCategory.LC => "< 20 %",
                CriteriaCategory.DD => nameof(CriteriaCategory.DD),
                CriteriaCategory.NE => nameof(CriteriaCategory.NE),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static string GetDescription(this CriteriaCategoryImpact criteriaCategoryChange)
    {
        return criteriaCategoryChange switch
        {
            CriteriaCategoryImpact.None => string.Empty,
            CriteriaCategoryImpact.AssumedOrKnown => "antatt eller kjent",
            _ => throw new ArgumentOutOfRangeException(nameof(criteriaCategoryChange), criteriaCategoryChange, null)
        };
    }

    public static string GetDescription(this CriteriaCategoryThreatDefinedlocation criteriaCategoryChange)
    {
        return criteriaCategoryChange switch
        {
            CriteriaCategoryThreatDefinedlocation.None => string.Empty,
            CriteriaCategoryThreatDefinedlocation.VU => "< 5 lokaliteter VU",
            CriteriaCategoryThreatDefinedlocation.NT => "< 10 lokaliteter NT",
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