using AutoMapper;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Application.Mappings;

public class PackageProfile : Profile
{
    public PackageProfile()
    {
        // Master package mappings
        CreateMap<Package, PackageResponse>()
            .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility.ToString()));

        CreateMap<Package, PackageListResponse>()
            .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility.ToString()))
            .ForMember(dest => dest.Tenants, opt => opt.MapFrom(_ => Array.Empty<string>()))
            .ForMember(dest => dest.syncedTenantsCount, opt => opt.MapFrom(_ => 0));

        // TenantPackage → ResolvedPackageResponse uses custom mapping due to complex null coalescing logic
        // Handlers perform this mapping manually to support conditional field resolution
    }
}
