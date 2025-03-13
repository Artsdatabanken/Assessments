using Assessments.Web.Models;
using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.AlienSpecies.Model.Enums;
using Assessments.Shared.Helpers;
using Assessments.Web.Models.AlienSpecies;

namespace Assessments.Web.Infrastructure.AlienSpecies
{
    public class StatisticsHelper
    {
        private readonly IQueryable<AlienSpeciesAssessment2023> _query;
        private readonly IQueryable<AlienSpeciesAssessment2023> _unfilteredQuery;

        public StatisticsHelper(IQueryable<AlienSpeciesAssessment2023> query, IQueryable<AlienSpeciesAssessment2023> unfilteredQuery)
        {
            // NR and TaxonEvaluatedAtAnotherLevel should never appear in the statistics figures
            _query = query.Where(x => x.Category != AlienSpeciesAssessment2023Category.NR && x.AlienSpeciesCategory != AlienSpeciecAssessment2023AlienSpeciesCategory.TaxonEvaluatedAtAnotherLevel);
            _unfilteredQuery = unfilteredQuery;
        }

        public AlienSpeciesStatisticsViewModel GetStatistics()
        {
            var statistics = new AlienSpeciesStatisticsViewModel();

            statistics.DecisiveCriteria = this.GetDecisiveCriteria();
            statistics.RiskCategories = this.GetRiskCategories();
            statistics.RiskMatrix = this.GetRiskMatrixValues();
            statistics.SpeciesGroups = this.GetSpeciesGroups();
            statistics.MajorNatureTypesEffect = this.GetMajorNatureTypesEffect();
            statistics.NatureTypesEffect = this.GetNatureTypesEffect();
            statistics.SpreadWays = this.GetSpreadWays();
            statistics.SpreadWaysIntroduction = this.GetSpreadWaysIntroduction();
            statistics.EstablishmentClass = this.GetEstablishmentClass();

            return statistics;
        }

        private BarChart GetRiskCategories()
        {
            var distinctCategories = new List<AlienSpeciesAssessment2023Category>((IEnumerable<AlienSpeciesAssessment2023Category>)Enum.GetValues(typeof(AlienSpeciesAssessment2023Category))).Where(x => x != AlienSpeciesAssessment2023Category.NR);

            var barChartDatas = distinctCategories.Select(x => new BarChart.BarChartData
            {
                Name = x.DisplayName(),
                NameShort = x.ToString(),
                Count = _query.Where(y => y.Category == x).Count()
            }).OrderBy(x => x.NameShort.ToString(), new AlienSpeciesCategoryComparer()).ToList();

            return new BarChart()
            {
                Data = barChartDatas,
                MaxValue = barChartDatas.MaxBy(x => x.Count).Count
            };
        }

