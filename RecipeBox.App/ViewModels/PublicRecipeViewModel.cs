using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using RecipeBox.Data.DataContext;
using System.Collections.ObjectModel;

namespace RecipeBox.App.ViewModels
{
    public partial class PublicRecipeViewModel : ObservableObject
    {
        private readonly IDbContextFactory<RecipeBoxContext> _contextFactory;

        public ObservableCollection<PublicRecipeDisplay> PublicRecipes { get; set; }

        public PublicRecipeViewModel(IDbContextFactory<RecipeBoxContext> contextFactory)
        {
            _contextFactory = contextFactory;
            PublicRecipes = new ObservableCollection<PublicRecipeDisplay>();
            LoadPublicRecipes();
        }

        private void LoadPublicRecipes()
        {
            using var context = _contextFactory.CreateDbContext();

            var recipes = context.Recipes
                .Where(r => r.IsPublic)
                .Select(r => new PublicRecipeDisplay 
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    AuthorUsername = context.Users
                                            .Where(u => u.UserId == r.AuthorId)
                                            .Select(u => u.Username)
                                            .FirstOrDefault()
                })
                .ToList();

            PublicRecipes = new ObservableCollection<PublicRecipeDisplay>(recipes);
            OnPropertyChanged(nameof(PublicRecipes));
        }
    }
}