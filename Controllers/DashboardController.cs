using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;

namespace LouietexERP.Controllers
{
    [Authorize]
    public class DashboardController(ApplicationDbContext context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalEmployees = await context.Employees.CountAsync(),

                TotalOrders = await context.Orders.CountAsync(),

                LowStockItems = await context.Inventories
                    .CountAsync(i => i.Quantity <= i.MinStockLevel),

                TodayProduction = await context.Productions
                    .Where(p => p.ProductionDate == DateTime.Today)
                    .SumAsync(p => p.ActualOutput),

                RecentOrders = await context.Orders
                    .OrderByDescending(o => o.Id)
                    .Take(5)
                    .ToListAsync(),

                // ✅ ADD THESE TWO LINES (IMPORTANT)
                PendingUsers = await context.Users
                    .CountAsync(u => !u.IsApproved),

                PendingRequests = await context.ProfileRequests
                    .CountAsync(r => !r.IsProcessed)
            };

            return View(viewModel);
        }
    }
}