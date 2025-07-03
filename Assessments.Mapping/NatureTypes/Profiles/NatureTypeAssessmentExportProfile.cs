using System.Collections.Generic;
using System.Linq;
using System.Net;
using Assessments.Mapping.NatureTypes.Model;
using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.Extensions;
using Assessments.Shared.Helpers;
using AutoMapper;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Mapping.NatureTypes.Profiles;

public class NatureTypeAssessmentExportProfile : Profile
{
    public NatureTypeAssessmentExportProfile()
    {
        List<CommitteeUser> committeeUsers = [];

        CreateMap<Assessment, NatureTypeAssessmentExport>()
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region.GetDescription()))
            .ForMember(dest => dest.DescriptionHtml, opt => opt.MapFrom(src => HtmlInput(src.DescriptionHtml)))
            .ForMember(dest => dest.CriteriaDocumentationHtml, opt => opt.MapFrom(src => HtmlInput(src.CriteriaDocumentationHtml).Trim()))
            .ForMember(dest => dest.ImpactsCommentHtml, opt => opt.MapFrom(src => HtmlInput(src.ImpactsCommentHtml).Trim()))
            .ForMember(dest => dest.AreaInformationCommentHtml, opt => opt.MapFrom(src => HtmlInput(src.AreaInformation.CommentHtml).Trim()))
            .ForMember(dest => dest.RegionsList, opt => opt.MapFrom(src => string.Join(";", src.Regions.Select(x => x.Name))))
            .ForMember(dest => dest.ReferencesList, opt => opt.MapFrom(src => string.Join("|", src.References.Select(x => x.ReferencePresentation))))
            .ForMember(dest => dest.CodeitemsList, opt => opt.MapFrom(src => GetCodeItemList(src.CodeItems.Select(x => new AssessmentCodeItem
                {
                    CodeItemId = x.CodeItemId,
                    CodeItemDescription = x.CodeItemDescription,
                    CodeItemParamLevel = new CodeItemParamLevel
                    {
                        Description = x.CodeItemParamLevel.Description,
                        CodeItemParamTypeId = x.CodeItemParamLevel.CodeItemParamTypeId
                    }
                }
            ).ToList().GetCodeItemModels())))
            .ForMember(dest => dest.Citation, opt => opt.MapFrom(src => committeeUsers.GetCitation(src.Committee)));
    }

    private static string HtmlInput(string input) => WebUtility.HtmlDecode(input.StripHtml()).Trim();

    private static string GetCodeItemList(ICollection<CodeItemDto> codeItems)
    {
        if (codeItems.Count == 0)
            return string.Empty;

        var values = codeItems
            .Select(codeItem => $"{codeItem.CodeItemDescription}_{codeItem.TimeOfIncident}_{codeItem.InfluenceFactor}_{codeItem.Magnitude}");

        return string.Join(";", values);
    }
}