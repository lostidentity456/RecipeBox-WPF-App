using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBox.Domain.Models;
using RecipeBox.Data.DataContext;
using System.Windows;

namespace RecipeBox.App.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username;
        [RelayCommand]
        private void Register(object parameter)
        {
            // Cast the parameter to the correct type
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            if (passwordBox == null)
            {
                MessageBox.Show("An error occurred with the password input.");
                return;
            }

            // Get the password from the control directly
            string plainPassword = passwordBox.Password;

            // The rest of your logic now uses the local 'plainPassword' variable
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(plainPassword))
            {
                MessageBox.Show("Username and password cannot be empty.");
                return;
            }

            using var context = new RecipeBoxContext();

            if (context.Users.Any(u => u.Username == Username))
            {
                MessageBox.Show("Username already taken.");
                return;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var newUser = new User
            {
                Username = this.Username,
                PasswordHash = hashedPassword,
                Role = UserRole.Standard
            };

            context.Users.Add(newUser);
            context.SaveChanges();

            MessageBox.Show("Registration successful! You can now log in.");
        }
    }
}