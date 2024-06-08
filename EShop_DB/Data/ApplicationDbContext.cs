using EShop_BL.Models.MainModels;
using EShop_BL.Models.SecondaryModels;
using Microsoft.EntityFrameworkCore;

namespace EShop_DB.Data;

public class ApplicationDbContext : DbContext
{ 
    public DbSet<Seller> Sellers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OrderEvent> OrderEvents { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<DeliveryAddress> DeliveryAddresses { get; set; } = null!;
    public DbSet<Recipient> Recipients { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        #region Entity constraints
        
        modelBuilder.Entity<Product>()
            .Property(p => p.PricePerUnit)
            .HasColumnType("decimal(18, 2)");
        
        modelBuilder.Entity<Role>()
            .Property(r => r.RoleTag)
            .HasConversion<string>();
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.RoleTag)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();
        
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.AnonymousToken)
            .IsUnique();
        
        modelBuilder.Entity<Recipient>()
            .HasIndex(r => r.PhoneNumber)
            .IsUnique();
        
        modelBuilder.Entity<OrderEvent>()
            .Property(oe => oe.Stage)
            .HasConversion<string>();

        #endregion
    }
}