namespace Assessments.Shared.DTOs.Drupal;

public record ImageModelDto
{
    public Uri Url { get; init; }

    public Uri Link { get; init; }

    public string License { get; init; }

    public string Text { get; init; }

    public List<string> Authors { get; init; } = [];
}