using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LouietexERP.Models;

[Authorize(Roles = "SuperAdmin")]
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

    // ✅ Updated Index (User + Role)
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();

        var userRoles = new List<(ApplicationUser User, string Role)>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add((user, roles.FirstOrDefault() ?? "None"));
        }

        return View(userRoles);
    }

    // ✅ DEBUG Change Role
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Content("❌ User NOT found");

        var roleExists = await _roleManager.RoleExistsAsync(newRole);
        if (!roleExists)
            return Content("❌ Role NOT found in database");

        var currentRoles = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await _userManager.AddToRoleAsync(user, newRole);

        if (!result.Succeeded)
            return Content("❌ Failed to assign role");

        return RedirectToAction("Index");
    }
}