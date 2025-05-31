using event_horizon_backend.Core.Cache.Interfaces;
using event_horizon_backend.Core.Context;
using event_horizon_backend.Core.Mail.Services;
using event_horizon_backend.Core.Utils;
using event_horizon_backend.Modules.Authentication.DTO;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace event_horizon_backend.Modules.Authentication.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ICacheService _cacheService;
    private readonly AuthMailService _authMailService;
    
    public AuthService(
        AppDbContext context, 
        UserManager<User> userManager,
        ICacheService cacheService,
        AuthMailService authMailService)
    {
        _userManager = userManager;
        _context = context;
        _cacheService = cacheService;
        _authMailService = authMailService;
    }
    
    public async Task<ActionResult<User>> Register(RegisterDto model)
    {
        var passwordValidators = _userManager.PasswordValidators;

        foreach (var validator in passwordValidators)
        {
            var result = await validator.ValidateAsync(_userManager, null, model.Password);
            if (!result.Succeeded)
            {
                List<string> errorMessages = result.Errors.Select(e => e.Description).ToList();
                return new BadRequestObjectResult(new { message = "Invalid Password", errors = errorMessages });
            }
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return new BadRequestObjectResult(new { message = "Email already exists" });
        }

        User user;
        
        if (model.BeOrganizer)
        {
            user = CreateOrganizer(model);
        }
        else
        {
            user = CreateUser(model);
        }

        string token = CodeGeneration.New();
        try
        {
            await _authMailService.SendVerificationEmailAsync(user.Email, user.UserName, token);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending email: {e.Message}");
            return new StatusCodeResult(500);
        }
        
        CacheRegisterDto cacheRegisterDto = new CacheRegisterDto
        {
            DataUser = user,
            Token = token,
            Password = model.Password
        };

        await _cacheService.SetAsync(user.Email, cacheRegisterDto, TimeSpan.FromMinutes(3));

        return new OkResult();
    }
    
    private User CreateUser(RegisterDto model)
    {
        return new User
        {
            UserName = model.Username,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Active = true,
            Balance = 5
        };
    }

    private User CreateOrganizer(RegisterDto model)
    {
        return new User
        {
            UserName = model.Username,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Active = true,
            Balance = 10
        };
    }
    
}