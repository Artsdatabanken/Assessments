using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.Services;
using Assessments.Web.Models;
using Assessments.Mapping.RedlistSpecies;
using Assessments.Shared.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Web;
using X.PagedList.Extensions;

namespace Assessments.Web.Controllers
{
    [Route("rodlisteforarter")]
    public class RedlistController : BaseController<RedlistController>
    {
        public RedlistController(ArtskartApiService artskartApiService)
        {
            _artskartApiService = artskartApiService;
        }
        public IActionResult RodlisteForArter() => View("Species/Rodlisteforarter");

        private static readonly Dictionary<string, JObject> _resourceCache = new();
        private readonly ArtskartApiService _artskartApiService;

        [Route("2021")]
        public async Task<IActionResult> Index2021([FromQueryAttribute] RL2021ViewModel viewModel, int? page, bool export)
        {
            if (!string.IsNullOrEmpty(HttpContext.Request.Query[Constants.SearchAndFilter.RemoveFilters].ToString()))
            {
                var queryParams = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                queryParams = Helpers.RemoveFiltersFromQuery(queryParams);
                string url = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase + HttpContext.Request.Path : HttpContext.Request.Path;
                var queryString = "?" + queryParams.ToString();
                Response.Redirect(url + queryString);
            }

            if (!string.IsNullOrEmpty(HttpContext.Request.Query[Constants.SearchAndFilter.RemoveSearch].ToString()))
            {
                var queryParams = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                queryParams.Remove(nameof(viewModel.Name));
                queryParams.Remove(Constants.SearchAndFilter.RemoveSearch);
                string url = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase + HttpContext.Request.Path : HttpContext.Request.Path;
                var queryString = "?" + queryParams.ToString();
                Response.Redirect(url + queryString);
            }

            viewModel ??= new RL2021ViewModel();

            ViewBag.glossary = await GetResource("wwwroot/json/glossary.json");

            ViewBag.speciesgroup = await GetResource("wwwroot/json/speciesgroup.json");

            ViewBag.kriterier = await GetResource("wwwroot/json/kriterier.json");

            ViewBag.habitat = await GetResource("wwwroot/json/habitat.json");

            ViewBag.categories = await GetResource("wwwroot/json/categories.json");


            // Pagination
            const int pageSize = 25;
            var pageNumber = page ?? 1;

            var query = await DataRepository.GetSpeciesAssessments();

            // Søk
            if (!string.IsNullOrEmpty(viewModel.Name))
            {
                var name = viewModel.Name.Trim().ToLower();

                var queryByName = Helpers.GetQueryByName(query, name);

                if (queryByName.Any())
                {
                    query = queryByName;
                }
                else // bruk populærnavn om ingen treff på navn
                {
                    var popularNames = await _artskartApiService.Get<List<ArtskartTaxon>>($"data/SearchTaxons?maxCount=20&name={name}");
                    if (popularNames != null && popularNames.Any())
                        query = query.Where(x => popularNames.Select(y => y.ScientificNameId).Contains(x.ScientificNameId));
                    else
                        query = queryByName;
                }
            }

            // Filter

            // Areas
            if (viewModel.Area?.Any() == true)
                query = query.Where(x => viewModel.Area.Contains(x.AssessmentArea));

            // Categories
            viewModel.Category = Helpers.FindSelectedRedlistSpeciesCategories(viewModel.Redlisted, viewModel.Endangered, viewModel.Category);

            if (viewModel.Category?.Any() == true)
                query = query.Where(x => !string.IsNullOrEmpty(x.Category) && viewModel.Category.Any(y => x.Category.Contains(y)));

            // Criterias
            if (viewModel.Criterias?.Any() == true)
                query = query.Where(x => !string.IsNullOrEmpty(x.CriteriaSummarized) && viewModel.Criterias.Any(y => x.CriteriaSummarized.Contains(y)));

            // Habitat
            if (viewModel.Habitats?.Any() == true)
            {
                // The model names are not compatible with enum naming rules. We need to consider this.
                if (viewModel.Habitats.Contains("Fastmark"))
                    query = query.Where(x => viewModel.Habitats.Any(y => x.MainHabitat.Contains(y) || x.MainHabitat.Contains("Semi-naturlig fastmark")));
                else
                    query = query.Where(x => viewModel.Habitats.Any(y => x.MainHabitat.Contains(y)));
            }

            // Regions
            ViewBag.AllRegions = Helpers.GetRegionsDict();
            string[] chosenRegions = Helpers.FindSelectedRegions(viewModel.Regions, ViewBag.AllRegions);

            if (chosenRegions?.Any() == true)
                query = query.Where(x => x.RegionOccurrences.Any(y => y.State == 0 && chosenRegions.Contains(y.Fylke)));

            // SpeciesGroups
            ViewBag.AllSpeciesGroups = Helpers.GetAllSpeciesGroups(ViewBag.speciesgroup.ToObject<Dictionary<string, Dictionary<string, string>>>());
            ViewBag.AllInsects = Constants.AllInsects;

            if (viewModel.SpeciesGroups?.Any() == true)
            {
                if (viewModel.SpeciesGroups.Contains("Insekter"))
                    viewModel.SpeciesGroups = Helpers.GetSelectedSpeciesGroups(viewModel.SpeciesGroups.ToList());

                query = query.Where(x => !string.IsNullOrEmpty(x.SpeciesGroup) && viewModel.SpeciesGroups.Contains(x.SpeciesGroup));
            }

            // TaxonRank
            if (viewModel.TaxonRank?.Any() == true)
                query = query.Where(x => viewModel.TaxonRank.Contains(x.TaxonRank));

            // European population percentages
            string[] chosenEuropeanPopulation = Helpers.FindEuropeanPopProcentages(viewModel.EuroPop);

            if (chosenEuropeanPopulation?.Any() == true)
                query = query.Where(x => !string.IsNullOrEmpty(x.PercentageEuropeanPopulation) && chosenEuropeanPopulation.Contains(x.PercentageEuropeanPopulation));

            // Extinct
            if (viewModel.PresumedExtinct)
                query = query.Where(x => x.PresumedExtinct);

            // Sort
            query = Helpers.SortResults(query, viewModel.Name, viewModel.SortBy);

            if (export)
            {
                var assessmentsForExport = Mapper.Map<IEnumerable<SpeciesAssessment2021Export>>(query.ToList());
                var expertCommitteeMembers = await DataRepository.GetData<ExpertCommitteeMember>(DataFilenames.SpeciesExpertCommitteeMembers);
                expertCommitteeMembers = expertCommitteeMembers.Where(x => x.Year == 2021);

                return new FileStreamResult(ExportHelper.GenerateSpeciesAssessment2021Export(assessmentsForExport, expertCommitteeMembers.ToList(), Request.GetDisplayUrl()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "rødliste-2021.xlsx"
                };
            }

            if (!string.IsNullOrEmpty(viewModel.View) && viewModel.View.Equals("stat"))
            {
                SetupStatisticsViewModel(query.ToList(), viewModel);
            }
            else
            {
                viewModel.Redlist2021Results = query.ToPagedList(pageNumber, pageSize);
            }

            return View("Species/2021/List/List", viewModel);
        }

        [HttpGet, Route("2021/suggestions")]
        public async Task<IActionResult> Suggestion([FromQueryAttribute] string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return BadRequest("Søket kan ikke være tomt.");
            }

            var jsonSpeciesGroup = await GetResource("wwwroot/json/speciesgroup.json");
            Dictionary<string, Dictionary<string, string>> speciesgroupDict = jsonSpeciesGroup.ToObject<Dictionary<string, Dictionary<string, string>>>();

            var name = search.Trim().ToLower();
            var query = await DataRepository.GetSpeciesAssessments();

            try
            {
                var artskartResult = await _artskartApiService.Get<List<ArtskartTaxon>>($"data/SearchTaxons?maxCount=20&name={name}");

                // Remove species not present in 'rødlista for arter'
                var suggestions = artskartResult.Where(x =>
                                                        (x.TaxonCategory != Constants.TaxonCategoriesEn.Species &&                          // Not species
                                                        x.TaxonCategory != Constants.TaxonCategoriesEn.SubSpecies &&                        // Not subspecies
                                                        x.TaxonCategory != Constants.TaxonCategoriesEn.Variety) &&                          // Not variety
                                                        query.Any(y => y.VurdertVitenskapeligNavnHierarki.Contains(x.ScientificName)) ||    // Check if none of the above -> exists in taxonomic rank
                                                        query.Any(y => y.ScientificNameId == x.ScientificNameId)).ToList();                // or match on scientific name id: exact match on species/subsp./var.

                // Add assessments if they are in redlist, but not in artskart. "Subsp." and "var." are not included in scientific names in artskart.
                var redlistHits = query.Where(x => x.ScientificName.Trim().ToLower().Contains(name)).ToArray();

                foreach (var hit in redlistHits)
                {
                    if (suggestions.Any(x => x.ScientificNameId.Equals(hit.ScientificNameId))) continue;
                    suggestions.Add(new ArtskartTaxon
                    {
                        ScientificNameId = hit.ScientificNameId,
                        PopularName = hit.PopularName,
                        MatchedName = hit.ScientificName,
                        ScientificName = hit.ScientificName,
                        TaxonCategory = hit.TaxonRank == nameof(Constants.TaxonCategoriesEn.Species) ? Constants.TaxonCategoriesEn.Species :
                                        hit.TaxonRank == nameof(Constants.TaxonCategoriesEn.SubSpecies) ? Constants.TaxonCategoriesEn.SubSpecies :
                                        hit.TaxonRank == nameof(Constants.TaxonCategoriesEn.Variety) ? Constants.TaxonCategoriesEn.Variety : 0
                    });
                }

                // Add assessmentIds to species, subspecies and variety
                foreach (var item in suggestions.Select((hit, i) => new { i, hit }))
                {
                    if (
                        item.hit.TaxonCategory == Constants.TaxonCategoriesEn.Species ||
                        item.hit.TaxonCategory == Constants.TaxonCategoriesEn.SubSpecies ||
                        item.hit.TaxonCategory == Constants.TaxonCategoriesEn.Variety
                        )
                    {
                        var ids = query
                            .Where(x => x.ScientificNameId == item.hit.ScientificNameId)
                            .Select(x => new
                            {
                                id = x.Id,
                                area = x.AssessmentArea,
                                category = x.Category,
                                speciesGroup = x.SpeciesGroup,
                                speciesGroupIconUrl = speciesgroupDict[x.SpeciesGroup]["image"],
                                scientificName = x.ScientificName
                            })
                            .ToArray();

                        item.hit.assessments = ids;
                        if (ids.Length != 1) continue;
                        if (item.hit.MatchedName == item.hit.ScientificName)// artskart har ikke underartsepitet o.l. men vi har det her
                        {
                            item.hit.MatchedName = ids[0].scientificName;
                        }
                        item.hit.ScientificName = ids[0].scientificName;
                    }
                }

                if (artskartResult.Any() && suggestions.Any() != true)
                {
                    return Json(new List<object>() { new { message = "Her får du treff, men ingen av artene er behandlet i Rødlista for arter 2021" } });
                }
                else if (artskartResult.Any() != true && suggestions.Any() != true)
                {
                    return Json(new List<object>() { new { message = "Her får du ingen treff." } });
                }

                return Json(suggestions);
            }
            catch (Exception)
            {
                return Json(new List<object>() { });
            }
        }

