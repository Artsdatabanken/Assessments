namespace Assessments.Shared.DTOs.Drupal;

public record ContentRootResponseDto
{
    public string Id { get; init; }

    public string Title { get; init; }

    public string Body { get; init; }

    public string Heading { get; init; }

    public string Intro { get; init; }

    public List<ReferenceResponseDto> References { get; init; } = [];

    public List<RecordResponseDto> Records { get; init; } = [];
}