using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // SHOW PROFILE REQUESTS
        // =========================
        public async Task<IActionResult> Requests()
        {
            var requests = await _context.ProfileRequests
                .OrderByDescending(r => r.RequestDate) // latest first
                .ToListAsync();

            return View(requests);
        }
    }
}