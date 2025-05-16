using System.ComponentModel.DataAnnotations;

namespace event_horizon_backend.Modules.Events.DTO.PublicDTO;

public class EventPublicDTO
{
    
}

public class EventPublicCreateDTO
{
    [Required]
    public string Title { get; set; } = null!;
    
    [Required]
    public string Description { get; set; } = null!;
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public int Duration { get; set; }
    
    public bool RequireAcceptance { get; set; } = false;
    
    public int LimitParticipants { get; set; } = 30;
    
    [Required]
    public bool IsPublished { get; set; }
    
    [Required]
    public string Address { get; set; } = null!;
    
    [Required] 
    public Guid CategoryId { get; set; }
    
    [Required]
    public Guid OrganizerId { get; set; }
}