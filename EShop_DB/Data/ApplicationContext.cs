using EShop_DB.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop_DB.Data;

public class ApplicationContext : DbContext
{ 
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated(); 
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}