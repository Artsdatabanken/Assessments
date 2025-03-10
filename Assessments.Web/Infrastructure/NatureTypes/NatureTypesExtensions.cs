using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Web.Infrastructure.NatureTypes;

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
}