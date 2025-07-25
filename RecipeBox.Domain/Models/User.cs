namespace RecipeBox.Domain.Models
{
    public enum UserRole { Standard, Contributor, Administrator }

    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        // public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
