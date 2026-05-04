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

    // Optional: make password simple (for testing)
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

var app = builder.Build();


// ✅ RUN SEEDING (IMPORTANT FIX)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedRolesAndAdmin(services).GetAwaiter().GetResult();
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

// ✅ MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Homecontroller1}/{action=Landing}/{id?}");

// ✅ Identity pages
app.MapRazorPages();

app.Run();


// ✅ SEEDING METHOD (ADMIN + ROLES)
static async Task SeedRolesAndAdmin(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // 🔹 Create roles
    string[] roles = { "SuperAdmin", "NormalUser" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 🔹 Admin credentials
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
    else
    {
        // 🔥 Ensure admin is always usable
        var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
        await userManager.ResetPasswordAsync(adminUser, token, adminPassword);

        adminUser.IsApproved = true;
        adminUser.EmailConfirmed = true;

        await userManager.UpdateAsync(adminUser);

        if (!await userManager.IsInRoleAsync(adminUser, "SuperAdmin"))
        {
            await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
        }
    }
}