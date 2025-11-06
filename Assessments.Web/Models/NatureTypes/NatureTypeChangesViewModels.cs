namespace Assessments.Web.Models.NatureTypes;

public record NatureTypeChangesViewModel : NatureTypeChangesModel
{
    public List<NatureTypeChangesModel> Models { get; set; } = [];
}

public record NatureTypeChangesModel
{
    public string Name { get; set; }

    public string Description { get; set; }
}
