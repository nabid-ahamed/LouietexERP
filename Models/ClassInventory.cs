using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Models
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Material Name")]
        public string Name { get; set; } // e.g., Cotton Fabric

        public string Category { get; set; } // Fabric, Accessories 

        [Required]
        public int Quantity { get; set; }

        [Display(Name = "Minimum Stock Level")]
        public int MinStockLevel { get; set; } // For Low Stock Alerts 

        public string Supplier { get; set; }
        
    }
}