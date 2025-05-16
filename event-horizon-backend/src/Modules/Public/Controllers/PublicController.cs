using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Modules.Events.Models;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Public.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PublicController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    // Events views
    /*[HttpGet("event/")]
    public async Task<ActionResult<PagedResponse<EventModel>>> GetEvents()
    {
        throw NotImplementedException("Not implemented yet");
    }*/

    [HttpGet("event/{id}")]
    public async Task<ActionResult<EventModel>> GetEventInfo(Guid id)
    {
        
        var eventModel = await _context.Events.FindAsync(id);
        return eventModel == null ? NotFound() : _mapper.Map<EventModel>(eventModel);
    }
}