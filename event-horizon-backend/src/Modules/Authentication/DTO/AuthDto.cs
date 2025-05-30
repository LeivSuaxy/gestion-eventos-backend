namespace event_horizon_backend.Modules.Authentication.DTO;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    
    public bool BeOrganizer { get; set; }
}