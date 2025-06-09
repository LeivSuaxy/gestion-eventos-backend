using AutoMapper;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace event_horizon_backend.Modules.Events.Services;

public class AssistanceService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AssistanceService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> CreateAssistance(Guid EventId, Guid userId)
    {
        return false;
    }

    public async Task<bool> CreateAssistance(EventModel eventModel, User user)
    {
        try
        {
            AssistanceModel assistance = new()
            {
                Event = eventModel,
                Participant = user,
                Active = true
            };

            _context.Assistance.Add(assistance);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public bool IsUserAssisted(Guid eventId, Guid userId)
    {
        return _context.Set<AssistanceModel>()
            .Any(a => a.Event.Id == eventId 
                      && a.Participant.Id == userId 
                      && a.Active 
                      && a.DeletedAt == null);
    }
    
    public int CountAssistance(Guid eventId)
    {
        return _context.Set<AssistanceModel>()
            .Count(a => a.Event.Id == eventId 
                        && a.Active 
                        && a.DeletedAt == null);
    }
    
    public async Task<IEnumerable<Guid>> GetAssistanceByUserId(Guid userId)
    {
        return await _context.Assistance
            .Where(a => a.Participant.Id == userId && a.Active && a.DeletedAt == null)
            .Select(a => a.Event.Id)
            .ToListAsync();
    }
}