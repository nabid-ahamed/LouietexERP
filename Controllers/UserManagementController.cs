using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = "SuperAdmin")] // Only Super Admin can access
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ✅ Proper constructor (your version was not correct for MVC)
        public UserManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Show users who are NOT approved
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingUsers = await _context.Users
                .Where(u => !u.IsApproved)
                .ToListAsync();

            return View(pendingUsers);
        }

        // ✅ Approve user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            user.IsApproved = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingApprovals));
        }

        // ✅ Reject user (optional but important)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            // Option 1: Delete user
            _context.Users.Remove(user);

            // Option 2 (alternative): mark as rejected instead of deleting
            // user.IsApproved = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingApprovals));
        }
    }
}