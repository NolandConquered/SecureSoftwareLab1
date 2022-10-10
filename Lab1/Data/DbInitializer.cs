using Lab1.Data;
using Lab1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab1.Data
{
    public static class DbInitializer
    {
        public static AppSecrets appSecrets { get; set; }
        public static async Task<int> SeedUsersAndRoles(IServiceProvider serviceProvider)
        {
            // create the database if it doesn't exist
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Check if roles already exist and exit if there are
            if (roleManager.Roles.Count() > 0)
                return 1;  // should log an error message here

            // Seed roles
            int result = await SeedRoles(roleManager);
            if (result != 0)
                return 2;  // should log an error message here

            // Check if users already exist and exit if there are
            if (userManager.Users.Count() > 0)
                return 3;  // should log an error message here

            // Seed users
            result = await SeedUsers(userManager);
            if (result != 0)
                return 4;  // should log an error message here

            return 0;
        }

        private static async Task<int> SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Create Admin Role
            var result = await roleManager.CreateAsync(new IdentityRole(System.Configuration.ConfigurationManager.AppSettings["APPSETTING_AdminRole"]));
            if (!result.Succeeded)
                return 1;  // should log an error message here

            // Create Member Role
            result = await roleManager.CreateAsync(new IdentityRole(appSecrets.MemberRole));
            if (!result.Succeeded)
                return 2;  // should log an error message here

            return 0;
        }

        private static async Task<int> SeedUsers(UserManager<ApplicationUser> userManager)
        {
            // Create Admin User
            var adminUser = new ApplicationUser
            {
                UserName = appSecrets.AdminUsername,
                Email = appSecrets.AdminEmail,
                FirstName = appSecrets.AdminFirstName,
                LastName = appSecrets.AdminLastName,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, appSecrets.AdminPassword);
            if (!result.Succeeded)
                return 1;  // should log an error message here

            // Assign user to Admin role
            result = await userManager.AddToRoleAsync(adminUser, appSecrets.AdminRole);
            if (!result.Succeeded)
                return 2;  // should log an error message here

            // Create Member User
            var memberUser = new ApplicationUser
            {
                UserName = appSecrets.MemberUsername,
                Email = appSecrets.MemberEmail,
                FirstName = appSecrets.MemberFirstName,
                LastName = appSecrets.MemberLastName,
                EmailConfirmed = true
            };
            result = await userManager.CreateAsync(memberUser, appSecrets.MemberPassword);
            if (!result.Succeeded)
                return 3;  // should log an error message here

            // Assign user to Member role
            result = await userManager.AddToRoleAsync(memberUser, appSecrets.MemberRole);
            if (!result.Succeeded)
                return 4;  // should log an error message here

            return 0;
        }
    }
}
