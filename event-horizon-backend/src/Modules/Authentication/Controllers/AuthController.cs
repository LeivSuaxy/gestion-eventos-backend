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
        // Ver si existe el email
        User? user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or not find user" });
        
        if (user.Active == false)
            return Unauthorized(new { message = "User is not active" });

        // Verificar si la contraseña es correcta
        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password" });

        // Se toman los roles del usuario
        IList<string> roles = await _userManager.GetRolesAsync(user);

        // Se genera el token JWT
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
        // Invoca al servicio para que registre
        var result = await _service.Register(model);
        Console.WriteLine(result.Result);

        // Verifica si el resultado es BadRequest, Ok o un error interno
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

        // Se usa condicional con el balance del usuario, si es 10 es organizador por defecto
        if (user.Balance == 10)
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