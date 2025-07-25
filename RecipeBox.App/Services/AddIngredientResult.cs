namespace RecipeBox.App.Services
{
    public class AddIngredientResult
    {
        public bool Confirmed { get; set; } // Did the user click "OK"?
        public string Name { get; set; }
        public string Quantity { get; set; }
    }
}
