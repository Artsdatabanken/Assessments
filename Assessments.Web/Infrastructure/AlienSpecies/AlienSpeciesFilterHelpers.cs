using Assessments.Web.Models.AlienSpecies;
using static Assessments.Web.Infrastructure.FilterHelpers;

namespace Assessments.Web.Infrastructure.AlienSpecies
{
    public class AlienSpeciesFilterHelpers : IFilter<AlienSpeciesListParameters>
    {
        public string IGetActiveFilters(string filterType, AlienSpeciesListParameters parameters)
        {
            return GetActiveFilters(filterType, parameters);
        }

        public string IGetActiveSelection(AlienSpeciesListParameters parameters)
        {
            return GetActiveSelection(parameters);
        }

        public int IGetActiveSelectionCount(AlienSpeciesListParameters parameters)
        {
            return GetActiveSelectionCount(parameters);
        }

        public string[] IGetActiveSelectionElement(AlienSpeciesListParameters parameters)
        {
            return GetActiveSelectionElement(parameters);
        }

        public string IGetChipText(string filter, FilterItem[] filterItems)
        {
            return GetChipText(filter, filterItems);
        }

        public static string GetActiveFilters(string filterType, AlienSpeciesListParameters parameters)
        {
            switch (filterType)
            {
                case nameof(parameters.Area):
                    if (parameters.Area?.Any() == true)
                        return $"{parameters.Area.Length}";
                    return String.Empty;
                case nameof(parameters.Category):
                    if (parameters.Category?.Any() == true)
                    {
                        if (parameters.Category.Contains(nameof(parameters.Category)))
                            return $"{parameters.Category.Length - 1}";
                        return $"{parameters.Category.Length}";
                    }
                    return String.Empty;
                case nameof(parameters.EcologicalEffect):
                    if (parameters.EcologicalEffect?.Any() == true)
                        return $"{parameters.EcologicalEffect.Length}";
                    return String.Empty;
                case nameof(parameters.Environment):
                    if (parameters.Environment?.Any() == true)
                        return $"{parameters.Environment.Length}";
                    return String.Empty;
                case nameof(parameters.NatureTypes):
                    if (parameters.NatureTypes?.Any() == true)
                    {
                        var counter = parameters.NatureTypes.Length;
                        if (parameters.NatureTypes.Contains("Nta"))
                            counter--;
                        if (parameters.NatureTypes.Contains("Ntn"))
                            counter--;
                        return $"{counter}";
                    }
                    return String.Empty;
                case nameof(parameters.InvasionPotential):
                    if (parameters.InvasionPotential?.Any() == true)
                        return $"{parameters.InvasionPotential.Length}";
                    return String.Empty;
                case nameof(parameters.CategoryChanged):
                    if (parameters.CategoryChanged?.Any() == true)
                    {
                        if (parameters.CategoryChanged.Contains("ccke"))
                            return $"{parameters.CategoryChanged.Length - 1}";
                        return $"{parameters.CategoryChanged.Length}";
                    }
                    return String.Empty;
                case nameof(parameters.DecisiveCriterias):
                    if (parameters.DecisiveCriterias?.Any() == true)
                    {
                        if (parameters.DecisiveCriterias.Contains("dcok"))
                            return $"{parameters.DecisiveCriterias.Length - 1}";
                        return $"{parameters.DecisiveCriterias.Length}";
                    }
                    return String.Empty;
                case nameof(parameters.SpeciesStatus):
                    if (parameters.SpeciesStatus?.Any() == true)
                    {
                        if (parameters.SpeciesStatus.Contains("eds"))
                            return $"{parameters.SpeciesStatus.Length - 1}";
                        return $"{parameters.SpeciesStatus.Length}";
                    }
                    return String.Empty;
                case nameof(parameters.SpreadWays):
                    if (parameters.SpreadWays?.Any() == true)
                    {
                        var counter = parameters.SpreadWays.Length;
                        if (parameters.SpreadWays.Contains("Swnat"))
                            counter--;
                        if (parameters.SpreadWays.Contains("Swspr"))
                            counter--;
                        return $"{counter}";
                    }
                    return String.Empty;
                case nameof(parameters.TaxonRank):
                    if (parameters.TaxonRank?.Any() == true)
                    {
                        if (parameters.TaxonRank.Contains("ttn"))
                            return $"{parameters.TaxonRank.Length - 1}";
                        return $"{parameters.TaxonRank.Length}";
                    }
                    return String.Empty;
                case nameof(parameters.NotAssessed):
                    if (parameters.NotAssessed?.Any() == true)
                        return $"{parameters.NotAssessed.Length}";
                    return String.Empty;
                case nameof(parameters.ProductionSpecies):
                    if (parameters.ProductionSpecies?.Any() == true)
                        return $"{parameters.ProductionSpecies.Length}";
                    return String.Empty;
                case nameof(parameters.SpeciesGroups):
                    if (parameters.SpeciesGroups?.Any() == true)
                    {
                        var count = parameters.SpeciesGroups.Length;
                        if (parameters.SpeciesGroups.Contains("sin"))
                            count--;
                        if (parameters.SpeciesGroups.Contains(nameof(parameters.SpeciesGroups)))
                            count--;
                        return $"{count}";
                    }
                    return String.Empty;
                case nameof(parameters.Habitats):
                    if (parameters.Habitats?.Any() == true)
                        return $"{parameters.Habitats.Length}";
                    return String.Empty;
                case nameof(parameters.RegionallyAlien):
                    if (parameters.RegionallyAlien?.Any() == true)
                    {
                        if (parameters.RegionallyAlien.Contains("Rar"))
                            return $"{parameters.RegionallyAlien.Length - 1}";
                        return $"{parameters.RegionallyAlien.Length}";
                    }
                    return String.Empty;
                case nameof(parameters.Regions):
                    if (parameters.Regions?.Any() == true)
                    {
                        if (parameters.Regions.Contains(nameof(parameters.Regions)))
                            return $"{parameters.Regions.Length - 1}";
                        return $"{parameters.Regions.Length}";
                    }
                    return String.Empty;
                case nameof(parameters.Criterias):
                    if (parameters.Criterias?.Any() == true)
                        return $"{parameters.Criterias.Length}";
                    return String.Empty;
                case nameof(parameters.ClimateEffects):
                    if (parameters.ClimateEffects?.Any() == true)
                        return $"{parameters.ClimateEffects.Length}";
                    return String.Empty;
                case nameof(parameters.GeographicVariations):
                    if (parameters.GeographicVariations?.Any() == true)
                        return $"{parameters.GeographicVariations.Length}";
                    return String.Empty;
                default:
                    return String.Empty;
            }
        }

