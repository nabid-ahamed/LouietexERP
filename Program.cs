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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Identity with roles + UI
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

var app = builder.Build();


// ✅ COMBINED ROLE + ADMIN SEEDING
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // 1️⃣ Create roles
    string[] roles = { "SuperAdmin", "NormalUser" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 2️⃣ Create Super Admin
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
            throw new Exception("Admin creation failed: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(admin, "SuperAdmin");
    }
}


// ✅ Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Identity middleware
app.UseAuthentication();
app.UseAuthorization();

// ✅ MVC routing (IMPORTANT for your controllers)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// ✅ Identity pages (Login/Register)
app.MapRazorPages();

app.Run();