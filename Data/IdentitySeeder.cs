using Microsoft.AspNetCore.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
        {
            await roleManager.CreateAsync(
                new IdentityRole(AppRoles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(AppRoles.Customer))
        {
            await roleManager.CreateAsync(
                new IdentityRole(AppRoles.Customer));
        }
    }
}