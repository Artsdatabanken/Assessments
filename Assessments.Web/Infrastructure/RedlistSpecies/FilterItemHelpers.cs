using Assessments.Web.Infrastructure.RedlistSpecies.Enums;
using Assessments.Shared.Helpers;
using static Assessments.Web.Infrastructure.FilterHelpers;

namespace Assessments.Web.Infrastructure.RedlistSpecies
{
    public class RedlistAreas
    {
        public static readonly FilterItem[] RedlistSpecies2021AreasFilters = Enum.GetValues<RedlistSpeciesAssessment2021EvaluationContext>()
            .Select(x => new FilterItem
            {
                Name = x.DisplayName(),
                NameShort = x.ToString()
            }).ToArray();

        public static readonly FilterAndMetaData RedlistSpecies2021Areas = new()
        {
            Filters = RedlistSpecies2021AreasFilters,
            FilterDescription = "",
            FilterButtonName = "områdefiltre",
            FilterButtonText = "Vurderingsområde"
        };
    }

    public class RedlistCategories
    {
        public static readonly FilterItem[] RedlistSpecies2021CategoriesFilters = Enum.GetValues<RedlistSpeciesAssessment2021Category>()
            .Select(x => new FilterItem
            {
                NameShort = x.ToString(),
                Name = x.DisplayName(),
                Description = x.DisplayName()
            }).ToArray();

        public static readonly FilterAndMetaData RedlistSpecies2021Categories = new()
        {
            Filters = RedlistSpecies2021CategoriesFilters,
            FilterDescription = "",
            FilterButtonName = "kategorifiltre",
            FilterButtonText = "Kategori"
        };
    }

    public class RedlistCriteria
    {
        public static readonly FilterItem[] RedlistSpecies2021CriteriaFilters = Enum.GetValues<RedlistSpeciesAssessment2021Criteria>()
            .Select(x => new FilterItem
            {
                NameShort = x.ToString(),
                Name = x.DisplayName(),
                Description = x.DisplayName()
            }).ToArray();

        public static readonly FilterAndMetaData RedlistSpecies2021Criteria = new()
        {
            Filters = RedlistSpecies2021CriteriaFilters,
            FilterDescription = "",
            FilterButtonName = "kriteriefiltre",
            FilterButtonText = "Kriterier"
        };
    }

    public class RedlistEuropeanPopulation
    {
        public static readonly FilterItem[] RedlistSpecies2021EuropeanPopulationFilters = Enum.GetValues<RedlistSpeciesAssessment2021EuropeanPopulation>()
            .Select(x => new FilterItem
            {
                NameShort = x.ToString(),
                Name = x.DisplayName(),
                Description = x.DisplayName()
            }).ToArray();

        public static readonly FilterAndMetaData RedlistSpecies2021EuropeanPopulation = new()
        {
            Filters = RedlistSpecies2021EuropeanPopulationFilters,
            FilterDescription = "",
            FilterButtonName = "europeiskandelpopulasjonsfiltre",
            FilterButtonText = "Andel av europeisk populasjon"
        };
    }

    public class RedlistRegions
    {
        public static readonly FilterItem[] RedlistSpecies2021RegionsFilters = Enum.GetValues<RedlistSpeciesAssessment2021Regions>()
            .Select(x => new FilterItem
            {
                NameShort = ((int) x).ToString(),
                Name = x.DisplayName(),
                Description = x.DisplayName()
            }).ToArray();

        public static readonly FilterAndMetaData RedlistSpecies2021Regions = new()
        {
            Filters = RedlistSpecies2021RegionsFilters,
            FilterDescription = "",
            FilterButtonName = "regionsfiltre",
            FilterButtonText = "Regioner og havområder"
        };
    }

    public class RedlistHabitats
    {
        public static readonly FilterItem[] RedlistSpecies2021HabitatFilters = Enum.GetValues<RedlistSpeciesAssessment2021Habitats>()
            .Select(x => new FilterItem
            {
                NameShort = x.ToString(),
                Name = x.DisplayName(),
                Description = x.DisplayName()
            }).ToArray();

        public static readonly FilterAndMetaData RedlistSpecies2021Habitats = new()
        {
            Filters = RedlistSpecies2021HabitatFilters,
            FilterDescription = "",
            FilterButtonName = "habitatfiltre",
            FilterButtonText = "Hovedhabitat"
        };
    }

    public class RedlistTaxonRank
    {
        public static readonly FilterItem[] RedlistSpecies2021TaxonRankFilters = Enum.GetValues<RedlistSpeciesAssessment2021TaxonRank>()
            .Select(x => new FilterItem
            {
                NameShort = x.ToString(),
                Name = x.DisplayName(),
                Description = x.DisplayName()
            }).ToArray();

        public static readonly FilterAndMetaData RedlistSpecies2021Habitats = new()
        {
            Filters = RedlistSpecies2021TaxonRankFilters,
            FilterDescription = "",
            FilterButtonName = "taksonomisknivåfiltre",
            FilterButtonText = "Taksonomisk nivå"
        };
    }

