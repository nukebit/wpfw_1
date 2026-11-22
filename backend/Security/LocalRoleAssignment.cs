using Microsoft.AspNetCore.Identity;

namespace PortfolioApi.Security;

internal static class LocalRoleAssignment
{
    public static async Task<int> RunAsync(WebApplication app, string[] args)
    {
        if (!app.Environment.IsDevelopment())
        {
            Console.Error.WriteLine("Local role assignment requires the Development environment.");
            return 1;
        }

        if (args.Length != 3 || args[2] is not (PortfolioRoles.Editor or PortfolioRoles.Admin))
        {
            Console.Error.WriteLine("Usage: --assign-role <email> <Editor|Admin>");
            return 1;
        }

        await using var scope = app.Services.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await users.FindByEmailAsync(args[1].Trim());
        if (user is null)
        {
            Console.Error.WriteLine("Register the user before assigning a role.");
            return 1;
        }

        var role = args[2];
        if (!await users.IsInRoleAsync(user, role))
        {
            var result = await users.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.Error.WriteLine(error.Description);
                }

                return 1;
            }
        }

        Console.WriteLine($"Assigned {role} to {user.Email}. Sign in again to use the new role.");
        return 0;
    }
}
