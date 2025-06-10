using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.AlienSpecies.Model.Enums;
using Assessments.Mapping.AlienSpecies.Source;
using Assessments.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Assessments.Mapping.AlienSpecies.Model.Enums.AlienSpeciesAssessment2023IntroductionPathway;

namespace Assessments.Mapping.AlienSpecies.Helpers
{
    public static class AlienSpeciesAssessment2023ProfileHelper
    {
        private static readonly PropertyInfo[] riskAssessmentProperties = typeof(RiskAssessment).GetProperties();
        private static readonly PropertyInfo[] riskAssessmentPropertiesFirstObservations = riskAssessmentProperties.Where(x => x.Name.StartsWith("YearFirst") && !x.Name.Contains("Insecure") && !x.Name.Contains("Domestic")).ToArray();

        internal static string GetAlienSpeciesCategory(string alienSpeciesCategory, string expertGroup)
        {
            if (alienSpeciesCategory == "RegionallyAlien" && expertGroup != "Fisker")
            {
                return "AlienSpecie";
            }
            return alienSpeciesCategory;
        }

        internal static AlienSpeciesAssessment2023ChangedFromAlien GetAlienSpeciesAssessment2023Changed(string changedFrom)
        {
            return changedFrom switch
            {
                "wasThoughtToBeAlien" => AlienSpeciesAssessment2023ChangedFromAlien.WasThoughtToBeAlien,
                "wasAlienButEstablishedNow" => AlienSpeciesAssessment2023ChangedFromAlien.WasAlienButEstablishedNow,
                _ => AlienSpeciesAssessment2023ChangedFromAlien.Unknown
            };
        }

        internal static AlienSpeciesAssessment2023Environment GetEnvironmentEnum(bool limnic, bool marine, bool terrestrial)
        {
            var value = 0;
            if (limnic)
                value += 1;
            if (marine)
                value += 2;
            if (terrestrial)
                value += 4;

            return value switch
            {
                1 => AlienSpeciesAssessment2023Environment.Limnisk,
                2 => AlienSpeciesAssessment2023Environment.Marint,
                3 => AlienSpeciesAssessment2023Environment.LimMar,
                4 => AlienSpeciesAssessment2023Environment.Terrestrisk,
                5 => AlienSpeciesAssessment2023Environment.LimTer,
                6 => AlienSpeciesAssessment2023Environment.MarTer,
                7 => AlienSpeciesAssessment2023Environment.LimMarTer,
                _ => AlienSpeciesAssessment2023Environment.Unknown
            };
        }

        internal static string GetExpertGroup(string expertGroup)
        {
            // NOTE: synchronise logic with ExpertGroupExport in TransformAlienSpecies

            if (expertGroup.Contains("(Svalbard)"))
            {
                return expertGroup.Replace("(Svalbard)", "");
            }

            if (expertGroup is "Bakterier" or "Kromister" or "Sopper")
            {
                return "Sopper, det gule riket og bakterier";
            }

            return expertGroup;
        }

        internal static string GetEstablishmentCategory(string speciesEstablishmentCategory, string speciesStatus, string alienSpeciesCategory)
        {
            bool isExcludedAlienSpeciesCategory = alienSpeciesCategory is "HorizonScannedButNoRiskAssessment" or "NotAlienSpecie";

            if (isExcludedAlienSpeciesCategory || string.IsNullOrEmpty(speciesStatus))
            {
                return string.Empty;
            }
            
            return speciesStatus is "C3" && speciesEstablishmentCategory is not null ? speciesEstablishmentCategory : speciesStatus;
        }

        internal static bool GetHasIndoorProduction(string indoorProductionString)
        {
            if (string.IsNullOrEmpty(indoorProductionString))
                return false;
            if (indoorProductionString.Equals("negative"))
                return true;
            return false;
        }

