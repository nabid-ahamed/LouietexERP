using Microsoft.AspNetCore.Identity;

namespace LouietexERP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; } = false; // Default to false for new registrations
        public string FullName { get; set; }
        public string PreferredTheme { get; set; } = "light";
    }
}