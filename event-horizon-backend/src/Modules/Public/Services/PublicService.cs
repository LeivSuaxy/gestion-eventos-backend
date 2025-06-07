using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Events.Services;
using event_horizon_backend.Modules.Public.DTO;

namespace event_horizon_backend.Modules.Public.Services;

public class PublicService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly EventService _eventService;
    
    public PublicService(AppDbContext context, IMapper mapper, EventService eventService)
    {
        _context = context;
        _mapper = mapper;
        _eventService = eventService;
    }

    public async Task<object> GetMainView(PaginationParameters parameters)
    {
        DateTime currentDate = DateTime.UtcNow;
        var featuredEvents = _eventService.GetFeaturedEvents(parameters.PageSize, currentDate);
        PagedResponse<EventResponseDTO> events = await _eventService.GetPaginated(parameters);
        
        return new
        {
            status = "success",
            events,
            featuredEvents
        };
    }
    
    public async Task<object> GetEventsView(PaginationParameters parameters)
    {
        DateTime currentDate = DateTime.UtcNow;
        var featuredEvents = _eventService.GetFeaturedEvents(2, currentDate);
        PagedResponse<EventResponseDTO> events = await _eventService.GetPaginated(parameters);
        
        return new
        {
            status = "success",
            events,
            featuredEvents
        };
    }
    
}