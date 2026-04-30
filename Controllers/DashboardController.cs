using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
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

                // ✅ FIXED DATE + SAFE SUM
                TodayProduction = await context.Productions
                    .Where(p => p.ProductionDate.Date == DateTime.Today)
                    .SumAsync(p => (int?)p.ActualOutput) ?? 0,

                RecentOrders = await context.Orders
                    .OrderByDescending(o => o.Id)
                    .Take(5)
                    .ToListAsync(),

                PendingUsers = await context.Users
                    .CountAsync(u => !u.IsApproved),

                PendingRequests = await context.ProfileRequests
                    .CountAsync(r => !r.IsProcessed)
            };

            return View(viewModel);
        }
    }
}