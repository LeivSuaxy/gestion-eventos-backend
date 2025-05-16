using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using event_horizon_backend.Common.Models;

namespace event_horizon_backend.Modules.Category.Models;

public class CategoryModel : BaseModel
{
    [Required]
    [Column(TypeName = "varchar(255)")]
    public required string Name { get; set; }
}