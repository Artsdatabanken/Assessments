namespace Assessments.Web.Models.AlienSpecies;

public class AlienSpeciesStatisticsViewModel
{
    public BarChart DecisiveCriteria { get; set; }

    public BarChart RiskCategories { get; set; }

    public List<List<int>> RiskMatrix { get; set; }

    public BarChart SpeciesGroups { get; set; }

    public BarChart MajorNatureTypesEffect { get; set; }

    public List<BarChart> NatureTypesEffect { get; set; }

    public BarChart SpreadWays { get; set; }

    public List<BarChart> SpreadWaysIntroduction { get; set; }

    public List<BarChart> EstablishmentClass { get; set; }
}