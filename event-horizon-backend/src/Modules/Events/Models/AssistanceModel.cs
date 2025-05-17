using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using event_horizon_backend.Common.Models;
using event_horizon_backend.Modules.Users.Models;

namespace event_horizon_backend.Modules.Events.Models;

public class AssistanceModel : BaseModel
{
    [Required]
    [ForeignKey("UserId")]
    public required User Participant { get; set; }
    
    [Required]
    [ForeignKey("EventId")]
    public required EventModel Event { get; set; }
}