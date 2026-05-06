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
            // Last 6 months
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Reverse()
                .ToList();

            var monthlyOrders = new List<int>();
            var monthlyProduction = new List<int>();

            foreach (var month in last6Months)
            {
                var orderCount = await context.Orders
                    .CountAsync(o => o.DeliveryDate.Month == month.Month
                                  && o.DeliveryDate.Year == month.Year);

                var productionSum = await context.Productions
                    .Where(p => p.ProductionDate.Month == month.Month
                             && p.ProductionDate.Year == month.Year)
                    .SumAsync(p => (int?)p.ActualOutput) ?? 0;

                monthlyOrders.Add(orderCount);
                monthlyProduction.Add(productionSum);
            }

            var viewModel = new DashboardViewModel
            {
                TotalEmployees = await context.Employees.CountAsync(),

                TotalOrders = await context.Orders.CountAsync(),

                LowStockItems = await context.Inventories
                    .CountAsync(i => i.Quantity <= i.MinStockLevel),

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
                    .CountAsync(r => !r.IsProcessed),

                MonthLabels = last6Months.Select(m => m.ToString("MMM")).ToList(),
                MonthlyOrders = monthlyOrders,
                MonthlyProduction = monthlyProduction
            };

            return View(viewModel);
        }
    }
}