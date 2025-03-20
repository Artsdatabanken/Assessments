namespace Assessments.Shared.DTOs.NatureTypes.Statistics;

public record CategoryStatisticsResponse
{
    public string Category { get; set; }

    public int Count { get; set; }
}