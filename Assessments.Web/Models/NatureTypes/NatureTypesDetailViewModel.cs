using RodlisteNaturtyper.Data.Models;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesDetailViewModel(Assessment Assessment)
{
    public FeedbackViewModel FeedbackViewModel { get; set; }
    
    public string Citation { get; set; }
}