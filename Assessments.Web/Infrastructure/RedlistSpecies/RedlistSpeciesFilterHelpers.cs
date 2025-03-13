using Assessments.Web.Models.AlienSpecies;
using Assessments.Web.Models.Species;
using static Assessments.Web.Infrastructure.FilterHelpers;

namespace Assessments.Web.Infrastructure.RedlistSpecies
{
    public static class RedlistSpeciesFilterHelpers
    {
        public static string GetActiveFilters(string filterType, SpeciesViewModel Model)
        {
            switch (filterType)
            {
                case "Area":
                    if (Model.Area?.Any() == true)
                    {
                        // Removes the value added by "mark all"
                        if (Model.Area.Contains(nameof(Model.Area)))
                            return $"{Model.Area.Length - 1}";
                        return $"{Model.Area.Length}";
                    }
                    return string.Empty;
                case "Category":
                    if (Model.Category?.Any() == true)
                    {
                        // Removes the value added by "mark all"
                        if (Model.Category.Contains(nameof(Model.Category)))
                            return $"{Model.Category.Length - 1}";
                        return $"{Model.Category.Length}";
                    }
                    return string.Empty;
                case "TaxonRank":
                    if (Model.TaxonRank?.Any() == true)
                    {
                        // Removes the value added by "mark all"
                        if (Model.TaxonRank.Contains(nameof(Model.TaxonRank)))
                            return $"{Model.TaxonRank.Length - 1}";
                        return $"{Model.TaxonRank.Length}";
                    }
                    return string.Empty;
                case "SpeciesGroups":
                    if (Model.SpeciesGroups?.Any() == true)
                    {
                        var count = Model.SpeciesGroups.Length;
                        if (Model.SpeciesGroups.Contains("Insekter"))
                            count--;
                        if (Model.SpeciesGroups.Contains(nameof(Model.SpeciesGroups)))
                            count--;
                        return $"{count}";
                    }
                    return string.Empty;
                case "Habitats":
                    if (Model.Habitats?.Any() == true)
                    {
                        // Removes the value added by "mark all"
                        if (Model.Habitats.Contains(nameof(Model.Habitats)))
                            return $"{Model.Habitats.Length - 1}";
                        return $"{Model.Habitats.Length}";
                    }
                    return string.Empty;
                case "Regions":
                    if (Model.Regions?.Any() == true)
                    {
                        // Removes the value added by "mark all"
                        if (Model.Regions.Contains(nameof(Model.Regions)))
                            return $"{Model.Regions.Length - 1}";
                        return $"{Model.Regions.Length}";
                    }
                    return string.Empty;
                case "EuroPop":
                    if (Model.EuroPop?.Any() == true)
                    {
                        // Removes the value added by "mark all"
                        if (Model.EuroPop.Contains(nameof(Model.EuroPop)))
                            return $"{Model.EuroPop.Length - 1}";
                        return $"{Model.EuroPop.Length}";
                    }
                    return string.Empty;
                case "Criterias":
                    if (Model.Criterias?.Any() == true)
                    {
                        // Removes the value added by "mark all"
                        if (Model.Criterias.Contains(nameof(Model.Criterias)))
                            return $"{Model.Criterias.Length - 1}";
                        return $"{Model.Criterias.Length}";
                    }
                    return string.Empty;
                case "PresumedExtinct":
                    if (Model.PresumedExtinct == true)
                        return $"1";
                    return string.Empty;
                default:
                    return string.Empty;
            }
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

        public static string GetActiveSelection(SpeciesViewModel Model)
        {
            if (!string.IsNullOrEmpty(Model.Name))
            {
                return $"{Model.Name}";
            }
            return string.Empty;
        }

        public static int GetActiveSelectionCount(SpeciesViewModel Model)
        {
            int count = 0;
            count += Model.Area.Length;
            count += Model.Category.Length;
            count += Model.SpeciesGroups.Length;
            count += Model.TaxonRank.Length;
            count += Model.Habitats.Length;
            count += Model.Regions.Length;
            count += Model.EuroPop.Length;
            count += Model.Criterias.Length;
            if (Model.PresumedExtinct)
                count++;

            return count;
        }

        public static string[] GetActiveSelectionElement(SpeciesViewModel Model)
        {
            var selectionlist = Model.Area;
            selectionlist = selectionlist.Concat(Model.Category).ToArray();
            selectionlist = selectionlist.Concat(Model.SpeciesGroups).ToArray();
            selectionlist = selectionlist.Concat(Model.TaxonRank).ToArray();
            selectionlist = selectionlist.Concat(Model.Habitats).ToArray();
            selectionlist = selectionlist.Concat(Model.Regions).ToArray();
            selectionlist = selectionlist.Concat(Model.EuroPop).ToArray();
            selectionlist = selectionlist.Concat(Model.Criterias).ToArray();
            if (Model.PresumedExtinct)
            {
                string[] PresumedExtinct = new string[] { "Antatt utdødd" };
                selectionlist = selectionlist.Concat(PresumedExtinct).ToArray();
            }
            return selectionlist;
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
    }
}
