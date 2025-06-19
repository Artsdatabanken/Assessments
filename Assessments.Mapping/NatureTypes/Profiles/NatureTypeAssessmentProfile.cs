using System.Linq;
using System.Net;
using Assessments.Mapping.NatureTypes.Model;
using Assessments.Shared.Extensions;
using Assessments.Shared.Helpers;
using AutoMapper;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Mapping.NatureTypes.Profiles;

public class NatureTypeAssessmentProfile : Profile
{
    public NatureTypeAssessmentProfile()
    {
        CreateMap<Assessment, NatureTypeAssessmentExport>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region.GetDescription()))
            .ForMember(dest => dest.DescriptionHtml, opt => opt.MapFrom(src => HtmlInput(src.DescriptionHtml)))
            .ForMember(dest => dest.CriteriaDocumentationHtml, opt => opt.MapFrom(src => HtmlInput(src.CriteriaDocumentationHtml).Trim()))
            .ForMember(dest => dest.ImpactsCommentHtml, opt => opt.MapFrom(src => HtmlInput(src.ImpactsCommentHtml).Trim()))
            .ForMember(dest => dest.AreaInformationCommentHtml, opt => opt.MapFrom(src => HtmlInput(src.AreaInformation.CommentHtml).Trim()))

            .ForMember(dest => dest.Regions, opt => opt.MapFrom(src => string.Join(";", src.Regions.Select(x => x.Name))))

            // TODO: eksport må ha annet skilletegn
            .ForMember(dest => dest.References, opt => opt.MapFrom(src => string.Join(";", src.References.Select(x => $"{x.ReferencePresentation}"))))
            ;
    }

    private static string HtmlInput(string input) => WebUtility.HtmlDecode(input.StripHtml()).Trim();
}