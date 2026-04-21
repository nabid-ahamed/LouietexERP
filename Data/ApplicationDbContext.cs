using Microsoft.EntityFrameworkCore;
using LouietexERP.Models;

namespace LouietexERP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Inventory> Inventories { get; set; }
    }
}