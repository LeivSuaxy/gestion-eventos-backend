namespace event_horizon_backend.Common.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Clase abstracta que representa un modelo base para garantizar auditoria en los modelos.
public class BaseModel
{
    [Key]
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; } 
    
    [Column(TypeName = "date")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "date")]
    public DateTime UpdatedAt { get; set; }
    
    [Column(TypeName = "date")]
    public DateTime? DeletedAt { get; set; }
    
    public bool Active { get; set; }

    protected BaseModel()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow.Date;
        UpdatedAt = DateTime.UtcNow.Date;
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