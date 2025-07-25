namespace RecipeBox.Domain.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Instructions { get; set; }
        public bool IsPublic { get; set; }
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public virtual ICollection<RecipeIngredient> Ingredients { get; set; }
    }
}
