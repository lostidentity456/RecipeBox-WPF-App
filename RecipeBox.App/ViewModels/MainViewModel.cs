using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeBox.App.Services;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System;
using System.Linq; 

namespace RecipeBox.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ShowUserManagementViewCommand))]
        private User _loggedInUser;

        public bool IsAdmin => _loggedInUser?.Role == UserRole.Administrator;

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ShowLoginView();
        }

        [RelayCommand(CanExecute = nameof(IsAdmin))]
        private void ShowUserManagementView()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<UserManagementViewModel>();
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
            LoggedInUser = null;
            ShowLoginView();
        }

        private void OnLoginSuccess(User loggedInUser)
        {
            LoggedInUser = loggedInUser;
            ShowRecipeListView();
        }

        [RelayCommand]
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
            // === CRITICAL FIX IS HERE ===
            // Provide the IDbContextFactory, not a direct DbContext.
            var recipeDetailVM = new RecipeDetailViewModel(
                recipeToEdit,
                _loggedInUser,
                _serviceProvider.GetRequiredService<IDialogService>(),
                _serviceProvider.GetRequiredService<IDbContextFactory<RecipeBoxContext>>()
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

            // The rest of your SaveRecipe logic is excellent and does not need to change.
            if (recipeData.RecipeId != 0)
            {
                // ... Update Logic ...
                var originalRecipe = context.Recipes
                    .Include(r => r.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefault(r => r.RecipeId == recipeData.RecipeId);

                if (originalRecipe != null)
                {
                    originalRecipe.Title = recipeData.Title;
                    originalRecipe.Instructions = recipeData.Instructions;
                    originalRecipe.IsPublic = recipeData.IsPublic;

                    originalRecipe.Ingredients.Clear();
                    foreach (var newRecipeIngredient in recipeData.Ingredients)
                    {
                        var ingredient = context.Ingredients.Find(newRecipeIngredient.Ingredient.IngredientId);
                        if (ingredient == null)
                        {
                            originalRecipe.Ingredients.Add(newRecipeIngredient);
                        }
                        else
                        {
                            newRecipeIngredient.Ingredient = ingredient;
                            originalRecipe.Ingredients.Add(newRecipeIngredient);
                        }
                    }
                }
            }
            else
            {
                // ... Create Logic ...
                foreach (var recipeIngredient in recipeData.Ingredients)
                {
                    var existingIngredient = context.Ingredients
                        .FirstOrDefault(i => i.Name.ToLower() == recipeIngredient.Ingredient.Name.ToLower());

                    if (existingIngredient != null)
                    {
                        recipeIngredient.Ingredient = existingIngredient;
                    }
                }
                context.Recipes.Add(recipeData);
            }
            context.SaveChanges();
        }
    }
}