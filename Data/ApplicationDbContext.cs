using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Models;

namespace LouietexERP.Data
{
    // ✅ Now uses ApplicationUser instead of default IdentityUser
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProfileRequest> ProfileRequests { get; set; }
    }
}