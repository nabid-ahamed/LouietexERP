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
    [Authorize(Roles = SD.Role_ProductionManager + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin + "," + SD.Role_OperationsManager)]
    public class ProductionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly LouietexERP.Services.IExportService _exportService;
 
        public ProductionsController(ApplicationDbContext context, LouietexERP.Services.IExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        // GET: Productions
        public async Task<IActionResult> Index(string? status, string? lineNumber, int? orderId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Productions.Include(p => p.Order).AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status == status);

            if (!string.IsNullOrEmpty(lineNumber))
                query = query.Where(p => p.LineNumber.Contains(lineNumber));

            if (orderId.HasValue)
                query = query.Where(p => p.OrderId == orderId.Value);

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt.Date <= endDate.Value.Date);

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentLine = lineNumber;
            ViewBag.OrderId = new SelectList(_context.Orders, "Id", "StyleCode", orderId);
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(await query.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        // GET: Productions/ExportPdf
        public async Task<IActionResult> ExportPdf(string? status, string? lineNumber, int? orderId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Productions.Include(p => p.Order).AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status == status);

            if (!string.IsNullOrEmpty(lineNumber))
                query = query.Where(p => p.LineNumber.Contains(lineNumber));

            if (orderId.HasValue)
                query = query.Where(p => p.OrderId == orderId.Value);

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt.Date <= endDate.Value.Date);

            var data = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            var headers = new[] { "Order", "Line", "Supervisor", "Target", "Actual", "Status", "Date" };
            var pdf = _exportService.GeneratePdf("Production Report", data, headers, p => new[] {
                p.Order?.StyleCode ?? "N/A",
                p.LineNumber ?? "",
                p.Supervisor ?? "",
                p.TargetQuantity.ToString(),
                p.ActualOutput.ToString(),
                p.Status ?? "",
                p.CreatedAt.ToShortDateString()
            });

            return File(pdf, "application/pdf", $"Production_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // GET: Productions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var production = await _context.Productions
                .Include(p => p.Order)
                .Include(p => p.QCInspections)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (production == null)
            {
                return NotFound();
            }

            return View(production);
        }

        // GET: Productions/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "StyleCode");
            return View();
        }

        // POST: Productions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,LineNumber,Supervisor,TargetQuantity,ActualOutput,StartDate,EndDate,Status")] Production production)
        {
            if (ModelState.IsValid)
            {
                production.CreatedAt = DateTime.UtcNow;
                production.UpdatedAt = DateTime.UtcNow;
                _context.Add(production);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "StyleCode", production.OrderId);
            return View(production);
        }

        // GET: Productions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var production = await _context.Productions.FindAsync(id);
            if (production == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "StyleCode", production.OrderId);
            return View(production);
        }

        // POST: Productions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,LineNumber,Supervisor,TargetQuantity,ActualOutput,DefectCount,StartDate,EndDate,Status,CreatedAt")] Production production)
        {
            if (id != production.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    production.UpdatedAt = DateTime.UtcNow;
                    _context.Update(production);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductionExists(production.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "StyleCode", production.OrderId);
            return View(production);
        }

        // GET: Productions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var production = await _context.Productions
                .Include(p => p.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (production == null)
            {
                return NotFound();
            }

            return View(production);
        }

        // POST: Productions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var production = await _context.Productions.FindAsync(id);
            if (production != null)
            {
                _context.Productions.Remove(production);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductionExists(int id)
        {
            return _context.Productions.Any(e => e.Id == id);
        }
    }
}