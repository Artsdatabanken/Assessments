using Assessments.Data.Models;
using Assessments.Shared.Constants;
using Assessments.Shared.DTOs.NatureTypes.Enums;
using Assessments.Shared.Extensions;
using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.Enums;
using RodlisteNaturtyper.Core.Models;
using RodlisteNaturtyper.Data.Models;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesDetailViewModel(Assessment Assessment)
{
    public List<Region> Regions { get; init; } = [];

    public List<CodeItemModel> CodeItemModels { get; init; }

    public FeedbackViewModel FeedbackViewModel => new()
    {
        AssessmentId = Assessment.Id,
        AssessmentName = Assessment.Name,
        ExpertGroup = Assessment.Committee.Name,
        Type = FeedbackType.NatureTypes,
        Year = 2025
    };
    
    public PageMenuViewModel PageMenuViewModel => new()
    {
        PageMenuContentId = NatureTypesConstants.PageMenuContentId,
        PageMenuExpandButtonText = NatureTypesConstants.Title2025,
        AssessmentType = AssessmentType.NatureTypes2025,
        TableOfContentsViewModel = new TableOfContentsViewModel
        {
            Contents =
            [
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(NatureTypesConstants.Headings.Description),
                    Title = NatureTypesConstants.Headings.Description,
                    ShouldShow = !string.IsNullOrEmpty(Assessment.DescriptionHtml)
                },
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(NatureTypesConstants.Headings.Summary),
                    Title = NatureTypesConstants.Headings.Summary,
                    ShouldShow = true
                },
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(NatureTypesConstants.Headings.CodeItems),
                    Title = NatureTypesConstants.Headings.CodeItems,
                    ShouldShow = CodeItemModels.Count != 0
                },
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(NatureTypesConstants.Headings.CriteriaInformation),
                    Title = NatureTypesConstants.Headings.CriteriaInformation,
                    ShouldShow = true
                },
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(NatureTypesConstants.Headings.AreaInformation),
                    Title = NatureTypesConstants.Headings.AreaInformation,
                    ShouldShow = true
                },
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(NatureTypesConstants.Headings.Regions),
                    Title = NatureTypesConstants.Headings.Regions,
                    ShouldShow = true
                },
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(NatureTypesConstants.Headings.References),
                    Title = NatureTypesConstants.Headings.References,
                    ShouldShow = Assessment.References.Count != 0
                },
                new TableOfContentsViewModel.Content
                {
                    ElementId = nameof(Constants.HeadingsNo.Citation),
                    Title = Constants.HeadingsNo.Citation,
                    ShouldShow = true
                }
            ]
        }
    };
    
    public CitationForAssessmentViewModel CitationForAssessmentViewModel { get; init; }
    
    public CategoryDescriptionViewModel CategoryDescriptionViewModel = new()
    {
        CategoryShort = Assessment.Category.ToString(),
        CategoryBar = [.. Enum.GetValues<Category>().Except([Category.NA]).Select(x => new CategoryBarListElement
        {
            Name = x.GetDescription(),
            NameShort = x.ToString()
        }).Reverse()],
        MethodUrl = "https://artsdatabanken.no/Pages/258616"
    };

    public List<CategoryCriteriaType> CategoryCriteriaTypes { get; set; } = [];
}