        public static string GetActiveSelection(AlienSpeciesListParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Name))
            {
                return $"{parameters.Name}";
            }
            return String.Empty;
        }

        public static int GetActiveSelectionCount(AlienSpeciesListParameters parameters)
        {
            int count = 0;
            count += parameters.Area.Length;
            count += parameters.Category.Length;
            count += parameters.EcologicalEffect.Length;
            count += parameters.Environment.Length;
            count += parameters.NatureTypes.Length;
            count += parameters.InvasionPotential.Length;
            count += parameters.CategoryChanged.Length;
            count += parameters.DecisiveCriterias.Length;
            count += parameters.SpeciesStatus.Length;
            count += parameters.SpeciesGroups.Length;
            count += parameters.NotAssessed.Length;
            count += parameters.ProductionSpecies.Length;
            count += parameters.SpreadWays.Length;
            count += parameters.TaxonRank.Length;
            count += parameters.Habitats.Length;
            count += parameters.RegionallyAlien.Length;
            count += parameters.Regions.Length;
            count += parameters.Criterias.Length;
            count += parameters.ClimateEffects.Length;
            count += parameters.GeographicVariations.Length;

            return count;
        }

        public static string[] GetActiveSelectionElement(AlienSpeciesListParameters parameters)
        {
            var selectionlist = parameters.Area;
            selectionlist = selectionlist.Concat(parameters.Category).ToArray();
            selectionlist = selectionlist.Concat(parameters.EcologicalEffect).ToArray();
            selectionlist = selectionlist.Concat(parameters.Environment).ToArray();
            selectionlist = selectionlist.Concat(parameters.NatureTypes).ToArray();
            selectionlist = selectionlist.Concat(parameters.InvasionPotential).ToArray();
            selectionlist = selectionlist.Concat(parameters.CategoryChanged).ToArray();
            selectionlist = selectionlist.Concat(parameters.DecisiveCriterias).ToArray();
            selectionlist = selectionlist.Concat(parameters.SpeciesStatus).ToArray();
            selectionlist = selectionlist.Concat(parameters.NotAssessed).ToArray();
            selectionlist = selectionlist.Concat(parameters.ProductionSpecies).ToArray();
            selectionlist = selectionlist.Concat(parameters.SpeciesGroups).ToArray();
            selectionlist = selectionlist.Concat(parameters.SpreadWays).ToArray();
            selectionlist = selectionlist.Concat(parameters.TaxonRank).ToArray();
            selectionlist = selectionlist.Concat(parameters.Habitats).ToArray();
            selectionlist = selectionlist.Concat(parameters.RegionallyAlien).ToArray();
            selectionlist = selectionlist.Concat(parameters.Regions).ToArray();
            selectionlist = selectionlist.Concat(parameters.Criterias).ToArray();
            selectionlist = selectionlist.Concat(parameters.GeographicVariations).ToArray();
            selectionlist = selectionlist.Concat(parameters.ClimateEffects).ToArray();
            return selectionlist;
        }

        public static string GetChipText(string filter, FilterItem[] filterItems)
        {
            for (int i = 0; i < filterItems.Length; i++)
            {
                FilterItem item = filterItems[i];
                if (item.NameShort == filter)
                {
                    return item.Name;
                }
                else if (item.SubGroup != null)
                {
                    string buttonText = GetChipText(filter, item.SubGroup.Filters);
                    if (!string.IsNullOrEmpty(buttonText))
                        return buttonText;
                }
            }
            return String.Empty;
        }
    }
}
