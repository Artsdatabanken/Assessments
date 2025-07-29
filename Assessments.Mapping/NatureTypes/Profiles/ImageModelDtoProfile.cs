using System.Linq;
using Assessments.Shared.DTOs.Drupal;
using Assessments.Shared.Helpers;
using AutoMapper;

namespace Assessments.Mapping.NatureTypes.Profiles;

public class ImageModelDtoProfile : Profile
{
    public ImageModelDtoProfile()
    {
        CreateMap<ContentByLongCodeResponseDto, ImageModelDto>()
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Id.Replace("Nodes/", "https://artsdatabanken.no/Media/")))
            .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Id.Replace("Nodes/", "https://artsdatabanken.no/Pages/")))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Fields.FirstOrDefault(x => x.Name.Equals("metadata")).Fields.FirstOrDefault(x => x.Name.Equals("reference")).References))
            .ForMember(dest => dest.License, opt => opt.MapFrom(src => src.Fields.FirstOrDefault(x => x.Name.Equals("license")).References.FirstOrDefault(x => x.StartsWith("Nodes/T"))))
            .ForMember(dest => dest.License, opt => opt.NullSubstitute(string.Empty))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Fields.FirstOrDefault(x => x.Name.Equals("annotation")).Values.FirstOrDefault().StripHtml()))
            .ForMember(dest => dest.Text, opt => opt.NullSubstitute(string.Empty));
    }
}