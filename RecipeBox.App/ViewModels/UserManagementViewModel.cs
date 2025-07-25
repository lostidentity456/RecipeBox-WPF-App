using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RecipeBox.App.Services;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System.Collections.ObjectModel;

namespace RecipeBox.App.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly IDbContextFactory<RecipeBoxContext> _contextFactory; 

        [ObservableProperty]
        private User _selectedUser;

        public ObservableCollection<User> Users { get; set; }

        public UserManagementViewModel(IDialogService dialogService, IDbContextFactory<RecipeBoxContext> contextFactory)
        {
            _dialogService = dialogService;
            _contextFactory = contextFactory;

            Users = new ObservableCollection<User>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using var context = _contextFactory.CreateDbContext();
            var usersFromDb = context.Users.ToList();

            Users = new ObservableCollection<User>(usersFromDb);
            OnPropertyChanged(nameof(Users));
        }

        [RelayCommand]
        private void AddUser()
        {
            var result = _dialogService.ShowAddUserDialog(); // Assume this service method exists now

            if (result.Confirmed)
            {
                using var context = _contextFactory.CreateDbContext();

                // Check if username already exists
                if (context.Users.Any(u => u.Username.ToLower() == result.Username.ToLower()))
                {
                    _dialogService.ShowMessage("A user with that username already exists.");
                    return;
                }

                var newUser = new User
                {
                    Username = result.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(result.Password),
                    Role = result.Role
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                _dialogService.ShowMessage("New user created successfully.");
                LoadUsers(); // Refresh the list
            }
        }

        [RelayCommand]
        private void ChangeRole()
        {
            if (SelectedUser == null)
            {
                _dialogService.ShowMessage("Please select a user to change their role.");
                return;
            }

            if (SelectedUser.Role == UserRole.Administrator)
            {
                _dialogService.ShowMessage("Administrator roles cannot be changed from this screen.");
                return;
            }

            var result = _dialogService.ShowChangeRoleDialog(SelectedUser.Username, SelectedUser.Role);

            if (result.Confirmed)
            {
                using var context = _contextFactory.CreateDbContext();
                var userToUpdate = context.Users.Find(SelectedUser.UserId);

                if (userToUpdate != null)
                {
                    userToUpdate.Role = result.NewRole;
                    context.SaveChanges();
                    _dialogService.ShowMessage($"{userToUpdate.Username}'s role has been updated to {result.NewRole}.");
                    LoadUsers(); // Refresh the list
                }
            }
        }

        [RelayCommand]
        private void DeleteUser()
        {
            if (SelectedUser == null)
            {
                _dialogService.ShowMessage("Please select a user to delete.");
                return;
            }
    
            if (SelectedUser.Role == UserRole.Administrator)
            {
                _dialogService.ShowMessage("Cannot delete an Administrator account.");
                return;
            }

            bool confirmed = _dialogService.ShowConfirmation(
                $"Are you sure you want to delete the user '{SelectedUser.Username}' and all their recipes? This action cannot be undone.",
                "Confirm Delete User");

            if (confirmed)
            {
                // Create a final short-lived context for the delete operation
                using var context = _contextFactory.CreateDbContext();

                // First, find and delete all recipes by this user
                var recipesToDelete = context.Recipes.Where(r => r.AuthorId == SelectedUser.UserId);
                context.Recipes.RemoveRange(recipesToDelete);

                // Now, attach and remove the user
                context.Users.Remove(new User { UserId = SelectedUser.UserId });

                context.SaveChanges();

                _dialogService.ShowMessage($"User '{SelectedUser.Username}' has been deleted.");

                // Refresh the UI list
                Users.Remove(SelectedUser);
            }
        }
    }
}