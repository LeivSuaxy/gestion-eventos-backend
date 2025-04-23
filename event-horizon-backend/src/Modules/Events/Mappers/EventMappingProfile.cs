using AutoMapper;
using event_horizon_backend.Modules.Events.DTO.PublicDTO;
using event_horizon_backend.Modules.Events.Models;

namespace event_horizon_backend.Modules.Events.Mappers;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<EventPublicCreateDTO, EventModel>();
    }
}