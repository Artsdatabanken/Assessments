using System;
using System.Collections.Generic;
using System.Linq;
using Assessments.Shared.Constants;
using Assessments.Shared.DTOs.NatureTypes;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Shared.Extensions;

public static class NatureTypesExtensions
{
    public static string GetDescription(this Category category) => category switch
    {
        Category.CO => "Gått tapt",
        Category.CR => "Kritisk truet",
        Category.EN => "Sterkt truet",
        Category.VU => "Sårbar",
        Category.NT => "Nær truet",
        Category.DD => "Datamangel",
        Category.LC => "Uten risiko",
        Category.NA => "Ikke egnet",
        Category.NE => "Ikke vurdert",
        _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
    };

    public static string GetCitation(this List<CommitteeUserDto> committeeUsers, string committeeName)
    {
        var users = committeeUsers.OrderByDescending(x => x.Level).ThenBy(x => x.UserLastName)
            .Select(x => !string.IsNullOrEmpty(x.UserCitationName) ? x.UserCitationName : $"{x.UserLastName}, {x.UserFirstName[0]}.")
            .ToList();

        var citation = $"{users.JoinAnd(", ", " og ")} (alfabetisk) (2025). {committeeName}. {NatureTypesConstants.CitationSummary}";
        
        return citation;
    }
}