        [Route("2021/{id:required}")]
        public async Task<IActionResult> Detail(int id)
        {
            var data = await DataRepository.GetSpeciesAssessments();

            var assessment = data.FirstOrDefault(x => x.Id == id);

            if (assessment == null)
                return NotFound();

            ViewBag.revisionid = null;

            ViewBag.kriterier = await GetResource("wwwroot/json/kriterier.json");

            ViewBag.glossary = await GetResource("wwwroot/json/glossary.json");

            ViewBag.categories = await GetResource("wwwroot/json/categories.json");

            ViewBag.habitat = await GetResource("wwwroot/json/habitat.json");

            ViewBag.speciesgroup = await GetResource("wwwroot/json/speciesgroup.json");

            ViewBag.impactfactors = await GetResource("wwwroot/json/impactfactors.json");

            return View("Species/2021/Assessment/SpeciesAssessment2021", assessment);
        }
        [Route("2021/{id:required}/{revisionid:required}")]
        public async Task<IActionResult> Detail(int id, int revisionid)
        {
            var data = await DataRepository.GetSpeciesAssessments();

            var assessment = data.FirstOrDefault(x => x.Id == id);

            if (assessment == null)
                return NotFound();

            if (assessment.Revisions == null || assessment.Revisions.All(x => x.Revision != revisionid))
            {
                return NotFound();
            }

            ViewBag.revisionid = revisionid;

            assessment = assessment.Revisions.Single(x => x.Revision == revisionid);

            ViewBag.kriterier = await GetResource("wwwroot/json/kriterier.json");

            ViewBag.glossary = await GetResource("wwwroot/json/glossary.json");

            ViewBag.categories = await GetResource("wwwroot/json/categories.json");

            ViewBag.habitat = await GetResource("wwwroot/json/habitat.json");

            ViewBag.speciesgroup = await GetResource("wwwroot/json/speciesgroup.json");

            ViewBag.impactfactors = await GetResource("wwwroot/json/impactfactors.json");

            return View("Species/2021/Assessment/SpeciesAssessment2021", assessment);
        }

