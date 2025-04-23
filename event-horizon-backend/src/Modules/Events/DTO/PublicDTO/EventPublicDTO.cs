namespace event_horizon_backend.Modules.Events.DTO.PublicDTO;

public class EventPublicDTO
{
    
}

public class EventPublicCreateDTO
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool RequireAcceptance { get; set; } = false;
    public int LimitParticipants { get; set; } = 30;
    public bool IsPublished { get; set; }
}