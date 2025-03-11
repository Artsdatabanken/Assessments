using System.Drawing;
using System.Linq.Expressions;
using Assessments.Data.Models;
using Assessments.Shared.Extensions;
using Assessments.Shared.Interfaces;
using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.AlienSpecies;
using Assessments.Web.Models;
using Assessments.Web.Models.NatureTypes;
using Assessments.Web.Models.NatureTypes.Enums;
using Microsoft.AspNetCore.Mvc;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using X.PagedList.Extensions;

namespace Assessments.Web.Controllers;

[NotReadyForProduction]
[Route("naturtyper")]
public class NatureTypesController(INatureTypesRepository repository) : BaseController<NatureTypesController>
{
    public IActionResult Home()
    {
        return View();
    }

    [Route("2025")]
    public IActionResult List(NatureTypesListViewModelParameters parameters, int? page)
    {
        var assessments = repository.GetAssessments();
        
        if (!string.IsNullOrEmpty(parameters.Name))
            assessments = assessments.Where(x => x.Name.Contains(parameters.Name) || x.ShortCode == parameters.Name);

        if (parameters.Category.Count != 0)
        {
            var categories = parameters.Category.Aggregate<Category, Expression<Func<Assessment, bool>>>(null, (current, category) => ExpressionExtensions.Combine(current, c => c.Category == category));

            if (categories != null)
                assessments = assessments.Where(categories);
        }

        if (parameters.Committee.Count != 0)
            assessments = assessments.Where(x => parameters.Committee.ToArray().Contains(x.Committee.Name));

        if (parameters.Region.Count != 0)
            assessments = assessments.Where(x => x.Regions.Any(x => parameters.Region.ToArray().Contains(x.Id)));

        if (parameters.Area != null)
            assessments = assessments.Where(x => x.Region == parameters.Area);

        assessments = parameters.SortBy switch
        {
            SortByEnum.Category => assessments.OrderBy(x => x.Category),
            SortByEnum.Committee => assessments.OrderBy(x => x.Committee.Name),
            _ => assessments.OrderBy(x => x.Name)
        };

        var pagedList = assessments.ToPagedList(page ?? 1, DefaultPageSize);
        
        var viewModel = new NatureTypesListViewModel(pagedList)
        {
            Category = parameters.Category,
            Committee = parameters.Committee,
            Region = parameters.Region,
            Committees = repository.GetCommittees(),
            Regions = repository.GetRegions()
        };

        return View(viewModel);
    }

    [Route("2025/{id:int}")]
    public IActionResult Detail(int id)
    {
        var assessment = repository.GetAssessment(id);

        if (assessment == null)
            return NotFound();

        var committeeUsers = repository.GetCommitteeUsers().Where(x => x.CommitteeId == assessment.CommitteeId).ToList();

        var viewModel = new NatureTypesDetailViewModel(assessment)
        {
            Citation = committeeUsers.GetCitation(assessment.Committee.Name),
            FeedbackViewModel = new FeedbackViewModel
            {
                AssessmentId = assessment.Id,
                AssessmentName = assessment.Name,
                ExpertGroup = assessment.Committee.Name,
                Type = FeedbackType.NatureTypes,
                Year = 2025
            }
        };

        return View(viewModel);
    }
}