using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LouietexERP.Models
{
    public class QCInspection
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Selecting a Production run is required.")]
        [Display(Name = "Linked Production")]
        public int ProductionId { get; set; }

        [ForeignKey("ProductionId")]
        public Production? Production { get; set; }

        [Required]
        [Display(Name = "Inspected By (User ID)")]
        public string CheckedByUserId { get; set; } = string.Empty;

        [ForeignKey("CheckedByUserId")]
        public ApplicationUser? CheckedByUser { get; set; }

        [Display(Name = "Defect Count")]
        [Range(0, int.MaxValue, ErrorMessage = "Defects cannot be negative")]
        public int DefectCount { get; set; }

        [Display(Name = "Remarks/Notes")]
        public string? Remarks { get; set; }

        [Required]
        [Display(Name = "QC Status")]
        public string QCStatus { get; set; } = "Passed"; // Passed, Failed, Recheck Required

        [DataType(DataType.Date)]
        [Display(Name = "Inspection Date")]
        public DateTime InspectionDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
