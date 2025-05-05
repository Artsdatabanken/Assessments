using Assessments.Web.Models.NatureTypes.Enums;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListParameters
{
    public string Name { get; set; }

    public string[] Area { get; set; } = [];

    public string[] Category { get; set; } = [];
    
    public string[] Topic { get; set; } = [];

    public string[] Region { get; set; } = [];

    public SortByEnum SortBy { get; set; }
    
    public string[] Meta { get; set; } = [];

    public string[] IsCheck { get; set; } = [];

    public string View { get; set; }
}