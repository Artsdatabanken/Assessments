using Assessments.Shared.Constants;
using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.DTOs.NatureTypes.Enums;
using Assessments.Shared.Helpers;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Shared.Extensions;

public static class NatureTypesExtensions
{
    public static string GetDescription(this Category category) => category.ConvertTo<NatureTypeCategoryDto>().DisplayName();

    public static string GetDescription(this AssessmentRegion region) => region.ConvertTo<AssessmentRegionDto>().DisplayName();

    public static string GetCitation(this List<CommitteeUserDto> committeeUsers, string committeeName)
    {
        var users = committeeUsers.OrderByDescending(x => x.Level).ThenBy(x => x.UserLastName)
            .Select(x => !string.IsNullOrEmpty(x.UserCitationName) ? x.UserCitationName : $"{x.UserLastName}, {x.UserFirstName[0]}.")
            .ToList();

        var citation = $"{users.JoinAnd(", ", " og ")} (alfabetisk) (2025). {committeeName}. {NatureTypesConstants.CitationSummary}";
        
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
            CriteriaCategoryType.C => category switch
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
}