using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Modules.Events.DTO.PublicDTO;
using event_horizon_backend.Modules.Events.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace event_horizon_backend.Modules.Events.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EventController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventModel>>> GetEvents()
    {
        return await _context.Events.ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<EventModel>> GetEvent(Guid id)
    {
        var eventModel = await _context.Events.FindAsync(id);

        if (eventModel == null)
        {
            return NotFound();
        }

        return eventModel;
    }

    // POST: api/Event
    [HttpPost]
    public async Task<ActionResult<EventModel>> CreateEvent(EventPublicCreateDTO eventPublicCreate)
    {
        EventModel eventModel = _mapper.Map<EventModel>(eventPublicCreate);
        
        _context.Events.Add(eventModel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = eventModel.Id }, eventModel);
    }

    // PUT: api/Event/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(Guid id, EventModel eventModel)
    {
        if (!id.Equals(eventModel.Id.ToString()))
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
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
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