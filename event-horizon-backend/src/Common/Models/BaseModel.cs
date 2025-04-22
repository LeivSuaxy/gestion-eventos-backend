namespace event_horizon_backend.Common.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BaseModel
{
    [Key]
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; } 
    
    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "timestamp")]
    public DateTime UpdatedAt { get; set; }
    
    [Column(TypeName = "timestamp")]
    public DateTime? DeletedAt { get; set; }
    
    public bool Active { get; set; }

    protected BaseModel()
    {
        Id = Guid.NewGuid();
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