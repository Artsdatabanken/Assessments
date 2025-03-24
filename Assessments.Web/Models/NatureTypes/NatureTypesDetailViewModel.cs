using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure.Enums;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesDetailViewModel(Assessment Assessment)
{
    public FeedbackViewModel FeedbackViewModel { get; set; }
    
    public PageMenuViewModel PageMenuViewModel => new()
    {
        PageMenuContentId = NatureTypesConstants.PageMenuContentId,
        PageMenuExpandButtonText = NatureTypesConstants.Title2025,
        AssessmentType = AssessmentType.NatureTypes2025
    };

    public List<Region> Regions { get; set; } = [];

    public CitationForAssessmentViewModel CitationForAssessmentViewModel { get; set; }
}