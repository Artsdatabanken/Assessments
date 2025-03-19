using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure.Enums;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesDetailViewModel(Assessment Assessment)
{
    public FeedbackViewModel FeedbackViewModel { get; set; }
    
    public string Citation { get; set; }

    public PageMenuViewModel PageMenuViewModel => new()
    {
        PageMenuContentId = NatureTypesConstants.PageMenuContentId,
        PageMenuExpandButtonText = NatureTypesConstants.Title2025,
        AssessmentType = AssessmentType.NatureTypes2025
    };
}