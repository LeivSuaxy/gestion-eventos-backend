using AutoMapper;
using event_horizon_backend.Modules.Category.Models;
using static event_horizon_backend.Modules.Category.DTO.AdminDTO.CategoryAdminDTO;

namespace event_horizon_backend.Modules.Category.Mappers;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CategoryAdminCreateDTO, CategoryModel>();
    }
    
}