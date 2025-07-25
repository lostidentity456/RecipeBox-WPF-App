using RecipeBox.Domain.Models;

namespace RecipeBox.App.Services
{
    public class AddUserResult
    {
        public bool Confirmed { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}