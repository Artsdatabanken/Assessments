namespace Assessments.Shared.DTOs.Drupal;

public record FieldResponseDto
{
    public string Name { get; init; }

    public List<string> Values { get; init; } = [];

    public List<string> References { get; init; } = [];

    public List<FieldResponseDto> Fields { get; init; } = [];
}