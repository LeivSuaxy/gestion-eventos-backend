using event_horizon_backend.Core.Cache.Interfaces;
using event_horizon_backend.Core.Mail.Services;
using event_horizon_backend.Core.Utils;
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
    private readonly ICacheService _cacheService;
    private const string PendingRegistrationKey = "pending_registrations";
    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        TokenService tokenService,
        AuthMailService authMailService,
        ICacheService cacheService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _authMailService = authMailService;
        _cacheService = cacheService;
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

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = _tokenService.GenerateToken(user, roles);

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
        
        string token = CodeGeneration.New();
        await _authMailService.SendVerificationEmailAsync(user.Email, user.UserName, token);

        CacheRegisterDto cacheRegisterDto = new CacheRegisterDto
        {
            DataUser = user,
            Token = token,
            Password = model.Password
        };

        await _cacheService.SetAsync(user.Email, cacheRegisterDto, TimeSpan.FromMinutes(3));
        
        return Ok(new { message = "Email sent successfully" });
    }
    
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromQuery] string email, [FromQuery] string token)
    {
        var userData = await _cacheService.GetAsync<CacheRegisterDto>(email);
        
        if (userData == null)
            return BadRequest(new { message = "Expired Token" });

        if (userData.Token != token)
            return BadRequest(new { message = "Invalid Token" });
        
        User user = userData.DataUser;
        
        var result = await _userManager.CreateAsync(user, userData.Password);

        if (!result.Succeeded)
        {
            Console.WriteLine(result.Errors);
            return BadRequest(new { message = "User already exists" });
        }

        await _userManager.AddToRoleAsync(user, "User");
        await _cacheService.RemoveAsync(email);
        return Ok(new { message = "User verified successfully" });
    }
}