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
    public class QCController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QCController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: QC
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.QCInspections
                .Include(q => q.Production).ThenInclude(p => p!.Order)
                .Include(q => q.CheckedByUser)
                .AsQueryable();

            // Map "Pending" to the combined pending+recheck filter
            if (status == "Pending")
                query = query.Where(q => q.QCStatus == "Pending" || q.QCStatus == "Recheck Required");
            else if (!string.IsNullOrEmpty(status))
                query = query.Where(q => q.QCStatus == status);

            ViewBag.CurrentStatus = status;
            return View(await query.OrderByDescending(q => q.InspectionDate).ToListAsync());
        }

        // GET: QC/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var qCInspection = await _context.QCInspections
                .Include(q => q.Production)
                .ThenInclude(p => p.Order)
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
        public async Task<IActionResult> Create([Bind("ProductionId,DefectCount,Remarks,QCStatus")] QCInspection qCInspection)
        {
            // Populate defaults and missing fields
            qCInspection.CheckedByUserId = _userManager.GetUserId(User);
            qCInspection.InspectionDate = DateTime.UtcNow;
            qCInspection.CreatedAt = DateTime.UtcNow;
            qCInspection.UpdatedAt = DateTime.UtcNow;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductionId,CheckedByUserId,DefectCount,Remarks,QCStatus,InspectionDate,CreatedAt")] QCInspection qCInspection)
        {
            if (id != qCInspection.Id) return NotFound();

            ModelState.Remove("CheckedByUser");
            ModelState.Remove("Production");

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate defects if needed, but for simplicity just update inspection
                    qCInspection.UpdatedAt = DateTime.UtcNow;
                    _context.Update(qCInspection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QCInspectionExists(qCInspection.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductionId"] = new SelectList(_context.Productions, "Id", "LineNumber", qCInspection.ProductionId);
            return View(qCInspection);
        }

        private bool QCInspectionExists(int id)
        {
            return _context.QCInspections.Any(e => e.Id == id);
        }
    }
}