    public class RedlistSpeciesGroups
    {
        private static readonly FilterItem[] RedlistSpecies2021Insects =
        {
            new()
            {
                Name = "Biller",
                NameShort = "Biller",
            },
            new()
            {
                Name = "Døgnfluer",
                NameShort = "Døgnfluer",
            },
            new()
            {
                Name = "Kakerlakker",
                NameShort = "Kakerlakker",
            },
            new()
            {
                Name = "Kamelhalsfluer",
                NameShort = "Kamelhalsfluer",
            },
            new()
            {
                Name = "Mudderfluer",
                NameShort = "Mudderfluer",
            },
            new()
            {
                Name = "Nebbfluer",
                NameShort = "Nebbfluer",
            },
            new()
            {
                Name = "Nebbmunner",
                NameShort = "Nebbmunner",
            },
            new()
            {
                Name = "Nettvinger",
                NameShort = "Nettvinger",
            },
            new()
            {
                Name = "Rettvinger",
                NameShort = "Rettvinger",
            },
            new()
            {
                Name = "Saksedyr",
                NameShort = "Saksedyr",
            },
            new()
            {
                Name = "Sommerfugler",
                NameShort = "Sommerfugler",
            },
            new()
            {
                Name = "Steinfluer",
                NameShort = "Steinfluer",
            },
            new()
            {
                Name = "Tovinger",
                NameShort = "Tovinger",
            },
            new()
            {
                Name = "Vepser",
                NameShort = "Vepser",
            },
            new()
            {
                Name = "Vårfluer",
                NameShort = "Vårfluer",
            },
            new()
            {
                Name = "Øyenstikkere",
                NameShort = "Øyenstikkere",
            }
        };

        public static readonly FilterItem[] RedlistSpecies2021SpeciesGroupsFilters =
        {
            new()
            {
                Name = "Alger",
                NameShort = "Alger",
            },
            new()
            {
                Name = "Amfibier og reptiler",
                NameShort = "Amfibier og reptiler",
            },
            new()
            {
                Name = "Armfotinger",
                NameShort = "Armfotinger",
            },
            new()
            {
                Name = "Bløtdyr",
                NameShort = "Bløtdyr",
            },
            new()
            {
                Name = "Edderkoppdyr",
                NameShort = "Edderkoppdyr",
            },
            new()
            {
                Name = "Fisker",
                NameShort = "Fisker",
            },
            new()
            {
                Name = "Fugler",
                NameShort = "Fugler",
            },
            new()
            {
                Name = "Hydrozoer",
                NameShort = "Hydrozoer",
            },
            new()
            {
                Name = "Insekter",
                NameShort = "Insekter",
                SubGroup = new()
                {
                    Filters = RedlistSpecies2021Insects,
                    FilterDescription = ""
                }
            },
            new()
            {
                Name = "Karplanter",
                NameShort = "Karplanter",
            },
            new()
            {
                Name = "Koralldyr",
                NameShort = "Koralldyr",
            },
            new()
            {
                Name = "Krepsdyr",
                NameShort = "Krepsdyr",
            },
            new()
            {
                Name = "Laver",
                NameShort = "Laver",
            },
            new()
            {
                Name = "Leddormer",
                NameShort = "Leddormer",
            },
            new()
            {
                Name = "Mangefotinger",
                NameShort = "Mangefotinger",
            },
            new()
            {
                Name = "Mosdyr",
                NameShort = "Mosdyr",
            },
            new()
            {
                Name = "Moser",
                NameShort = "Moser",
            },
            new()
            {
                Name = "Pattedyr",
                NameShort = "Pattedyr",
            },
            new()
            {
                Name = "Pigghuder",
                NameShort = "Pigghuder",
            },
            new()
            {
                Name = "Sekkdyr",
                NameShort = "Sekkdyr",
            },
            new()
            {
                Name = "Sopper",
                NameShort = "Sopper",
            },
            new()
            {
                Name = "Spretthaler",
                NameShort = "Spretthaler",
            },
            new()
            {
                Name = "Stormaneter",
                NameShort = "Stormaneter",
            },
            new()
            {
                Name = "Svamper",
                NameShort = "Svamper",
            },
        };

        public static readonly FilterAndMetaData RedlistSpecies2021SpeciesGroups = new()
        {
            Filters = RedlistSpecies2021SpeciesGroupsFilters,
            FilterDescription = "",
            FilterButtonName = "artsgruppefiltre",
            FilterButtonText = "Artsgruppe"
        };
    }

    public class PresumedExtinct
    {
        public static readonly FilterItem[] RedlistSpecies2021PresumedExtinctFilters =
        {
            new()
            {
                Name = "Trolig utdødd",
                NameShort = "True",
            },
        };

        public static readonly FilterAndMetaData RedlistSpecies2021PresumedExtinct = new()
        {
            Filters = RedlistSpecies2021PresumedExtinctFilters,
            FilterDescription = "",
            FilterButtonName = "utdøddfiltre",
            FilterButtonText = "Trolig utdødd"
        };
    }
}
