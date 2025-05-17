using System.Security.Claims;
using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Modules.Events.DTO.PublicDTO;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Organizer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace event_horizon_backend.Modules.Organizer.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrganizerController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly OrganizerService _service;

    public OrganizerController(AppDbContext context, IMapper mapper, OrganizerService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Organizer,Admin")]
    [HttpGet("event/{id}")]
    public async Task<ActionResult<EventModel>> GetEvent(Guid id)
    {
        EventModel? eventModel = await _context.Events
            .Include(e => e.Organizer)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (eventModel == null) 
            return NotFound();
        
        if (User.IsInRole("Admin"))
            return eventModel;
        
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            return Forbid();
        
        if (eventModel.Organizer.Id != userGuid)
            return Forbid("You don't have permission to access this event");
    
        return eventModel;
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Organizer,Admin")]
    [HttpPost("event/")]
    public async Task<ActionResult<EventModel>> CreateEvent(EventPublicCreateDTO eventPublicCreate)
    {
        ActionResult<EventModel> result = await _service.Create(eventPublicCreate);

        if (result.Result is BadRequestObjectResult badRequest) return badRequest;
        
        if (result.Result is OkObjectResult okResult && okResult.Value is EventModel createdEvent)
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        
        return result;
    }
    
}