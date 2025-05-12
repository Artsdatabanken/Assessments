using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using X.PagedList.Extensions;
using static Assessments.Shared.Extensions.ExpressionExtensions;

namespace Assessments.Web.Controllers;

[FeatureGate(FeatureManagementConstants.PublicAccessPeriodNatureTypes)]
[Route("naturtyper")]
public class NatureTypesController(INatureTypesRepository repository, IOptions<ApplicationOptions> options) : BaseController<NatureTypesController>
{
    // TODO: vis landingssiden før lansering av rødlista for naturtyper 2025
    //public IActionResult Home() => View();

    [CookieRequired]
    [Route("2025")]
    public async Task<IActionResult> List(NatureTypesListParameters parameters, int? page)
    {
        var assessments = repository.GetAssessments();
        var regions = repository.GetRegions();
        var topics = repository.GetNinCodeTopics();

        assessments = ApplyParametersToList(parameters, assessments, regions, topics);

        assessments = parameters.SortBy switch
        {
            SortByEnum.Category => assessments.OrderBy(x => x.Category),
            _ => assessments.OrderBy(x => x.Name)
        };

        var pagedList = assessments.ToPagedList(page ?? 1, DefaultPageSize);

        var viewModel = new NatureTypesListViewModel(pagedList)
        {
            Name = parameters.Name,
            Category = parameters.Category,
            Topic = parameters.Topic,
            Region = parameters.Region,
            Area = parameters.Area,
            Meta = parameters.Meta,
            IsCheck = parameters.IsCheck,
            Regions = regions,
            NinCodeTopics = topics,
            ListViewViewModel = new ListViewViewModel
            {
                Results = pagedList.Select(_ => new ListViewViewModel.Result()),
                AssessmentType = AssessmentType.NatureTypes2025,
                View = parameters.View
            }
        };

        if (!string.IsNullOrEmpty(parameters.View) && parameters.View.Equals("stat"))
        {
            viewModel.NatureTypesStatisticsViewModel = await SetupStatisticsViewModel(assessments.ToString());
        }

        return View(viewModel);
    }

    [CookieRequired]
    [Route("2025/{id:int}")]
    public IActionResult Detail(int id)
    {
        var assessment = repository.GetAssessment(id);

        if (assessment == null)
            return NotFound();

        var committeeUsers = repository.GetCommitteeUsers().Where(x => x.CommitteeId == assessment.CommitteeId).ToList();

        var codeItemModels = repository.GetAssessmentCodeItemModels(assessment.Id);

        var viewModel = new NatureTypesDetailViewModel(assessment)
        {
            Regions = repository.GetRegions(),
            CodeItemModels = codeItemModels,
            CategoryCriteriaTypes = NatureTypesHelper.GetCategoryCriteriaTypes(assessment.CategoryCriteria),
            CitationForAssessmentViewModel = new CitationForAssessmentViewModel
            {
                AssessmentName = assessment.Name,
                AssessmentYear = 2025,
                ExpertCommittee = assessment.Committee.Name,
                FirstPublished = "2025",
                YearPreviousAssessment = 2018,
                ExpertGroupMembers = committeeUsers.GetCitation(assessment.Committee.Name),
                HasBackToTopLink = true
            }
        };

        return View(viewModel);
    }

    public IActionResult EnableNaturetypes([Required] string key)
    {
        if (options.Value.NatureTypes.TemporaryAccessKey == null || key != options.Value.NatureTypes.TemporaryAccessKey)
            return NotFound();

        HttpContext.Response.Cookies.Append(NatureTypesConstants.TemporaryAccessCookieName, options.Value.NatureTypes.TemporaryAccessKey, new CookieOptions { Expires = DateTime.Now.AddDays(14) });

        return RedirectToAction("List");
    }

