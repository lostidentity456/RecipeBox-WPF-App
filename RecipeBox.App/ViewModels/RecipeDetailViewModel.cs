using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBox.App.Views;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System.Collections.ObjectModel;

namespace RecipeBox.App.ViewModels
{
    public partial class RecipeDetailViewModel : ObservableObject
    {
        public Action OnSave { get; set; }
        public Action OnCancel { get; set; }
        public int RecipeId { get; private set; }
        public bool CanMakePublic { get; }

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _instructions;

        [ObservableProperty]
        private bool _isPublic;

        public ObservableCollection<RecipeIngredient> Ingredients { get; set; }

        [ObservableProperty]
        private RecipeIngredient _selectedIngredient;

        public RecipeDetailViewModel(User currentUser)
        {
            CanMakePublic = currentUser.Role == UserRole.Contributor ||
                currentUser.Role == UserRole.Administrator;
            Ingredients = new ObservableCollection<RecipeIngredient>();
        }

        public RecipeDetailViewModel(Recipe recipeToEdit, User currentUser)
        {
            CanMakePublic = currentUser.Role == UserRole.Contributor ||
                currentUser.Role == UserRole.Administrator;

            RecipeId = recipeToEdit.RecipeId;
            Title = recipeToEdit.Title;
            Instructions = recipeToEdit.Instructions;
            IsPublic = recipeToEdit.IsPublic;
            
            Ingredients = new ObservableCollection<RecipeIngredient>
                (recipeToEdit.Ingredients ?? Enumerable.Empty<RecipeIngredient>());
        }

        [RelayCommand]
        private void AddIngredient()
        {
            var dialogVM = new AddIngredientViewModel();
            var dialog = new AddIngredientDialog
            {
                DataContext = dialogVM,
                Owner = App.Current.MainWindow 
            };

            if (dialog.ShowDialog() == true)
            {
                var newIngredient = new RecipeIngredient
                {
                    Ingredient = new Ingredient { Name = dialogVM.Name },
                    Quantity = dialogVM.Quantity
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
            OnSave?.Invoke();
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCancel?.Invoke();
        }
    }
}