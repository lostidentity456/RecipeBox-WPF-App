using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;

namespace RecipeBox.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject _currentViewModel;

        private User _loggedInUser;

        public MainViewModel()
        {
            ShowLoginView();
        }

        [RelayCommand]
        private void ShowLoginView()
        {
            var loginVM = new LoginViewModel();
            loginVM.OnLoginSuccess += OnLoginSuccess;
            CurrentViewModel = loginVM;
        }

        [RelayCommand]
        private void ShowRegistrationView()
        {
            CurrentViewModel = new RegistrationViewModel();
        }

        private void ShowRecipeListView()
        {
            var recipeListVM = new RecipeListViewModel(_loggedInUser);
            recipeListVM.OnCreateNewRecipeRequested += () => ShowRecipeDetailView();

            
            recipeListVM.OnEditRecipeRequested += (recipe) =>
            {
                using var context = new RecipeBoxContext();

                var recipeWithIngredients = context.Recipes
                    .Include(r => r.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefault(r => r.RecipeId == recipe.RecipeId);

                if (recipeWithIngredients != null)
                {
                    ShowRecipeDetailView(recipeWithIngredients);
                }
            };
            CurrentViewModel = recipeListVM;
        }

        private void OnLoginSuccess(User loggedInUser)
        {
            _loggedInUser = loggedInUser;

            ShowRecipeListView();
        }

        private void ShowRecipeDetailView(Recipe recipeToEdit = null)
        {
            var recipeDetailVM = recipeToEdit == null
                ? new RecipeDetailViewModel(_loggedInUser)
                : new RecipeDetailViewModel(recipeToEdit, _loggedInUser);

            recipeDetailVM.OnCancel += ShowRecipeListView;

            recipeDetailVM.OnSave += () =>
            {
                using var context = new RecipeBoxContext();

                // ==== LOGIC FOR UPDATING AN EXISTING RECIPE ====
                if (recipeToEdit != null)
                {
                    // 1. Fetch the original recipe from DB, including its ingredients
                    var existingRecipe = context.Recipes
                                                .Include(r => r.Ingredients)
                                                .FirstOrDefault(r => r.RecipeId == recipeToEdit.RecipeId);

                    if (existingRecipe != null)
                    {
                        // 2. Update simple properties
                        existingRecipe.Title = recipeDetailVM.Title;
                        existingRecipe.Instructions = recipeDetailVM.Instructions;
                        existingRecipe.IsPublic = recipeDetailVM.IsPublic;

                        // 3. Clear the old ingredients list
                        existingRecipe.Ingredients.Clear();

                        // 4. Add the new/updated ingredients
                        foreach (var ingredientVM in recipeDetailVM.Ingredients)
                        {
                            // Find or create the ingredient
                            var ingredient = context.Ingredients
                                                    .FirstOrDefault(i => i.Name.ToLower() == ingredientVM.Ingredient.Name.ToLower())
                                             ?? new Ingredient { Name = ingredientVM.Ingredient.Name };

                            existingRecipe.Ingredients.Add(new RecipeIngredient
                            {
                                Ingredient = ingredient,
                                Quantity = ingredientVM.Quantity
                            });
                        }
                    }
                }
                // ==== LOGIC FOR CREATING A NEW RECIPE ====
                else
                {
                    // 1. Create the new Recipe object
                    var newRecipe = new Recipe
                    {
                        Title = recipeDetailVM.Title,
                        Instructions = recipeDetailVM.Instructions,
                        IsPublic = recipeDetailVM.IsPublic,
                        AuthorId = _loggedInUser.UserId,
                        Ingredients = new List<RecipeIngredient>() // Initialize the collection
                    };

                    // 2. Add the ingredients from the ViewModel
                    foreach (var ingredientVM in recipeDetailVM.Ingredients)
                    {
                        // Find or create the ingredient
                        var ingredient = context.Ingredients
                                                .FirstOrDefault(i => i.Name.ToLower() == ingredientVM.Ingredient.Name.ToLower())
                                         ?? new Ingredient { Name = ingredientVM.Ingredient.Name };

                        newRecipe.Ingredients.Add(new RecipeIngredient
                        {
                            Ingredient = ingredient,
                            Quantity = ingredientVM.Quantity
                        });
                    }

                    // 3. Add the fully constructed recipe to the context
                    context.Recipes.Add(newRecipe);
                }

                context.SaveChanges();
                ShowRecipeListView();
            };

            CurrentViewModel = recipeDetailVM;
        }

        [RelayCommand]
        private void Logout()
        {
            _loggedInUser = null;
            ShowLoginView();
        }
    }
}
