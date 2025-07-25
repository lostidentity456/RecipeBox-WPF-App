using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBox.App.Services;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;


namespace RecipeBox.App.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly RecipeBoxContext _context;
        
        [ObservableProperty]
        private string _username;

        public RegistrationViewModel(IDialogService dialogService, RecipeBoxContext context)
        {
            _dialogService = dialogService;
            _context = context;
        }

        [RelayCommand]
        private void Register(object parameter)
        {
            // Cast the parameter to the correct type
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            if (passwordBox == null)
            {
                _dialogService.ShowMessage("An error occurred with the password input.");
                return;
            }

            // Get the password from the control directly
            string plainPassword = passwordBox.Password;

            // The rest of your logic now uses the local 'plainPassword' variable
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(plainPassword))
            {
                _dialogService.ShowMessage("Username and password cannot be empty.");
                return;
            }

            if (_context.Users.Any(u => u.Username == Username))
            {
                _dialogService.ShowMessage("Username already taken.");
                return;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var newUser = new User
            {
                Username = this.Username,
                PasswordHash = hashedPassword,
                Role = UserRole.Standard
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            _dialogService.ShowMessage("Registration successful! You can now log in.");
        }
    }
}