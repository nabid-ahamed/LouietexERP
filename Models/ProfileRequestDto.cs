using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Models
{
    public class ProfileRequestDto
    {
        [Required]
        [Display(Name = "Requested New Name")]
        public string NewFullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Requested New Email")]
        public string NewEmail { get; set; }
    }
}
