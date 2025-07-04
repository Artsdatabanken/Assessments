using Assessments.Shared.DTOs.NatureTypes.Enums;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesStatisticsViewModel
{
    public Dictionary<Category, double> Categories { get; init; } = [];

    public Dictionary<string, double> Regions { get; init; } = [];

    public Dictionary<CategoryCriteriaType, double> CategoryCriteriaType { get; init; } = [];
}