using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Data;
using LouietexERP.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ✅ Configure EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;

    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
});

var app = builder.Build();


// ✅ SEED ROLES + ADMIN
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedRolesAndAdminAsync(services);
}


// ✅ Configure Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();


// ✅ MVC ROUTING
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Homecontroller1}/{action=Landing}/{id?}");


// ✅ Identity Razor Pages
app.MapRazorPages();

app.Run();


// =======================================================
// ✅ ROLE + ADMIN SEEDING
// =======================================================

static async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // ✅ SYSTEM ROLES
    string[] roles =
    {
        SD.Role_SuperAdmin,
        SD.Role_Admin,
        SD.Role_HR,
        SD.Role_Merchandiser,
        SD.Role_ProductionManager,
        SD.Role_QC,
        SD.Role_User
    };

    // ✅ CREATE ROLES
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ✅ ADMIN ACCOUNT
    string adminEmail = "admin@louietex.com";
    string adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Super Admin",
            IsApproved = true,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);

        if (!result.Succeeded)
        {
            throw new Exception(
                "Admin creation failed: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ✅ Assign SuperAdmin Role
        await userManager.AddToRoleAsync(admin, SD.Role_SuperAdmin);
    }
    else
    {
        // ✅ Ensure admin stays valid
        var token =
            await userManager.GeneratePasswordResetTokenAsync(adminUser);

        await userManager.ResetPasswordAsync(
            adminUser,
            token,
            adminPassword);

        adminUser.IsApproved = true;
        adminUser.EmailConfirmed = true;

        await userManager.UpdateAsync(adminUser);

        // ✅ Ensure role exists
        if (!await userManager.IsInRoleAsync(adminUser, SD.Role_SuperAdmin))
        {
            await userManager.AddToRoleAsync(adminUser, SD.Role_SuperAdmin);
        }
    }
}