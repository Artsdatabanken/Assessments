using System.Linq.Expressions;
using System.Text.Json;
using Assessments.Mapping.NatureTypes.Model;
using Assessments.Shared.Constants;
using Assessments.Shared.DTOs.NatureTypes.Enums;
using Assessments.Shared.Extensions;
using Assessments.Shared.Helpers;
using Assessments.Shared.Interfaces;
using Assessments.Shared.Options;
using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.Enums;
using Assessments.Web.Models;
using Assessments.Web.Models.NatureTypes;
using Assessments.Web.Models.NatureTypes.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using X.PagedList.Extensions;
using static Assessments.Shared.Extensions.ExpressionExtensions;

namespace Assessments.Web.Controllers;

[FeatureGate(FeatureManagementConstants.EnableNatureTypes)]
[Route("naturtyper")]
public class NatureTypesController(INatureTypesRepository repository, IOptions<ApplicationOptions> options) : BaseController<NatureTypesController>
{
    [HttpGet]
    public IActionResult Home(string key)
    {
        // midlertidig tilgangskontroll før lansering
        if (options.Value.NatureTypes.TemporaryAccessKey == null || key != options.Value.NatureTypes.TemporaryAccessKey)
            return NotFound();

        HttpContext.Response.Cookies.Append(NatureTypesConstants.TemporaryAccessCookieName, options.Value.NatureTypes.TemporaryAccessKey, new CookieOptions { Expires = DateTime.Now.AddDays(90) });

        return RedirectToAction("List");
    }

