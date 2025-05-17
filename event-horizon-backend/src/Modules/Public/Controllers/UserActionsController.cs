using System.Security.Claims;
using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Modules.Public.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Public.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
[ApiController]
[Route("api/[controller]")]
public class UserActionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserActionsService _service;
    
    public UserActionsController(AppDbContext context, IMapper mapper, UserActionsService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<String>> InscribeEvent([FromBody] Guid eventId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return BadRequest("User ID not found in token");
            
        Guid userGuid = Guid.Parse(userId);
        
        ActionResult result = await _service.InscribeEvent(eventId, userGuid);
        
        if (result is BadRequestObjectResult badRequest)
            return BadRequest(badRequest.Value);

        if (result is ConflictObjectResult conflict)
            return Conflict(conflict.Value);

        if (result is OkObjectResult ok)
            return Ok(ok.Value);

        return BadRequest("Nothing has worked");
    }
    
}