using event_horizon_backend;
using event_horizon_backend.Modules.Authentication.Services;

WebApplicationBuilder builder = EventHorizonBuilder.Create(WebApplication.CreateBuilder());

WebApplication app = builder.Build();

await RolesService.InitializeRoles(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();