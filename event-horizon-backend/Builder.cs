using event_horizon_backend.Core.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using event_horizon_backend.Core.Cache.Interfaces;
using event_horizon_backend.Core.Cache.Providers;
using event_horizon_backend.Core.Mail;
using event_horizon_backend.Core.Mail.Services;
using event_horizon_backend.Modules.Users.Models;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;

namespace event_horizon_backend;

public class EventHorizonBuilder
{
    private readonly WebApplicationBuilder _builder;

    private EventHorizonBuilder(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public static WebApplicationBuilder Create(WebApplicationBuilder builder)
    {
        return new EventHorizonBuilder(builder)
            .AddControllers()
            .AddContext()
            .AddMailServices()
            .AddAuth()
            .AddMappers()
            .AddOpenApi()
            .AddCacheService()
            .GetBuilder();
    }

    private EventHorizonBuilder AddControllers()
    {
        _builder.Services.AddControllers();
        return this;
    }

    private EventHorizonBuilder AddContext()
    {
        _builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(_builder.Configuration.GetConnectionString("DefaultConnection")));

        return this;
    }

    private EventHorizonBuilder AddAuth()
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _builder.Configuration["JWT:Secret"] ??
                    throw new InvalidOperationException("Secret must be not empty")))
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

        return this;
    }

    private EventHorizonBuilder AddMappers()
    {
        _builder.Services.AddAutoMapper(typeof(Program).Assembly);

        return this;
    }

    private EventHorizonBuilder AddOpenApi()
    {
        bool useSwagger = _builder.Configuration.GetValue<bool>("Global:Production");

        if (!useSwagger)
        {
            _builder.Services.AddEndpointsApiExplorer();
            _builder.Services.AddSwaggerGen();
        }

        return this;
    }

    private EventHorizonBuilder AddMailServices()
    {
        _builder.Services.Configure<MailSettings>(_builder.Configuration.GetSection("MailSettings"));
        _builder.Services.AddScoped<AuthMailService>();

        return this;
    }
    
    private EventHorizonBuilder AddCacheService()
    {
        bool useRedis = _builder.Configuration.GetValue<bool>("Redis:Enabled");

        if (useRedis)
        {
            _builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(_builder.Configuration.GetConnectionString("RedisConnection") ??
                                              "localhost:6379"));

            _builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        }
        else
        {
            _builder.Services.AddMemoryCache();
            _builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
        }
        
        return this;
    }
    private WebApplicationBuilder GetBuilder() => _builder;
}