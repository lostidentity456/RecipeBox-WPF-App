using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RecipeBox.Data.DataContext;

namespace RecipeBox.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RecipeBoxContext>
    {
        public RecipeBoxContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../RecipeBox.App"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<RecipeBoxContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            });
            return new RecipeBoxContext(builder.Options);
        }
    }
}