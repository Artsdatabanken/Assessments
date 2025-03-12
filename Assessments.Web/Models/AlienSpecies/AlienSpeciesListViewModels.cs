using Assessments.Mapping.AlienSpecies.Model;
using X.PagedList;

namespace Assessments.Web.Models.AlienSpecies;

public class AlienSpeciesListViewModel : AlienSpeciesListParameters
{
    public IPagedList<AlienSpeciesAssessment2023> Results { get; set; }

    public AlienSpeciesStatisticsViewModel Statistics { get; set; }
}

public class AlienSpeciesListParameters
{
    public IEnumerable<string> FilterParameters { get; } =
    [
        nameof(Area),
        nameof(Category),
        nameof(EcologicalEffect),
        nameof(Environment),
        nameof(InvasionPotential),
        nameof(Criterias),
        nameof(DecisiveCriterias),
        nameof(SpeciesStatus),
        nameof(Habitats),
        nameof(NatureTypes),
        nameof(NotAssessed),
        nameof(ProductionSpecies),
        nameof(RegionallyAlien),
        nameof(Regions),
        nameof(SpeciesGroups),
        nameof(SpreadWays),
        nameof(TaxonRank),
        nameof(CategoryChanged),
        nameof(GeographicVariations),
        nameof(ClimateEffects)
    ];

    public string SortBy { get; set; }

    public string View { get; set; }

    public string[] Meta { get; set; } = [];

    public string[] IsCheck { get; set; } = [];

    public string Name { get; set; }

    public string[] Area { get; set; } = [];

    public string[] Category { get; set; } = [];

    public string[] CategoryChanged { get; set; } = [];

    public string[] ClimateEffects { get; set; } = [];

    public string[] Criterias { get; set; } = [];

    public string[] EcologicalEffect { get; set; } = [];

    public string[] Environment { get; set; } = [];

    public string[] InvasionPotential { get; set; } = [];

    public string[] NatureTypes { get; set; } = [];

    public string[] DecisiveCriterias { get; set; } = [];

    public string[] SpeciesStatus { get; set; } = [];

    public string[] GeographicVariations { get; set; } = [];

    public string[] Habitats { get; set; } = [];

    public string[] NotAssessed { get; set; } = [];

    public string[] ProductionSpecies { get; set; } = [];

    public string[] Regions { get; set; } = [];

    public string[] RegionallyAlien { get; set; } = [];

    public string[] SpeciesGroups { get; set; } = [];

    public string[] SpreadWays { get; set; } = [];

    public string[] TaxonRank { get; set; } = [];
}