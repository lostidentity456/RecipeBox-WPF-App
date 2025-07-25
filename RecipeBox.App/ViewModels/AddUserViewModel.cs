using CommunityToolkit.Mvvm.ComponentModel;
using RecipeBox.Domain.Models;

namespace RecipeBox.App.ViewModels
{
    public partial class AddUserViewModel : ObservableObject
    {
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _password;
        [ObservableProperty] private UserRole _selectedRole = UserRole.Standard;
    }
}
