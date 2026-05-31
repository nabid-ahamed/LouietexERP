using System;
using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Models
{
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Subtitle { get; set; } = null!;

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        public string Icon { get; set; } = null!;

        [Required]
        public string IconBg { get; set; } = null!;

        [Required]
        public string IconText { get; set; } = null!;

        public string? Module { get; set; }
    }
}
