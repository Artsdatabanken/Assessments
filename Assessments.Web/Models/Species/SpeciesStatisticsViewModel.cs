namespace Assessments.Web.Models.Species;

public class SpeciesStatisticsViewModel
{
    public Dictionary<string, int> Categories { get; set; }

    public Dictionary<string, int> Criteria { get; set; }

    public Dictionary<string, int> Habitat { get; set; }

    public Dictionary<string, int> ImpactFactors { get; set; }

    public Dictionary<string, int> Region { get; set; }

    public List<string> RegionNames { get; set; }
}