using RodlisteNaturtyper.Data.Models;

namespace Assessments.Shared.DTOs.NatureTypes;

public record CodeItemDto
{
    public int CodeItemId { get; set; }

    public List<CodeItem> ParentCodeItems { get; set; } = [];

    public string CodeItemDescription { get; set; }

    public string TimeOfIncident { get; set; }

    public string InfluenceFactor { get; set; }

    public string Magnitude { get; set; }
}