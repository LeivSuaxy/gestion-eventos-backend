using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Public.Services;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Public.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly PublicService _service;

    public PublicController(AppDbContext context, IMapper mapper, PublicService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }
    // Events views
    [HttpGet("event/")]
    public async Task<IActionResult> GetEvents(
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _service.GetEventsView(parameters);
        
        return Ok(result);
    }

    [HttpGet("event/{id}")]
    public async Task<ActionResult<EventModel>> GetEventInfo(Guid id)
    {
        
        var eventModel = await _context.Events.FindAsync(id);
        return eventModel == null ? NotFound() : _mapper.Map<EventModel>(eventModel);
    }
}