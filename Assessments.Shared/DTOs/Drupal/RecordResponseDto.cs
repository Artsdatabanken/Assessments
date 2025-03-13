namespace Assessments.Shared.DTOs.Drupal;

public record RecordResponseDto
{
    public string Label { get; init; }

    public List<string> Values { get; init; } = [];

    public List<ReferenceResponseDto> References { get; init; } = [];
}