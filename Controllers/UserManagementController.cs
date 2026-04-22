using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = "SuperAdmin")] // Only Super Admin can access
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // ✅ SHOW PROFILE REQUESTS
        public async Task<IActionResult> ViewModifications()
        {
            var requests = await _context.ProfileRequests
                .Where(r => !r.IsProcessed)
                .ToListAsync();

            return View(requests);
        }
        // ✅ PROCESS APPROVE / REJECT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessModification(int requestId, bool approved)
        {
            var request = await _context.ProfileRequests.FindAsync(requestId);

            if (request == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return NotFound();

            if (approved)
            {
                user.FullName = request.NewFullName;
                user.Email = request.NewEmail;
                user.UserName = request.NewEmail;

                user.NormalizedEmail = request.NewEmail.ToUpper();
                user.NormalizedUserName = request.NewEmail.ToUpper();

                request.Status = "Approved";
            }
            else
            {
                request.Status = "Rejected";
            }

            request.IsProcessed = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewModifications));
        }

        // ✅ Proper constructor (your version was not correct for MVC)
        public UserManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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