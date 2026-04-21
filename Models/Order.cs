using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Buyer Name")]
        public string BuyerName { get; set; }

        [Required]
        [Display(Name = "Style/Item Code")]
        public string StyleCode { get; set; } // e.g., T-Shirt-Blue-XL

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int TotalQuantity { get; set; }

        [Required]
        [Display(Name = "Delivery Deadline")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Order Status")]
        public string Status { get; set; } = "Pending"; // e.g., Pending, In Production, Shipped
    }
}