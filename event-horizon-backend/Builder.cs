using System.Runtime.CompilerServices;
using event_horizon_backend.Core.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Identity;
namespace event_horizon_backend;

public class EventHorizonBuilder
{
    private static WebApplicationBuilder _builder;
    
    public EventHorizonBuilder()
    {
        
    }

    public WebApplicationBuilder Create(WebApplicationBuilder builder)
    {
        _builder = builder;
        AddControllers();
        AddContext();
        AddAuth();
        AddMappers();
        AddOpenApi();
        return _builder;
    }

    private void AddControllers()
    {
        _builder.Services.AddControllers();
    }

    private void AddContext()
    {
        _builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(_builder.Configuration.GetConnectionString("DefaultConnection")));
    }

    private void AddAuth()
    {
        _builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _builder.Configuration["JWT:ValidIssuer"],
                ValidAudience = _builder.Configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("Secret must be somethig")))
            };
        });
        
        _builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        _builder.Services.AddScoped<TokenService>();
    }

    private void AddMappers()
    {
        _builder.Services.AddAutoMapper(typeof(Program).Assembly);
    }

    private void AddOpenApi()
    {
        _builder.Services.AddEndpointsApiExplorer();
        _builder.Services.AddSwaggerGen();
    }
}