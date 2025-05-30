using event_horizon_backend.Core.Cache.Interfaces;
using event_horizon_backend.Core.Mail.Services;
using event_horizon_backend.Modules.Authentication.DTO;
using event_horizon_backend.Modules.Authentication.Services;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Authorization;
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
    private readonly AuthService _service;
    private const string PendingRegistrationKey = "pending_registrations";

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        TokenService tokenService,
        AuthMailService authMailService,
        ICacheService cacheService,
        AuthService service)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _authMailService = authMailService;
        _cacheService = cacheService;
        _service = service;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto model)
    {
        User? user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or not find user" });
        
        if (user.Active == false)
            return Unauthorized(new { message = "User is not active" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password" });

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = _tokenService.GenerateToken(user, roles);

        return Ok(new
        {
            user = new
            {
                user.Id,
                user.UserName,
                user.Balance,
                user.Email
            },
            token,
            roles
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var result = await _service.Register(model);
        Console.WriteLine(result.Result);
        if (result.Result is BadRequestResult badRequest)
            return BadRequest(badRequest);
        
        if (result.Result is OkResult)
            return Ok(new { message = "Email sent successfully" });
        
        return StatusCode(500, new { message = "An error occurred while processing your request" });
    }

    [HttpPost("verify")]
    [AllowAnonymous]
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
            var errorMessages = result.Errors.Select(e => e.Description).ToList();
            Console.WriteLine(string.Join(", ", errorMessages));
            return BadRequest(new { message = "Registration failed", errors = errorMessages });
        }

        if (!user.Active)
        {
            await _userManager.AddToRoleAsync(user, "Organizer");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "User");
        }
        
        await _cacheService.RemoveAsync(email);
        return Ok(new { message = "User verified successfully" });
    }
}