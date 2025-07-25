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
        private readonly RecipeBoxContext _context;

        public int RecipeId { get; private set; }
        public bool CanMakePublic { get; }

        [ObservableProperty] private string _title;
        [ObservableProperty] private string _instructions;
        [ObservableProperty] private bool _isPublic;
        [ObservableProperty] private RecipeIngredient _selectedIngredient;

        public ObservableCollection<RecipeIngredient> Ingredients { get; set; }

        public RecipeDetailViewModel(Recipe recipeToEdit, User currentUser,
            IDialogService dialogService, RecipeBoxContext context)
        {
            _currentUser = currentUser;
            _dialogService = dialogService;
            _context = context;

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
            var ingredientsFromDb = _context.RecipeIngredients
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
                var ingredientEntity = GetOrCreateIngredient(result.Name);

                var newIngredient = new RecipeIngredient
                {
                    Ingredient = ingredientEntity,
                    Quantity = result.Quantity
                };

                Ingredients.Add(newIngredient);
            }
        }

        private Ingredient GetOrCreateIngredient(string name)
        {
            var ingredient = _context.Ingredients.FirstOrDefault(i => i.Name.ToLower() == name.ToLower());

            if (ingredient == null)
            {
                ingredient = new Ingredient { Name = name };
                _context.Ingredients.Add(ingredient);
            }
            return ingredient;
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