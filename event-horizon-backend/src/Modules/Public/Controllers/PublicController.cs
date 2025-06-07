using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Modules.Category.Models;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Public.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("search/")]
    [AllowAnonymous]
    public async Task<object> Search([FromQuery] string query)
    {
        return new { OPP = "OPP" };
    }

    [HttpGet("main/")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMain(
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _service.GetMainView(parameters);
        return Ok(result);
    }
    
    // Events views
    [HttpGet("event/")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvents(
        [FromQuery] PaginationParameters parameters)
    {
        var result = await _service.GetEventsView(parameters);
        
        return Ok(result);
    }

    [HttpGet("event/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventModel>> GetEventInfo(Guid id)
    {
        
        var eventModel = await _context.Events.FindAsync(id);
        return eventModel == null ? NotFound() : _mapper.Map<EventModel>(eventModel);
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryModel>> GetCategories()
    {
        List<CategoryModel>? categories = await _context.Categories.ToListAsync();
        return categories == null ? NotFound() : Ok(categories);
    }
}