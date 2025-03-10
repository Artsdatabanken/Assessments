using Assessments.Web.Models;
using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.AlienSpecies.Model.Enums;
using Assessments.Shared.Helpers;

namespace Assessments.Web.Infrastructure.AlienSpecies
{
    public static class QueryHelpers
    {
        public static IQueryable<AlienSpeciesAssessment2023> ApplyParameters(AlienSpeciesListParameters parameters, IQueryable<AlienSpeciesAssessment2023> query)
        {
            if (!string.IsNullOrEmpty(parameters.Name))
                query = ApplySearch(parameters.Name, query);

            if (parameters.Area.Any())
                query = query.Where(x => parameters.Area.ToEnumerable<AlienSpeciesAssessment2023EvaluationContext>().Contains(x.EvaluationContext) && x.AlienSpeciesCategory != AlienSpeciecAssessment2023AlienSpeciesCategory.RegionallyAlien); // remove "regionallyalien" assessments when filtering by evaluation context

            if (parameters.Category.Any())
                query = query.Where(x => parameters.Category.ToEnumerable<AlienSpeciesAssessment2023Category>().Contains(x.Category));

            if (parameters.EcologicalEffect.Any())
                query = query.Where(x => parameters.EcologicalEffect.Any(y => x.ScoreEcologicalEffect != AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect.Unknown && y.Contains(x.ScoreEcologicalEffect.Description())));

            if (parameters.Environment.Any())
                query = ApplyEnvironments(parameters.Environment, query);

            if (parameters.NatureTypes.Any())
                query = query.Where(x => parameters.NatureTypes.Any(y => y == x.AreaOfOccupancyInStronglyAlteredEcosystems.ToString() || x.ImpactedNatureTypes.Any(z => z.MajorTypeGroup.ToString() == y && z.IsThreatened)));

            if (parameters.InvasionPotential.Any())
                query = query.Where(x => parameters.InvasionPotential.Any(y => x.ScoreInvasionPotential != AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential.Unknown && y.Contains(x.ScoreInvasionPotential.Description())));

            if (parameters.CategoryChanged.Any())
                query = ApplyCategoryChange(parameters.CategoryChanged, query);

            if (parameters.DecisiveCriterias.Any())
                query = ApplyDecisiveCriteria(parameters.DecisiveCriterias, query);

            if (parameters.NotAssessed.Any())
                query = ApplyNotAssessed(parameters.NotAssessed, query);

            if (parameters.SpeciesStatus.Any())
                query = query.Where(x => parameters.SpeciesStatus.Contains(x.SpeciesStatus.ToString()));

            if (parameters.ProductionSpecies.Any())
                query = query.Where(x => parameters.ProductionSpecies.Contains(x.ProductionSpecies.ToString()));

            if (parameters.RegionallyAlien.Any())
                query = ApplyRegionallyAlien(parameters.RegionallyAlien, query);

            if (parameters.Regions.Any())
                query = query.Where(x => x.RegionOccurrences.Any(y => parameters.Regions.Contains(y.Region.ToString()) && (y.IsAssumedInFuture || y.IsAssumedToday || y.IsKnown)));

            if (parameters.SpeciesGroups.Any())
                query = ApplySpeciesGroups(parameters.SpeciesGroups, query);

            if (parameters.SpreadWays.Any())
                query = ApplySpreadWays(parameters.SpreadWays, query);

            if (parameters.TaxonRank.Any())
                query = ApplyTaxonRank(parameters.TaxonRank, query);

            if (parameters.GeographicVariations.Any())
                query = ApplyGeographicVariation(parameters.GeographicVariations, query);

            if (parameters.ClimateEffects.Any())
                query = ApplyClimateEffects(parameters.ClimateEffects, query);

            if (string.IsNullOrEmpty(parameters.SortBy) || parameters.SortBy.Equals(nameof(AlienSpeciesAssessment2023.ScientificName), StringComparison.InvariantCultureIgnoreCase))
                query = query.OrderBy(x => x.ScientificName.ScientificName.Replace("×", string.Empty));
            else if (parameters.SortBy.Equals(nameof(AlienSpeciesAssessment2023.VernacularName), StringComparison.InvariantCultureIgnoreCase))
                query = query.OrderBy(x => x.VernacularName);
            else if (parameters.SortBy.Equals(nameof(AlienSpeciesAssessment2023.Category), StringComparison.InvariantCultureIgnoreCase))
                query = query.OrderBy(x => x.Category);

            return query;
        }

