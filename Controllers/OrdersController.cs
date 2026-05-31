using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = SD.Role_Merchandiser + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin + "," + SD.Role_OperationsManager)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly LouietexERP.Services.IExportService _exportService;
 
        public OrdersController(ApplicationDbContext context, LouietexERP.Services.IExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string search)
        {
            var orders = _context.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                orders = orders.Where(o =>
                    o.BuyerName.Contains(search) ||
                    o.StyleCode.Contains(search) ||
                    o.Status.Contains(search)
                );
            }

            return View(await orders.ToListAsync());
        }

        // GET: Orders/ExportPdf
        public async Task<IActionResult> ExportPdf(string search)
        {
            var orders = _context.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                orders = orders.Where(o =>
                    o.BuyerName.Contains(search) ||
                    o.StyleCode.Contains(search) ||
                    o.Status.Contains(search)
                );
            }

            var data = await orders.ToListAsync();
            var headers = new[] { "Buyer Name", "Style Code", "Quantity", "Delivery Date", "Status" };
            var pdf = _exportService.GeneratePdf("Orders Report", data, headers, o => new[] {
                o.BuyerName ?? "",
                o.StyleCode ?? "",
                o.TotalQuantity.ToString(),
                o.DeliveryDate.ToShortDateString(),
                o.Status ?? ""
            });

            return File(pdf, "application/pdf", $"Orders_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // ✅ ADDED: Track Status with filter
        public async Task<IActionResult> TrackStatus(string status)
        {
            var orders = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(o => o.Status == status);

            ViewBag.CurrentStatus = status;

            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = SD.Role_Merchandiser + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Merchandiser + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Create([Bind("Id,BuyerName,StyleCode,TotalQuantity,DeliveryDate,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                
                await LouietexERP.Services.ActivityLogger.LogActivityAsync(
                    _context,
                    $"New Order: {order.StyleCode}",
                    $"Buyer: {order.BuyerName} | Quantity: {order.TotalQuantity:N0}",
                    "bi-bag-plus",
                    "bg-primary-subtle",
                    "text-primary",
                    "Orders"
                );
                
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = SD.Role_Merchandiser + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Merchandiser + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BuyerName,StyleCode,TotalQuantity,DeliveryDate,Status")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    
                    await LouietexERP.Services.ActivityLogger.LogActivityAsync(
                        _context,
                        $"Order Updated: {order.StyleCode}",
                        $"Buyer: {order.BuyerName} | Status set to: {order.Status}",
                        "bi-pencil-square",
                        "bg-warning-subtle",
                        "text-warning",
                        "Orders"
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = SD.Role_Merchandiser + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Merchandiser + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}