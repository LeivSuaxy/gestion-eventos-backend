using AutoMapper;
using event_horizon_backend.Common.Extensions;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Modules.Category.Models;
using event_horizon_backend.Modules.Events.DTO.PublicDTO;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Events.Services;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace event_horizon_backend.Modules.Events.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly EventService _service;

    public EventController(AppDbContext context, IMapper mapper, EventService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<EventModel>>> GetEvents(
        [FromQuery] PaginationParameters parameters)
    {
        IQueryable<EventModel> events = _context.Events.AsQueryable();

        PagedResponse<EventModel> pagedResult = await events.ToPagedListAsync(
            parameters.PageNumber,
            parameters.PageSize
        );

        return Ok(pagedResult);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<EventModel>> GetEvent(Guid id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        return eventModel == null ? NotFound() : eventModel;
    }

    // POST: api/Event
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<EventModel>> CreateEvent(EventPublicCreateDTO eventPublicCreate)
    {
        ActionResult<EventModel> result = await _service.Create(eventPublicCreate);

        if (result.Result is BadRequestObjectResult badRequest) return badRequest;
        
        if (result.Result is OkObjectResult okResult && okResult.Value is EventModel createdEvent)
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        
        return result;
    }

    // PUT: api/Event/5
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(Guid id, EventModel eventModel)
    {
        if (!id.Equals(eventModel.Id))
        {
            return BadRequest();
        }

        _context.Entry(eventModel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Event/5
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var eventModel = await _context.Events.FindAsync(id);
        if (eventModel == null)
        {
            return NotFound();
        }

        // Soft delete since your model inherits from BaseModel
        eventModel.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EventExists(Guid id)
    {
        return _context.Events.Any(e => e.Id == id);
    }
}