namespace event_horizon_backend.Modules.Public.DTO;

public class EventResponseDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Address { get; set; } = null!;
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
    public bool RequireAcceptance { get; set; }
    public int LimitParticipants { get; set; }
    public decimal Price { get; set; }
    
    // Simplified Category info
    public CategoryInfo Category { get; set; } = null!;
    
    // Simplified Organizer info
    public OrganizerInfo Organizer { get; set; } = null!;
    
    // Other properties from EventModel
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool Active { get; set; }
}

public class CategoryInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class OrganizerInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}