    [Route("2025/[action]")]
    [ResponseCache(Duration = 86400)]
    public IActionResult Suggestions()
    {
        var codeItems = repository.GetCodeItems()
            .Where(x => x.ParentId != 0)
            .Select(x => new string($"{x.Description} (todo Påvirkningsfaktor"));

        var ninCodeTopics = repository.GetNinCodeTopics().Select(x => new string($"{x.Name} ({x.Description})"));

        var suggestions = codeItems.Concat(ninCodeTopics).OrderBy(x => x);

        return Json(suggestions);
    }

    private static IQueryable<Assessment> ApplyParametersToList(NatureTypesListParameters parameters, IQueryable<Assessment> assessments, List<Region> regions, List<NinCodeTopic> topics)
    {
        if (!string.IsNullOrEmpty(parameters.Name?.StripHtml().Trim()))
        {
            var searchParameter = parameters.Name.StripHtml();

            const string quotationMark = "\"";

            if (searchParameter.StartsWith(quotationMark) && searchParameter.EndsWith(quotationMark))
            {
                searchParameter = searchParameter.Replace(quotationMark, string.Empty);
                assessments = assessments.Where(x => x.Name == searchParameter);
            }
            else
            {
                var topicSuggestions = topics.Select(x => new { Text = new string($"{x.Name} ({x.Description})"), x.Id});
                var topic = topicSuggestions.FirstOrDefault(x => x.Text.Equals(searchParameter, StringComparison.OrdinalIgnoreCase));

                if (topic != null)
                {
                    assessments = assessments.Where(x => x.NinCodeTopicId == topic.Id);
                }
                else
                {
                    //var searchParameters = searchParameter.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
                    //var searchTerms = searchParameters.Aggregate<string, Expression<Func<Assessment, bool>>>(null, (current, term) => Combine(current, x => x.Name.Contains(term) || x.ShortCode.Contains(searchParameter) || x.LongCode == searchParameter, CombineExpressionType.AndAlso));

                    assessments = assessments.Where(x => x.Name.Contains(searchParameter) || x.ShortCode.Contains(searchParameter) || x.LongCode == searchParameter);
                }
            }

            parameters.Name = searchParameter;
        }

        if (parameters.Area.Length != 0)
        {
            var enumerable = parameters.Area.ToEnumerable<AssessmentRegion>();
            var assessmentRegions = enumerable.Aggregate<AssessmentRegion, Expression<Func<Assessment, bool>>>(null, (current, category) => Combine(current, c => c.Region == category));

            if (assessmentRegions != null)
                assessments = assessments.Where(assessmentRegions);
        }

        if (parameters.Category.Length != 0)
        {
            var categories = parameters.Category.ToEnumerable<Category>().Aggregate<Category, Expression<Func<Assessment, bool>>>(null, (current, category) => Combine(current, c => c.Category == category));

            if (categories != null)
                assessments = assessments.Where(categories);
        }

        if (parameters.Topic.Length != 0)
            assessments = assessments.Where(x => parameters.Topic.ToArray().Contains(x.NinCodeTopic.Description));

        if (parameters.Region.Length != 0)
        {
            var selectedRegionIds = regions.Where(x => parameters.Region.Contains(x.Name)).Select(y => y.Id).ToArray();

            assessments = assessments.Where(x => x.Regions.Any(y => selectedRegionIds.Contains(y.Id)));
        }

        return assessments;
    }

    private async Task<NatureTypesStatisticsViewModel> SetupStatisticsViewModel(string queryUrl)
    {
        var categoryStatistics = await repository.GetCategoryStatistics(new Uri(queryUrl));

        var viewModel = new NatureTypesStatisticsViewModel();

        foreach (var category in Enum.GetValues<Category>())
        {
            if (category == Category.NA)
                continue;

            var statistics = categoryStatistics.FirstOrDefault(x => x.Category == category.ToString());
            viewModel.Categories.Add(category, statistics?.Count ?? 0);
        }

        return viewModel;
    }
}