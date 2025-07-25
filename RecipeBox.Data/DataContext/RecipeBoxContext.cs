using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecipeBox.Domain.Models;

namespace RecipeBox.Data.DataContext
{
    public class RecipeBoxContext : DbContext
    {
        public RecipeBoxContext(DbContextOptions<RecipeBoxContext> options) : base(options) 
        { 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
        }
    }
}
