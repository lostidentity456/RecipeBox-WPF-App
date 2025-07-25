using CommunityToolkit.Mvvm.ComponentModel;
using RecipeBox.Domain.Models;

namespace RecipeBox.App.ViewModels
{
    public partial class ChangeRoleViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username; // To display "Change role for: [Username]"

        [ObservableProperty]
        private UserRole _selectedRole;
    }
}