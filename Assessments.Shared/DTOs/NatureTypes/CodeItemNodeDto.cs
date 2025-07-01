namespace Assessments.Shared.DTOs.NatureTypes;

public class CodeItemNodeDto
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int Level { get; set; }

    public string Description { get; set; }

    public IEnumerable<CodeItemNodeDto> Nodes { get; set; } = [];

    public CodeItemDto CodeItemDto { get; set; }
}