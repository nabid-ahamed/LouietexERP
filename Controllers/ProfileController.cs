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

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // ✅ SAVE THEME (SAFE VERSION)
        // =========================
        [HttpPost]
        public async Task<IActionResult> SaveTheme([FromBody] ThemeDto model)
        {
            // 🔒 Validate input
            if (model == null || (model.Theme != "light" && model.Theme != "dark"))
                return BadRequest("Invalid theme");

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            user.PreferredTheme = model.Theme;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500, "Failed to save theme");
            }

            return Ok();
        }

        public class ThemeDto
        {
            public string Theme { get; set; }
        }

        // =========================
        // ✅ SHOW PROFILE CHANGE FORM
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
        // ✅ HANDLE PROFILE CHANGE REQUEST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestChange(ProfileRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            // Attach current user
            request.UserId = _userManager.GetUserId(User);

            // 🚫 Prevent duplicate pending requests
            var existingRequest = await _context.ProfileRequests
                .FirstOrDefaultAsync(r => r.UserId == request.UserId && !r.IsProcessed);

            if (existingRequest != null)
            {
                TempData["Warning"] = "You already have a pending request.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Set metadata
            request.RequestDate = DateTime.Now;
            request.IsProcessed = false;
            request.Status = "Pending";

            // Save to DB
            _context.ProfileRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your request has been sent for approval.";

            return RedirectToAction("Index", "Dashboard");
        }
    }
}