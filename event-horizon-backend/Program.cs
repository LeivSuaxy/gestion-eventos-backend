using event_horizon_backend;
using event_horizon_backend.Modules.Authentication.Services;
using System.Text.Json;

WebApplicationBuilder builder = EventHorizonBuilder.Create(WebApplication.CreateBuilder());

WebApplication app = builder.Build();

await RolesService.InitializeRoles(app.Services);

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new { message = "An error occurred" });
        await context.Response.WriteAsync(result);
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    await next();
    
    if (context.Response.StatusCode == 401)
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new { message = "Unauthorized" });
        await context.Response.WriteAsync(result);
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();