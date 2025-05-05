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

    public static string GetDescription(this CriteriaCategoryChange criteriaCategoryChange)
    {
        return criteriaCategoryChange switch
        {
            CriteriaCategoryChange.None => string.Empty,
            CriteriaCategoryChange.Area => "i. Areal",
            CriteriaCategoryChange.Quality => "ii. Kvalitet",
            CriteriaCategoryChange.Interactions => "iii. Interaksjoner",
            CriteriaCategoryChange.ProbablyOngoingDecline => "iv. Trolig pågående nedgang i kvalitet eller areal",
            _ => throw new ArgumentOutOfRangeException(nameof(criteriaCategoryChange), criteriaCategoryChange, null)
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
}