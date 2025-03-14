using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure.Enums;
using RodlisteNaturtyper.Data.Models;
using X.PagedList;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModel(IPagedList<Assessment> Assessments) : NatureTypesListViewModelParameters
{
    public List<Committee> Committees { get; set; }

    public List<Region> Regions { get; set; }

    public ListViewViewModel ListViewViewModel { get; set; }

    public PageMenuViewModel PageMenuViewModel => new()
    {
        PageMenuContentId = NatureTypesConstants.PageMenuContentId,
        PageMenuExpandButtonText = NatureTypesConstants.Title2025,
        AssessmentType = AssessmentType.NatureTypes2025
    };
    
    public PageHeaderViewModel PageHeaderViewModel => new()
    {
        HeaderText = NatureTypesConstants.Title2025,
        // TODO: ikke vis publisert dato under innsynet for rødlista for naturtyper 2025
        HeaderByline = NatureTypesConstants.HeaderByline
    };
    
    public IntroductionViewModel IntroductionViewModel => new()
    {
        Introduction = NatureTypesConstants.Introduction
    };

    public CitationForListViewModel CitationForListViewModel => new()
    {
        CitationString = NatureTypesConstants.Citation
    };
}