        internal static List<AlienSpeciesAssessment2023Pathways> GetIntroductionPathways(List<MigrationPathway> assessmentVectors)
        {
            InfluenceFactor GetInfluenceFactor(string influenceFactor)
            {
                return influenceFactor switch
                {
                    "unknown" => AlienSpeciesAssessment2023IntroductionPathway.InfluenceFactor.Unknown,
                    "numerousYearly" => AlienSpeciesAssessment2023IntroductionPathway.InfluenceFactor.NumerousYearly,
                    "yearly" => AlienSpeciesAssessment2023IntroductionPathway.InfluenceFactor.Yearly,
                    "severalPr10years" => AlienSpeciesAssessment2023IntroductionPathway.InfluenceFactor.SeveralPr10years,
                    "rarerThan10years" => AlienSpeciesAssessment2023IntroductionPathway.InfluenceFactor.RarerThan10years,
                    _ => AlienSpeciesAssessment2023IntroductionPathway.InfluenceFactor.NotChosen
                };
            }

            Magnitude GetMagnitude(string magnitude)
            {
                return magnitude switch
                {
                    "unknown" => AlienSpeciesAssessment2023IntroductionPathway.Magnitude.Unknown,
                    "1" => AlienSpeciesAssessment2023IntroductionPathway.Magnitude.Smallest,
                    "2-10" => AlienSpeciesAssessment2023IntroductionPathway.Magnitude.Small,
                    "11-100" => AlienSpeciesAssessment2023IntroductionPathway.Magnitude.Medium,
                    "101-1000" => AlienSpeciesAssessment2023IntroductionPathway.Magnitude.Large,
                    "moreThan1000" => AlienSpeciesAssessment2023IntroductionPathway.Magnitude.MoreThan1000,
                    _ => AlienSpeciesAssessment2023IntroductionPathway.Magnitude.NotChosen
                };
            }

            TimeOfIncident GetTimeOfIncident(string timeOfIncident)
            {
                return timeOfIncident switch
                {
                    "unknown" => AlienSpeciesAssessment2023IntroductionPathway.TimeOfIncident.Unknown,
                    "historic" => AlienSpeciesAssessment2023IntroductionPathway.TimeOfIncident.Historic,
                    "ceased" => AlienSpeciesAssessment2023IntroductionPathway.TimeOfIncident.Ceased,
                    "ongoing" => AlienSpeciesAssessment2023IntroductionPathway.TimeOfIncident.Ongoing,
                    "future" => AlienSpeciesAssessment2023IntroductionPathway.TimeOfIncident.Future,
                    _ => AlienSpeciesAssessment2023IntroductionPathway.TimeOfIncident.NotChosen
                };
            }

            MainCategory GetMainCategory(string mainCategory)
            {
                return mainCategory switch
                {
                    "R\u00F8mning/forvilling" => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Escaped,
                    "Blindpassasjer med transport" => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Stowaway,
                    "Korridor" => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Corridor,
                    "Tilsiktet utsetting" => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Released,
                    "Egenspredning" => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.NaturalDispersal,
                    "Forurensning av vare" => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Transportpolution,
                    "Direkte import" => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.ImportDirect,
                    _ => AlienSpeciesAssessment2023IntroductionPathway.MainCategory.Unknown
                };
            }

            List<AlienSpeciesAssessment2023Pathways> filteredAssessmentVectors = assessmentVectors.Select(x => new AlienSpeciesAssessment2023Pathways()
            {
                IntroductionSpread = (IntroductionSpread)Enum.Parse(typeof(IntroductionSpread), string.IsNullOrEmpty(x.IntroductionSpread) ? "NotChosen" : x.IntroductionSpread, true),
                InfluenceFactor = GetInfluenceFactor(x.InfluenceFactor),
                Magnitude = GetMagnitude(x.Magnitude),
                TimeOfIncident = GetTimeOfIncident(x.TimeOfIncident),
                Category = x.Category,
                MainCategory = GetMainCategory(x.MainCategory)
            }).ToList();

            return filteredAssessmentVectors;
        }

        internal static int? GetScores(string category, string criteria, string axis)
        {
            if (string.IsNullOrEmpty(category) || category is "NR")
            {
                return null;
            }
            else
            {
                int scoreInvationAxis = (int)Char.GetNumericValue(criteria[0]);
                string criteriaEcoEffectAxis = criteria.Split(",")[1];
                int scoreEcoEffectAxis = (int)Char.GetNumericValue(criteriaEcoEffectAxis[0]);
                return axis == "inv" ? scoreInvationAxis : scoreEcoEffectAxis;
            }
        }

        internal static AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect GetScoreEcologicalEffect(string category, string criteria)
        {
            if (string.IsNullOrEmpty(category) || category is "NR")
            {
                return AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect.Unknown;
            }
            var axis = criteria.Split(",")[1];
            var score = (int)Char.GetNumericValue(axis[0]);

            return score switch
            {
                1 => AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect.NotKnown,
                2 => AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect.Small,
                3 => AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect.Medium,
                4 => AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect.Great,
                _ => AlienSpeciesAssessment2023MatrixAxisScore.EcologicalEffect.Unknown
            };
        }

        internal static AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential GetScoreInvasionPotential(string category, string criteria)
        {
            if (string.IsNullOrEmpty(category) || category is "NR")
            {
                return AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential.Unknown;
            }
            var axis = criteria.Split(",")[0];
            var score = (int)Char.GetNumericValue(axis[0]);

            return score switch
            {
                1 => AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential.Small,
                2 => AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential.Limited,
                3 => AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential.Moderate,
                4 => AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential.Great,
                _ => AlienSpeciesAssessment2023MatrixAxisScore.InvasionPotential.Unknown
            };
        }

        internal static bool? GetGeographicVarInCat(string category, string geographicVar)
        {
            if (category is "NR")
            {
                return null;
            }
            if (string.IsNullOrEmpty(geographicVar))
            {
                return false;
            }
            return geographicVar == "yes";
        }

        internal static List<AlienSpeciesAssessment2023GeographicalVariation> GetGeographicVarCause(string category, string geographicVar, List<string> geographicalVariation, AlienSpeciesAssessment2023Environment environment)
        {
            if (GetGeographicVarInCat(category, geographicVar) is null or false)
            {
                return new List<AlienSpeciesAssessment2023GeographicalVariation>();
            }

            var isMarine = environment == AlienSpeciesAssessment2023Environment.Limnisk || environment == AlienSpeciesAssessment2023Environment.Marint || environment == AlienSpeciesAssessment2023Environment.LimMar || environment == AlienSpeciesAssessment2023Environment.LimTer || environment == AlienSpeciesAssessment2023Environment.MarTer || environment == AlienSpeciesAssessment2023Environment.LimMarTer;
            var geographicalVariationsEnumList = new List<AlienSpeciesAssessment2023GeographicalVariation>();
            Object current;

            foreach (var variation in geographicalVariation)
            {
                if (isMarine)
                {
                    if (Enum.TryParse(typeof(AlienSpeciesAssessment2023GeographicalVariation), $"{variation.TrimEnd()}Marine", true, out current))
                    {
                        geographicalVariationsEnumList.Add((AlienSpeciesAssessment2023GeographicalVariation)current);
                    }
                }
                else
                {
                    if (Enum.TryParse(typeof(AlienSpeciesAssessment2023GeographicalVariation), variation.TrimEnd(), true, out current))
                    {
                        geographicalVariationsEnumList.Add((AlienSpeciesAssessment2023GeographicalVariation)current);
                    }
                }
            }
            return geographicalVariationsEnumList;
        }

