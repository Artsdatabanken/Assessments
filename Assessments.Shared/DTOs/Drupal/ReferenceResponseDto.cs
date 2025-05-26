namespace Assessments.Shared.DTOs.Drupal;

public record ReferenceResponseDto
{
    public string Heading { get; init; }

    public string Url { get; init; }

    public string Title { get; init; }

    public string Type { get; init; }

    public DateTime? Published { get; set; }

    public List<RecordResponseDto> Records { get; init; } = [];
}