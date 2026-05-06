using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LouietexERP.Models;
using System.ComponentModel.DataAnnotations;

namespace LouietexERP.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public IndexModel(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        public string? Username { get; set; }
        public string? ApprovedProfilePicture { get; set; }
        public string? PendingProfilePicture { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Display(Name = "Full Name")]
            public string? FullName { get; set; }

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

            Input = new InputModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            };

            return Page();
        }

        // SAVE PROFILE
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                Username = user.UserName;
                return Page();
            }

            user.FullName = Input.FullName ?? string.Empty;

            if (Input.PhoneNumber != user.PhoneNumber)
                await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Profile updated successfully!";
            return RedirectToPage();
        }

        // UPLOAD PROFILE PICTURE
        public async Task<IActionResult> OnPostUploadPictureAsync(IFormFile profilePicture)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (profilePicture != null && profilePicture.Length > 0)
            {
                // Save to wwwroot/uploads/profiles/
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await profilePicture.CopyToAsync(stream);

                // Save as pending — needs admin approval
                user.PendingProfilePicturePath = $"/uploads/profiles/{fileName}";
                user.ProfilePictureApproved = false;
                await _userManager.UpdateAsync(user);

                StatusMessage = "Profile picture uploaded! Waiting for admin approval.";
            }

            return RedirectToPage();
        }
    }
}