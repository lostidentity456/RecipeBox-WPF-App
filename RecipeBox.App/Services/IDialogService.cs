using RecipeBox.Domain.Models;

namespace RecipeBox.App.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        bool ShowConfirmation(string message, string title);
        AddIngredientResult ShowAddIngredientDialog();
        AddUserResult ShowAddUserDialog();
        ChangeRoleResult ShowChangeRoleDialog(string username, UserRole currentRole);
    }
}