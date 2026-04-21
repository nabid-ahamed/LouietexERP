using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LouietexERP.Models;

namespace LouietexERP.Data
{
    // ✅ Enables login, users, roles, authentication
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}