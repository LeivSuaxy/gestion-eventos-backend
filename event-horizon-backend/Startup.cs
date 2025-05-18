using System.Reflection;
using System.Security.Claims;
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
using System.Text.Json;
using DotNetEnv;
using event_horizon_backend.Modules.Events.Services;
using event_horizon_backend.Modules.Organizer.Services;
using event_horizon_backend.Modules.Public.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace event_horizon_backend;

public class EventHorizonBuilder
{
    private readonly WebApplicationBuilder _builder;

    private EventHorizonBuilder(WebApplicationBuilder builder)
    {
        Env.Load();
        _builder = builder;
    }

    public static WebApplicationBuilder Create(WebApplicationBuilder builder)
    {
        return new EventHorizonBuilder(builder)
            .LoadEnvs()
            .AddControllers()
            .AddServices()
            .AddCors()
            .AddContext()
            .AddMailServices()
            .AddAuth()
            .AddMappers()
            .AddOpenApi()
            .AddCacheService()
            .GetBuilder();
    }

    private EventHorizonBuilder LoadEnvs()
    {
        _builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{_builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        string connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                  $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                  $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                  $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                  $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        // Connection Strings
        _builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
        _builder.Configuration["ConnectionStrings:RedisConnection"] =
            Environment.GetEnvironmentVariable("REDIS_CONNECTION");

        // MailSettings
        _builder.Configuration["MailSettings:Host"] = Environment.GetEnvironmentVariable("MAIL_HOST");
        _builder.Configuration["MailSettings:Port"] = Environment.GetEnvironmentVariable("MAIL_PORT");
        _builder.Configuration["MailSettings:UserName"] = Environment.GetEnvironmentVariable("MAIL_USER");
        _builder.Configuration["MailSettings:Password"] = Environment.GetEnvironmentVariable("MAIL_PASSWORD");
        _builder.Configuration["MailSettings:From"] = Environment.GetEnvironmentVariable("MAIL_USER");

        // Jwt
        _builder.Configuration["JWT:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET");
        _builder.Configuration["JWT:TokenValidityInMinutes"] =
            Environment.GetEnvironmentVariable("JWT_VALIDITY_MINUTES");

        return this;
    }

    private EventHorizonBuilder AddServices()
    {
        Type[] objects =
        {
            typeof(EventService),
            typeof(PublicService),
            typeof(OrganizerService),
            typeof(UserActionsService),
            typeof(AssistanceService)
        };

        foreach (Type service in objects)
        {
            _builder.Services.AddScoped(service);
        }

        return this;
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

    private EventHorizonBuilder AddCors()
    {
        _builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return this;
    }

    private EventHorizonBuilder AddLocalUse()
    {
        _builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5000);
        });

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
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _builder.Configuration["JWT:ValidIssuer"],
                ValidAudience = _builder.Configuration["JWT:ValidAudience"],
                RoleClaimType = ClaimTypes.Role,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _builder.Configuration["JWT:Secret"] ??
                    throw new InvalidOperationException("Secret must be not empty")))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    // Handle all challenges, whether token is invalid or missing
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new { message = "Unauthorized access" });
                    return context.Response.WriteAsync(result);
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated successfully");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    // This event is called when the token is missing entirely
                    return Task.CompletedTask;
                }
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
        
        _builder.Services.AddAuthorization(options =>
        {
            // This makes all endpoints require authentication by default
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            
            // You can add named policies for specific roles if needed
            // options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        });
        _builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Unauthorized" }));
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Forbidden" }));
            };
            options.Events.OnRedirectToLogout = context =>
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Logged out" }));
            };
            // Prevent other redirects like two-factor auth
            options.Events.OnRedirectToReturnUrl = context =>
            {
                context.Response.StatusCode = 200;
                return Task.CompletedTask;
            };
        });
        
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
        // We want Swagger in non-production environments
        bool useSwagger = true;//!_builder.Configuration.GetValue<bool>("Global:Production", false);

        if (useSwagger)
        {
            _builder.Services.AddEndpointsApiExplorer();
            _builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "E-Event Horizon API", 
                    Version = "v1",
                    Description = "API for Event Management System"
                });

                // Define JWT Bearer auth schema
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = 
                        "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                        "Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
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
        bool useRedis = false; //_builder.Configuration.GetValue<bool>("Redis:Enabled");

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