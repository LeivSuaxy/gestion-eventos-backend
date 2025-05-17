 
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace event_horizon_backend.Modules.Users.Models;

public class User : IdentityUser<Guid>
{
    [Column(TypeName = "date")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "date")]
    public DateTime UpdatedAt { get; set; }
    
    [Column(TypeName = "date")]
    public DateTime? DeletedAt { get; set; }
    
    [Column(TypeName = "boolean")]
    public bool Active { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;
    
    public User()
    {
        CreatedAt = DateTime.UtcNow.Date;
        UpdatedAt = DateTime.UtcNow.Date;
        Balance = 0;
        Active = true;
    }

    public virtual void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow.Date;
        Active = false;
    }

    public virtual void Update()
    {
        UpdatedAt = DateTime.UtcNow.Date;
    }
}