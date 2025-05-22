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
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Event Horizon API v1"));
}*/

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Event Horizon API v1"));

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();

// Place this BEFORE app.UseAuthorization()
app.Use(async (context, next) =>
{
    // Store the original response body stream
    var originalBodyStream = context.Response.Body;
    
    // Create a new memory stream
    using var responseBody = new MemoryStream();
    context.Response.Body = responseBody;
    
    // Let the request continue
    await next();
    
    // Check if it's a 401 response
    if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
    {
        // Reset the stream
        context.Response.Body = originalBodyStream;
        context.Response.Clear();
        
        // Set status code and content type
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        
        // Write the response
        var result = JsonSerializer.Serialize(new { message = "Unauthorized" });
        await context.Response.WriteAsync(result);
        return;
    }
    
    // Copy the modified response to the original stream
    responseBody.Seek(0, SeekOrigin.Begin);
    await responseBody.CopyToAsync(originalBodyStream);
});

app.UseAuthorization();

app.MapControllers();

app.Run();