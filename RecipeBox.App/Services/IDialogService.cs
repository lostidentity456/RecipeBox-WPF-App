namespace RecipeBox.App.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        bool ShowConfirmation(string message, string title);
        AddIngredientResult ShowAddIngredientDialog();
    }
}