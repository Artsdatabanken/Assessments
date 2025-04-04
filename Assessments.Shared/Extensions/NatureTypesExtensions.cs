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
}