        private BarChart GetSpeciesGroups()
        {
            var distinctSpeciesGroups = new List<AlienSpeciesAssessment2023SpeciesGroups>((IEnumerable<AlienSpeciesAssessment2023SpeciesGroups>)Enum.GetValues(typeof(AlienSpeciesAssessment2023SpeciesGroups)));
            var singleAlgae = AlienSpeciesAssessment2023SpeciesGroups.Rhodophyta_Chlorophyta_Phaeophyceae.DisplayName();
            var singleCrayfish = AlienSpeciesAssessment2023SpeciesGroups.Crustacea.DisplayName();
            var singleInsect = AlienSpeciesAssessment2023SpeciesGroups.Insecta.DisplayName();
            var algae = new AlienSpeciesAssessment2023SpeciesGroups[]
            {
                AlienSpeciesAssessment2023SpeciesGroups.Rhodophyta,
                AlienSpeciesAssessment2023SpeciesGroups.Phaeophyceae,
                AlienSpeciesAssessment2023SpeciesGroups.Chlorophyta
            };
            var crayfish = new AlienSpeciesAssessment2023SpeciesGroups[]
            {
                AlienSpeciesAssessment2023SpeciesGroups.Malacostraca,
                AlienSpeciesAssessment2023SpeciesGroups.Branchiopoda,
                AlienSpeciesAssessment2023SpeciesGroups.Copepoda,
                AlienSpeciesAssessment2023SpeciesGroups.Thecostraca
            };
            var insects = new AlienSpeciesAssessment2023SpeciesGroups[]
            {
                AlienSpeciesAssessment2023SpeciesGroups.Coleoptera,
                AlienSpeciesAssessment2023SpeciesGroups.Zygentoma,
                AlienSpeciesAssessment2023SpeciesGroups.Phthiraptera,
                AlienSpeciesAssessment2023SpeciesGroups.Hemiptera,
                AlienSpeciesAssessment2023SpeciesGroups.Lepidoptera,
                AlienSpeciesAssessment2023SpeciesGroups.Psocoptera,
                AlienSpeciesAssessment2023SpeciesGroups.Diptera,
                AlienSpeciesAssessment2023SpeciesGroups.Thysanoptera,
                AlienSpeciesAssessment2023SpeciesGroups.Hymenoptera
            };

            var algaeBarChart = new BarChart()
            {
                Data = distinctSpeciesGroups.Where(x => algae.Any(y => x == y)).Select(x => new BarChart.BarChartData
                {
                    Name = singleAlgae,
                    Count = _query.Where(y => y.SpeciesGroup == x).Count()
                }).ToList()
            };

            var crayfishBarChart = new BarChart()
            {
                Data = distinctSpeciesGroups.Where(x => crayfish.Any(y => x == y)).Select(x => new BarChart.BarChartData
                {
                    Name = singleCrayfish,
                    Count = _query.Where(y => y.SpeciesGroup == x).Count()
                }).ToList()
            };

            var insectsBarChart = new BarChart()
            {
                Data = distinctSpeciesGroups.Where(x => insects.Any(y => x == y)).Select(x => new BarChart.BarChartData
                {
                    Name = singleInsect,
                    Count = _query.Where(y => y.SpeciesGroup == x).Count()
                }).ToList()
            };

            var speciesBarChart = new BarChart()
            {
                Data = distinctSpeciesGroups.Where(x => x is not AlienSpeciesAssessment2023SpeciesGroups.Unknown && !algae.Contains(x) && !crayfish.Contains(x) && !insects.Contains(x)).Select(x => new BarChart.BarChartData
                {
                    Name = x.DisplayName(),
                    Count = _query.Where(y => y.SpeciesGroup.DisplayName() == x.DisplayName()).Count()
                }).ToList()
            };

            var algaeBarChartData = new BarChart.BarChartData()
            {
                Name = singleAlgae,
                Count = algaeBarChart.Data.Sum(x => x.Count)
            };
            var crayFishBarChartData = new BarChart.BarChartData()
            {
                Name = singleCrayfish,
                Count = crayfishBarChart.Data.Sum(x => x.Count)
            };
            var insectsBarChartData = new BarChart.BarChartData()
            {
                Name = singleInsect,
                Count = insectsBarChart.Data.Sum(x => x.Count)
            };

            speciesBarChart.Data.Add(algaeBarChartData);
            speciesBarChart.Data.Add(crayFishBarChartData);
            speciesBarChart.Data.Add(insectsBarChartData);

            speciesBarChart.Data = speciesBarChart.Data.OrderByDescending(x => x.Count).DistinctBy(x => x.Name).ToList();
            speciesBarChart.MaxValue = speciesBarChart.Data.MaxBy(x => x.Count).Count;

            return speciesBarChart;
        }

        private List<List<int>> GetRiskMatrixValues()
        {
            var riskMatrix = new List<List<int>>()
            {
                new List<int>() {0, 0, 0, 0},
                new List<int>() {0, 0, 0, 0},
                new List<int>() {0, 0, 0, 0},
                new List<int>() {0, 0, 0, 0}
            };

            foreach (var assessment in _query)
            {
                riskMatrix[(int)assessment.ScoreInvasionPotential - 1][(int)assessment.ScoreEcologicalEffect - 1] += 1;
            }

            return riskMatrix;
        }

