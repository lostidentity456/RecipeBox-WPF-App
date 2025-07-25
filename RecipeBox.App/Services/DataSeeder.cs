using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;

namespace RecipeBox.App.Services
{
    public class DataSeeder
    {
        public static void SeedAdminUser(IServiceProvider serviceProvider)
        {
            var contextFactory = serviceProvider.GetRequiredService
                <IDbContextFactory<RecipeBoxContext>>();

            using var context = contextFactory.CreateDbContext();

            context.Database.EnsureCreated();

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string adminUsername = configuration["DefaultAdminUser:Username"];
            string adminPassword = configuration["DefaultAdminUser:Password"];

            if (!context.Users.Any(u => u.Username == adminUsername))
            {
                var adminUser = new User
                {
                    Username = adminUsername,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    Role = UserRole.Administrator
                };

                context.Users.Add(adminUser);
                context.SaveChanges(); // Synchronous version
            }
        }
    }
}