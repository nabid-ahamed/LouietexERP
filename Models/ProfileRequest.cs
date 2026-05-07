using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LouietexERP.Models
{
    public static class ProfileRequestStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }

    public static class ProfileRequestType
    {
        public const string ProfileChange = "ProfileChange";
    }

    public class ProfileRequest
    {
        [Key]
        public int Id { get; set; }

        // 🔗 User link (set in controller, NOT from form)
        [ValidateNever]   // 🔥 IMPORTANT FIX
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]   // 🔥 IMPORTANT FIX
        public ApplicationUser User { get; set; }

        // 🔄 Form inputs
        [Required]
        [Display(Name = "Requested New Name")]
        public string NewFullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Requested New Email")]
        public string NewEmail { get; set; }

        [Phone]
        [Display(Name = "Requested New Phone Number")]
        public string? NewPhoneNumber { get; set; }

        [Display(Name = "Requested Profile Picture")]
        public string? NewProfilePicturePath { get; set; }

        // 📅 Metadata
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public bool IsProcessed { get; set; } = false;

        // 📌 Type & Status
        [MaxLength(50)]
        public string RequestType { get; set; } = ProfileRequestType.ProfileChange;

        [MaxLength(50)]
        public string Status { get; set; } = ProfileRequestStatus.Pending;

        // 🛡 Audit Tracking
        public DateTime? ProcessedDate { get; set; }

        public string? ProcessedByUserId { get; set; }

        [ForeignKey("ProcessedByUserId")]
        [ValidateNever]
        public ApplicationUser? ProcessedByUser { get; set; }
    }
}