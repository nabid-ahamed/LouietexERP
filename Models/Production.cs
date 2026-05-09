using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LouietexERP.Models
{
    public class Production
    {
        [Key]
        public int Id { get; set; }

        // Relationship to Order
        [Required(ErrorMessage = "Selecting an Order is required.")]
        [Display(Name = "Linked Order")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [Required]
        [Display(Name = "Production Line")]
        public string LineNumber { get; set; } = null!; // e.g., Line-01, Line-02

        [Display(Name = "Supervisor")]
        public string? Supervisor { get; set; }

        [Required]
        [Display(Name = "Target Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Target must be greater than 0")]
        public int TargetQuantity { get; set; }

        [Required]
        [Display(Name = "Actual Output")]
        [Range(0, int.MaxValue, ErrorMessage = "Output cannot be negative")]
        public int ActualOutput { get; set; }

        [Display(Name = "Defect Count")]
        public int DefectCount { get; set; } // Tracked here, but also from QC module later

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Production Status")]
        public string Status { get; set; } = "Pending"; // Pending, Running, Completed, Delayed

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Simple calculation for efficiency
        public double Efficiency => TargetQuantity > 0
            ? (double)ActualOutput / TargetQuantity * 100
            : 0;

        // Navigation to QC
        public ICollection<QCInspection>? QCInspections { get; set; }
    }
}