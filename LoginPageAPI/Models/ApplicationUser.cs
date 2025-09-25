using Microsoft.AspNetCore.Identity;

namespace LoginPageAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extra fields can go here (e.g., FirstName, LastName)
        public string? LanguageCode { get; set; }
    }
}
