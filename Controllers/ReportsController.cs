using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LouietexERP.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Reports Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // ✅ Production Reports
        [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin + "," + SD.Role_ProductionManager)]
        public async Task<IActionResult> ProductionReports()
        {
            var productions = await _context.Productions.Include(p => p.Order).ToListAsync();
            
            ViewBag.TotalQty = productions.Sum(p => p.TargetQuantity);
            ViewBag.CompletedQty = productions.Where(p => p.Status == "Completed").Sum(p => p.TargetQuantity);
            ViewBag.RunningLines = productions.Where(p => p.Status == "Running").Count();
            
            return View(productions);
        }

        // ✅ QC Reports
        [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin + "," + SD.Role_QC)]
        public async Task<IActionResult> QCReports()
        {
            var inspections = await _context.QCInspections.Include(q => q.Production).ToListAsync();
            
            ViewBag.TotalChecked = inspections.Count;
            ViewBag.Passed = inspections.Count(q => q.QCStatus == "Passed");
            ViewBag.Failed = inspections.Count(q => q.QCStatus == "Failed");
            
            return View(inspections);
        }

        // ✅ Inventory Reports
        [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin + "," + SD.Role_ProductionManager)]
        public async Task<IActionResult> InventoryReports()
        {
            var inventory = await _context.Inventories.ToListAsync();
            
            ViewBag.TotalItems = inventory.Count;
            ViewBag.LowStockCount = inventory.Count(i => i.Quantity <= i.MinStockLevel);
            ViewBag.OutStockCount = inventory.Count(i => i.Quantity == 0);
            
            return View(inventory);
        }

        // ✅ Order Reports
        [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin + "," + SD.Role_Merchandiser)]
        public async Task<IActionResult> OrderReports()
        {
            var orders = await _context.Orders.ToListAsync();
            
            ViewBag.TotalOrders = orders.Count;
            ViewBag.ShippedCount = orders.Count(o => o.Status == "Shipped");
            ViewBag.PendingCount = orders.Count(o => o.Status == "Pending");
            
            return View(orders);
        }

        // ✅ Employee Reports
        [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin + "," + SD.Role_HR)]
        public async Task<IActionResult> EmployeeReports()
        {
            var employees = await _context.Employees.ToListAsync();
            
            ViewBag.TotalStaff = employees.Count;
            ViewBag.Depts = employees.Select(e => e.Department).Distinct().Count();
            
            return View(employees);
        }
    }
}
