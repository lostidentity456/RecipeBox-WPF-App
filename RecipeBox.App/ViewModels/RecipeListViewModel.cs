using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RecipeBox.App.Services;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System.Collections.ObjectModel;

namespace RecipeBox.App.ViewModels
{
    public partial class RecipeListViewModel : ObservableObject
    {
        private readonly User _currentUser;
        private readonly IDialogService _dialogService;
        private readonly IDbContextFactory<RecipeBoxContext> _contextFactory;

        [ObservableProperty] private string _welcomeMessage;
        [ObservableProperty] private Recipe _selectedRecipe;
        [ObservableProperty] private string _searchTerm;

        public Action OnCreateNewRecipeRequested { get; set; }
        public Action<Recipe> OnEditRecipeRequested { get; set; }

        public RecipeListViewModel(User loggedInUser, IDialogService dialogService,
            IDbContextFactory<RecipeBoxContext> contextFactory)
        {
            _currentUser = loggedInUser;
            _dialogService = dialogService;
            _contextFactory = contextFactory;

            WelcomeMessage = $"Welcome, {loggedInUser.Username}! You're logged in as {loggedInUser.Role}.";
            Recipes = new ObservableCollection<Recipe>();
            LoadRecipes();
        }


        public ObservableCollection<Recipe> Recipes { get; set; }

        partial void OnSearchTermChanged(string value)
        {
            LoadRecipes();
        }

        private void LoadRecipes()
        {
            if (_currentUser == null) return;

            using var context = _contextFactory.CreateDbContext();

            var query = context.Recipes.Where(r => r.AuthorId == _currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(r => r.Title.ToLower().Contains(SearchTerm.ToLower()));
            }

            Recipes = new ObservableCollection<Recipe>(query.ToList());
            OnPropertyChanged(nameof(Recipes));
        }

        [RelayCommand]
        private void CreateNewRecipe() => OnCreateNewRecipeRequested?.Invoke();

        [RelayCommand]
        private void EditRecipe()
        {
            if (SelectedRecipe != null)
            {
                OnEditRecipeRequested?.Invoke(SelectedRecipe);
            }
        }

        [RelayCommand]
        private void DeleteRecipe()
        {
            if (SelectedRecipe == null)
            {
                _dialogService.ShowMessage("Please select a recipe to delete.");
                return;
            }

            bool confirmed = _dialogService.ShowConfirmation(
                $"Are you sure you want to delete '{SelectedRecipe.Title}'?", "Confirm Delete");

            if (confirmed)
            {
                using var context = _contextFactory.CreateDbContext();

                context.Recipes.Remove(new Recipe { RecipeId = SelectedRecipe.RecipeId });
                context.SaveChanges();

                Recipes.Remove(SelectedRecipe);
            }
        }
    }
}