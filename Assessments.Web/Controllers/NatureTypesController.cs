using System.Linq.Expressions;
using Assessments.Mapping.NatureTypes.Model;
using Assessments.Shared.Constants;
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
        if (string.IsNullOrEmpty(key))
            return View();

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
            SortByEnum.Name => query.OrderBy(x => x.Name),
            _ => query.OrderBy(x => x.NinCodeTopic.ShortCode)
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
            CodeItems = codeItems,
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
                AssessmentName = assessment.Name,
                AssessmentYear = 2025,
                ExpertCommittee = assessment.Committee.Name,
                FirstPublished = "2025",
                YearPreviousAssessment = 2018,
                ExpertGroupMembers = committeeUsers.GetCitation(assessment.Committee),
                HasBackToTopLink = true
            },
            CodeItemNodeDtos = assessmentCodeItemNodes ?? []
        };

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
                    query = query.Where(words.Aggregate<string, Expression<Func<Assessment, bool>>>(null, (current, keyword) => Combine(current, c => c.Name.Contains(keyword), CombineExpressionType.AndAlso)));
                }
                else
                {
                    query = query.Where(x => x.Name.Contains(searchParameter) || x.ShortCode.Contains(searchParameter) || x.LongCode == searchParameter);
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

    private static async Task<NatureTypesStatisticsViewModel> SetupStatisticsViewModel(IQueryable<Assessment> assessments)
    {
        var viewModel = new NatureTypesStatisticsViewModel();

        var categories = await assessments.GroupBy(x => x.Category).Select(x => new { x.Key, Count = x.Count() }).ToListAsync();

        foreach (var category in Enum.GetValues<Category>().Where(x => x != Category.NA))
        {
            var statistics = categories.FirstOrDefault(x => x.Key == category);
            viewModel.Categories.Add(category, statistics?.Count ?? 0);
        }

        return viewModel;
    }
}