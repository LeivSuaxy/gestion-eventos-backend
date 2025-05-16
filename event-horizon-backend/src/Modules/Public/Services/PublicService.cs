using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Core.Services;
using event_horizon_backend.Modules.Events.Models;

namespace event_horizon_backend.Modules.Public.Services;

public class PublicService : IPublicService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEventService _eventService;
    
    public PublicService(AppDbContext context, IMapper mapper, IEventService eventService)
    {
        _context = context;
        _mapper = mapper;
        _eventService = eventService;
    }
    
    public object GetEventsView(PaginationParameters parameters)
    {
        var currentDate = DateTime.UtcNow;
        var featuredEvents = _eventService.GetFeaturedEvents(2, currentDate);
        
        return new
        {
            status = "success",
            featuredEvents
        };
    }
    
}