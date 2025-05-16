using event_horizon_backend.Core.Models;

namespace event_horizon_backend.Core.Services;

public interface IEventService
{
    public object GetFeaturedEvents(int quantity, DateTime currentDate);
}

public interface IPublicService
{
    public object GetEventsView(PaginationParameters parameters);
}