        internal static string GetGeographicVarDoc(string category, string geographicVar, string geoVarDoc)
        {
            if (GetGeographicVarInCat(category, geographicVar) is null or false)
            {
                return string.Empty;
            }

            return geoVarDoc;
        }

        internal static bool? GetClimateEffects(string category, string criteria, string axis, RiskAssessment ra)
        {
            if (category is "NR")
            {
                return null;
            }
            string climateEffect = axis == "inv" ? ra.ClimateEffectsInvationpotential : ra.ClimateEffectsEcoEffect;

            if (string.IsNullOrEmpty(climateEffect) || GetScores(category, criteria, axis) == 1)
            {
                return false;
            }
            return climateEffect == "yes";
        }

        internal static string GetClimateEffectsDoc(string category, string criteria, RiskAssessment ra, string climateDoc)
        {
            if (GetClimateEffects(category, criteria, "inv", ra) is null or false && GetClimateEffects(category, criteria, "eco", ra) is null or false)
            {
                return string.Empty;
            }

            return climateDoc.StripUnwantedHtml();
        }

        internal static AlienSpeciesAssessment2023SpeciesGroups GetSpeciesGroup(string taxonHierarchy)
        {
            // Reversing the list to get a match as low as possible in the taxon hierarchy.
            var scientificNames = taxonHierarchy.Split("/").Reverse();
            foreach (var name in scientificNames)
            {
                if (Enum.TryParse(name, out AlienSpeciesAssessment2023SpeciesGroups speciesGroup))
                {
                    return speciesGroup;
                }
            }
            return AlienSpeciesAssessment2023SpeciesGroups.Unknown;
        }

        internal static int GetTaxonRank(string taxonRank)
        {
            // 0 maps to "Ukjent" (unknown) in the view. All assessments should have a taxonomic rank, but at this time not all do.
            // 22 through 24 corresponds to species, sub species, and variety, respectively. 
            // These numbers might seem mysterious, but I can assure you, they are not. 
            // The numbers are used in multiple repositories, and corresponds to the correct taxon rank, of which there are about 25. 
            var isParsable = int.TryParse(taxonRank, out var result);
            if (!isParsable && taxonRank == "Species")
                result = 22;

            return result;
        }

        internal static List<AlienSpeciesAssessment2023PreviousAssessment> GetPreviousAssessments(List<AlienSpeciesAssessment2023PreviousAssessment> previousAssessments)
        {
            foreach (var previousAssessment in previousAssessments)
            {
                if (previousAssessment.RevisionYear == 2018)
                {
                    previousAssessment.Url = $"https://artsdatabanken.no/fremmedarter/2018/{previousAssessment.AssessmentId.Replace("FA3", string.Empty)}";

                    // https://github.com/Artsdatabanken/Fremmedartsbase2023/blob/65212142a2dbb29317a0cc94bfde26688ab1bc67/Prod.Api/Helpers/ExportMapper.cs#L31-L72
                    if (previousAssessment.MainCategory == "NotApplicable" || (previousAssessment.MainCategory == "DoorKnocker" && previousAssessment.MainSubCategory == "noRiskAssessment") || (previousAssessment.MainCategory == "RegionallyAlien" && previousAssessment.MainSubCategory == "noRiskAssessment"))
                    {
                        previousAssessment.Category = AlienSpeciesAssessment2023Category.NR;
                    }
                    else
                    {
                        previousAssessment.Category = previousAssessment.RiskLevel switch
                        {
                            0 => AlienSpeciesAssessment2023Category.NK,
                            1 => AlienSpeciesAssessment2023Category.LO,
                            2 => AlienSpeciesAssessment2023Category.PH,
                            3 => AlienSpeciesAssessment2023Category.HI,
                            4 => AlienSpeciesAssessment2023Category.SE,
                            _ => AlienSpeciesAssessment2023Category.NR
                        };
                    }
                }
                else
                {
                    if (previousAssessment.MainSubCategory == "noRiskAssessment")
                    {
                        previousAssessment.Category = AlienSpeciesAssessment2023Category.NR;
                    }
                    else
                    {
                        previousAssessment.Category = previousAssessment.RiskLevel switch
                        {
                            0 => AlienSpeciesAssessment2023Category.NK,
                            1 => AlienSpeciesAssessment2023Category.LO,
                            2 => AlienSpeciesAssessment2023Category.PH,
                            3 => AlienSpeciesAssessment2023Category.HI,
                            4 => AlienSpeciesAssessment2023Category.SE,
                            _ => AlienSpeciesAssessment2023Category.NR
                        };
                    }

                    previousAssessment.Url = !previousAssessment.AssessmentId.Contains(':')
                        ? "https://databank.artsdatabanken.no/FremmedArt2012"
                        : $"https://databank.artsdatabanken.no/FremmedArt2012/{previousAssessment.AssessmentId.Split(":")[1]}";


                }
            }

            return previousAssessments;
        }

        private static readonly Dictionary<int, int> introLowTable = new()
        {
            { 1, 1 },
            { 5, 2 },
            { 13, 3 },
            { 26, 4 },
            { 43, 5 },
            { 65, 6 },
            { 91, 7 },
            { 121, 8 },
            { 156, 9 },
            { 195, 10 }
        };

        private static readonly Dictionary<int, int> introHighTable = new()
        {
            { 1, 1 },
            { 6, 2 },
            { 15, 3 },
            { 29, 4 },
            { 47, 5 },
            { 69, 6 },
            { 96, 7 },
            { 127, 8 },
            { 163, 9 },
            { 204, 10 }
        };

