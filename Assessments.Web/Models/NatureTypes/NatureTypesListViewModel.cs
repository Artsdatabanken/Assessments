using Assessments.Shared.Constants;
using RodlisteNaturtyper.Data.Models;
using X.PagedList;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModel(IPagedList<Assessment> Assessments) : NatureTypesListParameters
{
    public List<Region> Regions { get; init; }

    public List<NinCodeTopic> NinCodeTopics { get; init; }

    public List<CodeItem> CodeItems { get; init; }

    public ListViewViewModel ListViewViewModel { get; init; }
    
    //public PageHeaderViewModel PageHeaderViewModel => new()
    //{
    //    HeaderText = NatureTypesConstants.Title2025,
    //    HeaderByline = $"Publisert: {NatureTypesConstants.PublishedDate:d. MMMM yyy}"
    //};

    public CitationForListViewModel CitationForListViewModel => new(citationString: $"Artsdatabanken ({NatureTypesConstants.PublishedDate:yyyy, d. MMMM}). {NatureTypesConstants.Title2025}.");

    public NatureTypesStatisticsViewModel NatureTypesStatisticsViewModel { get; set; }
}