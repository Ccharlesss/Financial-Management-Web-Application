// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using System.Threading.Tasks;

// namespace ManageFinance.Services
// {
//     public class RoleAndUserInitializer
//     {
//         // class contains a single static method, InitializeAsync, which performs the initialization tasks:
//         // IServiceProvider: This is a service provider that comes from the Dependency Injection (DI) system in ASP.NET Core.
//         // It is used to get instances of services such as RoleManager<IdentityRole> and UserManager<IdentityUser>.       
//         public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger<RoleAndUserInitializer> logger)
//         {   // resolves (or retrieves) two key services from the DI container:
//             // GetRequiredService<T>() method throws an exception if the service cannot be found, ensuring that the required services are available.
//             var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//             var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
//             logger.LogInformation("Starting Role and User Initialization...");

//             // Define the roles you want to add
//             var roles = new[] {"Admin", "User" };
//             // Create Roles (if they don’t exist)
//             logger.LogInformation("Checking if roles exist...");
//             foreach (var role in roles)
//             {
//                 if (!await roleManager.RoleExistsAsync(role))
//                 {
//                     var result = await roleManager.CreateAsync(new IdentityRole(role));
//                     if (result.Succeeded)
//                     {
//                         logger.LogInformation($"Role '{role}' created successfully.");
//                     }
//                     else
//                     {
//                         logger.LogError($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
//                     }
//                 }
//             }

//             // Check if the admin user exists
//             string adminEmail = "AdminAccount@gmail.com";
//             string adminPassword = "Defaultpasseword14$";
//             // Create the Admin User (if they don’t exist)
//             var adminUser = await userManager.FindByEmailAsync(adminEmail);
//             if (adminUser == null)
//             {
//                 // Create the admin user
//                 var user = new IdentityUser { UserName = adminEmail, Email = adminEmail };
//                 var result = await userManager.CreateAsync(user, adminPassword);

//                 if (result.Succeeded)
//                 {
//                     await userManager.AddToRoleAsync(user, "Admin");
//                     logger.LogInformation($"Admin user '{adminEmail}' created and assigned to 'Admin' role.");
//                 }
//                 else
//                 {
//                     logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//                 }
//             }
//         }
//     }
// }







using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ManageFinance.Services
{
    public class RoleAndUserInitializer
    {
        // This method performs the initialization tasks for roles and users
        public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger<RoleAndUserInitializer> logger)
        {
            // Resolve RoleManager and UserManager from the DI container
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>(); // Use AppUser here
            logger.LogInformation("Starting Role and User Initialization...");

            // Define the roles you want to add
            var roles = new[] { "Admin", "User" };

            // Create Roles (if they don’t exist)
            logger.LogInformation("Checking if roles exist...");
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                    else
                    {
                        logger.LogError($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Check if the admin user exists
            string adminEmail = "AdminAccount@gmail.com";
            string adminPassword = "Defaultpasseword14$";

            // Create the Admin User (if they don’t exist)
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                // Create the admin user using the custom AppUser class
                var user = new AppUser { UserName = adminEmail, Email = adminEmail }; // Use AppUser instead of IdentityUser
                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    logger.LogInformation($"Admin user '{adminEmail}' created and assigned to 'Admin' role.");
                }
                else
                {
                    logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
