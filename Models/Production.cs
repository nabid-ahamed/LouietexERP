using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Models
{
    public class Production
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Production Line")]
        public string LineNumber { get; set; } // e.g., Line-01, Line-02

        [Required]
        [Display(Name = "Target Quantity")]
        public int TargetQuantity { get; set; }

        [Required]
        [Display(Name = "Actual Output")]
        public int ActualOutput { get; set; }

        [Display(Name = "Defect Count")]
        public int DefectCount { get; set; } // To track quality issues

        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; } = DateTime.Today;

        // Simple calculation for efficiency
        public double Efficiency => TargetQuantity > 0
            ? (double)ActualOutput / TargetQuantity * 100
            : 0;
    }
}