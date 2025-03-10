using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.AlienSpecies.Model.Enums;
using Assessments.Shared.Helpers;
using static Assessments.Web.Infrastructure.FilterHelpers;

namespace Assessments.Web.Infrastructure.AlienSpecies
{
    public class AlienSpeciesHelpers
    {
        public static FilterItem GetSpeciesGroup(string speciesGroupName)
        {
            var speciesGroups = new SpeciesGroups().AlienSpecies2023SpeciesGroups();
            foreach (var speciesGroup in speciesGroups.Filters)
            {
                if (speciesGroup.Name == speciesGroupName)
                {
                    return speciesGroup;
                }

                if (speciesGroup.SubGroup != null)
                {
                    foreach (var subGroup in speciesGroup.SubGroup.Filters)
                    {
                        if (subGroup.Name == speciesGroupName)
                        {
                            return subGroup;
                        }
                    }
                }
            }
            return null;
        }

        public static string CriteriaDescription(AlienSpeciesAssessment2023CriteriaLetter letter, int value)
        {
            var description = string.Empty;

            switch (letter)
            {
                case AlienSpeciesAssessment2023CriteriaLetter.A:
                    description = value switch
                    {
                        1 => Resources.SharedResources.less_than_ten_years,
                        2 => Resources.SharedResources.between_ten_59_years,
                        3 => Resources.SharedResources.between_60_649_years,
                        4 => Resources.SharedResources.minimum_650_years,
                        _ => description
                    };
                    break;
                case AlienSpeciesAssessment2023CriteriaLetter.B:
                    description = value switch
                    {
                        1 => Resources.SharedResources.less_than_50_myear,
                        2 => Resources.SharedResources.between_50_159_myear,
                        3 => Resources.SharedResources.between_160_499_myear,
                        4 => Resources.SharedResources.minimum_500_myear,
                        _ => description
                    };
                    break;
                case AlienSpeciesAssessment2023CriteriaLetter.C:
                case AlienSpeciesAssessment2023CriteriaLetter.G:
                    description = value switch
                    {
                        1 => Resources.SharedResources.less_than_five_percent,
                        2 => Resources.SharedResources.min_five_percent,
                        3 => Resources.SharedResources.minimum_ten_percent,
                        4 => Resources.SharedResources.minimum_20_percent,
                        _ => description
                    };
                    break;
                case AlienSpeciesAssessment2023CriteriaLetter.D:
                    description = value switch
                    {
                        1 => Resources.SharedResources.absent,
                        2 => Resources.SharedResources.weak_confined,
                        3 => Resources.SharedResources.weak_large_scale_or_moderate_confined,
                        4 => Resources.SharedResources.moderate_large_scale_or_displacement,
                        _ => description
                    };
                    break;
                case AlienSpeciesAssessment2023CriteriaLetter.E:
                    description = value switch
                    {
                        1 => Resources.SharedResources.weak_or_moderate_confined,
                        2 => Resources.SharedResources.moderate_large_scale,
                        3 => Resources.SharedResources.confined_displacement,
                        4 => Resources.SharedResources.large_scale_displacement,
                        _ => description
                    };
                    break;
                case AlienSpeciesAssessment2023CriteriaLetter.F:
                    description = value switch
                    {
                        1 => Resources.SharedResources.zero_percent,
                        2 => Resources.SharedResources.over_zero_percent,
                        3 => Resources.SharedResources.min_two_percent,
                        4 => Resources.SharedResources.min_five_percent,
                        _ => description
                    };
                    break;
                case AlienSpeciesAssessment2023CriteriaLetter.H:
                    description = value switch
                    {
                        1 => Resources.SharedResources.no_transfer,
                        2 => Resources.SharedResources.confined_native_species,
                        3 => Resources.SharedResources.large_scale_non_threatened_OR_confined_threatened,
                        4 => Resources.SharedResources.large_scale_threatened,
                        _ => description
                    };
                    break;
                case AlienSpeciesAssessment2023CriteriaLetter.I:
                    description = value switch
                    {
                        1 => Resources.SharedResources.absent_OR_confined_known_host,
                        2 => Resources.SharedResources.large_scale_known_host_OR_confined_new_host,
                        3 => Resources.SharedResources.large_scale_new_host_OR_confined_threathened_new_host,
                        4 => Resources.SharedResources.large_scale_new_threatened_host_OR_doorknocker_parasite,
                        _ => description
                    };
                    break;
                default:
                    return description;
            }

            return description;
        }

        public static int GetUncertaintyValueMinMax(int value, int uncertainty)
        {
            return uncertainty == 0 ? value : uncertainty;
        }
        public static int GetUncertaintyHigh(AlienSpeciesAssessment2023Criterion criterion)
        {
            return criterion.UncertaintyValues.Where(x => x > criterion.Value).FirstOrDefault();
        }
        public static int GetUncertaintyLow(AlienSpeciesAssessment2023Criterion criterion)
        {
            return criterion.UncertaintyValues.Where(x => x < criterion.Value).FirstOrDefault();
        }

