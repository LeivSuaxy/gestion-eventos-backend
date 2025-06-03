using AutoMapper;
using event_horizon_backend.Common.Extensions;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Models;
using event_horizon_backend.Modules.Category.Models;
using event_horizon_backend.Modules.Events.DTO.PublicDTO;
using event_horizon_backend.Modules.Events.Models;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Events.Services;

public class EventService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public EventService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ActionResult<EventModel>> Create(EventPublicCreateDTO eventPublicCreate)
    {
        // Verifica si la categoría existe
        CategoryModel? category = await _context.Categories.FindAsync(eventPublicCreate.CategoryId);

        if (category == null)
            return new BadRequestObjectResult($"Category with ID {eventPublicCreate.CategoryId} does not exist.");

        // Busca si el usuario existe
        User? user = await _context.Users.FindAsync(eventPublicCreate.OrganizerId);

        if (user == null)
            return new BadRequestObjectResult($"User with ID {eventPublicCreate.OrganizerId} does not exists");

        // Mapea el modelo de evento
        EventModel eventModel = _mapper.Map<EventModel>(eventPublicCreate);

        // Asigna la categoría y el organizador al evento
        eventModel.Category = category;
        eventModel.Organizer = user;
        // Se gurda en la bd
        _context.Events.Add(eventModel);
        await _context.SaveChangesAsync();

        return new OkObjectResult(eventModel);
    }

    // Obtiene elementos paginados
    // Paginaicion significa que se obtienen los elementos en partes, por ejemplo, 10 elementos por página
    public async Task<PagedResponse<EventModel>> GetPaginated(PaginationParameters parameters)
    {
        IQueryable<EventModel> events = _context.Events.AsQueryable();

        PagedResponse<EventModel> pagedResult = await events.ToPagedListAsync(
            parameters.PageNumber,
            parameters.PageSize
        );

        return pagedResult;
    }

    // Simplemente una query compleja
    public object GetFeaturedEvents(int quantity, DateTime currentDate)
    {
        // Get top 2 events with highest attendance
        return _context.Events
            .Where(e => e.Date > currentDate && e.IsPublished && e.DeletedAt == null && e.Active)
            .Select(e => new
            {
                Event = e,
                AttendanceCount = _context
                    .Set<AssistanceModel>()
                    .Count(a => a.Event.Id == e.Id && a.DeletedAt == null && a.Active)
            })
            .OrderByDescending(x => x.AttendanceCount)
            .Take(quantity)
            .Select(x => new
            {
                x.Event.Id,
                x.Event.Title,
                x.Event.Description,
                x.Event.ImageUrl,
                x.Event.Date,
                x.Event.Address,
                Category = x.Event.Category.Name,
                OrganizedBy = x.Event.Organizer.UserName,
                AttendeeCount = x.AttendanceCount
            })
            .ToList();
    }
}