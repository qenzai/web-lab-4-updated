using Microsoft.EntityFrameworkCore;
using MyWebsiteBackend.Models;

namespace MyWebsiteBackend.Data
{

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product>? Products { get; set; }
        public DbSet<Category>? Categories { get; set; }
    }

    
}
