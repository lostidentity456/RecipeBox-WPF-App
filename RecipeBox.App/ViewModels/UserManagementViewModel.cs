using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBox.Data.DataContext;
using RecipeBox.Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RecipeBox.App.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        public Action OnGoBack { get; set; }

        public ObservableCollection<User> Users { get; set; }

        [ObservableProperty]
        private User _selectedUser;

        [ObservableProperty]
        private UserRole _selectedUserRole;

        // This provides the list of all possible roles to the ComboBox
        public UserRole[] AllRoles => (UserRole[])Enum.GetValues(typeof(UserRole));

        public UserManagementViewModel()
        {
            Users = new ObservableCollection<User>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();
            using var context = new RecipeBoxContext();
            var users = context.Users.ToList();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        // When the user selection changes, update the ComboBox to show their current role
        partial void OnSelectedUserChanged(User value)
        {
            if (value != null)
            {
                SelectedUserRole = value.Role;
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (SelectedUser != null)
            {
                using var context = new RecipeBoxContext();
                var userToUpdate = context.Users.Find(SelectedUser.UserId);
                if (userToUpdate != null)
                {
                    userToUpdate.Role = SelectedUserRole;
                    context.SaveChanges();
                    // Optional: Refresh the list to reflect changes immediately
                    LoadUsers();
                }
            }
        }

        [RelayCommand]
        private void GoBack()
        {
            OnGoBack?.Invoke();
        }
    }
}
