using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesStatisticsViewModel
{
    public Dictionary<Category, double> Categories { get; init; } = [];
}