        private BarChart GetDecisiveCriteria()
        {
            var decisiveCriteria = new List<AlienSpeciesAssessment2023CriteriaLetter>((IEnumerable<AlienSpeciesAssessment2023CriteriaLetter>)Enum.GetValues(typeof(AlienSpeciesAssessment2023CriteriaLetter)));
            var AxBText = "A×B Levetid × Ekspansjonshastighet";

            var AxB = decisiveCriteria.Where(x => x == AlienSpeciesAssessment2023CriteriaLetter.A || x == AlienSpeciesAssessment2023CriteriaLetter.B).Select(x => new BarChart.BarChartData
            {
                Name = AxBText,
                Count = _query.Where(y => y.DecisiveCriteria.Contains(x.ToString())).Count()
            }).ToList();

            var aXbBarChartData = new BarChart.BarChartData()
            {
                Name = AxBText,
                Count = AxB.Sum(x => x.Count)
            };

            var allDecisiveCriteria = new BarChart()
            {
                Data = decisiveCriteria.Where(x => x != AlienSpeciesAssessment2023CriteriaLetter.A && x != AlienSpeciesAssessment2023CriteriaLetter.B).Select(x => new BarChart.BarChartData
                {
                    Name = $"{x.ToString()} {x.DisplayName()}",
                    Count = _query.Where(y => y.DecisiveCriteria.Contains(x.ToString())).Count()
                }).OrderBy(x => x.Name).ToList()
            };

            allDecisiveCriteria.Data.Insert(0, aXbBarChartData);
            allDecisiveCriteria.MaxValue = allDecisiveCriteria.Data.MaxBy(x => x.Count).Count;
            return allDecisiveCriteria;
        }

        private BarChart GetMajorNatureTypesEffect()
        {
            var threatenedMajorTypeGroups = new List<AlienSpeciesAssessment2023MajorTypeGroup>();
            foreach (var assessment in _unfilteredQuery.Where(x => x.Category != AlienSpeciesAssessment2023Category.NR && x.AlienSpeciesCategory != AlienSpeciecAssessment2023AlienSpeciesCategory.TaxonEvaluatedAtAnotherLevel))
            {
                var typeGroups = assessment.ImpactedNatureTypes.Where(x => x.IsThreatened).Select(x => x.MajorTypeGroup);
                threatenedMajorTypeGroups = threatenedMajorTypeGroups.Concat(typeGroups).ToList();
            }
            threatenedMajorTypeGroups = threatenedMajorTypeGroups.Distinct().ToList();

            var majorNatureTypesEffects = new BarChart()
            {
                Data = threatenedMajorTypeGroups.Select(x => new BarChart.BarChartData()
                {
                    Name = x.DisplayName(),
                    Count = _query.Where(y => y.ImpactedNatureTypes.Any(z => z.MajorTypeGroup == x)).Count()
                }).OrderByDescending(x => x.Count).ToList()
            };
            majorNatureTypesEffects.MaxValue = majorNatureTypesEffects.Data.MaxBy(x => x.Count).Count;

            return majorNatureTypesEffects;
        }

        private List<BarChart> GetNatureTypesEffect()
        {
            var unfilteredQuery = _unfilteredQuery.Where(x => x.Category != AlienSpeciesAssessment2023Category.NR && x.AlienSpeciesCategory != AlienSpeciecAssessment2023AlienSpeciesCategory.TaxonEvaluatedAtAnotherLevel);

            var impactedNatureTypes = unfilteredQuery.SelectMany(x => x.ImpactedNatureTypes).Where(x => x.IsThreatened);

            var barCharts = new List<BarChart>();

            foreach (var impactedNatureType in impactedNatureTypes.GroupBy(x => x.MajorTypeGroup).OrderBy(x => x.Key))
            {
                var data = impactedNatureType.GroupBy(x => x.Name).Select(x => new
                {
                    Name = x.Key,
                    Count = _query.SelectMany(y => y.ImpactedNatureTypes.DistinctBy(z => z.Name)).Where(y => y.Name == x.Key).Count()
                });

                var barChart = new BarChart
                {
                    Name = impactedNatureType.Key.DisplayName()
                };

                foreach (var item in data.OrderBy(x => x.Name))
                {
                    barChart.Data.Add(new()
                    {
                        Name = item.Name,
                        Count = item.Count
                    });
                }

                barCharts.Add(barChart);
            }

            foreach (var chart in barCharts)
            {
                chart.MaxValue = barCharts.SelectMany(x => x.Data).Max(y => y.Count);
            }

            return barCharts;
        }

        private BarChart GetSpreadWays()
        {
            var allSpreadWays = new List<AlienSpeciesAssessment2023IntroductionPathway.MainCategory>((IEnumerable<AlienSpeciesAssessment2023IntroductionPathway.MainCategory>)Enum.GetValues(typeof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory))).Skip(2).Take(6).ToList();

