using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Shared.DTOs.NatureTypes;

public record CommitteeUserDto
{
    public int CommitteeId { get; init; }

    public string CommitteeName { get; init; }

    public CommitteeLevel Level { get; init; }

    public string UserCitationName { get; init; }
    
    public string UserLastName { get; init; }

    public string UserFirstName { get; init; }
}