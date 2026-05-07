using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Models;
using LouietexERP.Data;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin)]
    public class UserAccessController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAccessController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ==========================================
        // INDEX - LIST ALL USERS
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "None",
                    ProfilePicturePath = user.ProfilePicturePath,
                    IsApproved = user.IsApproved,
                    IsDisabled = user.IsDisabled
                });
            }

            return View(model);
        }

        // ==========================================
        // DETAILS
        // ==========================================
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                Role = roles.FirstOrDefault() ?? "None",
                ProfilePicturePath = user.ProfilePicturePath,
                IsApproved = user.IsApproved,
                IsDisabled = user.IsDisabled
            };

            return View(model);
        }

        // ==========================================
        // EDIT (GET)
        // ==========================================
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "None";

            // PROTECTION: Admin cannot edit SuperAdmin
            if (User.IsInRole(SD.Role_Admin) && userRole == SD.Role_SuperAdmin)
            {
                TempData["Warning"] = "Admins cannot edit SuperAdmin accounts.";
                return RedirectToAction(nameof(Index));
            }

            var model = new UserEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                Role = userRole,
                IsApproved = user.IsApproved,
                IsDisabled = user.IsDisabled
            };

            // Populate ViewBag for Roles dropdown
            ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            return View(model);
        }

        // ==========================================
        // EDIT (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault() ?? "None";

            // PROTECTION: Admin cannot edit SuperAdmin
            if (User.IsInRole(SD.Role_Admin) && currentRole == SD.Role_SuperAdmin)
            {
                TempData["Warning"] = "Admins cannot edit SuperAdmin accounts.";
                return RedirectToAction(nameof(Index));
            }

            // PROTECTION: Admin cannot ASSIGN SuperAdmin role
            if (User.IsInRole(SD.Role_Admin) && model.Role == SD.Role_SuperAdmin)
            {
                TempData["Warning"] = "Admins cannot assign the SuperAdmin role.";
                return RedirectToAction(nameof(Index));
            }

            // Update Basic Info
            user.FullName = model.FullName;
            user.IsApproved = model.IsApproved;
            
            // Cannot disable self
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId && model.IsDisabled)
            {
                TempData["Warning"] = "You cannot disable your own account.";
            }
            else
            {
                user.IsDisabled = model.IsDisabled;
            }

            if (user.Email != model.Email)
            {
                await _userManager.SetEmailAsync(user, model.Email);
                await _userManager.SetUserNameAsync(user, model.Email);
            }

            if (user.PhoneNumber != model.PhoneNumber)
            {
                await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            }

            // Update Role
            if (currentRole != model.Role && await _roleManager.RoleExistsAsync(model.Role))
            {
                // Prevent removing last SuperAdmin
                if (currentRole == SD.Role_SuperAdmin)
                {
                    var allSuperAdmins = await _userManager.GetUsersInRoleAsync(SD.Role_SuperAdmin);
                    if (allSuperAdmins.Count <= 1)
                    {
                        TempData["Warning"] = "Cannot change role of the last SuperAdmin.";
                        await _userManager.UpdateAsync(user); // save other changes
                        return RedirectToAction(nameof(Index));
                    }
                }

                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            await _userManager.UpdateAsync(user);

            TempData["Message"] = "User updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // TOGGLE STATUS (AJAX/POST)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
                return Json(new { success = false, message = "You cannot disable your own account" });

            var roles = await _userManager.GetRolesAsync(user);
            var isSuperAdmin = roles.Contains(SD.Role_SuperAdmin);

            if (User.IsInRole(SD.Role_Admin) && isSuperAdmin)
                return Json(new { success = false, message = "Admins cannot disable a SuperAdmin" });

            if (isSuperAdmin && !user.IsDisabled)
            {
                var allSuperAdmins = await _userManager.GetUsersInRoleAsync(SD.Role_SuperAdmin);
                var activeSuperAdmins = allSuperAdmins.Count(u => !u.IsDisabled);
                if (activeSuperAdmins <= 1)
                    return Json(new { success = false, message = "Cannot disable the last active SuperAdmin" });
            }

            user.IsDisabled = !user.IsDisabled;
            await _userManager.UpdateAsync(user);

            return Json(new { success = true, isDisabled = user.IsDisabled, message = "Status updated successfully" });
        }

        // ==========================================
        // DELETE (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Warning"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isSuperAdmin = roles.Contains(SD.Role_SuperAdmin);

            if (User.IsInRole(SD.Role_Admin) && isSuperAdmin)
            {
                TempData["Warning"] = "Admins cannot delete a SuperAdmin.";
                return RedirectToAction(nameof(Index));
            }

            if (isSuperAdmin)
            {
                var allSuperAdmins = await _userManager.GetUsersInRoleAsync(SD.Role_SuperAdmin);
                if (allSuperAdmins.Count <= 1)
                {
                    TempData["Warning"] = "Cannot delete the last SuperAdmin.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Optional: If you want to soft delete instead, use:
            // user.IsDisabled = true;
            // await _userManager.UpdateAsync(user);

            await _userManager.DeleteAsync(user);

            TempData["Message"] = "User deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}