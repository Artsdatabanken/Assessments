namespace Assessments.Shared.DTOs.NatureTypes.Statistics;

public record CategoryStatisticsRootResponse
{
    public List<CategoryStatisticsResponse> Value { get; set; }
}