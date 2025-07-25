using RecipeBox.Domain.Models;

namespace RecipeBox.App.Services
{
    public class ChangeRoleResult
    {
        public bool Confirmed { get; set; }
        public UserRole NewRole { get; set; }
    }
}