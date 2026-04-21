using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Department { get; set; } // e.g., Cutting, Sewing, Finishing

        [Required]
        public string Role { get; set; } // e.g., Admin, Supervisor, Operator

        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; } = DateTime.Now;
    }
}