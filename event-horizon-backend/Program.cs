using event_horizon_backend;
using event_horizon_backend.Modules.Authentication.Services;
using System.Text.Json;
using event_horizon_backend.Core.Context;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = EventHorizonBuilder.Create(WebApplication.CreateBuilder());
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

WebApplication app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

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
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Event Horizon API v1"));
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowSpecificOrigins");

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