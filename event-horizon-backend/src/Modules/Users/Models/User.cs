 
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace event_horizon_backend.Modules.Users.Models;

public class User : IdentityUser<Guid>
{
    [Column(TypeName = "time zone")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "time zone")]
    public DateTime UpdatedAt { get; set; }
    
    [Column(TypeName = "time zone")]
    public DateTime? DeletedAt { get; set; }
    
    public bool Active { get; set; }

    protected User()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Active = true;
    }

    public virtual void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        Active = false;
    }

    public virtual void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}