            var barChart = new BarChart()
            {
                Data = allSpreadWays.Select(x => new BarChart.BarChartData()
                {
                    Name = x.DisplayName(),
                    Count = _query.SelectMany(y => y.IntroductionAndSpreadPathways.Where(z => z.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Introduction && z.MainCategory == x).DistinctBy(z => z.MainCategory)).Count()
                }).OrderByDescending(x => x.Count).ToList()
            };
            barChart.MaxValue = barChart.Data.Max(x => x.Count);
            return barChart;
        }

        private List<BarChart> GetSpreadWaysIntroduction()
        {
            var allMainSpreadWays = new List<AlienSpeciesAssessment2023IntroductionPathway.MainCategory>((IEnumerable<AlienSpeciesAssessment2023IntroductionPathway.MainCategory>)Enum.GetValues(typeof(AlienSpeciesAssessment2023IntroductionPathway.MainCategory))).Skip(2).Take(6).ToList();

            var barCharts = new List<BarChart>();

            var uniqueIntroductionPathwaysPerSpecies = new List<string>();

            var introductionPathwaysSpecies = _query.Select(y =>  y.IntroductionAndSpreadPathways.Where(z => z.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Introduction).Select(z => z.Category).ToArray()).ToList();

            foreach (var speciesPathways in introductionPathwaysSpecies)
            {
                var uniquePathways = speciesPathways.Distinct().ToList();
                uniqueIntroductionPathwaysPerSpecies.AddRange( uniquePathways );
            };

            foreach (var spreadWay in allMainSpreadWays)
            {
                var barChart = new BarChart()
                {
                    Name = spreadWay.DisplayName(),
                    Data = _unfilteredQuery.SelectMany(x => x.IntroductionAndSpreadPathways.Where(y => y.IntroductionSpread == AlienSpeciesAssessment2023IntroductionPathway.IntroductionSpread.Introduction && y.MainCategory == spreadWay)).DistinctBy(x => x.Category).Select(x => new BarChart.BarChartData()
                    {
                        Name = x.Category,
                        Count = uniqueIntroductionPathwaysPerSpecies.Where(item => x.Category.Equals(item)).Count()
                    }).OrderBy(x => x.Name).ToList()
                };
                barCharts.Add(barChart);
            }

            var maxValue = barCharts.Where(x => x.Data.Any()).Max(x => x.Data.Max(y => y.Count));
            foreach (var barChart in barCharts)
            {
                barChart.MaxValue = maxValue;
            }

            return barCharts.ToList();
        }

        private List<BarChart> GetEstablishmentClass()
        {
            var reproductiveEstablishmentClasses = new List<AlienSpeciesAssessment2023SpeciesStatus>((IEnumerable<AlienSpeciesAssessment2023SpeciesStatus>)Enum.GetValues(typeof(AlienSpeciesAssessment2023SpeciesStatus))).Skip(6).ToList();

            var doorKnockerEstablishmentClasses = new List<AlienSpeciesAssessment2023SpeciesStatus>((IEnumerable<AlienSpeciesAssessment2023SpeciesStatus>)Enum.GetValues(typeof(AlienSpeciesAssessment2023SpeciesStatus))).Skip(1).Take(5).ToList();

            var barCharts = new List<BarChart>()
            {
                new BarChart()
                {
                    Name = "Reproduserende",
                    Data = _unfilteredQuery.Where(x => reproductiveEstablishmentClasses.Contains(x.SpeciesStatus)).DistinctBy(x => x.SpeciesStatus).Select(x => new BarChart.BarChartData()
                    {
                        Name = x.SpeciesStatus.DisplayName(),
                        Count = _query.Where(y => y.SpeciesStatus == x.SpeciesStatus).Count()
                    }).ToList()
                },
                new BarChart()
                {
                    Name = "Dørstokkart",
                    Data = _unfilteredQuery.Where(x => doorKnockerEstablishmentClasses.Contains(x.SpeciesStatus)).DistinctBy(x => x.SpeciesStatus).OrderBy(x => x.SpeciesStatus.ToString(), new AlienSpeciesSpeciesStatusComparer()).Select(x => new BarChart.BarChartData()
                    {
                        Name = x.SpeciesStatus.DisplayName(),
                        Count = _query.Where(y => y.SpeciesStatus == x.SpeciesStatus).Count()
                    }).ToList()
                }
            };

            foreach (var barChart in barCharts)
            {
                barChart.MaxValue = barCharts.Max(x => x.Data.Max(y => y.Count));
            }

            return barCharts;
        }
    }
}
