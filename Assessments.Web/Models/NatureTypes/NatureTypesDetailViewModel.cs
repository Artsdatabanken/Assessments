using Assessments.Data.Models;
using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure.Enums;
using RodlisteNaturtyper.Core.Models;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesDetailViewModel(Assessment Assessment)
{
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
        AssessmentType = AssessmentType.NatureTypes2025
    };

    public List<Region> Regions { get; set; } = [];

    public CitationForAssessmentViewModel CitationForAssessmentViewModel { get; set; }
    
    public List<CodeItemViewModel> CodeItemViewModels { get; set; }
}