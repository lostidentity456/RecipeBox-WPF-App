using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; 
using RecipeBox.App.Services;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System;

namespace RecipeBox.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private User _loggedInUser;

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ShowLoginView(); 
        }

        [RelayCommand]
        private void ShowLoginView()
        {
            var loginVM = _serviceProvider.GetRequiredService<LoginViewModel>();
            loginVM.OnLoginSuccess += OnLoginSuccess;
            CurrentViewModel = loginVM;
        }

        [RelayCommand]
        private void ShowRegistrationView()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<RegistrationViewModel>();
        }

        [RelayCommand]
        private void Logout()
        {
            _loggedInUser = null;
            ShowLoginView();
        }

        private void OnLoginSuccess(User loggedInUser)
        {
            _loggedInUser = loggedInUser;
            ShowRecipeListView(); 
        }

        private void ShowRecipeListView()
        {
            var recipeListVM = new RecipeListViewModel(
                _loggedInUser,
                _serviceProvider.GetRequiredService<IDialogService>(),
                 _serviceProvider.GetRequiredService<IDbContextFactory<RecipeBoxContext>>()
            );

            recipeListVM.OnCreateNewRecipeRequested += () => ShowRecipeDetailView();
            recipeListVM.OnEditRecipeRequested += (recipe) => ShowRecipeDetailView(recipe);
            CurrentViewModel = recipeListVM;
        }

        private void ShowRecipeDetailView(Recipe recipeToEdit = null)
        {
            var recipeDetailVM = new RecipeDetailViewModel(
                recipeToEdit,
                _loggedInUser,
                _serviceProvider.GetRequiredService<IDialogService>(),
                _serviceProvider.GetRequiredService<RecipeBoxContext>()
            );

            recipeDetailVM.OnSave += (recipeData) =>
            {
                SaveRecipe(recipeData);
                ShowRecipeListView(); 
            };

            recipeDetailVM.OnCancel += () =>
            {
                ShowRecipeListView();
            };

            CurrentViewModel = recipeDetailVM;
        }

        private void SaveRecipe(Recipe recipeData)
        {
            var contextFactory = _serviceProvider.GetRequiredService<IDbContextFactory<RecipeBoxContext>>();
            using var context = contextFactory.CreateDbContext();

            if (recipeData.RecipeId != 0)
            {
                var originalRecipe = context.Recipes
                                            .Include(r => r.Ingredients)
                                            .ThenInclude(ri => ri.Ingredient)
                                            .FirstOrDefault(r => r.RecipeId == recipeData.RecipeId);

                if (originalRecipe != null)
                {
                    originalRecipe.Title = recipeData.Title;
                    originalRecipe.Instructions = recipeData.Instructions;
                    originalRecipe.IsPublic = recipeData.IsPublic;

                    // Clear existing ingredients and re-add from the incoming data
                    originalRecipe.Ingredients.Clear();
                    foreach (var newRecipeIngredient in recipeData.Ingredients)
                    {
                        // Check if the ingredient entity itself is new or existing
                        var ingredient = context.Ingredients.Find(newRecipeIngredient.Ingredient.IngredientId);
                        if (ingredient == null)
                        {
                            // It's a brand new ingredient
                            originalRecipe.Ingredients.Add(newRecipeIngredient);
                        }
                        else
                        {
                            // It's an existing ingredient, so use the tracked one
                            newRecipeIngredient.Ingredient = ingredient;
                            originalRecipe.Ingredients.Add(newRecipeIngredient);
                        }
                    }
                }
            }
            else
            {
                // === CORRECTED CREATE LOGIC ===

                // 1. Loop through the ingredients provided by the ViewModel
                foreach (var recipeIngredient in recipeData.Ingredients)
                {
                    // 2. For each ingredient, check if it's a new one (ID is 0)
                    //    or if it's an existing one we're re-using.
                    var existingIngredient = context.Ingredients
                        .FirstOrDefault(i => i.Name.ToLower() == recipeIngredient.Ingredient.Name.ToLower());

                    if (existingIngredient != null)
                    {
                        // 3. If it EXISTS, discard the new object from the ViewModel
                        //    and point to the one we found in the database.
                        recipeIngredient.Ingredient = existingIngredient;
                    }
                    // 4. If it does NOT exist, we do nothing. EF will see the new object
                    //    (with ID=0) and correctly mark it as 'Added'.
                }

                // 5. Now, when we add the recipe, EF knows which ingredients are 'Added'
                //    and which ones already exist and just need linking.
                context.Recipes.Add(recipeData);
            }

            context.SaveChanges();
        }
    }
}