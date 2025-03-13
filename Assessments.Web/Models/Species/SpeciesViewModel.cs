using Assessments.Mapping.RedlistSpecies;
using X.PagedList;

namespace Assessments.Web.Models.Species;

public class SpeciesViewModel
{
    public IPagedList<SpeciesAssessment2021> Redlist2021Results { get; set; }

    public bool Endangered { get; set; }

    public string Name { get; set; }

    public bool PresumedExtinct { get; set; }

    public bool Redlisted { get; set; }

    public string SortBy { get; set; }

    public string View { get; set; }

    public string[] Area { get; set; } = [];

    public string[] Category { get; set; } = [];

    public string[] Criterias { get; set; } = [];

    public string[] EuroPop { get; set; } = [];

    public string[] IsCheck { get; set; } = [];

    public string[] Habitats { get; set; } = [];

    public string[] Meta { get; set; } = [];

    public string[] Regions { get; set; } = [];

    public string[] SpeciesGroups { get; set; } = [];

    public string[] TaxonRank { get; set; } = [];

    public SpeciesStatisticsViewModel Statistics { get; set; } = new();
}