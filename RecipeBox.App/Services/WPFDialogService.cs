using RecipeBox.App.ViewModels;
using RecipeBox.App.Views;
using RecipeBox.Domain.Models;
using System.Windows;

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

        public AddUserResult ShowAddUserDialog()
        {
            var dialogVM = new AddUserViewModel();
            var dialog = new AddUserDialog
            {
                DataContext = dialogVM,
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                // The password was set on the ViewModel by the code-behind
                return new AddUserResult
                {
                    Confirmed = true,
                    Username = dialogVM.Username,
                    Password = dialogVM.Password,
                    Role = dialogVM.SelectedRole
                };
            }
            else
            {
                return new AddUserResult { Confirmed = false };
            }
        }

        public ChangeRoleResult ShowChangeRoleDialog(string username, UserRole currentRole)
        {
            var dialogVM = new ChangeRoleViewModel
            {
                Username = username,
                SelectedRole = currentRole
            };

            var dialog = new ChangeRoleDialog
            {
                DataContext = dialogVM,
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                return new ChangeRoleResult { Confirmed = true, NewRole = dialogVM.SelectedRole };
            }
            else
            {
                return new ChangeRoleResult { Confirmed = false };
            }
        }
    }
}