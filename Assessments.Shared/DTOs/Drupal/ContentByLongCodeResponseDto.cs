namespace Assessments.Shared.DTOs.Drupal;

public record ContentByLongCodeResponseDto
{
    public string Id { get; init; }

    public string Type { get; init; }

    public List<FieldResponseDto> Fields { get; init; } = [];
}