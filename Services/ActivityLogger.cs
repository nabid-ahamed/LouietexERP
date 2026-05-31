using System;
using System.Threading.Tasks;
using LouietexERP.Data;
using LouietexERP.Models;

namespace LouietexERP.Services
{
    public static class ActivityLogger
    {
        public static async Task LogActivityAsync(
            ApplicationDbContext context, 
            string title, 
            string subtitle, 
            string icon, 
            string iconBg, 
            string iconText, 
            string? module = null)
        {
            try
            {
                var log = new ActivityLog
                {
                    Title = title,
                    Subtitle = subtitle,
                    Timestamp = DateTime.Now,
                    Icon = icon,
                    IconBg = iconBg,
                    IconText = iconText,
                    Module = module
                };
                context.ActivityLogs.Add(log);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ActivityLogger failed: {ex.Message}");
            }
        }
    }
}
