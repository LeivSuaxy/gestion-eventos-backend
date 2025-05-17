using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using event_horizon_backend.Common.Models;
using event_horizon_backend.Modules.Category.Models;
using event_horizon_backend.Modules.Users.Models;

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
    [Column(TypeName = "varchar")]
    public required string Address { get; set; }
    
    [Required]
    [Column(TypeName = "date")]
    public required DateTime Date { get; set; }
    
    [Required]
    [Column(TypeName = "integer")]
    public required int Duration { get; set; } // Duration in minutes
    
    [Column(TypeName = "boolean")]
    public bool IsPublished { get; set; } = false;

    [Column(TypeName = "boolean")]
    public bool RequireAcceptance { get; set; } = false;

    [Column(TypeName = "integer")]
    public int LimitParticipants { get; set; } = 30;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } = 0;
    
    // Foreign Keys
    [ForeignKey("OrganizerId")]
    public required User Organizer { get; set; }
    
    [ForeignKey("CategoryId")]
    public required CategoryModel Category { get; set; }

    public bool IsFree() => Price == 0;
}