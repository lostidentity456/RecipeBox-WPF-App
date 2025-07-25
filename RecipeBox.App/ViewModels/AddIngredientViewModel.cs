using CommunityToolkit.Mvvm.ComponentModel;

namespace RecipeBox.App.ViewModels
{
    public partial class AddIngredientViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _quantity;
    }
}