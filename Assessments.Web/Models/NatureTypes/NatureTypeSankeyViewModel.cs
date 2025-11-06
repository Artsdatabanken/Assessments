namespace Assessments.Web.Models.NatureTypes;

public record NatureTypeSankeyViewModel : NatureTypeSankeyModel
{
    public List<NatureTypeSankeyModel> Models { get; set; } = [];
}

public record NatureTypeSankeyModel
{
    public string Name { get; set; }

    public string Description { get; set; }
}
