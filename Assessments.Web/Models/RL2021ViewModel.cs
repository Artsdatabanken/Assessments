using Assessments.Mapping.RedlistSpecies;
using X.PagedList;

namespace Assessments.Web.Models
{
    public class RL2021ViewModel
    {
        public RL2021ViewModel()
        {
            this.Area = Array.Empty<string>();
            this.Category = Array.Empty<string>();
            this.Criterias = Array.Empty<string>();
            this.EuroPop = Array.Empty<string>();
            this.IsCheck = Array.Empty<string>();
            this.Meta = Array.Empty<string>();
            this.Regions = Array.Empty<string>();
            this.Habitats = Array.Empty<string>();
            this.SpeciesGroups = Array.Empty<string>();
            this.TaxonRank = Array.Empty<string>();
        }
        public IPagedList<SpeciesAssessment2021> Redlist2021Results { get; set; }
        public bool Endangered { get; set; }
        public string Name { get; set; }
        public bool PresumedExtinct { get; set; }
        public bool Redlisted { get; set; }
        public string SortBy { get; set; }
        public string View { get; set; }
        public string[] Area { get; set; }
        public string[] Category { get; set; }
        public string[] Criterias { get; set; }
        public string[] EuroPop { get; set; }
        public string[] IsCheck { get; set; }
        public string[] Habitats { get; set; }
        public string[] Meta { get; set; }
        public string[] Regions { get; set; }
        public string[] SpeciesGroups { get; set; }
        public string[] TaxonRank { get; set; }
        
        public string Redlist2021ResultsCount =>
            Redlist2021Results.Count > 0 ? 
                $"Viser {Redlist2021Results.Count} av {Redlist2021Results.TotalItemCount:N0}" : "Ingen resultater";

        public Species2001StatisticsViewModel Statistics { get; set; } = new();
    }

    public class Species2001StatisticsViewModel
    {
        public Dictionary<string, int> Categories { get; set; }

        public Dictionary<string, int> Criteria { get; set; }

        public Dictionary<string, int> Habitat { get; set; }

        public Dictionary<string, int> ImpactFactors { get; set; }

        public Dictionary<string, int> Region { get; set; }

        public List<string> RegionNames { get; set; }
    }
}