using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBox.App.Services;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System.Collections.ObjectModel;

namespace RecipeBox.App.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly RecipeBoxContext _context;

        [ObservableProperty]
        private User _selectedUser;

        public ObservableCollection<User> Users { get; set; }

        public UserManagementViewModel(IDialogService dialogService, RecipeBoxContext context)
        {
            _dialogService = dialogService;
            _context = context;

            Users = new ObservableCollection<User>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();
            // Use the injected _context
            var usersFromDb = _context.Users.ToList();
            foreach (var user in usersFromDb)
            {
                Users.Add(user);
            }
        }

        [RelayCommand]
        private void ChangeRole()
        {
            if (SelectedUser == null)
            {
                _dialogService.ShowMessage("Please select a user.");
                return;
            }

            var currentRole = SelectedUser.Role;
            var nextRole = (UserRole)(((int)currentRole + 1) % 3);
            SelectedUser.Role = nextRole;

            _context.Users.Update(SelectedUser);
            _context.SaveChanges();

            _dialogService.ShowMessage($"{SelectedUser.Username}'s role changed to {SelectedUser.Role}.");

            LoadUsers();
        }

    }
}