using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LouietexERP.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LouietexERP.ViewComponents
{
    public class PendingApprovalsViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PendingApprovalsViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = await _userManager.Users.CountAsync(u => !u.IsApproved);
            return View(count);
        }
    }
}
