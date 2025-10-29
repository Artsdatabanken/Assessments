using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.AlienSpecies;
using Assessments.Web.Infrastructure.Services;
using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Shared.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList.Extensions;
using Assessments.Web.Models.AlienSpecies;

namespace Assessments.Web.Controllers;

[Route("fremmedartslista")]
public class AlienSpeciesController(AttachmentRepository attachmentRepository, ArtskartApiService artskartApiService, IStringLocalizer<AlienSpeciesController> localizer)
    : BaseController<AlienSpeciesController>
{
    [Route("2023")]
    public async Task<IActionResult> Index(AlienSpeciesListViewModel viewModel, int? page, bool export)
    {
        // permanent redirect to fix google cache
        if (viewModel.TaxonRank.Length != 0 && viewModel.TaxonRank[0] == "tvi")
        {
            return RedirectToActionPermanent("Index", new { TaxonRank = "AssessedAtSameRank" });
        }

        var query = await DataRepository.GetAlienSpeciesAssessments();
        var unfilteredQuery = query;

        query = QueryHelpers.ApplyParameters(viewModel, query);

        if (export)
            return GetExport(query);
            
        if (viewModel.View == "stat")
        {
            var statistics = new StatisticsHelper(query, unfilteredQuery);
            viewModel.Statistics = statistics.GetStatistics();
        }
        else
        {
            viewModel.Results = query.ToPagedList(page ?? 1, DefaultPageSize);
        }

        return View("2023/AlienSpeciesIndex", viewModel);
    }

    [Route("2023/{id:required:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        var data = await DataRepository.GetAlienSpeciesAssessments();

        var assessment = data.FirstOrDefault(x => x.Id == id);

        if (assessment == null)
            return NotFound();

        var expertGroupMembers = await DataRepository.GetData<AlienSpeciesAssessment2023ExpertGroupMember>(DataFilenames.AlienSpeciesExpertCommitteeMembers);

        // members by assessment id (if available)
        var assessmentExpertGroupMembers = expertGroupMembers.Where(x => x.Id == assessment.Id).OrderBy(x => x.CitationOrder).ToList();

        // members by expertgroup
        if (!assessmentExpertGroupMembers.Any())
        {
            assessmentExpertGroupMembers = expertGroupMembers.Where(x => x.ExpertGroup == assessment.ExpertGroup).DistinctBy(x => x.FullName)
                .OrderBy(x => new List<string> { "Leder", "Medlem" }.IndexOf(x.ExpertGroupRole)).ThenBy(x => x.LastName).ToList();
        }

        var viewModel = new AlienSpeciesDetailViewModel(assessment)
        {
            ExpertGroupMembers = assessmentExpertGroupMembers
        };

        return View("2023/AlienSpeciesDetail", viewModel);
    }

    [Route("2023/Attachment/{attachmentId:required:int}")]
    public async Task<IActionResult> Attachment(int attachmentId)
    {
        var data = await DataRepository.GetAlienSpeciesAssessments();
        var assessment = data.FirstOrDefault(x => x.Attachments.Any(y => y.Id == attachmentId));

        if (assessment == null)
            return NotFound();

        var attachment = assessment.Attachments.Single(x => x.Id == attachmentId);

        var stream = await attachmentRepository.GetFileStream(DataFilenames.CalculateAlienSpecies2023AttachmentFilePath(attachmentId, attachment.FileName));

        if (stream == null)
            return NotFound();

        return new FileStreamResult(stream, attachment.MimeType)
        {
            FileDownloadName = attachment.FileName
        };
    }

    private IActionResult GetExport(IEnumerable<AlienSpeciesAssessment2023> query)
    {
        var assessmentsForExport = Mapper.Map<IEnumerable<AlienSpeciesAssessment2023Export>>(query.ToList());

        return new FileStreamResult(ExportHelper.GenerateAlienSpeciesAssessment2023Export(assessmentsForExport, Request.GetDisplayUrl()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "fremmedartslista-2023.xlsx"
        };
    }

    [HttpGet, Route("2023/suggestions")]
    public async Task<IActionResult> Suggestion([FromQuery] string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return BadRequest(localizer["search-validation"].Value);
        }

        var name = search.Trim().ToLower();

        var query = await DataRepository.GetAlienSpeciesAssessments();

        // At the moment the artskart hits are used to indicate to the user that there exists taxons with the searched name, but that it may not be present in the alien species list. 
        // We might want to extend this method to also add a few of the most relevant hits even though they are not in the alien species list. 
        try
        {
            var artskartResult = await artskartApiService.Get<List<ArtskartTaxon>>($"data/SearchTaxons?maxCount=20&name={name}");

            // Remove species not present in alien species list
            var suggestions = artskartResult.Where(x =>
                (x.TaxonCategory != Constants.TaxonCategoriesEn.Species &&                          // Not species
                 x.TaxonCategory != Constants.TaxonCategoriesEn.SubSpecies &&                        // Not subspecies
                 x.TaxonCategory != Constants.TaxonCategoriesEn.Variety &&                          // Not variety
                 x.TaxonCategory != Constants.TaxonCategoriesEn.Form) &&                          // Not form
                query.Any(y => y.NameHiearchy.Any(z => z.ScientificName.Contains(x.ScientificName))) ||    // Check if none of the above -> exists in taxonomic rank
                query.Any(y => y.ScientificName.ScientificNameId == x.ScientificNameId)).ToList();                // or match on scientific name id: exact match on species/subsp./var.

            // Add assessments if they are in alien species list, but not in artskart. "Subsp." and "var." are not included in scientific names in artskart.
            var alienSpeciesHits = QueryHelpers.ApplySearch(search, query);

            foreach (var hit in alienSpeciesHits)
            {
                if (suggestions.Any(x => x.ScientificNameId.Equals(hit.ScientificName.ScientificNameId))) continue;
                suggestions.Add(new ArtskartTaxon
                {
                    ScientificNameId = (int)hit.ScientificName.ScientificNameId,
                    PopularName = hit.VernacularName,
                    MatchedName = hit.ScientificName.ScientificName,
                    ScientificName = hit.ScientificName.ScientificName,
                    TaxonCategory = (int)hit.ScientificName.ScientificNameRank
                });
            }

            // Add assessmentIds to species, subspecies, form and variety
            foreach (var item in suggestions.Select((hit, i) => new { i, hit }))
            {
                if (
                    item.hit.TaxonCategory == Constants.TaxonCategoriesEn.Species ||
                    item.hit.TaxonCategory == Constants.TaxonCategoriesEn.SubSpecies ||
                    item.hit.TaxonCategory == Constants.TaxonCategoriesEn.Variety ||
                    item.hit.TaxonCategory == Constants.TaxonCategoriesEn.Form
                )
                {
                    var ids = query
                        .Where(x => x.ScientificName.ScientificNameId == item.hit.ScientificNameId)
                        .Select(x => new
                        {
                            id = x.Id,
                            area = x.EvaluationContext,
                            category = x.Category,
                            speciesGroup = x.SpeciesGroup,
                            speciesGroupIconUrl = AlienSpeciesHelpers.GetSpeciesGroup(x.SpeciesGroup.DisplayName()).ImageUrl,
                            scientificName = x.ScientificName.ScientificName
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
                return Json(new List<object>() { new { message = "Her får du treff, men ingen av artene er behandlet i Fremmedartslista 2023" } });
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
}