using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Modules.Events.DTO.PublicDTO;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Organizer.Services;

public class OrganizerService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly EventService _eventService;
    
    public OrganizerService(AppDbContext context, IMapper mapper, EventService eventService)
    {
        _context = context;
        _mapper = mapper;
        _eventService = eventService;
    }

    public async Task<ActionResult<EventModel>> Create(EventPublicCreateDTO eventPublicCreate)
    {
        return await _eventService.Create(eventPublicCreate);
    }
}