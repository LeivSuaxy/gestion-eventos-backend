using event_horizon_backend.Core.Mail.Services;
using event_horizon_backend.Modules.Authentication.DTO;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Authentication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly TokenService _tokenService;
    private readonly AuthMailService _authMailService;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        TokenService tokenService,
        AuthMailService authMailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _authMailService = authMailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid username or password" });

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return Ok(new
        {
            token,
            roles
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        User user = new User
        {
            UserName = model.Username,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Active = true
        };
        
        //await _userManager.AddToRoleAsync(user, "User");
        await _authMailService.SendVerificationEmailAsync(user.Email, user.UserName);
        return Ok(new { message = "Email sent successfully" });
    }
    
    

    [HttpPost("verify")]
    public async Task<IActionResult> Verify()
    {
        return Ok(new { message = "User verified successfully" });
    }

}