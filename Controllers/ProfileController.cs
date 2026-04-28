using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LouietexERP.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ProfileController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // =========================
        // SHOW FORM
        // =========================
        public async Task<IActionResult> RequestChange()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var model = new ProfileRequest
            {
                NewFullName = user.FullName,
                NewEmail = user.Email
            };

            return View(model);
        }

        // =========================
        // HANDLE SUBMIT
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestChange(ProfileRequest model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid profile request model");
                return View(model);
            }

            // ✅ Prevent duplicate pending request
            var existingRequest = await _context.ProfileRequests
                .FirstOrDefaultAsync(r => r.UserId == user.Id && !r.IsProcessed);

            if (existingRequest != null)
            {
                TempData["Warning"] = "You already have a pending request.";
                return RedirectToAction("Index", "Dashboard");
            }

            // ✅ CREATE CLEAN OBJECT (FIXES EMPTY DATA BUG)
            var newRequest = new ProfileRequest
            {
                UserId = user.Id,
                NewFullName = model.NewFullName,
                NewEmail = model.NewEmail,
                RequestDate = DateTime.UtcNow,
                IsProcessed = false,
                Status = ProfileRequestStatus.Pending
            };

            try
            {
                _context.ProfileRequests.Add(newRequest);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving profile request");

                ModelState.AddModelError("", "Failed to submit request.");
                return View(model);
            }

            TempData["Message"] = "Request submitted successfully.";

            return RedirectToAction("Index", "Dashboard");
        }
    }
}