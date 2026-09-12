using Microsoft.AspNetCore.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
        {
            var adminRoleResult =
                await roleManager.CreateAsync(
                    new IdentityRole(AppRoles.Admin));

            if (!adminRoleResult.Succeeded)
            {
                throw new Exception(
                    "Failed to create Admin role.");
            }
        }

        if (!await roleManager.RoleExistsAsync(AppRoles.Customer))
        {
            var customerRoleResult =
                await roleManager.CreateAsync(
                    new IdentityRole(AppRoles.Customer));

            if (!customerRoleResult.Succeeded)
            {
                throw new Exception(
                    "Failed to create Customer role.");
            }
        }

        var adminEmail =
            configuration["Admin:Email"];

        var adminPassword =
            configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var adminUser =
            await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            var createResult =
                await userManager.CreateAsync(
                    adminUser,
                    adminPassword);

            if (!createResult.Succeeded)
            {
                throw new Exception(
                    "Failed to create bootstrap admin.");
            }
        }

        if (!await userManager.IsInRoleAsync(
                adminUser,
                AppRoles.Admin))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    adminUser,
                    AppRoles.Admin);

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    "Failed to assign Admin role.");
            }
        }
    }
}