        public static string GetLetterFullText(AlienSpeciesAssessment2023Criterion criterion)
        {
            var letterText = "";
            letterText += criterion.CriteriaLetter.DisplayName() + ": ";
            letterText += CriteriaDescription(criterion.CriteriaLetter, criterion.Value);
            letterText += GetUncertaintyHigh(criterion) == 0 && GetUncertaintyLow(criterion) == 0 ? "." : "";
            if (GetUncertaintyHigh(criterion) != 0 || GetUncertaintyLow(criterion) != 0)
            {
                letterText += " ("+ Resources.SharedResources.with_uncertainty + " ";
            }
            if (GetUncertaintyLow(criterion) != 0)
            {
                letterText += Resources.SharedResources.down_to + " ";
                letterText += CriteriaDescription(criterion.CriteriaLetter, AlienSpeciesHelpers.GetUncertaintyLow(criterion));
                letterText += GetUncertaintyHigh(criterion) == 0 && GetUncertaintyLow(criterion) != 0 ? ")." : "";
            }
            if (GetUncertaintyHigh(criterion) != 0 && GetUncertaintyLow(criterion) != 0)
            {
                letterText += " " + Resources.SharedResources.and + " ";
            }
            if (AlienSpeciesHelpers.GetUncertaintyHigh(criterion) != 0)
            {
                letterText += Resources.SharedResources.up_to + " ";
                letterText += CriteriaDescription(criterion.CriteriaLetter, GetUncertaintyHigh(criterion));
                letterText += GetUncertaintyHigh(criterion) != 0 || GetUncertaintyLow(criterion) != 0 ? ")." : "";
            }

            return letterText;
        }
        public static string LowerFirstCharacter(string text)
        {
            return char.ToLower(text[0]) + text[1..];
        }

        public static string GetInvasionPotentialExplanation(List<AlienSpeciesAssessment2023Criterion> criteria)
        {
            var explanation = string.Empty;
            var hasAorB = criteria.Any(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.A || x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.B);
            var hasC = criteria.Any(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.C);

            if (criteria.Count != 0)
            {
                if (hasAorB)
                {
                    var aCriterion = criteria.Where(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.A).SingleOrDefault();
                    var bCriterion = criteria.Where(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.B).SingleOrDefault();
                    explanation += $"Arten har en forventet levetid i Norge på {CriteriaDescription(aCriterion.CriteriaLetter, aCriterion.Value)} og en ekspansjonshastighet  på {CriteriaDescription(bCriterion.CriteriaLetter, bCriterion.Value)}. ";
                }
                if (hasC)
                {
                    var cCriterion = criteria.Where(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.C).SingleOrDefault();
                    explanation += $"Arten koloniserer {CriteriaDescription(cCriterion.CriteriaLetter, cCriterion.Value)} av en naturtype.";
                }
            }
            return explanation;
        }

        public static string GetEcologicalEffectExplanation(List<AlienSpeciesAssessment2023Criterion> criteria)
        {
            var hasDE = criteria.Any(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.D || x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.E);
            var hasFG = criteria.Any(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.F || x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.G);
            var hasH = criteria.Any(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.H);
            var hasI = criteria.Any(x => x.CriteriaLetter == AlienSpeciesAssessment2023CriteriaLetter.I);

            var controlValue = 0;
            var explanation = "Arten";

            if (hasDE)
                controlValue += 1;
            if (hasFG)
                controlValue += 2;
            if (hasH)
                controlValue += 4;
            if (hasI)
                controlValue += 8;

            var deText = " har negative interaksjoner med stedegne arter";
            var fgText = " medfører endring i naturtyper";
            var hText = " overfører genetisk materiale til stedegne arter";
            var iText = " overfører parasitter/patogener til stedegne arter";
            var noCriteria = " har ingen utslagsgivende kriterier for økologisk effekt";

            explanation += controlValue switch
            {
                1 => $"{deText}.",
                2 => $"{fgText}.",
                3 => $"{deText} og {fgText}.",
                4 => $"{hText}.",
                5 => $"{deText} og {hText}.",
                6 => $"{fgText} og {hText}.",
                7 => $"{deText}, {fgText} og {hText}.",
                8 => $"{iText}.",
                9 => $"{deText} og {iText}.",
                10 => $"{fgText} og {iText}.",
                11 => $"{deText}, {fgText} og {iText}.",
                12 => $"{hText} og {iText}.",
                13 => $"{deText}, {hText} og {iText}.",
                14 => $"{fgText}, {hText} og {iText}.",
                15 => $"{deText}, {fgText}, {hText} og {iText}.",
                _ => $"{noCriteria}."
            };

            return explanation;
        }
    }
}
