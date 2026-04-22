using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LouietexERP.Models
{
    public class ProfileRequest
    {
        [Key]
        public int Id { get; set; }

        // 🔗 Link to user (FOREIGN KEY)
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        // 🔄 Requested changes
        [Required]
        [Display(Name = "Requested New Name")]
        public string NewFullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Requested New Email")]
        public string NewEmail { get; set; }

        // 📅 Metadata
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public bool IsProcessed { get; set; } = false;

        // ✅ Use controlled values
        public string Status { get; set; } = "Pending";
        // Later you can convert this to enum if needed
    }
}