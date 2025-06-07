using Microsoft.AspNetCore.Identity;

namespace event_horizon_backend.Modules.Authentication.Services;

public class RolesService
{
    // Mecanismo de creacion de roles
    public static async Task InitializeRoles(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = { "Admin", "User", "Organizer" };

        foreach (string rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(rol));
            }
        }
    }
}