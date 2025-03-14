using Assessments.Web.Infrastructure.Enums;

namespace Assessments.Web.Models;

public class PageMenuViewModel
{
    public AssessmentType AssessmentType { get; set; }

    public ListOrAssessmentView ListOrAssessmentView { get; set; }

    public int PageMenuContentId { get; set; }

    public int PageMenuSubContentId { get; set; }

    public string PageMenuExpandButtonText { get; set; }

    public string PageMenuHeaderText { get; set; }

    public TableOfContentsViewModel TableOfContentsViewModel { get; set; }
}