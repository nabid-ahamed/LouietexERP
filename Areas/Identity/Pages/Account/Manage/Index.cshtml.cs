using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LouietexERP.Models;
using System.ComponentModel.DataAnnotations;
using LouietexERP.Data;
using Microsoft.EntityFrameworkCore;

namespace LouietexERP.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          IWebHostEnvironment env,
                          ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _context = context;
        }

        public string? Username { get; set; }
        public string? ApprovedProfilePicture { get; set; }
        public string? PendingProfilePicture { get; set; }
        public bool HasPendingRequest { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Display(Name = "Full Name")]
            public string? FullName { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Username = user.UserName;
            ApprovedProfilePicture = user.ProfilePictureApproved ? user.ProfilePicturePath : null;
            PendingProfilePicture = user.PendingProfilePicturePath;

            var existingRequest = await _context.ProfileRequests
                .FirstOrDefaultAsync(r => r.UserId == user.Id && !r.IsProcessed);
            
            HasPendingRequest = existingRequest != null;

            Input = new InputModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? profilePicture)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                Username = user.UserName;
                return Page();
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            string? newPicturePath = null;

            // Handle file upload
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await profilePicture.CopyToAsync(stream);
                newPicturePath = $"/uploads/profiles/{fileName}";
            }

            if (isAdmin)
            {
                // Direct update for Admins/SuperAdmins
                user.FullName = Input.FullName ?? string.Empty;
                if (!string.IsNullOrEmpty(Input.Email) && Input.Email != user.Email)
                {
                    user.Email = Input.Email;
                    user.UserName = Input.Email;
                    user.NormalizedEmail = Input.Email.ToUpper();
                    user.NormalizedUserName = Input.Email.ToUpper();
                }
                
                if (Input.PhoneNumber != user.PhoneNumber)
                    await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);

                if (newPicturePath != null)
                {
                    user.ProfilePicturePath = newPicturePath;
                    user.ProfilePictureApproved = true;
                    user.PendingProfilePicturePath = null;
                }

                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                StatusMessage = "Profile updated successfully!";
                return RedirectToPage();
            }
            else
            {
                // Create a ProfileRequest for normal users
                var existingRequest = await _context.ProfileRequests
                    .FirstOrDefaultAsync(r => r.UserId == user.Id && !r.IsProcessed);

                if (existingRequest != null)
                {
                    StatusMessage = "Error: You already have a pending profile change request.";
                    return RedirectToPage();
                }

                var newRequest = new ProfileRequest
                {
                    UserId = user.Id,
                    NewFullName = Input.FullName ?? string.Empty,
                    NewEmail = Input.Email ?? user.Email ?? string.Empty,
                    NewPhoneNumber = Input.PhoneNumber,
                    NewProfilePicturePath = newPicturePath,
                    RequestDate = DateTime.UtcNow,
                    IsProcessed = false,
                    Status = ProfileRequestStatus.Pending
                };

                _context.ProfileRequests.Add(newRequest);
                await _context.SaveChangesAsync();

                StatusMessage = "Profile update request submitted and is waiting for admin approval.";
                return RedirectToPage();
            }
        }
    }
}