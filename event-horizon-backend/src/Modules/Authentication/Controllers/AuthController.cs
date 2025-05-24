using event_horizon_backend.Core.Cache.Interfaces;
using event_horizon_backend.Core.Mail.Services;
using event_horizon_backend.Core.Utils;
using event_horizon_backend.Modules.Authentication.DTO;
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
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or not find user" });

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
        var passwordValidators = _userManager.PasswordValidators;
        foreach (var validator in passwordValidators)
        {
            var result = await validator.ValidateAsync(_userManager, null, model.Password);
            if (!result.Succeeded)
            {
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { message = "Invalid password", errors = errorMessages });
            }
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email is already registered" });
        }

        User user = new User
        {
            UserName = model.Username,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Active = true,
            Balance = 5
        };

        string token = CodeGeneration.New();
        try
        {
            await _authMailService.SendVerificationEmailAsync(user.Email, user.UserName, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return StatusCode(500, new { message = "Failed to send verification email" });
        }


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

        await _userManager.AddToRoleAsync(user, "User");
        await _cacheService.RemoveAsync(email);
        return Ok(new { message = "User verified successfully" });
    }
}