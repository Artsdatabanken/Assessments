using Assessments.Mapping.AlienSpecies.Model;
using X.PagedList;

namespace Assessments.Web.Models
{
    public class AlienSpeciesListViewModel : AlienSpeciesListParameters
    {
        public IPagedList<AlienSpeciesAssessment2023> Results { get; set; }

        public AlienSpeciesStatistics2023 Statistics { get; set; }
    }

    public class AlienSpeciesListParameters
    {
        public IEnumerable<string> FilterParameters { get; } = new[]
        {
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
        };

        public string SortBy { get; set; }

        public string View { get; set; }

        public string[] Meta { get; set; } = Array.Empty<string>();

        public string[] IsCheck { get; set; } = Array.Empty<string>();

        public string Name { get; set; }

        public string[] Area { get; set; } = Array.Empty<string>();

        public string[] Category { get; set; } = Array.Empty<string>();

        public string[] CategoryChanged { get; set; } = Array.Empty<string>();

        public string[] ClimateEffects { get; set; } = Array.Empty<string>();

        public string[] Criterias { get; set; } = Array.Empty<string>();

        public string[] EcologicalEffect { get; set; } = Array.Empty<string>();

        public string[] Environment { get; set; } = Array.Empty<string>();

        public string[] InvasionPotential { get; set; } = Array.Empty<string>();

        public string[] NatureTypes { get; set; } = Array.Empty<string>();

        public string[] DecisiveCriterias { get; set; } = Array.Empty<string>();

        public string[] SpeciesStatus { get; set; } = Array.Empty<string>();

        public string[] GeographicVariations { get; set; } = Array.Empty<string>();

        public string[] Habitats { get; set; } = Array.Empty<string>();

        public string[] NotAssessed { get; set; } = Array.Empty<string>();

        public string[] ProductionSpecies { get; set; } = Array.Empty<string>();

        public string[] Regions { get; set; } = Array.Empty<string>();

        public string[] RegionallyAlien { get; set; } = Array.Empty<string>();

        public string[] SpeciesGroups { get; set; } = Array.Empty<string>();

        public string[] SpreadWays { get; set; } = Array.Empty<string>();

        public string[] TaxonRank { get; set; } = Array.Empty<string>();
    }

    public class AlienSpeciesStatistics2023
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
}