    [CookieRequired]
    [Route("2025")]
    public async Task<IActionResult> List(NatureTypesListParameters parameters, int? page, bool export)
    {
        var query = repository.GetAssessments();

        var regions = await repository.GetRegions();
        var topics = await repository.GetNinCodeTopics();
        var codeItems = await repository.GetCodeItems();

        query = await ApplyParametersToList(parameters, query, regions, codeItems, repository);

        query = parameters.SortBy switch
        {
            SortByEnum.Category => query.OrderBy(x => x.Category),
            SortByEnum.NinCode => query.OrderBy(x => x.NinCodeTopic.ShortCode),
            _ => query.OrderBy(x => string.IsNullOrEmpty(x.PopularName)).ThenBy(x => x.PopularName).ThenBy(x => x.Name)
        };

        if (export)
        {
            var assessmentExports = Mapper.ProjectTo<NatureTypeAssessmentExport>(query, new
            {
                committeeUsers = await repository.GetCommitteeUsers()
            });

            return new FileStreamResult(ExportHelper.GenerateNatureTypeAssessment2025Export(assessmentExports, Request.GetDisplayUrl()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "rødlista-naturtyper-2025.xlsx"
            };
        }

        var pagedList = query.ToPagedList(page ?? 1, DefaultPageSize);

        var viewModel = new NatureTypesListViewModel(pagedList)
        {
            Name = parameters.Name,
            Area = parameters.Area,
            Category = parameters.Category,
            Topic = parameters.Topic,
            Region = parameters.Region,
            Criteria = parameters.Criteria,
            CodeItem = parameters.CodeItem,
            Meta = parameters.Meta,
            IsCheck = parameters.IsCheck,
            Regions = regions,
            NinCodeTopics = topics,
            CodeItems = await repository.GetMainCodeItems(),
            ListViewViewModel = new ListViewViewModel
            {
                Results = pagedList.Select(_ => new ListViewViewModel.Result()),
                AssessmentType = AssessmentType.NatureTypes2025,
                View = parameters.View
            }
        };

        if (!string.IsNullOrEmpty(parameters.View) && parameters.View.Equals("stat"))
            viewModel.NatureTypesStatisticsViewModel = await SetupStatisticsViewModel(query);

        return View(viewModel);
    }

    [CookieRequired]
    [Route("2025/{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        var assessment = await repository.GetAssessment(id);

        if (assessment == null)
            return NotFound();

        var committeeUsers = await repository.GetCommitteeUsers();
        var assessmentCodeItemNodes = await repository.GetAssessmentCodeItemNodes(assessment.Id);

        var viewModel = new NatureTypesDetailViewModel(assessment)
        {
            CategoryCriteriaTypes = NatureTypesExtensions.GetCategoryCriteriaTypes(assessment.CategoryCriteria),
            CitationForAssessmentViewModel = new CitationForAssessmentViewModel
            {
                AssessmentName = $"{assessment.Committee.Name}: Vurdering av {assessment.Name}.",
                PublicationText = NatureTypesConstants.CitationSummary,
                AssessmentYear = 2025,
                FirstPublished = NatureTypesConstants.PublishedDate.ToString("d.M.yyy"),
                ExpertGroupMembers = committeeUsers.GetCitation(assessment.Committee, includeDetails: false),
                HasBackToTopLink = true
            },
            CodeItemNodeDtos = assessmentCodeItemNodes ?? []
        };

        await GetChanges(assessment, viewModel);

        return View(viewModel);
    }

    [HttpGet]
    [Route("2025/[action]")]
    public async Task<IActionResult> Suggestions()
    {
        var ninCodeTopicSuggestions = await repository.GetNinCodeTopicSuggestions();
        var ninCodeTopics = ninCodeTopicSuggestions.Select(x => x.Key).OrderBy(x => x);

        return Json(ninCodeTopics);
    }

    private static async Task<IQueryable<Assessment>> ApplyParametersToList(NatureTypesListParameters parameters, IQueryable<Assessment> query, List<Region> regions, List<CodeItem> codeItems, INatureTypesRepository repository)
    {
        if (!string.IsNullOrEmpty(parameters.Name?.StripHtml().Trim()))
        {
            var searchParameter = parameters.Name.StripHtml().Trim();
            searchParameter = new string([.. searchParameter.Take(175)]);

            var ninCodeTopicSuggestions = await repository.GetNinCodeTopicSuggestions();
            var topic = ninCodeTopicSuggestions.FirstOrDefault(x => x.Key.Equals(searchParameter, StringComparison.OrdinalIgnoreCase));

            if (topic.Key == null)
            {
                var words = searchParameter.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 1)
                {
                    query = query.Where(words.Aggregate<string, Expression<Func<Assessment, bool>>>(null, (current, keyword) => Combine(current, c => c.PopularName.Contains(keyword), CombineExpressionType.AndAlso)));
                }
                else
                {
                    query = query.Where(x => x.PopularName.Contains(searchParameter) || x.ShortCode.Contains(searchParameter) || x.LongCode == searchParameter);
                }
            }
            else
            {
                // søk på valgt forslag for tema
                query = query.Where(x => x.NinCodeTopicId == topic.Value);
            }

            parameters.Name = searchParameter;
        }

        if (parameters.Area.Length != 0)
        {
            var enumerable = parameters.Area.ToEnumerable<AssessmentRegion>();
            var assessmentRegions = enumerable.Aggregate<AssessmentRegion, Expression<Func<Assessment, bool>>>(null, (current, category) => Combine(current, c => c.Region == category));

            if (assessmentRegions != null)
                query = query.Where(assessmentRegions);
        }

        if (parameters.Category.Length != 0)
        {
            var categories = parameters.Category.ToEnumerable<Category>().Aggregate<Category, Expression<Func<Assessment, bool>>>(null, (current, category) => Combine(current, c => c.Category == category));

            if (categories != null)
                query = query.Where(categories);
        }

        if (parameters.Topic.Length != 0)
            query = query.Where(x => parameters.Topic.Contains(x.NinCodeTopic.Description));

        if (parameters.Region.Length != 0)
        {
            var selectedRegionIds = regions.Where(x => parameters.Region.Contains(x.Name)).Select(y => y.Id).ToArray();

            query = query.Where(x => x.Regions.Any(y => selectedRegionIds.Contains(y.Id)));
        }

        if (parameters.Criteria.Length != 0)
        {
            List<string> criteria = [];

            if (parameters.Criteria.Contains("A"))
                criteria.AddRange(["A1", "A2a", "A2b"]);
            if (parameters.Criteria.Contains("B"))
                criteria.AddRange(["B1", "B2", "B3"]);
            if (parameters.Criteria.Contains("C"))
                criteria.AddRange(["C1", "C2a", "C2b"]);
            if (parameters.Criteria.Contains("D"))
                criteria.AddRange(["D1", "D2a", "D2b"]);
            if (parameters.Criteria.Contains("E"))
                criteria.AddRange([" E"]);

            if (criteria.Count != 0)
            {
                query = query.Where(criteria.Aggregate<string, Expression<Func<Assessment, bool>>>(null, (current, keyword) => Combine(current, c => c.CategoryCriteria.Contains(keyword))));
            }
        }

        if (parameters.CodeItem.Length != 0)
        {
            List<int> selectedCodeItemIds = [];

            foreach (var codeItemId in parameters.CodeItem)
            {
                if (!int.TryParse(codeItemId, out var id))
                    continue;

                var codeItem = codeItems.FirstOrDefault(x => x.Id == id);

                if (codeItem == null)
                    continue;

                selectedCodeItemIds.AddRange(codeItems.Where(x => x.IdNr.StartsWith(codeItem.IdNr)).Select(x => x.Id));
            }

            if (selectedCodeItemIds.Count != 0)
            {
                query = query.Where(x => x.CodeItems.Any(y => selectedCodeItemIds.Contains(y.CodeItemId)));
            }
        }

        return query;
    }

    private async Task<NatureTypesStatisticsViewModel> SetupStatisticsViewModel(IQueryable<Assessment> assessments)
    {
        var viewModel = new NatureTypesStatisticsViewModel();

        var categoryStats = await assessments.GroupBy(x => x.Category).Select(x => new { x.Key, Count = x.Count() }).ToListAsync();

        foreach (var category in Enum.GetValues<Category>().Where(x => x != Category.NA))
        {
            var stats = categoryStats.FirstOrDefault(x => x.Key == category);
            viewModel.Categories.Add(category, stats?.Count ?? 0);
        }

        var regionStats = await assessments.SelectMany(x => x.Regions)
            .GroupBy(y => y.Id).Select(x => new { x.Key, Count = x.Count() }).ToListAsync();

        foreach (var region in await repository.GetRegions())
        {
            var stats = regionStats.FirstOrDefault(x => x.Key == region.Id);
            viewModel.Regions.Add(region.Name, stats?.Count ?? 0);
        }

        foreach (var categoryCriteriaType in Enum.GetValues<CategoryCriteriaType>())
            viewModel.CategoryCriteriaType.Add(categoryCriteriaType, 0);

        foreach (var assessment in assessments)
        {
            var categoryCriteriaTypes = NatureTypesExtensions.GetCategoryCriteriaTypes(assessment.CategoryCriteria);

            foreach (var categoryCriteriaType in categoryCriteriaTypes)
                viewModel.CategoryCriteriaType[categoryCriteriaType] += 1;
        }

        foreach (var codeItem in await repository.GetMainCodeItems())
        {
            viewModel.CodeItems.Add(codeItem.Description, await assessments.CountAsync(x => x.CodeItems.Any(y => y.CodeItem.IdNr.StartsWith(codeItem.IdNr))));
        }

        return viewModel;
    }

    private async Task GetChanges(Assessment assessment, NatureTypesDetailViewModel viewModel)
    {
        var lookups = await DataRepository.GetData<NatureTypes2018To2025Lookup>(DataFilenames.NatureTypes2018To2025);

        var lookup = lookups.FirstOrDefault(x => x.Id2025 == assessment.Id);

        if (lookup != null && lookups.Any(x => x.Id2018 == lookup.Id2018))
        {
            var changes = lookups.Where(x => x.Id2018 == lookup.Id2018);
            var assessmentIds = changes.Select(y => y.Id2025).ToList();

            var assessmentsWithChanges = repository.GetAssessments().Where(x => assessmentIds.Contains(x.Id)).Select(x => new
            {
                x.PopularName,
                x.Category
            }).ToList();

            var categoryDescription2018 = lookup.Category2018.ToEnum(Category.NA).GetDescription();

            // LC i 2018 har beskrivelsen "intakt"
            if (lookup.Category2018 == "LC")
                categoryDescription2018 = "intakt";

            var nodes = new[] { new
            {
                Name = lookup.Name2018,
                Color = lookup.Category2018.ToEnum(Category.NA).GetColor(),
                Category = lookup.Category2018,
                CategoryDescription = categoryDescription2018
            }}.ToList();

            foreach (var assessmentsWithChange in assessmentsWithChanges)
            {
                nodes.Add(new
                {
                    Name = assessmentsWithChange.PopularName,
                    Color = assessmentsWithChange.Category.GetColor(),
                    Category = assessmentsWithChange.Category.ToString(),
                    CategoryDescription = assessmentsWithChange.Category.GetDescription()
                });
            }

            var target = 1;
            var data = new
            {
                Nodes = nodes,
                Links = assessmentsWithChanges.Select(_ => new
                {
                    Source = 0,
                    Target = target++,
                    Value = 1
                })
            };

            viewModel.HasChanges = true;
            viewModel.Changes = JsonSerializer.Serialize(data, JsonSerializerOptions.Web);
        }
    }
}