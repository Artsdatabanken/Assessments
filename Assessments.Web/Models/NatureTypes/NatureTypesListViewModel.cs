using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure.Enums;
using RodlisteNaturtyper.Data.Models;
using X.PagedList;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModel(IPagedList<Assessment> Assessments) : NatureTypesListParameters
{
    public List<Region> Regions { get; init; }

    public List<NinCodeTopic> NinCodeTopics { get; init; }

    public List<CodeItem> CodeItems { get; init; }

    public ListViewViewModel ListViewViewModel { get; init; }

    public PageMenuViewModel PageMenuViewModel => new()
    {
        PageMenuContentId = NatureTypesConstants.PageMenuContentId,
        PageMenuExpandButtonText = NatureTypesConstants.Title2025,
        AssessmentType = AssessmentType.NatureTypes2025
    };

    public PageHeaderViewModel PageHeaderViewModel => new()
    {
        HeaderText = NatureTypesConstants.Title2025,
        HeaderByline = $"Publisert: {NatureTypesConstants.PublishedDate:d. MMMM yyy}"
    };

    public IntroductionViewModel IntroductionViewModel => new(introduction:
        NatureTypesConstants.Introduction
    );

    public CitationForListViewModel CitationForListViewModel => new(citationString: $"Artsdatabanken ({NatureTypesConstants.PublishedDate:yyyy, d. MMMM}). {NatureTypesConstants.Title2025}.");

    public NatureTypesStatisticsViewModel NatureTypesStatisticsViewModel { get; set; }
}