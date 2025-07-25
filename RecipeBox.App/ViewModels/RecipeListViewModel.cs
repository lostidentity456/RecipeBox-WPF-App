using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace RecipeBox.App.ViewModels
{
    public partial class RecipeListViewModel : ObservableObject
    {
        private readonly User _currentUser;
        public bool IsAdmin { get; }

        [ObservableProperty]
        private string _welcomeMessage;
        [ObservableProperty]
        private Recipe _selectedRecipe;
        [ObservableProperty]
        private string _searchTerm;

        public Action OnCreateNewRecipeRequested { get; set; }
        public Action<Recipe> OnEditRecipeRequested { get; set; }

        [RelayCommand]
        private void CreateNewRecipe()
        {
            OnCreateNewRecipeRequested?.Invoke();
        }
        [RelayCommand]
        private void EditRecipe()
        {
            if (SelectedRecipe != null)
            {
                OnEditRecipeRequested?.Invoke(SelectedRecipe);
            }
        }

        public ObservableCollection<Recipe> Recipes { get; set; }

        public RecipeListViewModel(User loggedInUser)
        {
            _currentUser = loggedInUser;
            IsAdmin = loggedInUser.Role == UserRole.Administrator;
            WelcomeMessage = $"Welcome, {loggedInUser.Username}! You're logged in as {loggedInUser.Role}.";

            Recipes = new ObservableCollection<Recipe>();
            LoadRecipes();
        }

        partial void OnSearchTermChanged(string value)
        {
            LoadRecipes();
        }

        private void LoadRecipes()
        {
            Recipes.Clear();

            using var context = new RecipeBoxContext();

            var query = context.Recipes.Where(r => r.AuthorId == _currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(r => r.Title.ToLower().Contains(SearchTerm.ToLower()));
            }

            var userRecipes = query.ToList();

            foreach (var recipe in userRecipes)
            {
                Recipes.Add(recipe);
            }
        }

        [RelayCommand]
        private void DeleteRecipe()
        {
            if (SelectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe to delete.");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedRecipe.Title}'?",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using var context = new RecipeBoxContext();
                context.Recipes.Remove(SelectedRecipe);
                context.SaveChanges();

                Recipes.Remove(SelectedRecipe);
            }
        }
    }
}