using Microsoft.AspNetCore.Identity;

namespace LouietexERP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; } = false;
        public bool IsDisabled { get; set; } = false;
        public string FullName { get; set; } = string.Empty;

        // Profile Picture
        public string? ProfilePicturePath { get; set; }
        public bool ProfilePictureApproved { get; set; } = false;
        public string? PendingProfilePicturePath { get; set; } // waiting for admin approval
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}