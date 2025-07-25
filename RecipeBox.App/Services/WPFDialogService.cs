using RecipeBox.App.ViewModels;
using RecipeBox.App.Views;
using System.Windows; // Required for MessageBox

namespace RecipeBox.App.Services
{
    public class WPFDialogService : IDialogService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public bool ShowConfirmation(string message, string title)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        public AddIngredientResult ShowAddIngredientDialog()
        {
            var dialogVM = new AddIngredientViewModel();
            var dialog = new AddIngredientDialog
            {
                DataContext = dialogVM,
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                return new AddIngredientResult { Confirmed = true, Name = dialogVM.Name, Quantity = dialogVM.Quantity };
            }
            else
            {
                return new AddIngredientResult { Confirmed = false };
            }
        }
    }
}