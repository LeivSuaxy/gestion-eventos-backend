namespace event_horizon_backend.Modules.Category.DTO.AdminDTO;

public class CategoryAdminDTO
{
    public class CategoryAdminCreateDTO
    {
        public required string Name { get; set; } = null!;
    }
}