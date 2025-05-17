using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Events.Services;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Public.Services;

public class UserActionsService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly AssistanceService _assistanceService;

    public UserActionsService(AppDbContext context, IMapper mapper, AssistanceService assistanceService)
    {
        _context = context;
        _mapper = mapper;
        _assistanceService = assistanceService;
    }

    public async Task<ActionResult> InscribeEvent(Guid eventId, Guid userId)
    {
        EventModel? eventModel = await _context.Events.FindAsync(eventId);
        
        if (eventModel == null)
            return new BadRequestObjectResult($"Model with ID {eventId} does not exist.");

        User? user = await _context.Users.FindAsync(userId);

        if (user == null)
            return new BadRequestObjectResult($"User with ID {userId} does not exist.");

        if (_assistanceService.IsUserAssisted(eventModel.Id, user.Id))
            return new BadRequestObjectResult("You are already registered");

        int count = _assistanceService.CountAssistance(eventModel.Id);

        if (count >= eventModel.LimitParticipants)
            return new BadRequestObjectResult("The event is already fullish");

        if (!eventModel.IsFree())
        {
            if (user.Balance < eventModel.Price)
                return new BadRequestObjectResult("You don't have enough balance to be inscribed");
            
            user.Balance -= eventModel.Price;
        }

        if (!await _assistanceService.CreateAssistance(eventModel, user))
            return new ConflictObjectResult("Error creating assistance");

        await _context.SaveChangesAsync();
        return new OkObjectResult("Successfully inscribed");
    }
}