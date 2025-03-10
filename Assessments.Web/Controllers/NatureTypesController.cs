using System.Linq.Expressions;
using Assessments.Data.Models;
using Assessments.Shared.Options;
using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.NatureTypes;
using Assessments.Web.Models;
using Assessments.Web.Models.NatureTypes;
using Default;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;
using X.PagedList.Extensions;

namespace Assessments.Web.Controllers;

[NotReadyForProduction]
[Route("naturtyper")]
public class NatureTypesController : BaseController<NatureTypesController>
{
    private readonly Container _context;

    public NatureTypesController(IOptions<ApplicationOptions> options)
    {
        _context = new Container(options.Value.NatureTypes.ODataUrl);
        _context.BuildingRequest += (_, e) => e.Headers.Add("X-API-KEY", options.Value.NatureTypes.ODataApiKey);
    }

    public IActionResult Home()
    {
        return View();
    }

    [Route("2025")]
    public IActionResult List(NatureTypesListViewModelParameters parameters, int? page)
    {
        IQueryable<Assessment> assessments = _context.Assessments;

        if (!string.IsNullOrEmpty(parameters.Name))
            assessments = assessments.Where(x => x.Name.Contains(parameters.Name) || x.ShortCode == parameters.Name);

        if (parameters.Category.Count != 0)
        {
            var categories = parameters.Category.Aggregate<Category, Expression<Func<Assessment, bool>>>(null, (current, category) => ExpressionHelpers.Combine(current, c => c.Category == category));

            if (categories != null)
                assessments = assessments.Where(categories);
        }

        var pagedList = assessments.ToPagedList(page ?? 1, DefaultPageSize);

        var viewModel = new NatureTypesListViewModel(pagedList)
        {
            Category = parameters.Category.Count != 0 ? parameters.Category : []
        };

        return View(viewModel);
    }

    [Route("2025/{id:int}")]
    public IActionResult Detail(int id)
    {
        var assessment = _context.Assessments.Expand(x => x.Committee).FirstOrDefault(c => c.Id == id);

        if (assessment == null)
            return NotFound();

        var viewModel = new NatureTypesDetailViewModel(assessment)
        {
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