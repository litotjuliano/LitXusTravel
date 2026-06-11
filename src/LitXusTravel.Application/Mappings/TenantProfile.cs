using AutoMapper;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Mappings;

public class TenantProfile : Profile
{
    public TenantProfile()
    {
        CreateMap<Tenant, TenantResponse>()
            .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.ContactEmail.Value));

        CreateMap<Tenant, TenantListResponse>()
            .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.ContactEmail.Value));
    }
}
