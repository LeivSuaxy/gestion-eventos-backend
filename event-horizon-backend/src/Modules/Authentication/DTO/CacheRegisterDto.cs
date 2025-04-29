using event_horizon_backend.Modules.Users.Models;

namespace event_horizon_backend.Modules.Authentication.DTO;

public class CacheRegisterDto
{
    public User DataUser { get; set; }
    public string Token { get; set; }
    
    public string Password { get; set; }
}