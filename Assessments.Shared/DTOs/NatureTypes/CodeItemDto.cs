namespace Assessments.Shared.DTOs.NatureTypes;

public record CodeItemDto
{
    public string CodeItemDescription { get; set; }

    public string TimeOfIncident { get; set; }

    public string InfluenceFactor { get; set; }

    public string Magnitude { get; set; }
}