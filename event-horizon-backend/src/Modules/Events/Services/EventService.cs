using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Services;
using event_horizon_backend.Modules.Events.Models;

namespace event_horizon_backend.Modules.Events.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EventService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public object GetFeaturedEvents(int quantity, DateTime currentDate)
    {
        // Get top 2 events with highest attendance
        return _context.Events
            .Where(e => e.Date > currentDate && e.IsPublished && e.DeletedAt == null && e.Active)
            .Select(e => new
            {
                Event = e,
                AttendanceCount = _context
                    .Set<AssistanceModel>()
                    .Count(a => a.Event.Id == e.Id && a.DeletedAt == null && a.Active)
            })
            .OrderByDescending(x => x.AttendanceCount)
            .Take(quantity)
            .Select(x => new
            {
                x.Event.Id,
                x.Event.Title,
                x.Event.Description,
                x.Event.ImageUrl,
                x.Event.Date,
                x.Event.Address,
                Category = x.Event.Category.Name,
                OrganizedBy = x.Event.Organizer.UserName,
                AttendeeCount = x.AttendanceCount
            })
            .ToList();
    }
}