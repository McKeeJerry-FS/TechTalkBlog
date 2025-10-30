using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;
using TechTalkBlog.Models;

namespace TechTalkBlog.Data
{
    public static class DataUtility
    {
        // Admin & Moderator - use with roles
        private const string? _adminRole = "Admin";
        private const string? _moderatorRole = "Moderator";

        public static string? GetConnectionString(IConfiguration config)
        {
            // 1) Normal config (appsettings / env: ConnectionStrings__DefaultConnection)
            var cs = config.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(cs)) return cs;

            // 2) Railway / Heroku URL-style variable
            var url = Environment.GetEnvironmentVariable("RAILWAY_DATABASE_URL")
                      ?? Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrWhiteSpace(url))
                return BuildFromUrl(url);

            // 3) Railway PG* vars
            var host = Environment.GetEnvironmentVariable("PGHOST");
            var db   = Environment.GetEnvironmentVariable("PGDATABASE");
            var user = Environment.GetEnvironmentVariable("PGUSER");
            var pwd  = Environment.GetEnvironmentVariable("PGPASSWORD");
            var port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";

            if (!string.IsNullOrWhiteSpace(host) &&
                !string.IsNullOrWhiteSpace(db) &&
                !string.IsNullOrWhiteSpace(user) &&
                !string.IsNullOrWhiteSpace(pwd))
            {
                return $"Host={host};Port={port};Database={db};Username={user};Password={pwd};SSL Mode=Require;Trust Server Certificate=true";
            }

            return null;
        }

        private static string BuildFromUrl(string url)
        {
            // Accept postgres:// or postgresql://
            url = url.Replace("postgres://", "postgresql://", StringComparison.OrdinalIgnoreCase);
            var uri = new Uri(url);

            var userInfo = uri.UserInfo.Split(':', 2);
            var user = Uri.UnescapeDataString(userInfo[0]);
            var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
            var db   = uri.AbsolutePath.TrimStart('/');
            var host = uri.Host;
            var port = uri.IsDefaultPort ? 5432 : uri.Port;

            return $"Host={host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
        }

        public static async Task ManageDataAsync(IServiceProvider serviceProvider)
        {
            var dbContextSvc    = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc  = serviceProvider.GetRequiredService<UserManager<BlogUser>>();
            var configuration   = serviceProvider.GetRequiredService<IConfiguration>();
            var roleManagerSvc  = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await WaitForDatabaseAsync(dbContextSvc); // retry until DB is reachable
            await dbContextSvc.Database.MigrateAsync();

            await SeedRolesAsync(roleManagerSvc);
            await SeedBlogUsersAsync(userManagerSvc, configuration);
        }

        private static async Task WaitForDatabaseAsync(DbContext db, int retries = 10, int delayMs = 2000)
        {
            var conn = (NpgsqlConnection)db.Database.GetDbConnection();

            // Safe log of where we’re connecting (no secrets)
            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            Console.WriteLine($"DB connecting to Host={csb.Host}; Port={csb.Port}; Database={csb.Database}; SSL Mode={csb.SslMode}");

            for (int i = 1; i <= retries; i++)
            {
                try
                {
                    await conn.OpenAsync();
                    await conn.CloseAsync();
                    Console.WriteLine("DB reachable.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DB not ready (attempt {i}/{retries}): {ex.Message}");
                    if (i == retries) throw;
                    await Task.Delay(delayMs);
                }
            }
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
