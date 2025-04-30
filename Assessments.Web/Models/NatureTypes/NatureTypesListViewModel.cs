using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure.Enums;
using RodlisteNaturtyper.Data.Models;
using X.PagedList;

namespace Assessments.Web.Models.NatureTypes;

public record NatureTypesListViewModel(IPagedList<Assessment> Assessments) : NatureTypesListParameters
{
    public List<Region> Regions { get; init; }

    public List<NinCodeTopic> NinCodeTopics { get; init; }

    public ListViewViewModel ListViewViewModel { get; init; }

    public PageMenuViewModel PageMenuViewModel => new()
    {
        PageMenuContentId = NatureTypesConstants.PageMenuContentId,
        PageMenuExpandButtonText = NatureTypesConstants.Title2025,
        AssessmentType = AssessmentType.NatureTypes2025
    };
    
    public PageHeaderViewModel PageHeaderViewModel => new()
    {
        // TODO: endre før lansering av rødlista for naturtyper 2025
        HeaderText = "Innsyn i Norsk rødliste for naturtyper 2025", // NatureTypesConstants.Title2025 
        HeaderByline = string.Empty //NatureTypesConstants.HeaderByline
    };

    public IntroductionViewModel IntroductionViewModel => new(introduction:
        // TODO: endre før lansering av rødlista for naturtyper 2025
        //NatureTypesConstants.Introduction
        "Artsdatabanken ønsker informasjon som kan utfylle kunnskapsgrunnlaget for vurdering av naturtypene. Foreløpige resultater er åpne for innsyn fra 5. mai til 16. juni 2025. Send inn innspill ved å gå inn på en naturtype og gi tilbakemelding i skjemaet nederst på siden."
    );

    public CitationForListViewModel CitationForListViewModel => new(citationString: NatureTypesConstants.Citation);

    public NatureTypesStatisticsViewModel NatureTypesStatisticsViewModel { get; set; }
}