using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using event_horizon_backend.Common.Models;

namespace event_horizon_backend.Modules.Events.Models;

public class EventModel : BaseModel
{
    [Required]
    [Column(TypeName = "varchar(255)")]
    public required string Title { get; set; }
    
    [Required]
    [Column(TypeName = "text")]
    public required string Description { get; set; }
    
    [Column(TypeName = "varchar")]
    public string? ImageUrl { get; set; }
    
    [Required]
    [Column(TypeName = "date")]
    public required DateTime Date { get; set; }
    
    [Column(TypeName = "boolean")]
    public bool IsPublished { get; set; } = false;

    [Column(TypeName = "boolean")]
    public bool RequireAcceptance { get; set; } = false;

    [Column(TypeName = "integer")]
    public int LimitParticipants { get; set; } = 30;
    
    // Foreign Keys
    /*[ForeignKey("OrganizerId")]
    public required UserModel Organizer { get; set; }*/

}