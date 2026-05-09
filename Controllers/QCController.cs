using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Data;
using LouietexERP.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = SD.Role_QC + "," + SD.Role_Admin + "," + SD.Role_SuperAdmin)]
    public class QCController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        // GET: QC
        public async Task<IActionResult> Index(string? status, int? productionId, DateTime? startDate, DateTime? endDate, string? searchString)
        {
            var query = _context.QCInspections
                .Include(q => q.Production).ThenInclude(p => p!.Order)
                .Include(q => q.CheckedByUser)
                .AsQueryable();

            // Status Filter
            if (status == "Pending")
                query = query.Where(q => q.QCStatus == "Pending" || q.QCStatus == "Recheck Required");
            else if (!string.IsNullOrEmpty(status))
                query = query.Where(q => q.QCStatus == status);

            // Production Line Filter
            if (productionId.HasValue)
                query = query.Where(q => q.ProductionId == productionId.Value);

            // Date Range Filter
            if (startDate.HasValue)
                query = query.Where(q => q.InspectionDate.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                query = query.Where(q => q.InspectionDate.Date <= endDate.Value.Date);

            // Search Filter
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(q =>
                    (q.InspectorName != null && q.InspectorName.ToLower().Contains(searchString)) ||
                    (q.Production != null && q.Production.LineNumber.ToLower().Contains(searchString)) ||
                    (q.Production != null && q.Production.Order != null && q.Production.Order.StyleCode.ToLower().Contains(searchString)) ||
                    (q.Production != null && q.Production.Order != null && q.Production.Order.BuyerName.ToLower().Contains(searchString)) ||
                    (q.Remarks != null && q.Remarks.ToLower().Contains(searchString))
                );
            }

            ViewBag.CurrentStatus = status;
            ViewBag.ProductionId = new SelectList(_context.Productions, "Id", "LineNumber", productionId);
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentSearch = searchString;

            return View(await query.OrderByDescending(q => q.InspectionDate).ToListAsync());
        }

        // GET: QC/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var qCInspection = await _context.QCInspections
                .Include(q => q.Production)
                .ThenInclude(p => p!.Order)
                .Include(q => q.CheckedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (qCInspection == null) return NotFound();

            return View(qCInspection);
        }

        // GET: QC/Create
        public IActionResult Create()
        {
            ViewData["ProductionId"] = new SelectList(_context.Productions.Include(p => p.Order), "Id", "LineNumber");
            return View();
        }

        // POST: QC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductionId,DefectCount,Remarks,QCStatus,InspectorName")] QCInspection qCInspection)
        {
            // Populate defaults and missing fields
            qCInspection.CheckedByUserId = _userManager.GetUserId(User);
            qCInspection.InspectionDate = DateTime.Now;
            qCInspection.CreatedAt = DateTime.Now;
            qCInspection.UpdatedAt = DateTime.Now;

            ModelState.Remove("CheckedByUserId");
            ModelState.Remove("CheckedByUser");
            ModelState.Remove("Production");

            if (ModelState.IsValid)
            {
                _context.Add(qCInspection);
                // Also update Production Defect Count
                var production = await _context.Productions.FindAsync(qCInspection.ProductionId);
                if (production != null)
                {
                    production.DefectCount += qCInspection.DefectCount;
                    _context.Update(production);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductionId"] = new SelectList(_context.Productions, "Id", "LineNumber", qCInspection.ProductionId);
            return View(qCInspection);
        }

        // GET: QC/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var qCInspection = await _context.QCInspections.FindAsync(id);
            if (qCInspection == null) return NotFound();
            ViewData["ProductionId"] = new SelectList(_context.Productions, "Id", "LineNumber", qCInspection.ProductionId);
            return View(qCInspection);
        }

        // POST: QC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductionId,CheckedByUserId,DefectCount,Remarks,QCStatus,InspectionDate,CreatedAt,InspectorName")] QCInspection qCInspection)
        {
            if (id != qCInspection.Id) return NotFound();

            ModelState.Remove("CheckedByUser");
            ModelState.Remove("Production");

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate defects if needed, but for simplicity just update inspection
                    qCInspection.UpdatedAt = DateTime.Now;
                    _context.Update(qCInspection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!QCInspectionExists(qCInspection.Id))
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductionId"] = new SelectList(_context.Productions, "Id", "LineNumber", qCInspection.ProductionId);
            return View(qCInspection);
        }

        // GET: QC/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var qCInspection = await _context.QCInspections
                .Include(q => q.Production)
                .ThenInclude(p => p!.Order)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (qCInspection == null) return NotFound();

            return View(qCInspection);
        }

        // POST: QC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var qCInspection = await _context.QCInspections.FindAsync(id);
            if (qCInspection != null)
            {
                // Also update Production Defect Count (subtract deleted defects)
                var production = await _context.Productions.FindAsync(qCInspection.ProductionId);
                if (production != null)
                {
                    production.DefectCount = Math.Max(0, production.DefectCount - qCInspection.DefectCount);
                    _context.Update(production);
                }

                _context.QCInspections.Remove(qCInspection);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool QCInspectionExists(int id)
        {
            return _context.QCInspections.Any(e => e.Id == id);
        }
    }
}
