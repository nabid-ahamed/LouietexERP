using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin)]
    public class UserManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        // ✅ SHOW PROFILE REQUESTS IN ONE PAGE
        public async Task<IActionResult> ViewModifications()
        {
            var requests = await _context.ProfileRequests
                .Where(r => !r.IsProcessed || r.Status == Models.ProfileRequestStatus.Pending)
                .Include(r => r.User)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        // ✅ PROCESS APPROVE / REJECT PROFILE CHANGE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessModification(int requestId, bool approved)
        {
            var request = await _context.ProfileRequests.FindAsync(requestId);
            if (request == null) return NotFound();

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            if (approved)
            {
                user.FullName = request.NewFullName;
                if (!string.IsNullOrEmpty(request.NewEmail) && request.NewEmail != user.Email)
                {
                    user.Email = request.NewEmail;
                    user.UserName = request.NewEmail;
                    user.NormalizedEmail = request.NewEmail.ToUpper();
                    user.NormalizedUserName = request.NewEmail.ToUpper();
                }

                if (request.NewPhoneNumber != user.PhoneNumber)
                {
                    await _userManager.SetPhoneNumberAsync(user, request.NewPhoneNumber);
                }

                if (!string.IsNullOrEmpty(request.NewProfilePicturePath))
                {
                    user.ProfilePicturePath = request.NewProfilePicturePath;
                    user.ProfilePictureApproved = true;
                }

                request.Status = ProfileRequestStatus.Approved;
                TempData["Message"] = "Profile change approved!";
            }
            else
            {
                request.Status = ProfileRequestStatus.Rejected;
                TempData["Warning"] = "Profile change rejected.";
            }

            request.IsProcessed = true;
            request.ProcessedDate = DateTime.Now;
            request.ProcessedByUserId = currentUserId;

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewModifications));
        }

        // ✅ PENDING USER APPROVALS
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingUsers = await _context.Users
                .Where(u => !u.IsApproved)
                .ToListAsync();

            return View(pendingUsers);
        }

        // ✅ APPROVE USER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.IsApproved = true;
            await _context.SaveChangesAsync();
            
            await LouietexERP.Services.ActivityLogger.LogActivityAsync(
                _context,
                $"User Approved: {user.FullName}",
                $"Email: {user.Email} is now approved to log in.",
                "bi-person-check",
                "bg-success-subtle",
                "text-success",
                "UserManagement"
            );

            TempData["Message"] = "User approved successfully!";
            return RedirectToAction(nameof(PendingApprovals));
        }

        // ✅ REJECT USER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // ⛔ PROTECTION: Cannot reject self
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Warning"] = "You cannot reject or delete your own account.";
                return RedirectToAction(nameof(PendingApprovals));
            }

            // ⛔ PROTECTION: Cannot reject the last Super Admin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(SD.Role_SuperAdmin))
            {
                var allSuperAdmins = await _userManager.GetUsersInRoleAsync(SD.Role_SuperAdmin);
                if (allSuperAdmins.Count <= 1)
                {
                    TempData["Warning"] = "Cannot remove the last Super Admin from the system.";
                    return RedirectToAction(nameof(PendingApprovals));
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Warning"] = "User rejected and removed.";
            return RedirectToAction(nameof(PendingApprovals));
        }
    }
}