        private static int IntroductionNum(Dictionary<int, int> table, long? best)
        {
            var keys = table.Keys.Reverse();
            foreach (var key in keys)
            {
                if (best >= key)
                    return table[key];
            }
            return 0;
        }

        internal static int IntroductionsLow(RiskAssessment ra)
        {
            int num = IntroductionNum(introLowTable, ra.IntroductionsBest);
            return (int)(num == 0 ? 0 : ra.IntroductionsBest - num);
        }

        internal static int IntroductionsHigh(RiskAssessment ra)
        {
            int num = IntroductionNum(introHighTable, ra.IntroductionsBest);
            return (int)(num == 0 ? 0 : ra.IntroductionsBest + num);
        }

        private static long? AOO10yr(long? occurrences1, long? introductions)
        {
            if (introductions.HasValue == false || occurrences1.HasValue == false)
            {
                return null;
            }
            var occ = occurrences1.Value;
            var intr = introductions.Value;
            if (occ is 0 && intr is 0)
            {
                return 0;
            }
            else return occ is 0 ? (long)(4 * Math.Round(0.64 + 0.36 * intr, 0)) : (long)(4 * Math.Round(occ + Math.Pow(intr, ((double)occ + 9) / 10)));
        }

        internal static int GetAOOfuture(FA4 assessment, RiskAssessment riskAssessment, string estimateQuantile)
        {
            if (assessment.AssessmentConclusion == "WillNotBeRiskAssessed")
            {
                return 0;
            }
            //TODO: use ra.Occurrences1Low/Best/High without asking for HasValue (??) when all assessments are ready before innsynet (should not be any null for doorknockers at that point..)
            long? areaOfOccurrenceIn50Years;
            long? numberOfOccurrences;
            int numberOfIntroductions;
            if (estimateQuantile == "low")
            {
                areaOfOccurrenceIn50Years = riskAssessment.AOO50yrLowInput;
                numberOfOccurrences = riskAssessment.Occurrences1Low ?? 0;
                numberOfIntroductions = IntroductionsLow(riskAssessment);

            }
            else if (estimateQuantile == "best")
            {
                areaOfOccurrenceIn50Years = riskAssessment.AOO50yrBestInput;
                numberOfOccurrences = riskAssessment.Occurrences1Best ?? 0;
                numberOfIntroductions = (int?)riskAssessment.IntroductionsBest ?? 0;
            }
            else
            {
                areaOfOccurrenceIn50Years = riskAssessment.AOO50yrHighInput;
                numberOfOccurrences = riskAssessment.Occurrences1High ?? 0;
                numberOfIntroductions = IntroductionsHigh(riskAssessment);
            }

            var norway = "N";
            var assessedSelfReproducing = "AssessedSelfReproducing";
            var value = 0;
            if (assessment.Limnic)
                value += 1;
            if (assessment.Marine)
                value += 2;
            if (assessment.Terrestrial)
                value += 4;

            switch (value)
            {
                case 1:
                    if (assessment.EvaluationContext == norway)
                    {
                        return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(22000, (long)areaOfOccurrenceIn50Years) : Math.Min(22000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                    }
                    return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(500, (long)areaOfOccurrenceIn50Years) : Math.Min(500, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                case 2:
                    if (assessment.EvaluationContext == norway)
                    {
                        return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(934000, (long)areaOfOccurrenceIn50Years) : Math.Min(934000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                    }
                    return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(1099000, (long)areaOfOccurrenceIn50Years) : Math.Min(1099000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                case 3:
                    if (assessment.EvaluationContext == norway)
                    {
                        return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(956000, (long)areaOfOccurrenceIn50Years) : Math.Min(956000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                    }
                    return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(1099500, (long)areaOfOccurrenceIn50Years) : Math.Min(1099500, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                case 4:
                    if (assessment.EvaluationContext == norway)
                    {
                        return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(310000, (long)areaOfOccurrenceIn50Years) : Math.Min(310000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                    }
                    return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(24000, (long)areaOfOccurrenceIn50Years) : Math.Min(24000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                case 5:
                    if (assessment.EvaluationContext == norway)
                    {
                        return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(332000, (long)areaOfOccurrenceIn50Years) : Math.Min(332000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                    }
                    return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(24500, (long)areaOfOccurrenceIn50Years) : Math.Min(24500, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                case 6:
                    if (assessment.EvaluationContext == norway)
                    {
                        return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(1244000, (long)areaOfOccurrenceIn50Years) : Math.Min(1244000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                    }
                    return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(1123000, (long)areaOfOccurrenceIn50Years) : Math.Min(1123000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                case 7:
                    if (assessment.EvaluationContext == norway)
                    {
                        return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(1266000, (long)areaOfOccurrenceIn50Years) : Math.Min(1266000, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                    }
                    return (int)(assessment.AssessmentConclusion == assessedSelfReproducing ? Math.Min(1123500, (long)areaOfOccurrenceIn50Years) : Math.Min(1123500, (long)AOO10yr(numberOfOccurrences, numberOfIntroductions)));
                default:
                    return 0;
            }
        }

        internal static string GetExtinctionProbability(List<RiskAssessment.Criterion> aCriterionScore)
        {
            switch (aCriterionScore[0].Value)
            {
                case 0:
                    return "High";
                case 1:
                    return "MediumHigh";
                case 2:
                    return "MediumLow";
                case 3:
                    return "Low";
                default:
                    break;
            }
            return "NotEvaluated";
        }

        public static int GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(string assessmentConclusion, RiskAssessment riskAssessment)
        {
            var assessedDoorKnocker = "AssessedDoorknocker";
            if (assessmentConclusion == assessedDoorKnocker)
            {
                var numberOfOccurrences = riskAssessment.Occurrences1Best ?? 0;
                var numberOfIntroductions = (int?)riskAssessment.IntroductionsBest ?? 0;
                var AOOTenYearsBest = AOO10yr(numberOfOccurrences, numberOfIntroductions);

                return AOOTenYearsBest > 16 ? 4
                    : AOOTenYearsBest > 4 ? 3
                    : AOOTenYearsBest > 1 ? 2
                    : 1;
            }
            else
            {
                if (riskAssessment.AOO50yrBestInput.HasValue == false || riskAssessment.AOOtotalBestInput.HasValue == false) return 0;
                double AOOFiftyYearsBest = (double)riskAssessment.AOO50yrBestInput;
                double AOOtotalBest = (double)riskAssessment.AOOtotalBestInput;
                double AOOChangeBest = AOOtotalBest == 0 ? 1 : (double)(AOOFiftyYearsBest / AOOtotalBest);

                return AOOFiftyYearsBest >= 20 && AOOChangeBest > 0.2 ? 4
                    : AOOFiftyYearsBest >= 20 && AOOChangeBest > 0.05 ? 3
                    : AOOFiftyYearsBest >= 8 && AOOChangeBest > 0.2 ? 3
                    : AOOFiftyYearsBest >= 4 ? 2
                    : 1;
            }


        }

        public static int GetMedianLifetimeSimplifiedEstimationDefaultScoreUncertainty(string assessmentConclusion, RiskAssessment riskAssessment, string estimateQuantile)
        {
            var assessedDoorKnocker = "AssessedDoorknocker";
            if (estimateQuantile == "Low")
            {
                if (assessmentConclusion == assessedDoorKnocker)
                {
                    var numberOfOccurrences = riskAssessment.Occurrences1Low ?? 0;
                    int numberOfIntroductions = IntroductionsLow(riskAssessment);
                    var AOOTenYearsLow = AOO10yr(numberOfOccurrences, numberOfIntroductions);

                    return AOOTenYearsLow > 16 ? 4
                        : AOOTenYearsLow > 4 ? 3
                        : AOOTenYearsLow > 1 ? Math.Max(2, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) - 1)
                        : AOOTenYearsLow <= 1 ? Math.Max(1, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) - 1)
                        : 1;
                }
                else
                {
                    if (riskAssessment.AOO50yrLowInput.HasValue == false || riskAssessment.AOOtotalBestInput.HasValue == false) return 0;
                    double AOOFiftyYearsLow = (double)riskAssessment.AOO50yrLowInput;
                    double AOOtotalBest = (double)riskAssessment.AOOtotalBestInput;
                    double AOOChangeLow = AOOtotalBest == 0 ? 1 : (double)(AOOFiftyYearsLow / AOOtotalBest);

                    return AOOFiftyYearsLow >= 20 && AOOChangeLow > 0.2 ? 4
                        : AOOFiftyYearsLow >= 20 && AOOChangeLow > 0.05 ? 3
                        : AOOFiftyYearsLow >= 8 && AOOChangeLow > 0.2 ? 3
                        : AOOFiftyYearsLow >= 4 ? Math.Max(2, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) - 1)
                        : AOOFiftyYearsLow < 4 ? Math.Max(1, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) - 1)
                        : 1;
                }
            }
            else //estimateQuantile == "High"
            {
                if (assessmentConclusion == assessedDoorKnocker)
                {
                    var numberOfOccurrences = riskAssessment.Occurrences1High ?? 0;
                    int numberOfIntroductions = IntroductionsHigh(riskAssessment);
                    var AOOTenYearsHigh = AOO10yr(numberOfOccurrences, numberOfIntroductions);

                    return AOOTenYearsHigh > 16 ? Math.Min(4, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) + 1)
                        : AOOTenYearsHigh > 4 ? Math.Min(3, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) + 1)
                        : AOOTenYearsHigh > 1 ? 2
                        : 1;
                }
                else
                {
                    if (riskAssessment.AOO50yrHighInput.HasValue == false || riskAssessment.AOOtotalBestInput.HasValue == false) return 0;
                    double AOOFiftyYearsHigh = (double)riskAssessment.AOO50yrHighInput;
                    double AOOtotalBest = (double)riskAssessment.AOOtotalBestInput;
                    double AOOChangeHigh = AOOtotalBest == 0 ? 1 : (double)(AOOFiftyYearsHigh / AOOtotalBest);

                    return AOOFiftyYearsHigh >= 20 && AOOChangeHigh > 0.2 ? Math.Min(4, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) + 1)
                        : AOOFiftyYearsHigh >= 20 && AOOChangeHigh > 0.05 ? Math.Min(3, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) + 1)
                        : AOOFiftyYearsHigh >= 8 && AOOChangeHigh > 0.2 ? Math.Min(3, GetMedianLifetimeSimplifiedEstimationDefaultScoreBest(assessmentConclusion, riskAssessment) + 1)
                        : AOOFiftyYearsHigh >= 4 ? 2
                        : 1;
                }
            }

        }
        internal static string GetSpatioTemporalDatasetModel(string model)
        {
            return model switch
            {
                "1" => "ConstantDetectability",
                "2" => "ChangeDetectabilityOnce",
                "3" => "TestTwoModels",
                "4" => "KnownSamplingEffort",
                _ => "NotRelevant",
            };
        }

        internal static string GetExpansionEstimationMethod(string category, string chosenMainMethod, string assessmentConclusion, string chosenSubMethod)
        {
            if (category == "NR")
            {
                return "NotRelevant";
            }

            var mainMethodA = chosenMainMethod == "a";
            var mainMethodB = chosenMainMethod == "b";
            var assessedDoorKnocker = assessmentConclusion == "AssessedDoorknocker";

            if (mainMethodA)
            {
                return "SpatioTemporalDataset";
            }

            if (mainMethodB && assessedDoorKnocker)
            {
                return "EstimatedIncreaseInAOODoorKnockers";
            }

            if (mainMethodB && chosenSubMethod == "yes")
            {
                return "EstimatedIncreaseInAOOReproducingUnaided";
            }

            if (mainMethodB && chosenSubMethod == "no")
            {
                return "EstimatedIncreaseInAOOReproducingUnaidedFutureExpansion";
            }

            else return "NotRelevant";

        }

        static private long GetExpansionSpeedAOOSelfReproducing(RiskAssessment riskAssessment, long areaOfOccurrenceToday, long areaOfOccurrenceIn50Years)
        {
            bool chosenSubMethod = riskAssessment.AOOfirstOccurenceLessThan10Years == "yes";
            long? firstYear = riskAssessment.AOOyear1;
            long? lastYear = riskAssessment.AOOyear2;
            long? firstYearArea = riskAssessment.AOO1;
            long? lastYearArea = riskAssessment.AOO2;
            long? areaOfOccurrenceTodayBest = riskAssessment.AOOtotalBestInput;
            long? knownAreaToday = riskAssessment.AOOknownInput;

            decimal result;
            if (chosenSubMethod)
            {
                result = (firstYear == null || lastYear == null || (lastYear - firstYear) < 10 || firstYearArea <= 0 || lastYearArea <= 0) ?
                0
                : Math.Truncate((decimal)(Math.Sqrt((double)(areaOfOccurrenceToday / knownAreaToday)) * 2000 * (Math.Sqrt(Math.Ceiling((double)(lastYearArea / 4))) - Math.Sqrt(Math.Ceiling((double)(firstYearArea / 4)))) / ((lastYear - firstYear) * Math.Sqrt(Math.PI))));
            }

            else
            {
                result = (decimal)Math.Truncate(20 * (Math.Sqrt((double)areaOfOccurrenceIn50Years) - Math.Sqrt((double)areaOfOccurrenceTodayBest)) / Math.Sqrt(Math.PI));
            }

            return (long)Math.Round(result, 0);
        }

        internal static long GetExpansionSpeedEstimates(RiskAssessment riskAssessment, string estimateQuantile, string assessmentConclusion)
        {
            var mainMethodA = riskAssessment.ChosenSpreadYearlyIncrease is "a";
            var mainMethodB = riskAssessment.ChosenSpreadYearlyIncrease is "b";
            var assessedDoorKnocker = assessmentConclusion is "AssessedDoorknocker";

            if (mainMethodA)
            {
                return (long)(estimateQuantile is "best" ? riskAssessment.ExpansionSpeedInput
                    : estimateQuantile is "low" ? riskAssessment.ExpansionLowerQInput
                    : riskAssessment.ExpansionUpperQInput);
            }

            if (mainMethodB && assessedDoorKnocker)
            {
                long? numberOfOccurrences;
                long? numberOfIntroductions;
                long areaAfterTenYearsEstimate;

                if (estimateQuantile == "low")
                {
                    numberOfOccurrences = riskAssessment.Occurrences1Low ?? 0;
                    numberOfIntroductions = IntroductionsLow(riskAssessment);

                }
                else if (estimateQuantile == "best")
                {
                    numberOfOccurrences = riskAssessment.Occurrences1Best ?? 0;
                    numberOfIntroductions = (int?)riskAssessment.IntroductionsBest ?? 0;
                }
                else
                {
                    numberOfOccurrences = riskAssessment.Occurrences1High ?? 0;
                    numberOfIntroductions = IntroductionsHigh(riskAssessment);
                }

                areaAfterTenYearsEstimate = AOO10yr(numberOfOccurrences, numberOfIntroductions) ?? 0;

                return (long)Math.Round(Math.Truncate(200 * (Math.Sqrt((double)(areaAfterTenYearsEstimate / 4)) - 1) / Math.Sqrt(Math.PI)), 0);
            }

            else //mainMethodB and assessed as self-reproducing
            {
                long areaOfOccurrenceToday;
                long areaOfOccurrenceIn50Years;
                if (estimateQuantile == "low")
                {
                    areaOfOccurrenceToday = riskAssessment.AOOtotalLowInput ?? 0;
                    areaOfOccurrenceIn50Years = riskAssessment.AOO50yrLowInput ?? 0;
                }

                else if (estimateQuantile == "best")
                {
                    areaOfOccurrenceToday = riskAssessment.AOOtotalBestInput ?? 0;
                    areaOfOccurrenceIn50Years = riskAssessment.AOO50yrBestInput ?? 0;
                }

                else
                {
                    areaOfOccurrenceToday = riskAssessment.AOOtotalHighInput ?? 0;
                    areaOfOccurrenceIn50Years = riskAssessment.AOO50yrHighInput ?? 0;
                }

                return GetExpansionSpeedAOOSelfReproducing(riskAssessment, areaOfOccurrenceToday, areaOfOccurrenceIn50Years);
            }

        }

        internal static string GetMedianLifetimeEstimationMethod(string category, string chosenMethod)
        {
            if (category == "NR" || chosenMethod == "RedListCategoryLevel")
            {
                return "NotRelevant";
            }

            else
            {
                return chosenMethod switch
                {
                    "LifespanA1aSimplifiedEstimate" => AlienSpeciesAssessment2023MedianLifetimeEstimationMethod.SimplifiedEstimation.ToString(),
                    "SpreadRscriptEstimatedSpeciesLongevity" => AlienSpeciesAssessment2023MedianLifetimeEstimationMethod.NumericalEstimation.ToString(),
                    _ => chosenMethod
                };
            }
        }


        internal static List<AlienSpeciesAssessment2023YearFirstRecordItem> GetYearsFirstObserved(RiskAssessment riskAssessment, string establishmentCategory)
        {
            if (establishmentCategory is "A") //species not yet in Norway cannot have observations in Norway
            {
                return new List<AlienSpeciesAssessment2023YearFirstRecordItem>();
            }

            else
            {
                var yearEstablishmentType = new List<AlienSpeciesAssessment2023YearFirstRecordItem>();

                foreach (var firstObservationProperty in riskAssessmentPropertiesFirstObservations)
                {
                    var yearFirstValue = firstObservationProperty.GetValue(riskAssessment);

                    if (yearFirstValue is not null)
                    {
                        AlienSpeciesAssessment2023YearFirstRecordType establishmentTypeName = firstObservationProperty.Name switch
                        {
                            "YearFirstEstablishedNature" => AlienSpeciesAssessment2023YearFirstRecordType.EstablishedNature,
                            "YearFirstReproductionNature" => AlienSpeciesAssessment2023YearFirstRecordType.ReproductionNature,
                            "YearFirstNature" => AlienSpeciesAssessment2023YearFirstRecordType.IndividualNature,
                            "YearFirstEstablishmentProductionArea" => AlienSpeciesAssessment2023YearFirstRecordType.EstablishedProductionArea,
                            "YearFirstReproductionOutdoors" => AlienSpeciesAssessment2023YearFirstRecordType.ReproductionProductionArea,
                            "YearFirstProductionOutdoors" => AlienSpeciesAssessment2023YearFirstRecordType.IndividualProductionArea,
                            "YearFirstReproductionIndoors" => AlienSpeciesAssessment2023YearFirstRecordType.ReproductionIndoors,
                            _ => AlienSpeciesAssessment2023YearFirstRecordType.IndividualIndoors,
                        };
                        var firstObservationUncertaintyProperty = riskAssessmentProperties.Where(x => x.Name == firstObservationProperty.Name + "Insecure").Single();
                        bool isUncertaintyYearValue = (bool)firstObservationUncertaintyProperty.GetValue(riskAssessment);
                        yearEstablishmentType.Add(new AlienSpeciesAssessment2023YearFirstRecordItem()
                        {
                            RecordType = establishmentTypeName, 
                            Year = (int)yearFirstValue, 
                            IsUncertaintyYear = isUncertaintyYearValue
                        });
                    }
                }

                return yearEstablishmentType;
            }
        }

        public static string GetWaterRegionName(string id)
        {
            // https://raw.githubusercontent.com/Artsdatabanken/Fremmedartsbase2023/main/Prod.Api/Resources/WaterRegion.geojson

            return id switch
            {
                "5103" => "Agder",
                "2" => "Bottenhavet",
                "1" => "Bottenviken",
                "5107" => "Innlandet og Viken",
                "VHA5" => "Kemijoki",
                "1101" => "Møre og Romsdal",
                "1108" => "Nordland og Jan Mayen",
                "1106" => "Norsk-finsk",
                "5104" => "Rogaland",
                "1TO" => "Torneå",
                "VHA6" => "Tornionjoki",
                "1109" => "Troms og Finnmark",
                "1107" => "Trøndelag",
                "5108" => "Vestfold og Telemark",
                "5109" => "Vestland",
                "5" => "Västerhavet",
                _ => string.Empty
            };
        }

        public static AlienSpeciesAssessment2023ScientificName[] GetNameHiearchy(List<FA4.ScientificNameWithRankId> srcNameHiearchy)
        {
            var path = srcNameHiearchy == null ? Array.Empty<AlienSpeciesAssessment2023ScientificName>() : srcNameHiearchy.Skip(1).Reverse().Select(x => new AlienSpeciesAssessment2023ScientificName()
            {
                ScientificNameFormatted = x.ScientificName,
                ScientificNameRank = (AlienSpeciesAssessment2023ScientificNameRank)x.Rank,
                ScientificNameAuthor = x.Author
            }).ToArray();
            return path;
        }

        public static AlienSpeciesAssessment2023ScientificName GetScientificName(FA4 src)
        {
            return new AlienSpeciesAssessment2023ScientificName()
            {
                ScientificNameFormatted = string.IsNullOrWhiteSpace(src.EvaluatedScientificNameFormatted) ? src.EvaluatedScientificName : src.EvaluatedScientificNameFormatted,
                ScientificNameId = src.EvaluatedScientificNameId,
                ScientificNameAuthor = src.EvaluatedScientificNameAuthor,
                ScientificNameRank = Enum.Parse<AlienSpeciesAssessment2023ScientificNameRank>(src.EvaluatedScientificNameRank ?? "22")
            };
        }

        /// <summary>
        /// List of banned species illegal to import to Norway
        /// </summary>
        public static int[] AlienSpeciesBanList()
        {
            return new int[]
            {
                16522,
                100327,
                100328,
                100816,
                100817,
                101761,
                102995,
                100936,
                101463,
                101479,
                101672,
                101938,
                101939,
                101982,
                101980,
                101981,
                99333,
                99334,
                129594,
                129595,
                129593,
                103272,
                103273,
                103280,
                103403,
                102634,
                129589,
                103588,
                103590,
                102635,
                128903,
            };
        }

        internal static List<AlienSpeciesAssessment2023NaturalOriginContinent> GetNaturalOriginContinent(bool oceania, bool africa, bool asia, bool europe, bool northAndCentralAmerica, bool southAmerica)
        {
            var continent = new List<AlienSpeciesAssessment2023NaturalOriginContinent>();
            if (oceania)
            {
                continent.Add(AlienSpeciesAssessment2023NaturalOriginContinent.Oceania);
            }
            if (africa)
            {
                continent.Add(AlienSpeciesAssessment2023NaturalOriginContinent.Africa);
            }
            if (asia)
            {
                continent.Add(AlienSpeciesAssessment2023NaturalOriginContinent.Asia);
            }
            if (europe)
            {
                continent.Add(AlienSpeciesAssessment2023NaturalOriginContinent.Europe);
            }
            if (northAndCentralAmerica)
            {
                continent.Add(AlienSpeciesAssessment2023NaturalOriginContinent.AmericaNorth);
            }
            if (southAmerica)
            {
                continent.Add(AlienSpeciesAssessment2023NaturalOriginContinent.AmericaNorth);
            }
            return continent;
        }

        internal static AlienSpeciesAssessment2023MajorTypeGroup GetMajorTypeGroup(string typeGroup)
        {
            return typeGroup switch
            {
                "Ferskvann" => AlienSpeciesAssessment2023MajorTypeGroup.FreshWaterThreatned,
                "Fjell og berg" => AlienSpeciesAssessment2023MajorTypeGroup.MountainsThreatned,
                "Landform" => AlienSpeciesAssessment2023MajorTypeGroup.LandformThreatned,
                "Marint gruntvann" => AlienSpeciesAssessment2023MajorTypeGroup.MarineWaterThreatned,
                "Marint gruntvann, Svalbard" => AlienSpeciesAssessment2023MajorTypeGroup.MarineWaterSvalbardThreatned,
                "Semi-naturlig" => AlienSpeciesAssessment2023MajorTypeGroup.SemiNaturalThreatned,
                "Skog" => AlienSpeciesAssessment2023MajorTypeGroup.ForestThreatned,
                "Svalbard" => AlienSpeciesAssessment2023MajorTypeGroup.SvalbardThreatned,
                "V\u00E5tmark" => AlienSpeciesAssessment2023MajorTypeGroup.WetlandsThreatned,
                "Limniske vannmasser" => AlienSpeciesAssessment2023MajorTypeGroup.LimnicWaterbodySystems,
                "Marine vannmasser" => AlienSpeciesAssessment2023MajorTypeGroup.MarineWaterbodySystems,
                "Sn\u00F8- og issystemer" => AlienSpeciesAssessment2023MajorTypeGroup.SnowAndIceSystems,
                "Innsj\u00F8bunnsystemer" => AlienSpeciesAssessment2023MajorTypeGroup.FreshwaterBottomSystems,
                "Saltvannsbunnsystemer" => AlienSpeciesAssessment2023MajorTypeGroup.MarineSeabedSystems,
                "Elvebunnsystemer" => AlienSpeciesAssessment2023MajorTypeGroup.RiverBottomSystems,
                "Fastmarkssystemer" => AlienSpeciesAssessment2023MajorTypeGroup.TerrestrialSystems,
                "V\u00E5tmarkssystemer" => AlienSpeciesAssessment2023MajorTypeGroup.WetlandSystems,
                _ => AlienSpeciesAssessment2023MajorTypeGroup.Unknown,
            };
        }

        internal static List<AlienSpeciesAssessment2023ReasonForChangeOfCategory> GetReasonForChangeOfCategory(List<string> reasonForChangeOfCategory)
        {
            var reasons = new List<AlienSpeciesAssessment2023ReasonForChangeOfCategory>();

            if (reasonForChangeOfCategory is null || reasonForChangeOfCategory.Count == 0)
            {
                return reasons;
            }

            if (new[] { "realChange", "newInformation" }.All(y => reasonForChangeOfCategory.Contains(y)))
            {
                reasonForChangeOfCategory.RemoveAll(x => ((string)x) == "newInformation");
            }

            foreach (var reason in reasonForChangeOfCategory)
            {
               var oneReason = reason switch
                {
                    "realChange" or "newInformation" => AlienSpeciesAssessment2023ReasonForChangeOfCategory.NewKnowledge,
                    "newInterpretation" => AlienSpeciesAssessment2023ReasonForChangeOfCategory.NewInterpretation,
                    "changedCriteria" => AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedGuidelines,
                    "changedCriteriaInterpretation" => AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedGuidelinesInterpretation,
                    _ => AlienSpeciesAssessment2023ReasonForChangeOfCategory.ChangedStatus
                    
                 };
                reasons.Add(oneReason);
            }
            return reasons;
        }

        internal static DateTime GetRevisionDate(int assessmentId)
        {
            var dateObject = new DateTime();
            return assessmentId switch
            {
                2526 => new DateTime(2024, 05, 06),
                1711 => new DateTime(2025, 06, 06),  //TODO: use date for publishing
                2375 => new DateTime(2025, 06, 06), //TODO: use date for publishing
                5128 => new DateTime(2025, 06, 06), //TODO: use date for publishing
                5142 => new DateTime(2025, 06, 06), //TODO: use date for publishing
                5164 => new DateTime(2025, 06, 06), //TODO: use date for publishing
                5184 => new DateTime(2025, 06, 06), //TODO: use date for publishing
                1468 => new DateTime(2025, 06, 06), //TODO: use date for publishing
                _ => dateObject
            };
        }
    }
}
