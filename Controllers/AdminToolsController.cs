using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin)]
    public class AdminToolsController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // ✅ SEED HISTORICAL HIGH-VOLUME DATA
        // This will create real database records for the last 12 months
        public async Task<IActionResult> SeedHistoricalData()
        {
            var today = DateTime.Now;
            var rnd = new Random();

            // We will create data for the last 12 months to make the graph look real and high-volume
            for (int i = 11; i >= 0; i--)
            {
                var monthDate = today.AddMonths(-i);
                var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                
                // Create 2 large orders per month to reach the 80k-140k range
                for (int j = 1; j <= 2; j++)
                {
                    var order = new Order
                    {
                        BuyerName = $"Premium Buyer {rnd.Next(1, 5)}",
                        StyleCode = $"STYLE-{monthDate:MMM}{rnd.Next(1000, 9999)}",
                        TotalQuantity = rnd.Next(50000, 75000),
                        Status = "Completed",
                        DeliveryDate = startOfMonth.AddMonths(2),
                        CreatedAt = startOfMonth
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync(); // Save to get ID

                    // Split the order production across 5 lines
                    var lines = new[] { "Line-01", "Line-02", "Line-03", "Line-04", "Line-05" };
                    foreach (var line in lines)
                    {
                        var target = order.TotalQuantity / 5;
                        var production = new Production
                        {
                            OrderId = order.Id,
                            LineNumber = line,
                            Supervisor = "System Seeder",
                            TargetQuantity = target,
                            ActualOutput = (int)(target * (rnd.Next(90, 99) / 100.0)),
                            Status = "Completed",
                            CreatedAt = startOfMonth.AddDays(rnd.Next(1, 28)),
                            UpdatedAt = startOfMonth.AddDays(28)
                        };
                        _context.Productions.Add(production);
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Successfully seeded 12 months of high-volume historical data!";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