        public static IQueryable<AlienSpeciesAssessment2023> ApplySearch(string searchString, IQueryable<AlienSpeciesAssessment2023> query)
        {
            // Searching for a specific taxonomic rank
            var rankIndexAt = searchString.IndexOf('(');
            var rankIndexEnd = searchString.IndexOf(')');
            var rank = String.Empty;
            if (rankIndexAt > 0 && rankIndexEnd > 0)
            {
                rank = searchString.Substring(rankIndexAt + 1, rankIndexEnd - 1 - rankIndexAt);
                if (rank.Length > 2)
                {
                    rank = rank[0].ToString().ToUpperInvariant() + rank[1..].ToLowerInvariant();
                }
                searchString = searchString[..rankIndexAt].Trim().ToLowerInvariant();
            }

            // If taxonomic rank is not valid, the normal search will be used instead
            if (!string.IsNullOrEmpty(rank))
            {
                if (Constants.TaxonCategoriesNbToEn.TryGetValue(rank, out string rankValue))
                {
                    if (Enum.TryParse(typeof(AlienSpeciesAssessment2023ScientificNameRank), rankValue, true, out var newRank))
                    {
                        AlienSpeciesAssessment2023ScientificNameRank rankEnum = (AlienSpeciesAssessment2023ScientificNameRank)newRank;
                        return query.Where(x => x.NameHiearchy.Any(y => y.ScientificNameRank == rankEnum && y.ScientificName.ToLowerInvariant() == searchString));
                    }
                }
            }

            // Normal search
            searchString = searchString.Trim().ToLowerInvariant();
            var containsSubSpecies = query.Where(x =>
                !string.IsNullOrEmpty(x.VernacularName) &&
                x.VernacularName.ToLowerInvariant().Contains(searchString)).Select(x => x.ScientificName.ScientificName).ToArray();

            return query.Where(x =>
                x.ScientificName.ScientificName.ToLowerInvariant().Contains(searchString) ||
                containsSubSpecies.Any(hit => x.ScientificName.ScientificName.Contains(hit)) || // Search on species also includes sub species
                !string.IsNullOrEmpty(x.VernacularName) &&
                x.VernacularName.ToLowerInvariant().Contains(searchString) ||
                x.NameHiearchy.Any(x => x.ScientificName.ToLowerInvariant().Contains(searchString)) ||
                x.SpeciesGroup.DisplayName().ToLowerInvariant().Contains(searchString));
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplyCategoryChange(string[] changes, IQueryable<AlienSpeciesAssessment2023> query)
        {
            IQueryable<AlienSpeciesAssessment2023> newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();

            foreach (var change in changes)
            {
                var assessments = change switch
                {
                    nameof(Constants.SearchAndFiltersAlienSpecies.AssessedFirstTime) => query.Where(x => (x.PreviousAssessments.Count == 0 || x.PreviousAssessments.Any(y => y.RevisionYear == 2018 && y.Category == AlienSpeciesAssessment2023Category.NR)) && x.Category != AlienSpeciesAssessment2023Category.NR),
                    nameof(Constants.SearchAndFiltersAlienSpecies.AssessedChangedCategory) => query.Where(x => x.PreviousAssessments.Count > 0 && x.PreviousAssessments.Any(y => y.RevisionYear == 2018 && y.Category != AlienSpeciesAssessment2023Category.NR && x.Category == y.Category)),
                    nameof(AlienSpeciesAssessment2023ReasonForChangeOfCategory.NewKnowledge) => query.Where(x => x.PreviousAssessments.Any(y => y.RevisionYear == 2018 && y.Category != AlienSpeciesAssessment2023Category.NR && x.Category != y.Category && x.ReasonForChangeOfCategory.Contains(AlienSpeciesAssessment2023ReasonForChangeOfCategory.NewKnowledge))),
                    nameof(AlienSpeciesAssessment2023ReasonForChangeOfCategory.NewInterpretation) => query.Where(x => x.PreviousAssessments.Any(y => y.RevisionYear == 2018 && y.Category != AlienSpeciesAssessment2023Category.NR && x.Category != y.Category && x.ReasonForChangeOfCategory.Contains(AlienSpeciesAssessment2023ReasonForChangeOfCategory.NewInterpretation))),
                    nameof(AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedGuidelines) => query.Where(x => x.PreviousAssessments.Any(y => y.RevisionYear == 2018 && y.Category != AlienSpeciesAssessment2023Category.NR && x.Category != y.Category && x.ReasonForChangeOfCategory.Contains(AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedGuidelines))),
                    nameof(AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedGuidelinesInterpretation) => query.Where(x => x.PreviousAssessments.Any(y => y.RevisionYear == 2018 && y.Category != AlienSpeciesAssessment2023Category.NR && x.Category != y.Category && x.ReasonForChangeOfCategory.Contains(AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedGuidelinesInterpretation))),
                    nameof(AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedStatus) => query.Where(x => x.PreviousAssessments.Any(y => y.RevisionYear == 2018 && y.Category != AlienSpeciesAssessment2023Category.NR && x.Category != y.Category && x.ReasonForChangeOfCategory.Contains(AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedStatus))),
                    _ => null
                };
                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplyDecisiveCriteria(string[] criterias, IQueryable<AlienSpeciesAssessment2023> query)
        {
            IQueryable<AlienSpeciesAssessment2023> newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();
            foreach (var criteria in criterias)
            {
                var assessments = criteria switch
                {
                    nameof(DeciciveCriteria.DecisiveCriteriaInvasion.A) => query.Where(x => x.DecisiveCriteria.Contains(DeciciveCriteria.DecisiveCriteriaInvasion.A.ToString())),
                    nameof(DeciciveCriteria.DecisiveCriteriaInvasion.B) => query.Where(x => x.DecisiveCriteria.Contains(DeciciveCriteria.DecisiveCriteriaInvasion.B.ToString())),
                    nameof(DeciciveCriteria.DecisiveCriteriaInvasion.AxB) => query.Where(x => DeciciveCriteria.DecisiveCriteriaInvasion.AxB.ToString().Any(y => x.DecisiveCriteria.Contains(y))),
                    nameof(DeciciveCriteria.DecisiveCriteriaInvasion.C) => query.Where(x => x.DecisiveCriteria.Contains(DeciciveCriteria.DecisiveCriteriaInvasion.C.ToString())),
                    nameof(AlienSpeciesAssessment2023CriteriaLetter.D) => query.Where(x => x.DecisiveCriteria.Contains(AlienSpeciesAssessment2023CriteriaLetter.D.ToString())),
                    nameof(DeciciveCriteria.DecisiveCriteriaEcologicalEffect.E) => query.Where(x => x.DecisiveCriteria.Contains(AlienSpeciesAssessment2023CriteriaLetter.E.ToString())),
                    nameof(DeciciveCriteria.DecisiveCriteriaEcologicalEffect.F) => query.Where(x => x.DecisiveCriteria.Contains(AlienSpeciesAssessment2023CriteriaLetter.F.ToString())),
                    nameof(DeciciveCriteria.DecisiveCriteriaEcologicalEffect.G) => query.Where(x => x.DecisiveCriteria.Contains(AlienSpeciesAssessment2023CriteriaLetter.G.ToString())),
                    nameof(DeciciveCriteria.DecisiveCriteriaEcologicalEffect.H) => query.Where(x => x.DecisiveCriteria.Contains(AlienSpeciesAssessment2023CriteriaLetter.H.ToString())),
                    nameof(DeciciveCriteria.DecisiveCriteriaEcologicalEffect.I) => query.Where(x => x.DecisiveCriteria.Contains(AlienSpeciesAssessment2023CriteriaLetter.I.ToString())),
                    _ => null
                };
                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }
 
        private static IQueryable<AlienSpeciesAssessment2023> ApplyEnvironments(string[] environments, IQueryable<AlienSpeciesAssessment2023> query)
        {
            IQueryable<AlienSpeciesAssessment2023> newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();

            foreach (var environment in environments)
            {
                var assessments = environment switch
                {
                    Environments.Marine => query.Where(x => x.Environment == AlienSpeciesAssessment2023Environment.Marint || x.Environment == AlienSpeciesAssessment2023Environment.LimMar || x.Environment == AlienSpeciesAssessment2023Environment.MarTer || x.Environment == AlienSpeciesAssessment2023Environment.LimMarTer),
                    Environments.Limnic => query.Where(x => x.Environment == AlienSpeciesAssessment2023Environment.Limnisk || x.Environment == AlienSpeciesAssessment2023Environment.LimMar || x.Environment == AlienSpeciesAssessment2023Environment.LimTer || x.Environment == AlienSpeciesAssessment2023Environment.LimMarTer),
                    Environments.Terrestrial => query.Where(x => x.Environment == AlienSpeciesAssessment2023Environment.Terrestrisk || x.Environment == AlienSpeciesAssessment2023Environment.LimTer || x.Environment == AlienSpeciesAssessment2023Environment.MarTer || x.Environment == AlienSpeciesAssessment2023Environment.LimMarTer),
                    _ => null
                };
                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplyNotAssessed(string[] assessedType, IQueryable<AlienSpeciesAssessment2023> query)
        {
            IQueryable<AlienSpeciesAssessment2023> newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();
            foreach (var type in assessedType)
            {
                var assessments = type switch
                {
                    nameof(NotAssessed.NotAssessedAlienSpeciesCategory.NotAlienSpecies) => query.Where(x => x.AlienSpeciesCategory == AlienSpeciecAssessment2023AlienSpeciesCategory.NotAlienSpecie),
                    nameof(NotAssessed.NotAssessedAlienSpeciesCategory.UncertainBefore1800) => query.Where(x => x.AlienSpeciesCategory == AlienSpeciecAssessment2023AlienSpeciesCategory.UncertainBefore1800),
                    nameof(NotAssessed.NotAssessedAlienSpeciesCategory.MisIdentified) => query.Where(x => x.AlienSpeciesCategory == AlienSpeciecAssessment2023AlienSpeciesCategory.MisIdentified),
                    nameof(NotAssessed.NotAssessedAlienSpeciesCategory.HorizonScanned) => query.Where(x => x.AlienSpeciesCategory == AlienSpeciecAssessment2023AlienSpeciesCategory.HorizonScannedButNoRiskAssessment),
                    _ => null
                };
                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplyTaxonRank(string[] rankFilters, IQueryable<AlienSpeciesAssessment2023> query)
        {
            var newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();
            var evaluatedAtAnotherLevel = AlienSpeciecAssessment2023AlienSpeciesCategory.TaxonEvaluatedAtAnotherLevel;

            foreach (var filter in rankFilters)
            {
                var assessments = filter switch
                {
                    nameof(AlienSpeciesAssessment2023ScientificNameRank.AssessedAtAnotherRank) => query.Where(x => x.AlienSpeciesCategory == evaluatedAtAnotherLevel),
                    nameof(AlienSpeciesAssessment2023ScientificNameRank.AssessedAtSameRank) => query.Where(x => x.AlienSpeciesCategory != evaluatedAtAnotherLevel),
                    nameof(AlienSpeciesAssessment2023ScientificNameRank.Hybrid) => query.Where(x => x.ScientificName.ScientificName.Contains('×')),
                    _ => query.Where(x => x.ScientificName.ScientificNameRank.ToString() == filter)
                };
                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplyGeographicVariation(string[] geographicVariations, IQueryable<AlienSpeciesAssessment2023> query)
        {
            var newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();

            foreach (var variation in geographicVariations)
            {
                var assessments = variation switch
                {
                    nameof(Constants.SearchAndFiltersAlienSpecies.GeographicVariationNotEqual) => query.Where(x => x.GeographicVariationInCategory == true),
                    nameof(Constants.SearchAndFiltersAlienSpecies.GeographicVariationEqual) => query.Where(x => x.GeographicVariationInCategory == false),
                    _ => null
                };

                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery;
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplyClimateEffects(string[] climateEffects, IQueryable<AlienSpeciesAssessment2023> query)
        {
            var newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();

            foreach (var effect in climateEffects)
            {
                var assessments = effect switch
                {
                    nameof(Constants.SearchAndFiltersAlienSpecies.ClimateInvasionPotentialAffected) => query.Where(x => x.ClimateEffectsInvasionpotential == true),
                    nameof(Constants.SearchAndFiltersAlienSpecies.ClimateEcologicalEffectAffected) => query.Where(x => x.ClimateEffectsEcoEffect == true),
                    nameof(Constants.SearchAndFiltersAlienSpecies.ClimateInvasionPotentialNotAffected) => query.Where(x => x.ClimateEffectsInvasionpotential == false),
                    nameof(Constants.SearchAndFiltersAlienSpecies.ClimateEcologicalEffectNotAffected) => query.Where(x => x.ClimateEffectsEcoEffect == false),
                    _ => null
                };

                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplySpreadWays(string[] spreadWays, IQueryable<AlienSpeciesAssessment2023> query)
        {
            var newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();

            foreach (var way in spreadWays)
            {
                var assessments = way switch
                {
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.ImportDirect) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.NotChosen) => query.Where(x => x.ImportPathways.Any(y => y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.ImportDirect)),
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Transportpolution) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.NotChosen) => query.Where(x => x.ImportPathways.Any(y => y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Transportpolution)),
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Stowaway) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.NotChosen) => query.Where(x => x.ImportPathways.Any(y => y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Stowaway)),
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Released) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread) => query.Where(x => x.IntroductionAndSpreadPathways.Any(y => y.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread && y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Released)),
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Transportpolution) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread) => query.Where(x => x.IntroductionAndSpreadPathways.Any(y => y.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread && y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Transportpolution)),
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Stowaway) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread) => query.Where(x => x.IntroductionAndSpreadPathways.Any(y => y.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread && y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Stowaway)),
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Corridor) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread) => query.Where(x => x.IntroductionAndSpreadPathways.Any(y => y.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread && y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Corridor)),
                    nameof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory.NaturalDispersal) + nameof(AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread) => query.Where(x => x.IntroductionAndSpreadPathways.Any(y => y.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Spread && y.MainCategory == AlienSpeciesAssessment2023IntroductionPathway.MainCategory.NaturalDispersal)),
                    _ => query.Where(x => x.IntroductionAndSpreadPathways.Any(y => y.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Introduction && y.MainCategory.ToString() == way))
                };

                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplyRegionallyAlien(string[] regionFilters, IQueryable<AlienSpeciesAssessment2023> query)
        {
            var newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();

            foreach (var regionFilter in regionFilters)
            {
                IQueryable<AlienSpeciesAssessment2023> assessments;
                // ExcludeRegionallyAlien and OnlyRegionallyAlien are not regions, so they are treated differently
                if (regionFilter != nameof(Constants.SearchAndFiltersAlienSpecies.ExcludeRegionallyAlien) && regionFilter != nameof(Constants.SearchAndFiltersAlienSpecies.OnlyRegionallyAlien) && Enum.TryParse(regionFilter, out RegionallyAlien.RegionallyAlienEnum filterEnum))
                {
                    assessments = query.Where(x => x.FreshWaterRegionModel.FreshWaterRegions.Any(y => y.WaterRegionName == filterEnum.DisplayName() && y.IsIncludedInAssessmentArea == true && (y.IsKnown || y.IsAssumedToday || y.IsAssumedInFuture)));
                }
                else
                {
                    assessments = regionFilter switch
                    {
                        nameof(Constants.SearchAndFiltersAlienSpecies.ExcludeRegionallyAlien) => query.Where(x => x.AlienSpeciesCategory != AlienSpeciecAssessment2023AlienSpeciesCategory.RegionallyAlien && x.AlienSpeciesCategory != AlienSpeciecAssessment2023AlienSpeciesCategory.RegionallyAlienEstablishedBefore1800),
                        nameof(Constants.SearchAndFiltersAlienSpecies.OnlyRegionallyAlien) => query.Where(x => x.AlienSpeciesCategory == AlienSpeciecAssessment2023AlienSpeciesCategory.RegionallyAlien || x.AlienSpeciesCategory == AlienSpeciecAssessment2023AlienSpeciesCategory.RegionallyAlienEstablishedBefore1800),
                        _ => null
                    };
                }

                if (assessments != null)
                    newQuery = newQuery.Concat(assessments);
            }
            return newQuery.Distinct();
        }

        private static IQueryable<AlienSpeciesAssessment2023> ApplySpeciesGroups(string[] speciesGroupFilters, IQueryable<AlienSpeciesAssessment2023> query)
        {
            var newQuery = Enumerable.Empty<AlienSpeciesAssessment2023>().AsQueryable();

            foreach(var filter in speciesGroupFilters)
            {
                // the trhee letter strings represents collections of species groups 
                newQuery =  newQuery.Concat(filter switch
                {
                    "skd" => query.Where(x => x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Ascidiacea || x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Tunicata),
                    "smb" => query.Where(x => x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Ectoprocta || x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Entoprocta),
                    "smo" => query.Where(x => x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Bryophyta || x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Marchantiophyta),
                    "smf" => query.Where(x => x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Myriapoda || x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Chilopoda || x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Diplopoda),
                    "skp" => query.Where(x => x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Magnoliophyta || x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Pinophyta || x.SpeciesGroup == AlienSpeciesAssessment2023SpeciesGroups.Pteridophyta),
                    _ => query.Where(x => filter == x.SpeciesGroup.ToString())
                });
            }

            return newQuery;
        }
    }
}
