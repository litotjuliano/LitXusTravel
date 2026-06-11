using AutoMapper;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Mappings;

public class InquiryProfile : Profile
{
    public InquiryProfile()
    {
        CreateMap<Inquiry, InquiryResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CrmActivity, CrmActivityResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}
