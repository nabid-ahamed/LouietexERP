using Microsoft.EntityFrameworkCore;
using LouietexERP.Models;

namespace LouietexERP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Inventory> Inventories { get; set; }


        public DbSet<Employee> Employees { get; set; }

        public DbSet<Production> Productions { get; set; }

        public DbSet<Order> Orders { get; set; }
    }
}