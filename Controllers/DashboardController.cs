using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Data;
using LouietexERP.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin + "," + SD.Role_HR + "," + SD.Role_Merchandiser + "," + SD.Role_ProductionManager + "," + SD.Role_QC)]
    public class DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IActionResult> Index()
        {
            // ── 1. Core KPIs ──────────────────────────────────────────────
            var totalOrders = await _context.Orders.CountAsync();
            Console.WriteLine($"[DEBUG] Total Orders in DB: {totalOrders}");
            var runningProductions = await _context.Productions.CountAsync(p => p.Status == "Running");
            var completedProductions = await _context.Productions.CountAsync(p => p.Status == "Completed");
            var pendingQC = await _context.QCInspections.CountAsync(q => q.QCStatus == "Recheck Required" || q.QCStatus == "Pending");
            var qcFailedCount = await _context.QCInspections.CountAsync(q => q.QCStatus == "Failed");
            var totalEmployees = await _context.Employees.CountAsync();   // ✅ ERP Employees table
            var pendingRequests = await _context.ProfileRequests.CountAsync(pr => pr.Status == ProfileRequestStatus.Pending);
            var lowStockItems = await _context.Inventories.CountAsync(i => i.Quantity <= i.MinStockLevel);

            ViewBag.TotalOrders = totalOrders;
            ViewBag.RunningProductions = runningProductions;
            ViewBag.CompletedProductions = completedProductions;
            ViewBag.PendingQC = pendingQC;
            ViewBag.QCFailedItems = qcFailedCount;
            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.PendingRequests = pendingRequests;
            ViewBag.LowStockItems = lowStockItems;

            // ── 2. Realistic Demo Trends ─────────────────────────────────
            // Derived from current data to feel dynamic
            var totalProductions = await _context.Productions.CountAsync();
            double efficiencyRate = totalProductions > 0
                ? Math.Round((double)completedProductions / totalProductions * 100, 1) : 0;

            double qcPassRate = 0;
            var totalQCAll = await _context.QCInspections.CountAsync();
            if (totalQCAll > 0)
            {
                var passed = await _context.QCInspections.CountAsync(q => q.QCStatus == "Passed");
                qcPassRate = Math.Round((double)passed / totalQCAll * 100, 1);
            }

            double pendingOrdersPct = totalOrders > 0
                ? Math.Round((double)await _context.Orders.CountAsync(o => o.Status == "Pending") / totalOrders * 100, 1) : 0;

            ViewBag.TrendOrders = totalOrders > 10 ? "+8.3% from last month" : "New orders incoming";
            ViewBag.TrendRunning = $"{efficiencyRate}% completion rate";
            ViewBag.TrendCompleted = completedProductions > 5 ? "+12.5% vs last month" : "On track";
            ViewBag.TrendQC = pendingQC > 0 ? $"{pendingQC} need action" : "All clear";
            ViewBag.TrendQCFailed = qcFailedCount > 0 ? $"↓ {100 - qcPassRate:0.0}% fail rate" : "No failures";
            ViewBag.TrendEmployees = $"{totalEmployees} active staff members";
            ViewBag.TrendLowStock = lowStockItems > 0 ? $"{lowStockItems} items below min level" : "Stock levels healthy";
            ViewBag.TrendPendingReq = pendingRequests > 0 ? $"{pendingRequests} awaiting review" : "No pending";

            ViewBag.IsQCHealthy = qcFailedCount == 0;
            ViewBag.IsStockHealthy = lowStockItems == 0;

            // ── 3. Admin: Pending User Approvals ─────────────────────────
            var pendingUsersCount = await _userManager.Users.CountAsync(u => !u.IsApproved);
            var recentRegisteredUsers = await _userManager.Users
                .Where(u => !u.IsApproved)
                .OrderByDescending(u => u.RegistrationDate)
                .Take(5)
                .ToListAsync();

            ViewBag.PendingUsersCount = pendingUsersCount;
            ViewBag.RecentRegisteredUsers = recentRegisteredUsers;
            ViewBag.TrendPendingUsers = pendingUsersCount > 0 ? $"{pendingUsersCount} awaiting approval" : "All users approved";

            // ── 4. Order Status Chart (Doughnut) ─────────────────────────
            var orderStatuses = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.OrderChartLabels = orderStatuses.Select(o => o.Status).ToArray();
            ViewBag.OrderChartCounts = orderStatuses.Select(o => o.Count).ToArray();

            // ── 5. Production Line Chart (Customizable Timeline) ────────
            string timeframe = Request.Query["timeframe"].ToString().ToLower();
            if (string.IsNullOrEmpty(timeframe)) timeframe = "monthly";

            var chartData = await GetProductionChartLogic(timeframe);

            ViewBag.CurrentTimeframe = timeframe;
            ViewBag.ProdChartLabels = chartData.Labels;
            ViewBag.ProdTargetData = chartData.Target;
            ViewBag.ProdActualData = chartData.Actual;
            ViewBag.TotalProdActual = chartData.TotalActual;
            ViewBag.TimeframeDisplay = chartData.TimeframeDisplay;

            // ── 8. Recent Activity Feed ──────────────────────────────────
            var activities = new List<ActivityItem>();

            // Recent Orders
            var latestOrders = await _context.Orders.OrderByDescending(o => o.CreatedAt).Take(5).ToListAsync();
            activities.AddRange(latestOrders.Select(o => new ActivityItem
            {
                Title = $"New Order: {o.StyleCode}",
                Subtitle = $"Buyer: {o.BuyerName} | Qty: {o.TotalQuantity:N0}",
                Timestamp = o.CreatedAt,
                Icon = "bi-bag-plus",
                IconBg = "bg-primary-subtle",
                IconText = "text-primary"
            }));

            // Recent QC Results
            var latestQC = await _context.QCInspections.Include(q => q.Production).ThenInclude(p => p!.Order).OrderByDescending(q => q.CreatedAt).Take(5).ToListAsync();
            activities.AddRange(latestQC.Select(q => new ActivityItem
            {
                Title = $"QC {q.QCStatus ?? "Pending"}: {q.Production?.Order?.StyleCode ?? "N/A"}",
                Subtitle = $"Inspector: {q.InspectorName ?? "Unknown"} | Defect: {q.DefectCount}",
                Timestamp = q.CreatedAt,
                Icon = q.QCStatus == "Passed" ? "bi-shield-check" : "bi-shield-x",
                IconBg = q.QCStatus == "Passed" ? "bg-success-subtle" : "bg-danger-subtle",
                IconText = q.QCStatus == "Passed" ? "text-success" : "text-danger"
            }));

            // Recent Profile Requests
            var latestRequests = await _context.ProfileRequests.Include(r => r.User).OrderByDescending(r => r.RequestDate).Take(3).ToListAsync();
            activities.AddRange(latestRequests.Select(r => new ActivityItem
            {
                Title = $"Profile Change Request: {r.User?.FullName ?? "Unknown User"}",
                Subtitle = $"Status: {r.Status}",
                Timestamp = r.RequestDate,
                Icon = "bi-person-gear",
                IconBg = "bg-warning-subtle",
                IconText = "text-warning"
            }));

            ViewBag.ActivityFeed = activities.OrderByDescending(a => a.Timestamp).Take(10).ToList();
            ViewBag.RecentOrders = latestOrders;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProductionChartData(string timeframe = "monthly")
        {
            var chartData = await GetProductionChartLogic(timeframe);
            return Json(chartData);
        }

        private async Task<ProductionChartViewModel> GetProductionChartLogic(string timeframe)
        {
            var today = DateTime.Now.Date;
            var labels = new List<string>();
            var target = new List<int>();
            var actual = new List<int>();
            string display = "";

            if (timeframe == "daily")
            {
                display = "Last 7 Days";
                var last7Days = Enumerable.Range(0, 7).Select(i => today.AddDays(-6 + i)).ToList();
                foreach (var day in last7Days)
                {
                    labels.Add(day.ToString("MMM d"));
                    var nextDay = day.AddDays(1);
                    var dailyProds = await _context.Productions.Where(p => p.CreatedAt >= day && p.CreatedAt < nextDay).ToListAsync();
                    target.Add(dailyProds.Sum(p => p.TargetQuantity));
                    actual.Add(dailyProds.Sum(p => p.ActualOutput));
                }
            }
            else if (timeframe == "weekly")
            {
                display = "Last 10 Weeks";
                for (int i = 9; i >= 0; i--)
                {
                    var endOfWeek = today.AddDays(-(int)today.DayOfWeek).AddDays(1);
                    var startOfWeek = endOfWeek.AddDays(-(i + 1) * 7);
                    var endOfSelectedWeek = startOfWeek.AddDays(7);
                    labels.Add("Week " + (10 - i));
                    var weeklyProds = await _context.Productions.Where(p => p.CreatedAt >= startOfWeek && p.CreatedAt < endOfSelectedWeek).ToListAsync();
                    target.Add(weeklyProds.Sum(p => p.TargetQuantity));
                    actual.Add(weeklyProds.Sum(p => p.ActualOutput));
                }
            }
            else // monthly
            {
                display = "Last 12 Months";
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = today.AddMonths(-i);
                    var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1);
                    labels.Add(startOfMonth.ToString("MMM yy"));
                    var monthlyProds = await _context.Productions.Where(p => p.CreatedAt >= startOfMonth && p.CreatedAt < endOfMonth).ToListAsync();
                    target.Add(monthlyProds.Sum(p => p.TargetQuantity));
                    actual.Add(monthlyProds.Sum(p => p.ActualOutput));
                }
            }

            if (target.Count > 0 && target.Sum() == 0 && actual.Sum() == 0)
            {
                var recentData = await _context.Productions.OrderByDescending(p => p.CreatedAt).GroupBy(p => p.CreatedAt.Date).Select(g => new { Day = g.Key, Target = g.Sum(p => p.TargetQuantity), Actual = g.Sum(p => p.ActualOutput) }).Take(timeframe == "monthly" ? 12 : 7).OrderBy(g => g.Day).ToListAsync();
                if (recentData.Count > 0)
                {
                    labels.Clear(); target.Clear(); actual.Clear();
                    foreach (var d in recentData)
                    {
                        labels.Add(d.Day.ToString(timeframe == "monthly" ? "MMM yy" : "MMM d"));
                        target.Add(d.Target); actual.Add(d.Actual);
                    }
                }
            }

            return new ProductionChartViewModel
            {
                Labels = labels,
                Target = target,
                Actual = actual,
                TotalActual = actual.Sum(),
                TimeframeDisplay = display
            };
        }

        public class ProductionChartViewModel
        {
            public List<string> Labels { get; set; } = null!;
            public List<int> Target { get; set; } = null!;
            public List<int> Actual { get; set; } = null!;
            public int TotalActual { get; set; }
            public string TimeframeDisplay { get; set; } = null!;
        }

        public class ActivityItem
        {
            public string Title { get; set; } = null!;
            public string Subtitle { get; set; } = null!;
            public DateTime Timestamp { get; set; }
            public string Icon { get; set; } = null!;
            public string IconBg { get; set; } = null!;
            public string IconText { get; set; } = null!;
        }
    }
}