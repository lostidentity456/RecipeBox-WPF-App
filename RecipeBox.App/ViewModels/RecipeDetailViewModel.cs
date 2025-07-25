using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RecipeBox.App.Services;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System.Collections.ObjectModel;

namespace RecipeBox.App.ViewModels
{
    public partial class RecipeDetailViewModel : ObservableObject
    {
        public Action<Recipe> OnSave { get; set; }
        public Action OnCancel { get; set; }

        private readonly User _currentUser;
        private readonly IDialogService _dialogService;
        private readonly IDbContextFactory<RecipeBoxContext> _contextFactory; // Use the factory

        public int RecipeId { get; private set; }
        public bool CanMakePublic { get; }

        [ObservableProperty] private string _title;
        [ObservableProperty] private string _instructions;
        [ObservableProperty] private bool _isPublic;
        [ObservableProperty] private RecipeIngredient _selectedIngredient;

        public ObservableCollection<RecipeIngredient> Ingredients { get; set; }

        // The constructor is now consistent with RecipeListViewModel
        public RecipeDetailViewModel(Recipe recipeToEdit, User currentUser, IDialogService dialogService, IDbContextFactory<RecipeBoxContext> contextFactory)
        {
            _currentUser = currentUser;
            _dialogService = dialogService;
            _contextFactory = contextFactory;

            Ingredients = new ObservableCollection<RecipeIngredient>();
            CanMakePublic = _currentUser.Role == UserRole.Contributor || _currentUser.Role == UserRole.Administrator;

            if (recipeToEdit != null)
            {
                RecipeId = recipeToEdit.RecipeId;
                Title = recipeToEdit.Title;
                Instructions = recipeToEdit.Instructions;
                IsPublic = recipeToEdit.IsPublic;

                LoadIngredientsForRecipe(recipeToEdit.RecipeId);
            }
        }

        private void LoadIngredientsForRecipe(int recipeId)
        {
            using var context = _contextFactory.CreateDbContext();

            var ingredientsFromDb = context.RecipeIngredients
                                           .Include(ri => ri.Ingredient) 
                                           .Where(ri => ri.RecipeId == recipeId)
                                           .ToList();

            foreach (var ing in ingredientsFromDb)
            {
                Ingredients.Add(ing);
            }
        }

        [RelayCommand]
        private void AddIngredient()
        {
            var result = _dialogService.ShowAddIngredientDialog();

            if (result.Confirmed)
            {
                var newIngredient = new RecipeIngredient
                {
                    Ingredient = new Ingredient { Name = result.Name }, 
                    Quantity = result.Quantity
                };

                Ingredients.Add(newIngredient);
            }
        }

        [RelayCommand]
        private void RemoveIngredient()
        {
            if (SelectedIngredient != null)
            {
                Ingredients.Remove(SelectedIngredient);
            }
        }

        [RelayCommand]
        private void Save()
        {
            var recipeData = new Recipe
            {
                RecipeId = this.RecipeId,
                Title = this.Title,
                Instructions = this.Instructions,
                IsPublic = this.IsPublic,
                AuthorId = _currentUser.UserId,
                Ingredients = this.Ingredients.ToList()
            };
            OnSave?.Invoke(recipeData);
        }

        [RelayCommand]
        private void Cancel() => OnCancel?.Invoke();
    }
}