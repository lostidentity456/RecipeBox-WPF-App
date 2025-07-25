using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RecipeBox.App.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username;

        // This event will be triggered on successful login
        public Action<User> OnLoginSuccess { get; set; }

        [RelayCommand]
        private void Login(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox == null) return;

            string plainPassword = passwordBox.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(plainPassword))
            {
                MessageBox.Show("Username and password cannot be empty.");
                return;
            }

            using var context = new RecipeBoxContext();

            // 1. Find the user by username
            var user = context.Users.FirstOrDefault(u => u.Username == Username);

            // 2. Check if user exists and verify password
            if (user != null && BCrypt.Net.BCrypt.Verify(plainPassword, user.PasswordHash))
            {
                // 3. Trigger the success event and pass the logged-in user object
                OnLoginSuccess?.Invoke(user);
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }
    }
}