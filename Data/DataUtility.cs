using Npgsql;
using Microsoft.AspNetCore.Identity;
using TechTalkBlog.Models;
using Microsoft.EntityFrameworkCore;


namespace TechTalkBlog.Data
{
    public static class DataUtility
    {

        // Admin & Moderator - use with roles
        private const string? _adminRole = "Admin";
        private const string? _moderatorRole = "Moderator";
 

        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider serviceProvider)
        {
            var dbContextSvc = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc = serviceProvider.GetRequiredService<UserManager<BlogUser>>();
            var configurationSvc = serviceProvider.GetRequiredService<IConfiguration>();
            var roleManagerSvc = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // align the database by checking the migrations
            await dbContextSvc.Database.MigrateAsync();

            // Seed some info
            await SeedRolesAsync(roleManagerSvc);
            await SeedBlogUsersAsync(userManagerSvc, configurationSvc);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole!))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole!));
            }

            if (!await roleManager.RoleExistsAsync(_moderatorRole!))
            {
                await roleManager.CreateAsync(new IdentityRole(_moderatorRole!));
            }
        }

        private static async Task SeedBlogUsersAsync(UserManager<BlogUser> userManager, IConfiguration configuration)
        {
            string? adminEmail = configuration["AdminEmail"] ?? Environment.GetEnvironmentVariable("AdminEmail");
            string? adminPassword = configuration["AdminPWD"] ?? Environment.GetEnvironmentVariable("AdminPWD");
            string? moderatorEmail = configuration["ModeratorEmail"] ?? Environment.GetEnvironmentVariable("ModeratorEmail");
            string? moderatorPassword = configuration["ModeratorPWD"] ?? Environment.GetEnvironmentVariable("ModeratorPWD");

            try
            {
                BlogUser? adminUser = new()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Jerry",
                    LastName = "McKee",
                    EmailConfirmed = true
                };

                
                BlogUser? blogUser = await userManager.FindByEmailAsync(adminEmail!);
                

                if (blogUser == null)
                {
                    await userManager.CreateAsync(adminUser, adminPassword!);
                    await userManager.AddToRoleAsync(adminUser, _adminRole!);
                }


                BlogUser? moderatorUser = new()
                {
                    UserName = moderatorEmail,
                    Email = moderatorEmail,
                    FirstName = "John",
                    LastName = "Smith",
                    EmailConfirmed = true
                };

                blogUser = await userManager.FindByEmailAsync(moderatorEmail!);
                
                if (blogUser == null)
                {
                    await userManager.CreateAsync(moderatorUser, moderatorPassword!);
                    await userManager.AddToRoleAsync(moderatorUser, _moderatorRole!);
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("****************** ERROR *****************");
                Console.WriteLine($"Failure Seeding Default Blog Users Error: {ex.Message}");
                Console.WriteLine("****************** ERROR *****************");
                Console.ResetColor();
                throw;
            }

        }
    }
}
