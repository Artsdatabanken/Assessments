namespace Assessments.Web.Views.NatureTypes.Components.VariableNames;

public class VariableNamesModel
{
    public List<VariableNameViewModel> VariableNames { get; set; } = [];
}

public record VariableNameViewModel
{
    public string Name { get; init; }

    public string ShortCode { get; init; }

    public string LongCode { get; init; }

    public List<VariableNameStepViewModel> Steps { get; } = [];
}

public record VariableNameStepViewModel
{
    public string Description { get; init; }
}