        [Route("2021/Svalbard")]
        public async Task<IActionResult> Svalbard2021()
        {
            return await Index2021(new RL2021ViewModel() { Area = new[] { "S" }, IsCheck = new[] { "Area" } }, null, false);
        }

        [Route("2021/Norge")]
        public async Task<IActionResult> Norge2021()
        {
            return await Index2021(new RL2021ViewModel() { Area = new[] { "N" }, IsCheck = new[] { "Area" } }, null, false);
        }

        private static void SetupStatisticsViewModel(IList<SpeciesAssessment2021> data, RL2021ViewModel viewModel)
        {

            // STATISTICS.
            // INPUT dataset is an already filtered list, based on active filters in the view. 
            string[] relevantCategories = new string[] { "CR", "EN", "VU", "NT" };

            // CATEGORY
            var categories = data.Where(x => !string.IsNullOrEmpty(x.Category)).GroupBy(x => new
            {
                Category = x.Category[..2] // ignore degrees, ie "VUº = VU"
            }).Select(x => new KeyValuePair<string, int>(x.Key.Category, x.Count()));

            viewModel.Statistics.Categories = categories.ToDictionary(x => x.Key, x => x.Value);



            // For the following statistics, the active input should be filtered by only redlisted assessments. 
            // This needs to be re-inforced in the frontend-solution



            // SPECIES MAIN HABITAT

            // Fetch all habitat lists, flatten the lists and make it distinct to obtain all currently possible habitat names.
            var habitatNames = data
                .Where(x => relevantCategories.Contains(x.Category[..2]))
                .Select(x => x.MainHabitat)
                .SelectMany(x => x)
                .Distinct()
                .ToList();

            // For each of the habitatnames - count each occurence in the main dataset
            var habitatStats = habitatNames.Select(name => new KeyValuePair<string, int>(name, data.Count(x => x.MainHabitat.Contains(name))))
                .ToDictionary(x => x.Key, x => x.Value);
            viewModel.Statistics.Habitat = habitatStats;


            // REGION
            var regionNames = Helpers.SortedRegions();
            var regionStats = regionNames.Select(name => new KeyValuePair<string, int>(name, data
                .Where(x => relevantCategories.Contains(x.Category[..2]))
                .Select(x => x.RegionOccurrences)
                .SelectMany(x => x)
                .Where(x => x.Fylke == name && x.State == 0).Count()))
                .ToDictionary(x => x.Key, x => x.Value);
            viewModel.Statistics.Region = regionStats;

            viewModel.Statistics.RegionNames = regionNames;




            // CRITERIA

            var criteriaStrings = data
                .Where(x => !string.IsNullOrEmpty(x.CriteriaSummarized) && relevantCategories.Contains(x.Category[..2]))
                .Select(x => x.CriteriaSummarized);
            var criteria = new List<string> { "A", "B", "C", "D" }.Select(item => new KeyValuePair<string, int>(item, criteriaStrings.Count(x => x.Contains(item))));
            viewModel.Statistics.Criteria = criteria.ToDictionary(x => x.Key, x => x.Value);

            //Example on how to print out while working
            /*
            for (int i = 0; i < habitatNames.Count; ++i)
            {
                Console.WriteLine(habitatNames[i] + ": " + habitatStats[habitatNames[i]].ToString());
            }
            */



            // IMPACTFACTORS

            string excludedGroupingFactor = "Ingen trussel";
            string excludedPopulationScope = "En ubetydelig del av populasjonen påvirkes";
            string excludedSeverity = "Ubetydelig/ingen nedgang";

            var impactFactors = data
                .Where(x => relevantCategories.Contains(x.Category[..2]))
                .Select(x => x.ImpactFactors
                    .Where(x => x.GroupingFactor != excludedGroupingFactor &&
                        x.PopulationScope != excludedPopulationScope &&
                        x.Severity != excludedSeverity)
                    .Select(x => x.GroupingFactor)
                    .Distinct())
                .SelectMany(x => x)
                .GroupBy((x => x), (key, value) => new
                {
                    key,
                    value = value.Count()
                });

            viewModel.Statistics.ImpactFactors = new Dictionary<string, int>();

            foreach (var item in impactFactors)
            {
                viewModel.Statistics.ImpactFactors.Add(item.key, item.value);
            }
        }

        private static async Task<JObject> GetResource(string resourcePath)
        {
#if (DEBUG == true)
            if (_resourceCache.ContainsKey(resourcePath)) return _resourceCache[resourcePath];
#endif
            var json = await System.IO.File.ReadAllTextAsync(resourcePath);
            var jObject = JObject.Parse(json);
#if (DEBUG == true)
            if (!_resourceCache.ContainsKey(resourcePath)) _resourceCache.Add(resourcePath, jObject);
#endif
            return jObject;
        }
    }
}