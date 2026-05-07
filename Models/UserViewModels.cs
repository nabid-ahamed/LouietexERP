using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Models
{
    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        
        public bool IsApproved { get; set; }
        public bool IsDisabled { get; set; }

        public string Status
        {
            get
            {
                if (!IsApproved) return "Pending";
                if (IsDisabled) return "Disabled";
                return "Active";
            }
        }
    }

    public class UserDetailsViewModel : UserListViewModel
    {
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class UserEditViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Is Approved?")]
        public bool IsApproved { get; set; }

        [Display(Name = "Is Disabled?")]
        public bool IsDisabled { get; set; }
    }
}
